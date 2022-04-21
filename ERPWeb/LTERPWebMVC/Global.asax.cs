 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LTERPWebMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {  
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles); 
            //����ע��
            var t = LTERPWebMVC.App_Start.AutofacConfig.Init();
            var red = new Autofac.Integration.Mvc.AutofacDependencyResolver(t);
            DependencyResolver.SetResolver(red);

            //�Զ�ӳ��
            MapperConfig.Config();
            //Agv�������
            //AgvService1 agvsrv = new AgvService1(); 
        } 
    }
}
