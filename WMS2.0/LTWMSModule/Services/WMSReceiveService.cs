using LTWMSEFModel.Basic;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.Basic;
using LTWMSService.Hardware;
using LTWMSEFModel.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTWMSService.Logs;
using LTWMSService.ApplicationService.WmsServer;
using LTWMSService.ApplicationService.WmsServer.Model;
using LTLibrary.Wms;

namespace LTWMSModule.Services
{
    /// <summary>
    /// wcs接收处理线程（单独的dbcontext）
    /// </summary>
    public class WMSReceiveService : BaseService
    {
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_wh_shelfunits;
        LTWMSService.Warehouse.wh_trayBLL bll_wh_tray;
        log_sys_alarmBLL bll_sys_alarm_log;
        LTWMSService.Hardware.hdw_plcBLL bll_hdw_plc;
        //    LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        /// <summary>
        /// 出库单据表
        /// </summary>
        LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout;
        LTWMSService.ApplicationService.WmsServer.WCSService bll_wcsservice;
        LTWMSService.Warehouse.wh_wcs_deviceBLL bll_wh_wcs_device;
        LTWMSService.Warehouse.wh_shelvesBLL bll_wh_shelves;
        hdw_message_receivedBLL bll_hdw_message_received;
        public WMSReceiveService(Guid Wcs_srv_guid, string ServerName, string wcs_srv_ip, int wcs_srv_port) : base(Wcs_srv_guid, ServerName, wcs_srv_ip, wcs_srv_port)
        {
            CreateBLL(GetDbModel());
        }

        public void CreateBLL(LTWMSEFModel.LTModel dbmodel)
        {
            bll_hdw_stacker_taskqueue = new hdw_stacker_taskqueueBLL(dbmodel);
            bll_hdw_stacker_taskqueue_his = new hdw_stacker_taskqueue_hisBLL(dbmodel);
            bll_wh_shelfunits = new LTWMSService.Warehouse.wh_shelfunitsBLL(dbmodel);
            bll_sys_alarm_log = new log_sys_alarmBLL(dbmodel);
            bll_wh_tray = new LTWMSService.Warehouse.wh_trayBLL(dbmodel);
            bll_hdw_plc = new hdw_plcBLL(dbmodel);
            //  bll_sys_control_dic = new sys_control_dicBLL(dbmodel);
            bll_sys_table_id = new sys_table_idBLL(dbmodel);
            bll_bill_stockout = new LTWMSService.Bills.bill_stockoutBLL(dbmodel);
            bll_wh_wcs_device = new LTWMSService.Warehouse.wh_wcs_deviceBLL(dbmodel);
            bll_wh_shelves = new LTWMSService.Warehouse.wh_shelvesBLL(dbmodel);
            bll_hdw_message_received = new hdw_message_receivedBLL(dbmodel);
            bll_wcsservice = new LTWMSService.ApplicationService.WmsServer.WCSService(dbmodel, bll_wh_tray, bll_wh_shelfunits,
                bll_hdw_stacker_taskqueue, bll_sys_control_dic, bll_sys_alarm_log, bll_sys_table_id, bll_hdw_plc, bll_bill_stockout, bll_wh_wcs_device
                , bll_wh_shelves);
            bll_wcsservice.SetLedObj(ledDisplay);
            bll_wcsservice.OnDbExecuteLog += Bll_wcsservice_OnDbExecuteLog;
        }

        private void Bll_wcsservice_OnDbExecuteLog(string logs, int randDiff)
        {
            DbExecuteLog(logs, randDiff);
        }
        List<string> jsonData = new List<string>();
        public void ReceiveHandler(string json, LTProtocol.Tcp.Socket_Client_New socket, LTProtocol.Tcp.Socket_Server SocketServer)
        {
            if (json == "{100}" || json == "{200}")
            {//心跳包直接返回
                //DbExecuteLog("接收的数据包异常：详细==>>" + json, randDiff);
                return;
            }
            lock (jsonData)
            {
                SaveJsonData(json);

                if (jsonData.Count > 0)
                {//保持失败的继续保存
                    WinServiceFactory.Log.v("接收wcs-socket消息>>错误总计：jsonData.Count=" + jsonData.Count);
                    //  foreach (var jsonItem in jsonData)
                    for (int i = jsonData.Count - 1; i >= 0; i--)
                    {
                        var jsonItem = jsonData[i];
                        if (SaveJsonData(jsonItem))
                        {
                            WinServiceFactory.Log.v("保存成功，即将删除：>>" + jsonItem);
                            jsonData.Remove(jsonItem);
                        }
                    }
                    WinServiceFactory.Log.v("接收wcs-socket消息>>错误总计22：jsonData.Count=" + jsonData.Count);
                }
                
            }
        }
        private bool SaveJsonData(string json)
        {
            bool _saveStatus = false;
            int _cmd = WmsHelper.getCmd(json);
            if (_cmd == 202)
            {//处理设备状态，不存数据库
                ReceiveMachineStatus machineState = (ReceiveMachineStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveMachineStatus));
                bll_wcsservice.ReceivePLCStatus(machineState, Wcs_srv_guid);
                _saveStatus = true;
            }
            else
            {
                try
                {
                    //保存至数据库
                    var messRecv = new LTWMSEFModel.Hardware.hdw_message_received();
                    messRecv.createdate = DateTime.Now;
                    messRecv.deal_status = LTWMSEFModel.Hardware.InterfaceMessageDealStatus.None;
                    messRecv.guid = Guid.NewGuid();
                    messRecv.json_data = json;
                    messRecv.message_type = LTWMSEFModel.Hardware.InterfaceMessageTypeEnum.WCS;
                    messRecv.state = LTWMSEFModel.EntityStatus.Normal;
                    messRecv.wcs_srv_guid = Wcs_srv_guid;
                    var rtv1 = bll_hdw_message_received.Add(messRecv);
                    if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                    {//保存成功
                        _saveStatus = true;
                    }
                    else
                    {
                        if (!jsonData.Contains(json))
                        {
                            jsonData.Insert(0,json);
                            WinServiceFactory.Log.v("接收WCS数据，保持数据失败>>>json:【" + json + "】");
                        }
                    }
                }
                catch (System.InvalidOperationException inverr)
                {
                    if (!jsonData.Contains(json))
                    {
                        jsonData.Insert(0,json);
                    }
                    ResetDbModel();
                    Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:77777120sd23【" + inverr.ToString() + "】");
                    DbExecuteLog("数据库连接异常，已重新初始化dbContext:77777120sd23【" + inverr.ToString() + "】", 0);
                }
                catch (Exception ex)
                {
                    if (!jsonData.Contains(json))
                    {
                        jsonData.Insert(0,json);
                    }
                    WinServiceFactory.Log.v("接收WCS数据，保持数据失败>>>json:【" + json + "】，异常：【" +
                        ex.ToString() + "】");
                }
            }
            return _saveStatus;
        }

