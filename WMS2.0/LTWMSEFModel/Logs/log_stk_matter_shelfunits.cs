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
    /// 物料上下架历史记录表
    /// </summary>
    [Table("log_stk_matter_shelfunits")]
    public class log_stk_matter_shelfunits : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 关联表:stk_matter
        /// </summary> 
        public Guid? matter_guid { get; set; }
        /// <summary>
        /// 物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string matter_barcode { get; set; } 
        /// <summary>
        /// 货架guid（关联表：wh_shelves）
        /// </summary> 
        [Required]
        public Guid shelves_guid { get; set; }
        /// <summary>
        /// 库位（1排-32列-2层-2纵深）
        /// </summary>
        [StringLength(30)]
        public string shelfunits_pos { get; set; } 
        /// <summary>
        /// 关联托盘号
        /// </summary>
        [StringLength(50)]
        public string traybarcode { get; set; }
        /// <summary>
        /// 关联堆垛机任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary>
        public Guid? stacker_taskqueue_guid { get; set; } 
        /// <summary>
        /// 货架实时库存--修改前
        /// </summary>
        public int shelfstock_before { get; set; }
        /// <summary>
        /// 货架实时库存--修改后
        /// </summary>
        public int shelfstock_after { get; set; } 
        /// <summary>
        /// 上下架数量
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 类型:上架/下架
        /// </summary>
        public ShelfLogType log_type { get; set; }
        /// <summary>
        /// 上下架日期
        /// </summary> 
        public DateTime date_time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
       [StringLength(255)]
        public string memo { get; set; }
    }
    /// <summary>
    /// 类型
    /// </summary>
    public enum ShelfLogType
    {
        /// <summary>
        /// 下架
        /// </summary>
        [Description("下架")]
        OffTheShelf = 0,
        /// <summary>
        /// 上架
        /// </summary>
        [Description("上架")]
        Grounding = 1 
    }
}
