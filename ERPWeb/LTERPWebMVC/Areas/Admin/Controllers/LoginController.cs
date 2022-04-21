using LTERPWebMVC.App_Start.AutoMap;
using LTERPWebMVC.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.Admin.Controllers
{
    public class LoginController : BaseController
    {
        LTERPService.Basic.sys_loginBLL bll_sys_login;
        InitData bll_InitData;
        public LoginController(LTERPService.Basic.sys_loginBLL bll_sys_login, InitData bll_InitData)
        {
            this.bll_sys_login = bll_sys_login;
            this.bll_InitData = bll_InitData;
        }
        [HttpGet]
        [AllowAnonymous]
        // GET: Admin/Login
        public ActionResult Index()
        { 
             bll_InitData.AddLoginAdmin();
            // 模拟agv任务
            ////bll_InitData.AddAGVTaskInfo();
            //////bll_InitData.AddAgvTaskQueue();
            //////bll_InitData.AddAlarmData();
            //return View(new LoginModel() {  UserName="admin", Pwd="123456"});
            return View(new LoginModel() { UserName = "admin"});
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(LoginModel loginmodel)
        {
            LTERPEFModel.Basic.sys_login u = bll_sys_login.Login(loginmodel.UserName, loginmodel.Pwd);
            if (u != null)
            {//登录成功  
                AddUserOperationLog("用户["+u.loginname+"/"+u.username+"]登录成功！");
                var usr = new LTLibrary.UserInfo();
                usr.UserGuid = u.guid;
                usr.UserName = u.username;
                string _token = LTLibrary.JwtHelp.Encryption(usr, LTERPWebMVC.WMSFactory.Config.JwtExp);
                //将token存入cookie
                LTERPWebMVC.WMSFactory.AuthorCookieHelper.AddToken(_token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                AddUserOperationLog("用户["+ loginmodel.UserName+ "]登录失败,用户名或密码错误！");
                loginmodel.Error = "用户名或密码错误！";
            }
            return View("Index", loginmodel);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUserPwd(string old,string new1,string new2)
        {
            try
            {
                //{ old: _oldpwd, new: _newpwd, new2: _newpwd2 }
                if ((old ?? "").Trim() == "" || (new1 ?? "").Trim() == "" || (new2 ?? "").Trim() == "")
                {
                    AddJsonError("密码不能为空");
                    return JsonError();
                }
                if (new1 != new2)
                {//新密码不一致
                    AddJsonError("新密码不一致");
                    return JsonError();
                }
                //判断旧密码是否相等
                var _obj = bll_sys_login.GetFirstDefault(w => w.guid == App_Start.AppCode.CurrentUser.Guid); 
                if(_obj==null)
                {
                    AddJsonError("参数错误，未找到当前登录用户的记录");
                    return JsonError();
                }
                else if(bll_sys_login.SHA256(old) != _obj.password)
                { //判断旧密码是否相等
                    AddJsonError("原密码与数据库不一致");
                    return JsonError(); 
                }
                //相等保存新密码
                var rtv = bll_sys_login.UpdatePassword(App_Start.AppCode.CurrentUser.Guid, new1);
                if (rtv == LTERPEFModel.SimpleBackValue.True)
                {
                    return JsonSuccess();
                }
                else if (rtv == LTERPEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                {
                    AddJsonError("数据更新并发异常，请重试。"); 
                }
                else
                {
                    AddJsonError("保存失败");
                } 
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        [HttpGet]
        public ActionResult LoginOut()
        {
            //删除cookie
            WMSFactory.AuthorCookieHelper.RemoveToken();
            //重新跳转至登录页
             return RedirectToAction("Index");
        }
    }
}