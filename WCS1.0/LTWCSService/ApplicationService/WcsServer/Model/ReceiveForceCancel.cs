using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer.Model
{
    /// <summary>
    /// WMS->>WCS 强制完成、取消任务
    /// </summary>
    public class ReceiveForceCancel
    {  
        /// <summary>
        ///  强制完成、取消
        /// </summary>
        public int cmd { get => 105; }
        public int seq;
        public int task_id;//:888,
        /// <summary>
        /// 0 强制完成、1取消 
        /// </summary>
        public int type; 
    }
}
