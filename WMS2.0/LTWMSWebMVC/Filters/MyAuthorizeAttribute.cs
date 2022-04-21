using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Filters
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    { 
        /// <summary>
        /// 验证入口
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
        /// <summary>
        /// 验证核心代码
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = LTWMSWebMVC.WMSFactory.AuthorCookieHelper.GetToken();
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                LTLibrary.UserInfo user = LTLibrary.JwtHelp.Decrypt(authHeader);
                if (user != null)
                {//只要解密数据不为空，则说明token有效。
                    HttpContext.Current.Items.Add("Current-Validate-Guid", user.UserGuid);
                    HttpContext.Current.Items.Add("Current-Validate-UserName", user.UserName);
                    //获取当前页面的areas / controller /action
                    //判断当前用户是否有权限
                    ///  object areas = httpContext.Request.RequestContext.RouteData.Values["Areas"];
                    string _controller = Convert.ToString(httpContext.Request.RequestContext.RouteData.Values["Controller"]);
                    string _action = Convert.ToString(httpContext.Request.RequestContext.RouteData.Values["Action"]);
                    bool rtv= App_Start.AppCode.MenuHelper.HasPermission(App_Start.AutofacConfig.GetFromFac<LTWMSService.Basic.sys_loginBLL>(), App_Start.AppCode.CurrentUser.Guid,
                         _controller, _action);
                    if (rtv)
                    {
                        return true;
                    }
                }
            }
            httpContext.Response.StatusCode = 403;
            return false;
        }
        /// <summary>
        /// 验证失败处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                filterContext.Result = new EmptyResult();
                filterContext.HttpContext.Response.Redirect("/ErrorPage/Error/Unauthorized", true);
            }
        }
    }
}