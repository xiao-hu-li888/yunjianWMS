using LTERPWebMVC.App_Start;
using LTERPService.Basic;
using LTERPService.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas
{
    public class BaseController : Controller
    {
        log_sys_useroperationBLL bll_useroperation_log;
        public BaseController()
        {
            this.bll_useroperation_log = AutofacConfig.GetFromFac<log_sys_useroperationBLL>();
            //输出日志 
          //  AutofacConfig.GetFromFac<LTERPEFModel.LTModel>().Database.Log = message => LTERPWebMVC.WMSFactory.Log.v("LT-DBContext==>>" + message);
        }
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="log"></param>
        public void AddUserOperationLog(string log)
        {
            try
            {
                bll_useroperation_log.Add(new LTERPEFModel.Logs.log_sys_useroperation()
                {
                    guid = Guid.NewGuid(),
                    log_date = DateTime.Now,
                    operator_u = LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName,
                    remark = log
                });
            }catch(Exception ex)
            {
                LTERPWebMVC.WMSFactory.Log.v(ex);
            }
        }
        /// <summary>
        /// 调用通用错误页模板
        /// </summary>
        /// <param name="mess"></param>
        public ViewResult ErrorView(string mess)
        {
            return View("~/Areas/ErrorPage/Views/Error/Index.cshtml",
                new LTERPWebMVC.Areas.ErrorPage.Data.ErrorModel() { Message = mess });
        }
        public JsonResult JsonSuccess()
        {
            return Json(new { success = true }); ;
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
    }
}