using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.System.Data
{
    public class GeneralConfigModel
    {
        /// <summary>
        /// 密码过期时间配置（过期每次登录进行提示！）单位:秒
        /// </summary>
        [Required(ErrorMessage = "请输入一个大于300的整数"), Display(Name = "密码过期时间配置(单位:秒)")]
        public string PassWordExpiration { get; set; }
        /// <summary>
        /// 登录超时时间配置(单位:秒)
        /// </summary>
        [Required(ErrorMessage = "请输入一个大于300的整数"), Display(Name = "登录超时时间配置(单位:秒)")]
        public string LoginTimeOut { get; set; }

        /// <summary>
        /// 临近有效期
        /// </summary>
        [Required(ErrorMessage = "请输入一个大于0的整数"), Display(Name = "临近有效期设置(单位:天)")]
        public string NearTerm { get; set; }
        /// <summary>
        /// 弹出消息
        /// </summary>
        public string Message { get; set; }
    }
}