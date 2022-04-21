using LTWMSWebMVC.App_Start.WebMvCEx; 
using LTWMSEFModel.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class AlarmLogModel:BaseModel
    {
        /// <summary>
        /// 报警来源
        /// </summary>
        [Display(Name = "报警来源"),DropDownList("AlarmFrom")]
        public AlarmFrom log_from { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        [Display(Name = "日志记录")]
         public string remark { get; set; }
        /// <summary>
        /// 是否已弹出提示框（true 已弹出不在弹出提示，false/null 未弹出）
        /// </summary> 
        public bool? is_popup { get; set; }
        /// <summary>
        /// 日志发生时间（没有则默认系统当前时间）
        /// </summary>        
        [Display(Name = "日期")]
        public DateTime log_date { get; set; }
    }
}