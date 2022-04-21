using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.StockInOut
{
    public class StockInOutAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StockInOut";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StockInOut_default",
                "StockInOut/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}