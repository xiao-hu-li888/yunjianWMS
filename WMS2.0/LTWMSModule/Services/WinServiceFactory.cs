using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.Services
{
    public class RFIDBarcodeRequest
    {
        /// <summary>
        /// RFID条码/托盘条码
        /// </summary>
        public string rfidBarcode { get; set; }
        /// <summary>
        ///  入库扫码口编号1、2、3
        /// </summary>
        public int num { get; set; }
    }
    public class WinServiceFactory
    {
        /// <summary>
        /// 尾部条码值（每次取到后置空）
        /// </summary>
        public static string BarcodeOfEnd { get; set; }
        /// <summary>
        /// 根目录
        /// </summary>
        public static string rootDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        private static LTLibrary.MyLog _mylog;
        public static LTLibrary.MyLog Log
        {
            get
            {
                if (_mylog == null)
                {
                    bool _enablelog = LTLibrary.ConvertUtility.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["wms_enablelog"]);
                    if (_enablelog)
                    {
                        _mylog = new LTLibrary.MyLog(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    else
                    {
                        _mylog = new LTLibrary.MyLog(false);
                    }
                }
                return _mylog;
            }
        }
        public class Config
        {
            private static int? _refreshcycle;
            /// <summary>
            /// WMS服务刷新周期(毫秒)
            /// </summary>
            public static int RefreshCycle
            {
                get
                {
                    if (_refreshcycle == null)
                    {
                        int _rtnV = 200;//默认值 200毫秒
                        string _RefCycle = System.Configuration.ConfigurationManager.AppSettings["wms_refreshcycle"];
                        if (!string.IsNullOrWhiteSpace(_RefCycle))
                        {
                            _rtnV = LTLibrary.ConvertUtility.ToInt(_RefCycle);
                        }
                        if (_rtnV < 200)
                        {
                            _rtnV = 200;
                        }
                        else if (_rtnV > 5000)
                        {//小于5000毫秒
                            _rtnV = 5000;
                        }
                        _refreshcycle = _rtnV;
                    }
                    return _refreshcycle.Value;
                }
            }
      
            /*******tcp 客户端 *******/
            /// <summary>
            /// wcs ip
            /// </summary>
            public static string wcsIp { get => System.Configuration.ConfigurationManager.AppSettings["wms_wcsIp"]; }
            /// <summary>
            /// wcs port
            /// </summary>
            public static int wcsPort { get => LTLibrary.ConvertUtility.ToInt(System.Configuration.ConfigurationManager.AppSettings["wms_wcsPort"]); }
            public static string GetSqlConnectionStr
            {
                get => System.Configuration.ConfigurationManager.ConnectionStrings["LTWMSModel"].ConnectionString;
            }
            /// <summary>
            /// 1出库口LED屏IP
            /// </summary>
            public static string LedIp1
            {
                get => System.Configuration.ConfigurationManager.AppSettings["wms_ledip1"];
            }
            /// <summary>
            /// 2出库口LED屏IP
            /// </summary>
            public static string LedIp2
            {
                get => System.Configuration.ConfigurationManager.AppSettings["wms_ledip2"];
            }
            /// <summary>
            /// 3出库口LED屏IP
            /// </summary>
            public static string LedIp3
            {
                get => System.Configuration.ConfigurationManager.AppSettings["wms_ledip3"];
            }

            /// <summary>
            /// 4出库口LED屏IP
            /// </summary>
            public static string LedIp4
            {
                get => System.Configuration.ConfigurationManager.AppSettings["wms_ledip4"];
            }
            /// <summary>
            /// LED屏宽
            /// </summary>
            public static int LedWidth
            {
                get => LTLibrary.ConvertUtility.ToInt(System.Configuration.ConfigurationManager.AppSettings["wms_ledwidth"], 128);
            }
            /// <summary>
            /// LED屏高
            /// </summary>
            public static int LedHeight
            {
                get => LTLibrary.ConvertUtility.ToInt(System.Configuration.ConfigurationManager.AppSettings["wms_ledheight"],32);
            }
            /// <summary>
            /// 托盘条码正则表达式
            /// </summary>
            public static string RegexTrayBarcode
            {
                get => System.Configuration.ConfigurationManager.AppSettings["wms_regexTrayBarcode"];
            }
            /*****tcp 服务器端******/
            /// <summary>
            /// WMSServerIP（WMS提供的Tcp Socket访问接口服务端）
            /// </summary>
            public static string WMSServerIP { get => System.Configuration.ConfigurationManager.AppSettings["WMSServerIP"]; }
            /// <summary>
            /// WMSServerPort（WMS提供的Tcp Socket访问接口服务端）
            /// </summary>
            public static int WMSServerPort { get => LTLibrary.ConvertUtility.ToInt(System.Configuration.ConfigurationManager.AppSettings["WMSServerPort"]); }

        }
    }
}
