using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Admin.Models
{
    public class LoginModel
    {
        [Display(Name = "用户名", ShortName = "用户名", Description = "用户名或邮箱地址")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空"), MinLength(2,ErrorMessage = "用户名长度不能小于2个字符"), MaxLength(20, ErrorMessage = "用户名长度不能超过20个字符")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "密码不能为空"), DataType(DataType.Password)] 
        [Display(Name = "密码")]
        public string Pwd { get; set; } 
        [Display(Name = "错误信息")]
        public string Error { get; set; }
    }
}