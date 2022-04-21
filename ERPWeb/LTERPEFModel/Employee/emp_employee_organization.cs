using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Employee
{
    /// <summary>
    /// 员工组织架构关联表（一对多）
    /// </summary>
    [Table("emp_employee_organization")]
    public class emp_employee_organization : BaseBaseEntity
    {
        /// <summary>
        /// 员工guid 关联表emp_employeeInfo
        /// </summary>
        public Guid employeeInfo_guid { get; set; }
        /// <summary>
        /// 组织架构guid 关联表：emp_organization
        /// </summary>
        public Guid organization_guid { get; set; }
    }
}
