using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.StockInOut.Data
{
    /// <summary>
    /// 艾华解析二维码返回的物料信息
    /// </summary>
    public class AHMatterInfosModel
    {
        /// <summary>
        /// 批次
        /// </summary>
        public string spec_id { get; set; }
        /// <summary>
        /// sap料号 
        /// </summary>
        public string goods_id { get; set; }
        /// <summary>
        /// 采购订单
        /// </summary>
        public string order_code { get; set; }
        /// <summary>
        /// 供应商代码
        /// </summary>
        public string vendor_code { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal qty { get; set; }

    }
}