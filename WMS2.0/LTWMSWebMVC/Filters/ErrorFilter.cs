using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Filters
{
    public class ErrorFilter: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            string errorMsg;
            var exception = filterContext.Exception;
            errorMsg = exception.Message; 
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                var accept = filterContext.RequestContext.HttpContext.Request.AcceptTypes;
                if (accept.Contains("application/json"))
                {
                    filterContext.Result = new JsonResult() { Data = new { Success = false, Msg = errorMsg } };
                }
                else
                {
                    filterContext.Result = new JavaScriptResult() { Script = "alert( '系统异常>>>" + errorMsg + "');" };
                }
            }
            else
            {
                //普通异常 
               // filterContext.Result = new ViewResult() { ViewName = "~/Areas/ErrorPage/Views/Error/Exception.cshtml", ViewData = new ViewDataDictionary() { { "Message", errorMsg } } };
                filterContext.Result=new ViewResult() { ViewName = "~/Areas/ErrorPage/Views/Error/Exception.cshtml", ViewData = new ViewDataDictionary() { { "Message", exception.ToString() } } };
            }
            filterContext.ExceptionHandled = true;
            //保存异常日志
            LTWMSWebMVC.WMSFactory.Log.v(exception.ToString());
        }
    }
}