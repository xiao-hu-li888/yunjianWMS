using LTLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.Services
{
    /// <summary>
    /// 艾华接口服务线程
    /// </summary>
    public class InterfaceService
    {
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        List<ThreadHandler> threadList = new List<ThreadHandler>();
        public InterfaceService()
        {
            /**************************************************************************************/
            //添加线程处理
            ThreadHandler threadWmsTcp = new ThreadHandler();
            threadWmsTcp.Name = "艾华通信服务线程";
            threadWmsTcp.ThreadReboot = true;
            threadWmsTcp.Exit = false;
            threadWmsTcp.threadstart = threadWmsTcpSocketStart;
            threadWmsTcp.AlarmSecond = 120;//120秒无响应重启线程
            threadList.Add(threadWmsTcp);
            //添加服务
            ThreadHandler threadAgvAlive = new ThreadHandler();
            threadAgvAlive.Name = "艾华WebApi接口服务可用性监测线程";
            threadAgvAlive.ThreadReboot = true;
            threadAgvAlive.Exit = false;
            threadAgvAlive.threadstart = threadWmsAgvServerCheck;
            threadAgvAlive.AlarmSecond = 180;//120秒无响应重启线程
            threadList.Add(threadAgvAlive);
            /**************************************************************************************/
        }
        InterfaceServerCheckService agvSrvChkService;
        private bool isConnectedToAgv = false;
        public void threadWmsAgvServerCheck(object threadhandler)
        {
            isConnectedToAgv = false;
            ThreadHandler thd = threadhandler as ThreadHandler;
            WMSFactory.Log.v("启动线程：" + thd.Name);
            bool _lastStatus = false;
            agvSrvChkService = new InterfaceServerCheckService();
            try
            {//默认启动修改连接状态为：未连接
                agvSrvChkService.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v("失败：22212555555555>>>" + ex);
            }
            while (true)
            {
                try
                {
                    thd.lastBeginDate = DateTime.Now;
                    if (exitServer)
                    {
                        break;
                    }
                    if (thd.Exit)
                    {
                        break;
                    }
                    //   WMSFactory.Log.v(thd.Name + "执行中222：");
                    isConnectedToAgv = agvSrvChkService.CheckAgvServerConnected();
                    if (isConnectedToAgv != _lastStatus)
                    { //只保存状态变更 
                        try
                        {
                            if (exitServer)
                            {
                                break;
                            }
                            _lastStatus = isConnectedToAgv;
                            if (isConnectedToAgv)
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                agvSrvChkService.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Connected);
                            }
                            else
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                agvSrvChkService.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
                            }
                        }
                        catch (System.InvalidOperationException inverr)
                        {
                            if (exitServer)
                            {
                                break;
                            }
                            agvSrvChkService.ResetDbconnection();
                            _lastStatus = !isConnectedToAgv;
                            WMSFactory.Log.v("线程：" + thd.Name + "，数据库连接异常，已重新初始化dbContext:666663954454", inverr);
                        }
                        catch (Exception ex)
                        {//保存异常改回之前状态。。。
                            _lastStatus = !isConnectedToAgv;
                            WMSFactory.Log.v("异常信息5526805693：>>>>>" + ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    WMSFactory.Log.v("异常信息。。。99638451>>", ex);
                }
                thd.lastEndDate = DateTime.Now;
                if (exitServer)
                {
                    break;
                }
                System.Threading.Thread.Sleep(5000);
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;
        }
        public void threadWmsTcpSocketStart(object threadhandler)
        {
            ThreadHandler thd = threadhandler as ThreadHandler;
            WMSFactory.Log.v("启动线程：" + thd.Name);
            //WMSDealSendServer wmsDealsendServer = Services.AutofacConfig.GetFromFac<WMSDealSendServer>();
            InterfaceDealSendService agvdealsendsrv = new InterfaceDealSendService();
            while (true)
            {
                try
                {
                    try
                    {
                        thd.lastBeginDate = DateTime.Now;
                        if (exitServer)
                        {
                            break;
                        }
                        if (thd.Exit)
                        {
                            break;
                        }
                        if (isConnectedToAgv)
                        {//agv调度服务可用才执行
                         //  WMSFactory.Log.v(thd.Name + "执行中111：");
                         //   logtodb.DbExecuteLog("agv....xxxx", randDiff);
                         //检查发送批量任务
                         //if (exitServer)
                         //{
                         //    break;
                         //}
                         //agvdealsendsrv.CheckSendTasks(randDiff);
                         //if (exitServer)
                         //{
                         //    break;
                         //}
                         ////发送当前任务id和电池条码及朝向
                         //agvdealsendsrv.GetScanTaskAndSend(randDiff);
                            if (exitServer)
                            {
                                break;
                            }
                            //发送出入库流水
                            agvdealsendsrv.SendInoutRecordCMD();
                            if (exitServer)
                            {
                                break;
                            }
                            agvdealsendsrv.InoutSendedRecordToHis();
                        }
                        if (exitServer)
                        {
                            break;
                        }
                        //处理agv任务完成、取消等状态
                        //   agvdealsendsrv.DealAgvTaskFinish(isConnectedToAgv);
                    }catch(System.Net.WebException ex)
                    {
                        WMSFactory.Log.v("线程：" + thd.Name + "异常:9991555>>>", ex);
                    }
                    catch (System.InvalidOperationException inverr)
                    {
                        if (exitServer)
                        {
                            break;
                        }
                        agvdealsendsrv.ResetDbconnection();//重新初始化dbcontext
                        WMSFactory.Log.v("线程：" + thd.Name + "，数据库连接异常，已重新初始化dbContext:xxxx5521", inverr);
                    }
                    catch (Exception ex)
                    {
                        WMSFactory.Log.v("线程：" + thd.Name + "异常:", ex);
                    }
                    thd.lastEndDate = DateTime.Now;
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    WMSFactory.Log.v("异常信息。。。6698547>>", ex);
                }
                System.Threading.Thread.Sleep(3000);
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;
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
                WMSFactory.Log.v("系统异常>>>" + ex);
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
                WMSFactory.Log.v("正在停止艾华接口服务...");
                agvSrvChkService.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
                //WCSDealSendService wcsdsServer = new WCSDealSendService();
                //// wcsdsServer.OnDisconnectedResetData();
                ////wcs服务已停止
                //wcsdsServer.WCSStoped();
                //wcsdsServer.AddStateChange(false);
                // WMSDealSendServer wMSDealSend = new WMSDealSendServer();
                // wMSDealSend.AddStateChange(LTEFModel.Warehouse.WcsStatus.Disconnected);
                WMSFactory.Log.v("正在关闭所有线程...");
                //等待所有线程停止 
                mainthread.Abort();
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Abort();
                    }
                    catch (Exception ex) { WMSFactory.Log.v(ex); }
                }
                foreach (var t in threadList)
                {
                    try
                    {
                        t.thread.Join();
                    }
                    catch (Exception ex) { WMSFactory.Log.v(ex); }
                }
                try
                {
                    mainthread.Join();
                }
                catch (Exception ex) { WMSFactory.Log.v(ex); }
                WMSFactory.Log.v("艾华接口服务已停止...");
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v("异常》》》WCSService1>>Stop>>" + ex);
            }
        }
        public void MonitorHandler()
        {
            WMSFactory.Log.v("艾华接口服务：监视线程已启动");
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
                                //    LTLibrary.ConvertUtility.DiffSeconds(t.lastBeginDate.Value, t.lastEndDate.Value) > t.AlarmSecond)
                                //{//如果超过设定时间，还没有执行一轮循环，则销毁线程，重新创建，线程可能已经奔溃？？
                                //    WMSFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                //    t.Exit = true;//设置退出标志
                                //    t.ThreadReboot = true;//设置重启标志
                                //}
                                if (!(t.lastBeginDate == null && t.lastEndDate == null) &&
                                   ((t.lastEndDate != null && LTLibrary.ConvertUtility.DiffSeconds(t.lastEndDate.Value, DateTime.Now) > t.AlarmSecond)
                                   || (t.lastEndDate == null && LTLibrary.ConvertUtility.DiffSeconds(t.lastBeginDate.Value, DateTime.Now) > t.AlarmSecond))
                                    )
                                {//如果超过设定时间，还没有执行一轮循环，则销毁线程，重新创建，线程可能已经奔溃？？ 
                                    WMSFactory.Log.v("线程" + t.Name + "超过设定时间" + t.AlarmSecond + "(秒)没有响应，重新启动线程... ==>>");
                                    t.Exit = true;//设置退出标志
                                    t.ThreadReboot = true;//设置重启标志
                                }
                                if (t.ThreadReboot)
                                {
                                    WMSFactory.Log.v("正在重启关闭的线程 ==>>" + t.Name);
                                    if (t.thread != null)
                                    {
                                        try
                                        {
                                            if (exitServer)
                                            {
                                                break;
                                            }
                                            WMSFactory.Log.v("正在终止意外关闭的线程:" + t.Name);
                                            t.thread.Abort();//终止线程 
                                            t.thread.Join();
                                        }
                                        catch (Exception ex)
                                        {
                                            WMSFactory.Log.v("Error code:1010121 终止意外关闭的线程[" + t.Name + "]异常：" + ex.Message);
                                        }
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
                                //if (t.thread != null && !t.thread.IsAlive)
                                //{//如果线程意外终止，则重启线程？？？
                                //    t.ThreadReboot = true;
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            WMSFactory.Log.v("Error code:10010", ex);
                        }
                        if (exitServer)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5000);
                    }
                    //防止意外跳出
                    WMSFactory.Log.v("Error code:9110421 线程意外跳出...");
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    // WMSFactory.Log.v("异常：1111>>>" + ex.ToString());
                }
            }
        }

    }
}