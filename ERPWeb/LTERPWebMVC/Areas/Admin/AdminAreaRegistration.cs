using System.Web.Mvc;

namespace LTERPWebMVC.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}