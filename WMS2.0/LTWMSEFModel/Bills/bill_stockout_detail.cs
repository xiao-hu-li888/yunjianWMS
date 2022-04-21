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
    /// 出库单据详细
    /// </summary>
    [Table("bill_stockout_detail")]
    public class bill_stockout_detail : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 出库单据guid（关联表：bill_stockout）
        /// </summary>
        [Required]
        public Guid stockout_guid { get; set; }

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
        public string matter_name { get; set; }
        /// <summary>
        /// 是否指定出库批次（优先出指定批次的物料）
        /// </summary> 
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 出库数量（计量单位）
        /// </summary>
        public int out_number { get; set; }
        /// <summary>
        /// 实际已出货数量
        /// </summary>
        public int out_realnumber { get; set; }
      
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 关联入库单据(该字段用来查询入库表直接判断是否已存在出库任务)
        /// </summary>
        [StringLength(50)]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 配送点（总共四个集装箱配送点R1/R2/R3/R4）起点P1
        /// </summary>
        [StringLength(50)]
        public string destination { get; set; }
        /// <summary>
        /// 出库总数量
        /// </summary>
        [NotMapped]
        public int total_num;
        /// <summary>
        /// 剩余出库总数量
        /// </summary>
        [NotMapped]
        public int remain_num;
    }
}
