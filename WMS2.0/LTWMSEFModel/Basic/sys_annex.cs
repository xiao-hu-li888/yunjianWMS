using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Basic
{
    /// <summary>
    /// 系统附件表（保存所有图片/文档/附件）
    /// </summary>
    [Table("sys_annex")]
    public class sys_annex : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        [StringLength(50)]
        public string file_name { get; set; }
        /// <summary>
        /// 关联表
        /// </summary>
        [StringLength(50)]
        public string table_name { get; set; }
        /// <summary>
        /// 自定义列（关联表的扩展列）
        /// </summary>
        [StringLength(50)]
        public string column_name { get; set; }
        /// <summary>
        /// 关联的表guid
        /// </summary>
        [Required]
        public Guid ref_table_guid { get; set; }
        /// <summary>
        /// 如果文件是图片则自动生成缩略图，其它文件类型缩略图为空。
        /// </summary>
        [StringLength(255)]
        public string thumb_path { get; set; }
        /// <summary>
        /// 图片/文件原始路径
        /// </summary>
        [StringLength(255)]
        public string original_path { get; set; }
        /// <summary>
        /// 后缀名 pdf/jpg/doc/等文件后缀
        /// </summary>
        [StringLength(15)]
        public string suffix { get; set; }
        /// <summary>
        /// 单位字节byte
        /// </summary>
        public long size { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }
    }
}
