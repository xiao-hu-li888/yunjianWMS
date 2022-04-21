using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LTWMSServer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // LTWMSModule.WMSServiceClient wmsSrv1;
        LTWMSModule.WMSServiceServer wmsSrv2;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //开启wms处理服务
            //开启连接wcs处理服务
            //wmsSrv1=new LTWMSModule.WMSServiceClient();
            //wmsSrv1.Start();
            wmsSrv2 = new LTWMSModule.WMSServiceServer();
            wmsSrv2.Start();
            //开启wms socket服务 
        }
        //Application_End
        //Application_Disposed
        protected void Application_End()
        {
            //开启连接wcs处理服务
            //wmsSrv1.Stop();
            wmsSrv2.Stop();
        }
    }
}
