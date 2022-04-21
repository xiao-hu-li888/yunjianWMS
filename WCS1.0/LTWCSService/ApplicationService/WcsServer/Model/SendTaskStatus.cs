using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer.Model
{
    /// <summary>
    /// 发送任务执行状态
    /// </summary>
    public class SendTaskStatus
    {
        public int cmd { get => 201; }
        public int seq;///:1,
	    public int task_id;//:888,
        /// <summary>
        ///-1写入失败 0=未处理  1=任务执行中   2=任务暂停   3=任务完成   4=任务异常  5=任务取消 6=任务强制完成
        /// </summary>
        public int task_status;//:0,	//-1写入失败 0=未处理  1=任务执行中   2=任务暂停   3=任务完成   4=任务异常  5=任务取消 6=任务强制完成
        public string task_info;//"successful"	
        /// <summary>
        /// 流程字
        /// </summary>
        public int flow;
    }
}
