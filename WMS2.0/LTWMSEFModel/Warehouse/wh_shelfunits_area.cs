using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Warehouse
{
    /// <summary>
    /// 库位分区存放（特殊要求，一般库位不分区）
    /// </summary>
    [Table("wh_shelfunits_area")]
    public class wh_shelfunits_area : BaseBaseEntity
    {

        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// 不同的仓库可能分区定义不一样
        /// </summary>
        [Required]
        public Guid warehouse_guid { get; set; }
     
        /// <summary>
        /// 分区编码
        /// </summary>
        [StringLength(50)]
        public string area_code { get; set; }
        /// <summary>
        /// 分区名称
        /// </summary>
        [StringLength(50)]
        public string area_name { get; set; }
     
        /// <summary>
        /// 分区背景色块
        /// </summary>
        [StringLength(50)]
        public string area_background_color { get; set; }

    }
}
