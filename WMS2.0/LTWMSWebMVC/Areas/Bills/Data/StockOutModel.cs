using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockOutModel : BaseModel
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        [Display(Name = "出库单号")]
        public string odd_numbers { get; set; }
        [Display(Name ="订单类型")]
        public BillsProperty bill_property { get; set; }
        /// <summary>
        /// 配送点（总共四个集装箱配送点1/2/3/4）
        /// </summary>
        [Display(Name = "配送点")]
        public string destination { get; set; }
        /// <summary>
        /// 关联入库单据(用来直接判断是否已存在出库任务)
        /// </summary>
        [Display(Name = "关联入库单据")]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        [Display(Name = "出库日期"), DataType(DataType.Date),Required(AllowEmptyStrings =false)]
        public DateTime out_date { get; set; }
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
        /// 出库物料总数量（电池总数量）
        /// </summary>
        [Display(Name = "出库物料总数")]
        public int total_matter { get; set; }
        /// <summary>
        /// 实际出库总数量（电池总数量）
        /// </summary>
        [Display(Name = "实际出库总数")]
        public int total_out { get; set; }
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
        /// 收货人
        /// </summary>
        [Display(Name = "收货人")]
        public string receiver { get; set; }
        /// <summary>
        /// 经办人/仓管(手动填写)
        /// </summary>
        [Display(Name = "经办人")]
        public string operator_user { get; set; }
        /// <summary>
        /// 单据进行状态
        /// </summary>
        [Display(Name = "进行状态"),DropDownList("BillsStatus_Out")]
        public BillsStatus_Out bill_status { get; set; }
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        [Display(Name = "仓库")]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [Display(Name = "仓库编号")]
        public string code { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        [Display(Name = "出库类型"),DropDownList("StockOutType")]
        public StockOutType stockout_type { get; set; }
        /// <summary>
        /// 订单出库状态
        /// </summary>
        public GetStatus_Out get_status { get; set; }

        /// <summary>
        /// （重要：通过算法计算出哪些托盘需要出库，且出对应的哪些物料...）已生成对应的出库物料明细（包括出库任务）====>>>>>详细表： bill_stockout_task
        /// </summary>
        public bool generated_task { get; set; }
        /********项目相关**********/
        /// <summary>
        /// 项目编号（关联表：_project）[直接从erp获取进行中的项目列表？] [{prj_no:'项目编号',prj_name:'项目名称',cust_name:'客户名称',...},...]
        /// </summary>
        [StringLength(50)]
        public string project_no { get; set; }
        /// <summary>
        /// 关联项目
        /// </summary>
        [StringLength(50)]
        public string project_name { get; set; }
        /// <summary>
        /// 关联项目对应的客户
        /// </summary>
        [StringLength(50)]
        public string customer_name { get; set; }


        public List<StockOutDetailModel> List_StockOutDetailModel { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号")]
        public int Id { get; set; }
    }
} 
 

