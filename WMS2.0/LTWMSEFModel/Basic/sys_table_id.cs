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
    /// 管理表自增id(支持并发)
    /// </summary>
   [Table("sys_table_id")]
    public class sys_table_id: BaseEntity
    {
        /// <summary>
        /// 表名
        /// </summary>
        [StringLength(50)]
        public string table { get; set; }
        /// <summary>
        /// 当前数据库最大id
        /// </summary>
        public int max_id { get; set; } 

        /// <summary>
        /// 初始值
        /// </summary>
        public int init_val { get; set; }
        /// <summary>
        /// 当id自增到最大值，是否重置id值为 init_val(初始值)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_reset { get; set; }
        /// <summary>
        /// 重置id的最大值(int.max)
        /// </summary>
        public int reset_maxval { get; set; }

    }
}
