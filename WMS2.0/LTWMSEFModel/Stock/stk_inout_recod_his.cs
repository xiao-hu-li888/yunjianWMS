using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Stock
{
    /// <summary>
    /// 出入库流水--历史表
    /// </summary>
    [Table("stk_inout_recod_his")]
   public class stk_inout_recod_his:BaseEntity
    { /// <summary>
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
}
