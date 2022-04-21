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
    /// 项目详细表
    /// </summary>
    [Table("pro_project_details")]
    public class pro_project_details : BaseEntity
    {
        /// <summary>
        /// 关联项目表：pro_project
        /// </summary>
        public Guid project_guid { get; set; }
        /// <summary>
        /// 项目名称(分拣库/堆垛机/输送线/清洗机)
        /// </summary>
        [StringLength(50)]
        [Required]
        public string name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 项目单位（字典：sys_dictionary>>ProjectUnit 枚举）
        /// </summary>
        public Guid? unit_guid { get; set; }
        /// <summary>
        /// 项目单位【套/台/列】
        /// </summary>
        [StringLength(50)]
        public string uni_name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        public string memo { get; set; }

    }
}
