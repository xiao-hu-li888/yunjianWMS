using LTERPWebMVC.Areas.Admin.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        LTERPService.Basic.sys_loginBLL bll_login; 
        public HomeController(LTERPService.Basic.sys_loginBLL bll_login )
        {
            this.bll_login = bll_login; 
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
            LTERPWebMVC.App_Start.AppCode.MenuData menudata = App_Start.AppCode.MenuHelper.GetCurrentLoginMenu(bll_login, App_Start.AppCode.CurrentUser.Guid);
            return View(new Models.HomeModel() { Menu = menudata });
        }
        public ActionResult Welcome()
        {
            WelcomDataModel Model = new WelcomDataModel(); 
            /////
            return View(Model);
        }
       
    }
}