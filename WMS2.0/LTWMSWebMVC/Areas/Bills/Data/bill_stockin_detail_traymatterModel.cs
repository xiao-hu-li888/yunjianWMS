using LTWMSEFModel.Bills;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class bill_stockin_detail_traymatterModel:BaseModel
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }

        /// <summary>
        /// 入库单据guid (关联表：bill_stockin)
        /// </summary>
        [Display(Name = "入库单据guid")]
        public Guid stockin_guid { get; set; }

        /// <summary>
        /// 入库单详细表guid（关联表：bill_stockin_detail）
        /// </summary> 
        public Guid stockin_detail_guid { get; set; }
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
        /// 存储库位（1-1-1）
        /// </summary>
        [Display(Name = "存储库位")]
        public string dest_shelfunits_pos { get; set; }
        /// <summary>
        /// 存储库位guid（关联表：wh_shelfunits）
        /// </summary>
        [Display(Name = "存储库位guid")]
        public Guid? dest_shelfunits_guid { get; set; }
        /// <summary>
        /// 任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        [Display(Name = "任务guid")]
        public Guid? stacker_taskqueue_guid { get; set; }

        /// <summary>
        /// 托盘入库状态
        /// </summary>
        [Display(Name = "入库状态"),DropDownList("TrayInStockStatusEnum")]
        public TrayInStockStatusEnum tray_status { get; set; }
        /// <summary>
        /// 托盘入库时间
        /// </summary>
        [Display(Name = "入库时间")]
        public DateTime? tray_in_date { get; set; }

        /// <summary>
        /// 备注（任务失败，自动重新生成任务）
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }

    }
}


