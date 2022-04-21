using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.BillsAihua
{
    /// <summary>
    /// 预留单 对应物料详细明细
    /// </summary>
    [Table("billah_reserved_order_detail")]
    public class billah_reserved_order_detail : BaseBaseTaskStatusEntity
    {
        /// <summary>
        /// （关联表：billah_reserved_order）
        /// </summary>
        [Required]
        public Guid reserved_order_guid { get; set; }
        /// <summary>
        /// sap料号
        /// </summary>
        [StringLength(100)]
        public string goods_id { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(200)]
        public string goods_name { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        [StringLength(100)]
        public string spec_id { get; set; }
        /// <summary>
        ///  数量
        /// </summary>
        public decimal qty { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(50)]
        public string unit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
    }
}
