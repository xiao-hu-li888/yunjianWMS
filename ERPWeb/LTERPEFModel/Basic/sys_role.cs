using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Basic
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Table("sys_role")]
    public class sys_role : BaseEntity
    { 
        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(50)]
        public string rolename { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string remark { get; set; }
        /// <summary>
        /// 所有权限
        /// </summary>
        [Column(TypeName = "text")]
        public string permissiontext { get; set; }
        /// <summary>
        /// 启用或禁用
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? active { get; set; }
        //[NotMapped]
        //public List<sys_login> syslogins{ get; set; }
  
    }
}
