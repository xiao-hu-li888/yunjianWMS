using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Logs
{
    /// <summary>
    /// wcs连接日志记录表
    /// </summary>
    [Table("log_wh_wcs")]
    public class log_wh_wcs : BaseBaseEntity
    {
        /// <summary>
       /// 所属仓库 关联表：wh_warehouse
       /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// wcs guid （表：wh_wcs）
        /// </summary>
        [Required]
        public Guid wcs_guid { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        [StringLength(500)]
        public string remark { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary> 
        public DateTime log_date { get; set; }
    }
}
