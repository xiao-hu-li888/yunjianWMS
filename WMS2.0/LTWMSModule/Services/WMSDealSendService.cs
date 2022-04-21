using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.Basic;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTWMSService.Logs;
using LTWMSService.ApplicationService.WmsServer.Model;
using LTWMSService.Warehouse;
using LTWMSService.BillsAihua;
using LTWMSEFModel.Logs;

namespace LTWMSModule.Services
{
    /// <summary>
    /// 处理数据发送至wcs（单独线程，独立的数据连接）
    /// </summary>
    public class WMSDealSendService : BaseService
    {
        hdw_plcBLL bll_hdw_plc;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        // LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_wh_shelfunits;
        wh_service_statusBLL bll_wh_service_status;
        LTWMSService.ApplicationService.WmsServer.WCSService bll_wcsservice;
        LTWMSService.Warehouse.wh_trayBLL bll_wh_tray;
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        log_sys_alarmBLL bll_sys_alarm_log;
        LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        wh_shelvesBLL bll_wh_shelves;
        billah_reserved_orderBLL bll_billah_reserved_order;
        billah_reserved_order_detailBLL bll_billah_reserved_order_detail;
        hdw_message_waitedsendBLL bll_hdw_message_waitedsend;
        public WMSDealSendService(Guid Wcs_srv_guid, string Wcs_srv_Name, string wcs_srv_ip, int wcs_srv_port) : base(Wcs_srv_guid, Wcs_srv_Name, wcs_srv_ip, wcs_srv_port)
        {
            CreateBLL(GetDbModel());
        }
        public void CreateBLL(LTWMSEFModel.LTModel dbmodel)
        {
            bll_hdw_plc = new hdw_plcBLL(dbmodel);
            //   bll_sys_control_dic = new LTWMSService.Basic.sys_control_dicBLL(dbmodel);
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbmodel);
            bll_hdw_stacker_taskqueue_his = new hdw_stacker_taskqueue_hisBLL(dbmodel);
            bll_wh_shelfunits = new LTWMSService.Warehouse.wh_shelfunitsBLL(dbmodel);
            bll_wh_service_status = new wh_service_statusBLL(dbmodel);
            bll_wh_tray = new LTWMSService.Warehouse.wh_trayBLL(dbmodel);
            bll_sys_table_id = new sys_table_idBLL(dbmodel);
            bll_sys_alarm_log = new log_sys_alarmBLL(dbmodel);
            bll_bill_stockout = new LTWMSService.Bills.bill_stockoutBLL(dbmodel);
            bll_wh_shelves = new wh_shelvesBLL(dbmodel);
            bll_wh_wcs_device = new wh_wcs_deviceBLL(dbmodel);
            bll_hdw_message_waitedsend = new hdw_message_waitedsendBLL(dbmodel);
            bll_billah_reserved_order = new billah_reserved_orderBLL(dbmodel);
            bll_billah_reserved_order_detail = new billah_reserved_order_detailBLL(dbmodel);
            bll_wcsservice = new LTWMSService.ApplicationService.WmsServer.WCSService(dbmodel, bll_wh_tray, bll_wh_shelfunits,
              bll_hdw_stacker_taskqueue, bll_sys_control_dic, bll_sys_alarm_log, bll_sys_table_id, bll_hdw_plc, bll_bill_stockout, bll_wh_wcs_device
              , bll_wh_shelves);
            bll_wcsservice.SetLedObj(ledDisplay);
        }
        object stateChangeCC = new object();
        public void AddStateChange(LTWMSEFModel.Warehouse.WcsStatus status)
        {
            lock (stateChangeCC)
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                if (status == WcsStatus.Connected)
                {
                    DbExecuteLog("[" + Wcs_srv_guid + "]wcs连接成功...", randDiff);
                }
                else
                {
                    DbExecuteLog("[" + Wcs_srv_guid + "]与wcs断开连接...", randDiff);
                }
                var _whWcs = bll_wh_service_status.GetFirstDefault(w => w.wcs_srv_guid == Wcs_srv_guid &&
                 w.wcstype == LTWMSEFModel.Warehouse.WCSType.WCSServer);
                if (_whWcs != null)
                {//修改  
                    _whWcs.wcs_status = status;
                    _whWcs.ip = Wcs_srv_Ip;// Services.WinServiceFactory.Config.wcsIp;
                    _whWcs.port = Wcs_srv_Port;// Services.WinServiceFactory.Config.wcsPort;
                    _whWcs.desc = Wcs_srv_Name;
                    bll_wh_service_status.Update(_whWcs);
                }
                else
                {//新增
                    _whWcs = new LTWMSEFModel.Warehouse.wh_service_status()
                    {
                        createdate = DateTime.Now,
                        guid = Guid.NewGuid(),
                        createuser = "WMS服务",
                        ip = Wcs_srv_Ip,// Services.WinServiceFactory.Config.wcsIp,
                        port = Wcs_srv_Port,// Services.WinServiceFactory.Config.wcsPort,
                        //  number = 1001,
                        desc = Wcs_srv_Name,
                        state = LTWMSEFModel.EntityStatus.Normal,
                        //  warehouse_guid = warehouse.guid,
                        wcs_srv_guid = Wcs_srv_guid,
                        wcstype = LTWMSEFModel.Warehouse.WCSType.WCSServer,
                        wcs_status = status
                    };
                    bll_wh_service_status.Add(_whWcs);
                }
            }
        }
        object addStateServer = new object();
        /// <summary>
        /// 服务状态改变
        /// </summary>
        /// <param name="status"></param>
        public void AddStateChange_Server(WcsStatus status, string remote)
        {
            lock (addStateServer)
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                if (!string.IsNullOrWhiteSpace(remote))
                {//远程连接不为空
                    DbExecuteLog("[" + Wcs_srv_guid + "]WCS连接成功...==>" + remote, randDiff);
                }
                else
                {//连接为空，断开连接
                    DbExecuteLog("[" + Wcs_srv_guid + "]与WCS断开连接...", randDiff);
                }
                var _whWcs = bll_wh_service_status.GetFirstDefault(w => w.wcs_srv_guid == Wcs_srv_guid &&
                w.wcstype == LTWMSEFModel.Warehouse.WCSType.WCSServer);
                if (_whWcs != null)
                {//修改
                    _whWcs.wcs_status = status;
                    _whWcs.ip = Wcs_srv_Ip;// Services.WinServiceFactory.Config.WMSServerIP;
                    _whWcs.port = Wcs_srv_Port;// Services.WinServiceFactory.Config.WMSServerPort;
                    _whWcs.desc = Wcs_srv_Name;
                    if (_whWcs.wcs_status == WcsStatus.Connected)
                    {
                        _whWcs.memo = remote;
                    }
                    else
                    {
                        _whWcs.memo = "";
                    }
                    var rtv = bll_wh_service_status.Update(_whWcs);
                    if (rtv != LTWMSEFModel.SimpleBackValue.True)
                    {
                        Services.WinServiceFactory.Log.v("99295344添加数据失败:" + Enum.GetName(typeof(LTWMSEFModel.SimpleBackValue), rtv));
                    }
                }
                else
                {//新增
                    _whWcs = new LTWMSEFModel.Warehouse.wh_service_status()
                    {
                        createdate = DateTime.Now,
                        guid = Guid.NewGuid(),
                        createuser = "winserver",
                        ip = Wcs_srv_Ip,//Services.WinServiceFactory.Config.WMSServerIP,
                        port = Wcs_srv_Port,//Services.WinServiceFactory.Config.WMSServerPort,
                        //   number = 4001,
                        desc = Wcs_srv_Name,
                        state = LTWMSEFModel.EntityStatus.Normal,
                        //warehouse_guid = warehouse.guid,
                        wcs_srv_guid = Wcs_srv_guid,
                        wcstype = LTWMSEFModel.Warehouse.WCSType.WCSServer,
                        wcs_status = status
                    };
                    if (_whWcs.wcs_status == LTWMSEFModel.Warehouse.WcsStatus.Connected)
                    {
                        _whWcs.memo = remote;
                    }
                    else
                    {
                        _whWcs.memo = "";
                    }
                    var rtv = bll_wh_service_status.Add(_whWcs);
                    if (rtv != LTWMSEFModel.SimpleBackValue.True)
                    {
                        Services.WinServiceFactory.Log.v("999984744添加数据失败:" + Enum.GetName(typeof(LTWMSEFModel.SimpleBackValue), rtv));
                    }
                }
            }
        }
        /// <summary>
        /// 服务状态改变
        /// </summary>
        /// <param name="status"></param>
        public void AddStateChange_Server(WcsStatus status)
        {
            AddStateChange_Server(status, "");
        }


        /// <summary>
        /// 发送待发送的消息
        /// </summary>
        /// <param name="_SocketC"></param>
        /// <param name="SocketServer"></param>
        public void SendWaitedMessage(LTProtocol.Tcp.Socket_Client_New SocketClient, LTProtocol.Tcp.Socket_Server SocketServer)
        {
            var ListMessageSended = bll_hdw_message_waitedsend.GetAllQueryOrderby(o => o.createdate, w =>
           w.send_status == LTWMSEFModel.Hardware.InterfaceWaitedSendStatus.None && w.wcs_srv_guid == Wcs_srv_guid
        && w.message_type == LTWMSEFModel.Hardware.InterfaceMessageTypeEnum.WCS
        , true);
            if (ListMessageSended != null && ListMessageSended.Count > 0)
            {
                foreach (var item in ListMessageSended)
                {
                    bool sendOk = false;
                    string Msg = "";
                    bool commit = false;
                    using (var tran = bll_hdw_message_waitedsend.BeginTransaction())
                    {
                        try
                        {
                            if (SocketClient != null)
                            {
                                sendOk = SocketClient.SendMessage(item.json_data);
                            }
                            else if (SocketServer != null)
                            {
                                sendOk = SocketServer.SendALL(item.json_data);
                            }
                            //修改发送状态
                            if (sendOk)
                            {
                                item.send_date = DateTime.Now;
                                item.send_status = LTWMSEFModel.Hardware.InterfaceWaitedSendStatus.SendOk;
                                item.updatedate = DateTime.Now;
                                var rtv1 = bll_hdw_message_waitedsend.Update(item);
                                if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    //   _tran.Commit();
                                    commit = true;
                                }
                                else
                                {//修改失败
                                    Msg = "修改发送状态为【SendOk】失败！";
                                    sendOk = false;
                                }
                            }
                            else
                            {
                                Msg = "数据发送至WCS失败";
                            }
                        }
                        finally
                        {
                            if (commit)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                    }
                    if (commit == false)
                    {//发送失败或修改失败
                     //   item.send_date = DateTime.Now;
                     // item.send_status = LTWMSEFModel.Hardware.InterfaceWaitedSendStatus.SendOk;
                        item.updatedate = DateTime.Now;
                        item.error_count += 1;
                        item.memo += ";" + Msg;
                        if (item.error_count > 10)
                        {//发送10次失败，不再发送
                            item.send_date = DateTime.Now;
                            item.send_status = InterfaceWaitedSendStatus.Failed;
                        }
                        var rtv1 = bll_hdw_message_waitedsend.Update(item);
                    }
                }
            }
        }

    }
}
