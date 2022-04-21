using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class ShelfUnitsTop
    {
        /// <summary>
        /// 总库位
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 使用中
        /// </summary>
        public int Used { get; set; }
        /// <summary>
        /// 可用库位
        /// </summary>
        public int Free { get; set; }
         
        /// <summary>
        /// 电池总计
        /// </summary>
        public int BatteryCount { get; set; }
        /// <summary>
        /// 其它物料总计
        /// </summary>
        public int OtherMatterCout { get; set; }
        /// <summary>
        /// 空托盘总计
        /// </summary>
        public int EmptyCout { get; set; }


    }
}