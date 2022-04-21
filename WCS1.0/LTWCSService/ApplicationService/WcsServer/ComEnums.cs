using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer
{
    public enum DeviceStatusEnum
    {
        /// <summary>
        /// 默认无状态（未准备）
        /// </summary>
        None=0,
        /// <summary>
        /// 准备好
        /// </summary>
        Ready=1,
        /// <summary>
        /// 运行中
        /// </summary>
        Running=2,
        /// <summary>
        /// 报警中
        /// </summary>
        Warnning=3
    } 
}
