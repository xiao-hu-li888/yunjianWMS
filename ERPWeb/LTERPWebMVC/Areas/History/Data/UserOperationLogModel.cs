using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.Areas.History.Data
{
    public class UserOperationLogModel:BaseModel
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        [Display(Name = "日志内容")]
        public string remark { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary>
        [Display(Name = "时间")]
        public DateTime? log_date { get; set; }
        /// <summary>
        /// 操作人员
        /// </summary>
        [Display(Name = "操作人")]
        public string operator_u { get; set; }
    }
}