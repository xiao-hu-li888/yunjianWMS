using LTWMSWebMVC.Filters;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyAuthorizeAttribute());

            filters.Add(new ErrorFilter());
        }
    }
}
