using LTWMSWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LTWMSWebMVC.Controllers.PDA
{
    public class Test_PdaController : ApiBaseController
    {
        /// <summary>
        /// 测试通过电池条码(T01-1-1)获取详细信息
        /// </summary>
        /// <param name="barcorde"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage BatteryInfoByBarcode(string barcorde)
        {
            try
            {
                List<BatteryKeyValues> lstData = new List<BatteryKeyValues>();
                lstData.Add(new BatteryKeyValues() { Text="尺寸", Val="100mmx500mm" });
                lstData.Add(new BatteryKeyValues() { Text = "规格", Val = "AL-1" });
                lstData.Add(new BatteryKeyValues() { Text = "重量", Val = "15KG" });
                lstData.Add(new BatteryKeyValues() { Text = "颜色", Val = "黑色" });
                return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = lstData});
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }

        public class BatteryKeyValues
        {
            /// <summary>
            /// 显示
            /// </summary>
            public string Text;
            /// <summary>
            /// 值
            /// </summary>
            public string Val;
        }
    }
}
