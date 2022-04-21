using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.query_model
{
    /// <summary>
    ///视图模型： 单据汇总（打印）
    /// </summary>
    public class BillsGather
    {
        /// <summary>
        /// 出入库类型
        /// </summary>
        public BillsInOutEnum bills_type { get; set; }


        /// <summary>
        /// 入库类型
        /// </summary>
        public StockInType stockin_type { get; set; }

        /// <summary>
        /// 出库类型
        /// </summary>
        public StockOutType stockout_type { get; set; }



        /// <summary>
        /// 出库、入库单号
        /// </summary>
        [StringLength(50)]
        public string odd_numbers { get; set; }
        /// <summary>
        /// 出库、入库日期
        /// </summary>
        public DateTime inout_date { get; set; }

        /// <summary>
        /// 物料条码
        /// </summary>
        [StringLength(100)]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }
        /// <summary>
        /// 批次号（不填自动生成）
        /// </summary> 
        [StringLength(50)]
        public string lot_number { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int in_out_number { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? producedate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? effective_date { get; set; }

        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// </summary>
        public TestStatusEnum test_status { get; set; }

    }
    public enum BillsInOutEnum
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        BillsIn =0,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        BillsOut =1
    }
}
