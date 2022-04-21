using LTLibrary;
using LTWMSEFModel.Warehouse;
using LTWMSModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.ThreadSrv
{
    /// <summary>
    /// Socket服务端处理逻辑
    /// </summary>
    public class SocketServer : IThreadService
    {
        //WCS配置信息
        //  public Guid WareHouseGuid;
        public Guid Wcs_srv_guid;
        public string ServerName;
        public string wcs_srv_ip;
        public int wcs_srv_port;
        /////////////
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        /// <summary>
        /// 保存上一次的Socket，防止重启先出，上一个socket未正常关闭，导致Socket重新绑定端口报错
        /// </summary>
        Socket LastSocketSrv;
        LTProtocol.Tcp.Socket_Server _socketServer;
        WMSDealSendService wmsDealsendServer;
        public void OnStop()
        {
            exitServer = true;
            try
            {
                if (_socketServer != null)
                {
                    _socketServer.Abort(LastSocketSrv);
                }
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v(ex);
            }
            //修改wcs的连接状态 
            // foreach (wh_warehouse item in ListWareHouse)
            // {
            wmsDealsendServer.AddStateChange_Server(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
            // }
        }
        //List<wh_warehouse> ListWareHouse;
        public void ThreadStart(object threadhandler)
        {
            exitServer = false;
            ThreadHandler thd = threadhandler as ThreadHandler;
            Services.WinServiceFactory.Log.v("启动线程：【" + thd.Name + "】 socket 服务端>> ip：" + thd.Ip + "，端口：" + thd.Port);
            _socketServer = new LTProtocol.Tcp.Socket_Server(
               thd.Ip,
               thd.Port,
               LTProtocol.Tcp.SocketClientEncoding.UTF8);
            //thd.Socket = _socketServer;
            _socketServer.onLogHandler += _socketServer_onLogHandler;
            _socketServer.onReceiveHandler += _socketServer_onReceiveHandler;
            _socketServer.onDisConnectHandler += _socketServer_onDisConnectHandler;
            wmsDealsendServer = new WMSDealSendService(thd.Wcs_srv_guid, thd.Name, thd.Ip, thd.Port);
            //WareHouseGuid = thd.WareHouseGuid;
            Wcs_srv_guid = thd.Wcs_srv_guid;
            ServerName = thd.Name;
            wcs_srv_ip = thd.Ip;
            wcs_srv_port = thd.Port;
            //根据wcs服务查找所有对应的仓库信息
            //  ListWareHouse = wmsDealsendServer.GetAllWareHouseByWcsSrvGuid(Wcs_srv_guid);
            /***********************************/
            string _lastclients = "";
            bool flag = true;
            while (true)
            {
                try
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
                        // foreach (wh_warehouse item in ListWareHouse)
                        // {


                        if (!_socketServer.IsStartListen)
                        {
                            //wmsDealsendServer.AddStateChange(LTEFModel.Warehouse.WcsStatus.Disconnected);
                            //  wmsDealsendServer.WMSStoped();
                            _socketServer.StartListen(LastSocketSrv);
                            LastSocketSrv = _socketServer.socketSrv;//启动监听后赋值
                                                                    //wmsDealsendServer.AddStateChange(LTEFModel.Warehouse.WcsStatus.Connected);
                                                                    //  wmsDealsendServer.WMSStarted();
                            Services.WinServiceFactory.Log.v(thd.Name + "已启动监听...");
                        }

                        thd.lastBeginDate = DateTime.Now;
                        string _clients = _socketServer.GetClients();
                        if (!string.IsNullOrWhiteSpace(_clients) && _clients != _lastclients)
                        {//远程不为空/和上一次值不一致 
                            flag = true;
                            wmsDealsendServer.AddStateChange_Server(LTWMSEFModel.Warehouse.WcsStatus.Connected, _clients);
                            _lastclients = _clients;
                        }
                        else if (string.IsNullOrWhiteSpace(_clients) && flag)
                        {//远程为空修改一次
                            flag = false;
                            wmsDealsendServer.AddStateChange_Server(LTWMSEFModel.Warehouse.WcsStatus.Connected, "");
                            _lastclients = "";
                        }
                        if (exitServer)
                        {
                            break;
                        }
                        // 只有远程连接状态/才处理。。。 
                        if (!string.IsNullOrWhiteSpace(_clients))
                        {//WinServiceFactory.BarcodeOfEnd  
                            wmsDealsendServer.SendWaitedMessage(null, _socketServer);
                            if (exitServer)
                            {
                                break;
                            }
                            /*
                            //定时循环任务，并发送至堆垛机
                            wmsDealsendServer.CheckOverTaskQueue(item, null, _socketServer);
                            if (exitServer)
                            {
                                break;
                            }
                            //将取消或者完成的任务归入历史表中
                            wmsDealsendServer.SetTaskToHistory(item);
                            if (exitServer)
                            {
                                break;
                            }
                            wmsDealsendServer.ReturnStationForRFID(item, null, _socketServer);
                            if (exitServer)
                            {
                                break;
                            }
                            // 检查入库未分配库位，分配库位
                            wmsDealsendServer.CheckWaiteDispatchStockCell(item);
                            if (exitServer)
                            {
                                break;
                            }
                            //wmsDealsendServer.DealSendCMD(_socketServer);
                            //检查强制完成、取消任务操作
                            wmsDealsendServer.CheckForeceCancelTaskHandler(item, null, _socketServer);
                            if (exitServer)
                            {
                                break;
                            }
                            //防止并发导致未解锁库位，查询并解锁syslock
                            wmsDealsendServer.CheckSysLockShelfUnit_Free(item);*/
                        }
                        thd.lastEndDate = DateTime.Now;
                        //    System.Threading.Thread.Sleep(30);
                    }
                    //}
                    catch (System.InvalidOperationException inverr)
                    {
                        if (exitServer)
                        {
                            break;
                        }
                        wmsDealsendServer.ResetDbModel();
                        Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:9921482121", inverr);
                        if (exitServer)
                        {
                            break;
                        }
                        wmsDealsendServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:9921482121" + inverr.ToString(), 0);
                    }
                    catch (Exception ex)
                    {
                        Services.WinServiceFactory.Log.v(thd.Name + "异常1:", ex);
                        if (exitServer)
                        {
                            break;
                        }
                        wmsDealsendServer.DbExecuteLog(thd.Name + "异常1:" + ex.ToString(), 0);
                    }
                    //System.Threading.Thread.Sleep(1000);
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(WinServiceFactory.Config.RefreshCycle);
                }
                catch (Exception ex)
                {
                    Services.WinServiceFactory.Log.v("异常信息。。。69855267>>", ex);
                }
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;

        }
        private void _socketServer_onDisConnectHandler(string socketKey)
        {
            //远程关闭socket
            lock (dictList)
            {
                if (dictList.ContainsKey(socketKey))
                {//关闭对应的ef连接
                 //垃圾回收
                 //  dictList[socketKey].RemoveDBConnection();
                 //连接字典移除ef
                    dictList.Remove(socketKey);
                    Services.WinServiceFactory.Log.v("已从字典中移除EF对象:" + socketKey);
                }
            }
        }
        private void _socketServer_onLogHandler(string log)
        {//注释socket 异常日志记录 
            Services.WinServiceFactory.Log.v("wmservice2 log handler 55513555543>>" + log);
        }
        Dictionary<string, WMSReceiveService> dictList = new Dictionary<string, WMSReceiveService>();
        private void _socketServer_onReceiveHandler(LTProtocol.Tcp.Socket_Server socket, string socketKey, string json)
        {
            lock (dictList)
            {
                try
                {
                    try
                    {
                        Services.WinServiceFactory.Log.v(socketKey + "::接收到的数据：" + json);
                        //接收到消息
                        if (!dictList.ContainsKey(socketKey))
                        {
                            WMSReceiveService _wmsserver = new WMSReceiveService(Wcs_srv_guid, ServerName, wcs_srv_ip, wcs_srv_port);
                            dictList.Add(socketKey, _wmsserver);
                        }
                        //处理json数据 
                        dictList[socketKey].ReceiveHandler(json, null, socket);
                    }
                    catch (System.InvalidOperationException inverr)
                    {
                        dictList.Remove(socketKey);
                        Services.WinServiceFactory.Log.v("EF 连接对象异常，重新初始化:30113080", inverr);
                    }
                    catch (Exception ex)
                    {
                        // dictList.Remove(socketKey);
                        Services.WinServiceFactory.Log.v("异常1123-823：===>>>>" + ex);
                        dictList[socketKey].DbExecuteLog("接收json数据：" + json + "。处理异常>>>" + ex.ToString(), 0);
                    }
                }
                catch (Exception ex)
                {
                    Services.WinServiceFactory.Log.v("异常信息。。。555566732>>", ex);
                }
            }
        }
    }
}
