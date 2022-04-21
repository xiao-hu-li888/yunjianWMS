using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace LTWMSEFModel.Basic
{
    /// <summary>
    /// 系统字典表
    /// </summary>
    [Table("sys_dictionary")]
    public class sys_dictionary : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// parent id（最多存两级）
        /// </summary>
        [StringLength(50)]
        public string parent_id { get; set; }
        /// <summary>
        /// 字典键（系统或用户自定义，部分可修改）
        /// </summary>
        [StringLength(50)]
        public string key { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [StringLength(255)]
        public string value { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50)]
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
