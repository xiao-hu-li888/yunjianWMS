using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Models.Erp
{
    public class ResponseBase<T> where T:class
    {
        /// <summary>
        /// 消息编码
        /// </summary>
        public ResponseCode code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 分页的总条数
        /// </summary>
        public int totalcount { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> data { get; set; }
    }
}