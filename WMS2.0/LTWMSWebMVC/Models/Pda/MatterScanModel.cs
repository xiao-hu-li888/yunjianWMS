using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Models.Pda
{
    public class MatterScanModel
    {
        /// <summary>
        /// 物料条码
        /// </summary>
        public String MatterBarcode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary> 
        public string Matter_Name { get; set; }
        /// <summary>
        /// 批次
        /// </summary> 
        public string lot_number { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public decimal number { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string producedate { get; set; }

        /// <summary>
        /// 有效日期
        /// </summary>
        public string effective_date { get; set; }

        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// 待检状态锁定出库！！！
        /// </summary>
        public string test_status { get; set; }
        /// <summary>
        /// 托盘物料关联表guid（表：wh_traymatter）
        /// </summary>
        public Guid? TrayMatter_Guid { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 是否检测
        /// </summary>
        public bool is_check_ok { get; set; }
        public string ShelfUnit_Pos { get; set; }
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string traybarcode { get; set; }
    }
}