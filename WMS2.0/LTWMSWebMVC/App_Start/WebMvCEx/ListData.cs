using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.WebMvCEx
{
    public class ListData
    {
        public IEnumerable<App_Start.WebMvCEx.ListItem> List { get; set; }
        public DateTime? ExpireTime { get; set; }
        public ListData(IEnumerable<App_Start.WebMvCEx.ListItem> list, DateTime? expiretime)
        {
            this.List = list;
            this.ExpireTime = expiretime;
        }
    }
}