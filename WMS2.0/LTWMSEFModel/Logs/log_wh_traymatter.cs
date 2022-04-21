using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Logs
{
    /// <summary>
    /// 托盘物料绑定/解绑记录表
    /// </summary>
    [Table("log_wh_traymatter")]
    public class log_wh_traymatter : BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; } 
        /// <summary>
        /// 托盘条码
        /// </summary>
        [StringLength(50)]
        public string traybarcode { get; set; }
        /// <summary>
        /// 箱包装条码
        /// </summary>
        [StringLength(50)]
        public string print_barcode { get; set; }
        /// <summary>
        /// 物料guid（表：stk_matter）
        /// </summary>  
        public Guid? matter_guid { get; set; }
         
        /// <summary>
        /// 物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string matter_barcode { get; set; }
        /// <summary>
        /// 批次（订单号）
        /// </summary>
        [StringLength(50)]
        public string batch { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        public decimal single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal total_weight { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? producedate { get; set; }

        /// <summary>
        /// 托盘物料绑定类型(绑定：入库,解绑：出库)
        /// </summary>
        public TrayMatterBindType bind_type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 日志发生时间
        /// </summary> 
        public DateTime log_date { get; set; }
        /// <summary>
        /// 操作人员
        /// </summary>
        [StringLength(30)]
        public string operator_u { get; set; }

        ///// <summary>
        ///// 重量
        ///// </summary>
        //public decimal weight { get; set; }

    }
    /// <summary>
    /// 托盘物料绑定类型
    /// </summary>
    public enum TrayMatterBindType
    {
        /// <summary>
        ///  解绑
        /// </summary>
        [Description("解绑")]
        Unbind = 0,
        /// <summary>
        /// 绑定
        /// </summary>
        [Description("绑定")]
        Binding = 1
    }
}
