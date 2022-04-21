using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Models.Pda
{
    public class TrayMatterBarcodeModel
    {
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string traybarcode;
        /// <summary>
        /// 电池条码数组
        /// </summary>
        public List<string> matterbarcode;
    }
}