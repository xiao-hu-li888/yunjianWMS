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
    /// 系统执行日志，只保留最近3-7天的日志？？
    /// </summary>
    [Table("log_sys_execute")]
    public class log_sys_execute : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 每个调用过程的辨别值，辨别不同的线程调用（不同线程之间一般不会重复，不同日期可以允许重复）
        /// </summary>
        public int diff { get;set;}
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
