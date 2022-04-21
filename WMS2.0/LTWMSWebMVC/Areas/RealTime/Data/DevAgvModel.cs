using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class DevAgvModel:BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>  
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 唯一编号（1/2/3）
        /// </summary>
        [Display(Name = "编号")]
        public string number { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        [Display(Name = "ip地址")]
        public string ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [Display(Name = "端口")]
        public int port { get; set; }
        /// <summary>
        /// AGV实时位置(实时位置) 毫米?
        /// </summary>
        [Display(Name = "实时位置")]
        public int position { get; set; }
        /// <summary>
        /// agv电量0-100？%
        /// </summary>
        [Display(Name = "电量")]
        public int power { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        [Display(Name = "速度")]
        public int speed { get; set; }
        /// <summary>
        /// 当前agv执行的任务id
        /// </summary>
        [Display(Name = "任务id")]
        public int task_id { get; set; }
        ///// <summary>
        ///// 任务状态（空闲/任务中）
        ///// </summary>
        //[Display(Name = "任务状态"),DropDownList("AGVRunStatus")]
        //public AGVRunStatus run_status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
    }
}