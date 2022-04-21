using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Stock
{
    /// <summary>
    /// 实时库存总表
    /// </summary>
    [Table("stk_stock")]
    public class stk_stock : BaseEntity
    {
        /// <summary>
        /// 物料/货品表  （关联表：stk_matter）
        /// </summary>
        [Required]
        public Guid matter_guid { get; set; }
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 实时库存数
        /// </summary>
        public int stock { get; set; }
        /// <summary>
        /// 出库锁定
        /// </summary>
        public int stock_locked { get; set; }

        /// <summary>
        /// 架上库存数+非架上库存=实时库存数（stock）
        /// </summary>
        public int onshelf_stock { get; set; }
        /// <summary>
        /// 下架锁定
        /// </summary>
        public int onshelf_stock_locked { get;set; } 
    }
}
