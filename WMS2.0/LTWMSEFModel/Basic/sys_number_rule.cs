using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Basic
{
    /// <summary>
    /// 编号生成规则
    /// </summary>
    [Table("sys_number_rule")]
    public class sys_number_rule : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 规则名称（数据库中只能存在一行数据/可以修改，不能删除）
        /// </summary>
        public NumberRuleName rule_name { get; set; }
        /// <summary>
        /// 前缀 T-/Ord- 等开头
        /// </summary>
        [StringLength(20)]
        public string prefix { get; set; }
        /// <summary>
        /// 分割符（-/./...）
        /// </summary>
        [StringLength(2)]
        public string split_str { get; set; }
        /// <summary>
        /// 日期格式(yyyyMMdd-)
        /// </summary>
        [StringLength(20)]
        public string date_format { get; set; }
        /// <summary>
        /// 序号值的宽度（4：0001/7：0000001）
        /// </summary>
        public int number { get; set; }


        /// <summary>
        /// 最后生成的值--日期（年月日）
        /// </summary>
        public DateTime? last_date { get; set; }
        /// <summary>
        /// 最后生成的值--增长（每天从1开始增长）
        /// </summary> 
        public int last_increase { get; set; }
        /// <summary>
        /// 最后生成的值
        /// </summary>
        [StringLength(50)]
        public string last_value { get; set; }

    }
    /// <summary>
    ///编号生成规则名称 
    /// </summary> 
    public enum NumberRuleName
    {
        /// <summary>
        /// 入库单号
        /// </summary>
        [Description("入库单号")]
        BillStockIn = 0,
        /// <summary>
        /// 出库单号
        /// </summary>
        [Description("出库单号")]
        BillStockOut = 1
    }
}
