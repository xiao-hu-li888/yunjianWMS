using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.Services
{
    public class Srv_DealToHistory : BaseService
    {
        wh_service_statusBLL bll_wh_service_status;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        hdw_message_receivedBLL bll_hdw_message_received;
        hdw_message_waitedsendBLL bll_hdw_message_waitedsend;
        hdw_message_received_hisBLL bll_hdw_message_received_his;
        hdw_message_waitedsend_hisBLL bll_hdw_message_waitedsend_his;
        //   hdw_station_requestBLL bll_hdw_station_request;
        //   hdw_station_request_hisBLL bll_hdw_station_request_his;
        public Srv_DealToHistory(Guid Wcs_srv_guid, string Wcs_srv_Name, string wcs_srv_ip, int wcs_srv_port) : base(Wcs_srv_guid, Wcs_srv_Name, wcs_srv_ip, wcs_srv_port)
        {
            CreateBLL(GetDbModel());
        }
        public void CreateBLL(LTWMSEFModel.LTModel dbmodel)
        {
            bll_wh_service_status = new wh_service_statusBLL(dbmodel);
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbmodel);
            bll_hdw_message_received = new hdw_message_receivedBLL(dbmodel);
            bll_hdw_message_waitedsend = new hdw_message_waitedsendBLL(dbmodel);
            bll_hdw_message_received_his = new hdw_message_received_hisBLL(dbmodel);
            bll_hdw_message_waitedsend_his = new hdw_message_waitedsend_hisBLL(dbmodel);
            // bll_hdw_station_request = new hdw_station_requestBLL(dbmodel);
            // bll_hdw_station_request_his = new hdw_station_request_hisBLL(dbmodel);
            // bll_hdw_plc = new hdw_plcBLL(dbmodel);
            //   bll_sys_control_dic = new LTWMSService.Basic.sys_control_dicBLL(dbmodel);

        }
        object stateChangeCC = new object();
        public void AddStateChange(LTWMSEFModel.Warehouse.WcsStatus status)
        {
            lock (stateChangeCC)
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                //if (status == WcsStatus.Connected)
                //{
                //    DbExecuteLog("[" + Wcs_srv_guid + "]wcs连接成功...", randDiff);
                //}
                //else
                //{
                //    DbExecuteLog("[" + Wcs_srv_guid + "]与wcs断开连接...", randDiff);
                //}

                var _whWcs = bll_wh_service_status.GetFirstDefault(w => w.wcs_srv_guid == Wcs_srv_guid &&
                w.wcstype == WCSType.SRV_DealToHistory);
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
                        wcstype = WCSType.SRV_DealToHistory,
                        wcs_status = status
                    };
                    bll_wh_service_status.Add(_whWcs);
                }
            }
        }
        /// <summary>
        /// 存历史数据
        /// </summary>
        public void DealToHistory()
        {
            //将取消或者完成的任务存入历史表中
            SetTaskToHistory();
            //接收到的消息归入历史
            MessageToHis();
            //请求站台转历史表
            // SetStationRequestToHistory();
            BackUpDataBase();
        }
        bool CanBackUp = false;//是否可备份
        bool isBacked = false;//是否备份
        /// <summary>
        /// 备份数据库
        /// </summary>
        private void BackUpDataBase()
        {
            //每天定时晚上10-11点自动备份数据库
            //if (DateTime.Now.Hour == 22)
            if (DateTime.Now.Hour == 1)
            {
                if (!isBacked)
                {
                    CanBackUp = true;
                }
            }
            else
            {//其它时段不进行备份
                CanBackUp = false;
                isBacked = false;
            }
            if (CanBackUp)
            {
                string savedir = WinServiceFactory.rootDir + "/db_backup/";
                LTLibrary.FileHelper.CreateDirectoryIfNotExists(savedir);
                string bakPath = savedir + "WMS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";

                using (SqlConnection conn = new SqlConnection(WinServiceFactory.Config.GetSqlConnectionStr))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand();
                        comm.Connection = conn;
                        comm.CommandText = "use master;backup database @dbname to disk = @backupname;";
                        comm.Parameters.Add(new SqlParameter("dbname", SqlDbType.NVarChar));
                        comm.Parameters["dbname"].Value = "LTDB-WMS-ShangShengSuo";
                        comm.Parameters.Add(new SqlParameter("backupname", SqlDbType.NVarChar));
                        comm.Parameters["backupname"].Value = bakPath;
                        comm.CommandType = CommandType.Text;
                        comm.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                isBacked = true;
                CanBackUp = false;
            }
        }
        /* /// <summary>
         /// 请求站台转历史表
         /// </summary>
         private void SetStationRequestToHistory()
         {
             var listStationRequest = bll_hdw_station_request.GetAllQuery(w => w.deal_status == DealStatusEnum.WaitToHistory);
             if (listStationRequest != null && listStationRequest.Count > 0)
             {
                 foreach (var item in listStationRequest)
                 {
                     using (var _tran = bll_hdw_station_request_his.BeginTransaction())
                     {
                         var stationrequesthis = new hdw_station_request_his();
                         stationrequesthis.abnormal = item.abnormal;
                         stationrequesthis.bind_date = item.bind_date;
                         stationrequesthis.createdate = item.createdate;
                         stationrequesthis.createuser = item.createuser;
                         stationrequesthis.deal_status = item.deal_status;
                         stationrequesthis.dispatch_station = item.dispatch_station;
                         stationrequesthis.fnum = item.fnum;
                         stationrequesthis.fproduct_code = item.fproduct_code;
                         stationrequesthis.fproduct_direction = item.fproduct_direction;
                         stationrequesthis.fproduct_name = item.fproduct_name;
                         stationrequesthis.fproduct_specsign = item.fproduct_specsign;
                         stationrequesthis.frfid16_no = item.frfid16_no;
                         stationrequesthis.frfid_innercode = item.frfid_innercode;
                         stationrequesthis.frfid_no = item.frfid_no;
                         stationrequesthis.fweight = item.fweight;
                         stationrequesthis.guid = Guid.NewGuid();
                         stationrequesthis.is_byproduct = item.is_byproduct;
                         stationrequesthis.is_in_timeout = item.is_in_timeout;
                         stationrequesthis.memo = item.memo;
                         stationrequesthis.product_type = item.product_type;
                         stationrequesthis.real_station = item.real_station;
                         stationrequesthis.rfid_num = item.rfid_num;
                         stationrequesthis.state = item.state;
                         stationrequesthis.tray_barcode = item.tray_barcode;
                         stationrequesthis.updatedate = item.updatedate;
                         stationrequesthis.updateuser = item.updateuser;
                         var rtvstationrequest = bll_hdw_station_request_his.Add(stationrequesthis);
                         var rtvstationrequestdel = bll_hdw_station_request.Delete(item);
                         if (rtvstationrequest == LTWMSEFModel.SimpleBackValue.True
                             && rtvstationrequestdel == LTWMSEFModel.SimpleBackValue.True)
                         {
                             _tran.Commit();
                         }
                     }
                 }
             }
         }*/
        /// <summary>
        ///将取消或者完成的任务存入历史表中
        /// </summary>
        private void SetTaskToHistory()
        {
            List<hdw_stacker_taskqueue> lstTaskqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.taskstatus ==
            WcsTaskStatus.Canceled || w.taskstatus == WcsTaskStatus.Finished || w.taskstatus == WcsTaskStatus.ForceComplete);
            if (lstTaskqueue != null && lstTaskqueue.Count > 0)
            {

                foreach (var item in lstTaskqueue)
                {
                    bool commit = false;
                    using (var _tran = bll_hdw_stacker_taskqueue.BeginTransaction())
                    {
                        try
                        {
                            var rtv = bll_hdw_stacker_taskqueue.AddToHistory(item);
                            if (rtv != LTWMSEFModel.SimpleBackValue.True)
                            {
                                Services.WinServiceFactory.Log.v("8772227777520修改数据失败:" + Enum.GetName(typeof(LTWMSEFModel.SimpleBackValue), rtv));
                            }
                            else
                            {//添加成功，删除
                                var rtv2 = bll_hdw_stacker_taskqueue.Delete(item);
                                if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    commit = true;
                                }
                            }
                        }
                        finally
                        {
                            if (commit)
                            {
                                _tran.Commit();
                            }
                            else
                            {
                                _tran.Rollback();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///接收到的消息归入历史
        /// </summary>
        private void MessageToHis()
        {
            //接收到的消息归入历史
            var ListMessageRecv = bll_hdw_message_received.GetAllQueryOrderby(o => o.createdate, w =>
            (w.deal_status == LTWMSEFModel.Hardware.InterfaceMessageDealStatus.Done || w.deal_status == LTWMSEFModel.Hardware.InterfaceMessageDealStatus.Failed)
          && w.message_type == LTWMSEFModel.Hardware.InterfaceMessageTypeEnum.WCS
          , true);
            if (ListMessageRecv != null && ListMessageRecv.Count > 0)
            {
                foreach (var item in ListMessageRecv)
                {
                    bool commit = false;
                    using (var tran = bll_hdw_message_received.BeginTransaction())
                    {
                        try
                        {
                            var mess_recv_his = new LTWMSEFModel.Hardware.hdw_message_received_his();
                            mess_recv_his.createdate = item.createdate;
                            mess_recv_his.createuser = item.createuser;
                            mess_recv_his.deal_status = item.deal_status;
                            mess_recv_his.error_count = item.error_count;
                            mess_recv_his.guid = Guid.NewGuid();
                            mess_recv_his.handle_date = item.handle_date;
                            mess_recv_his.json_data = item.json_data;
                            mess_recv_his.memo = item.memo;
                            mess_recv_his.warehouse_guid = item.warehouse_guid;
                            mess_recv_his.wcs_srv_guid = item.wcs_srv_guid;
                            mess_recv_his.message_type = item.message_type;
                            mess_recv_his.state = item.state;
                            mess_recv_his.updatedate = item.updatedate;
                            mess_recv_his.updateuser = item.updateuser;
                            var rtv1 = bll_hdw_message_received_his.Add(mess_recv_his);
                            var rtv2 = bll_hdw_message_received.Delete(item);
                            if (rtv1 == LTWMSEFModel.SimpleBackValue.True &&
                                rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {
                                //_tran.Commit();
                                commit = true;
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
                }
            }


            //发送完毕的消息归入历史
            var ListMessageSended = bll_hdw_message_waitedsend.GetAllQueryOrderby(o => o.createdate, w =>
            (w.send_status == LTWMSEFModel.Hardware.InterfaceWaitedSendStatus.SendOk || w.send_status == LTWMSEFModel.Hardware.InterfaceWaitedSendStatus.Failed)
         && w.message_type == LTWMSEFModel.Hardware.InterfaceMessageTypeEnum.WCS
         , true);
            if (ListMessageSended != null && ListMessageSended.Count > 0)
            {
                foreach (var item in ListMessageSended)
                {
                    bool commit = false;
                    using (var tran = bll_hdw_message_waitedsend.BeginTransaction())
                    {
                        try
                        {
                            var mess_send_his = new LTWMSEFModel.Hardware.hdw_message_waitedsend_his();
                            mess_send_his.createdate = item.createdate;
                            mess_send_his.createuser = item.createuser;
                            mess_send_his.send_status = item.send_status;
                            mess_send_his.error_count = item.error_count;
                            mess_send_his.guid = Guid.NewGuid();
                            mess_send_his.send_date = item.send_date;
                            mess_send_his.json_data = item.json_data;
                            mess_send_his.warehouse_guid = item.warehouse_guid;
                            mess_send_his.wcs_srv_guid = item.wcs_srv_guid;
                            mess_send_his.memo = item.memo;
                            mess_send_his.message_type = item.message_type;
                            mess_send_his.state = item.state;
                            mess_send_his.updatedate = item.updatedate;
                            mess_send_his.updateuser = item.updateuser;
                            var rtv1 = bll_hdw_message_waitedsend_his.Add(mess_send_his);
                            var rtv2 = bll_hdw_message_waitedsend.Delete(item);
                            if (rtv1 == LTWMSEFModel.SimpleBackValue.True &&
                                rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {
                                //_tran.Commit();
                                commit = true;
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
                }
            }
        }

    }
}
