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
    /// 分区下关联存储的物料信息
    /// </summary>
    [Table("wh_shelfunits_area_matters")]
    public class wh_shelfunits_area_matters:BaseBaseEntity
    {
        /// <summary>
        /// 分区表guid（关联表：wh_shelfunits_area）
        /// </summary>
        [Required]
        public Guid shelfunits_area_guid { get; set; }
        /// <summary>
        /// 分区编码
        /// </summary>
        [StringLength(50)]
        public string area_code { get; set; }

        /// <summary>
        /// 物料表guid（关联表：stk_matter）
        /// </summary>
        [Required]
        public Guid stk_matter_guid { get; set; }
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [StringLength(50)]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(100)] 
        public string matter_name { get; set; }

    }
}
