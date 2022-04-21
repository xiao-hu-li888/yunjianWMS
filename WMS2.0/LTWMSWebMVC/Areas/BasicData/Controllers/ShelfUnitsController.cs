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

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class ShelfUnitsController : BaseController
    {
        wh_shelfunitsBLL bll_shelfunits;
        hdw_stacker_taskqueueBLL bll_stacker_taskqueue;
        wh_trayBLL bll_wh_tray;
        public ShelfUnitsController(wh_shelfunitsBLL bll_shelfunits, LTWMSEFModel.LTModel dbmodel, hdw_stacker_taskqueueBLL bll_stacker_taskqueue,
            wh_trayBLL bll_wh_tray)
        {
            this.bll_shelfunits = bll_shelfunits;
            this.bll_stacker_taskqueue = bll_stacker_taskqueue;
            this.bll_wh_tray = bll_wh_tray;
        }
        // GET: BasicData/ShelfUnits
        public ActionResult Index(ShelfUnitsSearch Model)
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            int TotalSize = 0;
            var aa = bll_shelfunits.Pagination(Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize, o => o.createdate,
               w => w.warehouse_guid == warehouseguid && (Model.s_keywords == "" || (w.depth1_traybarcode ?? "").Equals(Model.s_keywords)
               || (w.shelfunits_pos ?? "").Equals(Model.s_keywords))
                && (Model.rack == null || w.rack == Model.rack)
                 && (Model.column == null || w.columns == Model.column)
                 && (Model.row == null || w.rows == Model.row)
                 && ((Model.cell_state == null || (int)Model.cell_state.Value == -1) || w.cellstate == Model.cell_state)
                  && ((Model.lock_type == null || (int)Model.lock_type.Value == -1) || w.locktype == Model.lock_type)
                   && ((Model.special_lock_type == null || (int)Model.special_lock_type.Value == -1) || w.special_lock_type == Model.special_lock_type)
                 , true);
            /*var aa = bll_shelfunits.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.id,
                w => (Model.s_keywords == "" || (w.depth1_traybarcode ?? "").Contains(Model.s_keywords)
                || (w.shelfunits_pos ?? "").Contains(Model.s_keywords))
                  && (Model.rack == null || w.rack == Model.rack)
                  && (Model.column == null || w.columns == Model.column)
                  && (Model.row == null || w.rows == Model.row)
                  && ((Model.cell_state == null || (int)Model.cell_state.Value == -1) || w.cellstate == Model.cell_state)
                  && ((Model.lock_type == null || (int)Model.lock_type.Value == -1) || w.locktype == Model.lock_type)
                        , false);*/
            Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };

            return View(Model);
        }
        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            var model = bll_shelfunits.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Update", Md);
        }
        [HttpPost]
        public JsonResult Update(ShelfUnitsModel model)
        {
            try
            {
                var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                if (info != null)
                {
                    string _oldtray = info.depth1_traybarcode;
                    if (info.cellstate != ShelfCellState.Stored)
                    {
                        AddJsonError("非存储状态不能修改托盘条码");
                        return JsonError();
                    }
                    if (string.IsNullOrWhiteSpace(model.depth1_traybarcode))
                    {
                        AddJsonError("托盘条码不能为空");
                        return JsonError();
                    }
                    info.updatedate = DateTime.Now;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.depth1_traybarcode = model.depth1_traybarcode;
                    //并发控制（乐观锁）
                    info.OldRowVersion = model.OldRowVersion;
                    //var rtv = bll_shelfunits.Update(info);
                    //判断库位是否存在相同的托盘条码
                    var rtv = bll_shelfunits.UpdateIfNotExists(info, w => w.depth1_traybarcode);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("将仓位信息guid=[" + info.guid + "] " + info.rack + "排/" + info.columns + "列/" + info.rows + "层 托盘条码由[" + _oldtray + "]修改为:[" + info.depth1_traybarcode + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("修改失败，库位已存在相同的托盘条码:" + model.depth1_traybarcode);
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
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }


        [HttpPost]
        public JsonResult Disable(ShelfUnitsModel model)
        {
            try
            {
                var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                if (info != null)
                {
                    info.updatedate = DateTime.Now;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;

                    if (info.state == LTWMSEFModel.EntityStatus.Normal)
                    {//正常库位禁用，如果有出入库任务，则不能进行操作
                        if (info.cellstate == ShelfCellState.CanIn)
                        {
                            info.state = LTWMSEFModel.EntityStatus.Disabled;
                        }
                        else
                        {
                            AddJsonError("只有空库位（可入库状态）才能禁用");
                            return JsonError();
                        }
                    }
                    else if (info.state == LTWMSEFModel.EntityStatus.Disabled)
                    {//禁用的库位可以直接启用
                        info.state = LTWMSEFModel.EntityStatus.Normal;
                    }
                    else
                    {
                        AddJsonError("删除的库位，不能执行该操作！");
                        return JsonError();
                    }
                    //并发控制（乐观锁）
                    info.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_shelfunits.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        string str = info.state == LTWMSEFModel.EntityStatus.Normal ? "启用" : "禁用";
                        AddUserOperationLog("将仓位信息guid=[" + info.guid + "] 【" + info.shelfunits_pos + "】 库位状态修改为:[" + str + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        [HttpPost]
        public JsonResult Lock(ShelfUnitsModel model)
        {
            try
            {
                var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                if (info != null)
                {
                    info.updatedate = DateTime.Now;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;

                    if (info.locktype == ShelfLockType.ManLock)
                    {//人工锁可以直接解锁
                        info.locktype = ShelfLockType.Normal;
                    }
                    else if (info.locktype == ShelfLockType.Normal)
                    {//正常可入库状态，锁定库位
                        info.locktype = ShelfLockType.ManLock;
                    }
                    else if (info.locktype == ShelfLockType.SysLock)
                    {//系统锁，则不能锁定和解锁
                        AddJsonError("系统锁状态不能执行锁定和解锁操作！");
                        return JsonError();
                    }
                    //并发控制（乐观锁）
                    info.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_shelfunits.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        string str = info.locktype == ShelfLockType.ManLock ? "人工锁" : "正常";
                        AddUserOperationLog("将仓位信息guid=[" + info.guid + "] 库位锁定状态修改为:[" + str + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }

        [HttpPost]
        public JsonResult Reset(ShelfUnitsModel model)
        {
            try
            {
                using (var _tran = bll_shelfunits.BeginTransaction())
                {
                    try
                    {
                        var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                        if (info != null)
                        {
                            //将库位绑定的托盘修改为：未上架
                            // var rtv2 = bll_wh_tray.DisConnectedALLMatter(info.depth1_traybarcode);
                            var rtv2 = bll_wh_tray.DeleteTrayInfoAndMatterDetails(info.depth1_traybarcode);
                            info.updatedate = DateTime.Now;
                            info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                            //库位重置代码。。。。
                            //重置库位信息
                            info.cellstate = ShelfCellState.CanIn;
                            info.locktype = ShelfLockType.ManLock;
                            info.depth1_traybarcode = "";
                            info.depth2_traybarcode = "";
                            info.tray_indatetime = null;
                            info.tray_outdatetime = DateTime.Now;
                            //删除对应库位的任务
                            //bll_stacker_taskqueue.Delete(w =>
                            //(w.dest_rack == info.rack && w.dest_col == info.columns && w.dest_row == info.rows)
                            //|| (w.src_rack == info.rack && w.src_col == info.columns && w.src_row == info.rows));
                            LTWMSEFModel.SimpleBackValue rtvdel = LTWMSEFModel.SimpleBackValue.False;
                            var dellist = bll_stacker_taskqueue.GetAllQuery(w =>
                             (w.dest_rack == info.rack && w.dest_col == info.columns && w.dest_row == info.rows)
                             || (w.src_rack == info.rack && w.src_col == info.columns && w.src_row == info.rows));
                            if (dellist != null && dellist.Count > 0)
                            {
                                foreach (var item in dellist)
                                {
                                    rtvdel = bll_stacker_taskqueue.Delete(item);
                                    if (rtvdel == LTWMSEFModel.SimpleBackValue.False)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {//没有历史遗留，默认true
                                rtvdel = LTWMSEFModel.SimpleBackValue.True;
                            }
                            info.special_lock_type = SpecialLockTypeEnum.Normal;
                            //并发控制（乐观锁）
                            info.OldRowVersion = model.OldRowVersion;
                            var rtv = bll_shelfunits.Update(info);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True && rtvdel == LTWMSEFModel.SimpleBackValue.True
                                && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {
                                _tran.Commit();
                                AddUserOperationLog("仓位信息guid=[" + info.guid + "] 对库位[" + info.shelfunits_pos + "] 进行库位重置操作。", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                        // _tran.Rollback();
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
        [HttpPost]
        public JsonResult ResetDispatchLock(ShelfUnitsModel model)
        {
            try
            {
                using (var _tran = bll_shelfunits.BeginTransaction())
                {
                    try
                    {
                        var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                        if (info != null && info.guid != Guid.Empty)
                        {
                            LTWMSEFModel.SimpleBackValue rtv2 = LTWMSEFModel.SimpleBackValue.False;
                            //修改对应托盘表
                            var trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == info.depth1_traybarcode);
                            if (trayM != null && trayM.guid != Guid.Empty)
                            {//清除对应的指定标记
                                trayM.dispatch_shelfunits_guid = null;
                                trayM.dispatch_shelfunits_pos = "";
                                trayM.OldRowVersion = trayM.rowversion;
                                rtv2 = bll_wh_tray.Update(trayM);
                            }
                            else
                            {
                                rtv2 = LTWMSEFModel.SimpleBackValue.True;
                            }
                            /************************************/
                            info.special_lock_type = SpecialLockTypeEnum.Normal;
                            //并发控制（乐观锁）
                            info.OldRowVersion = model.OldRowVersion;
                            var rtv = bll_shelfunits.Update(info);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True &&
                                rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {
                                _tran.Commit();
                                AddUserOperationLog("仓位信息guid=[" + info.guid + "] 对库位[" + info.shelfunits_pos + "] 进行清除指定入库标记操作。", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                return JsonSuccess();
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException
                                || rtv2 == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                            }
                            else
                            {
                                AddJsonError("保存失败，请重试！");
                            }
                        }
                        else
                        {
                            AddJsonError("数据库中不存在该条记录或已删除！");
                        }

                    }
                    catch (Exception ex)
                    {
                        // _tran.Rollback();
                        WMSFactory.Log.v(ex);
                        AddJsonError("异常：" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        [HttpPost]
        public JsonResult ResetStockOutLock(ShelfUnitsModel model)
        {
            try
            { 
                var info = bll_shelfunits.GetFirstDefault(w => w.guid == model.guid);
                if (info != null && info.guid != Guid.Empty)
                {
                    info.special_lock_type = SpecialLockTypeEnum.Normal;
                    //并发控制（乐观锁）
                    info.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_shelfunits.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    { 
                        AddUserOperationLog("仓位信息guid=[" + info.guid + "] 对库位[" + info.shelfunits_pos + "] 进行出库锁定标记清除操作。", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                    }
                    else
                    {
                        AddJsonError("保存失败，请重试！");
                    }  
                }
                else
                {
                    AddJsonError("数据库中不存在该条记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
    }
}