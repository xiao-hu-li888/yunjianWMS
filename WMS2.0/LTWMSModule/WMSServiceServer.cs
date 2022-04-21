using LTLibrary;
using LTWMSEFModel.Warehouse;
using LTWMSModule.Services;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule
{
    /// <summary>
    /// 服务处理入口(WMS做服务端)
    /// </summary>
    public class WMSServiceServer
    {
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        List<ThreadHandler> threadList = new List<ThreadHandler>();
        private LTWMSEFModel.LTModel dbmodel;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_service_statusBLL bll_wh_service_status;
        public WMSServiceServer()
        {
            dbmodel = new LTWMSEFModel.LTModel();
            bll_wh_wcs_srv = new wh_wcs_srvBLL(dbmodel);
            bll_wh_service_status = new wh_service_statusBLL(dbmodel);
            //查找所有wcs配置 且已关联货架
            var ListWCSSrv = bll_wh_wcs_srv.GetAllWcsSrvUnionShelves(); 
            if (ListWCSSrv != null && ListWCSSrv.Count() > 0)
            {
                foreach (var item in ListWCSSrv)
                {
                    //添加线程处理
                    ThreadHandler threadWmsTcp = new ThreadHandler();
                    threadWmsTcp.Name = item.name;// "WMS Tcp Socket服务端线程";
                    threadWmsTcp.ThreadReboot = true;
                    threadWmsTcp.Exit = false;
                    threadWmsTcp.Ip = item.ip;
                   // threadWmsTcp.WareHouseGuid = item.warehouse_guid.Value;
                    threadWmsTcp.Wcs_srv_guid = item.guid;
                    threadWmsTcp.Port = item.port;
                    threadWmsTcp.AlarmSecond = 180;//180秒无响应重启线程
                    if (item.srv_type == LTWMSEFModel.Warehouse.WcsServerType.Server)
                    {//服务端
                     //  threadWmsTcp.socketType = SocketTypeEnum.SocketServer;
                        threadWmsTcp.Service = new ThreadSrv.SocketServer();
                        threadWmsTcp.threadstart = threadWmsTcp.Service.ThreadStart;//threadWmsTcpSocketStart
                    }
                    else
                    {//客户端
                     // threadWmsTcp.socketType = SocketTypeEnum.SocketClient;
                        threadWmsTcp.Service = new ThreadSrv.SocketClient();
                        threadWmsTcp.threadstart = threadWmsTcp.Service.ThreadStart;// threadWmsTcpSocketStart;
                    }
                    threadList.Add(threadWmsTcp);

                    /**************添加线程：处理发送消息**********************/
                    ThreadHandler threadDealSend = new ThreadHandler();
                    threadDealSend.Name = item.name + ">>WMS生产消息";// "WMS Tcp Socket服务端线程";
                    threadDealSend.ThreadReboot = true;
                    threadDealSend.Exit = false;
                    threadDealSend.Ip = "";// WinServiceFactory.Config.ZhongKongIp;
                    threadDealSend.Wcs_srv_guid = item.guid;// Guid.Parse("be0a29cb-43b6-452f-add1-60a99fdb2637");
                    threadDealSend.Port = 0;// Convert.ToInt32(WinServiceFactory.Config.ZhongKongPort);
                    threadDealSend.AlarmSecond = 180;//180秒无响应重启线程

                    threadDealSend.Service = new ThreadSrv.WMSSrv_DealSend();
                    threadDealSend.threadstart = threadDealSend.Service.ThreadStart;
                    threadList.Add(threadDealSend);
                    /******************处理接收到的消息************************/
                    ThreadHandler threadDealReceive = new ThreadHandler();
                    threadDealReceive.Name = item.name + ">>WMS消费消息";// "WMS Tcp Socket服务端线程";
                    threadDealReceive.ThreadReboot = true;
                    threadDealReceive.Exit = false;
                    threadDealReceive.Ip = "";// WinServiceFactory.Config.ZhongKongIp;
                    threadDealReceive.Wcs_srv_guid = item.guid;// Guid.Parse("be0a29cb-43b6-452f-add1-60a99fdb2637");
                    threadDealReceive.Port = 0;// Convert.ToInt32(WinServiceFactory.Config.ZhongKongPort);
                    threadDealReceive.AlarmSecond = 180;//180秒无响应重启线程

                    threadDealReceive.Service = new ThreadSrv.WMSSrv_DealReceive();
                    threadDealReceive.threadstart = threadDealReceive.Service.ThreadStart;
                    threadList.Add(threadDealReceive);
                    /************************************/
                }
            }
            /**************添加线程：处理数据转历史记录**********************/
            ThreadHandler threadDealToHistory = new ThreadHandler();
            threadDealToHistory.Name = "WMS历史处理线程";// "WMS Tcp Socket服务端线程";
            threadDealToHistory.ThreadReboot = true;
            threadDealToHistory.Exit = false;
            threadDealToHistory.Ip = "";// WinServiceFactory.Config.ZhongKongIp;
            threadDealToHistory.Wcs_srv_guid = Guid.Empty;// wcs_srv_guid;// Guid.Parse("be0a29cb-43b6-452f-add1-60a99fdb2637");
            threadDealToHistory.Port = 0;// Convert.ToInt32(WinServiceFactory.Config.ZhongKongPort);
            threadDealToHistory.AlarmSecond = 180;//180秒无响应重启线程

            threadDealToHistory.Service = new ThreadSrv.WMSSrv_DealToHistory();
            threadDealToHistory.threadstart = threadDealToHistory.Service.ThreadStart;
            threadList.Add(threadDealToHistory);
            /************************************/
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
                WMSStarted();
                mainthread = new System.Threading.Thread(MonitorHandler);
                mainthread.IsBackground = true;
                mainthread.Priority = System.Threading.ThreadPriority.Normal;
                mainthread.Start();
                isStart = true;
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("系统异常>>>" + ex);
                System.Threading.Thread.Sleep(500);
            }
        }
        /// <summary>
        /// WMS服务停止
        /// </summary>
        public void WMSStoped()
        {
            try
            {
                UpdateWmsWhileStop();
            }
            catch (System.InvalidOperationException inverr)
            {
                dbmodel = new LTWMSEFModel.LTModel();
                Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:96214536256325", inverr);
                //修改失败再修改一次
                UpdateWmsWhileStop();
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("wms服务停止修改状态失败。。。>>>异常【" + ex + "】");
            }
        }
        private void UpdateWmsWhileStop()
        {
            var winSrv = bll_wh_service_status.GetFirstDefault(w => w.wcstype == WCSType.WMSWinServer);
            if (winSrv != null && winSrv.guid != Guid.Empty)
            {
                winSrv.wcs_status = WcsStatus.Disconnected;
                winSrv.updatedate = DateTime.Now;
                bll_wh_service_status.Update(winSrv);
            }
        }
        /// <summary>
        /// WMS服务运行中
        /// </summary>
        public void WMSStarted()
        {
            try
            {
                var winSrv = bll_wh_service_status.GetFirstDefault(w => w.wcstype == WCSType.WMSWinServer);
                if (winSrv != null && winSrv.guid != Guid.Empty)
                {
                    winSrv.wcs_status = WcsStatus.Connected;
                    winSrv.updatedate = DateTime.Now;
                    bll_wh_service_status.Update(winSrv);
                }
                else
                {
                    winSrv = new LTWMSEFModel.Warehouse.wh_service_status()
                    {
                        createdate = DateTime.Now,
                        guid = Guid.NewGuid(),
                        createuser = "WMS服务",
                        ip = "",
                        port = 0,
                        // number = 8888,
                        desc = "WMS 服务",
                        state = LTWMSEFModel.EntityStatus.Normal,
                        wcstype = LTWMSEFModel.Warehouse.WCSType.WMSWinServer,
                        wcs_status = WcsStatus.Connected
                    };
                    bll_wh_service_status.Add(winSrv);
                }
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("wms服务启动修改状态失败。。。>>>异常【" + ex + "】");
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
                Services.WinServiceFactory.Log.v("正在停止WMS服务...");
                try
                {
                    WMSStoped();
                }
                catch (Exception ex)
                {
                    Services.WinServiceFactory.Log.v("异常99916969595959654>>>" + ex);
                }
                //WMSDealSendService wcsdsServer = new WMSDealSendService();
                //wcsdsServer.OnDisconnectedResetData();
                ////wms服务已停止
                //wcsdsServer.WMSStoped();
                foreach (var t in threadList)
                {
                    try
                    {
                        t.Service.OnStop();
                    }
                    catch (Exception ex) { Services.WinServiceFactory.Log.v("关闭线程 OnStop 失败62828373211>>>>【" + ex + "】"); }
                }
                // WMSDealSendServer wMSDealSend = new WMSDealSendServer();
                // wMSDealSend.AddStateChange(LTEFModel.Warehouse.WcsStatus.Disconnected);
                Services.WinServiceFactory.Log.v("正在关闭所有线程...");
                //等待所有线程停止
                //  _socketServer.Abort(LastSocketSrv); 
                mainthread.Abort();
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Abort();
                    }
                    catch (Exception ex) { Services.WinServiceFactory.Log.v(ex); }
                }
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Join();
                    }
                    catch (Exception ex) { Services.WinServiceFactory.Log.v(ex); }
                }
                try
                {
                    mainthread.Join();
                }
                catch (Exception ex) { Services.WinServiceFactory.Log.v(ex); }
                Services.WinServiceFactory.Log.v("WMS服务已停止...");
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("异常》》》WMSService1>>Stop>>" + ex);
            }
        }

        public void MonitorHandler()
        {
            Services.WinServiceFactory.Log.v("WMS服务：监视线程已启动");
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
                                //    Services.WinServiceFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                //    t.Exit = true;//设置退出标志
                                //    t.ThreadReboot = true;//设置重启标志
                                //}
                                if (!(t.lastBeginDate == null && t.lastEndDate == null) &&
                                  ((t.lastEndDate != null && LTLibrary.ConvertUtility.DiffSeconds(t.lastEndDate.Value, DateTime.Now) > t.AlarmSecond)
                                  || (t.lastEndDate == null && LTLibrary.ConvertUtility.DiffSeconds(t.lastBeginDate.Value, DateTime.Now) > t.AlarmSecond))
                                   )
                                {//如果超过设定时间，还没有执行一轮循环，则销毁线程，重新创建，线程可能已经奔溃？？ 
                                    Services.WinServiceFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                    t.Exit = true;//设置退出标志
                                    t.ThreadReboot = true;//设置重启标志
                                }
                                if (t.ThreadReboot)
                                {
                                    Services.WinServiceFactory.Log.v("正在重启关闭的线程 ==>>" + t.Name);
                                    if (t.thread != null)
                                    {
                                        try
                                        {
                                            if (exitServer)
                                            {
                                                break;
                                            }
                                            Services.WinServiceFactory.Log.v("正在终止意外关闭的线程:" + t.Name);
                                            //调用对应的停止方法
                                            t.Service.OnStop();
                                            t.thread.Abort();//终止线程 
                                            t.thread.Join();
                                        }
                                        catch (Exception ex)
                                        {
                                            Services.WinServiceFactory.Log.v("Error code:1010121 终止意外关闭的线程[" + t.Name + "]异常：" + ex.Message);
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
                            Services.WinServiceFactory.Log.v("Error code:10010", ex);
                        }
                        if (exitServer)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5000);
                    }
                    //防止意外跳出
                    Services.WinServiceFactory.Log.v("Error code:9110421 线程意外跳出...");
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Services.WinServiceFactory.Log.v("异常：1111>>>" + ex.ToString());
                }
            }
        }

    }
}
