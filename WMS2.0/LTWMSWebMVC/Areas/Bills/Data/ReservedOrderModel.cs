using LTWMSEFModel.BillsAihua;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class ReservedOrderModel : BaseModel
    {
        /// <summary>
        /// 预留单号
        /// </summary>
        [Display(Name = "预留单号")]
        public string yl_id { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        [Display(Name = "总记录数")]
         public int total_record { get; set; }
        /// <summary>
        /// 出库成功总数
        /// </summary>
        [Display(Name = "出库成功总数")]
        public int total_success { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        /// <summary>
        /// 预留单状态
        /// </summary>
        [Display(Name = "预留单状态"), DropDownList("ReserveBillOutStatus")]
        public ReserveBillOutStatus bill_out_status { get; set; }
    }
}