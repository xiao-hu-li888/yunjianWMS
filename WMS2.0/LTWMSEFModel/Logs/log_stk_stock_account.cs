using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Logs
{
    /// <summary>
    /// 库存流水账（入库/出库/盘盈/盘亏/报损）
    /// </summary>
    [Table("log_stk_stock_account")]
    public class log_stk_stock_account : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 物料/货品表  （关联表：stk_matter）
        /// </summary>
        [Required]
        public Guid matter_guid { get; set; } 
        /// <summary>
        /// 库存修改前实时库存
        /// </summary>
        public int stock_before { get; set; }
        /// <summary>
        /// 库存修改后实时库存
        /// </summary>
        public int stock_after { get; set; }
        /// <summary>
        /// 修改库存数量
        /// </summary>
        public int modify_number { get; set; }
        /// <summary>
        /// 库存流水类型
        /// </summary>
        public StockAccountType account_type { get; set; }
        /// <summary>
        /// 入库单据guid （关联表：bill_stockin_detail）
        /// </summary> 
        public Guid? stock_in_guid { get; set; }
        /// <summary>
        /// 出库单据guid（关联表:bill_stockout_detail）
        /// </summary> 
        public Guid? stock_out_guid { get; set; }
        /// <summary>
        /// 批次号
        /// </summary> 
        [StringLength(50)]
        public string batch_number { get; set; }
        /// <summary>
        /// 记账日期
        /// </summary> 
        public DateTime account_date { get; set; }
        /// <summary>
        /// 流水日志(例：入库库存+5 ...)
        /// </summary>
        [StringLength(255)]
        public string remark { get; set; }
    }
    /// <summary>
    /// 库存流水类型
    /// </summary>
    public enum StockAccountType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        StockIn = 0,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        StockOut = 1 
    }
}
