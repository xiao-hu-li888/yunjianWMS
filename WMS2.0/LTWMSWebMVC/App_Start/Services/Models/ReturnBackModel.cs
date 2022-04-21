using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.Services.Models
{
    public class ReturnBackModel
    {
        /// <summary>
        /// 0成功，非0失败
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string msg { get; set; }

    }
}