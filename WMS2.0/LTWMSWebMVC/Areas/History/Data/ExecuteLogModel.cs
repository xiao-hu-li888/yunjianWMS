using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.History.Data
{
    public class ExecuteLogModel:BaseModel
    { 
        /// 每个调用过程的辨别值，辨别不同的线程调用（不同线程之间一般不会重复，不同日期可以允许重复）
       /// </summary>
        public int diff { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        [Display(Name = "日志内容")]
        public string remark { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary>       
        [Display(Name = "时间")]
        public DateTime log_date { get; set; }
    }
}