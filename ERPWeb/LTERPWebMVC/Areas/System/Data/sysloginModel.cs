using LTERPWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.System.Data
{
    public class sysloginModel:BaseModel
    {   
        /// <summary>
        /// 仓库guid 关联表:wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [Required(ErrorMessage = "登录名不能为空"), Display(Name = "登录名")]
        public string loginname { get; set; }

        /// <summary>
        /// 密码
        /// </summary>  
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空"), Display(Name= "姓名")]
        public string username { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        public string mobile { get; set; }

        /// <summary>
        /// 职位
        /// </summary> 
        [Display(Name = "职位")]
        public string position { get; set; }

        /// <summary>
        /// 性别(true: 男 false/null:女)
        /// </summary> 
        [Display(Name = "性别"),Radio("gender")]
        public bool gender { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Radio("YesNoState"), Display(Name = "超级管理员")]
        public bool issuperadmin { get; set; }
        /// <summary>
        /// 所属角色
        /// </summary>
        [CheckBox("RoleList"),Display(Name = "所属角色")]
        public string ex_role { get; set; }  
    }
}