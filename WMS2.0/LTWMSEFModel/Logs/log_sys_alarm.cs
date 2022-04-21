using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Logs
{
    /// <summary>
    /// 系统报警日志（包括：异常、硬件等运行警告日志）
    /// </summary>
    [Table("log_sys_alarm")]
    public class log_sys_alarm : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 报警来源
        /// </summary>
        public AlarmFrom log_from { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        [StringLength(500)]
        public string remark { get; set; }
        /// <summary>
        /// 是否已弹出提示框（true 已弹出不在弹出提示，false/null 未弹出）
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_popup { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary>        
        public DateTime log_date { get; set; } 
    }
    /// <summary>
    /// 报警来源
    /// </summary>
    public enum AlarmFrom
    {
        /// <summary>
        /// 系统
        /// </summary>
        [Description("系统")]
        System = 0,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 1,
        /// <summary>
        /// 输送线
        /// </summary>
        [Description("输送线")]
        Transport = 2,
        /// <summary>
        /// Agv小车
        /// </summary>
        [Description("Agv小车")]
        Agv = 3,
        /// <summary>
        /// 堆垛机
        /// </summary>
        [Description("堆垛机")]
        Stacker = 4
    }
}
