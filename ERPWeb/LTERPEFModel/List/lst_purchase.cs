using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.List
{
    /// <summary>
    /// 采购清单(主表)
    /// </summary>
    [Table("lst_purchase")]
    public class lst_purchase : BaseEntity
    {
        /// <summary>
        /// 关联项目表：pro_project
        /// </summary>
        public Guid project_guid { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [StringLength(50)]
        public string project_name { get; set; }

        /// <summary>
        ///设计人员（表：emp_employeeInfo）
        /// </summary>
        public Guid? employeeInfo_guid { get; set; }

        /// <summary>
        /// 设计人员
        /// </summary>
        [StringLength(20)]
        public string designer { get; set; }

        /// <summary>
        /// 关联项目产品guid（表：pro_project_matter）
        /// </summary>
        public Guid? project_matter_guid { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime add_datetime { get; set; }
        /// <summary>
        /// 要求到位日期
        /// </summary>
        public DateTime? requirement_datetime { get; set; }
        //设计清单=》》设计采购单
        //电器采购单
        //自制件采购单
        /// <summary>
        /// 审核状态
        /// </summary>
        public AuditStatusEnum audit_status { get; set; }
        /// <summary>
        /// 采购单类型
        /// </summary>
        public PurchaseTypeEnum purchase_type { get; set; }


    }
    /// <summary>
    /// 采购单类型
    /// </summary>
    public enum PurchaseTypeEnum
    {
        /// <summary>
        /// 机械采购清单
        /// </summary>
        [Description("机械采购清单")]
        MachineDesign = 0,
        /// <summary>
        /// 电器采购清单
        /// </summary>
        [Description("电器采购清单")]
        Electrical = 1,
        /// <summary>
        /// 自制件清单
        /// </summary>
        [Description("自制件清单")]
        SelfMade = 2
    }
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum AuditStatusEnum
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        None = 0,
        /// <summary>
        /// 审核成功
        /// </summary>
        [Description("审核成功")]
        Ok = 1,
        /// <summary>
        /// 审核失败
        /// </summary>
        [Description("审核失败")]
        Failed = 2
    }
}
