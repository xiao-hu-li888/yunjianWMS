using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// wms->wcs返回预分配的站台编号 1-30
    /// </summary>
    public class SendInStationCMD
    {
        public int cmd { get => 107; }
        public int seq { get; set; }
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string barcode;  
        /// <summary>
        /// RGV终点站台 2、4、6、8、10、12
        /// </summary>
        public int station;

        /// <summary>
        /// 入库口编号： 1、2、3 
        /// </summary>
        public int num;

    }
}
