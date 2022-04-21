using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class BatteryOutModel
    {
        /// <summary>
        /// 出库订单(T01)
        /// </summary>
        [Display(Name = "订单号")]
        public string Order { get; set; }
        /// <summary>
        /// 簇
        /// </summary>
        [Display(Name = "簇")]
        public string Cluster { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        [Display(Name = "编号")]
        public string Number { get; set; }
    }
}