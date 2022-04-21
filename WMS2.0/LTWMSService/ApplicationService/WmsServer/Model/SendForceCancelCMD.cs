using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// WMS->>WCS 强制完成、取消任务
    /// </summary>
    public class SendForceCancelCMD
    {
        /// <summary>
        ///  强制完成、取消
        /// </summary>
        public int cmd { get => 105; }
        public int seq;
        public int task_id;//:888,
        /// <summary>
        /// 0 强制完成、1取消 2、任务异常  【 3（1#rgv）、4（2#rgv）、5（3#rgv）】结束任务！！！
        /// </summary>
        public int type;
    }
}
