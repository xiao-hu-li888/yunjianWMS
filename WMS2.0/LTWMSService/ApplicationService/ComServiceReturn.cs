using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService
{
    /// <summary>
    /// 通用返回结果（方法中不带事务，事务写在controller或win服务器调用层）
    /// </summary>
    public class ComServiceReturn
    {
        /// <summary>
        /// 返回结果（成功，失败）
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 结果（失败原因）
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 结果集
        /// </summary>
        public object data { get; set; }
    }
   
}
