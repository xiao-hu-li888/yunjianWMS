using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace LTWMSWebMVC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();
            //添加对text/plain的支持
            //var formatter= GlobalConfiguration.Configuration.Formatters.Where(f => 
            //f is System.Net.Http.Formatting.MediaTypeFormatter).FirstOrDefault();
            //if (!formatter.SupportedMediaTypes.Any(mt => mt.MediaType == "text/plain"))
            //    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));

            //config.Formatters.Add(formatter);
            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
