using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Employee
{
    /// <summary>
    /// 员工信息表
    /// </summary>
    [Table("emp_employeeInfo")]
    public class emp_employeeInfo : BaseEntity
    {
        /// <summary>
        /// 员工编号
        /// </summary>
        [StringLength(30)]
        [Required]
        public string empid { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string empname { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        [StringLength(50)]
        public string name_pinyin { get; set; }
        /// <summary>
        /// 性别(true: 男 false/null:女)
        /// </summary> 
        [Column(TypeName = "bit")]
        public bool? gender { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(30)]
        public string mobile { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [StringLength(30)]
        public string comp_phone { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(50)]
        public string idcard { get; set; }
        /// <summary>
        /// 邮箱号
        /// </summary>
        [StringLength(50)]
        public string email { get; set; }
        /// <summary>
        /// 在职状态
        /// </summary>
        public OnJobStatusEnum onjob { get; set; }
        ///// <summary>
        ///// 组织架构guid （关联表：emp_organization）
        ///// </summary>
        //public Guid organization_guid { get; set; }  
    }
    public enum OnJobStatusEnum
    {
        /// <summary>
        /// 离职
        /// </summary>
        Off = 0,
        /// <summary>
        /// 在职
        /// </summary>
        On = 1
    }
}
