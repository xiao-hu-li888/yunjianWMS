using LTWMSEFModel.Bills;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockInDetailModel : BaseModel
    {
        /// <summary>
        /// 入库单据guid (关联表：bill_stockin)
        /// </summary> 
        public Guid stockin_guid { get; set; }

        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }



        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary>  
        [Display(Name = "物料"), DropDownList("StockMatter_List")]
        public Guid matter_guid { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [Display(Name = "物料编码")]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string name { get; set; }
        /// <summary>
        /// 批次号（不填自动生成）
        /// </summary> 
        [Display(Name = "批次号"), Required(AllowEmptyStrings = false)]
        public string lot_number { get; set; }

        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        [Display(Name = "单个重量")]
        public decimal single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        [Display(Name = "总重量")]
        public decimal total_weight { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        [Display(Name = "生产日期"), DataType(DataType.Date), Required(AllowEmptyStrings = false)]
        public DateTime? producedate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        [Display(Name = "有效日期"), DataType(DataType.Date), Required(AllowEmptyStrings = false)]
        public DateTime? effective_date { get; set; }
        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// </summary>
        [Display(Name = "检验状态"), DropDownList("TestStatusEnum")]
        public TestStatusEnum test_status { get; set; }

        /// <summary>
        /// 入库数量（计量单位）
        /// </summary>
        [Display(Name = "入库数量")]
        public int in_number { get; set; }
        /// <summary>
        /// 实际收货数量
        /// </summary>
        [Display(Name = "实际入库数")]
        public int get_number { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }


        public List<bill_stockin_detail_traymatterModel> List_bill_stockin_detail_traymatterModel { get; set; }
    }
}

