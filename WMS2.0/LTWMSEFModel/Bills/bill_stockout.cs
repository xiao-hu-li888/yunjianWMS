using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Bills
{
    /// <summary>
    /// 出库单据（出货）--出库非下架（下架后可默认出库，一道手续走完）
    /// </summary>
    [Table("bill_stockout")]
    public class bill_stockout : BaseEntity
    {
        /// <summary>
        /// 出库单号
        /// </summary>
        [StringLength(50)]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 电池出库单(电池出库单处理方式不一样)/其它物料出库
        /// </summary>
        public BillsProperty bill_property { get; set; }
        /// <summary>
        /// 配送点（总共四个集装箱配送点R1/R2/R3/R4）起点P1
        /// </summary>
       [StringLength(50)]
        public string destination { get; set; }
        /// <summary>
        /// 关联入库单据(该字段用来查询入库表直接判断是否已存在出库任务)
        /// </summary>
        [StringLength(50)]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime out_date { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        public BillsFrom from { get; set; }

        /// <summary>
        ///  物料种类总数量（一个托盘多物料混装）
        /// </summary>
        public int total_category { get; set; }
        /// <summary>
        /// 出库物料总数量（电池总数量）
        /// </summary>
        public int total_matter { get; set; }
        /// <summary>
        /// 实际出库总数量（电池总数量）
        /// </summary>
        public int total_out { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
        /// <summary>
        /// 往来单位
        /// </summary>
        [StringLength(50)]
        public string contact_department { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        [StringLength(30)]
        public string receiver { get; set; }
        /// <summary>
        /// 经办人/仓管(手动填写)
        /// </summary>
        [StringLength(30)]
        public string operator_user { get; set; }
        /// <summary>
        /// 订单出库状态
        /// </summary>
        public GetStatus_Out get_status { get; set; }
        /// <summary>
        /// 单据进行状态
        /// </summary>
        public BillsStatus_Out bill_status { get; set; }
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [StringLength(30)]
        public string code { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        public StockOutType stockout_type { get; set; }
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
    }
    /// <summary>
    /// 订单出库状态
    /// </summary>
    public enum GetStatus_Out
    {
        /// <summary>
        /// 默认未处理状态
        /// </summary>
        [Description("未处理")]
        None = 0,
        /// <summary>
        /// 部分出库
        /// </summary>
        [Description("部分出库")]
        GetPart = 1,
        /// <summary>
        /// 全部出库
        /// </summary>
        [Description("全部出库")]
        GetALL = 2
    }
    /// <summary>
    /// 单据状态（单据强制完成，允许部分出库？）
    /// </summary>
    public enum BillsStatus_Out
    {
        /// <summary>
        /// 待出库
        /// </summary>
        [Description("待出库")]
        None = 0,
        /// <summary>
        /// 出库中
        /// </summary>
        [Description("出库中")]
        Running = 1,
        /// <summary>
        /// 出库完成（结束出库任务+结束agv任务=入库完成）
        /// </summary>
        [Description("出库完成")]
        Finished = 2
        ///// <summary>
        ///// 正在结束出库任务（接收到结束插箱命令）
        ///// </summary>
        //[Description("正在结束")]
        //Finishing=3
    }

    /// <summary>
    /// 出库类型
    /// </summary>
    public enum StockOutType
    {
        /// <summary>
        /// 领用出库
        /// </summary>
        [Description("领用出库")]
        UseOut = 0,
        /// <summary>
        /// 销售出库
        /// </summary>
        [Description("销售出库")]
        SellOut = 1,
        /// <summary>
        /// 抽样出库
        /// </summary>
        [Description("抽样出库")]
        SamplingOut = 2,
        /// <summary>
        /// 盘点出库
        /// </summary>
        [Description("盘点出库")]
        CheckOut = 3,  
        /// <summary>
        /// 其它出库
        /// </summary>
        [Description("其它出库")]
        OtherOut = 4
    }
}
