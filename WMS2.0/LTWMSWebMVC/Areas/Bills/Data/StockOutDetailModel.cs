using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockOutDetailModel : BaseModel
    {
        /// <summary>
        /// 出库单据guid（关联表：bill_stockout）
        /// </summary> 
        public Guid stockout_guid { get; set; }
        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary> 
        [Display(Name = "物料"), DropDownList("StockMatter_List")]
        public Guid matter_guid { get; set; }

        /// <summary>
        /// 出库数量（计量单位）
        /// </summary>
        [Display(Name = "出库数量")]
        public int out_number { get; set; }

        /// <summary>
        /// 实际已出货数量
        /// </summary>
        [Display(Name = "实际出库数")]
        public int out_realnumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }


        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        [Display(Name = "仓库表guid")]
        public Guid? warehouse_guid { get; set; }



        /// <summary>
        /// 物料条码
        /// </summary>
        [Display(Name = "物料编码")]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string matter_name { get; set; }
        /// <summary>
        /// 是否指定出库批次（优先出指定批次的物料）
        /// </summary> 
        [Display(Name = "批次号")]
        public string lot_number { get; set; }

        /// <summary>
        /// 关联入库单据(该字段用来查询入库表直接判断是否已存在出库任务)
        /// </summary>
        [Display(Name = "关联入库单据")]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 配送点（总共四个集装箱配送点R1/R2/R3/R4）起点P1
        /// </summary>
        [Display(Name = "出库口"), DropDownList("AgvDestiNation")]
        public string destination { get; set; }

        public List<bill_stockout_detail_traymatterModel> List_bill_stockout_detail_traymatterModel { get; set; }
    }
}
