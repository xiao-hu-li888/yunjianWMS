using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// 接收WCS入库指令
    /// </summary>
    public class ReceiveStockIn
    {
        public int cmd { get => 203; } //请求入库
        public int seq { get; set; }//:1,
        /// <summary>
        /// 1=A口(1排货架)    2=  B口（2排货架）
        /// </summary>
        public int station { get; set; }
        /// <summary>
        /// 条形码1
        /// </summary>
        public string x_1_barcode;
        /// <summary>
        /// 条形码2
        /// </summary>
        public string x_2_barcode; 
    }

}
