using LTLibrary;
using LTWCSModule.Services;
using LTWCSService.ApplicationService.WcsServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSModule
{

    /// <summary>
    /// WCS处理入库（客户端）
    /// </summary>
    public class WCSServiceClient
    {
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        private WCSReceiveService wcsreceiveSrv = new WCSReceiveService();
        LTProtocol.Tcp.Socket_Client SocketClient;
        private List<ThreadHandler> threadList = new List<ThreadHandler>();
        /// <summary>
        /// 堆垛机连接对象
        /// </summary>
        StackerService stackerSrv;
        public WCSServiceClient()
        {
            //处理wcs tcp 连接
            ThreadHandler threadWcsTcp = new ThreadHandler();
            threadWcsTcp.Name = "WCS tcp Socket 客户端线程";
            threadWcsTcp.ThreadReboot = true;
            threadWcsTcp.Exit = false;
            threadWcsTcp.AlarmSecond = 120;//120秒无响应重启线程
            threadWcsTcp.threadstart = threadWcsClientServiceStart;
            threadList.Add(threadWcsTcp);
            //连接PLC线程
            ThreadHandler threadPLCTcp = new ThreadHandler();
            threadPLCTcp.Name = "PLC连接线程";
            threadPLCTcp.ThreadReboot = true;
            threadPLCTcp.Exit = false;
            threadPLCTcp.AlarmSecond = 120;//120秒无响应重启线程
            threadPLCTcp.threadstart = threadPLCServiceStart;
            threadList.Add(threadPLCTcp);
        }
        public void threadPLCServiceStart(object threadhandler)
        {
            ThreadHandler thd = threadhandler as ThreadHandler;
            Services.WcsServiceFactory.Log.v("启动线程：" + thd.Name);
            stackerSrv = new StackerService(WcsServiceFactory.Config.StackerIp2001, S7.Net.CpuType.S71200,
               new StackerConfig()
               {
                   dbStackerStatus = WcsServiceFactory.Config.dbStackerStatus,
                   dbTaskStatus = WcsServiceFactory.Config.dbTaskStatus,
                   dbFlow = WcsServiceFactory.Config.dbFlow,
                   dbBoot = WcsServiceFactory.Config.dbBoot,
                   dbTaskId = WcsServiceFactory.Config.dbTaskId,
                   dbSrcRack = WcsServiceFactory.Config.dbSrcRack,
                   dbSrcCol = WcsServiceFactory.Config.dbSrcCol,
                   dbSrcRow = WcsServiceFactory.Config.dbSrcRow,
                   dbSrcStation = WcsServiceFactory.Config.dbSrcStation,
                   dbDestRack = WcsServiceFactory.Config.dbDestRack,
                   dbDestCol = WcsServiceFactory.Config.dbDestCol,
                   dbDestRow = WcsServiceFactory.Config.dbDestRow,
                   dbDestStation = WcsServiceFactory.Config.dbDestStation,
                   dbReady100 = WcsServiceFactory.Config.dbReady100,
                   dbReady200 = WcsServiceFactory.Config.dbReady200,
                   dbReady300 = WcsServiceFactory.Config.dbReady300,
                   dbReady400 = WcsServiceFactory.Config.dbReady400
               });
            stackerSrv.onLogHandler += StackerSrv_onLogHandler;
            while (true)
            {
                try
                {
                    if (exitServer)
                    {
                        break;
                    }
                    if (thd.Exit)
                    {
                        break;
                    }
                    if (!stackerSrv.GetConnected())
                    {
                        thd.lastBeginDate = null;
                        thd.lastEndDate = null;
                        stackerSrv.Connect();
                    }
                    thd.lastBeginDate = DateTime.Now;
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(Services.WcsServiceFactory.Config.RefreshCycle);
                    thd.lastEndDate = DateTime.Now;
                }
                catch (Exception ex)
                {

                }
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;
        }

        private void StackerSrv_onLogHandler(string logs)
        {
            Services.WcsServiceFactory.Log.v("StackerSrv Logs>>>" + logs);
        }

        public void threadWcsClientServiceStart(object threadhandler)
        {
            ThreadHandler thd = threadhandler as ThreadHandler;
            Services.WcsServiceFactory.Log.v("启动线程：" + thd.Name);
            SocketClient = new LTProtocol.Tcp.Socket_Client(
                Services.WcsServiceFactory.Config.wcsIp
                  , Services.WcsServiceFactory.Config.wcsPort, LTProtocol.Tcp.SocketClientEncoding.UTF8);
            SocketClient.onLogHandler += _socketServer_onLogHandler;
            SocketClient.onReceiveHandler += Client_onReceiveHandler;
            SocketClient.onDisConnectHandler += Client_onDisConnectHandler;
            WCSDealSendService wcsdsServer = new WCSDealSendService();
            //wcs服务已启动
            wcsdsServer.WCSStarted();
            // System.Threading.Thread.Sleep(20000);
            while (true)
            {
                try
                {
                    if (exitServer)
                    {
                        break;
                    }
                    try
                    {
                        if (thd.Exit)
                        {
                            break;
                        }
                        //减少数据库的访问频率？？
                        if (!SocketClient.IsConnected)
                        {
                            wcsdsServer.AddStateChange(false);
                            thd.lastBeginDate = null;
                            thd.lastEndDate = null;
                            ////断开连接，同时修改数据库状态
                            //wcsdsServer.OnDisconnectedResetData();
                            //连接socket
                            Services.WcsServiceFactory.Log.v("正在连接wms... ...");
                            SocketClient.Connect(); //阻塞操作，如果未连接则一直等待
                            wcsdsServer.AddStateChange(true);
                            Services.WcsServiceFactory.Log.v("与wms已建立连接");
                        }
                        int _sendMiniSec = 3000;//推送间隔 大于3000毫秒 推送一次设备状态
                        int recyCount = _sendMiniSec;//每次连接成功后 立即推送
                        while (true)
                        {//递归执行任务 
                            try
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                if (thd.Exit)
                                {
                                    break;
                                }
                                thd.lastBeginDate = DateTime.Now;
                                //if (SocketClient.IsConnected)
                                //{//检查发送任务状态
                                //    wcsdsServer.CheckOverTaskStatus(SocketClient, stackerSrv);
                                //}
                                if (exitServer)
                                {
                                    break;
                                }
                                if (SocketClient.IsConnected)
                                {//发送设备状态 3秒推一次 
                                    if (recyCount >= _sendMiniSec)
                                    {
                                        wcsdsServer.SendDeviceStatus(SocketClient, stackerSrv);
                                        recyCount = 0;
                                    }
                                    recyCount += Services.WcsServiceFactory.Config.RefreshCycle;//每次累加刷新频率
                                }
                                if (exitServer)
                                {
                                    break;
                                }
                                //if (SocketClient.IsConnected)
                                //{//发送条码请求入库
                                //    wcsdsServer.SendTrayBarcode(SocketClient);
                                //}
                                if (exitServer)
                                {
                                    break;
                                }

                            }
                            catch (System.InvalidOperationException inverr)
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                //   wcsdsServer.ResetDbModel();
                                Services.WcsServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:30110", inverr);
                                //if (exitServer)
                                //{
                                //    break;
                                //}
                                //wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:30110=>>" + inverr.ToString(), 0);
                            }
                            catch (Exception ex)
                            {
                                //wcsdsServer.ResetDbModel();
                                Services.WcsServiceFactory.Log.v("异常:304131", ex);
                                //if (exitServer)
                                //{
                                //    break;
                                //}
                                // wcsdsServer.DbExecuteLog("异常:304131=>>" + ex.ToString(), 0);
                            }
                            if (!SocketClient.IsConnected)
                            {//连接失败重新连接
                             // wcsdsServer.OnDisconnectedResetData();
                                break;
                            }
                            thd.lastEndDate = DateTime.Now;
                            // System.Threading.Thread.Sleep(1000);
                            if (exitServer)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(Services.WcsServiceFactory.Config.RefreshCycle);
                        }
                    }
                    catch (System.InvalidOperationException inverr)
                    {
                        if (exitServer)
                        {
                            break;
                        }
                        // wcsdsServer.ResetDbModel();
                        Services.WcsServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:66669", inverr);
                        //if (exitServer)
                        //{
                        //    break;
                        //}
                        //wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:66669" + inverr.ToString(), 0);
                    }
                    catch (Exception ex)
                    {
                        Services.WcsServiceFactory.Log.v("[" + thd.Name + "]异常16:", ex);
                        if (exitServer)
                        {
                            break;
                        }
                        //wcsdsServer.DbExecuteLog("[" + thd.Name + "]异常16:" + ex.ToString(), 0);
                    }
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception exx)
                {
                    Services.WcsServiceFactory.Log.v("[" + thd.Name + "]异常27:", exx);
                }
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;
        }

        /// <summary>
        /// 断开后执行的事件
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ErrorMessage"></param>
        private void Client_onDisConnectHandler(string ErrorMessage)
        {
            try
            {
                Services.WcsServiceFactory.Log.v(ErrorMessage);
                //与wms断开连接，堆垛机也即断开连接.... 
                Services.WcsServiceFactory.Log.v("与wms断开连接，修改堆垛机的状态！ ");
                //  OnDisconnectedResetData();
            }
            catch (Exception ex)
            {
                Services.WcsServiceFactory.Log.v("Error code:10001", ex);
            }
        }
        private void _socketServer_onLogHandler(string log)
        {//注释socket 异常日志记录
            Services.WcsServiceFactory.Log.v("wcservice1 log handler 695511235>>" + log);
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="json"></param>
        private void Client_onReceiveHandler(LTProtocol.Tcp.Socket_Client socket, string json)
        {
            try
            {
                if ("{200}" != json)
                {
                    Services.WcsServiceFactory.Log.v("wcs接收wms的json数据>>>>:10003" + json);
                }
                try
                {
                    wcsreceiveSrv.ReceiveHandler(json, socket, stackerSrv);
                }
                catch (System.InvalidOperationException inverr)
                {
                    //wcsreceiveSrv.ResetDbModel();
                    Services.WcsServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:6e2w51", inverr);
                    // wcsreceiveSrv.DbExecuteLog("数据库连接异常，已重新初始化dbContext:6e2w5" + inverr.ToString(), 0);
                }
                catch (Exception ex)
                {
                    // wcsreceiveSrv.ResetDbModel();
                    Services.WcsServiceFactory.Log.v("异常:85332", ex);
                    //  wcsreceiveSrv.DbExecuteLog("异常:85332" + ex.ToString(), 0);
                }
            }
            catch (Exception exx)
            {
                Services.WcsServiceFactory.Log.v("异常:1322252423", exx);
            }
        }

        /// <summary>
        /// 是否启动
        /// </summary>
        private bool isStart;
        static System.Threading.Thread mainthread;
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (isStart)
            {
                return;
            }
            //开启监视线程
            try
            {
                mainthread = new System.Threading.Thread(MonitorHandler);
                mainthread.IsBackground = true;
                mainthread.Priority = System.Threading.ThreadPriority.Normal;
                mainthread.Start();
                isStart = true;
            }
            catch (Exception ex)
            {
                Services.WcsServiceFactory.Log.v("系统异常>>>" + ex);
                System.Threading.Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            try
            {
                exitServer = true;
                Services.WcsServiceFactory.Log.v("正在停止WCS服务...");
                WCSDealSendService wcsdsServer = new WCSDealSendService();
                // wcsdsServer.OnDisconnectedResetData();
                //wcs服务已停止
                wcsdsServer.WCSStoped();
                wcsdsServer.AddStateChange(false);
                // WMSDealSendServer wMSDealSend = new WMSDealSendServer();
                // wMSDealSend.AddStateChange(LTEFModel.Warehouse.WcsStatus.Disconnected);
                Services.WcsServiceFactory.Log.v("正在关闭所有线程...");
                if (stackerSrv != null)
                {//关闭PLC连接
                    stackerSrv.Close();
                }
                //等待所有线程停止
                SocketClient.Abort();
                mainthread.Abort();
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Abort();
                    }
                    catch (Exception ex) { Services.WcsServiceFactory.Log.v(ex); }
                }
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Join();
                    }
                    catch (Exception ex) { Services.WcsServiceFactory.Log.v(ex); }
                }
                try
                {
                    mainthread.Join();
                }
                catch (Exception ex) { Services.WcsServiceFactory.Log.v(ex); }
                Services.WcsServiceFactory.Log.v("WCS服务已停止...");
            }
            catch (Exception ex)
            {
                Services.WcsServiceFactory.Log.v("异常》》》WCSService1>>Stop>>" + ex);
            }
        }
        public void MonitorHandler()
        {
            Services.WcsServiceFactory.Log.v("WCS服务：监视线程已启动");
            while (true)
            {
                try
                {
                    if (exitServer)
                    {
                        break;
                    }
                    while (true)
                    {
                        try
                        {
                            if (exitServer)
                            {
                                break;
                            }
                            foreach (var t in threadList)
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                //if (t.lastBeginDate != null && t.lastEndDate != null &&
                                //      LTLibrary.ConvertUtility.DiffSeconds(t.lastBeginDate.Value, t.lastEndDate.Value) > t.AlarmSecond)
                                //{//如果超过设定时间，还没有执行一轮循环，则销毁线程，重新创建，线程可能已经奔溃？？
                                //    Services.WcsServiceFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                //    t.Exit = true;//设置退出标志
                                //    t.ThreadReboot = true;//设置重启标志
                                //} 
                                if (!(t.lastBeginDate == null && t.lastEndDate == null) &&
                                    ((t.lastEndDate != null && LTLibrary.ConvertUtility.DiffSeconds(t.lastEndDate.Value, DateTime.Now) > t.AlarmSecond)
                                    || (t.lastEndDate == null && LTLibrary.ConvertUtility.DiffSeconds(t.lastBeginDate.Value, DateTime.Now) > t.AlarmSecond))
                                     )
                                {//如果超过设定时间，还没有执行一轮循环，则销毁线程，重新创建，线程可能已经奔溃？？ 
                                    Services.WcsServiceFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                    t.Exit = true;//设置退出标志
                                    t.ThreadReboot = true;//设置重启标志
                                }
                                if (t.ThreadReboot)
                                {
                                    Services.WcsServiceFactory.Log.v("正在重启关闭的线程 ==>>" + t.Name);
                                    if (t.thread != null)
                                    {
                                        try
                                        {
                                            if (exitServer)
                                            {
                                                break;
                                            }
                                            Services.WcsServiceFactory.Log.v("正在终止意外关闭的线程:" + t.Name);
                                            t.thread.Abort();//终止线程 
                                            t.thread.Join();
                                        }
                                        catch (Exception ex)
                                        {
                                            Services.WcsServiceFactory.Log.v("Error code:1010121 终止意外关闭的线程[" + t.Name + "]异常：" + ex.Message);
                                        }
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    t.thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(t.threadstart));
                                    t.thread.IsBackground = true;
                                    t.Exit = false;
                                    t.ThreadReboot = false;
                                    t.lastBeginDate = null;
                                    t.lastEndDate = null;
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    t.thread.Start(t);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Services.WcsServiceFactory.Log.v("Error code:10010", ex);
                        }
                        if (exitServer)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5000);
                    }
                    //防止意外跳出
                    Services.WcsServiceFactory.Log.v("Error code:9110421 线程意外跳出...");
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Services.WcsServiceFactory.Log.v("异常：1111>>>" + ex.ToString());
                }
            }
        }
    }
}
