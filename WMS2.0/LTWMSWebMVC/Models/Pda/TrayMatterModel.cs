using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Models.Pda
{
    [Serializable]
    public class TrayMatterModel
    {
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string traybarcode { get; set; }
        public MatterInfo[] data { get; set; }
    }
    [Serializable]
    public class MatterInfo
    {
        /// <summary>
        /// 物料条码
        /// </summary>
        public string MatterBarcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 是否检测OK
        /// </summary>
        public bool is_check_ok { get; set; }
    }
}