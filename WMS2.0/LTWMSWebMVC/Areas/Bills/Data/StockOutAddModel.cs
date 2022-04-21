using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockOutAddModel
    {
        /// <summary>
        /// 入库单列表（根据入库单出库）
        /// </summary>
        [Display(Name = "选择入库订单"), DropDownList("BillInOddNumberGuidList")]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 出库单号
        /// </summary>
        [Display(Name = "出库单号")]
        public string odd_numbers { get; set; }
        /// <summary>
        /// AGV输送终点站（R1-R4）
        /// </summary>
        [Display(Name ="AGV终点"),DropDownList("AgvDestiNation")]
        public string destination { get; set; }

    }
}