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
    /// 项目发货详情
    /// </summary>
    [Table("pro_project_deliver")]
    public class pro_project_deliver : BaseEntity
    {
        /// <summary>
        /// 关联项目表：pro_project
        /// </summary>
        public Guid project_guid { get; set; }
        /// <summary>
        /// 管理项目明细guid（表：pro_project_details）
        /// </summary>
        public Guid project_details_guid { get; set; }
        /// <summary>
        /// 说明(交货说明)
        /// </summary>
        [StringLength(500)]
        public string desc { get; set; }
        /// <summary>
        /// 应交货日期
        /// </summary>
        public DateTime? delivery_datetime { get; set; }
        /// <summary>
        /// 实际交货日期
        /// </summary>
        public DateTime? actual_delivery_datetime { get; set; }
        /// <summary>
        /// 交货状态（提前、正常、延迟 交货）
        /// </summary>
        public DeliveryStatusEnum delivery_status { get; set; }

    }
    /// <summary>
    /// 交货状态（提前、正常、延迟 交货）
    /// </summary>
    public enum DeliveryStatusEnum
    {
        /// <summary>
        /// 提前交货
        /// </summary>
        [Description("提前交货")]
        Advance = 0,
        /// <summary>
        /// 正常交货
        /// </summary>
        [Description("正常交货")]
        Normal = 1,
        /// <summary>
        /// 延迟交货
        /// </summary>
        [Description("延迟交货")]
        Delay = 2
    }
}
