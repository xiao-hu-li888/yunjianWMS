using System.Web.Mvc;

namespace LTERPWebMVC.Areas.ErrorPage
{
    public class ErrorPageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ErrorPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ErrorPage_default",
                "ErrorPage/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}