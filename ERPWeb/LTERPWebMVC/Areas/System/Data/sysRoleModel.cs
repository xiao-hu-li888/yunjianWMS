using LTERPWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.Areas.System.Data
{
    public class sysRoleModel:BaseModel
    {  
        /// <summary>
       /// 角色名称
       /// </summary>
        [Required(ErrorMessage = "角色名称不能为空"), Display(Name = "角色名称")]
        public string rolename { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string remark { get; set; }
        /// <summary>
        /// 所有权限
        /// </summary>
        [Display(Name = "所有权限")]
        public string permissiontext { get; set; }
        /// <summary>
        /// 启用或禁用
        /// </summary>
        [Display(Name = "启用状态"),DropDownList("Active")]
        public bool active { get; set; }
    }
}