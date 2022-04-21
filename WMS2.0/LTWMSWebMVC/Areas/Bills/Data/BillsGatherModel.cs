using LTWMSEFModel.Bills;
using LTWMSEFModel.query_model;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class BillsGatherModel
    {

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public int Id { get; set; }

        /// <summary>
        /// 出入库类型
        /// </summary>
        [Display(Name = "出入库类型"), DropDownList("BillsInOutEnum")]
        public BillsInOutEnum bills_type { get; set; }


        /// <summary>
        /// 入库类型
        /// </summary>
        [Display(Name = "入库类型"), DropDownList("StockInType")]
        public StockInType stockin_type { get; set; }

        /// <summary>
        /// 出库类型
        /// </summary>
        [Display(Name = "出库类型"), DropDownList("StockOutType")]
        public StockOutType stockout_type { get; set; }



        /// <summary>
        /// 出库、入库单号
        /// </summary>
        [Display(Name = "单号")]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 出库、入库日期
        /// </summary>
        [Display(Name = "日期"),DataType(DataType.Date)]
        public DateTime inout_date { get; set; }

        /// <summary>
        /// 物料条码
        /// </summary>
        [Display(Name = "条码")]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string name { get; set; }
        /// <summary>
        /// 批次号（不填自动生成）
        /// </summary>  
        [Display(Name = "批次号")]
        public string lot_number { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Display(Name = "数量")]
        public int in_out_number { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        [Display(Name = "生产日期")]
        public DateTime? producedate { get; set; }
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

    }
}