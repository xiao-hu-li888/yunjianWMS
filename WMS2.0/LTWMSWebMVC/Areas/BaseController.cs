using LTWMSWebMVC.App_Start;
using LTWMSService.Basic;
using LTWMSService.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSEFModel.Basic;
using LTWMSService.Warehouse;

namespace LTWMSWebMVC.Areas
{
    public class BaseController : Controller
    {
        log_sys_useroperationBLL bll_useroperation_log;
        sys_loginBLL bll_sys_login;
        wh_warehouseBLL bll_wh_warehouse;
        public BaseController()
        {
            this.bll_useroperation_log = AutofacConfig.GetFromFac<log_sys_useroperationBLL>();
            bll_sys_login = AutofacConfig.GetFromFac<sys_loginBLL>();
            bll_wh_warehouse = AutofacConfig.GetFromFac<wh_warehouseBLL>();
            //输出日志 
            //  AutofacConfig.GetFromFac<LTWMSEFModel.LTModel>().Database.Log = message => LTWMSWebMVC.WMSFactory.Log.v("LT-DBContext==>>" + message);
        }
        private sys_login loginUser;
        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        public sys_login GetCurrentLoginUser()
        {
            if (loginUser == null)
            {
                loginUser = bll_sys_login.GetFirstDefault(w => w.guid == App_Start.AppCode.CurrentUser.Guid);
            }
            return loginUser;
        }


        /// <summary>
        /// 获取登录用户所属角色的所有有权限的仓库列表
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetLoginRole_WarehouseGuid()
        {
            var currUser = GetCurrentLoginUser();
            if (currUser != null && currUser.guid != Guid.Empty)
            {
                //判断当前用户是否是管理员
                if (currUser.issuperadmin == true)
                {
                    List<Guid> wareGuids = new List<Guid>();
                    var listWareHouse = bll_wh_warehouse.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal);
                    if (listWareHouse != null && listWareHouse.Count > 0)
                    {
                        foreach (var bb in listWareHouse)
                        {
                            if (!wareGuids.Exists(w => w == bb.guid))
                            {
                                wareGuids.Add(bb.guid);
                            }
                        }
                    }
                    return wareGuids;
                }
                else
                {
                    var Roles = bll_sys_login.GetLoginRoles(currUser.guid);
                    if (Roles != null && Roles.Count > 0)
                    {
                        List<Guid> wareGuids = new List<Guid>();
                        foreach (var item in Roles)
                        {
                            var guidarr = LTLibrary.ConvertUtility.ParseToGuids(item.warehouse_guid_text);
                            if (guidarr != null && guidarr.Count > 0)
                            {
                                foreach (var bb in guidarr)
                                {
                                    if (!wareGuids.Exists(w => w == bb))
                                    {
                                        wareGuids.Add(bb);
                                    }
                                }
                            }
                        }
                        return wareGuids;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取当前登录用户默认仓库guid
        /// </summary>
        /// <returns></returns>
        public Guid GetCurrentLoginUser_WareGuid()
        {
            var LoginUser = GetCurrentLoginUser();
            if (LoginUser != null && LoginUser.warehouse_guid != null)
            {
                return LoginUser.warehouse_guid.Value;
            }
            return Guid.Empty;
        }
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="log"></param>
        public void AddUserOperationLog(string log,string createuser, Guid? _warehouseguid = null)
        {
            Guid? warehouseguid = null;
            try
            {
                  warehouseguid = _warehouseguid ?? GetCurrentLoginUser_WareGuid();
            }
            catch (Exception ex)
            {
              //  LTWMSWebMVC.WMSFactory.Log.v(ex);
            }
            try
            {
                bll_useroperation_log.Add(log, createuser, 0, warehouseguid); 
            }
            catch (Exception ex)
            {
                LTWMSWebMVC.WMSFactory.Log.v(ex);
            }
        }
        /// <summary>
        /// 调用通用错误页模板
        /// </summary>
        /// <param name="mess"></param>
        public ViewResult ErrorView(string mess)
        {
            return View("~/Areas/ErrorPage/Views/Error/Index.cshtml",
                new LTWMSWebMVC.Areas.ErrorPage.Data.ErrorModel() { Message = mess });
        }
        public JsonResult JsonSuccess()
        {
            return Json(new { success = true });
        }
        public void AddJsonError(string error)
        {
            ModelState.AddModelError("errorMess", error);
        }
        public JsonResult JsonError()
        {
            var errmess = ModelState.SelectMany(x => x.Value.Errors.Select(er => er.ErrorMessage));
            return Json(new { errors = errmess });
        }
        /// <summary>
        /// 判断是否回传
        /// </summary>
        /// <returns></returns>
        public bool IsPostBack()
        {
            if (Request.HttpMethod == "GET")
            {
                return false;
            }
            return true;
        }
    }
}