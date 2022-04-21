using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity.Validation;

using System.Text;



namespace LTWMSWebMVC
{
    public class WMSFactory
    {
        public class AuthorCookieHelper
        {
            private const string cookiename = "Author-LT-WMSWebMVC";
            /// <summary>
            /// 
            /// </summary>
            /// <param name="token"></param>
            /// <param name="JwtExp">jwt过期时间(单位：秒)</param>
            public static void AddToken(string token, int JwtExp)
            {
                HttpCookie objCookie = new HttpCookie(cookiename);
                objCookie.Value = token;
                objCookie.Expires = DateTime.Now.AddSeconds(JwtExp);
                HttpContext.Current.Response.Cookies.Add(objCookie);
            }
            public static string GetToken()
            {
                var val = HttpContext.Current.Request.Cookies.Get(cookiename);
                if (val != null)
                {
                    return val.Value;
                }
                return "";
            }
            /// <summary>
            /// 删除token
            /// </summary>
            /// <returns></returns>
            public static void RemoveToken()
            {
                HttpContext.Current.Response.Cookies[cookiename].Expires = DateTime.Now.AddDays(-1);
            }
        }
        private static LTLibrary.MyLog _mylog;
        public static LTLibrary.MyLog Log
        {
            get
            {
                if (_mylog == null)
                {
                    bool _enablelog = LTLibrary.ConvertUtility.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["enablelog"]);
                    if (_enablelog)
                    {
                        //_mylog = new LTLibrary.MyLog(AppDomain.CurrentDomain.BaseDirectory);
                        _mylog = new LTLibrary.MyLog(System.Web.Hosting.HostingEnvironment.MapPath("~/"));
                        //HttpContext.Current.Server.MapPath("~/");
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
            ///// <summary>
            ///// jwt过期时间(单位：秒)
            ///// </summary>
            //public static int JwtExp
            //{
            //    get
            //    {
            //        return LTLibrary.ConvertUtility.ToInt(ConfigurationManager.AppSettings["jwtexp"]);
            //    }
            //}
            /****************************************/
            /// <summary>
            ///  下发任务
            /// </summary>
            public static string AgvApiSendTask
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvapi_agvtask"]);
                }
            }

            /// <summary>
            ///  电池方向
            /// </summary>
            public static string AgvApiBatteryDirect
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvapi_batterydirection"]);
                }
            }

            /// <summary>
            ///  辊筒对接
            /// </summary>
            public static string AgvApiRoller
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvapi_roller"]);
                }
            }

            /// <summary>
            ///  取消任务
            /// </summary>
            public static string AgvApiCancelTask
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvapi_canceltask"]);
                }
            }
            public static string GetSqlConnectionStr
            {
                get
                { 
                    return ConfigurationManager.ConnectionStrings["LTWMSModel"].ConnectionString;
                }
            }
            /****************************************/
            /// <summary>
            /// Agvapi测试连接地址
            /// </summary>
            public static string AgvApi_TestConnect
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvapi_testconnect"]);
                }
            }
            /// <summary>
            /// 艾华入库流水接收地址
            /// </summary>
            public static string AihuaStockInURL
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["aihua_stockin_url"]);
                }
            }
            /// <summary>
            /// 艾华出库流水接收地址
            /// </summary>
            public static string AihuaStockOutURL
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["aihua_stockout_url"]);
                }
            }
            /// <summary>
            /// 艾华解析二维码返回物料信息接口
            /// </summary>
            public static string AihuaAnalysisMatterURL
            {
                get
                {
                    return ConfigurationManager.AppSettings["aihua_analysismatter_url"];
                }
            }
            /// <summary>
            /// Agv调度服务ip
            /// </summary>
            public static string AgvServerHttpURL
            {
                get
                {
                    return Convert.ToString(ConfigurationManager.AppSettings["agvserverurl"]);
                }
            }
            /// <summary>
            /// Agv调度服务端口
            /// </summary>
            public static int AgvServerPort
            {
                get
                {
                    return LTLibrary.ConvertUtility.ToInt(ConfigurationManager.AppSettings["agvserverport"]);
                }
            }
            /// <summary>
            /// Agv目的站点(R1,R2,R3,R4)
            /// </summary>
            public static string AgvDestinationList
            {
                get
                {
                    return ConfigurationManager.AppSettings["agvdestinationlist"];
                }
            }
            /// <summary>
            /// 匹配电池条码表达式
            /// </summary>
            public static string RegexBatteryBarcode
            {
                get => ConfigurationManager.AppSettings["regexbatterybarcode"];
            }
            public static string WmsServerWebUrl
            {
                get => ConfigurationManager.AppSettings["wmsserverweburl"];
            }
            /// <summary>
            /// 网站标题
            /// </summary>
            public static string WebSiteTitle
            {
                get => ConfigurationManager.AppSettings["websitetitle"];
            }
            public static string ErpMatterUrl
            {
                get => ConfigurationManager.AppSettings["ErpMatterUrl"];
            }
            /// <summary>
            /// 本地打印服务URL
            /// </summary>

            public static string PrintServerURL
            {
                get => ConfigurationManager.AppSettings["PrintServerURL"];
            }
        }
    }
}