        /*
        public void ReceiveHandler(string json, LTProtocol.Tcp.Socket_Client_New socket, LTProtocol.Tcp.Socket_Server SocketServer)
        {
            int randDiff = new Random().Next(1, int.MaxValue);
            int _cmd = WmsHelper.getCmd(json);
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
                         * /
        ReceiveTaskStatus receiveTaskStatus = (ReceiveTaskStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveTaskStatus));
                        bll_wcsservice.ReceiveTaskStatus(receiveTaskStatus, Wcs_srv_guid);
                        break;
                    case 202://设备状态信息
                        ReceiveMachineStatus machineState = (ReceiveMachineStatus)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveMachineStatus));
                        bll_wcsservice.ReceivePLCStatus(machineState, Wcs_srv_guid);
                        break;
                    case 203:
                        //请求入库
                        ReceiveStockIn receiveStockIn = (ReceiveStockIn)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveStockIn));
                        var rtvSvs = bll_wcsservice.ScanOnShelf(receiveStockIn, curr_username, Wcs_srv_guid);
                        //添加执行日志（不需要判断成功失败，失败做记录，成功也记录）（如果是MVC控制器调用需要判断执行成功与否直接给出界面提示）
                        DbExecuteLog(rtvSvs.result, randDiff);
                        break;
                    case 204://1、2、3入库扫码口请求预分配入库站台（） 
                        //请求预分配站台，暂时简单做，后期跟WCS和立体库关联！！！  
                        ReceiveBarcodeOfEnd receiveBarcodeOfEnd = (ReceiveBarcodeOfEnd)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveBarcodeOfEnd));
                        if (receiveBarcodeOfEnd != null && !string.IsNullOrWhiteSpace(receiveBarcodeOfEnd.barcode))
                        {//尾部条码值
                            if (receiveBarcodeOfEnd.num == 1)
                            {//扫描口编号1
                                bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid1,
                                    receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                //LED物料信息显示（南1）
                                ledDisplay.LED1_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));

                            }
                            else if (receiveBarcodeOfEnd.num == 2)
                            {//扫描口编号2
                                bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid2,
                                receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                //LED物料信息显示（北1）
                                ledDisplay.LED2_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));
                            }
                            else if (receiveBarcodeOfEnd.num == 3)
                            {//扫描口编号3
                                bll_sys_control_dic.SetValueByType(CommDictType.BarcodeOfRequest_Rfid3,
                                  receiveBarcodeOfEnd.barcode + "#" + receiveBarcodeOfEnd.num, Guid.Empty);
                                //LED物料信息显示（北2）
                                ledDisplay.LED3_Say(GetLedDisplayText(receiveBarcodeOfEnd.barcode));
                            }
                            else
                            {
                                DbExecuteLog("扫码编号【" + receiveBarcodeOfEnd.num + "】错误 详细==>>" + json, randDiff);
                            }
                        }
                        else
                        {
                            DbExecuteLog("解析预分配站台请求失败 详细==>>" + json, randDiff);
                        }
                        break;
                    /* ReceiveBarcodeOfEnd receiveBarcodeOfEnd = (ReceiveBarcodeOfEnd)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveBarcodeOfEnd));
                     if (receiveBarcodeOfEnd != null && !string.IsNullOrWhiteSpace(receiveBarcodeOfEnd.barcode))
                     {//尾部条码值
                         WinServiceFactory.BarcodeOfEnd = receiveBarcodeOfEnd.barcode;
                     }
                     else
                     {
                         DbExecuteLog("解析尾部人工扫码失败 详细==>>" + json, randDiff);
                     }
                     break;* /
                    case 205:
                        //显示托盘对应物料信息至LED屏
                        ReceiveDisplayLedTaskid receivedisplayled = (ReceiveDisplayLedTaskid)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveDisplayLedTaskid));
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
                            }
                        }
                        break;
                    default:
                        //未知数据包
                        DbExecuteLog("解析到未知数据包：详细==>>" + json, randDiff);
                        break;
                }
            }
            else
            {//接收的数据包异常
                if (json != "{100}")
                {
                    DbExecuteLog("接收的数据包异常：详细==>>" + json, randDiff);
                }
            }
        }
        public string GetLedDisplayText(string traybarcode)
        {
            string _text = "";
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

        */
    }
}
