using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSWebMVC.Areas.RealTime.Data;
using LTWMSEFModel.Warehouse;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Stock;
using LTWMSEFModel.Stock;

namespace LTWMSWebMVC.Areas.RealTime.Controllers
{
    public class ShelfStockController : BaseController
    {
        wh_shelfunitsBLL bll_shelfunits;
        wh_shelvesBLL bll_shelves;
        hdw_stacker_taskqueueBLL bll_stacker_taskqueue;
        wh_trayBLL bll_wh_tray;
        wh_traymatterBLL bll_wh_traymatter;
        stk_matterBLL bll_stk_matter;

        public ShelfStockController(wh_shelfunitsBLL bll_shelfunits, wh_shelvesBLL bll_shelves,
            hdw_stacker_taskqueueBLL bll_stacker_taskqueue, wh_trayBLL bll_wh_tray, wh_traymatterBLL bll_wh_traymatter, stk_matterBLL bll_stk_matter)
        {
            this.bll_shelfunits = bll_shelfunits;
            this.bll_shelves = bll_shelves;
            this.bll_stacker_taskqueue = bll_stacker_taskqueue;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_stk_matter = bll_stk_matter;
        }
        // GET: RealTime/ShelfStock
        public ActionResult Index()
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            List<wh_shelves> lstShelves = bll_shelves.GetAllQueryOrderby(o => o.rack, w => w.warehouse_guid == warehouseguid);
            if (lstShelves == null)
                return ErrorView("货架信息为空");
            ShelfUnitsList Mod = new ShelfUnitsList();
            Mod.Top = new ShelfUnitsTop();
            //获取统计信息 
            var list_shelfunit = bll_shelfunits.GetAllQuery(w => w.warehouse_guid == warehouseguid && w.state != LTWMSEFModel.EntityStatus.Deleted);
            Mod.Shelves = lstShelves.Select(a => MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(a)).ToList();
            foreach (var item in Mod.Shelves)
            {
                item.ShelfUnits = list_shelfunit.Where(w => w.shelves_guid == item.guid).Select(a =>
                // MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a)
                {
                    var ShelfU = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a);
                    if (ShelfU.cellstate == ShelfCellState.Stored)
                    {//存储中的库位查找 储存类型
                     //    ShelfU.matterType = bll_shelfunits.getStoredMatterType(a);
                        ShelfU.matterType = LTWMSEFModel.MatterTypeEnum.Matter;
                    }
                    return ShelfU;
                }
                ).ToList();
            }
            return View(Mod);
        }
        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            var model = bll_shelfunits.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(model);
            Md.OldRowVersion = model.rowversion;
            if (string.IsNullOrWhiteSpace(Md.depth1_traybarcode))
            {//托盘条码为空
                Md.trayModel = new TrayModel() { emptypallet = true };
            }
            else
            {
                Md.trayModel = MapperConfig.Mapper.Map<wh_tray, TrayModel>(bll_wh_tray.GetFirstDefault(w => w.traybarcode == Md.depth1_traybarcode));
                if (Md.trayModel == null)
                {
                    Md.trayModel = new TrayModel() { emptypallet = true };
                }
                else
                {
                    Md.trayModel.traymatterList = bll_wh_traymatter.GetAllQueryOrderby(o => o.createdate, w => w.tray_guid == Md.trayModel.guid, true)
                     .Select(s =>
                     {
                         var a = MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s);
                         //a.MatterModel = MapperConfig.Mapper.Map<stk_matter, MatterModel>(bll_stk_matter.GetFirstDefault(w => w.code == a.x_barcode));
                         return a;
                     }).ToList();
                }
            }
            return PartialView("Update", Md);
        }
        [HttpPost]
        public JsonResult Update(ShelfUnitsModel model)
        {
            try
            {
                using (var _tran = bll_shelfunits.BeginTransaction())
                {
                    try
                    {
                        Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                        var info = bll_shelfunits.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.guid == model.guid);
                        if (info != null && info.guid != Guid.Empty)
                        {
                            if (info.state == LTWMSEFModel.EntityStatus.Disabled)
                            {
                                _tran.Rollback();
                                AddJsonError("操作失败！该库位已禁用。");
                                return JsonError();
                            }
                            info.updatedate = DateTime.Now;
                            info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                            //强制出库  
                            if (info.cellstate == ShelfCellState.Stored
                            || info.locktype == ShelfLockType.ManLock)
                            {//存储状态、人工锁状态 强制出库 
                                info.cellstate = ShelfCellState.WaitOut;//强制出库 
                                info.locktype = ShelfLockType.SysLock;
                                info.tray_outdatetime = DateTime.Now;
                                // 生成对应的出库任务
                                //  var rtv3 = bll_stacker_taskqueue.CreateTaskByShelfUnits(info);
                                var rtv3 = bll_stacker_taskqueue.CreateTaskByShelfUnitsWithSort(info, 999);
                                if (rtv3 != LTWMSEFModel.SimpleBackValue.True)
                                {
                                    _tran.Rollback();
                                    AddJsonError("添加出库任务出错，请重试！");
                                    return JsonError();
                                }
                            }
                            else if (info.cellstate == ShelfCellState.WaitOut)
                            {//如果是出库则设置优先级
                             //将对应的出库任务优先级设置为最大 100
                             //var _task_out = bll_stacker_taskqueue.GetFirstDefault(w =>w.warehouse_guid== warehouseguid && w.src_rack == info.rack
                             //&& w.src_col == info.columns && w.src_row == info.rows);
                                var _task_out = bll_stacker_taskqueue.GetFirstDefault(w => w.warehouse_guid == warehouseguid
                                &&w.tasktype== LTWMSEFModel.Hardware.WcsTaskType.StockOut&& w.src_shelfunits_guid == info.guid);
                                if (_task_out != null)
                                {
                                    //查询到出库任务
                                    _task_out.sort = 999;
                                    bll_stacker_taskqueue.Update(_task_out);
                                }
                            }
                            else
                            {
                                string _cellTypeStr = "";
                                if (info.cellstate == ShelfCellState.CanIn)
                                {
                                    _cellTypeStr = "可入库";
                                }
                                else if (info.cellstate == ShelfCellState.TrayIn)
                                {
                                    _cellTypeStr = "入库中";
                                }
                                else if (info.cellstate == ShelfCellState.TrayOut)
                                {
                                    _cellTypeStr = "出库中";
                                }
                                _tran.Rollback();
                                AddJsonError("该库位状态为\"" + _cellTypeStr + "\"，不能进行该操作！(人工锁状态下可以强制出库)");
                                return JsonError();
                            }
                            //并发控制（乐观锁）
                            info.OldRowVersion = model.OldRowVersion;
                            var rtv = bll_shelfunits.Update(info);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                AddUserOperationLog("对库位：[" + info.shelfunits_pos + "] guid=[" + info.guid + "]进行强制出库操作", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                _tran.Commit();
                                return JsonSuccess();
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                            }
                            else
                            {
                                AddJsonError("保存失败");
                            }
                        }
                        else
                        {
                            AddJsonError("数据库中不存在该条记录或已删除！");
                        }
                    }
                    catch (Exception ex)
                    {
                        WMSFactory.Log.v(ex);
                        AddJsonError("异常：" + ex.ToString());
                    }
                    _tran.Rollback();
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        [HttpGet]
        public ActionResult GetRealTimeShelfStock(Guid guid)
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            wh_shelves shelvesObj = bll_shelves.GetFirstDefault(o => o.warehouse_guid == warehouseguid && o.guid == guid);
            if (shelvesObj == null)
                return ErrorView("货架信息为空");
            var list_shelfunit = bll_shelfunits.GetAllQuery(w => w.warehouse_guid == warehouseguid && w.state != LTWMSEFModel.EntityStatus.Deleted && w.shelves_guid == guid);
            var shelvesMod = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(shelvesObj);
            shelvesMod.ShelfUnits = list_shelfunit.Select(a =>
            {
                //MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a)
                var ShelfU = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a);
                if (ShelfU.cellstate == ShelfCellState.Stored)
                {//存储中的库位查找 储存类型
                 //ShelfU.matterType = bll_shelfunits.getStoredMatterType(a);
                    ShelfU.matterType = LTWMSEFModel.MatterTypeEnum.Matter;
                }
                return ShelfU;
            }).ToList();

            return PartialView("CellTable", shelvesMod);
        }
        [HttpGet]
        public ActionResult GetTop()
        {
            ShelfUnitsTop Md = new ShelfUnitsTop();
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            Md.Free = bll_shelfunits.GetCount(w => w.warehouse_guid == warehouseguid && w.cellstate == ShelfCellState.CanIn && w.state == LTWMSEFModel.EntityStatus.Normal);
            Md.Total = bll_shelfunits.GetCount(w => w.warehouse_guid == warehouseguid && w.state != LTWMSEFModel.EntityStatus.Deleted);
            Md.Used = Md.Total - Md.Free;

            int _batter = bll_shelfunits.GetShelfUnitCountOfBatter(warehouseguid);
            int _other_matter = bll_shelfunits.GetShelfUnitCountOfOther(warehouseguid);
            int _emptyCout = bll_shelfunits.GetShelfUnitCountOfEmpty(warehouseguid);
            Md.BatteryCount = _batter;
            Md.OtherMatterCout = _other_matter;
            Md.EmptyCout = _emptyCout;
            return PartialView("Top", Md);
        }
    }
}