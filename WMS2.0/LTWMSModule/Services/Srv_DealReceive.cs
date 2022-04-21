using LTLibrary.Wms;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Logs;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.StockInOut;
using LTWMSService.ApplicationService.WmsServer.Model;
using LTWMSService.Basic;
using LTWMSService.Hardware;
using LTWMSService.Logs;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.Services
{
    public class Srv_DealReceive : BaseService
    {
        LTWMSService.Warehouse.wh_shelvesBLL bll_wh_shelves;
        LTWMSService.Warehouse.wh_wcs_deviceBLL bll_wh_wcs_device;
        /// <summary>
        /// 出库单据表
        /// </summary>
        LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout;
        LTWMSService.Hardware.hdw_plcBLL bll_hdw_plc;
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        log_sys_alarmBLL bll_sys_alarm_log;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        LTWMSService.Warehouse.wh_trayBLL bll_wh_tray;
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_wh_shelfunits;
        //rabmy_rfid_detailBLL bll_rabmy_rfid_detail;
        wh_service_statusBLL bll_wh_service_status;
        hdw_message_receivedBLL bll_hdw_message_received;
        LTWMSService.ApplicationService.WmsServer.WCSService bll_wcsservice;
        // rabmy_abnormal_basket_infoBLL bll_rabmy_abnormal_basket_info;
        hdw_message_waitedsendBLL bll_hdw_message_waitedsend;
        StockInService srv_StockInService;
        stk_matterBLL bll_stk_matter;
        wh_traymatterBLL bll_wh_traymatter;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        //  hdw_station_requestBLL bll_hdw_station_request;
        public Srv_DealReceive(Guid Wcs_srv_guid, string Wcs_srv_Name, string wcs_srv_ip, int wcs_srv_port) : base(Wcs_srv_guid, Wcs_srv_Name, wcs_srv_ip, wcs_srv_port)
        {
            CreateBLL(GetDbModel());
        }
        public void LogV(string log)
        {
            Services.WinServiceFactory.Log.v(log);
        }
        public void CreateBLL(LTWMSEFModel.LTModel dbmodel)
        {
            // bll_rabmy_abnormal_basket_info = new rabmy_abnormal_basket_infoBLL(dbmodel);
            // bll_rabmy_rfid_detail = new rabmy_rfid_detailBLL(dbmodel);
            bll_wh_shelfunits = new LTWMSService.Warehouse.wh_shelfunitsBLL(dbmodel);
            bll_wh_tray = new LTWMSService.Warehouse.wh_trayBLL(dbmodel);
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbmodel);
            bll_sys_alarm_log = new log_sys_alarmBLL(dbmodel);
            bll_sys_table_id = new sys_table_idBLL(dbmodel);
            bll_hdw_plc = new hdw_plcBLL(dbmodel);
            // bll_hdw_station_request = new hdw_station_requestBLL(dbmodel);
            bll_bill_stockout = new LTWMSService.Bills.bill_stockoutBLL(dbmodel);
            bll_hdw_stacker_taskqueue_his = new hdw_stacker_taskqueue_hisBLL(dbmodel);
            bll_wh_wcs_device = new LTWMSService.Warehouse.wh_wcs_deviceBLL(dbmodel);
            //  bll_sys_control_dic = new sys_control_dicBLL(dbmodel);
            bll_wh_shelves = new LTWMSService.Warehouse.wh_shelvesBLL(dbmodel);
            bll_hdw_message_waitedsend = new hdw_message_waitedsendBLL(dbmodel);
            bll_stk_matter = new stk_matterBLL(dbmodel);
            bll_wh_service_status = new wh_service_statusBLL(dbmodel);
            bll_hdw_message_received = new hdw_message_receivedBLL(dbmodel);
            bll_wh_traymatter = new wh_traymatterBLL(dbmodel);
            // bll_hdw_plc = new hdw_plcBLL(dbmodel);
            //   bll_sys_control_dic = new LTWMSService.Basic.sys_control_dicBLL(dbmodel);
            bll_wcsservice = new LTWMSService.ApplicationService.WmsServer.WCSService(dbmodel, bll_wh_tray, bll_wh_shelfunits,
                  bll_hdw_stacker_taskqueue, bll_sys_control_dic, bll_sys_alarm_log, bll_sys_table_id, bll_hdw_plc, bll_bill_stockout, bll_wh_wcs_device
                  , bll_wh_shelves);
            bll_wcsservice.SetLedObj(ledDisplay);
            bll_wcsservice.OnDbExecuteLog += Bll_wcsservice_OnDbExecuteLog;
            srv_StockInService = new StockInService(dbmodel, bll_sys_control_dic, bll_stk_matter, bll_wh_traymatter, bll_wh_tray);

        }
        public List<wh_warehouse> GetAllWareHouseByWcsSrvGuid(Guid Wcs_srv_guid)
        {
            return bll_wh_shelves.getAllWareHouseByWcsSrvGuid(Wcs_srv_guid);
        }
        private void Bll_wcsservice_OnDbExecuteLog(string logs, int randDiff)
        {
            DbExecuteLog(logs, randDiff);
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
                w.wcstype == WCSType.SRV_DealReceive);
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
                        wcstype = LTWMSEFModel.Warehouse.WCSType.SRV_DealReceive,
                        wcs_status = status
                    };
                    bll_wh_service_status.Add(_whWcs);
                }
            }
        }
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="_SocketC"></param>
        /// <param name="SocketServer"></param>
        public void HandlerReceivedMessage(wh_warehouse warehouse)
        {
            var ListMessageRecv = bll_hdw_message_received.GetAllQueryOrderby(o => o.createdate, w => w.deal_status == LTWMSEFModel.Hardware.InterfaceMessageDealStatus.None
             && w.wcs_srv_guid == Wcs_srv_guid && w.message_type == LTWMSEFModel.Hardware.InterfaceMessageTypeEnum.WCS
             , true);
            if (ListMessageRecv != null && ListMessageRecv.Count > 0)
            {
                foreach (var item in ListMessageRecv)
                {
                    LTWMSEFModel.SimpleBackValue rtvsuccess = LTWMSEFModel.SimpleBackValue.False;
                    string errorMsg = "";
                    bool commit = false;
                    using (var _tran = bll_hdw_message_received.BeginTransaction())
                    {
                        try
                        {
                            // int randDiff = new Random().Next(1, int.MaxValue);
                            int _cmd = WmsHelper.getCmd(item.json_data);
                            if (_cmd > 0)
                            {
                                switch (_cmd)
                                {
                                    //任务完成状态 
                                    case 201:
                                        /*
                                         20210313 16:29:56.994----->>>WMS>>>WCS====>>>{"cmd":101,"seq":1,"task_id":1002,"src_station":100,"dest_station":0,"src_rack":0,"src_col":0,"src_row":0,"dest_rack":1,"dest_col":2,"dest_row":1,"weight":0.0,"barcode":null}
                20210313 16:29:57.205----->>>{"cmd":201,"seq":1,"task_id":1002,"task_status":-1,"task_info":"该库位状态不允许取货[100]."}
                20210313 16:29:57.205----->>>wms接收wcs的json数据>>>>:10003{"cmd":201,"seq":1,"task_id":1002,"task_status":-1,"task_info":"该库位状态不允许取货[100]."}
                                         */

                                        ReceiveTaskStatus receiveTaskStatus = (ReceiveTaskStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(item.json_data, typeof(ReceiveTaskStatus));
                                        var rtvTaskStatus = bll_wcsservice.ReceiveTaskStatus(receiveTaskStatus, Wcs_srv_guid);
                                        if (rtvTaskStatus.success)
                                        {//保存数据成功
                                            errorMsg += rtvTaskStatus.result;
                                            rtvsuccess = LTWMSEFModel.SimpleBackValue.True;
                                        }
                                        else
                                        {
                                            //添加执行日志（不需要判断成功失败，失败做记录，成功也记录）（如果是MVC控制器调用需要判断执行成功与否直接给出界面提示）
                                            //  DbExecuteLog(rtvSvs.result, randDiff);
                                            errorMsg += rtvTaskStatus.result;
                                        }
                                        break;
                                    // case 202://状态信息
                                    //设备状态不存数据库！！！，在接收的时候就处理。
                                    /* ReceiveMachineStatus machineState = (ReceiveMachineStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(item.json_data, typeof(ReceiveMachineStatus));
                                     bll_wcsservice.ReceivePLCStatus(machineState, Wcs_srv_guid);*/
                                    //      break;
                                    case 203:
                                        //请求入库
                                        ReceiveStockIn receiveStockIn = (ReceiveStockIn)Newtonsoft.Json.JsonConvert.DeserializeObject(item.json_data, typeof(ReceiveStockIn));
                                        var rtvSvs = bll_wcsservice.ScanOnShelf(receiveStockIn, curr_username, Wcs_srv_guid);
                                        if (rtvSvs.success)
                                        {//保存数据成功
                                            errorMsg += rtvSvs.result;
                                            rtvsuccess = LTWMSEFModel.SimpleBackValue.True;
                                        }
                                        else
                                        {
                                            //添加执行日志（不需要判断成功失败，失败做记录，成功也记录）（如果是MVC控制器调用需要判断执行成功与否直接给出界面提示）
                                            //  DbExecuteLog(rtvSvs.result, randDiff);
                                            errorMsg += rtvSvs.result;
                                        }
                                        break;
                                    case 204:////1、2、3入库扫码口请求预分配入库站台（） 
                                             //请求预分配站台，暂时简单做，后期跟WCS和立体库关联！！！  
                                        ReceiveBarcodeOfEnd receiveBarcodeOfEnd = (ReceiveBarcodeOfEnd)Newtonsoft.Json.JsonConvert.DeserializeObject(item.json_data, typeof(ReceiveBarcodeOfEnd));
                                        if (receiveBarcodeOfEnd != null && !string.IsNullOrWhiteSpace(receiveBarcodeOfEnd.barcode))
                                        {//尾部条码值
                                            if (receiveBarcodeOfEnd.num == 1)
                                            {//扫描口编号1
                                                rtvsuccess = ReturnStationForRFID(warehouse, 1, receiveBarcodeOfEnd.barcode);

                                                //bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid1,
                                                //  receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                                //LED物料信息显示（南1）
                                                //if (rtvsuccess == LTWMSEFModel.SimpleBackValue.True)
                                                //{
                                                //    ledDisplay.LED1_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));
                                                //}
                                            }
                                            else if (receiveBarcodeOfEnd.num == 2)
                                            {//扫描口编号2
                                                rtvsuccess = ReturnStationForRFID(warehouse, 2, receiveBarcodeOfEnd.barcode);
                                                //   bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid2,
                                                //  receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                                //LED物料信息显示（北1）
                                                //if (rtvsuccess == LTWMSEFModel.SimpleBackValue.True)
                                                //{
                                                //    ledDisplay.LED2_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));
                                                //}
                                            }
                                            else if (receiveBarcodeOfEnd.num == 3)
                                            {//扫描口编号3
                                                rtvsuccess = ReturnStationForRFID(warehouse, 3, receiveBarcodeOfEnd.barcode);
                                                //  bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid3,
                                                //    receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                                //LED物料信息显示（北2）
                                                //if (rtvsuccess == LTWMSEFModel.SimpleBackValue.True)
                                                //{
                                                //    ledDisplay.LED3_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));
                                                //}
                                            }
                                            else
                                            {
                                                errorMsg += "扫码编号【" + receiveBarcodeOfEnd.num + "】错误 详细==>>" + item.json_data;
                                                //DbExecuteLog("扫码编号【" + receiveBarcodeOfEnd.num + "】错误 详细==>>" + json, randDiff);
                                            }
                                        }
                                        else
                                        {
                                            errorMsg += "解析预分配站台请求失败 详细==>>" + item.json_data;
                                            //DbExecuteLog("解析预分配站台请求失败 详细==>>" + json, randDiff);
                                        }

                                        break;
                                    case 205:
                                        //显示托盘对应物料信息至LED屏
                                        ReceiveDisplayLedTaskid receivedisplayled = (ReceiveDisplayLedTaskid)Newtonsoft.Json.JsonConvert.DeserializeObject(item.json_data, typeof(ReceiveDisplayLedTaskid));
                                        if (receivedisplayled != null && receivedisplayled.task_id > 0)
                                        {
                                            var taskMd = bll_hdw_stacker_taskqueue_his.GetFirstDefault(w => w.taskqueue_id == receivedisplayled.task_id);
                                            if (taskMd != null && taskMd.guid != Guid.Empty)
                                            {
                                                var traymatterModel = bll_wcsservice.TrayOutedSet.Where(w => w.traybarcode == taskMd.tray1_barcode).FirstOrDefault();
                                                if (traymatterModel != null && traymatterModel.guid != Guid.Empty)
                                                {
                                                    ledDisplay.LED4_Say(GetDisplayTrayMatterInfo(traymatterModel));
                                                }
                                                else
                                                {
                                                    ledDisplay.LED4_Say("无信息...");
                                                }
                                                //执行到这里表示，消息处理ok
                                                rtvsuccess = LTWMSEFModel.SimpleBackValue.True;
                                            }
                                        }
                                        break;
                                    default:
                                        //未知数据包
                                        // DbExecuteLog("解析到未知数据包：详细==>>" + item.json_data, randDiff);
                                        errorMsg = "解析到未知数据包：详细==>>" + item.json_data;
                                        break;
                                }
                            }
                            else
                            {//接收的数据包异常
                                // DbExecuteLog("接收的数据包异常：详细==>>" + item.json_data, randDiff);
                                errorMsg = "接收的数据包异常：详细==>>" + item.json_data;
                            }
                            //修改接收的消息处理状态
                            if (rtvsuccess == LTWMSEFModel.SimpleBackValue.True)
                            {// 
                                item.updatedate = DateTime.Now;
                                item.handle_date = DateTime.Now;
                                item.memo += ";" + errorMsg;
                                item.deal_status = LTWMSEFModel.Hardware.InterfaceMessageDealStatus.Done;
                                var rvt2 = bll_hdw_message_received.Update(item);
                                if (rvt2 == LTWMSEFModel.SimpleBackValue.True)
                                {//数据提交
                                    //_tran.Commit();
                                    commit = true;
                                }
                                else
                                {
                                    rtvsuccess = LTWMSEFModel.SimpleBackValue.False;
                                    LogV("处理接收到的消息失败111>>>" + item.json_data);
                                    errorMsg += "处理接收到的消息失败111>>>" + item.json_data;
                                }
                            }
                        }
                        catch (System.InvalidOperationException inverr)
                        {
                            rtvsuccess = LTWMSEFModel.SimpleBackValue.False;
                            ResetDbModel();
                            Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:30s13234d11110", inverr);
                            errorMsg += "数据库连接异常，已重新初始化dbContext:30s13234d11110=>>" + inverr.ToString();
                        }
                        catch (Exception ex)
                        {
                            rtvsuccess = LTWMSEFModel.SimpleBackValue.False;
                            LogV("处理数据失败3310023>>" + ex.ToString());
                            errorMsg += "处理数据失败3310023>>" + ex.ToString();
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
                    if (rtvsuccess == LTWMSEFModel.SimpleBackValue.False)
                    {
                        LogV("处理接收到的消息失败222>>>" + item.json_data);
                        item.error_count += 1;
                        item.memo += ";" + errorMsg;
                        if (item.error_count > 10)
                        {
                            item.deal_status = InterfaceMessageDealStatus.Failed;
                            item.handle_date = DateTime.Now;
                        }
                        bll_hdw_message_received.Update(item);
                    }
                }
            }
        }
        /// <summary>
        /// 1、2RFID（输送线）请求预分配入库站台（1-30） 
        /// 请求预分配站台，暂时简单做，后期跟WCS和立体库关联！！！
        /// </summary>
        public LTWMSEFModel.SimpleBackValue ReturnStationForRFID(wh_warehouse warehouse, int rfidnum, string traybarcode)
        {
            // 请求预分配入库站台 => rfid查找自动组盘 保存组盘数据
            //如果是 空料筐请求预分配站台 = 》走异常尺寸出口出去
            /*bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid1,
                                  receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.abnormal, Guid.Empty);*/
            /**********扫码入库口1***********/


            RFIDBarcodeRequest rfidBarReq = new RFIDBarcodeRequest();
            rfidBarReq.rfidBarcode = traybarcode;
            rfidBarReq.num = rfidnum;
            if (!string.IsNullOrWhiteSpace(rfidBarReq.rfidBarcode))
            {//1号RFID请求不为空
                return SaveBindSendStation(warehouse, rfidBarReq, rfidnum);
            }
            return LTWMSEFModel.SimpleBackValue.False;
        }
        private LTWMSEFModel.SimpleBackValue SaveBindSendStation(wh_warehouse warehouse, RFIDBarcodeRequest _barcodeRfid, int rfidnum)
        {
            SendInStationCMD sendcmd = new SendInStationCMD();
            string LedDisplayText = "";
            //判断托盘重复入库，从入库任务队列中判断和货架上判断 
            string _repeatstr = bll_wcsservice.CheckTrayBarcodeIsRepeated(_barcodeRfid.rfidBarcode, "");
            //判断是否组盘，未组盘提示组盘才能入库(其它项目代码后期整理成方法调用)
            var TrayModel = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _barcodeRfid.rfidBarcode);
            string _checkTrayBindMsg = "";
            if (TrayModel != null && TrayModel.guid != Guid.Empty)
            {
                //var _TrayObj = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _barcodeRfid.rfidBarcode);
                int _forHourSec = 14400;//（默认）组盘时间大于4小时需重新组盘
                string _grouptraytimeout = bll_sys_control_dic.GetValueByType(CommDictType.GroupTrayTimeOut, Guid.Empty);
                if (string.IsNullOrWhiteSpace(_grouptraytimeout))
                {//如果组盘超时时间为空，默认设置超时时间
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "14400", Guid.Empty);
                    _grouptraytimeout = "14400";
                }
                _forHourSec = LTLibrary.ConvertUtility.ToInt(_grouptraytimeout);
                if (_forHourSec < 300)
                {//最小设置为300秒
                    _forHourSec = 300;
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "300", Guid.Empty);
                }
                //通过当前站台 获取是否需要组盘动作
                //var deviceObj = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                //  && w.wcs_srv_guid == Wcs_srv_guid && w.number == receiveStockIn.station);
                //所有站台入库必须组盘。。。
                if (TrayModel != null && TrayModel.guid != Guid.Empty &&
                    (TrayModel.isscan != true || TrayModel.scandate == null ||
                     LTLibrary.ConvertUtility.DiffSeconds(TrayModel.scandate.Value, bll_sys_control_dic.getServerDateTime()) > _forHourSec
                    )
                   )
                {//从系统中查找到对应的托盘记录 ,没有扫码记录则提示先组盘扫码。。。  
                 //发警告信息 
                 //log_sys_alarm _alarmA = new log_sys_alarm();
                 //_alarmA.warehouse_guid = warehouse.guid;// lstShelves[0].warehouse_guid;
                 //_alarmA.guid = Guid.NewGuid();
                 //_alarmA.log_date = DateTime.Now;
                 //_alarmA.log_from = AlarmFrom.Transport;
                 //_alarmA.remark = 
                 //bll_sys_alarm_log.Add(_alarmA);
                    _checkTrayBindMsg = "托盘条码[" + _barcodeRfid.rfidBarcode + "]未扫码组盘或组盘超过规定时间【" + (_forHourSec / 60) + "分钟】未入库！请先扫码组盘";
                    //return JsonReturn(false, _alarmA.remark);
                }
            }
            else
            {//托盘数据为空，没有进行组盘动作？
                _checkTrayBindMsg = "托盘条码[" + _barcodeRfid.rfidBarcode + "]未扫码组盘！请先扫码组盘";
            }

            /************************************/
            if (!string.IsNullOrWhiteSpace(_repeatstr) ||
               !string.IsNullOrWhiteSpace(_checkTrayBindMsg))
            {//条码重复内容不为空，则添加异常日志并返回
             //发警告信息
                log_sys_alarm _alarm = new log_sys_alarm();
                _alarm.warehouse_guid = warehouse.guid;
                _alarm.guid = Guid.NewGuid();
                _alarm.log_date = DateTime.Now;
                _alarm.log_from = AlarmFrom.Transport;
                if (!string.IsNullOrWhiteSpace(_repeatstr))
                {//判断是否重复。。。
                    _alarm.remark = "入库扫码编号[" + rfidnum + "]请求预分配站台 ，托盘条码=【" + _barcodeRfid.rfidBarcode + "】 >>" + _repeatstr + " 。";
                    LedDisplayText = _repeatstr;
                }
                else
                {//判断组盘
                    _alarm.remark = _checkTrayBindMsg;
                    LedDisplayText = _checkTrayBindMsg;
                }
                bll_sys_alarm_log.Add(_alarm);
                //DbExecuteLog("入库扫码编号[" + rfidnum + "]请求预分配站台 ，托盘条码=【" + _barcodeRfid.rfidBarcode + "】 >>" + _repeatstr + " 。", 0);
                sendcmd.station = 0; 
                //return;
            }
            else
            {
                //查找准备好状态的堆垛机
                var lstStacker = GetUseableStacker(warehouse);
                //根据堆垛机查找绑定的货架
                List<wh_shelves> lstUseableShelves = bll_wh_shelves.GetShelvesByStacker(lstStacker);
                //判断托盘条码是否指定入库库位

                if (TrayModel != null && TrayModel.guid != Guid.Empty && TrayModel.dispatch_shelfunits_guid != null)
                {//指定了入库库位，过滤入库货架
                    var shelfUnitModel = bll_wh_shelfunits.GetFirstDefault(w => w.guid == TrayModel.dispatch_shelfunits_guid);
                    if (shelfUnitModel != null)
                    {
                        if (lstUseableShelves == null || !lstUseableShelves.Exists(w => w.guid == shelfUnitModel.shelves_guid))
                        {  //不存在，添加、删除其它。。。
                            var _shelveModel = bll_wh_shelves.GetFirstDefault(w => w.guid == shelfUnitModel.shelves_guid);
                            if (_shelveModel != null)
                            {
                                var _listShlve = new List<wh_shelves>();
                                _listShlve.Add(_shelveModel);
                                lstUseableShelves = _listShlve;
                            }
                        }
                        else
                        {//存在
                            lstUseableShelves = lstUseableShelves.Where(w => w.guid == shelfUnitModel.shelves_guid).ToList();
                        }
                    }
                }
                /***************/
                if (lstUseableShelves != null && lstUseableShelves.Count > 0)
                {
                    //预分配对应物料的入库站台
                    int _station = bll_hdw_stacker_taskqueue.GetStationOfRFIDRequest(warehouse.guid, rfidnum, "", lstUseableShelves);//1=rfid编号
                    if (_station == 0)
                    {//没有可用的站台，等待？
                     //?????????????????????????????
                     //?????????????????????????????
                     //?????????????????????????????
                     //wcs 监控到30秒无响应重新发送一条库位分配请求？？？
                        log_sys_alarm _alarm = new log_sys_alarm();
                        _alarm.warehouse_guid = warehouse.guid;
                        _alarm.guid = Guid.NewGuid();
                        _alarm.log_date = DateTime.Now;
                        //_alarm.fexception_type = AlarmTypeEnum.System;
                        _alarm.log_from = AlarmFrom.Transport;
                        _alarm.remark = "没有可用的站台，托盘条码=【" + _barcodeRfid.rfidBarcode + "】未分配入库站台";
                        bll_sys_alarm_log.Add(_alarm);
                        sendcmd.station = 0;
                        LedDisplayText = "没有可用的站台";
                        // return;
                    }
                    else
                    {
                        DbExecuteLog("入库扫码编号[" + rfidnum + "]请求预分配站台>> 已分配【" + _station + "】站台", 0);
                        //给wcs发送预分配的站台编号  
                        sendcmd.station = _station;
                    }
                }
                else
                {
                    log_sys_alarm _alarm = new log_sys_alarm();
                    _alarm.warehouse_guid = warehouse.guid;
                    _alarm.guid = Guid.NewGuid();
                    _alarm.log_date = DateTime.Now;
                    //_alarm.fexception_type = AlarmTypeEnum.System;
                    _alarm.log_from = AlarmFrom.Transport;
                    //_alarm.remark = "堆垛机未准备好或库位未配置";
                    _alarm.remark = "堆垛机未准备好";
                    bll_sys_alarm_log.Add(_alarm);
                    sendcmd.station = 0;
                    LedDisplayText = "堆垛机未准备好";
                    //return;
                }
            }
            sendcmd.barcode = _barcodeRfid.rfidBarcode;
            sendcmd.num = rfidnum;
            sendcmd.seq = Seq;
            string sendJson = Newtonsoft.Json.JsonConvert.SerializeObject(sendcmd);
            /*  //做客户端：不为空则发送
              SocketClient?.SendMessage(sendJson);
              //做服务端：不为空则发送
              SocketServer?.SendALL(sendJson);*/
            var rtvWaitedSend = SaveWaitedSendMessage(sendJson);
            if (rtvWaitedSend == LTWMSEFModel.SimpleBackValue.True)
            {
                if (sendcmd.station > 0)
                {//只有站台大于零，才显示托盘的详细信息，否则显示错误报警信息

                    if (rfidnum == 1)
                    {
                        ledDisplay.LED1_Say(GetLedDisplayText(_barcodeRfid.rfidBarcode));
                    }
                    else if (rfidnum == 2)
                    {
                        ledDisplay.LED2_Say(GetLedDisplayText(_barcodeRfid.rfidBarcode));
                    }
                    else if (rfidnum == 3)
                    {
                        ledDisplay.LED3_Say(GetLedDisplayText(_barcodeRfid.rfidBarcode));
                    }
                }
                else
                {//站台请求失败，LED显示原因
                    if (rfidnum == 1)
                    {
                        ledDisplay.LED1_Say(LedDisplayText);
                    }
                    else if (rfidnum == 2)
                    {
                        ledDisplay.LED2_Say(LedDisplayText);
                    }
                    else if (rfidnum == 3)
                    {
                        ledDisplay.LED3_Say(LedDisplayText);
                    }
                }
                return LTWMSEFModel.SimpleBackValue.True;
            }
            else
            {
                WinServiceFactory.Log.v("RFID=【" + _barcodeRfid.rfidBarcode + "】请求入库站台，分配失败>>数据保存失败。");
            }
            return LTWMSEFModel.SimpleBackValue.False;
        }

        /// <summary>
        /// 保存待发送消息
        /// </summary>
        /// <param name="jsondata"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue SaveWaitedSendMessage(string jsondata)
        {
            var waitedSendModel = new hdw_message_waitedsend();
            waitedSendModel.createdate = DateTime.Now;
            waitedSendModel.createuser = "win服务";
            waitedSendModel.guid = Guid.NewGuid();
            waitedSendModel.json_data = jsondata;
            waitedSendModel.message_type = InterfaceMessageTypeEnum.WCS;
            // waitedSendModel.send_date = DateTime.Now;
            waitedSendModel.send_status = InterfaceWaitedSendStatus.None;
            waitedSendModel.state = LTWMSEFModel.EntityStatus.Normal;
            waitedSendModel.wcs_srv_guid = Wcs_srv_guid;

            return bll_hdw_message_waitedsend.Add(waitedSendModel);
        }
        /// <summary>
        /// 获取可用的堆垛机列表
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        public List<hdw_plc> GetUseableStacker(wh_warehouse warehouse)
        {
            List<hdw_plc> lstPLC = bll_hdw_plc.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.warehouse_guid == warehouse.guid
         && w.shvwcs_srv_guid == Wcs_srv_guid);
            if (lstPLC == null || lstPLC.Count == 0)
            {//plc、输送线等设备状态为空
                WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>plc、输送线等设备状态为空");
                return null;
            }
            //循环wcs控制的所有堆垛机
            return lstPLC.Where(w => w.type == DeviceTypeEnum.Stacker &&
            (w.run_status == PLCRunStatus.Ready || w.run_status == PLCRunStatus.Running)).ToList();
        }
        public string GetLedDisplayText(string traybarcode)
        {
            string _text = "空托盘";
            var listmatterdetail = bll_wh_tray.GetMatterDetailByTrayBarcode(traybarcode);
            if (listmatterdetail != null && listmatterdetail.Count > 0)
            {
                return GetDisplayTrayMatterInfo(listmatterdetail[0]);
            }
            return _text;
        }
        private string GetDisplayTrayMatterInfo(wh_traymatter model)
        {
            string _text = "名称:" + model.name_list;
            _text += "\r\n数量:" + model.number;
            _text += "\r\n批号:" + model.lot_number;
            _text += "\r\n状态:" + LTLibrary.EnumHelper.GetEnumDescription(model.test_status);
            return _text;
        }

    }
}
