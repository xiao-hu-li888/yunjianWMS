using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTERPEFModel.Stock
{
    /// <summary>
    /// 仓库表
    /// </summary>
    [Table("wh_warehouse")]
    public class wh_warehouse : BaseEntity
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        [StringLength(30)]
        public string name { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [StringLength(30)]
        [Required]
        public string code { get; set; }
        /// <summary>
        ///仓库地址
        /// </summary>
        [StringLength(50)]
        public string address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public string remark { get; set; } 
    } 
}
