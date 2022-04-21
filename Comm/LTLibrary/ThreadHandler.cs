using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary
{ 
    public  class ThreadHandler
    {
        /// <summary>
        /// 处理服务
        /// </summary>
        public IThreadService Service;
        ///// <summary>
        ///// 对应仓库guid
        ///// </summary>
        //public Guid WareHouseGuid;
        /// <summary>
        /// 对应wcs服务guid
        /// </summary>
        public Guid Wcs_srv_guid;
          
        /// <summary>
        /// Ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
       /// 线程名称
       /// </summary>
        public string Name;
        /// <summary>
        /// 重启线程，如果线程意外终止
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
        /// 超过多少秒报警?重启线程？
        /// </summary>
        public int AlarmSecond;

    }
}
