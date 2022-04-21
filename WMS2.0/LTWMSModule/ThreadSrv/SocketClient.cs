using LTLibrary;
using LTWMSEFModel.Warehouse;
using LTWMSModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.ThreadSrv
{
    /// <summary>
    /// socket客户端处理逻辑
    /// </summary>
    public class SocketClient : IThreadService
    {
        //WCS配置信息
        // public Guid WareHouseGuid;
        public Guid Wcs_srv_guid;
        public string ServerName;
        public string wcs_srv_ip;
        public int wcs_srv_port;
        /////////////
        LTProtocol.Tcp.Socket_Client_New _SocketC;
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        WMSDealSendService wcsdsServer;
        public void OnStop()
        {
            exitServer = true;
            try
            {
                _SocketC.Abort();
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v(ex);
            }
            //修改wcs的连接状态 
            //foreach (wh_warehouse item in ListWareHouse)
            //{
            wcsdsServer.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
            //}
        }
   //     List<wh_warehouse> ListWareHouse;
        public void ThreadStart(object threadhandler)
        {
            exitServer = false;
            ThreadHandler thd = threadhandler as ThreadHandler;
            Services.WinServiceFactory.Log.v("启动线程：【" + thd.Name + "】 socket 客户端>> ip：" + thd.Ip + "，端口：" + thd.Port);
            _SocketC = new LTProtocol.Tcp.Socket_Client_New(
                thd.Ip
                  , thd.Port, LTProtocol.Tcp.SocketClientEncoding.UTF8);
            _SocketC.onLogHandler += _socketServer_onLogHandler;
            _SocketC.onReceiveHandler += Client_onReceiveHandler;
            _SocketC.onDisConnectHandler += Client_onDisConnectHandler;
            wcsdsServer = new WMSDealSendService(thd.Wcs_srv_guid, thd.Name, thd.Ip, thd.Port);
            // WareHouseGuid = thd.WareHouseGuid;
            Wcs_srv_guid = thd.Wcs_srv_guid;
            ServerName = thd.Name;
            wcs_srv_ip = thd.Ip;
            wcs_srv_port = thd.Port;
            //根据wcs服务查找所有对应的仓库信息
        //    ListWareHouse = wcsdsServer.GetAllWareHouseByWcsSrvGuid(Wcs_srv_guid);
            //wms服务已启动
            //   wcsdsServer.WMSStarted();
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
                        if (!_SocketC.IsConnected)
                        {
                            // foreach (wh_warehouse item in ListWareHouse)
                            // {
                            wcsdsServer.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
                            //}
                            thd.lastBeginDate = null;
                            thd.lastEndDate = null;
                            //断开连接，同时修改数据库状态
                            //      wcsdsServer.OnDisconnectedResetData();
                            //连接socket
                            Services.WinServiceFactory.Log.v("正在连接wcs... ...");
                            _SocketC.Connect(); //阻塞操作，如果未连接则一直等待
                                                //  foreach (wh_warehouse item in ListWareHouse)
                                                //  {
                            wcsdsServer.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Connected);
                            // }
                            Services.WinServiceFactory.Log.v("与wcs已建立连接");
                        }
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
                             //   foreach (wh_warehouse item in ListWareHouse)
                             //   {
                                    
                                    thd.lastBeginDate = DateTime.Now;
                                    if (_SocketC.IsConnected)
                                    {//定时循环任务，并发送至堆垛机
                                        wcsdsServer.SendWaitedMessage(_SocketC, null);
                                    }
                                    /*
                                    if (_SocketC.IsConnected)
                                    {//定时循环任务，并发送至堆垛机
                                        wcsdsServer.CheckOverTaskQueue(item, _SocketC, null);
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (_SocketC.IsConnected)
                                    {//将取消或者完成的任务归入历史表中
                                        wcsdsServer.SetTaskToHistory(item);
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (_SocketC.IsConnected)
                                    {
                                        wcsdsServer.ReturnStationForRFID(item, _SocketC, null);
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (_SocketC.IsConnected)
                                    {// 检查入库未分配库位，分配库位
                                        wcsdsServer.CheckWaiteDispatchStockCell(item);
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (_SocketC.IsConnected)
                                    {
                                        //检查强制完成、取消任务操作
                                        wcsdsServer.CheckForeceCancelTaskHandler(item, _SocketC, null);
                                    }
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (_SocketC.IsConnected)
                                    {//防止并发导致未解锁库位，查询并解锁syslock
                                        wcsdsServer.CheckSysLockShelfUnit_Free(item);
                                    }
                                    ////////if (_SocketC.IsConnected)
                                    ////////{//检查发送控制指令信息，包括重发入库申请和强制完成任务/取消任务
                                    ////////    wcsdsServer.CheckSendControldCMD(_SocketC);
                                    ////////}
                                    ///
                                    */
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    thd.lastEndDate = DateTime.Now;
                                 //   System.Threading.Thread.Sleep(30);
                              //  }
                            }
                            catch (System.InvalidOperationException inverr)
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.ResetDbModel();
                                Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:30110", inverr);
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:30110=>>" + inverr.ToString(), 0);
                            }
                            catch (Exception ex)
                            {
                                //wcsdsServer.ResetDbModel();
                                Services.WinServiceFactory.Log.v("异常:304131", ex);
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.DbExecuteLog("异常:304131=>>" + ex.ToString(), 0);
                            }
                            if (!_SocketC.IsConnected)
                            {//连接失败重新连接
                             // wcsdsServer.OnDisconnectedResetData();
                                break;
                            }
                            // System.Threading.Thread.Sleep(1000);
                            if (exitServer)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(WinServiceFactory.Config.RefreshCycle);
                        }
                    }
                    catch (System.InvalidOperationException inverr)
                    {
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.ResetDbModel();
                        Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:66669", inverr);
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:66669" + inverr.ToString(), 0);
                    }
                    catch (Exception ex)
                    {
                        Services.WinServiceFactory.Log.v("[" + thd.Name + "]异常16:", ex);
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.DbExecuteLog("[" + thd.Name + "]异常16:" + ex.ToString(), 0);
                    }
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception exx)
                {
                    Services.WinServiceFactory.Log.v("[" + thd.Name + "]异常27:", exx);
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
                Services.WinServiceFactory.Log.v(ErrorMessage);
                //与wcs断开连接，堆垛机也即断开连接.... 
                Services.WinServiceFactory.Log.v("与wcs断开连接，修改堆垛机的状态！ ");
                //  OnDisconnectedResetData();
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("Error code:10001", ex);
            }
        }
        private void _socketServer_onLogHandler(string log)
        {//注释socket 异常日志记录
            Services.WinServiceFactory.Log.v("wmservice1 loghandler13555543>>" + log);
        }
        private WMSReceiveService wcsreceiveSrv;
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="json"></param>
        private void Client_onReceiveHandler(LTProtocol.Tcp.Socket_Client_New socket, string json)
        {
            try
            {
                if (wcsreceiveSrv == null)
                {
                    wcsreceiveSrv = new WMSReceiveService(Wcs_srv_guid, ServerName, wcs_srv_ip, wcs_srv_port);
                }
                if ("{200}" != json)
                {
                    Services.WinServiceFactory.Log.v("wms接收wcs的json数据>>>>:10003" + json);
                }
                try
                {
                    wcsreceiveSrv.ReceiveHandler(json, socket, null);
                }
                catch (System.InvalidOperationException inverr)
                {
                    wcsreceiveSrv.ResetDbModel();
                    Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:6e2w5", inverr);
                    wcsreceiveSrv.DbExecuteLog("数据库连接异常，已重新初始化dbContext:6e2w5" + inverr.ToString(), 0);
                }
                catch (Exception ex)
                {
                    // wcsreceiveSrv.ResetDbModel();
                    Services.WinServiceFactory.Log.v("异常:85332", ex);
                    wcsreceiveSrv.DbExecuteLog("异常:85332" + ex.ToString(), 0);
                }
            }
            catch (Exception exx)
            {
                Services.WinServiceFactory.Log.v("异常:1322252423", exx);
            }
        }


    }
}
