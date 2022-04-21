using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.RealTime
{
    public class RealTimeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RealTime";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RealTime_default",
                "RealTime/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}