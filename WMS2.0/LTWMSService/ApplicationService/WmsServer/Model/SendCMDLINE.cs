using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// wms-->wcs 执行命令行
    /// </summary>
    public class SendCMDLINE
    {
        public int cmd { get => 115; }
        public int seq;
        /// <summary>
        ///执行命令 s7_bit_set(2201,'DB201X1.4',1)
        /// </summary>
        public string cmdline;
    }
}
