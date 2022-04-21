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
    /// 入库单据详细表
    /// </summary>
    [Table("bill_stockin_detail")]
    public class bill_stockin_detail : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 入库单据guid (关联表：bill_stockin)
        /// </summary>
        [Required]
        public Guid stockin_guid { get; set; }
        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary>
        [Required]
        public Guid matter_guid { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        [StringLength(100)]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(50)] 
        public string name { get; set; }
        /// <summary>
        /// 批次号（不填自动生成）
        /// </summary> 
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 入库总数量
        /// </summary>
        [NotMapped]
        public int total_num;
        /// <summary>
        /// 剩余入库总数量
        /// </summary>
        [NotMapped]
        public int remain_num;
        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        public decimal single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal total_weight { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? producedate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? effective_date { get; set; }
        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// </summary>
        public TestStatusEnum test_status { get; set; }
        /// <summary>
        /// 入库数量（计量单位）
        /// </summary>
        public int in_number { get; set; }
        /// <summary>
        /// 实际收货数量
        /// </summary>
        public int get_number { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
         
    }
}
