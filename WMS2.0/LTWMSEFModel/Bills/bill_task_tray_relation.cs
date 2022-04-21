using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Bills
{
    /// <summary>
    /// 单据（入库、出库、盘点。。。）与托盘、任务的关联关系表
    /// 用于在托盘入库、出库流转、盘点回库等，将对应的信息进行关联绑定
    /// </summary>
    [Table("bill_task_tray_relation")]
    public class bill_task_tray_relation : BaseEntity
    {
        /// <summary>
        /// 单据类型
        /// </summary>
        public ReBillTypeEnum bill_type { get; set; }
        /// <summary>
        /// 出库类型（仅针对出库单）
        /// </summary>
        public StockOutType stockout_type { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        [StringLength(50)]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 与对应单据关联的详细绑定表的guid
        /// 入库单据：bill_stockin_detail_traymatter
        /// 出库单据：bill_stockout_detail_traymatter
        /// 盘点单据：
        /// </summary>
        public Guid re_detail_traymatter_guid { get; set; }
        /// <summary>
        /// 托盘条码(多个仓库托盘条码必须唯一) 新增修改检查唯一性
        /// </summary>
        [StringLength(100)]
        public string traybarcode { get; set; }

    }
    /// <summary>
    /// 单据类型
    /// </summary>
    public enum ReBillTypeEnum
    {
        /// <summary>
        /// 入库单据>>bill_stockin 
        /// </summary>
        StockIn = 0,
        /// <summary>
        /// 出库单据>>bill_stockout
        /// </summary>
        StockOut = 1,
        /// <summary>
        /// 盘点单据 
        /// </summary>
        Check = 2
    }
}
