using LTWMSEFModel;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.Basic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Hardware
{
    public class hdw_stacker_taskqueueBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>
    {
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_shelfunits;
        // LTWMSService.Hardware.hdw_task_bill_inoutBLL bll_task_bill_inout;
        LTWMSService.Warehouse.wh_trayBLL bll_wh_tray;
        // LTWMSService.Hardware.hdw_agv_task_mainBLL bll_hdw_agv_task_main;
        //  LTWMSService.Hardware.hdw_agv_taskqueueBLL bll_hdw_agv_taskqueue;
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        public LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        public hdw_stacker_taskqueueBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
            bll_shelfunits = new Warehouse.wh_shelfunitsBLL(dbContext);
            //  bll_task_bill_inout = new hdw_task_bill_inoutBLL(dbContext);
            bll_wh_tray = new Warehouse.wh_trayBLL(dbContext);
            //  bll_hdw_agv_task_main = new hdw_agv_task_mainBLL(dbcontext);
            //  bll_hdw_agv_taskqueue = new hdw_agv_taskqueueBLL(dbcontext);
            bll_sys_table_id = new Basic.sys_table_idBLL(dbContext);
            bll_sys_control_dic = new Basic.sys_control_dicBLL(dbContext);
            bll_hdw_stacker_taskqueue_his = new hdw_stacker_taskqueue_hisBLL(dbContext);
        }
        /// <summary>
        /// 判断系统是否存在运行中的入库任务
        /// </summary>
        /// <returns></returns>
        public bool ExistsRunningTaskIn()
        {
            IQueryable<hdw_stacker_taskqueue> query = from aa in dbcontext.hdw_stacker_taskqueue
                                                      where
                (from a in dbcontext.hdw_stacker_taskqueue
                 where a.tasktype == WcsTaskType.StockIn && a.state == LTWMSEFModel.EntityStatus.Normal
                    && a.taskstatus == WcsTaskStatus.WaiteDispatchStockCell
                 select a.guid
                                               ).Union(from a in dbcontext.hdw_stacker_taskqueue
                                                       join
                                                       b in dbcontext.wh_shelfunits
                                                       on a.dest_shelfunits_guid equals b.guid
                                                       where a.tasktype == WcsTaskType.StockIn &&
                                                        a.state == LTWMSEFModel.EntityStatus.Normal
                                                        && (a.taskstatus == WcsTaskStatus.Holding || //a.taskstatus == WcsTaskStatus.Exception||
                                                        a.taskstatus == WcsTaskStatus.ExceptionHandling || a.taskstatus == WcsTaskStatus.ExceptionSended
                                                        || a.taskstatus == WcsTaskStatus.Execute || a.taskstatus == WcsTaskStatus.IsSend
                                                        || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.WriteError ||
                                                        a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended
                                                        || a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended)
                                                        && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                       select a.guid
                                               ).Contains(aa.guid)
                                                      select aa;
            return query.AsNoTracking().Count() > 0 ? true : false;
        }
        /// <summary>
        /// 获取运行中的出入库任务
        /// </summary>
        /// <returns></returns>
        public List<hdw_stacker_taskqueue> GetRunningTaskList(Guid warehouseguid)
        { /*mysql 可用，sqlserver 报错
            IQueryable<hdw_stacker_taskqueue> query = (from a in dbcontext.hdw_stacker_taskqueue
                                                       where a.warehouse_guid == warehouseguid && a.tasktype == WcsTaskType.StockIn && a.state == LTWMSEFModel.EntityStatus.Normal
             && a.taskstatus == WcsTaskStatus.WaiteDispatchStockCell
                                                       select a
                                                ).Union(from a in dbcontext.hdw_stacker_taskqueue
                                                        join
                                                        b in dbcontext.wh_shelfunits
                                                        on a.dest_shelfunits_guid equals b.guid
                                                        where a.warehouse_guid == warehouseguid && a.tasktype == WcsTaskType.StockIn &&
                                                         a.state == LTWMSEFModel.EntityStatus.Normal
                                                         && (a.taskstatus == WcsTaskStatus.Holding || a.taskstatus == WcsTaskStatus.Exception
                                                         || a.taskstatus == WcsTaskStatus.Execute || a.taskstatus == WcsTaskStatus.IsSend
                                                         || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.WriteError ||
                                                         a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended
                                                         || a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended) && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                        select a
                                                ).Union(from a in dbcontext.hdw_stacker_taskqueue
                                                        join
                                               b in dbcontext.wh_shelfunits
                                               on a.src_shelfunits_guid equals b.guid
                                                        where a.warehouse_guid == warehouseguid && (a.tasktype == WcsTaskType.StockOut || a.tasktype == WcsTaskType.MoveTo) &&
                                                         a.state == LTWMSEFModel.EntityStatus.Normal &&
                                                         (a.taskstatus == WcsTaskStatus.Exception
                                                 || a.taskstatus == WcsTaskStatus.Execute || a.taskstatus == WcsTaskStatus.IsSend
                                                 || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.WriteError ||
                                                 a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended
                                                 || a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended) && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                        select a
                                               );
            return query.AsNoTracking().ToList(); */
            //mysql 和sqlserver 都可用
            IQueryable<hdw_stacker_taskqueue> query = from aa in dbcontext.hdw_stacker_taskqueue
                                                      where (from a in dbcontext.hdw_stacker_taskqueue
                                                             where a.warehouse_guid == warehouseguid && a.tasktype == WcsTaskType.StockIn && a.state == LTWMSEFModel.EntityStatus.Normal
                                    && a.taskstatus == WcsTaskStatus.WaiteDispatchStockCell
                                                             select a.guid
                                                                           ).Union(from a in dbcontext.hdw_stacker_taskqueue
                                                                                   join
                                                                                   b in dbcontext.wh_shelfunits
                                                                                   on a.dest_shelfunits_guid equals b.guid
                                                                                   where a.warehouse_guid == warehouseguid && a.tasktype == WcsTaskType.StockIn &&
                                                                                    a.state == LTWMSEFModel.EntityStatus.Normal
                                                                                    && (a.taskstatus == WcsTaskStatus.Holding || //a.taskstatus == WcsTaskStatus.Exception||
                                                                                    a.taskstatus == WcsTaskStatus.ExceptionHandling || a.taskstatus == WcsTaskStatus.ExceptionSended
                                                                                    || a.taskstatus == WcsTaskStatus.Execute || a.taskstatus == WcsTaskStatus.IsSend
                                                                                    || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.WriteError ||
                                                                                    a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended
                                                                                    || a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended) && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                                   select a.guid
                                                                           ).Union(from a in dbcontext.hdw_stacker_taskqueue
                                                                                   join
                                                                          b in dbcontext.wh_shelfunits
                                                                          on a.src_shelfunits_guid equals b.guid
                                                                                   where a.warehouse_guid == warehouseguid && (a.tasktype == WcsTaskType.StockOut || a.tasktype == WcsTaskType.MoveTo) &&
                                                                                    a.state == LTWMSEFModel.EntityStatus.Normal &&
                                                                                    (//a.taskstatus == WcsTaskStatus.Exception ||
                                                                                    a.taskstatus == WcsTaskStatus.ExceptionHandling ||
                                                                                    a.taskstatus == WcsTaskStatus.ExceptionSended
                                                                            || a.taskstatus == WcsTaskStatus.Execute || a.taskstatus == WcsTaskStatus.IsSend
                                                                            || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.WriteError ||
                                                                            a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended
                                                                            || a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended) && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                                   select a.guid
                                                                          ).Contains(aa.guid)
                                                      select aa;
            return query.AsNoTracking().ToList();
        }
        /// <summary>
        ///  重新生成出库任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns>新任务guid</returns>
        public string ReGenerateTask(hdw_stacker_taskqueue taskInfo)
        {
            hdw_stacker_taskqueue newtask = new hdw_stacker_taskqueue();
            newtask.id = bll_sys_table_id.GetId(Basic.sys_table_idBLL.TableIdType.hdw_stacker_taskqueue);
            newtask.createdate = DateTime.Now;
            newtask.guid = Guid.NewGuid();
            newtask.createuser = taskInfo.createuser;
            newtask.dest_col = taskInfo.dest_col;
            newtask.dest_rack = taskInfo.dest_rack;
            newtask.dest_row = taskInfo.dest_row;
            newtask.dest_shelfunits_guid = taskInfo.dest_shelfunits_guid;
            newtask.dest_shelfunits_pos = taskInfo.dest_shelfunits_pos;
            newtask.dest_station = taskInfo.dest_station;
            newtask.is_emptypallet = taskInfo.is_emptypallet;
            newtask.memo = taskInfo.memo;
            newtask.order = taskInfo.order;
            newtask.shelves_guid = taskInfo.shelves_guid;
            newtask.src_col = taskInfo.src_col;
            newtask.src_rack = taskInfo.src_rack;
            newtask.src_row = taskInfo.src_row;
            newtask.src_shelfunits_guid = taskInfo.src_shelfunits_guid;
            newtask.src_shelfunits_pos = taskInfo.src_shelfunits_pos;
            newtask.src_station = taskInfo.src_station;
            newtask.state = LTWMSEFModel.EntityStatus.Normal;
            newtask.taskstatus = WcsTaskStatus.Holding;
            newtask.tasktype = taskInfo.tasktype;
            newtask.tray1_barcode = taskInfo.tray1_barcode;
            newtask.tray1_matter_barcode1 = taskInfo.tray1_matter_barcode1;
            newtask.tray1_matter_barcode2 = taskInfo.tray1_matter_barcode2;
            newtask.tray2_barcode = taskInfo.tray2_barcode;
            newtask.tray2_matter_barcode1 = taskInfo.tray2_matter_barcode1;
            newtask.tray2_matter_barcode2 = taskInfo.tray2_matter_barcode2;
            newtask.warehouse_guid = taskInfo.warehouse_guid;
            newtask.matterbarcode_list = taskInfo.matterbarcode_list;
            newtask.bills_type = taskInfo.bills_type;
            newtask.re_detail_traymatter_guid = taskInfo.re_detail_traymatter_guid;
            //添加新任务
            Add(newtask);
            taskInfo.new_task_queue_guid = newtask.guid;
            //修改取消的任务
            Update(taskInfo);
            return newtask.guid.ToString();
        }
        /// <summary>
        /// 检查并返回一个出库任务 
        /// </summary>
        /// <returns></returns>
      /*  public hdw_stacker_taskqueue GetTaskObjOut()
        {
            if (GetRunningTaskInout() == 0)
            {
                IQueryable<hdw_stacker_taskqueue> query4 = from a in dbcontext.hdw_stacker_taskqueue
                                                           join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                           where a.tasktype == WcsTaskType.StockOut &&
                                                            a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                             && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                           orderby a.sort descending, a.createdate
                                                           select a;
                return query4.AsNoTracking().FirstOrDefault();
            }
            return null;
        }*/
        //public List<Guid> GetShelvesGuidList(List<wh_shelves> whshelvesList)
        //{
        //    List<Guid> shelfGuid = new List<Guid>();
        //    if (whshelvesList != null && whshelvesList.Count > 0)
        //    {
        //        shelfGuid = whshelvesList.Select(s => s.guid).ToList();
        //    }
        //    return shelfGuid;
        //}
        /// <summary>
        /// 返回运行中的出入库任务数量(包括移库)
        /// </summary>
        /// <returns></returns>
        public int GetRunningTaskInout(hdw_plc _stacker, List<wh_shelves> whshelvesList)
        {
            var shelfGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);
            /*IQueryable<hdw_stacker_taskqueue> query_in = (from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.dest_shelfunits_guid equals b.guid
                                                          where a.warehouse_guid == _stacker.warehouse_guid &&
                                                          shelfGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockIn
                                                        && a.state == LTWMSEFModel.EntityStatus.Normal && (a.taskstatus == WcsTaskStatus.IsSend
                                                            || a.taskstatus == WcsTaskStatus.Pause
                                                               || a.taskstatus == WcsTaskStatus.Exception || a.taskstatus == WcsTaskStatus.Execute
                                                               || a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended ||
                                                               a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended
                                                         )
                                                          select a).Union(
                                                           from a in dbcontext.hdw_stacker_taskqueue
                                                           join b in dbcontext.wh_shelfunits
                                                      on a.src_shelfunits_guid equals b.guid
                                                           where a.warehouse_guid == _stacker.warehouse_guid &&
                                                          shelfGuid.Contains(b.shelves_guid) && (a.tasktype == WcsTaskType.StockOut || a.tasktype == WcsTaskType.MoveTo)
                                                         && a.state == LTWMSEFModel.EntityStatus.Normal && (a.taskstatus == WcsTaskStatus.IsSend
                                                             || a.taskstatus == WcsTaskStatus.Pause
                                                                || a.taskstatus == WcsTaskStatus.Exception || a.taskstatus == WcsTaskStatus.Execute
                                                                || a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended ||
                                                                a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended
                                                          )
                                                           select a);
            return query_in.AsNoTracking().Count();*/
            IQueryable<hdw_stacker_taskqueue> query_in = from aa in dbcontext.hdw_stacker_taskqueue
                                                         where (from a in dbcontext.hdw_stacker_taskqueue
                                                                join b in dbcontext.wh_shelfunits
                                                           on a.dest_shelfunits_guid equals b.guid
                                                                where a.warehouse_guid == _stacker.warehouse_guid &&
                                                                shelfGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockIn
                                                              && a.state == LTWMSEFModel.EntityStatus.Normal && (a.taskstatus == WcsTaskStatus.IsSend
                                                                  || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.ExceptionHandling
                                                                  || a.taskstatus == WcsTaskStatus.ExceptionSended
                                                                     // || a.taskstatus == WcsTaskStatus.Exception
                                                                     || a.taskstatus == WcsTaskStatus.Execute
                                                                     || a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended ||
                                                                     a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended
                                                               )
                                                                select a.guid).Union(
                                                           from a in dbcontext.hdw_stacker_taskqueue
                                                           join b in dbcontext.wh_shelfunits
                                                      on a.src_shelfunits_guid equals b.guid
                                                           where a.warehouse_guid == _stacker.warehouse_guid &&
                                                          shelfGuid.Contains(b.shelves_guid) && (a.tasktype == WcsTaskType.StockOut || a.tasktype == WcsTaskType.MoveTo)
                                                         && a.state == LTWMSEFModel.EntityStatus.Normal && (a.taskstatus == WcsTaskStatus.IsSend
                                                             || a.taskstatus == WcsTaskStatus.Pause || a.taskstatus == WcsTaskStatus.ExceptionHandling
                                                             || a.taskstatus == WcsTaskStatus.ExceptionSended
                                                                //|| a.taskstatus == WcsTaskStatus.Exception 
                                                                || a.taskstatus == WcsTaskStatus.Execute
                                                                || a.taskstatus == WcsTaskStatus.CancelHandling || a.taskstatus == WcsTaskStatus.CancelSended ||
                                                                a.taskstatus == WcsTaskStatus.ForceCompleteHandling || a.taskstatus == WcsTaskStatus.ForceCompleteSended
                                                          )
                                                           select a.guid).Contains(aa.guid)
                                                         select aa;
            return query_in.AsNoTracking().Count();
            //int _intcount_out = query_out.AsNoTracking().Count();
            //return _intcount_in + _intcount_out;
        }
        /// <summary>
        /// 检查入库任务，如果有出入库任务则返回null
        /// </summary>
        /// <param name="rack"></param>
        /// <param name="_split_col"></param>
        /// <returns></returns>
       /* public hdw_stacker_taskqueue GetWaitedTaskIn()
        {//只要有出入库任务，就不能下入库  
            if (GetRunningTaskInout() == 0)
            {   //查询一个入库任务
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.dest_shelfunits_guid equals b.guid
                                                          where a.tasktype == WcsTaskType.StockIn &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.createdate
                                                          select a;
                hdw_stacker_taskqueue taskInstock = query.AsNoTracking().FirstOrDefault();
                if (taskInstock != null && Guid.Empty != taskInstock.guid)
                {
                    return taskInstock;
                }
            }
            return null;
        }*/
        /// <summary>
        /// 根据订单编号生成对应的出库任务
        /// </summary>
        /// <param name="odd_numbers"></param>
        /*public int CreateTaskByOrderNum(string odd_numbers, string location, Guid bill_stockout_guid)
        {
            if (string.IsNullOrWhiteSpace(odd_numbers))
            {
                odd_numbers = "";
            }
            int _retv = 0;
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                     join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                     where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.lot_number == odd_numbers.Trim()) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                     && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                     orderby a.columns descending, a.rows ascending
                                                                     select a;
            var listShelfUnits = query.AsNoTracking().ToList();
            //生成堆垛机出库任务。。。。。。。。。。。。。。。
            List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue> lsttaskqueue = new List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>();
            if (listShelfUnits != null && listShelfUnits.Count > 0)
            {// 按订单 簇  从小到大排序  依次出库
                var listTrayMatter = dbcontext.wh_traymatter.Where(w => w.lot_number == odd_numbers.Trim()).AsNoTracking().ToList().OrderBy(o =>
                {//按簇排序   
                    string[] arr = (o.x_barcode ?? "").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 3)
                    {//T03(订单)-1（簇）-1（编号）
                        return arr[1];
                    }
                    return "";
                }).ThenBy(o =>
                {
                    //按编号排序
                    string[] arr = (o.x_barcode ?? "").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 3)
                    {//T03(订单)-1（簇）-1（编号）
                        return arr[2];
                    }
                    return "";
                }).ToList();
                ////////////////////////////////////////////

                //   List<LTWMSEFModel.Hardware.hdw_task_bill_inout> lsttaskinoutRe = new List<LTWMSEFModel.Hardware.hdw_task_bill_inout>(); 
                foreach (var item in listShelfUnits)
                {
                    var task = AddTaskByShelfUnit(item, odd_numbers);
                    lsttaskqueue.Add(task);
                    //修改对应库位状态
                    item.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.WaitOut;
                    item.tray_outdatetime = DateTime.Now;
                    //对应库位有历史遗留任务/先删除历史任务再新增
                    //Delete(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut &&
                    //    w.src_rack == task.src_rack
                    // && w.src_col == task.src_col
                    // && w.src_row == task.src_row);
                    var DelList = GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut &&
                          w.src_rack == task.src_rack
                       && w.src_col == task.src_col
                       && w.src_row == task.src_row);
                    if (DelList != null && DelList.Count > 0)
                    {
                        foreach (var itemD in DelList)
                        {
                            Delete(itemD);
                        }
                    }
                    //出库单与出库之间关联 
                    /*LTWMSEFModel.Hardware.hdw_task_bill_inout reModel = new LTWMSEFModel.Hardware.hdw_task_bill_inout();
                     reModel.guid = Guid.NewGuid();
                     reModel.billtype = LTWMSEFModel.Hardware.BillType.BillStockOut;
                     reModel.bill_stockinout_guid = bill_stockout_guid;
                     reModel.task_queue_guid = task.guid;
                     lsttaskinoutRe.Add(reModel);
                     * /

                }
                ////  _retv = lsttaskqueue.Count();
                ///对任务队列进行排序
                List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue> orderListTaskQueue = new List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>();
                foreach (var item in listTrayMatter)
                {
                    var _objTask = lsttaskqueue.FirstOrDefault(w => w.tray1_matter_barcode1 == item.x_barcode || w.tray1_matter_barcode2 == item.x_barcode);
                    if (!orderListTaskQueue.Contains(_objTask))
                    {
                        _objTask.createdate = DateTime.Now;
                        orderListTaskQueue.Add(_objTask);
                        //休息5毫秒，为了排序
                        System.Threading.Thread.Sleep(5);
                    }
                }
                //添加任务
                //AddRange(lsttaskqueue);
                AddRange(orderListTaskQueue);
                //出库单据和任务关联
                //  bll_task_bill_inout.AddRange(lsttaskinoutRe);
                //修改库位状态
                bll_shelfunits.Update(listShelfUnits);
            }

            //生成AGV搬运任务 
            IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query2 = from a in dbcontext.wh_shelfunits
                                                                      join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                      where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && w.lot_number == odd_numbers.Trim()) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                      && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && (a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored ||
                                                                      a.cellstate == ShelfCellState.WaitOut || a.cellstate == ShelfCellState.TrayOut)
                                                                      orderby a.columns descending, a.rows ascending
                                                                      select a;

            var listShelfUnits2 = query2.AsNoTracking().ToList();
            //从数据库查询起点
            string agvpos1 = bll_sys_control_dic.GetValueByType(CommDictType.AgvStartPos1);
            if (string.IsNullOrWhiteSpace(agvpos1))
            {// 默认起点P1
                agvpos1 = "P1";
                bll_sys_control_dic.SetValueByType(CommDictType.AgvStartPos1, agvpos1);
            }
            //生成agv搬运任务
            / *  var agvTaskmain = new hdw_agv_task_main();
              agvTaskmain.createdate = DateTime.Now;
              agvTaskmain.createuser = "lt-server";
              agvTaskmain.dest_rack = location;
              agvTaskmain.guid = Guid.NewGuid();
              agvTaskmain.order = odd_numbers;
              agvTaskmain.rec_status = AgvReceiveStatus.WaitSend;
              agvTaskmain.src_rack = agvpos1;
              agvTaskmain.state = LTWMSEFModel.EntityStatus.Normal;
              agvTaskmain.task_execute_status = AgvTaskExecuteStatus.None;* /
            //  List<hdw_agv_taskqueue> lst_agv_taskqueue = new List<hdw_agv_taskqueue>();
            if (listShelfUnits2 != null && listShelfUnits2.Count > 0)
            {
                foreach (var item in listShelfUnits2)
                {
                    //先从本地查
                    var task = lsttaskqueue.FirstOrDefault(w => w.src_rack == item.rack
                          && w.src_col == item.columns
                          && w.src_row == item.rows);
                    //本地没有从数据库查
                    if (task == null)
                    {//从数据库查
                        task = GetFirstDefault(w => w.src_rack == item.rack
                          && w.src_col == item.columns
                          && w.src_row == item.rows);
                    }
                    //生成搬运任务//一个托盘上面最多有两个电池，每个电池生成一个出库任务。 
                    /* if (task != null)
                     {
                         for (int i = 0; i < 2; i++)
                         {
                             if ((i == 0 && !string.IsNullOrWhiteSpace(task.tray1_matter_barcode1))
                                || (i == 1 && !string.IsNullOrWhiteSpace(task.tray1_matter_barcode2)))
                             {
                                 var agvTaskQue = new hdw_agv_taskqueue();
                                 agvTaskQue.id = bll_sys_table_id.GetId(Basic.sys_table_idBLL.TableIdType.hdw_agv_taskqueue);
                                 agvTaskQue.agv_task_main_guid = agvTaskmain.guid;
                                 agvTaskQue.createdate = DateTime.Now;
                                 agvTaskQue.createuser = "lt-server";
                                 agvTaskQue.src_point = agvpos1;//起点默认P1
                                 agvTaskQue.dest_point = agvTaskmain.dest_rack;
                                 agvTaskQue.guid = Guid.NewGuid();
                                 if (i == 0)
                                 {
                                     agvTaskQue.matter_barcode1 = task.tray1_matter_barcode1;
                                 }
                                 else
                                 {
                                     agvTaskQue.matter_barcode1 = task.tray1_matter_barcode2;
                                 }
                                 agvTaskQue.order = agvTaskmain.order;
                                 agvTaskQue.state = LTWMSEFModel.EntityStatus.Normal;
                                 agvTaskQue.task_status = AgvTaskStatus.Holding;
                                 lst_agv_taskqueue.Add(agvTaskQue);
                                 _retv++;
                             }
                         }
                     }* /
                }
                //if (lst_agv_taskqueue.Count > 0)
                //{
                //    bll_hdw_agv_taskqueue.AddRange(lst_agv_taskqueue);
                //}
            }
            / *  if (lst_agv_taskqueue.Count == 0)
              {//没有数据 默认发送成功且执行结束。。。
                  agvTaskmain.rec_status = AgvReceiveStatus.Ok;
                  agvTaskmain.task_execute_status = AgvTaskExecuteStatus.Finished;
              }
              agvTaskmain.total_count = lst_agv_taskqueue.Count();
              bll_hdw_agv_task_main.Add(agvTaskmain);* /

            return _retv;
        }*/
        /// <summary>
        /// 根据库位生成对应的出库任务（强制出库）
        /// </summary>
        /// <param name="shelfunit"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue CreateTaskByShelfUnits(wh_shelfunits shelfunit)
        {
            var task = AddTaskByShelfUnit(shelfunit, null);
            //添加任务
            return Add(task);
        }

        /// <summary>
        /// 根据库位生成对应的出库任务（强制出库）,添加排序
        /// </summary>
        /// <param name="shelfunit"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue CreateTaskByShelfUnitsWithSort(wh_shelfunits shelfunit, int sort)
        {
            var task = AddTaskByShelfUnit(shelfunit, null);
            task.sort = sort;
            //添加任务
            return Add(task);
        }
        /// <summary>
        /// 根据出库口出空托盘
        /// </summary>
        /// <param name="dest_station"></param>
        /// <returns>返回消息</returns>
        public string SetOffAndEmptyTray(int dest_station, List<wh_shelves> whshelvesList)
        {
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);
            string _rtvStr = "";
            IQueryable<hdw_stacker_taskqueue> query4 = from a in dbcontext.hdw_stacker_taskqueue
                                                       join b in dbcontext.wh_shelfunits
                                                 on a.src_shelfunits_guid equals b.guid
                                                       where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut &&
                                                       a.dest_station == dest_station && a.is_emptypallet == true &&
                                                        a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus != WcsTaskStatus.Canceled &&
                                                        a.taskstatus != WcsTaskStatus.Finished && a.taskstatus != WcsTaskStatus.ForceComplete
                                                         && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                       //  orderby a.sort descending, a.createdate
                                                       select a;
            if (!query4.AsNoTracking().Any())
            {//生成空库位出库

                IQueryable<LTWMSEFModel.Warehouse.wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                                         join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                                         where shelvesGuid.Contains(a.shelves_guid) && dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid) == false && a.state == LTWMSEFModel.EntityStatus.Normal
                                                                         && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                                         orderby a.rows ascending, a.columns ascending
                                                                         select a;
                var ShelfUnit = query.AsNoTracking().FirstOrDefault();
                if (ShelfUnit != null)
                {
                    //修改对应库位状态
                    ShelfUnit.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.WaitOut;
                    ShelfUnit.tray_outdatetime = DateTime.Now;
                    var rtv = bll_shelfunits.Update(ShelfUnit);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {//修改成功，添加任务，没有修改成功则不修改
                        var task = AddTaskByShelfUnit(ShelfUnit, "");
                        task.dest_station = dest_station;
                        task.sort = 999;//999优先级最大，优先出库
                        //添加任务
                        Add(task);
                    }
                    else
                    {
                        _rtvStr = "出空托盘失败，请重试";
                    }
                }
                else
                {//库位没有空托盘
                    _rtvStr = "库位没有空托盘，请稍后再试...";
                }
            }
            else
            {//已存在一个电池插箱任务
                _rtvStr = "";
            }
            return _rtvStr;
        }
        /// <summary>
        /// 【调用该方法必须加事务】
        /// 检查阻挡库位是否需要移库
        /// 根据需要移动的库位，生成移库任务
        /// 自动查找并分配库位给待移动的库位
        /// </summary>
        /// <param name="item">需要移库的库位</param>
        /// <param name="whshelvesList">堆垛机能够操作的货架排列表</param>
        /// <returns></returns>
        public CheckAddMoveReturnType CheckAddTaskMoveByShelfUnit(wh_shelfunits waiteMoveShelfUnit, List<wh_shelves> whshelvesList)
        {
            //待移动库位查找库位是否有任务
            var TaskWaiteMd = GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
            (w.dest_shelfunits_guid == waiteMoveShelfUnit.guid || w.src_shelfunits_guid == waiteMoveShelfUnit.guid));
            if (TaskWaiteMd != null && TaskWaiteMd.guid != Guid.Empty)
            {//1、有出库任务：先出库
                //2、有入库任务  正在入库？等待。。。  入库锁定，任务未下发？？？
                if (TaskWaiteMd.tasktype == WcsTaskType.StockIn && TaskWaiteMd.taskstatus == WcsTaskStatus.Holding)
                {//如果是入库任务（极小概率），可以出库，不影响后期入库，最多浪费一个储位（入库分配库位时有检测，如果纵深有出库会另外分配入库库位）
                    return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.CanInOut };
                }
                else if ((TaskWaiteMd.tasktype == WcsTaskType.MoveTo ||
                    TaskWaiteMd.tasktype == WcsTaskType.StockOut) && TaskWaiteMd.taskstatus == WcsTaskStatus.Holding)
                {//出库任务或移库任务优先执行
                    return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.Blocked, stacker_queue = TaskWaiteMd };
                }
            }
            else
            {//没有对应的出入库任务
             //待移库库位可能存在多线程并发操作冲突，用事务控制回滚
                bool _lack_of_shelfunit = false;//库位不足
                if (waiteMoveShelfUnit.cellstate == ShelfCellState.Stored)
                {//生成移库任务 
                    //查找对应的待移动库位
                    var dest_shelfU = bll_shelfunits.GetNearbyMoveShelfUnitAndLock(waiteMoveShelfUnit, whshelvesList);
                    //目标移动库位
                    if (dest_shelfU != null && dest_shelfU.guid != Guid.Empty)
                    {
                        //保存 >>待移动库位  判断待移动库位是否有出库任务。。。？？？
                        waiteMoveShelfUnit.cellstate = ShelfCellState.TrayOut;
                        waiteMoveShelfUnit.locktype = ShelfLockType.SysLock;
                        waiteMoveShelfUnit.updatedate = DateTime.Now;
                        var shelfRtv = bll_shelfunits.Update(waiteMoveShelfUnit);
                        //保存 >>目标移动库位
                        dest_shelfU.cellstate = ShelfCellState.TrayIn;
                        dest_shelfU.locktype = ShelfLockType.SysLock;
                        dest_shelfU.updatedate = DateTime.Now;
                        var shelfRtv2 = bll_shelfunits.Update(dest_shelfU);
                        if (shelfRtv == LTWMSEFModel.SimpleBackValue.True && shelfRtv2 == LTWMSEFModel.SimpleBackValue.True)
                        {   //添加移库任务
                            var taskMoveObj = AddTaskMove(waiteMoveShelfUnit, dest_shelfU);
                            var shelfrtv3 = Add(taskMoveObj);
                            if (shelfrtv3 == LTWMSEFModel.SimpleBackValue.True)
                            {//移库任务生成成功
                                return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.Blocked, stacker_queue = taskMoveObj };
                            }
                        }
                    }
                    else
                    {//库位不足，不能移库。。。？？？ 直接出库？？？ 待完善 
                        _lack_of_shelfunit = true;
                    }
                }
                else if (waiteMoveShelfUnit.cellstate == ShelfCellState.CanIn && waiteMoveShelfUnit.locktype == ShelfLockType.SysLock)
                {//内侧库位出库，锁定外侧入库库位不入库，出库完成系统会自动解锁
                    return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.CanInOut };
                }

                if (_lack_of_shelfunit ||
                    (waiteMoveShelfUnit.cellstate == ShelfCellState.CanIn && waiteMoveShelfUnit.locktype == ShelfLockType.ManLock))
                {//库位不足
                    //人工锁  生成一个出库任务？如果有货则出库，没货堆垛机报取空异常（取空异常人工强制完成后需要解锁库位）
                    waiteMoveShelfUnit.cellstate = ShelfCellState.WaitOut;//待出库 
                    waiteMoveShelfUnit.locktype = ShelfLockType.SysLock;
                    waiteMoveShelfUnit.tray_outdatetime = DateTime.Now;
                    var rtv2 = bll_shelfunits.Update(waiteMoveShelfUnit);
                    // 生成对应的出库任务
                    //  var rtv3 = bll_stacker_taskqueue.CreateTaskByShelfUnits(info);
                    var task = AddTaskByShelfUnit(waiteMoveShelfUnit, null);
                    var rtv3 = Add(task);
                    if (rtv2 == LTWMSEFModel.SimpleBackValue.True && rtv3 == LTWMSEFModel.SimpleBackValue.True)
                    {
                        return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.Blocked, stacker_queue = task };
                    }
                }
            }
            return new CheckAddMoveReturnType() { checkResult = CheckBlockResultEnum.Blocked };
        }
        public hdw_stacker_taskqueue AddTaskMove(wh_shelfunits movStart, wh_shelfunits movDest)
        {
            //根据库位生成出库任务
            var task = new LTWMSEFModel.Hardware.hdw_stacker_taskqueue();
            task.id = bll_sys_table_id.GetId(Basic.sys_table_idBLL.TableIdType.hdw_stacker_taskqueue);
            task.createdate = DateTime.Now;
            task.createuser = "sys_server";
            task.guid = Guid.NewGuid();
            //task.is_emptypallet = null;
            task.state = LTWMSEFModel.EntityStatus.Normal;
            task.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.Holding;
            task.tasktype = LTWMSEFModel.Hardware.WcsTaskType.MoveTo;
            task.src_station = 0;
            task.src_rack = movStart.rack;
            task.src_col = movStart.columns;
            task.src_row = movStart.rows;
            task.order = "";
            task.src_shelfunits_pos = movStart.rack + "-" + movStart.columns + "-" + movStart.rows;
            task.src_shelfunits_guid = movStart.guid;
            task.shelves_guid = movStart.shelves_guid;
            task.warehouse_guid = movStart.warehouse_guid;

            //终点
            task.dest_rack = movDest.rack;
            task.dest_col = movDest.columns;
            task.dest_row = movDest.rows;
            task.dest_shelfunits_pos = movDest.rack + "-" + movDest.columns + "-" + movDest.rows;
            task.dest_shelfunits_guid = movDest.guid;
            task.shelves_guid_end = movDest.shelves_guid;
            // 下架明细。。。
            task.tray1_barcode = movStart.depth1_traybarcode;

            //根据托盘号查询对应的物料信息
            //IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query2 = from a in dbcontext.wh_tray
            //                                                       join b in dbcontext.wh_traymatter
            //                                                       on a.guid equals b.tray_guid
            //                                                       where a.traybarcode == item.depth1_traybarcode
            //纵深1                                                select b;
            var lst2 = bll_wh_tray.GetMatterDetailByTrayBarcode(movStart.depth1_traybarcode);
            if (lst2 != null && lst2.Count > 0)
            {
                task.order = lst2[0].lot_number;
                task.tray1_matter_barcode1 = lst2[0].x_barcode;
                if (lst2.Count > 1)
                {
                    task.tray1_matter_barcode2 = lst2[1].x_barcode;
                }
                task.matterbarcode_list = string.Join(",", lst2.Select(w => w.x_barcode).ToArray());
            }
            //纵深2
            var lstd12 = bll_wh_tray.GetMatterDetailByTrayBarcode(movStart.depth2_traybarcode);
            if (lstd12 != null && lstd12.Count > 0)
            {
                task.tray2_matter_barcode1 = lstd12[0].x_barcode;
                if (lstd12.Count > 1)
                {
                    task.tray2_matter_barcode2 = lstd12[1].x_barcode;
                }
                task.matterbarcode_list = string.Join(",", lstd12.Select(w => w.x_barcode).ToArray());
            }
            if (string.IsNullOrWhiteSpace(task.tray1_matter_barcode1) && string.IsNullOrWhiteSpace(task.tray1_matter_barcode2))
            {//
                task.is_emptypallet = true;
            }
            else
            {
                task.is_emptypallet = false;

            }
            return task;
        }
        /// <summary>
        /// 添加一条出库任务
        /// </summary>
        /// <param name="item"></param>
        /// <param name="odd_numbers"></param>
        public hdw_stacker_taskqueue AddTaskByShelfUnit(wh_shelfunits item, string odd_numbers)
        {
            return AddTaskByShelfUnit(item, odd_numbers, bll_sys_table_id.GetId(Basic.sys_table_idBLL.TableIdType.hdw_stacker_taskqueue));
        }
        /// <summary>
        /// 添加一条出库任务
        /// </summary>
        /// <param name="item"></param>
        /// <param name="odd_numbers"></param>
        public hdw_stacker_taskqueue AddTaskByShelfUnit(wh_shelfunits item, string odd_numbers, int task_id)
        {
            //根据库位生成出库任务
            var task = new LTWMSEFModel.Hardware.hdw_stacker_taskqueue();
            task.id = task_id;// bll_sys_table_id.GetId(Basic.sys_table_idBLL.TableIdType.hdw_stacker_taskqueue);
            task.createdate = DateTime.Now;
            task.createuser = "sys_server";
            task.guid = Guid.NewGuid();
            //task.is_emptypallet = null;
            task.state = LTWMSEFModel.EntityStatus.Normal;
            task.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.Holding;
            task.tasktype = LTWMSEFModel.Hardware.WcsTaskType.StockOut;
            task.src_station = 0;
            task.src_rack = item.rack;
            task.src_col = item.columns;
            task.src_row = item.rows;
            task.order = odd_numbers;
            task.src_shelfunits_pos = item.rack + "-" + item.columns + "-" + item.rows;
            task.src_shelfunits_guid = item.guid;
            task.shelves_guid = item.shelves_guid;
            task.warehouse_guid = item.warehouse_guid;
            // 下架明细。。。
            task.tray1_barcode = item.depth1_traybarcode;

            //根据托盘号查询对应的物料信息
            //IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query2 = from a in dbcontext.wh_tray
            //                                                       join b in dbcontext.wh_traymatter
            //                                                       on a.guid equals b.tray_guid
            //                                                       where a.traybarcode == item.depth1_traybarcode
            //纵深1                                                select b;
            var lst2 = bll_wh_tray.GetMatterDetailByTrayBarcode(item.depth1_traybarcode);
            if (lst2 != null && lst2.Count > 0)
            {
                task.order = lst2[0].lot_number;
                task.tray1_matter_barcode1 = lst2[0].x_barcode;
                if (lst2.Count > 1)
                {
                    task.tray1_matter_barcode2 = lst2[1].x_barcode;
                }
                task.matterbarcode_list = string.Join(",", lst2.Select(w => w.x_barcode).ToArray());
            }
            //纵深2
            var lstd12 = bll_wh_tray.GetMatterDetailByTrayBarcode(item.depth2_traybarcode);
            if (lstd12 != null && lstd12.Count > 0)
            {
                task.tray2_matter_barcode1 = lstd12[0].x_barcode;
                if (lstd12.Count > 1)
                {
                    task.tray2_matter_barcode2 = lstd12[1].x_barcode;
                }
                task.matterbarcode_list = string.Join(",", lstd12.Select(w => w.x_barcode).ToArray());
            }
            if (string.IsNullOrWhiteSpace(task.tray1_matter_barcode1) && string.IsNullOrWhiteSpace(task.tray1_matter_barcode2))
            {//
                task.is_emptypallet = true;
            }
            else
            {
                task.is_emptypallet = false;

            }
            return task;
        }
        /**********************************/
        /// <summary>
        /// 获取所有入库任务
        /// 按排序、和时间顺序查询
        /// </summary> 
        /// <returns></returns>
        public List<hdw_stacker_taskqueue> GetALLWaitedTaskIn(List<wh_shelves> whshelvesList)
        {
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);
            IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                      join b in dbcontext.wh_shelfunits
                                                 on a.dest_shelfunits_guid equals b.guid
                                                      where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockIn &&
                                                      a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                      && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                      //orderby a.sort descending, a.createdate
                                                      orderby a.createdate //以创建时间排序（分配任务是从外侧到内侧）
                                                      select a;
            return query.AsNoTracking().ToList();
        }

        /// <summary>
        /// 获取所有出库任务
        /// 按排序、和时间顺序查询
        /// </summary> 
        /// <returns></returns>
        public List<hdw_stacker_taskqueue> GetALLWaitedTaskOut(string keywords, int currentpage, int pagesize, out int totalcount, Guid warehouseguid)
        {
            /*
               keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query = from a in dbcontext.wh_traymatter
                                                            join b in dbcontext.wh_tray
                                                             on a.tray_guid equals b.guid
                                                            where b.state != LTWMSEFModel.EntityStatus.Deleted
                                                            && (keywords == "" || (b.traybarcode ?? "").ToLower().Contains(keywords) || (b.shelfunits_pos ?? "").ToLower().Contains(keywords) ||
                                                             (b.memo ?? "").ToLower().Contains(keywords) || (a.matter_barcode ?? "").ToLower().Contains(keywords))
                                                            select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
             
             */
            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                      join b in dbcontext.wh_shelfunits
                                                 on a.src_shelfunits_guid equals b.guid
                                                      where a.tasktype == WcsTaskType.StockOut && a.warehouse_guid == warehouseguid &&
                                                      b.warehouse_guid == warehouseguid &&
                                                      a.state == LTWMSEFModel.EntityStatus.Normal && (
                                                      keywords == ""
                                                       || a.id.ToString().ToLower().Equals(keywords)
                                                       || (a.order ?? "").Trim().ToLower().Contains(keywords)
                                                      || (a.tray1_barcode ?? "").Trim().ToLower().Contains(keywords)
                                                       || (a.tray1_matter_barcode1 ?? "").Trim().ToLower().Contains(keywords)
                                                        || (a.tray1_matter_barcode2 ?? "").Trim().ToLower().Contains(keywords)
                                                         || (b.shelfunits_pos ?? "").Trim().ToLower().Contains(keywords)
                                                         || (a.matterbarcode_list ?? "").Contains(keywords)
                                                      )
                                                      && a.taskstatus == WcsTaskStatus.Holding
                                                      && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                      //orderby a.sort descending, a.createdate
                                                      select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.sort).ThenBy(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }

        /// <summary>
        /// 获取入库对立面站点的出库任务
        /// </summary>
        /// <param name="oppositeStation">入库对立面的站点</param>
        /// <param name="transportNumber1">站点1的编号</param>
        /// <param name="transportNumber2">站点2的编号</param>
        /// <returns></returns>
        public hdw_stacker_taskqueue GetOppositeTaskOut(wh_wcs_device wcsStation, List<wh_shelves> whshelvesList)
        {
            if (wcsStation == null || wcsStation.run_status != PLCRunStatus.Ready)
            {//为空或未准备好，直接返回null
                return null;
            }
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);
            hdw_stacker_taskqueue taskout;
            //判断oppositeStation 是否等于 1、2站点
            if (wcsStation.default_out == true)
            {//站点等于1、2，可以出电池（dest_station=0 或dest_station=oppositeStation的物料）
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut
                                                          && (a.dest_station == 0 || a.dest_station == wcsStation.number) &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.dest_station descending, a.createdate// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                                                          select a;
                // a.dest_station descending 其它物料（非电池）优先出库
                taskout = query.AsNoTracking().FirstOrDefault();
                //判断当前出库口为1、2时+出库口为0 则修改对应的出库口为 出库口
                if (taskout != null && taskout.dest_station == 0)
                {
                    taskout.dest_station = wcsStation.number;
                }
            }
            else
            {//3、4口只能出其它物料
                //dest_station=0 只对应电池的出库任务，如果出其它物料会强制选择出库口。。。。dest_station>0
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut
                                                          && a.dest_station == wcsStation.number &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.createdate
                                                          select a;
                taskout = query.AsNoTracking().FirstOrDefault();
            }
            return taskout;
        }
        /// <summary>
        /// 根据入库起点站台查找对应的入库任务
        /// </summary>
        /// <param name="src_station">起点站台</param>
        /// <returns></returns>
        public hdw_stacker_taskqueue GetOppositeTaskIn(wh_wcs_device wcsStation, List<wh_shelves> whshelvesList)
        {
            if (wcsStation == null)
            {
                return null;
            }
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(whshelvesList);
            IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                      join b in dbcontext.wh_shelfunits
                                                 on a.dest_shelfunits_guid equals b.guid
                                                      where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockIn
                                                      && a.src_station == wcsStation.number &&
                                                      a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                      && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                      orderby a.sort descending, a.createdate
                                                      select a;
            return query.AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        ///按出库优先级出库
        ///出库任务 判断对应是否允许出库。。。只出允许出库的任务！！！ 
        /// </summary>
        /// <param name="Transport1">1号口输送线状态</param>
        /// <param name="Transport2">2号口输送线状态</param>
        /// <param name="Transport3">3号口输送线状态</param>
        /// <param name="Transport4">4号口输送线状态</param>
        /// <param name="transportNumber1">1号口输送线编号</param>
        /// <param name="transportNumber2">2号口输送线编号</param>
        /// <param name="transportNumber3">3号口输送线编号</param>
        /// <param name="transportNumber4">4号口输送线编号</param>
        /// <returns></returns>
        public hdw_stacker_taskqueue GetTaskDefaultTaskOut(List<wh_wcs_device> stationList, List<wh_shelves> lstShelves)
        {
            //if (Transport1 != PLCRunStatus.Ready && Transport2 != PLCRunStatus.Ready && Transport3 != PLCRunStatus.Ready && Transport4 != PLCRunStatus.Ready)
            //{//四个出库口都没准备好，则不出库
            //    return null;
            //}
            if (stationList == null ||
               stationList.Where(w => w.run_status != PLCRunStatus.Ready).Count() == stationList.Count())
            {//未准备好的数量等于所有数量，说明所有出库口都没有准备
                return null;
            }
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(lstShelves);
            foreach (var _station in stationList)
            {
                if (_station.device_type == DeviceTypeEnum.Station && _station.run_status == PLCRunStatus.Ready)
                {//出库口 准备好
                    if (_station.default_out == true)
                    {//默认出库口 
                        IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                                  join b in dbcontext.wh_shelfunits
                                                             on a.src_shelfunits_guid equals b.guid
                                                                  where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut
                                                                  && (a.dest_station == 0 || a.dest_station == _station.number) &&
                                                                  a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                                  && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                  orderby a.sort descending, a.dest_station descending, a.createdate// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                                                                  select a;// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                        var TranTaskOut1 = query.AsNoTracking().FirstOrDefault();//排序级别》物料出库优先级》电池优先级 【物料出库 dest_station >0】
                        if (TranTaskOut1 != null && TranTaskOut1.dest_station == 0)
                        {
                            TranTaskOut1.dest_station = _station.number;
                        }
                        //按优先级 1、2、3、4、号口返回出库任务。。。
                        if (TranTaskOut1 != null && TranTaskOut1.guid != Guid.Empty)
                        {//1号口不为空，返回
                            return TranTaskOut1;
                        }
                    }
                    else
                    {
                        IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                                  join b in dbcontext.wh_shelfunits
                                                             on a.src_shelfunits_guid equals b.guid
                                                                  where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut
                                                                  && a.dest_station == _station.number &&
                                                                  a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                                  && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                  orderby a.sort descending, a.createdate
                                                                  select a;
                        var TranTaskOut3 = query.AsNoTracking().FirstOrDefault();
                        if (TranTaskOut3 != null && TranTaskOut3.guid != Guid.Empty)
                        {
                            return TranTaskOut3;
                        }
                    }
                }
            }
            //hdw_stacker_taskqueue TranTaskOut1 = null;//1号口出库任务
            //hdw_stacker_taskqueue TranTaskOut2 = null;//2号口出库任务
            //hdw_stacker_taskqueue TranTaskOut3 = null;//3号口出库任务
            //hdw_stacker_taskqueue TranTaskOut4 = null;//4号口出库任务

            /*
            if (Transport2 == PLCRunStatus.Ready)
            {//2号口出库任务
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where a.tasktype == WcsTaskType.StockOut
                                                          && (a.dest_station == 0 || a.dest_station == transportNumber2) &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.dest_station descending, a.createdate//orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                                                          select a;// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                TranTaskOut2 = query.AsNoTracking().FirstOrDefault();//排序级别》物料出库优先级》电池优先级 【物料出库 dest_station >0】
                if (TranTaskOut2 != null && TranTaskOut2.dest_station == 0)
                {
                    TranTaskOut2.dest_station = transportNumber2;
                }
            }
            if (TranTaskOut2 != null && TranTaskOut2.guid != Guid.Empty)
            {
                return TranTaskOut2;
            }

          
               

            if (Transport4 == PLCRunStatus.Ready)
            {//4号口出库任务
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where a.tasktype == WcsTaskType.StockOut
                                                          && a.dest_station == transportNumber4 &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.createdate
                                                          select a;
                TranTaskOut4 = query.AsNoTracking().FirstOrDefault();
            }
            if (TranTaskOut4 != null && TranTaskOut4.guid != Guid.Empty)
            {
                return TranTaskOut4;
            }*/
            return null;//默认返回null
        }
        /// <summary>
        /// 查找最大优先级出库任务  （order>=999）
        /// </summary>
        /// <param name="Transport1">1号口输送线状态</param>
        /// <param name="Transport2">2号口输送线状态</param>
        /// <param name="Transport3">3号口输送线状态</param>
        /// <param name="Transport4">4号口输送线状态</param>
        /// <param name="transportNumber1">1号口输送线编号</param>
        /// <param name="transportNumber2">2号口输送线编号</param>
        /// <param name="transportNumber3">3号口输送线编号</param>
        /// <param name="transportNumber4">4号口输送线编号</param>
        /// <returns></returns>
        public hdw_stacker_taskqueue GetPriorityTaskInfo(List<wh_wcs_device> stationList, List<wh_shelves> lstShelves)
        {
            //if (Transport1 != PLCRunStatus.Ready && Transport2 != PLCRunStatus.Ready && 
            //    Transport3 != PLCRunStatus.Ready && Transport4 != PLCRunStatus.Ready)
            //{//四个出库口都没准备好，则不出库
            //    return null;
            //}
            if (stationList == null ||
                stationList.Where(w => w.run_status != PLCRunStatus.Ready).Count() == stationList.Count())
            {//未准备好的数量等于所有数量，说明所有出库口都没有准备
                return null;
            }
            var shelvesGuid = ComBLLService.GetBaseBaseGuidList(lstShelves);
            foreach (var _station in stationList)
            {
                if (_station.device_type == DeviceTypeEnum.Station && _station.run_status == PLCRunStatus.Ready)
                {//出库口 准备好
                    if (_station.default_out == true)
                    {//默认出库口 
                        IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                                  join b in dbcontext.wh_shelfunits
                                                             on a.src_shelfunits_guid equals b.guid
                                                                  where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut && a.sort >= 999
                                                                  && (a.dest_station == 0 || a.dest_station == _station.number) &&
                                                                  a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                                  && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                  orderby a.sort descending, a.dest_station descending, a.createdate //, a.src_row, a.src_col
                                                                  select a;// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                        var TranTaskOut1 = query.AsNoTracking().FirstOrDefault();//排序级别》物料出库优先级》电池优先级 【物料出库 dest_station >0】
                        if (TranTaskOut1 != null && TranTaskOut1.dest_station == 0)
                        {
                            TranTaskOut1.dest_station = _station.number;
                        }
                        //按优先级 1、2、3、4、号口返回出库任务。。。
                        if (TranTaskOut1 != null && TranTaskOut1.guid != Guid.Empty)
                        {//1号口不为空，返回
                            return TranTaskOut1;
                        }
                    }
                    else
                    {//非默认出库口 
                        IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                                  join b in dbcontext.wh_shelfunits
                                                             on a.src_shelfunits_guid equals b.guid
                                                                  where shelvesGuid.Contains(b.shelves_guid) && a.tasktype == WcsTaskType.StockOut && a.sort >= 999
                                                                  && a.dest_station == _station.number &&
                                                                  a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                                  && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                                  orderby a.sort descending, a.createdate
                                                                  select a;
                        var TranTaskOut3 = query.AsNoTracking().FirstOrDefault();
                        if (TranTaskOut3 != null && TranTaskOut3.guid != Guid.Empty)
                        {
                            return TranTaskOut3;
                        }
                    }
                }
            }
            /*
            hdw_stacker_taskqueue TranTaskOut1 = null;//1号口出库任务
            hdw_stacker_taskqueue TranTaskOut2 = null;//2号口出库任务
            hdw_stacker_taskqueue TranTaskOut3 = null;//3号口出库任务
            hdw_stacker_taskqueue TranTaskOut4 = null;//4号口出库任务
            
            if (Transport2 == PLCRunStatus.Ready)
            {//2号口出库任务
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where a.tasktype == WcsTaskType.StockOut && a.sort >= 999
                                                          && (a.dest_station == 0 || a.dest_station == transportNumber2) &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.dest_station descending, a.createdate //, a.src_row, a.src_col
                                                          select a;// orderby a.sort descending, a.dest_station descending, a.src_row, a.src_col
                TranTaskOut2 = query.AsNoTracking().FirstOrDefault();//排序级别》物料出库优先级》电池优先级 【物料出库 dest_station >0】
                if (TranTaskOut2 != null && TranTaskOut2.dest_station == 0)
                {
                    TranTaskOut2.dest_station = transportNumber2;
                }
            }
            if (TranTaskOut2 != null && TranTaskOut2.guid != Guid.Empty)
            {
                return TranTaskOut2;
            }
          

            if (Transport4 == PLCRunStatus.Ready)
            {//4号口出库任务
                IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                          join b in dbcontext.wh_shelfunits
                                                     on a.src_shelfunits_guid equals b.guid
                                                          where a.tasktype == WcsTaskType.StockOut && a.sort >= 999
                                                          && a.dest_station == transportNumber4 &&
                                                          a.state == LTWMSEFModel.EntityStatus.Normal && a.taskstatus == WcsTaskStatus.Holding
                                                          && b.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock
                                                          orderby a.sort descending, a.createdate
                                                          select a;
                TranTaskOut4 = query.AsNoTracking().FirstOrDefault();
            }
            if (TranTaskOut4 != null && TranTaskOut4.guid != Guid.Empty)
            {
                return TranTaskOut4;
            }*/
            return null;//默认返回null
        }
        /// <summary>
        /// 获取出库任务的最大优先级
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort(Guid warehouseguid)
        {
            IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                      where a.warehouse_guid == warehouseguid && a.tasktype == WcsTaskType.StockOut &&
                                                      a.state == LTWMSEFModel.EntityStatus.Normal
                                                      && a.taskstatus == WcsTaskStatus.Holding
                                                      //orderby a.sort descending, a.createdate
                                                      select a;

            var obj = query.AsNoTracking().OrderByDescending(o => o.sort).FirstOrDefault();
            if (obj != null && obj.guid != Guid.Empty)
            {
                return obj.sort;
            }
            return 0;
        }
        /// <summary>
        /// 通过货架和对应入库站台查找历史遗留任务
        /// </summary>
        /// <param name="lstShelves"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public List<hdw_stacker_taskqueue> GetTaskInOfHistory(List<wh_shelves> lstShelves, int station)
        {
            if (lstShelves == null || lstShelves.Count == 0)
            {
                return null;
            }
            Guid warehouseguid = lstShelves[0].warehouse_guid;
            /*  var shelvesGuid = ComBLLService.GetBaseBaseGuidList(lstShelves);
              IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                        join b in dbcontext.wh_shelfunits
                                                   on a.dest_shelfunits_guid equals b.guid
                                                        where a.warehouse_guid== warehouseguid && shelvesGuid.Contains(b.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal &&
                                                        a.tasktype == WcsTaskType.StockIn && a.src_station == station
                                                        && (a.taskstatus == WcsTaskStatus.Holding || a.taskstatus == WcsTaskStatus.WaiteDispatchStockCell)
                                                        select a;*/
            //var shelvesGuid = ComBLLService.GetBaseBaseGuidList(lstShelves);
            IQueryable<hdw_stacker_taskqueue> query = from a in dbcontext.hdw_stacker_taskqueue
                                                      where a.warehouse_guid == warehouseguid && a.state == LTWMSEFModel.EntityStatus.Normal &&
                                                      a.tasktype == WcsTaskType.StockIn && a.src_station == station
                                                      && (a.taskstatus == WcsTaskStatus.Holding || a.taskstatus == WcsTaskStatus.WaiteDispatchStockCell)
                                                      select a;
            return query.AsNoTracking().ToList();
        }
        /// <summary>
        /// 获取Bills单据的任务执行状态
        /// </summary>
        /// <param name="taskqueue_guid"></param>
        /// <returns></returns>
        public TaskStatusEnum GetTaskRunStatus(Guid taskqueue_guid)
        {
            //查找对应的任务执行状态
            var obj1 = GetFirstDefault(w => w.guid == taskqueue_guid);
            if (obj1 != null && obj1.guid != Guid.Empty)
            {//查实时任务
                return GetBillsStatus(obj1.taskstatus);
            }
            else
            {//查历史任务
                var query = from a in dbcontext.hdw_stacker_taskqueue_his
                            where a.taskqueue_guid == taskqueue_guid
                            select a;
                var obj2 = query.AsNoTracking().FirstOrDefault();
                if (obj2 != null && obj2.guid != Guid.Empty)
                {
                    return GetBillsStatus(obj2.taskstatus);
                }
            }
            return TaskStatusEnum.None;
        }
        private LTWMSEFModel.TaskStatusEnum GetBillsStatus(WcsTaskStatus status)
        {
            if (status == WcsTaskStatus.Finished ||
                    status == WcsTaskStatus.ForceComplete)
            {
                return LTWMSEFModel.TaskStatusEnum.Finished;
            }
            else if (status == WcsTaskStatus.Execute)
            {
                return LTWMSEFModel.TaskStatusEnum.Running;
            }
            else if (status == WcsTaskStatus.Canceled)
            {
                return TaskStatusEnum.Canceled;
            }
            else if (status == WcsTaskStatus.Holding)
            {
                return TaskStatusEnum.WaitedSend;
            }
            return TaskStatusEnum.None;
        }
        /// <summary>
        /// 根据入库扫码编号获取预分配的站台编号
        /// </summary>
        /// <param name="warehouseguid"></param>
        /// <param name="rfidnum"></param>
        /// <param name="mattercode"></param>
        /// <param name="lstUseableShelves"></param>
        /// <returns></returns>
        public int GetStationOfRFIDRequest(Guid? warehouseguid, int rfidnum, string mattercode
            , List<wh_shelves> lstUseableShelves)
        {
            if (lstUseableShelves == null || lstUseableShelves.Count == 0)
            {
                return 0;
            }
            int _countA = lstUseableShelves.Count(w => w.rack <= 4);
            int _countB = lstUseableShelves.Count(w => w.rack > 4);
            //1-4排可用库位总数
            int countCanIn_A = 0;
            //5-8排可用库位总数
            int countCanIn_B = 0;
            if (_countA > 0)
            {
                IQueryable<wh_shelfunits> query1 = from a in dbcontext.wh_shelfunits
                                                   where a.warehouse_guid == warehouseguid && a.rack <= 4
                                                   && a.state == LTWMSEFModel.EntityStatus.Normal
                                                   && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                   select a;
                //可用库位总数
                countCanIn_A = query1.AsNoTracking().Count();
            }
            if (_countB > 0)
            {
                IQueryable<wh_shelfunits> query2 = from a in dbcontext.wh_shelfunits
                                                   where a.warehouse_guid == warehouseguid && a.rack > 4
                                                   && a.state == LTWMSEFModel.EntityStatus.Normal
                                                   && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                                   select a;
                //可用库位总数
                countCanIn_B = query2.AsNoTracking().Count();
            }
            if (rfidnum == 1)
            {//2、4 入库口
                if (countCanIn_A > 0 && countCanIn_B > 0)
                {//对应站台2、4
                 //站台交叉切换分配
                    string stationPos = bll_sys_control_dic.GetValueByType(CommDictType.StationDispatch1, Guid.Empty);
                    if (string.IsNullOrWhiteSpace(stationPos))
                    {
                        stationPos = "2";
                    }
                    else
                    {
                        if (stationPos == "2")
                        {
                            stationPos = "4";
                        }
                        else
                        {
                            stationPos = "2";
                        }
                    }
                    bll_sys_control_dic.SetValueByType(CommDictType.StationDispatch1, stationPos, Guid.Empty);
                    return Convert.ToInt32(stationPos);
                }
                else if (countCanIn_A > 0)
                {//对应站台4
                    return 4;
                }
                else if (countCanIn_B > 0)
                {//对应站台2
                    return 2;
                }
            }
            else if (rfidnum == 2)
            {//6、8 入库口
                if (countCanIn_A > 0 && countCanIn_B > 0)
                {//对应站台6、8 
                 //站台交叉切换分配
                    string stationPos = bll_sys_control_dic.GetValueByType(CommDictType.StationDispatch2, Guid.Empty);
                    if (string.IsNullOrWhiteSpace(stationPos))
                    {
                        stationPos = "6";
                    }
                    else
                    {
                        if (stationPos == "6")
                        {
                            stationPos = "8";
                        }
                        else
                        {
                            stationPos = "6";
                        }
                    }
                    bll_sys_control_dic.SetValueByType(CommDictType.StationDispatch2, stationPos, Guid.Empty);
                    return Convert.ToInt32(stationPos);
                }
                else if (countCanIn_A > 0)
                {//对应站台8
                    return 8;
                }
                else if (countCanIn_B > 0)
                {//对应站台6
                    return 6;
                }
            }
            else if (rfidnum == 3)
            {//10、12 入库口
                if (countCanIn_A > 0 && countCanIn_B > 0)
                {//对应站台10、12
                    //站台交叉切换分配
                    string stationPos = bll_sys_control_dic.GetValueByType(CommDictType.StationDispatch3, Guid.Empty);
                    if (string.IsNullOrWhiteSpace(stationPos))
                    {
                        stationPos = "10";
                    }
                    else
                    {
                        if (stationPos == "10")
                        {
                            stationPos = "12";
                        }
                        else
                        {
                            stationPos = "10";
                        }
                    }
                    bll_sys_control_dic.SetValueByType(CommDictType.StationDispatch3, stationPos, Guid.Empty);
                    return Convert.ToInt32(stationPos);
                }
                else if (countCanIn_A > 0)
                {//对应站台12
                    return 12;
                }
                else if (countCanIn_B > 0)
                {//对应站台10
                    return 10;
                }
            }
            return 0;
            /*
            //总共60排货架 ，A面货架及编号 1-30 ，B面货架及编号31-60
            var _query = from a in dbcontext.wh_shelfunits
                         where a.warehouse_guid == warehouseguid && a.matter_barcode == mattercode && a.rack <= 30
                         select a;
            var _query2 = from a in dbcontext.wh_shelfunits
                          where a.warehouse_guid == warehouseguid && a.matter_barcode == mattercode && a.rack > 30
                          select a;
            //
            IQueryable<wh_shelfunits> query3 = from a in dbcontext.wh_shelfunits
                                               where a.warehouse_guid == warehouseguid && a.rack <= 30
                                               && a.state == LTWMSEFModel.EntityStatus.Normal
                                               && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                               select a;
            IQueryable<wh_shelfunits> query4 = from a in dbcontext.wh_shelfunits
                                               where a.warehouse_guid == warehouseguid && a.rack > 30
                                               && a.state == LTWMSEFModel.EntityStatus.Normal
                                               && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                                               select a;
            //入库左边（A面）物料总数量
            int countA = _query.AsNoTracking().Count();
            //右边（B面）物料总数量
            int countB = _query2.AsNoTracking().Count();
            //A面可用库位总数
            int countCanIn_A = query3.AsNoTracking().Count();
            //B面可用库位总数
            int countCanIn_B = query4.AsNoTracking().Count();
            //判断放左边库 还是放右边库？？？？？？？？？？？？？ 
            //A面和B面都有可用库位的情况
            if (countCanIn_A > 0 && countCanIn_B > 0)
            {//首先检查物料A、B面相差多少，保证左右两边立体库存放的物料数量基本一致
                int diffVal = countA - countB;
                if (diffVal <= 5 && diffVal >= -5)
                {//差值在-5和5之间，同侧入  
                 //rfidnum=1 A面入 ，rfidnum=2 B面入
                    return GetStationBySideBarcode(warehouseguid, mattercode, rfidnum, lstUseableShelves);
                }
                else if (diffVal > 5)
                {//入B面
                    return GetStationBySideBarcode(warehouseguid, mattercode, 2, lstUseableShelves);
                }
                else if (diffVal < -5)
                {
                    //入A面
                    return GetStationBySideBarcode(warehouseguid, mattercode, 1, lstUseableShelves);
                }
            }
            else if (countCanIn_A > 0)
            {
                // A面有可用库位,B面无可用库位（只能入A面）
                return GetStationBySideBarcode(warehouseguid, mattercode, 1, lstUseableShelves);
            }
            else if (countCanIn_B > 0)
            {
                // B面有可用库位，A面无可用库位（只能入B面）
                return GetStationBySideBarcode(warehouseguid, mattercode, 2, lstUseableShelves);
            }
            else
            {
                //都没有库位不分配

            }
            return 0;*/
        }

        /// <summary>
        /// 根据物料条码和存放的方位（side 1：A面，2：B面）
        /// </summary>
        /// <param name="matterbarcode"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private int GetStationBySideBarcode(Guid? warehouseguid, string matterbarcode, int side, List<wh_shelves> lstUseableShelves)
        {/*
            var _query = (from a in dbcontext.wh_shelfunits
                          where a.warehouse_guid == warehouseguid && a.matter_barcode == matterbarcode
                          && (side == 1 && a.rack <= 30 || side == 2 && a.rack > 30)
                          group a by new { a.rack } into g
                          select new
                          {
                              type = 0,//已占库位
                              rack = g.Key.rack,
                              count = g.Count()
                          }).Concat
                        (from a in dbcontext.wh_shelfunits
                         where a.warehouse_guid == warehouseguid && (side == 1 && a.rack <= 30 || side == 2 && a.rack > 30)
                         && a.state == LTWMSEFModel.EntityStatus.Normal
                           && a.cellstate == ShelfCellState.CanIn && a.locktype == ShelfLockType.Normal
                         group a by new { a.rack } into g
                         select new
                         {
                             type = 1,//可用库位
                             rack = g.Key.rack,
                             count = g.Count()
                         });
            /*  rack   已占库位      剩余库位
                 1      20            0
             union
                 1      0             15
             
             =>> 1      20            15
                orderby 已占库位升序 orderby rack升序   剩余库位 > 2
              * /
            var ListArr = _query.AsNoTracking().ToList();
            if (ListArr != null && ListArr.Count > 0)
            {
                List<RackInfoCount> lstRackInfo = new List<RackInfoCount>();
                foreach (var item in ListArr)
                {
                    var rackinfoObj = lstRackInfo.FirstOrDefault(w => w.rack == item.rack);
                    if (rackinfoObj == null)
                    {//添加
                        if (item.type == 0)
                        {//已占库位
                            lstRackInfo.Add(new RackInfoCount() { rack = item.rack, Used = item.count, Last = 0 });
                        }
                        else
                        {//可用库位
                            lstRackInfo.Add(new RackInfoCount() { rack = item.rack, Used = 0, Last = item.count });
                        }
                    }
                    else
                    {
                        //修改
                        if (item.type == 0)
                        {//已占库位
                            rackinfoObj.Used = item.count;
                        }
                        else
                        {//可用库位
                            rackinfoObj.Last = item.count;
                        }
                    }

                }
                //每排预留1个库位，总共预留60*1个库位
                var _objR = lstRackInfo.Where(w => w.Last > 1).OrderBy(o => o.Used).ThenBy(o => o.rack).FirstOrDefault();
                if (_objR != null)
                {//通过货架排 获取对应的入库站台编号
                    return bll_wh_shelves.GetStationByRack(_objR.rack, warehouseguid);
                    // bll_wh_wcs_device.GetFirstDefault(w=>w.nu)
                    //return _objR.rack;
                }
            }
            //按货架排分组？
            / * 按物料存放均分至立体库
             立库剩余容量？
             。。。。
             根据产品的段类查找库位，如果库位不足，正常的料筐可以入大库位
             库位不足提醒，提醒处理未及时，入大立库，大立库也存满了，只能堵线体等待了。。。。。* /
            */
            return 0;
        }

        /// <summary>
        /// 将任务添加至历史记录表中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue AddToHistory(hdw_stacker_taskqueue item)
        {
            var _taskqueue_history = new hdw_stacker_taskqueue_his();
            _taskqueue_history.createdate = item.createdate;
            _taskqueue_history.dest_shelfunits_guid = item.dest_shelfunits_guid;
            _taskqueue_history.createuser = item.createuser;
            _taskqueue_history.dest_col = item.dest_col;
            _taskqueue_history.dest_rack = item.dest_rack;
            _taskqueue_history.dest_row = item.dest_row;
            _taskqueue_history.dest_station = item.dest_station;
            _taskqueue_history.endup = item.endup;
            _taskqueue_history.guid = Guid.NewGuid();
            _taskqueue_history.is_emptypallet = item.is_emptypallet;
            _taskqueue_history.memo = item.memo;
            _taskqueue_history.order = item.order;
            _taskqueue_history.rowversion = item.rowversion;
            _taskqueue_history.shelves_guid = item.shelves_guid;
            _taskqueue_history.src_shelfunits_guid = item.src_shelfunits_guid;
            _taskqueue_history.taskqueue_id = item.id;
            _taskqueue_history.taskqueue_guid = item.guid;
            _taskqueue_history.tray1_matter_barcode1 = item.tray1_matter_barcode1;
            _taskqueue_history.tray1_matter_barcode2 = item.tray1_matter_barcode2;
            _taskqueue_history.tray2_matter_barcode1 = item.tray2_matter_barcode1;
            _taskqueue_history.tray2_matter_barcode2 = item.tray2_matter_barcode2;
            _taskqueue_history.updateuser = item.updateuser;
            _taskqueue_history.sort = item.sort;
            _taskqueue_history.src_col = item.src_col;
            _taskqueue_history.src_rack = item.src_rack;
            _taskqueue_history.src_row = item.src_row;
            _taskqueue_history.src_station = item.src_station;
            _taskqueue_history.startup = item.startup;
            _taskqueue_history.state = item.state;
            _taskqueue_history.taskstatus = item.taskstatus;
            _taskqueue_history.tasktype = item.tasktype;
            _taskqueue_history.tray1_barcode = item.tray1_barcode;
            _taskqueue_history.tray2_barcode = item.tray2_barcode;
            _taskqueue_history.updatedate = item.updatedate;
            _taskqueue_history.warehouse_guid = item.warehouse_guid;
            _taskqueue_history.src_shelfunits_pos = item.src_shelfunits_pos;
            _taskqueue_history.dest_shelfunits_pos = item.dest_shelfunits_pos;
            _taskqueue_history.matterbarcode_list = item.matterbarcode_list;
            _taskqueue_history.re_detail_traymatter_guid = item.re_detail_traymatter_guid;
            _taskqueue_history.bills_type = item.bills_type;
            return bll_hdw_stacker_taskqueue_his.Add(_taskqueue_history);
        }
    }

    /// <summary>
    /// 检查阻挡库位返回类型
    /// </summary>
    public class CheckAddMoveReturnType
    {
        public CheckBlockResultEnum checkResult { get; set; }
        /// <summary>
        /// 出库或移库任务
        /// </summary>
        public hdw_stacker_taskqueue? stacker_queue { get; set; }
    }
    public enum CheckBlockResultEnum
    {
        /// <summary>
        /// 被阻挡的库位可入/出库
        /// </summary>
        CanInOut = 0,
        /// <summary>
        /// 被阻挡的库位不可入/出库
        /// </summary>
        Blocked = 1
    }
}
