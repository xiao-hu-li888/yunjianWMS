using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockInModel:BaseModel
    {
        /// <summary>
      /// 入库单号（T01-簇-编号）
      /// </summary>
        [Display(Name = "入库单号")]
        public string odd_numbers { get; set; }

        /// <summary>
        /// 电池入库单(电池入库单处理方式不一样)/其它物料入库
        /// </summary>
       [Display(Name ="物料类型")]
        public BillsProperty bill_property { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        [Display(Name = "入库日期"),DataType( DataType.Date)]
          public DateTime in_date { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        [Display(Name = "数据来源")]
        public BillsFrom from { get; set; }
        /// <summary>
        ///  物料种类总数量（一个托盘多物料混装）
        /// </summary>
        [Display(Name = "物料种类总数")]
        public int total_category { get; set; }
        /// <summary>
        /// 入库物料总数量（电池总数量）
        /// </summary>
        [Display(Name = "入库物料总数")]
        public int total_matter { get; set; }
        /// <summary>
        /// 实收总数量（电池总数量）
        /// </summary>
        [Display(Name = "实入总数")]
        public int total_get { get; set; }
        /// <summary>
        /// 订单收货状态
        /// </summary>
        [Display(Name = "收货状态")]
        public GetStatus get_status { get; set; }
        /// <summary>
        /// 单据进行状态
        /// </summary>
        [Display(Name = "进行状态"), DropDownList("BillsStatus")]
        public BillsStatus bill_status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        [Display(Name = "往来单位")]
        public string contact_department { get; set; }
        /// <summary>
        /// 交货人
        /// </summary>
        [Display(Name = "交货人")]
        public string deliverer { get; set; }
        /// <summary>
        /// 经办人/仓管(手动填写)
        /// </summary>
        [Display(Name = "经办人")]
        public string operator_user { get; set; }
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary>
        [Display(Name = "仓库")]
        public Guid warehouse_guid { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [Display(Name = "仓库编号")]
        public string code { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        [Display(Name = "入库类型"),DropDownList("StockInType")]
        public StockInType stockin_type { get; set; }

        public List<StockInDetailModel> List_StockInDetailModel { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public int Id { get; set; }
    }
}



 
 
 
 
 
 
 
 
 
 
 
 
