
using LTWMSWebMVC.App_Start.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LTWMSWebMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /*InterfaceService intersrv;*/
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //依赖注入
            var t = LTWMSWebMVC.App_Start.AutofacConfig.Init();
            var red = new Autofac.Integration.Mvc.AutofacDependencyResolver(t);
            DependencyResolver.SetResolver(red);

            //自动映射
            MapperConfig.Config();

            //艾华接口处理服务
            /*  intersrv = new InterfaceService();
              intersrv.Start();*/
        }
        protected void Application_End()
        {
            //开启连接wcs处理服务
            //wmsSrv1.Stop();
            /*  intersrv.Stop(); */
        }
    }
}
