using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.Model
{
    public class MatterBarcode
    {
        /// <summary>
        /// 物料条码
        /// </summary>
        public string matter_code { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        public decimal number { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string project_name { get; set; }
        /// <summary>
        /// 项目号
        /// </summary>
        public string project_no { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customer_name { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string matter_name { get; set; }
        /*********************/
        /// <summary>
        /// 入库单号
        /// </summary> 
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 批次
        /// </summary> 
        public string lot_number { get; set; }

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
        /// 待检状态锁定出库！！！
        /// </summary>
        public TestStatusEnum test_status { get; set; }


    }
}
