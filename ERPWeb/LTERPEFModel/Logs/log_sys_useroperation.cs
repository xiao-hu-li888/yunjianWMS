using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Logs
{
    /// <summary>
    ///用户操作日志
    /// </summary>
    [Table("log_sys_useroperation")]
    public class log_sys_useroperation : BaseBaseEntity
    {    
        /// <summary>
        /// 日志记录
        /// </summary>
       [StringLength(500)]
        public string remark { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary>
        public DateTime log_date { get; set; }
        /// <summary>
        /// 操作人员
        /// </summary>
        [StringLength(30)]
        public string operator_u { get; set; }
    }
  
}
