using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class StackerTaskQueueSearch
    {
        /// <summary>
        /// 是否启用发送任务至堆垛机
        /// </summary>
        public bool IsSendToStacker { get; set; }
        public List<StackerTaskQueueModel> PageCont { get; set; }
      
    }
}