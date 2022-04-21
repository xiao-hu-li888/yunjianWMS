using LTWMSWebMVC.Areas.RealTime.Data;
using LTWMSEFModel.Hardware;
using LTWMSService.Basic;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using LTWMSService.ApplicationService.StockInOut;
using LTWMSEFModel.Warehouse;

namespace LTWMSWebMVC.Areas.RealTime.Controllers
{
    public class RgvTaskController : BaseController
    {
        hdw_stacker_taskqueueBLL bll_stacker_taskqueue;
        sys_control_dicBLL bll_control_dic;
        hdw_plcBLL bll_hdw_plc;
        wh_shelfunitsBLL bll_wh_shelfunits;
        TaskQueueService bll_TaskQueueService;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        public RgvTaskController(hdw_stacker_taskqueueBLL bll_stacker_taskqueue, sys_control_dicBLL bll_control_dic, hdw_plcBLL bll_hdw_plc,
            wh_shelfunitsBLL bll_wh_shelfunits, TaskQueueService bll_TaskQueueService, wh_wcs_srvBLL bll_wh_wcs_srv, wh_wcs_deviceBLL bll_wh_wcs_device)
        {
            this.bll_stacker_taskqueue = bll_stacker_taskqueue;
            this.bll_control_dic = bll_control_dic;
            this.bll_hdw_plc = bll_hdw_plc;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_TaskQueueService = bll_TaskQueueService;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
        }
        // GET: RealTime/RgvTask
        public ActionResult Index()
        {
            //初始化下拉数据 
            /******************/
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            StackerTaskQueueSearch stackqueueModel = new StackerTaskQueueSearch();
            stackqueueModel.IsSendToStacker = bll_control_dic.GetValueByType(CommDictType.SendToStacker, warehouseguid) == "1" ? true : false;
            //正式
            stackqueueModel.PageCont = bll_stacker_taskqueue.GetRunningTaskList(warehouseguid).Select(w =>
             MapperConfig.Mapper.Map<hdw_stacker_taskqueue, StackerTaskQueueModel>(w)).ToList();

            return View(stackqueueModel);
            // 
            /* //测试  
            stackqueueModel.PageCont = bll_stacker_taskqueue.GetAllQuery().Select(w =>
          MapperConfig.Mapper.Map<hdw_stacker_taskqueue, StackerTaskQueueModel>(w)).ToList(); 
            return View(stackqueueModel);  */
        }
        public ActionResult WaiteOutIndex(WaiteOutSearch Model)
        {
            //Model.Paging.PageSize = 100;
            Guid _warehouseguid = GetCurrentLoginUser_WareGuid();
            int TotalSize = 0;
            Model.PageCont = bll_stacker_taskqueue.GetALLWaitedTaskOut(Model.s_keywords, Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, _warehouseguid)
                .Select(s => MapperConfig.Mapper.Map<hdw_stacker_taskqueue, StackerTaskQueueModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        public ActionResult StackTaskRealtimeList()
        {
            Guid _warehouseguid = GetCurrentLoginUser_WareGuid();
            var Md = bll_stacker_taskqueue.GetRunningTaskList(_warehouseguid).Select(w =>
                  MapperConfig.Mapper.Map<hdw_stacker_taskqueue, StackerTaskQueueModel>(w)).ToList();
            return PartialView("ViewList", Md);
        }
        public ActionResult StatusOfDev()
        {
            RgvListStatusData Md = new RgvListStatusData();
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            Md.WaitInTaskCount = bll_stacker_taskqueue.GetCount(w => w.warehouse_guid == warehouseguid && w.tasktype == WcsTaskType.StockIn);
            Md.WaitOutTaskCount = bll_stacker_taskqueue.GetCount(w => w.warehouse_guid == warehouseguid && w.tasktype == WcsTaskType.StockOut && w.taskstatus != WcsTaskStatus.Canceled);
            /* var _pclList = bll_hdw_plc.GetAllQuery(w=>w.warehouse_guid== warehouseguid);
             if (_pclList != null && _pclList.Count > 0)
             {
                 Md.ListDeviceModel = _pclList.Select(s =>
                        MapperConfig.Mapper.Map<hdw_plc, Data.DevPlcModel>(s)).ToList(); 
             }*/
            //通过当前warehouse 查询对应关联的wcs
            var Model = bll_wh_wcs_srv.GetAllWcsSrvByWarehouseguid(warehouseguid);
            if (Model != null && Model.Count > 0)
            {
                var _Md = Model[0];
                List<DevPlcModel> ListPLC = bll_hdw_plc.GetAllQuery(w => w.warehouse_guid == warehouseguid
                && w.shvwcs_srv_guid == _Md.guid).Select(s =>
                  MapperConfig.Mapper.Map<hdw_plc, Data.DevPlcModel>(s)).ToList();
                var wcsSrvModel = MapperConfig.Mapper.Map<wh_wcs_srv, LTWMSWebMVC.Areas.Setting.Data.WcsSrvModel>(_Md);
                Md.ListDeviceModel = bll_wh_wcs_device.GetAllQueryOrderby(w => w.device_type, w => w.state == LTWMSEFModel.EntityStatus.Normal
                && w.warehouse_guid == warehouseguid && w.wcs_srv_guid == wcsSrvModel.guid, false).Select(s =>
                {
                    var wcsDevM = MapperConfig.Mapper.Map<wh_wcs_device, LTWMSWebMVC.Areas.Setting.Data.WcsDeviceModel>(s);
                    wcsDevM.DevPlcModel = ListPLC.Where(w => w.shvwcs_srv_guid == wcsDevM.wcs_srv_guid && w.number == wcsDevM.number).FirstOrDefault();
                    return wcsDevM;
                }).ToList();
            }
            return PartialView("StatusView", Md);
        }
        /// <summary>
        /// 获取发送停止按钮最新状态
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTopCheckView()
        {
            StackerTaskQueueSearch Md = new StackerTaskQueueSearch();
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            Md.IsSendToStacker = bll_control_dic.GetValueByType(CommDictType.SendToStacker, warehouseguid) == "1" ? true : false;

            return PartialView("TopCheckView", Md);
        }
        /// <summary>
        /// 是否发送任务至堆垛机
        /// </summary>
        /// <param name="issend"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetSendToStacker(int issend)
        {
            try
            {
                Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                //修改是否发送任务至堆垛机
                var rtv = bll_control_dic.SetValueByType(CommDictType.SendToStacker, issend == 1 ? "1" : "0", warehouseguid);
                //  int aa= bll_control_dic.GetCount();
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    string _sendStr = issend == 1 ? "下发至堆垛机" : "取消下发至堆垛机";
                    AddUserOperationLog("修改任务下发状态为：" + _sendStr, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    return JsonSuccess();
                }
                else
                {
                    AddJsonError("修改发送状态失败！错误代码:" + Enum.GetName(typeof(LTWMSEFModel.SimpleBackValue), rtv));
                }
            }
            catch (Exception ex)
            {
                AddJsonError("修改发送状态失败:" + ex.Message);
                //  services.WcsFactory.Log.v(ex);
            }
            return JsonError();
        }
        [HttpGet]
        public ActionResult Handle(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            //   ViewBag.SubmitText = "保存";
            // ViewBag.isUpdate = true;
            var model = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<hdw_stacker_taskqueue, StackerTaskQueueModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Handle", Md);
        }
        /// <summary>
        /// 取消或强制完成（方法已过时，弃用）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Handle(StackerTaskQueueModel model)
        {
            if (model.submit_type != 1 && model.submit_type != 2)
            {
                AddJsonError("参数错误。。。");
                return JsonError();
            }
            try
            {
                var info = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == model.guid);
                if (info != null)
                {
                    info.updatedate = DateTime.Now;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.memo = model.memo;
                    if (model.submit_type == 1)
                    {//取消
                        info.taskstatus = WcsTaskStatus.CancelHandling;
                    }
                    else if (model.submit_type == 2)
                    {//强制完成
                        info.taskstatus = WcsTaskStatus.ForceCompleteHandling;
                    }
                    //并发控制（乐观锁）
                    info.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_stacker_taskqueue.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        string handStr = "";
                        if (model.submit_type == 1)
                        {//取消
                            handStr = "取消任务";
                        }
                        else if (model.submit_type == 2)
                        {//强制完成
                            handStr = "强制完成";
                        }
                        AddUserOperationLog("将任务信息guid=[" + info.guid + "],id=[" + info.id + "] 完成状态修改为:" + handStr, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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

        /// <summary>
        /// 重发任务至堆垛机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReSendToWCS(StackerTaskQueueModel model)
        {
            try
            {
                hdw_stacker_taskqueue info = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == model.guid);
                //并发控制（乐观锁） 
                info.OldRowVersion = model.OldRowVersion;
                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                LTWMSService.ApplicationService.ComServiceReturn rtv = bll_TaskQueueService.ReSendToWCS(info);
                if (rtv.success)
                {//操作成功
                    AddUserOperationLog("重发任务[" + info.id + "]至WCS...托盘条码[" +
                        info.tray1_barcode + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    return JsonSuccess();
                }
                else
                {
                    AddJsonError(rtv.result);
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ForceComplete(StackerTaskQueueModel model)
        {
            try
            {
                hdw_stacker_taskqueue info = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == model.guid);
                //并发控制（乐观锁） 
                info.OldRowVersion = model.OldRowVersion;
                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                LTWMSService.ApplicationService.ComServiceReturn rtv = bll_TaskQueueService.ForceComplete(info);
                if (rtv.success)
                {//操作成功
                    AddUserOperationLog("强制完成任务[" + info.id + "] 托盘条码[" +
                        info.tray1_barcode + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    return JsonSuccess();
                }
                else
                {
                    AddJsonError(rtv.result);
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TaskCancel(StackerTaskQueueModel model)
        {
            try
            {
                hdw_stacker_taskqueue info = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == model.guid);
                if (info != null && info.guid != Guid.Empty)
                {//并发控制（乐观锁） 
                    info.OldRowVersion = model.OldRowVersion;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    LTWMSService.ApplicationService.ComServiceReturn rtv = bll_TaskQueueService.TaskCancel(info);
                    if (rtv.success)
                    {//操作成功
                        AddUserOperationLog("取消任务[" + info.id + "] 托盘条码[" +
                            info.tray1_barcode + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else
                    {
                        AddJsonError(rtv.result);
                    }
                }
                else
                {
                    AddJsonError("数据库中不存在该条记录或已删除。");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 移库异常出库操作（同时清空起点、终点库位数据并锁定）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult TaskMoveTrayOut(StackerTaskQueueModel model)
        {
            try
            {
                hdw_stacker_taskqueue info = bll_stacker_taskqueue.GetFirstDefault(w => w.guid == model.guid);
                if (info != null && info.guid != Guid.Empty)
                {
                    //并发控制（乐观锁） 
                    info.OldRowVersion = model.OldRowVersion;
                    info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    //using (var _tran = bll_stacker_taskqueue.BeginTransaction())
                    //{
                        LTWMSService.ApplicationService.ComServiceReturn rtv = bll_TaskQueueService.TaskMoveTrayOut(info);
                        if (rtv.success)
                        {//操作成功
                            AddUserOperationLog("操作移库异常出库按钮[" + info.id + "] 托盘条码[" +
                                info.tray1_barcode + "]，启动库位" + info.src_shelfunits_pos + "-终点库位" +
                                info.dest_shelfunits_pos + "已锁定，请及时解锁库位！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                          //  _tran.Commit();
                            return JsonSuccess();
                        }
                        else
                        {
                            AddJsonError(rtv.result);
                        }
                    //}
                }
                else
                {
                    AddJsonError("数据库中不存在该条记录或已删除。");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 批量取消任务(只能取消未执行的任务)
        /// </summary>
        /// <param name="guidstrs"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CancelTaskList(string guidstr)
        {
            try
            {
                List<Guid> lstguids = LTLibrary.ConvertUtility.ParseToGuids(guidstr);
                if (lstguids != null && lstguids.Count > 0)
                {
                    Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                    var ListHoldingTasks = bll_stacker_taskqueue.GetAllQuery(w => w.warehouse_guid == warehouseguid && lstguids.Contains(w.guid));
                    if (ListHoldingTasks != null && ListHoldingTasks.Count > 0)
                    {
                        string _issendtostacker = bll_control_dic.GetValueByType(CommDictType.SendToStacker, warehouseguid);
                        //暂停发送任务至堆垛机 
                        bll_control_dic.SetValueByType(CommDictType.SendToStacker, "0", warehouseguid);
                        //休息2秒等待发送任务线程接收到停止发送指令 
                        Thread.Sleep(2000);
                        bool _flag = false;
                        using (var tran = bll_stacker_taskqueue.BeginTransaction())
                        {
                            try
                            {
                                foreach (var item in ListHoldingTasks)
                                {
                                    item.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.Canceled;
                                    item.updatedate = DateTime.Now;
                                    item.memo += ";[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]["
                                        + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]操作批量任务取消！";
                                    //出库取消后对库位的操作
                                    bll_wh_shelfunits.StockOutCanceledHandler(item);
                                }
                                var rtv = bll_stacker_taskqueue.Update(ListHoldingTasks);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    tran.Commit();
                                    _flag = true;
                                }
                                else
                                {
                                    AddJsonError("修改失败！");
                                    tran.Rollback();
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                AddJsonError("操作失败！事务回滚。>>>" + ex.ToString());
                                WMSFactory.Log.v(ex);
                            }
                        }
                        //恢复之前发送指令
                        bll_control_dic.SetValueByType(CommDictType.SendToStacker, _issendtostacker, warehouseguid);
                        if (_flag)
                        {
                            AddUserOperationLog("批量操作取消任务执行成功！ 任务id>>>"
                                + string.Join(",",ListHoldingTasks.Select(s=>s.id).ToArray()), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                    }
                    else
                    {
                        AddJsonError("操作失败！没有查找到对应的出库任务");
                    }
                }
                else
                {
                    AddJsonError("参数错误！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 设置优先级（只能设置未执行的任务）
        /// </summary>
        /// <param name="guidstrs"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetPriorityTaskList(string guidstr)
        {
            try
            {
                List<Guid> lstguids = LTLibrary.ConvertUtility.ParseToGuids(guidstr);
                if (lstguids != null && lstguids.Count > 0)
                {
                    Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                    var ListHoldingTasks = bll_stacker_taskqueue.GetAllQuery(w => w.warehouse_guid == warehouseguid && lstguids.Contains(w.guid));
                    if (ListHoldingTasks != null && ListHoldingTasks.Count > 0)
                    {
                        int _maxsort = bll_stacker_taskqueue.GetMaxSort(warehouseguid);
                        if (_maxsort < 999)
                        {
                            _maxsort = 999;
                        }
                        int _currSort = _maxsort + ListHoldingTasks.Count;
                        string _issendtostacker = bll_control_dic.GetValueByType(CommDictType.SendToStacker, warehouseguid);
                        //暂停发送任务至堆垛机 
                        bll_control_dic.SetValueByType(CommDictType.SendToStacker, "0", warehouseguid);
                        //休息2秒等待发送任务线程接收到停止发送指令 
                        Thread.Sleep(2000);
                        bool _flag = false;
                        using (var tran = bll_stacker_taskqueue.BeginTransaction())
                        {
                            try
                            {
                                foreach (var item in ListHoldingTasks)
                                {
                                    // item.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.Canceled; 
                                    item.updatedate = DateTime.Now;
                                    item.memo += ";[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]["
                                        + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]设置优先级 " + item.sort + "->>" + _currSort + "！";
                                    item.sort = _currSort;
                                    ////出库取消后对库位的操作
                                    //bll_wh_shelfunits.StockOutCanceledHandler(item);
                                    _currSort--;
                                }
                                var rtv = bll_stacker_taskqueue.Update(ListHoldingTasks);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    tran.Commit();
                                    _flag = true;
                                }
                                else
                                {
                                    AddJsonError("修改失败！");
                                    //修改失败
                                    tran.Rollback();
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                AddJsonError("操作失败！事务回滚。>>>" + ex.ToString());
                                WMSFactory.Log.v(ex);
                            }
                        }
                        //恢复之前发送指令
                        bll_control_dic.SetValueByType(CommDictType.SendToStacker, _issendtostacker, warehouseguid);
                        if (_flag)
                        {
                            AddUserOperationLog("批量设置优先级执行成功！ 任务id>>>"
                                + string.Join(",", ListHoldingTasks.Select(s => s.id).ToArray()), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                    }
                    else
                    {
                        AddJsonError("操作失败！没有查找到对应的出库任务");
                    }
                }
                else
                {
                    AddJsonError("参数错误！");
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