using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.AppCode
{
    public class ThreadHandler
    {
        /// <summary>
        /// 线程名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 重启日志线程，如果线程意外终止
        /// </summary>
        public bool ThreadReboot;
        /// <summary>
        /// 线程退出标志
        /// </summary>
        public bool Exit;
        /// <summary>
        /// 线程对象
        /// </summary>
        public System.Threading.Thread thread;
        public delegate void ThreadStart(object threadhandler);
        public ThreadStart threadstart;
        /// <summary>
        /// 最近一轮开始时间
        /// </summary>
        public DateTime? lastBeginDate;
        /// <summary>
        /// 最近一轮结束时间
        /// </summary>
        public DateTime? lastEndDate;
        /// <summary>
        /// 超过多少(秒)报警?重启线程？
        /// </summary>
        public int AlarmSecond;
    }
}