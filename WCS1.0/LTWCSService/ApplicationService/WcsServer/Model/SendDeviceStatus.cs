using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer.Model
{
    /// <summary>
    /// 推送设备状态
    /// </summary>
    public class SendDeviceStatus
    {
        public int cmd { get => 202; }
        public int seq;//:1,
        public int dev_count;//:123,设备数量
        public List<DevInfo> dev_info;//设备状态数组	 
    }
    public class DevInfo
    {
        /// <summary>
        /// 设备ID号  WCS控制系统本身 ID = 1
        /// </summary>
        public int dev_id;
        /// <summary>
        /// // 0= 未启动  1 = 准备好	2=运行中 3=报警中
        /// </summary>
        public int status;
        /// <summary>
        /// 警代码
        /// </summary>
        public int error_code;
        /// <summary>
        /// 报警信息
        /// </summary>
        public string error_msg;
    }
}   
