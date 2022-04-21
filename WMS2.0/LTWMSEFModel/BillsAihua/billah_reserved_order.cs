using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.BillsAihua
{
    /// <summary>
    /// 预留单号
    /// 通过选择或查询预留单，将对应的所有批次物料全部出库
    /// </summary>
    [Table("billah_reserved_order")]
    public class billah_reserved_order : BaseEntity
    {
        /// <summary>
        /// 预留单号
        /// </summary>
        [StringLength(100)]
        public string yl_id { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int total_record { get; set; }
        /// <summary>
        /// 出库成功总数
        /// </summary>
        public int total_success { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 预留单状态
        /// </summary>
        public ReserveBillOutStatus bill_out_status { get; set; }
    }
    /// <summary>
    /// 预留单状态
    /// </summary>
    public enum ReserveBillOutStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        None = 0,
        /// <summary>
        /// 出库中
        /// </summary>
        [Description("出库中")]
        Running = 1,
        /// <summary>
        /// 出库完成
        /// </summary>
        [Description("出库完成")]
        Finished = 2,
        /// <summary>
        /// 出库已取消（终止出库，已出库的不管，未出库的取消出库）
        /// </summary>
        [Description("出库已取消")]
        Canceled = 3,
        /// <summary>
        /// 处理失败(处理失败，可以选择人工进行处理。。。)
        /// </summary>
        [Description("处理失败")]
        DealingFailures = 4
    }
}
