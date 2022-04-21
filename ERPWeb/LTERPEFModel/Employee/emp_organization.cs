using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Employee
{
    /// <summary>
    /// 组织架构
    /// </summary>
    [Table("emp_organization")]
    public class emp_organization : BaseEntity
    {
        /// <summary>
        /// 架构编码 如：一级：001 二级001-001 三级 001-001-001 依次类推）
        /// </summary>
        [StringLength(50)]
        public string code { get; set; }
        /// <summary>
        /// 组织架构名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string orgname { get; set; }
        /// <summary>
        /// 名称缩写
        /// </summary>
        [StringLength(50)]
        public string abbreviation { get; set; }
        /// <summary>
        /// 排序
        /// </summary> 
        public int sort { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(500)]
        public string memo { get; set; }
         

    }
}
