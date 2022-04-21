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
    /// 项目产品 详细物料清单(设计清单)  通过设计清单生成采购单？
    /// </summary>
    [Table("pro_promatterdesign_detail")]
    public class pro_promatterdesign_detail : BaseEntity
    {
        /// <summary>
        /// 关联项目guid（表：pro_project）
        /// </summary>
        public Guid project_guid { get; set; }
        /// <summary>
        /// 关联项目产品guid（表：pro_project_matter）
        /// </summary>
        public Guid project_matter_guid { get; set; }

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
        /// 单台数量
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 计量单位（件/个/米/kg）
        /// </summary>
        [StringLength(30)]
        public string unit_measurement { get; set; }
        /// <summary>
        /// 所属部件(关联表：stk_mattertype ？？？？)
        /// </summary>
        public Guid? parts_guid { get; set; }
        /// <summary>
        /// 部件名称(部件树)
        /// </summary>
        [StringLength(50)]
        public string parts_name { get; set; }

        /// <summary>
        ///设计人员（表：emp_employeeInfo）
        /// </summary>
        public Guid? employeeInfo_guid { get; set; }

        /// <summary>
        /// 设计人员
        /// </summary>
        [StringLength(20)]
        public string designer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }

    }
}
