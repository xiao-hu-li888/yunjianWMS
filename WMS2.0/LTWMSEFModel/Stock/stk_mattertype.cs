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
    /// 物料分类
    /// </summary>
    [Table("stk_mattertype")]
    public class stk_mattertype : BaseEntity
    {
        /// <summary>
        /// 物料分类编码（2位编码组合、每一级最大允许数量为99个，足够满足分类需求 如：一级：01 二级01-01 三级 01-01-01 依次类推）
        /// </summary>
        [StringLength(50)]
        public string code { get; set; }
        /// <summary>
        /// 编码（01-01-01） -分割符
        /// </summary>
        [StringLength(50)]
        public string code_full { get; set; }

        /// <summary>
        /// 数字编码（code=code_num+parent_code）
        /// </summary>
        public int code_num { get; set; }
        /// <summary>
        /// 父级编码（可以为空）
        /// </summary>
        [StringLength(50)]
        public string parent_code { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [StringLength(50)]
        [Required]
        public string name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }

        public string namename { get; set; }
    }
}
