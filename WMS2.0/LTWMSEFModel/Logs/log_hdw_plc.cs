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
    /// plc 运行日志
    /// </summary>
    [Table("log_hdw_plc")]
    public class log_hdw_plc : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// plc guid （表：hdw_plc）
        /// </summary>
        [Required]
        public Guid plc_guid { get; set; } 
        /// <summary>
        /// 任务id（关联表：hdw_stacker_taskqueue/hdw_stacker_taskqueue_his）
        /// </summary>
        public int? taskqueue_id { get; set; }
        /// <summary>
        /// 运行状态码（通过配置字典解码）
        /// </summary>
        public int code { get; set; }
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
