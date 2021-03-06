using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.System
{
    public class SystemAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "System";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "System_default",
                "System/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}