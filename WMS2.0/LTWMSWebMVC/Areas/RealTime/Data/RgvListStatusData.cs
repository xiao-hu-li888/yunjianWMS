using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class RgvListStatusData
    {
        /// <summary>
        /// 待出库任务数
        /// </summary>
        public int WaitOutTaskCount { get; set; }
        /// <summary>
        /// 待入库任务数
        /// </summary>
        public int WaitInTaskCount { get; set; }
        /// <summary>
        /// 设备列表（堆垛机、站台）
        /// </summary>
        public List<WcsDeviceModel> ListDeviceModel { get; set; }
        /*
        /// <summary>
        /// 堆垛机状态
        /// </summary>
        public string StackerStatus { get; set; }
        /// <summary>
        /// 站台1（可出库状态）
        /// </summary>
        public string TransportStatus1 { get; set; }
        /// <summary>
        /// 站台2（可出库状态）
        /// </summary>
        public string TransportStatus2 { get; set; }
        /// <summary>
        /// 站台3（可出库状态）
        /// </summary>
        public string TransportStatus3 { get; set; }
        /// <summary>
        /// 站台4（可出库状态）
        /// </summary>
        public string TransportStatus4{ get; set; }
        */
    }
}