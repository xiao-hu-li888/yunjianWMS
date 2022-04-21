using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class DeviceList
    {
        /// <summary>
        /// agv设备状态
        /// </summary>
        public List<DevAgvModel> ListDevAgv { get; set; }
        /// <summary>
        /// plc/stacker
        /// </summary>
        public List<DevPlcModel> ListDevPlc { get; set; }
    }
}