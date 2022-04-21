using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer.Model
{
    /// <summary>
    /// WCS->WMS 尾部人工扫码，将电池放至agv，wms请求pcs电池朝向并反馈给agv
    /// </summary>
    public class ReceiveBarcodeOfEnd
    {
        /// <summary>
        /// 入库扫码口
        /// </summary>
        public int cmd { get => 204; }
        public int seq;//:1,
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        ///扫码入库口编号： 1、2、3
        /// </summary>
        public int num { get; set; }  
    }
}
