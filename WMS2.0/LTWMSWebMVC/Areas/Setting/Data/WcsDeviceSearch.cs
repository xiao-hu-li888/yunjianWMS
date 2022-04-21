using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Setting.Data
{
    public class WcsDeviceSearch
    {
        /// <summary>
        /// wcs服务guid
        /// </summary>
        public Guid? wcssrv_guid { get; set; }
        public List<WcsDeviceModel> PageCont { get; set; }
    }
}