using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.BasicData
{
    public class BasicDataAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BasicData";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BasicData_default",
                "BasicData/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}