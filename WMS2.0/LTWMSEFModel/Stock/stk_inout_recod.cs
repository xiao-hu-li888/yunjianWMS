using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Stock
{
    /// <summary>
    /// 出入库流水
    /// </summary>
    [Table("stk_inout_recod")]
    public class stk_inout_recod : BaseEntity
    {
        /// <summary>
        /// sap料号
        /// </summary>
        [StringLength(50)]
        public string goods_id { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        [StringLength(50)]
        public string spec_id { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal qty { get; set; }
        /// <summary>
        /// 流水是否推送至其它系统
        /// </summary>
        public IsSendToEnum is_send { get; set; }
        /// <summary>
        /// 发送失败次数（超过3次发送失败报警提示？？？）
        /// </summary>
        public int error_count { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 出入库类型
        /// </summary>
        public InOutTypeEnum inout_type { get; set; }
    }
    /// <summary>
    /// 出入库类型
    /// </summary>
    public enum InOutTypeEnum
    {
        /// <summary>
        /// 入库流水
        /// </summary>
        [Description("入库")]
        In = 0,
        /// <summary>
        /// 出库流水
        /// </summary>
        [Description("出库")]
        Out = 1
    }
    /// <summary>
    /// 流水是否推送至其它系统
    /// </summary>
    public enum IsSendToEnum
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        None = 0,
        /// <summary>
        /// 已发送
        /// </summary>
        [Description("已发送")]
        Sended = 1,
        /// <summary>
        /// 发送失败
        /// </summary>
        [Description("发送失败")]
        Failed = 2
    }
}
