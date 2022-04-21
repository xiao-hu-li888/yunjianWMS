using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.Services.Models
{
    /// <summary>
    /// 出入库流水记录表
    /// </summary>
    public class InoutRecordModel
    {
        /// <summary>
        /// 厂别固定为1199
        /// </summary>
        public string shop_code { get => "1199"; }
        /// <summary>
        /// 仓库固定为1001
        /// </summary>
        public string location_id { get => "1001"; }
        /// <summary>
        /// 固定为1001加6个8
        /// </summary>
        public string carton_code { get => "1001888888"; }
        /// <summary>
        /// sap料号 
        /// </summary>
        public string goods_id { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string spec_id { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal qty { get; set; }
    }
}