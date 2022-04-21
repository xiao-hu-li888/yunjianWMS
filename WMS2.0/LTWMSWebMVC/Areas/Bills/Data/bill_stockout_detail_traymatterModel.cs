using LTWMSEFModel.Bills;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class bill_stockout_detail_traymatterModel:BaseModel
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 出库单据guid（关联表：bill_stockout）
        /// </summary>
        [Display(Name = "出库单据guid")]
        public Guid stockout_guid { get; set; }

        /// <summary>
        /// 出库单详细表guid（关联表：bill_stockout_detail）
        /// </summary>
        [Display(Name = "出库单详细表guid")]
        public Guid stockout_detail_guid { get; set; }
        /*******************物料相关**********************/
        /// <summary>
        ///物料表guid（ 关联表：stk_matter）
        /// </summary>
        [Display(Name = "物料")]
        public Guid? stk_matter_guid { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        [Display(Name = "物料条码")]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string matter_name { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        [Display(Name = "批次")]
        public string lot_number { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Display(Name = "数量")]
        public int number { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [Display(Name = "生产日期")]
        public DateTime? produce_date { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        [Display(Name = "有效日期")]
        public DateTime? effective_date { get; set; }
        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// </summary>
        [Display(Name = "检验状态"), DropDownList("TestStatusEnum")]
        public TestStatusEnum test_status { get; set; }
        /******************关联任务信息**************************/
        /// <summary>
        /// 托盘条码
        /// </summary>
        [Display(Name = "托盘条码")]
        public string traybarcode { get; set; }


        /// <summary>
        /// 托盘出库回库状态
        /// </summary>
        [Display(Name = "托盘状态"),DropDownList("TrayOutStockStatusEnum")]
        public TrayOutStockStatusEnum tray_status { get; set; }
        /**************出库****************/
        /// <summary>
        /// 出库库位（1-1-1）
        /// </summary>
        [Display(Name = "出库库位")]
         public string out_shelfunits_pos { get; set; }

        /// <summary>
        /// 出库库位guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? out_shelfunits_guid { get; set; }
        /// <summary>
        /// 任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? out_stacker_taskqueue_guid { get; set; }

        /// <summary>
        /// 托盘出库时间
        /// </summary>
        [Display(Name = "出库时间")]
        public DateTime? tray_out_date { get; set; }
        /*************回库***************/
        /// <summary>
        /// 存储库位（1-1-1）
        /// </summary>
        [Display(Name = "回库库位")]
        public string back_shelfunits_pos { get; set; }
        /// <summary>
        /// 存储库位guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? back_shelfunits_guid { get; set; }

        /// <summary>
        /// 回库任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? back_stacker_taskqueue_guid { get; set; }
        /// <summary>
        /// 托盘回库时间
        /// </summary>
        [Display(Name = "回库时间")]
        public DateTime? tray_back_date { get; set; }
        ///// <summary>
        ///// 出库库位（1-1-1）
        ///// </summary>
        //[Display(Name = "出库库位")]
        //public string src_shelfunits_pos { get; set; }

        ///// <summary>
        ///// 出库库位guid（关联表：wh_shelfunits）
        ///// </summary>
        //[Display(Name = "出库库位guid")]
        //public Guid? src_shelfunits_guid { get; set; }
        ///// <summary>
        ///// 任务guid （关联表：hdw_stacker_taskqueue）
        ///// </summary> 
        //[Display(Name = "任务guid")]
        //public Guid? stacker_taskqueue_guid { get; set; }
        /// <summary>
        /// 备注（任务失败，自动重新生成任务）
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
    }
}  
 
 