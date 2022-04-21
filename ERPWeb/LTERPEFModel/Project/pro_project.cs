using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Project
{
    /// <summary>
    /// 项目主表（签一份合同相当于一个项目？）
    /// </summary>
    [Table("pro_project")]
    public class pro_project : BaseEntity
    {
        /// <summary>
        /// 关联项目guid（表：pro_project）
        /// </summary>
        public Guid? ref_project_guid { get; set; }

        /// <summary>
        /// 项目序号（控制自增，保存的时候自动获取，避免浪费）
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 项目编号 SC-202003-001   （一个合同生产一个项目号：一个合同下可能有几个项目）
        ///           SC-202003-001-2 增补合同 
        /// </summary>
        [StringLength(50)]
        [Required]
        public string code { get; set; }
        /// <summary>
        /// 项目名称/合同名称
        /// </summary>
        [StringLength(50)]
        public string proj_name { get; set; }
        /// <summary>
        /// 关联客户（表：con_customer）
        /// </summary>
        public Guid customer_guid { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [StringLength(50)]
        public string customer_name { get; set; }
        /// <summary>
        /// 项目经理/负责人（表：emp_employeeInfo）
        /// </summary>
        public Guid? employeeInfo_guid { get; set; }
        /// <summary>
        /// 项目经理/负责人
        /// </summary>
        [StringLength(50)]
        public string employee_name { get; set; }
        /// <summary>
        /// 合同生效时间
        /// </summary>
        [Required]
        public DateTime effective_datetime { get; set; }
   
        /// <summary>
        /// 项目进行状态
        /// </summary>
        public ProjectStatusEnum pro_status { get; set; }
        /// <summary>
        /// 重要等级
        /// </summary>
        public ProjectLevelEnum pro_level { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        public string memo { get; set; }
    }
    /// <summary>
    /// 重要等级
    /// </summary>
    public enum ProjectLevelEnum
    {
        /// <summary>
        /// 一般
        /// </summary>
        [Description("一般")]
        Normal = 0,
        /// <summary>
        /// 紧急
        /// </summary>
        [Description("紧急")]
        Urgent = 1,
        /// <summary>
        /// 重大
        /// </summary>
        [Description("重大")]
        Major = 2
    }
    /// <summary>
    /// 项目进行状态
    /// </summary>
    public enum ProjectStatusEnum
    {
        /// <summary>
        /// 未开始(默认)
        /// </summary>
        [Description("未开始")]
        None = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Running = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 2,
        /// <summary>
        /// 暂停
        /// </summary>
        [Description("暂停")]
        Paused = 3,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Canceled = 4
    }
}
