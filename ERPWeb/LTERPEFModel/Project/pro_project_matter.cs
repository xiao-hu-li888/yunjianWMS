using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Project
{
    /// <summary>
    /// 项目产品关联表
    /// </summary>
    [Table("pro_project_matter")]
    public class pro_project_matter : BaseEntity
    {
        /// <summary>
        /// 关联项目guid（表：pro_project）
        /// </summary>
        public Guid project_guid { get; set; }

        /// <summary>
        /// 关联物料guid（表：stk_matter）
        /// </summary>
        public Guid matter_guid { get; set; } 
        /// <summary>
        /// 物料名称（产品）
        /// </summary>
        [StringLength(50)]
        public string matter_name { get; set; }
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [StringLength(50)]
        public string code { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [StringLength(50)]
        public string specs { get; set; }
        /// <summary>
        /// 描述(型号)
        /// </summary>
        [StringLength(50)]
        public string description { get; set; }
        /// <summary>
        /// 物料类型名称
        /// </summary>
        [StringLength(50)]
        public string mattertype_name { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [StringLength(50)]
        public string brand_name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; } 
        /// <summary>
        /// 计量单位（件/个/米/kg）
        /// </summary>
        [StringLength(30)]
        public string unit_measurement { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
    }
}
