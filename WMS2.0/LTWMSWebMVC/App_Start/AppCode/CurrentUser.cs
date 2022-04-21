using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.AppCode
{
    public class CurrentUser
    {
        public static Guid Guid
        {
            get
            {
                try
                {
                    return Guid.Parse(Convert.ToString(HttpContext.Current.Items["Current-Validate-Guid"]));
                }
                catch (Exception ex)
                { 
                
                }

                return Guid.Empty;
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