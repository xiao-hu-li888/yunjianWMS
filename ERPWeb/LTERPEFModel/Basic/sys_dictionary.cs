using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Basic
{
    /// <summary>
    /// 字典类型枚举（只做一级分类用）
    /// </summary>
    public enum DictionaryParentEnum
    {
        /// <summary>
        /// 物料单位
        /// </summary>
        MatterUnit=0,
        /// <summary>
        /// 品牌
        /// </summary>
        Brand=1,
        /// <summary>
        /// 项目单位
        /// </summary>
        ProjectUnit=2,
        /// <summary>
        /// 公司类别
        /// </summary>
        CompType=3,
        /// <summary>
        /// 客户类型
        /// </summary>
        CustomType=4,
        /// <summary>
        /// 供应商类型
        /// </summary>
        SupplierType=5
    }
    /// <summary>
    /// 系统字典表（通用字典，不在系统中判断，可以自由增减）
    /// </summary>
    [Table("sys_dictionary")]
    public class sys_dictionary : BaseBaseEntity
    {
        /// <summary>
        /// 关联枚举：DictionaryParentEnum 的属性名称
        /// </summary>
        [StringLength(50)]
        [Required]
        public string parent_id { get; set; } 
        /// <summary>
        /// 字典值
        /// </summary>
        [StringLength(255)]
        public string text { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        public string desc { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 是否可编辑 0不可编辑，1可编辑 
        /// </summary> 
        [Column(TypeName = "bit")]
        public bool? editable { get; set;}
        /// <summary>
        /// 状态
        /// </summary>
        public DicStatus status { get; set; }
    }
    /// <summary>
    /// 状态
    /// </summary>
    public enum DicStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 已禁用
        /// </summary>
        [Description("已禁用")]
        Disabled = 2,
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = 3
    }
}
