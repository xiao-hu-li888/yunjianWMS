using LTWMSWebMVC.Areas.Admin.Models;
using LTWMSEFModel.Warehouse;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSWebMVC.Areas.Setting.Data;
using LTWMSService.Basic;

namespace LTWMSWebMVC.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        LTWMSService.Basic.sys_loginBLL bll_login;
        wh_service_statusBLL bll_wh_service_status;
        wh_shelfunitsBLL bll_shelfunits;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        sys_control_dicBLL bll_sys_control_dic;
        public HomeController(LTWMSService.Basic.sys_loginBLL bll_login, wh_service_statusBLL bll_wh_service_status, wh_shelfunitsBLL bll_shelfunits,
            hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his, wh_warehouse_typeBLL bll_wh_warehouse_type, wh_warehouseBLL bll_wh_warehouse,
            wh_wcs_srvBLL bll_wh_wcs_srv, sys_control_dicBLL bll_sys_control_dic)
        {
            this.bll_login = bll_login;
            this.bll_wh_service_status = bll_wh_service_status;
            this.bll_shelfunits = bll_shelfunits;
            this.bll_hdw_stacker_taskqueue_his = bll_hdw_stacker_taskqueue_his;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_warehouse = bll_wh_warehouse;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_sys_control_dic = bll_sys_control_dic;
            //ListDataManager.setWareHouseGuidList2(bll_wh_warehouse, bll_wh_warehouse_type);
            //根据登录用户权限查询所有仓库列表 
            // ListDataManager.setWareHouseGuidListByPermission(bll_wh_warehouse, bll_wh_warehouse_type, GetLoginRole_WarehouseGuid());

        }
        /// <summary>
        /// 设置登录用户默认仓库
        /// </summary>
        /// <param name="warehouse_guid"></param>
        /// <returns></returns>
        public ActionResult SetCurrUserWareHouse(Guid warehouse_guid)
        {
            try
            {
                var Model = bll_wh_warehouse.GetFirstDefault(w => w.guid == warehouse_guid);
                if (Model != null && Model.guid != Guid.Empty)
                {
                    var loginModel = GetCurrentLoginUser();
                    if (loginModel != null && loginModel.guid != Guid.Empty)
                    {//设置当前登录用户默认仓库
                        loginModel.warehouse_guid = Model.guid;
                        var rtv = bll_login.Update(loginModel);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {//设置成功
                            return JsonSuccess();
                        }
                        else
                        {
                            AddJsonError("设置失败，请重试");
                        }
                    }
                    else
                    {
                        AddJsonError("当前登录用户为空，请刷新重试！");
                    }
                }
                else
                {
                    AddJsonError("参数错误，系统不存在该记录或已删除。");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("设置异常>>>" + ex.ToString());
            }
            return JsonError();
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
            ListDataManager.setWareHouseGuidListByPermission(bll_wh_warehouse, bll_wh_warehouse_type, GetLoginRole_WarehouseGuid());
            LTWMSWebMVC.App_Start.AppCode.MenuData menudata = App_Start.AppCode.MenuHelper.GetCurrentLoginMenu(bll_login, App_Start.AppCode.CurrentUser.Guid);
            var Model = new Models.HomeModel() { Menu = menudata };
            var loginModel = GetCurrentLoginUser();
            if (loginModel != null && loginModel.guid != Guid.Empty)
            {//设置当前登录用户默认仓库  
                /**********判断密码是否过期，过期进行提醒************/
                if (!IsPostBack())
                {
                    //第一次访问。。。
                    string passwordexpirationStr = bll_sys_control_dic.GetValueByType(CommDictType.PassWordExpiration, Guid.Empty);
                    if (string.IsNullOrWhiteSpace(passwordexpirationStr))
                    {
                        passwordexpirationStr = "300";//默认值300秒
                        bll_sys_control_dic.SetValueByType(CommDictType.PassWordExpiration, passwordexpirationStr, Guid.Empty);
                    }
                    int expirationPwd = LTLibrary.ConvertUtility.ToInt(passwordexpirationStr, 300);
                    if (loginModel.updatedate == null || LTLibrary.ConvertUtility.DiffSeconds(loginModel.updatedate, DateTime.Now) > expirationPwd)
                    {
                        //登录后进行密码修改提示。。。
                        //  updatedate 取消修改，只有改密码时才修改
                        Model.PassWordExpiration = 1;
                    }
                }
                /********************/
                if (loginModel.warehouse_guid == null || loginModel.warehouse_guid == Guid.Empty)
                {//判断当前是否超级管理员，超级管理员默认绑定第一个仓库，非管理员需要手动设置仓库权限
                    wh_warehouse _wareH = null;
                    if (loginModel.issuperadmin == true)
                    {//超级管理员
                        _wareH = bll_wh_warehouse.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal);
                    }
                    else
                    {//非超级管理员  通过权限查找对应的仓库
                        var list_wareHouseGuid = GetLoginRole_WarehouseGuid();
                        if (list_wareHouseGuid != null && list_wareHouseGuid.Count > 0)
                        {
                            _wareH = bll_wh_warehouse.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
                            list_wareHouseGuid.Contains(w.guid));
                        }
                    }
                    if (_wareH != null)
                    {
                        loginModel.warehouse_guid = _wareH.guid;
                        bll_login.Update(loginModel);
                    }
                }
                Model.warehouse_guid = loginModel.warehouse_guid;
            }
            string str = bll_sys_control_dic.GetValueByType(CommDictType.SendTaskToAllStackers, Guid.Empty);
            if (str == "1")
            {
                Model.SendTaskToAllStackers = true;
            }
            return View(Model);
        }
        /// <summary>
        /// 是否发送任务至所有堆垛机
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetSendToALLStacker(int issend)
        {
            try
            {
                //修改是否发送任务至堆垛机
                var rtv = bll_sys_control_dic.SetValueByType(CommDictType.SendTaskToAllStackers, issend == 1 ? "1" : "0", Guid.Empty);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    string _sendStr = issend == 1 ? "下发任务至所有堆垛机" : "取消下发任务至所有堆垛机";
                    AddUserOperationLog("修改所有堆垛机任务下发状态为：" + _sendStr, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
        public ActionResult Welcome()
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            WelcomDataModel Model = new WelcomDataModel();
            DateTime todayB = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime endD = DateTime.Now.AddDays(1);
            DateTime todayE = new DateTime(endD.Year, endD.Month, endD.Day, 0, 0, 0);
            Model.StockIn = bll_hdw_stacker_taskqueue_his.GetCount(w => w.warehouse_guid == warehouseguid && w.createdate > todayB && w.createdate < todayE &&
            w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockIn && (w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.Finished || w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.ForceComplete));
            Model.StockOut = bll_hdw_stacker_taskqueue_his.GetCount(w => w.warehouse_guid == warehouseguid && w.createdate > todayB && w.createdate < todayE && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut && (w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.Finished || w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.ForceComplete));
            Model.StockMove = bll_hdw_stacker_taskqueue_his.GetCount(w => w.warehouse_guid == warehouseguid && w.createdate > todayB && w.createdate < todayE && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.MoveTo && (w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.Finished || w.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.ForceComplete));
            //
            int Free = bll_shelfunits.GetCount(w => w.warehouse_guid == warehouseguid && w.cellstate == ShelfCellState.CanIn && w.state == LTWMSEFModel.EntityStatus.Normal);
            int Total = bll_shelfunits.GetCount(w => w.warehouse_guid == warehouseguid && w.state != LTWMSEFModel.EntityStatus.Deleted);
            int Used = Total - Free;
            double rateF = Math.Round((float)Free * 100 / Total, 2);
            double rateU = Math.Round(100 - rateF, 2);

            int _batter = bll_shelfunits.GetShelfUnitCountOfBatter(warehouseguid);
            int _other_matter = bll_shelfunits.GetShelfUnitCountOfOther(warehouseguid);
            int _emptyCout = bll_shelfunits.GetShelfUnitCountOfEmpty(warehouseguid);
            //xxxxxxxxxxxxxxxx

            Model.BatteryCount = Math.Round((float)_batter * 100 / Total, 2);
            Model.OtherMatterCout = Math.Round((float)_other_matter * 100 / Total, 2);
            Model.EmptyCout = Math.Round((float)_emptyCout * 100 / Total, 2);
            Model.Used = rateU;
            Model.UnUsed = rateF;
            //最近一个月出入库统计
            var list = new List<WelcomeDataObj>();
            // list.Add(new WelcomeDataObj() { xname = "3月1日", stockin = 2, stockout = 3 });
            List<chartData> ListIn = bll_hdw_stacker_taskqueue_his.GetStockInOutCount(StockInOutEnum.StockIn, warehouseguid);
            List<chartData> ListOut = bll_hdw_stacker_taskqueue_his.GetStockInOutCount(StockInOutEnum.StockOut, warehouseguid);
            //合并出入库 年月日  可能某一天存在只入库 或只出库
            List<chartData> ListAll = new List<chartData>();
            if (ListIn != null && ListIn.Count > 0)
            {
                foreach (var item in ListIn)
                {
                    if (!ListAll.Exists(w => w.year == item.year && w.month == item.month && w.day == item.day))
                    {
                        ListAll.Add(new chartData() { year = item.year, month = item.month, day = item.day });
                    }
                }
            }
            if (ListOut != null && ListOut.Count > 0)
            {
                foreach (var item in ListOut)
                {
                    if (!ListAll.Exists(w => w.year == item.year && w.month == item.month && w.day == item.day))
                    {
                        ListAll.Add(new chartData() { year = item.year, month = item.month, day = item.day });
                    }
                }
            }
            //日期重新排序
            ListAll = ListAll.OrderBy(w => w.year).ThenBy(w => w.month).ThenBy(w => w.day).ToList();
            if (ListAll != null && ListAll.Count > 0)
            {
                foreach (var item in ListAll)
                {
                    var objIn = ListIn.Where(w => w.year == item.year && w.month == item.month && w.day == item.day).FirstOrDefault() ?? new chartData();
                    var objOut = ListOut.Where(w => w.year == item.year && w.month == item.month && w.day == item.day).FirstOrDefault() ?? new chartData();
                    list.Add(new WelcomeDataObj() { xname = item.month + "/" + item.day, stockin = objIn.counts, stockout = objOut.counts });
                }
            }
            /*
            if (ListIn.Count >= ListOut.Count)
            {//入库大于出库 以入库年月日为准
                foreach (var item in ListIn)
                {
                    var obj = ListOut.Where(w => w.year == item.year && w.month == item.month && w.day == item.day).FirstOrDefault();
                    if (obj != null)
                    {
                        list.Add(new WelcomeDataObj() { xname = item.month + "/" + item.day, stockin = item.counts, stockout = obj.counts });
                    }
                    else
                    {
                        list.Add(new WelcomeDataObj() { xname = item.month + "/" + item.day, stockin = item.counts, stockout = 0 });
                    }
                }
            }
            else
            {//出库大于入库 
                foreach (var item in ListOut)
                {
                    var obj = ListIn.Where(w => w.year == item.year && w.month == item.month && w.day == item.day).FirstOrDefault();
                    if (obj != null)
                    {
                        list.Add(new WelcomeDataObj() { xname = item.month + "/" + item.day, stockin = obj.counts, stockout = item.counts });
                    }
                    else
                    {
                        list.Add(new WelcomeDataObj() { xname = item.month + "/" + item.day, stockin = 0, stockout = item.counts });
                    }
                }
            }
            */
            Model.List = list;
            return View(Model);
        }
        /// <summary>
        /// 获取系统连接等状态
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSystemStatus()
        {
            var listwcs = bll_wh_service_status.GetAllQuery(w => w.state != LTWMSEFModel.EntityStatus.Deleted)
                  .Select(s => MapperConfig.Mapper.Map<wh_service_status, ServiceStatusModel>(s)).ToList();
            //获取wcssrv 信息
            if (listwcs != null && listwcs.Count > 0)
            {
                foreach (var item in listwcs)
                {
                    if (item.wcstype == WCSType.WCSServer)
                    {
                        item.WcsSrvModel = MapperConfig.Mapper.Map<wh_wcs_srv, WcsSrvModel>(bll_wh_wcs_srv.GetFirstDefault(w => w.guid == item.wcs_srv_guid));
                    }
                }
            }
            return PartialView("WcsStatusView", listwcs);
        }
    }
}