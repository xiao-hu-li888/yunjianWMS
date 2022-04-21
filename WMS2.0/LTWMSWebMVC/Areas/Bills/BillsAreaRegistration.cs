using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Bills
{
    public class BillsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Bills";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Bills_default",
                "Bills/{controller}/{action}/{guid}",
                new { action = "Index", guid = UrlParameter.Optional }
            );
        }
    }
}