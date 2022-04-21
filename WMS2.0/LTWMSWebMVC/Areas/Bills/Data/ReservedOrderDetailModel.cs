using LTWMSEFModel;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class ReservedOrderDetailModel : BaseModel
    {
        /// <summary>
        /// （关联表：billah_reserved_order）
        /// </summary> 
        public Guid reserved_order_guid { get; set; }
        /// <summary>
        /// sap料号
        /// </summary>
        [Display(Name = "sap料号")]
        public string goods_id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string goods_name { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        [Display(Name = "批次")]
        public string spec_id { get; set; }
        /// <summary>
        ///  数量
        /// </summary>
        [Display(Name = "数量")]
        public decimal qty { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [Display(Name = "单位")]
        public string unit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }


        /// <summary>
        /// 任务状态
        /// </summary>
        [Display(Name = "任务状态"), DropDownList("TaskStatusEnum")]
        public TaskStatusEnum task_status { get; set; }
        /// <summary>
        /// 任务guid（关联表hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? task_guid { get; set; }
        // 托盘号
        //仓位

    }
}