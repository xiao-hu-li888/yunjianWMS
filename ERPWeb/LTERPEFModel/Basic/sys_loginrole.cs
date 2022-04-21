 
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LTERPEFModel.Basic
{
    /// <summary>
    /// 登录与角色关联表
    /// </summary>
    [Table("sys_loginrole")]
    public class sys_loginrole:BaseBaseEntity
    {
        /// <summary>
        /// 角色guid（表：sys_role）
        /// </summary>  
        public Guid roleguid { get; set; }
        /// <summary>
        /// 登录guid（表：sys_login）
        /// </summary> 
        public Guid loginguid { get; set; }
    }

}