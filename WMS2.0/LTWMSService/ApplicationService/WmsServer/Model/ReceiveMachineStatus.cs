using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    //WCS->WMS 发送所有设备状态  在WMS成功连接WCS系统后，WCS会主动每隔2秒更新一次
    public class ReceiveMachineStatus
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
        /// // 0= 未启动  1 = 正常	2=中断运行（WMS收到此状态后应该暂停任务派发）  
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
