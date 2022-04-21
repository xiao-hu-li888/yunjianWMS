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
    /// 入库单据(收货)-入库非上架
    /// </summary>
    [Table("bill_stockin")]
    public class bill_stockin : BaseEntity
    {
        /// <summary>
        /// 入库单号（T01）
        /// </summary>
        [StringLength(50)]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 电池入库单(电池入库单处理方式不一样)/其它物料入库
        /// </summary>
        public BillsProperty bill_property { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime in_date { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        public BillsFrom from { get; set; }
        /// <summary>
        ///  物料种类总数量（一个托盘多物料混装）
        /// </summary>
        public int total_category { get; set; }
        /// <summary>
        /// 入库物料总数量（电池总数量）
        /// </summary>
        public int total_matter { get; set; }
        /// <summary>
        /// 实收总数量（电池总数量）
        /// </summary>
        public int total_get { get; set; }
        /// <summary>
        /// 订单收货状态
        /// </summary>
        public GetStatus get_status { get; set; }
        /// <summary>
        /// 单据进行状态
        /// </summary>
        public BillsStatus bill_status { get; set; }
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
        /// 交货人
        /// </summary>
        [StringLength(30)]
        public string deliverer { get; set; }
        /// <summary>
        /// 经办人/仓管(手动填写)
        /// </summary>
        [StringLength(30)]
        public string operator_user { get; set; }
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
        /// 入库类型
        /// </summary>
        public StockInType stockin_type { get; set; }

    }
    /// <summary>
    /// 出入库订单属性（电池/其它物料）
    /// </summary>
    public enum BillsProperty
    {
        /// <summary>
        /// 电池（针对电池和其它物料的处理方式不一样）
        /// </summary>
        [Description("电池")]
        Battery = 0,
        /// <summary>
        /// 其它物料
        /// </summary>
        [Description("其它物料")]
        Other = 1
    }
    /// <summary>
    /// 入库类型
    /// </summary>
    public enum StockInType
    {
        /// <summary>
        /// 生产入库
        /// </summary>
        [Description("生产入库")]
        PurchaseIn = 0,
        /// <summary>
        /// 采购入库
        /// </summary>
        [Description("采购入库")]
        ProduceIn = 1,
        /// <summary>
        /// 调拨入库
        /// </summary>
        [Description("调拨入库")]
        MoveIn = 2,    
        /// <summary>
        /// 退货入库
        /// </summary>
        [Description("退货入库")]
        ReturnGoodsIn = 3,
        /// <summary>
        /// 补货入库
        /// </summary>
        [Description("补货入库")]
        ReplenishIn = 4,
        /// <summary>
        /// 其它入库
        /// </summary>
        [Description("其它入库")]
        OtherIn = 5
    }
    /// <summary>
    /// 单据状态（入库强制完成，允许部分收到？）
    /// </summary>
    public enum BillsStatus
    {
        /// <summary>
        /// 待入库
        /// </summary>
        [Description("待入库")]
        None = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Running = 1,
        /// <summary>
        /// 入库完成
        /// </summary>
        [Description("入库完成")]
        Finished = 2
    }
    /// <summary>
    /// 订单收货状态
    /// </summary>
    public enum GetStatus
    {
        /// <summary>
        /// 默认未处理状态
        /// </summary>
        [Description("未处理")]
        None = 0,
        /// <summary>
        /// 部分收到
        /// </summary>
        [Description("部分收到")]
        GetPart = 1,
        /// <summary>
        /// 全部收到
        /// </summary>
        [Description("全部收到")]
        GetALL = 2
    }
    /// <summary>
    /// 单据来源
    /// </summary>
    public enum BillsFrom
    {
        /// <summary>
        /// 默认-系统录入
        /// </summary>
        [Description("默认-系统录入")]
        System = 0,
        /// <summary>
        /// 盘盈（入库）
        /// </summary>
        [Description("盘盈（入库）")]
        StockCheck_win = 1,
        /// <summary>
        /// 盘亏（出库）
        /// </summary>
        [Description("盘亏（出库）")]
        StockCheck_lose = 2,
        /// <summary>
        /// 来源ERP
        /// </summary>
        [Description("来源ERP")]
        ERP = 3,
        /// <summary>
        /// 来源MESS
        /// </summary>
        [Description("来源MESS")]
        MESS = 4,
        /// <summary>
        /// 大捷系统
        /// </summary>
        [Description("大捷系统")]
        DajieSystem = 5,
        /// <summary>
        /// 来源Sap
        /// </summary>
        [Description("来源Sap")]
        Sap= 6
    }
}
