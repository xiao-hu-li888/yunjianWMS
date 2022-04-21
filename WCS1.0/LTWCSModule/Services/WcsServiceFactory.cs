using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSModule.Services
{
    public class WcsServiceFactory
    {
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
                    bool _enablelog = LTLibrary.ConvertUtility.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["wcs_enablelog"]);
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
                        int _rtnV = 100;//默认值 100毫秒
                        string _RefCycle = System.Configuration.ConfigurationManager.AppSettings["wcs_refreshcycle"];
                        if (!string.IsNullOrWhiteSpace(_RefCycle))
                        {
                            _rtnV = LTLibrary.ConvertUtility.ToInt(_RefCycle);
                        }
                        if (_rtnV < 100)
                        {
                            _rtnV = 100;
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
            public static string wcsIp { get => System.Configuration.ConfigurationManager.AppSettings["wcs_wmsIp"]; }
            /// <summary>
            /// wcs port
            /// </summary>
            public static int wcsPort { get => LTLibrary.ConvertUtility.ToInt(System.Configuration.ConfigurationManager.AppSettings["wcs_wmsPort"]); }

            /*********************堆垛机PLC信息配置**************************/
            /// <summary>
            /// 堆垛机IP地址
            /// </summary>
            public static string StackerIp2001 { get => System.Configuration.ConfigurationManager.AppSettings["stackerip2001"]; }

            /// <summary>
            /// INT(16位) 堆垛机状态(1自动准备好 2运行 3故障)  状态为1可以下发启动命令
            /// </summary>
            public static string dbStackerStatus { get => System.Configuration.ConfigurationManager.AppSettings["dbStackerStatus"]; }

            /// <summary>
            /// INT(16位) 任务完成（1任务完成 0未完成）  每次任务完成后为1
            /// </summary>
            public static string dbTaskStatus { get => System.Configuration.ConfigurationManager.AppSettings["dbTaskStatus"]; }

            /// <summary>
            /// INT(16位) 流程字（1货叉归中 2去取货点 3取货伸叉 4 取货抬起 5取货缩回 6去放货点 7放货伸叉 8放货下降 9放货缩回）
            /// </summary>
            public static string dbFlow { get => System.Configuration.ConfigurationManager.AppSettings["dbFlow"]; }

            /// <summary>
            /// INT(16位) 启动标志（0默认未启动 1入库 2出库 3站内中转）
            /// </summary>
            public static string dbBoot { get => System.Configuration.ConfigurationManager.AppSettings["dbBoot"]; }

            /// <summary>
            /// DINT(32位) 任务号（不等于0）
            /// </summary>
            public static string dbTaskId { get => System.Configuration.ConfigurationManager.AppSettings["dbTaskId"]; }

            /// <summary>
            /// INT（16位） 起点排
            /// </summary>
            public static string dbSrcRack { get => System.Configuration.ConfigurationManager.AppSettings["dbSrcRack"]; }

            /// <summary>
            /// INT（16位） 起点列
            /// </summary>
            public static string dbSrcCol { get => System.Configuration.ConfigurationManager.AppSettings["dbSrcCol"]; }

            /// <summary>
            /// INT（16位） 起点层
            /// </summary>
            public static string dbSrcRow { get => System.Configuration.ConfigurationManager.AppSettings["dbSrcRow"]; }

            /// <summary>
            /// 起点站台（扩展用）
            /// </summary>
            public static string dbSrcStation { get => System.Configuration.ConfigurationManager.AppSettings["dbSrcStation"]; }

            /// <summary>
            /// INT（16位） 终点排
            /// </summary>
            public static string dbDestRack { get => System.Configuration.ConfigurationManager.AppSettings["dbDestRack"]; }

            /// <summary>
            /// INT（16位） 终点列
            /// </summary>
            public static string dbDestCol { get => System.Configuration.ConfigurationManager.AppSettings["dbDestCol"]; }

            /// <summary>
            /// INT（16位） 终点层
            /// </summary>
            public static string dbDestRow { get => System.Configuration.ConfigurationManager.AppSettings["dbDestRow"]; }

            /// <summary>
            /// 终点站台（纵深1、2） 扩展用
            /// </summary>
            public static string dbDestStation { get => System.Configuration.ConfigurationManager.AppSettings["dbDestStation"]; }

            /// <summary>
            /// 100出库准备好
            /// </summary>
            public static string dbReady100 { get => System.Configuration.ConfigurationManager.AppSettings["dbReady100"]; }

            /// <summary>
            /// 200出库准备好
            /// </summary>
            public static string dbReady200 { get => System.Configuration.ConfigurationManager.AppSettings["dbReady200"]; }

            /// <summary>
            /// 300出库准备好
            /// </summary>
            public static string dbReady300 { get => System.Configuration.ConfigurationManager.AppSettings["dbReady300"]; }

            /// <summary>
            /// 400出库准备好
            /// </summary>
            public static string dbReady400 { get => System.Configuration.ConfigurationManager.AppSettings["dbReady400"]; }
        }
    }
}
