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
    /// 盘点时需要进行其它操作的锁定，比如分拣。盘点入库需要一个一个物料重新扫码或手动添加，入库最终还是入到原库位。
    /// wms只相当于记录盘点任务（erp中可能需要对库存进行冲正，盘盈/盘亏）
    /// 库存盘点表（一般是管理人员添加，底下员工处理）
    /// 盘点回库回原库位？？
    /// </summary>
    [Table("bill_stockcheck")]
    public class bill_stockcheck : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 盘点单号(不填则自动编号)
        /// </summary>
        [StringLength(50)]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 盘点类型（按库位、按物料）
        /// </summary>
        public StockCheckTypeEnum check_type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 盘点状态
        /// </summary>
        public StockCheckStatus check_status { get; set; }
        /// <summary>
        /// 盘点结果
        /// </summary>
        public CheckResult check_result { get; set; }
        /// <summary>
        /// 是否发送通知消息(true 发送通知消息给盘点员，false/null 不发送)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? send_message { get; set; }
    }
    /// <summary>
    /// 盘点单类型
    /// </summary>
    public enum StockCheckTypeEnum
    {
        /// <summary>
        /// 按库位盘点
        /// </summary>
        [Description("按库位盘点")]
        CheckByShelf = 0,
        /// <summary>
        /// 按物料盘点
        /// </summary>
        [Description("按物料盘点")]
        CheckByMatter = 1
    }
    /// <summary>
    /// 盘点结果
    /// </summary>
    public enum CheckResult
    {
        /// <summary>
        /// 未确认
        /// </summary>
        [Description("未确认")]
        None = 0,
        /// <summary>
        /// 无差异
        /// </summary>
        [Description("无差异")]
        NoDifference = 1,
        /// <summary>
        /// 有差异
        /// </summary>
        [Description("有差异")]
        HasDifferences = 2
    }
    /// <summary>
    /// 盘点状态
    /// </summary>
    public enum StockCheckStatus
    {
        /// <summary>
        /// 创建
        /// </summary>
        [Description("创建")]
        Create = 0,
        /// <summary>
        /// 已下发（下发出库任务至堆垛机）
        /// </summary>
        [Description("已下发")]
        IsSend = 1,
        /// <summary>
        /// 盘点中
        /// </summary>
        [Description("盘点中")]
        Checking = 2,
        /// <summary>
        /// 已盘点完成
        /// </summary>
        [Description("已盘点完成")]
        Finished = 3
    }
}
