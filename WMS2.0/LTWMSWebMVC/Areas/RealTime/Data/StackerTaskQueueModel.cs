using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class StackerTaskQueueModel:BaseModel
    {
        [Display(Name = "id")]
        public int id { get; set; }
        /// <summary>
        /// 关联 wh_shelfunits仓位详细表--出库、移库起点仓位
        /// </summary>  
        public Guid? src_shelfunits_guid { get; set; }
        /// <summary>
        /// 关联 wh_shelfunits仓位详细表--入库、移库终点仓位
        /// </summary> 
        public Guid? dest_shelfunits_guid { get; set; }
        /*************出入库**************/
        /// <summary>
        /// 起点:排-列-层-纵深
        /// </summary>
        [Display(Name = "起点坐标")]
        public string src_shelfunits_pos { get; set; }
        /// <summary>
        ///终点:排-列-层-纵深
        /// </summary>
        [Display(Name = "终点坐标")]
        public string dest_shelfunits_pos { get; set; }
        /// <summary>
        /// 起点站台([A]或B取料口，用数字1、2代替)  【纵深1/纵深2】
        /// </summary>
        public int src_station { get; set; }
        /// <summary>
        /// 货架(排) 
        /// </summary>
        public int src_rack { get; set; }
        /// <summary>
        /// 起始--列
        /// </summary>
        public int src_col { get; set; }
        /// <summary>
        /// 起始--层
        /// </summary>
        public int src_row { get; set; } 
        /// <summary>
        /// 终点站台（纵深1/纵深2）
        /// </summary>
        public int dest_station { get; set; }
        public int dest_rack { get; set; }
        /// <summary>
        ///目标--列
        /// </summary>
        public int dest_col { get; set; }
        /// <summary>
        ///目标--层
        /// </summary>
        public int dest_row { get; set; }
        ///// <summary>
        /////目标-- 纵深
        ///// </summary>
        //public int dest_depth { get; set; }
        //托盘信息******************************* 
        /// <summary>
        /// 纵深1--托盘条码
        /// </summary>
        [Display(Name = "托盘条码")]
        public string tray1_barcode { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [Display(Name = "电池条码1")]
        public string tray1_matter_barcode1 { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [Display(Name = "电池条码2")]
        public string tray1_matter_barcode2 { get; set; } 
        /// <summary>
        ///纵深2--托盘条码
        /// </summary>
        [StringLength(50)]
        public string tray2_barcode { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray2_matter_barcode1 { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray2_matter_barcode2 { get; set; }

        /******************************************/
        /// <summary>
        /// 执行任务的堆垛机编号
        /// </summary>
        [Display(Name = "流程字")]
        public string stacker_number { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        [Display(Name = "任务类型"), DropDownList("WcsTaskType")]
        public WcsTaskType tasktype { get; set; }
        /// <summary>
        /// 任务完成状态
        /// </summary>
        [Display(Name = "完成状态"), DropDownList("WcsTaskStatus")] 
        public WcsTaskStatus taskstatus { get; set; } 
        /// <summary>
        /// 排序（插队,默认0，值越大优先级越大）
        /// </summary>
        [Display(Name ="排序")]
        public int sort { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime? startup { get; set; }
        /// <summary>
        /// 任务结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? endup { get; set; }
        /// <summary>
        /// 任务信息
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        /// <summary>
        /// 物料信息集合(T01-1-1,T01-1-2,,,,,,)
        /// </summary>
        [Display(Name ="物料")]
        public string matterbarcode_list { get; set; }
        /********************************************************/
        /// <summary>
        /// 流水单号(批次)
        /// </summary>
        [Display(Name = "批次")]
        public string order { get; set; }
        /// <summary>
        /// 是否空托盘
        /// </summary>
        [Display(Name = "空托盘")]
        public bool? is_emptypallet { get; set; }
        [Display(Name = "时长（秒）")]
        public long task_time_length {
            get
            {
                if (startup != null)
                {
                    return LTLibrary.ConvertUtility.DiffSeconds(startup.Value,DateTime.Now);
                }
                return 0;
            }
        }
        /// <summary>
        /// 提交类型
        /// </summary>

        public int submit_type { get; set; }
    }
}