using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.App_Start.AppCode
{
    public class CurrentUser
    {
        public static Guid Guid
        {
            get
            {
                return Guid.Parse(Convert.ToString(HttpContext.Current.Items["Current-Validate-Guid"]));
            }
        }
        public static string UserName
        {
            get
            {
                return Convert.ToString(HttpContext.Current.Items["Current-Validate-UserName"]);
            }
        }
    }
}