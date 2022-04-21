using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class ReservedOrderDetailSearch
    {
        /// <summary>
        /// wcs服务guid
        /// </summary>
        public Guid? reserve_guid { get; set; }
        public List<ReservedOrderDetailModel> PageCont { get; set; }
    }
}