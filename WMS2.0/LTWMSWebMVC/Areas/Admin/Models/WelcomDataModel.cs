using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Admin.Models
{
    public class WelcomDataModel
    {
        /// <summary>
        /// 当日入库
        /// </summary>
        public int StockIn { get; set; }
        /// <summary>
        /// 当日出库
        /// </summary>
        public int StockOut { get; set; }
        /// <summary>
        /// 当日移库
        /// </summary>
        public int StockMove { get; set; }
        public List<WelcomeDataObj> List { get; set; }
        /// <summary>
        /// 使用
        /// </summary>
        public double Used { get; set; }
        /// <summary>
        /// 未使用
        /// </summary>
        public double UnUsed { get; set; }
        /// <summary>
        /// 电池总计
        /// </summary>
        public double BatteryCount { get; set; }
        /// <summary>
        /// 其它物料总计
        /// </summary>
        public double OtherMatterCout { get; set; }
        /// <summary>
        /// 空托盘总计
        /// </summary>
        public double EmptyCout { get; set; }
    }
    // { xname: '3月1日', '出库': 43.3, '入库': 85.8 },
    public class WelcomeDataObj
    {
        /// <summary>
        /// x轴显示
        /// </summary>
        public string xname { get; set; }
        public int stockin { get; set; }
        public int stockout { get; set; }
    }
}