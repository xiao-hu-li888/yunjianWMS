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
    /// 登录表
    /// </summary>
    [Table("sys_login")]
    public class sys_login : BaseEntity
    {
        /// <summary>
        /// 仓库guid 关联表:wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [StringLength(20)]
        [Required]
        public string loginname { get; set; }

        /// <summary>
        /// 密码
        /// </summary>  
        [Required]
        [StringLength(64)]
        public string password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(20)]
        public string username { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(20)]
        public string mobile { get; set; }

        /// <summary>
        /// 职位
        /// </summary> 
        [StringLength(50)]
        public string position { get; set; }

        /// <summary>
        /// 性别(true: 男 false/null:女)
        /// </summary> 
        [Column(TypeName = "bit")]
        public bool? gender { get; set; }
        /// <summary>
        /// 员工信息表guid（关联emp_employeeInfo）
        /// </summary>
        public Guid? employeeInfo_guid { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? issuperadmin { get; set; } 
        [NotMapped]
        public  List<sys_role> sysroles { get; set; }
    } 
}
