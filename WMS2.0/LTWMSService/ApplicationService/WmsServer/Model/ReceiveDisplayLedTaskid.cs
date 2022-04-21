using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// WCS-WMS 托盘到达出库口
    /// </summary>
    public class ReceiveDisplayLedTaskid
    {
        //{"cmd":205,"seq":1,"task_id":1000}
        public int cmd { get => 205; }
        public int seq;//:1,
        /// <summary>
        /// 根据对应的任务id查找对应的托盘条码，将物料等信息显示在对应的led上面
        /// </summary>
        public int task_id;

    }
}
