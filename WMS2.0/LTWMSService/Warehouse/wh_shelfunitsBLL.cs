using LTWMSEFModel;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.Basic;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_shelfunitsBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_shelfunits>
    {
        sys_control_dicBLL bll_sys_control_dic;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        wh_trayBLL bll_tray;
        public wh_shelfunitsBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
            bll_sys_control_dic = new sys_control_dicBLL(dbContext);
            bll_tray = new wh_trayBLL(dbContext);
            // bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbContext);
        }
        /// <summary>
        /// 根据批次号锁定对应的库位
        /// </summary>
        /// <param name="lotNumbers"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue SpecialLockOutByLotNumber(string lotNumbers, bool islock)
        {
            lotNumbers = (lotNumbers ?? "").Trim();
            IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                              join b in dbcontext.wh_tray
                                              on a.guid equals b.shelfunits_guid
                                              where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.lot_number == lotNumbers)
                                              select a;
            var lstM = query.AsNoTracking().ToList();
            if (lstM != null && lstM.Count > 0)
            {
                foreach (var item in lstM)
                {
                    if (islock)
                    {
                        item.special_lock_type = SpecialLockTypeEnum.StockOutLock;
                    }
                    else
                    {
                        item.special_lock_type = SpecialLockTypeEnum.Normal;
                    }
                    item.OldRowVersion = item.rowversion;
                    Update(item);
                }
                return SimpleBackValue.True;
            }
            return SimpleBackValue.False;
        }
        /// <summary>
        /// 获取货架未分配库位
        /// 单边多排 分配入库库位(预留几个库位，用于移库操作！)
        /// </summary>
        /// <returns></returns>
        public wh_shelfunits GetStoreShelfUnitsAndLock(List<wh_shelves> lstShelves)
        {
            if (lstShelves == null || lstShelves.Count == 0)
            {
                getcount = 0;
                return null;
            }
            List<Guid> lstShelvesGuid = ComBLLService.GetBaseBaseGuidList(lstShelves);
            /**********预留大概10个库位不能入库，方便出库时移库***********/
            IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                              where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                              && a.depth == 0
                                              && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                              && a.special_lock_type == SpecialLockTypeEnum.Normal
                                              select a;
            int _count_last = query.AsNoTracking().Count();
            if (_count_last <= 2)
            {//针对某个入库站台100/200绑定的货架>> 靠近堆垛机两侧 纵深1 总计预留10个库位，用于移库操作
                getcount = 0;
                return null;
            }
            /*********************/
            //一般一个站台对应的所有货架出入库原则基本一致，暂时取第一个，后期具体情况再分析
            StockDistributeEnum stockDistribute = lstShelves[0].stock_distribute;
            //查找仓库的存放策略
            Guid _warehouse_guid = lstShelves[0].warehouse_guid;
            var _wareHouseObj = dbcontext.wh_warehouse.AsNoTracking().FirstOrDefault(w => w.guid == _warehouse_guid);

            // 根据库位入库分配规则 ，查找对应未分配的库位 
            wh_shelfunits _shelfUnitsObj = null;
            List<int> depthList = new List<int>();
            foreach (var item in lstShelves)
            {
                if (!depthList.Contains(item.depth))
                {//不包含纵深，添加
                    depthList.Add(item.depth);
                }
            }
            //降序排 2 -> 1 -> 0
            depthList = depthList.OrderByDescending(w => w).ToList();
            if (_wareHouseObj.distribute_way == DistributeWayEnum.SidesToMiddle)
            {//将外侧的货架存满再存内侧（减少移库）  
                for (int i = 0; i < depthList.Count; i++)
                {
                    int _curr_depth = depthList[i];
                    _shelfUnitsObj = GetShelfUnit_CanIn(_curr_depth, stockDistribute, lstShelvesGuid);
                    if (_shelfUnitsObj != null && _shelfUnitsObj.guid != Guid.Empty)
                    {//查找到库位，立即退出循环
                        break;
                    }
                }
            }
            else
            {//外侧内侧同时存（频繁移库）
                if (stockDistribute == StockDistributeEnum.Random)
                {//随机分配
                    for (int i = 0; i < depthList.Count; i++)
                    {
                        int _curr_depth = depthList[i];
                        _shelfUnitsObj = GetShelfUnit_CanIn(_curr_depth, stockDistribute, lstShelvesGuid);
                        if (_shelfUnitsObj != null && _shelfUnitsObj.guid != Guid.Empty)
                        {//查找到库位，立即退出循环
                            break;
                        }
                    }
                }
                else
                {
                    _shelfUnitsObj = GetShelfUnit_CanIn(-1, stockDistribute, lstShelvesGuid);
                }
            }
            //单边多排 分配入库库位(预留几个库位，用于移库操作！)  
            if (_shelfUnitsObj != null && Guid.Empty != _shelfUnitsObj.guid)
            {//如果库位不为空，则只锁定库位  
                if (CheckExistBlockAndLock(_shelfUnitsObj))
                {//库位存在干扰返回null，等待分配库位线程再次进入
                    getcount = 0;
                    return null;
                }
                //
                _shelfUnitsObj.locktype = ShelfLockType.SysLock;
                _shelfUnitsObj.cellstate = ShelfCellState.TrayIn;//库位状态：入库中
                _shelfUnitsObj.updatedate = DateTime.Now;
                var rv = Update(_shelfUnitsObj);
                if (rv == LTWMSEFModel.SimpleBackValue.True)
                {
                    getcount = 0;
                    return _shelfUnitsObj;
                }
                else if (rv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                {
                    // return GetStoreShelfUnitsAndLock(lstShelves);
                    System.Threading.Thread.Sleep(20);//休息20毫秒，等待其它线程将数据处理完
                    return GetStoreShelfUnitsAndLock2(lstShelves);
                }
            }
            getcount = 0;
            return null;
        }
        //获取入库库位次数，超过3次自动返回null
        int getcount = 0;
        public wh_shelfunits GetStoreShelfUnitsAndLock2(List<wh_shelves> lstShelves)
        {
            if (getcount >= 3)
            {
                getcount = 0;
                return null;
            }
            getcount++;
            return GetStoreShelfUnitsAndLock(lstShelves);
        }
        /// <summary>
        /// 判断入库是否存在阻挡
        /// </summary>
        /// <param name="_shelfUnitsObj"></param>
        /// <returns></returns>
        public bool CheckExistBlock(wh_shelfunits _shelfUnitsObj)
        {
            //判断分配的库位最外围是否有物料？ 万一有物料锁定当前库位？？？内侧出完库则自动解锁为正常。。。。
            int _exist_count = GetCount(w => w.same_side_mark == _shelfUnitsObj.same_side_mark
                && w.columns == _shelfUnitsObj.columns && w.rows == _shelfUnitsObj.rows
                && w.depth < _shelfUnitsObj.depth &&
               (w.cellstate != ShelfCellState.CanIn || w.locktype != ShelfLockType.Normal));
            if (_exist_count > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        ///检查入库库位是否有阻挡、外侧纵深是否有出库任务
        ///如果有则禁用当前入库库位 返回true  ，false 不存在阻挡
        /// </summary>
        /// <param name="_shelfUnitsObj"></param>
        /// <returns></returns>
        public bool CheckExistBlockAndLock(wh_shelfunits _shelfUnitsObj)
        {
            //判断分配的库位最外围是否有物料？ 万一有物料锁定当前库位？？？内侧出完库则自动解锁为正常。。。。
            int _exist_count = GetCount(w => w.same_side_mark == _shelfUnitsObj.same_side_mark
                && w.columns == _shelfUnitsObj.columns && w.rows == _shelfUnitsObj.rows && w.depth < _shelfUnitsObj.depth &&
               (w.cellstate != ShelfCellState.CanIn || w.locktype != ShelfLockType.Normal));
            //判断入库库位的内侧是否有出库任务、？？如果有出库任务则不分配该入库库位,重新分配入库库位
            int _existout_count = GetCount(w => w.same_side_mark == _shelfUnitsObj.same_side_mark
            && w.columns == _shelfUnitsObj.columns && w.rows == _shelfUnitsObj.rows && w.depth > _shelfUnitsObj.depth &&
           (w.cellstate == ShelfCellState.TrayOut || w.cellstate == ShelfCellState.WaitOut));
            if (_exist_count > 0 || _existout_count > 0)
            {
                //1存在阻碍，则修改当前库位为不可用，继续查找下一个库位？？？？或者等待库位分配线程下一次分配库位。。。  
                //2存在待出库、或正在出库，不分配入库库位 
                _shelfUnitsObj.locktype = ShelfLockType.SysLock;//出库完成 解锁syslock 的canin 库位
                //  _shelfUnitsObj.cellstate = ShelfCellState.TrayIn;//库位状态：入库中
                _shelfUnitsObj.updatedate = DateTime.Now;
                //不管修改成功没有成功，直接返回null 等待库位分配线程下一次分配库位。。
                Update(_shelfUnitsObj);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 通过纵深、分配方式、对应的货架id 分配入库库位
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="stockDistribute"></param>
        /// <param name="lstShelvesGuid"></param>
        /// <returns></returns>
        public wh_shelfunits GetShelfUnit_CanIn(int depth, StockDistributeEnum stockDistribute, List<Guid> lstShelvesGuid)
        {
            wh_shelfunits _shelfUnitsObj = null;
            switch (stockDistribute)
            {
                case StockDistributeEnum.LeftToRigth:
                    IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                      where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                      && (depth == -1 || a.depth == depth)
                                                      && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                      && a.special_lock_type == SpecialLockTypeEnum.Normal
                                                      orderby a.columns ascending, a.rows ascending, a.depth descending
                                                      select a;
                    _shelfUnitsObj = query.AsNoTracking().FirstOrDefault();
                    break;
                case StockDistributeEnum.RightToLeft:
                    IQueryable<wh_shelfunits> query2 = from a in dbcontext.wh_shelfunits
                                                       where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                       && (depth == -1 || a.depth == depth)
                                                       && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                         && a.special_lock_type == SpecialLockTypeEnum.Normal
                                                       orderby a.columns descending, a.rows ascending, a.depth descending
                                                       select a;
                    _shelfUnitsObj = query2.AsNoTracking().FirstOrDefault();
                    break;
                case StockDistributeEnum.Random:
                    //单边多排，取随机值可能会出现分配库位出问题，假如纵深1有货，随机分配可能会分配纵深2 ，这样就会导致入不了库
                    //....... 随机一般是测试使用，正常不会使用
                    //随机分配 ，只能外侧全部随机放满 然后再随机分配内侧。
                    IQueryable<wh_shelfunits> query3 = from a in dbcontext.wh_shelfunits
                                                       where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                        && a.depth == depth
                                                       && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                         && a.special_lock_type == SpecialLockTypeEnum.Normal
                                                       orderby a.rows ascending, a.columns ascending, a.depth descending
                                                       select a;
                    var ListData = query3.AsNoTracking().ToList();
                    int rand = new Random().Next(0, ListData.Count);
                    _shelfUnitsObj = ListData[rand];
                    break;
                case StockDistributeEnum.LowerToUpper:
                default:
                    //0=最下层分配完再往上层分配（按层）
                    IQueryable<wh_shelfunits> query4 = from a in dbcontext.wh_shelfunits
                                                       where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                        && (depth == -1 || a.depth == depth)
                                                       && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                         && a.special_lock_type == SpecialLockTypeEnum.Normal
                                                       orderby a.rows ascending, a.columns ascending, a.depth descending
                                                       select a;
                    _shelfUnitsObj = query4.AsNoTracking().FirstOrDefault();
                    break;
            }
            return _shelfUnitsObj;
        }
        /// <summary>
        /// 出库取消后对库位的操作
        /// </summary>
        public wh_shelfunits StockOutCanceledHandler(hdw_stacker_taskqueue taskInfo)
        {
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbcontext);
            wh_shelfunits _shelfCell = GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid && w.warehouse_guid == taskInfo.warehouse_guid);
            string _new_task_guid = "";
            if (taskInfo.regenerate_task_queue == true)
            {//重新生成出库任务
                _shelfCell.cellstate = ShelfCellState.WaitOut;
                _new_task_guid = bll_hdw_stacker_taskqueue.ReGenerateTask(taskInfo);
                //taskInfo.new_task_queue_guid
            }
            else
            {//不重新生成出库任务
                _shelfCell.cellstate = ShelfCellState.Stored;
            }
            _shelfCell.updatedate = DateTime.Now;
            var rtvupdate = Update(_shelfCell);
            if (rtvupdate == SimpleBackValue.True)
            {
                return _shelfCell;
            }
            return null;
        }
        /// <summary>
        /// 移库取消后对库位的操作
        /// </summary>
        public wh_shelfunits StockMoveOutCanceledHandler(hdw_stacker_taskqueue taskInfo)
        {
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbcontext);
            wh_shelfunits _shelfCell = GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid && w.warehouse_guid == taskInfo.warehouse_guid);
            string _new_task_guid = "";
            if (taskInfo.regenerate_task_queue == true)
            {//重新生成移库任务
                _shelfCell.cellstate = ShelfCellState.WaitOut;
                _new_task_guid = bll_hdw_stacker_taskqueue.ReGenerateTask(taskInfo);
                //taskInfo.new_task_queue_guid
            }
            else
            {//不重新生成移库任务
                _shelfCell.cellstate = ShelfCellState.Stored;
            }
            _shelfCell.updatedate = DateTime.Now;
            if (Update(_shelfCell) == SimpleBackValue.True)
            {
                return _shelfCell;
            }
            return null;
        }

        public List<LTWMSEFModel.Warehouse.wh_shelfunits> GetShelfUnitOutToTaskByOrderCluster(string order_cluster, bool isequal)
        {
            if (string.IsNullOrWhiteSpace(order_cluster))
            {
                return null;
            }
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query;
            if (isequal)
            {//如果包含订单-簇-编号 则用相等匹配
                query = from a in dbcontext.wh_shelfunits
                        join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                        where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.x_barcode == order_cluster && w.lot_number != null && w.lot_number != "") && a.state == LTWMSEFModel.EntityStatus.Normal
                        && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                        orderby a.columns descending, a.rows ascending
                        select a;
            }
            else
            {//如果不包含编号 只包含 订单- 或 订单-簇-  则用模糊匹配
                query = from a in dbcontext.wh_shelfunits
                        join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                        where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.x_barcode.Contains(order_cluster) && w.lot_number != null && w.lot_number != "") && a.state == LTWMSEFModel.EntityStatus.Normal
                        && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                        orderby a.columns descending, a.rows ascending
                        select a;
            }
            return query.AsNoTracking().ToList();
        }
        /// <summary>
        /// 通过物料 TrayMatter_guid 将对应的库位出库(页面批量操作出库，不考虑电池条码 按簇 编号排序。。。。)
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<LTWMSEFModel.Warehouse.wh_shelfunits> GetShelfUnitOutToTaskByGuids(List<Guid> guids)
        {
            if (guids == null || guids.Count == 0)
            {
                return null;
            }
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && guids.Contains(w.guid)) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     && a.special_lock_type == SpecialLockTypeEnum.Normal
                                                                     orderby a.columns descending, a.rows ascending
                                                                     select a;

            return query.AsNoTracking().ToList();
        }
        /// <summary>
        /// 获取储存电池的库位数
        /// </summary>
        /// <returns></returns>
        public int GetShelfUnitCountOfBatter(Guid warehouseguid)
        {
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where a.warehouse_guid == warehouseguid && dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.lot_number != null && w.lot_number != "") && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     //  orderby a.columns descending, a.rows ascending
                                                                     select a;
            return query.AsNoTracking().Count();
        }  /// <summary>
           /// 获取储存其它物料的库位数
           /// </summary>
           /// <returns></returns>
        public int GetShelfUnitCountOfOther(Guid warehouseguid)
        {
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where a.warehouse_guid == warehouseguid && dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && (w.lot_number == null || w.lot_number == "")) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     //   orderby a.columns descending, a.rows ascending
                                                                     select a;
            return query.AsNoTracking().Count();
        }
        /// <summary>
        /// 获取空托盘的库位数量
        /// </summary>
        /// <returns></returns>
        public int GetShelfUnitCountOfEmpty(Guid warehouseguid)
        {
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where a.warehouse_guid == warehouseguid && dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid) == false && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     //  orderby a.columns descending, a.rows ascending
                                                                     select a;
            return query.AsNoTracking().Count();
        }
        /// <summary>
        /// 获取存储的物料类型
        /// </summary>
        /// <param name="shelfUnit"></param>
        /// <returns></returns>
        public MatterTypeEnum getStoredMatterType(wh_shelfunits shelfUnit)
        {
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where a.guid == shelfUnit.guid &&
                                                                     dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid) == false && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     select a;
            if (query.AsNoTracking().Count() > 0)
            {//空托盘
                return MatterTypeEnum.Empty;
            }
            //query = from a in dbcontext.wh_shelfunits
            //        join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
            //        where a.guid == shelfUnit.guid &&
            //        dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.batch != null && w.batch != "") && a.state == LTWMSEFModel.EntityStatus.Normal
            //        && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
            //        select a;
            //if (query.AsNoTracking().Count() > 0)
            //{//电池
            //    return MatterTypeEnum.Battery;
            //}
            return MatterTypeEnum.Matter;
        }

        /// <summary>
        /// 获取所有干涉的库位列表，按纵深升序排列(纵深从0开始)
        /// </summary>
        /// <returns></returns>
        public List<wh_shelfunits> getAllBlocksShelfUnitOrderByDepth(wh_shelfunits _shelfU_inout)
        {
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from w in dbcontext.wh_shelfunits
                                                                     where w.same_side_mark == _shelfU_inout.same_side_mark
                     && w.columns == _shelfU_inout.columns && w.rows == _shelfU_inout.rows && w.depth < _shelfU_inout.depth &&
                    (w.cellstate != ShelfCellState.CanIn || w.locktype != ShelfLockType.Normal)
                                                                     select w;
            return query.AsNoTracking().OrderBy(o => o.depth).ToList();
        }
        /// <summary>
        /// 不保存数据
        /// 获取待移动的目标库位
        /// </summary>
        /// <param name="waiteMoveShelfUnit"></param>
        /// <returns></returns>
        public wh_shelfunits GetNearbyMoveShelfUnitAndLock(wh_shelfunits waiteMoveShelfUnit, List<wh_shelves> whshelvesList)
        {
            List<Guid> lstShelvesGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);

            //IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
            //                                  where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
            //                               && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
            //                                  orderby a.columns ascending, a.rows ascending, a.depth descending
            //                                  select a;
            //查找待移动库位附近的可入库库位，通过比较列差值的绝对值和层差值的绝对值  进行升序排列。。。。。。
            //排除待移动库位同一侧的同一纵深库位（如果是3伸或以上必须要排除）
            IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                              where lstShelvesGuid.Contains(a.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                           && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                           && a.special_lock_type == SpecialLockTypeEnum.Normal
                                           && !(a.rows == waiteMoveShelfUnit.rows && a.columns == waiteMoveShelfUnit.columns
                                           && a.same_side_mark == waiteMoveShelfUnit.same_side_mark)
                                              orderby a.depth descending, (Math.Abs(a.columns - waiteMoveShelfUnit.columns)
                                            + Math.Abs(a.rows - waiteMoveShelfUnit.rows)) ascending //--优先存最外侧
                                              //orderby Math.Abs(a.columns - waiteMoveShelfUnit.columns) ascending
                                              //, Math.Abs(a.rows - waiteMoveShelfUnit.rows) ascending, a.depth descending --距离最短
                                              select a;
            //<= || => 查找最近移动库位 可以是另一边
            var dest_shelfUList = query.AsNoTracking().Take(5).ToList();
            wh_shelfunits dest_shelfU = null;
            if (dest_shelfUList != null && dest_shelfUList.Count > 0)
            {
                foreach (var item in dest_shelfUList)
                {
                    //判断目标库位纵深移库是否有阻挡，有阻挡则不用
                    if (CheckExistBlockAndLock(item))
                    {
                        continue;
                    }
                    dest_shelfU = item;
                    break;
                }
            }
            return dest_shelfU;
        }
        /// <summary>
        /// 指定库位进行锁定
        /// </summary>
        /// <param name="rack"></param>
        /// <param name="colum"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public wh_shelfunits CheckAndLockPosByRackColumRow(int rack, int colum, int row, string traybarcode)
        {
            //释放原来指定入库的库位
            var trayModel = bll_tray.GetFirstDefault(w => w.traybarcode == traybarcode);
            //判断指定的库位是否是原来指定的库位
            wh_shelfunits OldUnitM = null;
            if (trayModel != null && trayModel.guid != Guid.Empty && trayModel.dispatch_shelfunits_guid != null)
            {//存在对应的托盘记录 、判断是否指定过库位
                OldUnitM = GetFirstDefault(w => w.guid == trayModel.dispatch_shelfunits_guid);
                if (OldUnitM != null && OldUnitM.guid != Guid.Empty)
                //  OldUnitM.special_lock_type == SpecialLockTypeEnum.DispatchLock)
                {
                    /*  OldUnitM.special_lock_type = SpecialLockTypeEnum.Normal;
                      var rtv2 = Update(OldUnitM);
                      if (rtv2 != SimpleBackValue.True)
                      { //修改原指定库位失败。。。
                          return null;
                      }*/
                    //指定的库位是否等于传过来的库位
                    if (OldUnitM.rack == rack && OldUnitM.columns == colum && OldUnitM.rows == row)
                    {
                        return OldUnitM;
                    }
                }
            }
            //对应托盘没有指定库位,或指定另外的库位>>>>  
            var model = GetFirstDefault(w => w.rack == rack && w.columns == colum && w.rows == row &&
            w.cellstate == ShelfCellState.CanIn
            && w.locktype == ShelfLockType.Normal && w.special_lock_type == SpecialLockTypeEnum.Normal
            );
            if (model != null && model.guid != Guid.Empty)
            {
                model.special_lock_type = SpecialLockTypeEnum.DispatchLock;
                if (Update(model) == SimpleBackValue.True)
                {
                    if (OldUnitM != null && OldUnitM.guid != Guid.Empty)
                    {//释放库位
                        OldUnitM.special_lock_type = SpecialLockTypeEnum.Normal;
                        var rtv2 = Update(OldUnitM);
                        if (rtv2 != SimpleBackValue.True)
                        { //修改原指定库位失败。。。
                            return null;
                        }
                    }
                    return model;
                }
            }
            return null;
        }
    }
}
