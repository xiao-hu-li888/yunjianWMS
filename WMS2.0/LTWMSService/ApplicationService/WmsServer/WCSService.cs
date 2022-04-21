using LTWMSEFModel.Hardware;
using LTWMSEFModel.Logs;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.ApplicationService.WmsServer.Model;
using LTWMSService.Basic;
using LTWMSService.Bills;
using LTWMSService.Hardware;
using LTWMSService.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer
{
    public class WCSService : BaseService
    {
        //public int transportNumber1 = 100;//1号输送口编号100
        //public int transportNumber2 = 200;//2号输送口编号200
        //public int transportNumber3 = 300;//3号输送口编号300
        //public int transportNumber4 = 400;//4号输送口编号400

        LTWMSService.Warehouse.wh_trayBLL bll_wh_tray;
        LTWMSService.Warehouse.wh_traymatterBLL bll_wh_traymatter;
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_wh_shelfunits;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        LEDDisplay ledDisplay;
        log_sys_alarmBLL bll_sys_alarm_log;
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        LTWMSService.Hardware.hdw_plcBLL bll_hdw_plc;
        LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout;
        LTWMSService.Warehouse.wh_wcs_deviceBLL bll_wh_wcs_device;
        Warehouse.wh_shelvesBLL bll_wh_shelves;
        Warehouse.wh_warehouseBLL bll_wh_warehouse;
        LTWMSService.Stock.stk_inout_recodBLL bll_stk_inout_recod;
        bill_stockin_detail_traymatterBLL bll_bill_stockin_detail_traymatter;
        bill_stockinBLL bll_bill_stockin;
        bill_stockin_detailBLL bll_bill_stockin_detail;
        bill_task_tray_relationBLL bll_bill_task_tray_relation;
        bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter;
        public WCSService(LTWMSEFModel.LTModel dbcontext, LTWMSService.Warehouse.wh_trayBLL bll_wh_tray,
            LTWMSService.Warehouse.wh_shelfunitsBLL bll_wh_shelfunits, hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue,
            LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic, log_sys_alarmBLL bll_sys_alarm_log
            , LTWMSService.Basic.sys_table_idBLL bll_sys_table_id, LTWMSService.Hardware.hdw_plcBLL bll_hdw_plc,
             LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout, LTWMSService.Warehouse.wh_wcs_deviceBLL bll_wh_wcs_device,
             Warehouse.wh_shelvesBLL bll_wh_shelves) : base(dbcontext)
        {
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_sys_control_dic = bll_sys_control_dic;
            this.bll_sys_alarm_log = bll_sys_alarm_log;
            this.bll_sys_table_id = bll_sys_table_id;
            this.bll_hdw_plc = bll_hdw_plc;
            this.bll_bill_stockout = bll_bill_stockout;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
            this.bll_wh_shelves = bll_wh_shelves;
            ///
            bll_wh_warehouse = new Warehouse.wh_warehouseBLL(dbcontext);
            bll_wh_traymatter = new Warehouse.wh_traymatterBLL(dbcontext);
            bll_stk_inout_recod = new Stock.stk_inout_recodBLL(dbcontext);
            bll_bill_stockin_detail_traymatter = new bill_stockin_detail_traymatterBLL(dbcontext);
            bll_bill_stockin = new bill_stockinBLL(dbcontext);
            bll_bill_stockin_detail = new bill_stockin_detailBLL(dbcontext);
            bll_bill_task_tray_relation = new bill_task_tray_relationBLL(dbcontext);
            bll_bill_stockout_detail_traymatter = new bill_stockout_detail_traymatterBLL(dbcontext);
        }
        public void SetLedObj(LEDDisplay ledDisplay)
        {
            this.ledDisplay = ledDisplay;
        }
        /// <summary>
        ///检查托盘条码，如果没有记录自动新增一条
        /// </summary>
        /// <param name="traybarcode"></param>
        private void AddTrayIffNotExists(string traybarcode, string curr_username)
        {
            if (!string.IsNullOrWhiteSpace(traybarcode))
            {
                if (!bll_wh_tray.GetAny(w => w.traybarcode == traybarcode))
                {
                    wh_tray trayM = new wh_tray();
                    trayM.createdate = DateTime.Now;
                    trayM.createuser = curr_username;
                    trayM.emptypallet = true;
                    trayM.guid = Guid.NewGuid();
                    trayM.state = LTWMSEFModel.EntityStatus.Normal;
                    trayM.status = TrayStatus.OffShelf;
                    trayM.traybarcode = traybarcode;
                    trayM.memo = "未扫码组盘，系统默认空托盘";
                    bll_wh_tray.AddIfNotExists(trayM, w => w.traybarcode);
                }
            }
        }
        /// <summary>
        /// 站台对应的货架
        /// </summary>
        System.Collections.Hashtable tableShelvesOfStation = new System.Collections.Hashtable();
        /// <summary>
        /// 扫码请求入库
        /// </summary>
        /// <param name="receiveStockIn"></param>
        /// <param name="curr_username"></param>
        public ComServiceReturn ScanOnShelf(ReceiveStockIn receiveStockIn, string curr_username, Guid Wcs_srv_guid)
        {
            if (receiveStockIn == null ||
                (string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode)
                && string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode)))
            {//如果解析为空，或者条码解析都为空则直接返回 
                return JsonReturn(false, "请求入库解析数据异常：条码为空 详细==>>"
                    + Newtonsoft.Json.JsonConvert.SerializeObject(receiveStockIn));
            }
            //判断系统中是否存在该条码，不存在则添加，默认托盘为空托盘
            AddTrayIffNotExists(receiveStockIn.x_1_barcode, curr_username);
            // AddTrayIffNotExists(receiveStockIn.x_2_barcode, curr_username);
            ///////////////////////////////////////////
            //查询任务下发状态,一般只会出现在首次重启任务的时候 
            //删除旧历史任务，状态为未发送。。。。
            if (!tableShelvesOfStation.ContainsKey(Wcs_srv_guid + "-" + receiveStockIn.station))
            {//根据对应的站台 查找对应的库位
                wh_wcs_device deviceObj2 = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                  && w.wcs_srv_guid == Wcs_srv_guid && w.number == receiveStockIn.station && w.device_type == DeviceTypeEnum.Station);
                tableShelvesOfStation.Add(Wcs_srv_guid + "-" + receiveStockIn.station, bll_wh_shelves.GetAllShelvesByStation(deviceObj2));
            }
            //对应起点站台可分配的库位
            List<wh_shelves> lstShelves = tableShelvesOfStation[Wcs_srv_guid + "-" + receiveStockIn.station] as List<wh_shelves>;
            if (lstShelves == null || lstShelves.Count == 0)
            {//盘点对应的货架是否为空，为空则提示错误。
                return JsonReturn(false, "站台绑定的货架为空。。。。");
            }
            /*
            //判断托盘重复入库，从入库任务队列中判断和货架上判断 
            bool _isrepeat = false;//货架上是否有重复
            List<wh_shelfunits> _exist_whshelfcellList = null;//货架仓位 
            bool _isrepeatTask = false;//任务中是否有重复
            List<hdw_stacker_taskqueue> _exist_TaskList = null;//任务信息 
            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) &&
                !string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode))
            {//两个托盘入库
                string _barcode1 = receiveStockIn.x_1_barcode;//位置1条码
                string _barcode2 = receiveStockIn.x_2_barcode;//位置2条码 
                var _shelfcellArr = bll_wh_shelfunits.GetAllQuery(w => w.depth1_traybarcode == _barcode1 ||
                 w.depth1_traybarcode == _barcode2 || w.depth2_traybarcode == _barcode1
                 || w.depth2_traybarcode == _barcode2);
                if (_shelfcellArr != null && _shelfcellArr.Count > 0)
                {
                    _exist_whshelfcellList = _shelfcellArr;
                    _isrepeat = true;
                }
                if (!_isrepeat)
                {//从任务队列中查找 
                    var _listqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
                    (w.taskstatus == WcsTaskStatus.Exception || w.taskstatus == WcsTaskStatus.Execute || w.taskstatus == WcsTaskStatus.Holding
                    || w.taskstatus == WcsTaskStatus.IsSend || w.taskstatus == WcsTaskStatus.Pause)
                    &&
                     w.tasktype == WcsTaskType.StockIn && (w.tray1_barcode == _barcode1 || w.tray1_barcode == _barcode2
                     || w.tray2_barcode == _barcode1 || w.tray2_barcode == _barcode2));
                    if (_listqueue != null && _listqueue.Count > 0)
                    {
                        //判断如果两个条码对应与正在执行的任务的条码位置一致而且，同时两个条码对应位置存在任务，则认定为同一个任务，系统默认
                        //重新发送一条任务入库任务指令 
                        _exist_TaskList = _listqueue;
                        _isrepeatTask = true;
                    }
                }
            }
            else
            {//单个托盘入库
                string _barcode = "";
                if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode))
                {
                    _barcode = receiveStockIn.x_2_barcode;
                }
                if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode))
                {
                    _barcode = receiveStockIn.x_1_barcode;
                }

                //判断系统中的条码是否已经扫码组盘
                var _TrayObj = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _barcode);
                int _forHourSec = 14400;//（默认）组盘时间大于4小时需重新组盘
                string _grouptraytimeout = bll_sys_control_dic.GetValueByType(CommDictType.GroupTrayTimeOut, Guid.Empty);
                if (string.IsNullOrWhiteSpace(_grouptraytimeout))
                {//如果组盘超时时间为空，默认设置超时时间
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "14400", Guid.Empty);
                }
                _forHourSec = LTLibrary.ConvertUtility.ToInt(_grouptraytimeout);
                if (_forHourSec < 180)
                {//最小设置为180秒
                    _forHourSec = 180;
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "180", Guid.Empty);
                }
                //通过当前站台 获取是否需要组盘动作
                var deviceObj = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                  && w.wcs_srv_guid == Wcs_srv_guid && w.number == receiveStockIn.station);
                if (_TrayObj != null && _TrayObj.guid != Guid.Empty && deviceObj != null && deviceObj.need_scan_bind == true &&
                    (_TrayObj.isscan != true || _TrayObj.scandate == null ||
                     LTLibrary.ConvertUtility.DiffSeconds(_TrayObj.scandate.Value, bll_sys_control_dic.getServerDateTime()) > _forHourSec
                    )
                   )
                {//从系统中查找到对应的托盘记录 ,没有扫码记录则提示先组盘扫码。。。  
                 //发警告信息
                 //3、4站台强制组盘才能入库
                    string _text = "托盘" + _barcode + "未组盘";

                    //if (receiveStockIn.station == transportNumber3)
                    //{
                    ledDisplay?.LED3_Say(_text);
                    // }
                    //else if (receiveStockIn.station == transportNumber4)
                    //{
                    //    ledDisplay?.LED4_Say(_text);
                    //}
                    //1、2站台不强制组盘(通过配置，是否强制组盘)
                    //？？？？？？？？？？？？？？？？？？？？？
                    //////////////////////////////////
                    log_sys_alarm _alarmA = new log_sys_alarm();
                    _alarmA.warehouse_guid = lstShelves[0].warehouse_guid;
                    _alarmA.guid = Guid.NewGuid();
                    _alarmA.log_date = DateTime.Now;
                    _alarmA.log_from = AlarmFrom.Transport;
                    _alarmA.remark = "托盘[" + _barcode + "]未扫码组盘或组盘超过规定时间未入库！请先扫码组盘";
                    bll_sys_alarm_log.Add(_alarmA);
                    return JsonReturn(false, _alarmA.remark);
                }

                var _shelfcellArr = bll_wh_shelfunits.GetAllQuery(w => w.depth1_traybarcode == _barcode
                || w.depth2_traybarcode == _barcode);
                if (_shelfcellArr != null && _shelfcellArr.Count > 0)
                {//从货架上判断
                    _exist_whshelfcellList = _shelfcellArr;
                    _isrepeat = true;
                }
                if (!_isrepeat)
                {//从入库任务队列中判断
                    var _listqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
                    (w.taskstatus == WcsTaskStatus.Exception || w.taskstatus == WcsTaskStatus.Execute || w.taskstatus == WcsTaskStatus.Holding
                    || w.taskstatus == WcsTaskStatus.IsSend || w.taskstatus == WcsTaskStatus.Pause || w.taskstatus == WcsTaskStatus.WaiteDispatchStockCell)
                    && w.tasktype == WcsTaskType.StockIn && (w.tray1_barcode == _barcode || w.tray2_barcode == _barcode));
                    if (_listqueue != null && _listqueue.Count > 0)
                    {
                        _exist_TaskList = _listqueue;
                        _isrepeatTask = true;
                    }
                }
            }
            if (_isrepeat || _isrepeatTask)
            {
                //发警告信息
                log_sys_alarm _alarm = new log_sys_alarm();
                _alarm.warehouse_guid = lstShelves[0].warehouse_guid;
                _alarm.guid = Guid.NewGuid();
                _alarm.log_date = DateTime.Now;
                _alarm.log_from = AlarmFrom.Transport;
                if (_isrepeat)
                {//与已入库条码重复 
                    if (_exist_whshelfcellList != null && _exist_whshelfcellList.Count > 0)
                    {
                        string mess = "";
                        foreach (var item in _exist_whshelfcellList)
                        {
                            var _wareHo = bll_wh_warehouse.GetFirstDefault(w => w.guid == item.warehouse_guid);
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) && receiveStockIn.x_1_barcode == item.depth1_traybarcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_1_barcode + " 与  仓库[" + _wareHo?.name + "]" +
                                    item.rack + "排" + item.columns + "列" + item.rows + "层 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) && receiveStockIn.x_1_barcode == item.depth2_traybarcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_1_barcode + " 与  仓库[" + _wareHo?.name + "]" +
                                   item.rack + "排" + item.columns + "列" + item.rows + "层-纵深2 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode) && receiveStockIn.x_2_barcode == item.depth1_traybarcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_2_barcode + " 与  仓库[" + _wareHo?.name + "]" +
                                    item.rack + "排" + item.columns + "列" + item.rows + "层 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode) && receiveStockIn.x_2_barcode == item.depth2_traybarcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_2_barcode + " 与  仓库[" + _wareHo?.name + "]" +
                                   item.rack + "排" + item.columns + "列" + item.rows + "层-纵深2 物料条码重复;\r\n";
                            }
                        }
                        _alarm.remark = mess;
                    }
                }
                else
                {//入库队列重复
                    if (_exist_TaskList != null && _exist_TaskList.Count > 0)
                    {
                        string mess = "";
                        foreach (var item in _exist_TaskList)
                        {
                            var _wareHo = bll_wh_warehouse.GetFirstDefault(w => w.guid == item.warehouse_guid);
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) && receiveStockIn.x_1_barcode == item.tray1_barcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_1_barcode + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) && receiveStockIn.x_1_barcode == item.tray2_barcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_1_barcode + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode) && receiveStockIn.x_2_barcode == item.tray1_barcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_2_barcode + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 物料条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode) && receiveStockIn.x_2_barcode == item.tray2_barcode)
                            {
                                mess += "物料条码 " + receiveStockIn.x_2_barcode + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 物料条码重复;\r\n";
                            }
                        }
                        _alarm.remark = mess;
                    }
                }
                if (string.IsNullOrWhiteSpace(_alarm.remark))
                {
                    _alarm.remark = "物料条码重复！条码值：" + receiveStockIn.x_1_barcode;
                    if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode))
                    {
                        _alarm.remark += "/" + receiveStockIn.x_2_barcode;
                    }
                }
                // if (receiveStockIn.station == transportNumber3)
                //  {
                ledDisplay?.LED3_Say(_alarm.remark);
                // }
                /* 
                 else if (receiveStockIn.station == transportNumber4)
                 {
                     ledDisplay?.LED4_Say(_alarm.remark);
                 }
                 * /
                bll_sys_alarm_log.Add(_alarm);
                return JsonReturn(false, _alarm.remark);
            }
          */
            wh_tray _TrayObj = null;
            //判断系统中的条码是否已经扫码组盘
            if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode) &&
               !string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode))
            {//两个托盘入库

            }
            else
            {
                string _barcode = "";
                if (!string.IsNullOrWhiteSpace(receiveStockIn.x_2_barcode))
                {
                    _barcode = receiveStockIn.x_2_barcode;
                }
                if (!string.IsNullOrWhiteSpace(receiveStockIn.x_1_barcode))
                {
                    _barcode = receiveStockIn.x_1_barcode;
                }
                _TrayObj = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _barcode);
                int _forHourSec = 14400;//（默认）组盘时间大于4小时需重新组盘
                string _grouptraytimeout = bll_sys_control_dic.GetValueByType(CommDictType.GroupTrayTimeOut, Guid.Empty);
                if (string.IsNullOrWhiteSpace(_grouptraytimeout))
                {//如果组盘超时时间为空，默认设置超时时间
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "14400", Guid.Empty);
                }
                _forHourSec = LTLibrary.ConvertUtility.ToInt(_grouptraytimeout);
                if (_forHourSec < 180)
                {//最小设置为180秒
                    _forHourSec = 180;
                    bll_sys_control_dic.SetValueByType(CommDictType.GroupTrayTimeOut, "180", Guid.Empty);
                }
                //通过当前站台 获取是否需要组盘动作
                var deviceObj = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                  && w.wcs_srv_guid == Wcs_srv_guid && w.number == receiveStockIn.station);
                if (_TrayObj != null && _TrayObj.guid != Guid.Empty && deviceObj != null && deviceObj.need_scan_bind == true &&
                    (_TrayObj.isscan != true || _TrayObj.scandate == null ||
                     LTLibrary.ConvertUtility.DiffSeconds(_TrayObj.scandate.Value, bll_sys_control_dic.getServerDateTime()) > _forHourSec
                    )
                   )
                {//从系统中查找到对应的托盘记录 ,没有扫码记录则提示先组盘扫码。。。  
                 //发警告信息
                 //3、4站台强制组盘才能入库
                    /* string _text = "托盘" + _barcode + "未组盘";                   
                    if (receiveStockIn.station == transportNumber3)
                    {
                        ledDisplay?.LED3_Say(_text);
                    }
                    else if (receiveStockIn.station == transportNumber4)
                    {
                        ledDisplay?.LED4_Say(_text);
                    }*/
                    //1、2站台不强制组盘(通过配置，是否强制组盘)
                    //？？？？？？？？？？？？？？？？？？？？？
                    //////////////////////////////////
                    log_sys_alarm _alarmA = new log_sys_alarm();
                    _alarmA.warehouse_guid = lstShelves[0].warehouse_guid;
                    _alarmA.guid = Guid.NewGuid();
                    _alarmA.log_date = DateTime.Now;
                    _alarmA.log_from = AlarmFrom.Transport;
                    _alarmA.remark = "托盘[" + _barcode + "]未扫码组盘或组盘超过规定时间未入库！请先扫码组盘";
                    bll_sys_alarm_log.Add(_alarmA);
                    return JsonReturn(false, _alarmA.remark);
                }
            }
            //判断托盘重复入库，从入库任务队列中判断和货架上判断 
            string _repeatstr = CheckTrayBarcodeIsRepeated(receiveStockIn.x_1_barcode, receiveStockIn.x_2_barcode);
            if (!string.IsNullOrWhiteSpace(_repeatstr))
            {//条码重复内容不为空，则添加异常日志并返回
             //发警告信息
                log_sys_alarm _alarm = new log_sys_alarm();
                _alarm.warehouse_guid = lstShelves[0].warehouse_guid;
                _alarm.guid = Guid.NewGuid();
                _alarm.log_date = DateTime.Now;
                _alarm.log_from = AlarmFrom.Transport;
                _alarm.remark = _repeatstr;
                bll_sys_alarm_log.Add(_alarm);
                /*   if (receiveStockIn.station == transportNumber3)
                  {
                      ledDisplay?.LED3_Say(_alarm.remark);
                  }
                  else if (receiveStockIn.station == transportNumber4)
                  {
                      ledDisplay?.LED4_Say(_alarm.remark);
                  }
                  */
                return JsonReturn(false, _repeatstr);
            }


            //高并发下可能会出现，删除的同时数据被修改为已发送（数据删除会修改失败）。。一般情况下不会有问题
            /*  var _existHisInList = bll_hdw_stacker_taskqueue.GetTaskInOfHistory(lstShelves, receiveStockIn.station);
              if (_existHisInList != null && _existHisInList.Count > 0)
              {//恢复库位状态
               //   存在历史数据不删除，只提示！！
               //发警告信息
                  log_sys_alarm _alarm = new log_sys_alarm();
                  _alarm.warehouse_guid = lstShelves[0].warehouse_guid;
                  _alarm.guid = Guid.NewGuid();
                  _alarm.log_date = DateTime.Now;
                  _alarm.log_from = AlarmFrom.Transport;
                  _alarm.remark = "站台已存在入库任务，请先等待执行完成！>>条码【" + _existHisInList[0].tray1_barcode + "】";
                  bll_sys_alarm_log.Add(_alarm);
                  return JsonReturn(false, _alarm.remark);
                  /* foreach (var itm_exthis in _existHisInList)
                   {
                       bll_hdw_stacker_taskqueue.Delete(itm_exthis);
                       wh_shelfunits shelfcell = bll_wh_shelfunits.GetFirstDefault(w =>
                             w.guid == itm_exthis.dest_shelfunits_guid);
                       if (shelfcell != null && Guid.Empty != shelfcell.guid)
                       {
                           shelfcell.cellstate = ShelfCellState.CanIn;
                           shelfcell.locktype = ShelfLockType.Normal;
                           bll_wh_shelfunits.Update(shelfcell);
                       }
                   }* /
              }*/
            //查找库位，//如果库位已满则保存任务请求等待分配库位 
            ////////// 单边多排 出库 涉及到移库 
            //   单边多排 分配入库库位(预留几个库位，用于移库操作！)
            wh_shelfunits _shelfC = null;
            if (_TrayObj != null && _TrayObj.dispatch_shelfunits_guid != null)
            {// 判断指定的库位状态是否为指定入库锁。。
                _shelfC = bll_wh_shelfunits.GetFirstDefault(w => w.guid == _TrayObj.dispatch_shelfunits_guid
                && w.special_lock_type == SpecialLockTypeEnum.DispatchLock);
                if (_shelfC != null)
                {
                    //判断入库是否存在干扰，如果存在干扰则不能入库，重新分配库位
                    //或判断对应站台对应的库位是否包含指定的库位
                    if (!lstShelves.Exists(w => w.guid == _shelfC.shelves_guid)
                        || bll_wh_shelfunits.CheckExistBlock(_shelfC) ||
                       !(_shelfC.cellstate == ShelfCellState.CanIn && _shelfC.locktype == ShelfLockType.Normal)
                       )
                    {//库位存在干扰返回null，等待分配库位线程再次进入
                        //解锁指定分配库位锁！！
                        _shelfC.special_lock_type = SpecialLockTypeEnum.Normal;
                        bll_wh_shelfunits.Update(_shelfC);
                        _shelfC = null;
                    }
                    else
                    {//没有阻挡
                        //锁定库位
                        _shelfC.locktype = ShelfLockType.SysLock;
                        _shelfC.cellstate = ShelfCellState.TrayIn;//库位状态：入库中
                        _shelfC.updatedate = DateTime.Now;
                        var rv = bll_wh_shelfunits.Update(_shelfC);
                        if (rv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            //后期加上事务处理！！！！！！！

                        }
                        else
                        {//修改失败，自动分配库位。。？？？ 直接返回失败信息，
                            return JsonReturn(false, "修改指定入库库位信息为入库中失败，添加任务失败。。");
                        }
                    }
                }
            }
            if (_shelfC == null)
            {//指定库位为空，自动分配库位
                _shelfC = bll_wh_shelfunits.GetStoreShelfUnitsAndLock(lstShelves);
            }

            if (_shelfC != null && Guid.Empty != _shelfC.guid)
            {
                //添加入库任务至任务队列 
                var rtvstackerqueueadd = bll_hdw_stacker_taskqueue.Add(AddNewTaskInfo(_shelfC, receiveStockIn, curr_username));
                string _disptext = "物料入库至：" + _shelfC.shelfunits_pos;
                //if (receiveStockIn.station == transportNumber3)
                //{
                //////// ledDisplay?.LED3_Say(_disptext);
                //}
                //else if (receiveStockIn.station == transportNumber4)
                //{
                //    ledDisplay?.LED4_Say(_disptext);
                //}
                if (rtvstackerqueueadd == LTWMSEFModel.SimpleBackValue.True)
                {
                    return JsonReturn(true);
                }
                else
                {
                    return JsonReturn(false, "添加任务失败。。");
                }
            }
            else
            {
                if (lstShelves != null && lstShelves.Count > 0)
                {
                    var _Sfc = new wh_shelfunits();
                    _Sfc.warehouse_guid = lstShelves[0].warehouse_guid;
                    //库位满，系统生成等待分配库位任务
                    var rtv3 = bll_hdw_stacker_taskqueue.Add(AddNewTaskInfo(_Sfc, receiveStockIn, curr_username));
                    if (rtv3 == LTWMSEFModel.SimpleBackValue.True)
                    {
                        ////////////////////    ledDisplay?.LED3_Say("等待分配库位...");
                        //发送警告信息，同时提示库位已满 
                        return JsonReturn(true, "入库任务：库位已满或干涉，等待分配库位...=>>起点:" + receiveStockIn.station + " 条码:"
                            + receiveStockIn.x_1_barcode + "/" +
                            receiveStockIn.x_2_barcode);
                    }
                    else
                    {
                        /////////////////////     ledDisplay?.LED3_Say("入库分配库位失败");
                        return JsonReturn(false, "wcs_srv_guid【" + Wcs_srv_guid + "】，站点【" + receiveStockIn.station + "】入库分配库位失败。");
                    }

                }
                else
                {
                    /////////////////////////////  ledDisplay?.LED3_Say("没有对应的库位，入库分配库位失败。");
                    return JsonReturn(false, "wcs_srv_guid【" + Wcs_srv_guid + "】，站点【" + receiveStockIn.station + "】没有对应的库位，入库分配库位失败。");
                }
                // string _disptext = "库位满，等待分配库位...";
                //if (receiveStockIn.station == transportNumber3)
                //{
                //  ledDisplay?.LED3_Say(_disptext);
                //}
                //else if (receiveStockIn.station == transportNumber4)
                //{
                //    ledDisplay?.LED4_Say(_disptext);
                //} 
            }
        }
        /// <summary>
        /// 判断托盘条码是否重复
        /// </summary>
        /// <param name="traybarcode1"></param>
        /// <param name="traybarcode2"></param>
        /// <returns></returns>
        public string CheckTrayBarcodeIsRepeated(string traybarcode1, string traybarcode2)
        {
            string mess = "";
            if (string.IsNullOrWhiteSpace(traybarcode1) &&
                 string.IsNullOrWhiteSpace(traybarcode2))
            {//两个托盘都为空则返回提示
                mess = "托盘条码为空！";
                return mess;
            }
            bool _isrepeat = false;//货架上是否有重复
            List<wh_shelfunits> _exist_whshelfcellList = null;//货架仓位 
            bool _isrepeatTask = false;//任务中是否有重复
            List<hdw_stacker_taskqueue> _exist_TaskList = null;//任务信息 
            if (!string.IsNullOrWhiteSpace(traybarcode1) &&
                !string.IsNullOrWhiteSpace(traybarcode2))
            {//两个托盘入库
                string _barcode1 = traybarcode1;//位置1条码
                string _barcode2 = traybarcode2;//位置2条码 
                var _shelfcellArr = bll_wh_shelfunits.GetAllQuery(w => w.depth1_traybarcode == _barcode1 ||
                 w.depth1_traybarcode == _barcode2 || w.depth2_traybarcode == _barcode1
                 || w.depth2_traybarcode == _barcode2);
                if (_shelfcellArr != null && _shelfcellArr.Count > 0)
                {
                    _exist_whshelfcellList = _shelfcellArr;
                    _isrepeat = true;
                }
                if (!_isrepeat)
                {//从任务队列中查找 
                    var _listqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
                    (w.taskstatus == WcsTaskStatus.Exception || w.taskstatus == WcsTaskStatus.Execute || w.taskstatus == WcsTaskStatus.Holding
                    || w.taskstatus == WcsTaskStatus.IsSend || w.taskstatus == WcsTaskStatus.Pause)
                    &&
                     w.tasktype == WcsTaskType.StockIn && (w.tray1_barcode == _barcode1 || w.tray1_barcode == _barcode2
                     || w.tray2_barcode == _barcode1 || w.tray2_barcode == _barcode2));
                    if (_listqueue != null && _listqueue.Count > 0)
                    {
                        //判断如果两个条码对应与正在执行的任务的条码位置一致而且，同时两个条码对应位置存在任务，则认定为同一个任务，系统默认
                        //重新发送一条任务入库任务指令 
                        _exist_TaskList = _listqueue;
                        _isrepeatTask = true;
                    }
                }
            }
            else
            {//单个托盘入库
                string _barcode = "";
                if (!string.IsNullOrWhiteSpace(traybarcode2))
                {
                    _barcode = traybarcode2;
                }
                if (!string.IsNullOrWhiteSpace(traybarcode1))
                {
                    _barcode = traybarcode1;
                }
                var _shelfcellArr = bll_wh_shelfunits.GetAllQuery(w => w.depth1_traybarcode == _barcode
                || w.depth2_traybarcode == _barcode);
                if (_shelfcellArr != null && _shelfcellArr.Count > 0)
                {//从货架上判断
                    _exist_whshelfcellList = _shelfcellArr;
                    _isrepeat = true;
                }
                if (!_isrepeat)
                {//从入库任务队列中判断
                    var _listqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal &&
                    (w.taskstatus == WcsTaskStatus.Exception || w.taskstatus == WcsTaskStatus.Execute || w.taskstatus == WcsTaskStatus.Holding
                    || w.taskstatus == WcsTaskStatus.IsSend || w.taskstatus == WcsTaskStatus.Pause || w.taskstatus == WcsTaskStatus.WaiteDispatchStockCell)
                    && w.tasktype == WcsTaskType.StockIn && (w.tray1_barcode == _barcode || w.tray2_barcode == _barcode));
                    if (_listqueue != null && _listqueue.Count > 0)
                    {
                        _exist_TaskList = _listqueue;
                        _isrepeatTask = true;
                    }
                }
            }
            if (_isrepeat || _isrepeatTask)
            {
                if (_isrepeat)
                {//与已入库条码重复 
                    if (_exist_whshelfcellList != null && _exist_whshelfcellList.Count > 0)
                    {
                        foreach (var item in _exist_whshelfcellList)
                        {
                            var _wareHo = bll_wh_warehouse.GetFirstDefault(w => w.guid == item.warehouse_guid);
                            if (!string.IsNullOrWhiteSpace(traybarcode1) && traybarcode1 == item.depth1_traybarcode)
                            {
                                mess += "托盘条码 " + traybarcode1 + " 与  仓库[" + _wareHo?.name + "]" +
                                    item.rack + "排" + item.columns + "列" + item.rows + "层 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode1) && traybarcode1 == item.depth2_traybarcode)
                            {
                                mess += "托盘条码 " + traybarcode1 + " 与  仓库[" + _wareHo?.name + "]" +
                                   item.rack + "排" + item.columns + "列" + item.rows + "层-纵深2 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode2) && traybarcode2 == item.depth1_traybarcode)
                            {
                                mess += "托盘条码 " + traybarcode2 + " 与  仓库[" + _wareHo?.name + "]" +
                                    item.rack + "排" + item.columns + "列" + item.rows + "层 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode2) && traybarcode2 == item.depth2_traybarcode)
                            {
                                mess += "托盘条码 " + traybarcode2 + " 与  仓库[" + _wareHo?.name + "]" +
                                   item.rack + "排" + item.columns + "列" + item.rows + "层-纵深2 托盘条码重复;\r\n";
                            }
                        }
                    }
                }
                else
                {//入库队列重复
                    if (_exist_TaskList != null && _exist_TaskList.Count > 0)
                    {
                        foreach (var item in _exist_TaskList)
                        {
                            var _wareHo = bll_wh_warehouse.GetFirstDefault(w => w.guid == item.warehouse_guid);
                            if (!string.IsNullOrWhiteSpace(traybarcode1) && traybarcode1 == item.tray1_barcode)
                            {
                                mess += "托盘条码 " + traybarcode1 + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode1) && traybarcode1 == item.tray2_barcode)
                            {
                                mess += "托盘条码 " + traybarcode1 + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode2) && traybarcode2 == item.tray1_barcode)
                            {
                                mess += "托盘条码 " + traybarcode2 + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 托盘条码重复;\r\n";
                            }
                            if (!string.IsNullOrWhiteSpace(traybarcode2) && traybarcode2 == item.tray2_barcode)
                            {
                                mess += "托盘条码 " + traybarcode2 + " 与 仓库[" + _wareHo?.name + "]入库任务ID[" + item.id + "] 托盘条码重复;\r\n";
                            }
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(mess))
                {
                    mess = "托盘条码重复！条码值：" + traybarcode1;
                    if (!string.IsNullOrWhiteSpace(traybarcode2))
                    {
                        mess += "/" + traybarcode2;
                    }
                }
            }
            return mess;
        }

        public hdw_stacker_taskqueue AddNewTaskInfo(wh_shelfunits _shelfC, ReceiveStockIn receiveStockIn, string curr_username)
        {
            if (_shelfC == null)
            {
                _shelfC = new wh_shelfunits();
            }
            hdw_stacker_taskqueue _Taskqueue = new hdw_stacker_taskqueue();
            _Taskqueue.id = bll_sys_table_id.GetId(sys_table_idBLL.TableIdType.hdw_stacker_taskqueue);
            _Taskqueue.dest_shelfunits_guid = _shelfC.guid;
            _Taskqueue.createdate = DateTime.Now;
            _Taskqueue.createuser = curr_username;
            _Taskqueue.dest_col = _shelfC.columns;
            _Taskqueue.dest_rack = _shelfC.rack;
            _Taskqueue.dest_row = _shelfC.rows;
            _Taskqueue.warehouse_guid = _shelfC.warehouse_guid;
            _Taskqueue.shelves_guid = _shelfC.shelves_guid;
            _Taskqueue.dest_shelfunits_pos = _shelfC.shelfunits_pos;
            var trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == receiveStockIn.x_1_barcode);
            if (trayM != null && trayM.guid != Guid.Empty)
            {
                _Taskqueue.is_emptypallet = trayM.emptypallet;
            }
            else
            {
                _Taskqueue.is_emptypallet = true;
            }
            _Taskqueue.src_station = receiveStockIn.station;//入库站点 1或2  3/4
                                                            //if (_shelfC.rack == 2)
                                                            //{
            _Taskqueue.tray1_barcode = receiveStockIn.x_1_barcode;
            //    _Taskqueue.tray2_barcode = receiveStockIn.x_2_barcode;
            //}
            //else
            //{//第一排
            //    _Taskqueue.tray1_barcode = receiveStockIn.x_2_barcode;
            //    _Taskqueue.tray2_barcode = receiveStockIn.x_1_barcode;
            //}
            //根据托盘号查询对应的物料信息 
            var lst1 = bll_wh_tray.GetMatterDetailByTrayBarcode(_Taskqueue.tray1_barcode);
            if (lst1 != null && lst1.Count > 0)
            {
                _Taskqueue.tray1_matter_barcode1 = lst1[0].x_barcode;
                if (lst1.Count > 1)
                {
                    _Taskqueue.tray1_matter_barcode2 = lst1[1].x_barcode;
                }
                _Taskqueue.matterbarcode_list = string.Join(",", lst1.Select(w => w.x_barcode).ToArray());
            }
            /* var lst2 = bll_wh_tray.GetMatterDetailByTrayBarcode(_Taskqueue.tray2_barcode);
             if (lst2 != null && lst2.Count > 0)
             {
                 _Taskqueue.tray2_matter_barcode1 = lst2[0].matter_barcode;
                 if (lst2.Count > 1)
                 {
                     _Taskqueue.tray2_matter_barcode2 = lst2[1].matter_barcode;
                 }
             }*/
            _Taskqueue.guid = Guid.NewGuid();
            _Taskqueue.src_col = 0;
            _Taskqueue.src_rack = 0;
            _Taskqueue.src_row = 0;
            _Taskqueue.state = LTWMSEFModel.EntityStatus.Normal;
            //_Taskqueue.taskid = 0;
            if (Guid.Empty == _shelfC.guid)
            {//暂未分配库位
                _Taskqueue.taskstatus = WcsTaskStatus.WaiteDispatchStockCell;
            }
            else
            {
                _Taskqueue.taskstatus = WcsTaskStatus.Holding;
            }
            _Taskqueue.tasktype = WcsTaskType.StockIn;
            _Taskqueue.sort = 1;//设置入库优先级大于出库  
            return _Taskqueue;
        }
        private WcsTaskStatus GetTaskStatus(int _task_status)
        {////  1=任务执行中   2=任务暂停   3=任务完成   4=任务异常  5=任务取消 6=任务强制完成
            switch (_task_status)
            {
                case 1:
                    return WcsTaskStatus.Execute;
                case 2:
                    return WcsTaskStatus.Pause;
                case 3:
                    return WcsTaskStatus.Finished;
                case 4:
                    return WcsTaskStatus.Exception;
                case 5:
                    return WcsTaskStatus.Canceled;
                case 6:
                    return WcsTaskStatus.ForceComplete;
                case -1://任务写入失败
                    return WcsTaskStatus.WriteError;
            }
            return WcsTaskStatus.IsSend;
        }
        public List<wh_traymatter> TrayOutedSet = new List<wh_traymatter>();
        public ComServiceReturn ReceiveTaskStatus(ReceiveTaskStatus receiveTaskStatus, Guid wcssrvguid)
        {
            int randDiff = new Random().Next(1, int.MaxValue);
            LTWMSEFModel.SimpleBackValue rtvtaskqueue = LTWMSEFModel.SimpleBackValue.False;
            LTWMSEFModel.SimpleBackValue dealsuccess = LTWMSEFModel.SimpleBackValue.False;
            string rtvMsg = "";
            //出入库完成。。。  
            hdw_stacker_taskqueue taskInfo = bll_hdw_stacker_taskqueue.GetFirstDefault(w => w.id == receiveTaskStatus.task_id);
            if (taskInfo != null && taskInfo.guid != Guid.Empty)
            {
                WcsTaskStatus oldstatus = taskInfo.taskstatus;
                // DbExecuteLog();
                //OnExecuteLog("已成功解析任务完成状态 任务id=" + receiveTaskStatus.task_id, randDiff);
                //修改任务状态 
                taskInfo.taskstatus = GetTaskStatus(receiveTaskStatus.task_status);
                ///流程字
                taskInfo.stacker_number = receiveTaskStatus.flow.ToString();
                if (taskInfo.taskstatus == WcsTaskStatus.Execute && taskInfo.startup == null)
                {//任务执行为开始时间 起始时间为空则
                    taskInfo.startup = DateTime.Now;
                    // taskInfo.stacker_number = receiveTaskStatus.task_info;//保存堆垛机编号
                }
                else
                {//其它为结束时间
                    taskInfo.endup = DateTime.Now;
                }
                if (oldstatus != taskInfo.taskstatus)
                {//状态有变化
                    taskInfo.memo += DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff")
                        + "=>>" + LTLibrary.EnumHelper.GetEnumDescription(taskInfo.taskstatus) +
                        (//执行失败的错误提示信息。。。
                        string.IsNullOrWhiteSpace(receiveTaskStatus.task_info) ? "" : "【" + receiveTaskStatus.task_info + "】"
                        )
                        + "; ";
                }
                //显示LED内容
                if (taskInfo.tasktype == WcsTaskType.StockIn)
                {//入库
                    string _disptext = "";
                    if (taskInfo.taskstatus == WcsTaskStatus.Finished || taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                    {
                        _disptext = "物料入库至" + taskInfo.dest_shelfunits_pos + "已完成";
                    }
                    else if (taskInfo.taskstatus == WcsTaskStatus.Canceled)
                    {
                        _disptext = "物料入库至" + taskInfo.dest_shelfunits_pos + "已取消";
                    }
                    if (!string.IsNullOrWhiteSpace(_disptext))
                    {
                        //if (taskInfo.src_station == transportNumber3)
                        //{
                        ///////////  ledDisplay.LED3_Say(_disptext);
                        //}
                        //else if (taskInfo.src_station == transportNumber4)
                        //{
                        //    ledDisplay.LED4_Say(_disptext);
                        //}
                    }
                }
                else if (taskInfo.tasktype == WcsTaskType.StockOut)
                {//出库 
                    string _disptext = "";
                    if (taskInfo.taskstatus == WcsTaskStatus.Finished || taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                    {
                        _disptext = "物料从" + taskInfo.src_shelfunits_pos + "出库已完成";
                    }
                    else if (taskInfo.taskstatus == WcsTaskStatus.Canceled)
                    {
                        _disptext = "物料从" + taskInfo.src_shelfunits_pos + "出库已取消";
                    }
                    if (!string.IsNullOrWhiteSpace(_disptext))
                    {
                        //if (taskInfo.dest_station == transportNumber3)
                        //{
                        //    ledDisplay.LED3_Say(_disptext);
                        //}
                        //else if (taskInfo.dest_station == transportNumber4)
                        //{
                        ////////////////   ledDisplay.LED4_Say(_disptext);
                        //}
                    }
                }
                //其它处理  
                switch (taskInfo.tasktype)
                {
                    case WcsTaskType.StockIn://入库 
                        if (taskInfo.taskstatus == WcsTaskStatus.Finished ||
                            taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                        {
                            wh_shelfunits _shelfcell = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.dest_shelfunits_guid);
                            if (_shelfcell != null && _shelfcell.guid != Guid.Empty)
                            {
                                _shelfcell.updatedate = DateTime.Now;
                                _shelfcell.depth1_traybarcode = taskInfo.tray1_barcode;
                                _shelfcell.tray_indatetime = DateTime.Now;
                                _shelfcell.cellstate = ShelfCellState.Stored;
                                _shelfcell.locktype = ShelfLockType.SysLock;//入库完成，防止获取库位时修改失败  
                                _shelfcell.special_lock_type = SpecialLockTypeEnum.Normal;
                                //修改对应托盘状态
                                LTWMSEFModel.SimpleBackValue rtvwhtray = LTWMSEFModel.SimpleBackValue.True;
                                var trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == taskInfo.tray1_barcode);
                                if (trayM != null)
                                {
                                    trayM.shelfunits_guid = _shelfcell.guid;
                                    trayM.shelfunits_pos = _shelfcell.shelfunits_pos;
                                    trayM.status = TrayStatus.OnShelf;
                                    trayM.warehouse_guid = _shelfcell.warehouse_guid;
                                    trayM.dispatch_shelfunits_guid = null;
                                    trayM.dispatch_shelfunits_pos = "";
                                    rtvwhtray = bll_wh_tray.Update(trayM);
                                    //添加入库流水
                                    /* var trayMatterObj = bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == trayM.guid);
                                     if (trayMatterObj != null && trayMatterObj.guid != Guid.Empty)
                                     {
                                         var intouRecod = new LTWMSEFModel.Stock.stk_inout_recod();
                                         intouRecod.createdate = DateTime.Now;
                                         intouRecod.goods_id = trayMatterObj.x_barcode;
                                         intouRecod.guid = Guid.NewGuid();
                                         intouRecod.inout_type = LTWMSEFModel.Stock.InOutTypeEnum.In;
                                         intouRecod.is_send = LTWMSEFModel.Stock.IsSendToEnum.None;
                                         intouRecod.qty = trayMatterObj.number;
                                         intouRecod.spec_id = trayMatterObj.lot_number;
                                         intouRecod.state = LTWMSEFModel.EntityStatus.Normal;
                                         bll_stk_inout_recod.Add(intouRecod);
                                     }*/
                                }
                                //修改数据
                                var rtvshelfunits = bll_wh_shelfunits.Update(_shelfcell);

                                //入库完成 修改入库单入库数量 检测对应入库单是否全部入库完成
                                /************************/
                                LTWMSEFModel.SimpleBackValue rtvtasktrayrelationdel = LTWMSEFModel.SimpleBackValue.True;
                                LTWMSEFModel.SimpleBackValue rtvdetailtraymatterupdate = LTWMSEFModel.SimpleBackValue.True;
                                /************************/
                                // 修改对应的单据信息
                                var reMd = bll_bill_task_tray_relation.GetAllQueryOrderby(o => o.createdate
                                , w => w.traybarcode == taskInfo.tray1_barcode, false).FirstOrDefault();
                                if (reMd != null && reMd.guid != Guid.Empty)
                                {
                                    taskInfo.order = reMd.odd_numbers;
                                    taskInfo.re_detail_traymatter_guid = reMd.re_detail_traymatter_guid;
                                    if (reMd.bill_type == LTWMSEFModel.Bills.ReBillTypeEnum.StockIn)
                                    {

                                        taskInfo.bills_type = BillsTypeEnum.BillsIn;
                                        var MMd = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == reMd.re_detail_traymatter_guid);
                                        if (MMd != null && MMd.guid != Guid.Empty)
                                        {
                                            MMd.tray_status = LTWMSEFModel.Bills.TrayInStockStatusEnum.Stored;
                                            MMd.tray_in_date = DateTime.Now;
                                            MMd.dest_shelfunits_guid = _shelfcell.guid;
                                            MMd.dest_shelfunits_pos = _shelfcell.shelfunits_pos;
                                            MMd.stacker_taskqueue_guid = taskInfo.guid;
                                            rtvdetailtraymatterupdate = bll_bill_stockin_detail_traymatter.Update(MMd);
                                            if (rtvdetailtraymatterupdate == LTWMSEFModel.SimpleBackValue.True)
                                            {
                                                //自动修改入库主单的状态
                                                var _stockInBill = bll_bill_stockin.GetFirstDefault(w => w.guid == MMd.stockin_guid);
                                                //判断实际入库数量是否大于入库数量
                                                if (_stockInBill != null && _stockInBill.guid != Guid.Empty)
                                                {
                                                    var _stockInDetails = bll_bill_stockin_detail.GetFirstDefault(w => w.stockin_guid == _stockInBill.guid);
                                                    if (_stockInDetails != null && _stockInDetails.guid != Guid.Empty)
                                                    {
                                                        //_stockInDetails.in_number;
                                                        var stockInDetailTrayMatter = bll_bill_stockin_detail_traymatter.GetAllQuery(w => w.stockin_guid == _stockInBill.guid
                                                          && w.tray_status == LTWMSEFModel.Bills.TrayInStockStatusEnum.Stored);
                                                        if (stockInDetailTrayMatter != null && stockInDetailTrayMatter.Count > 0)
                                                        {
                                                            if (stockInDetailTrayMatter.Sum(w => w.number) >= _stockInDetails.in_number)
                                                            {
                                                                _stockInBill.bill_status = LTWMSEFModel.Bills.BillsStatus.Finished;
                                                                _stockInBill.updatedate = DateTime.Now;
                                                                _stockInBill.memo += "【"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"】>>入库单自动结束。";
                                                                rtvdetailtraymatterupdate= bll_bill_stockin.Update(_stockInBill);
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (reMd.bill_type == LTWMSEFModel.Bills.ReBillTypeEnum.StockOut)
                                    {
                                        taskInfo.bills_type = BillsTypeEnum.BillsOut;
                                        var MMd = bll_bill_stockout_detail_traymatter.GetFirstDefault(w => w.guid == reMd.re_detail_traymatter_guid);
                                        if (MMd != null && MMd.guid != Guid.Empty)
                                        {
                                            MMd.tray_status = LTWMSEFModel.Bills.TrayOutStockStatusEnum.Stored;
                                            MMd.tray_back_date = DateTime.Now;
                                            MMd.back_shelfunits_guid = _shelfcell.guid;
                                            MMd.back_shelfunits_pos = _shelfcell.shelfunits_pos;
                                            MMd.back_stacker_taskqueue_guid = taskInfo.guid;
                                            rtvdetailtraymatterupdate = bll_bill_stockout_detail_traymatter.Update(MMd);
                                        }
                                    }
                                    //删除关联数据
                                    rtvtasktrayrelationdel = bll_bill_task_tray_relation.Delete(reMd);
                                }
                                /************************/
                                if (rtvshelfunits == LTWMSEFModel.SimpleBackValue.True && rtvwhtray == LTWMSEFModel.SimpleBackValue.True &&
                                    rtvtasktrayrelationdel == LTWMSEFModel.SimpleBackValue.True && rtvdetailtraymatterupdate == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                                }
                                else
                                {
                                    rtvMsg = "入库完成/强制完成修改数据失败>>";
                                }
                                /************************/
                                /************************/
                                // OnExecuteLog("入库任务完成 已修改对应的库位信息：warehouseguid=[" + _shelfcell.warehouse_guid + "][" + _shelfcell.shelfunits_pos + "] 为存储状态", randDiff);
                            }
                        }
                        //根据任务状态==》》执行相应的操作。。；
                        else if (taskInfo.taskstatus == WcsTaskStatus.Canceled)
                        {//解锁库位
                            wh_shelfunits _shelfcell = bll_wh_shelfunits.GetFirstDefault(w =>
                                 w.guid == taskInfo.dest_shelfunits_guid);
                            _shelfcell.cellstate = ShelfCellState.CanIn;
                            _shelfcell.updatedate = DateTime.Now;
                            //取消入库加人工锁，防止人为操作失误，实际托盘已入库位（人工锁的库位可以强制出库）
                            _shelfcell.locktype = ShelfLockType.ManLock;
                            var rtvshelfUnit = bll_wh_shelfunits.Update(_shelfcell);
                            log_sys_alarm _alarm = new log_sys_alarm();
                            _alarm.warehouse_guid = taskInfo.warehouse_guid;
                            _alarm.guid = Guid.NewGuid();
                            _alarm.log_date = DateTime.Now;
                            _alarm.log_from = AlarmFrom.Stacker;
                            _alarm.remark = "入库任务[" + taskInfo.id + "]已取消：warehouseguid=[" + _shelfcell.warehouse_guid + "]目标库位（" +
                                taskInfo.dest_rack + "排" + taskInfo.dest_col + "列" + taskInfo.dest_row + "层）";
                            var rtvalarmlog = bll_sys_alarm_log.Add(_alarm);
                            //OnExecuteLog("入库任务取消 已修改对应的库位信息：warehouseguid=[" + _shelfcell.warehouse_guid + "][" + _shelfcell.shelfunits_pos + "] 可入库/人工锁状态", randDiff);

                            if (rtvalarmlog == LTWMSEFModel.SimpleBackValue.True && rtvshelfUnit == LTWMSEFModel.SimpleBackValue.True)
                            {
                                dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                            }
                            else
                            {
                                rtvMsg = "入库取消修改数据失败>>";
                            }
                        }
                        else if (taskInfo.taskstatus == WcsTaskStatus.Exception)
                        {
                            //发送一条警告信息
                            log_sys_alarm _alarm = new log_sys_alarm();
                            _alarm.warehouse_guid = taskInfo.warehouse_guid;
                            _alarm.guid = Guid.NewGuid();
                            _alarm.log_date = DateTime.Now;
                            _alarm.log_from = AlarmFrom.Stacker;
                            _alarm.remark = "来自WCS的入库任务[" + taskInfo.id + "]异常：warehouseguid=[" + taskInfo.warehouse_guid + "]目标库位（" +
                                taskInfo.dest_rack + "排" + taskInfo.dest_col + "列" + taskInfo.dest_row + "层）";
                            dealsuccess = bll_sys_alarm_log.Add(_alarm);
                        }
                        else
                        {//其它状态，默认true
                            dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                        }
                        break;
                    case WcsTaskType.StockOut:
                        if (taskInfo.taskstatus == WcsTaskStatus.Finished ||
                           taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                        { //出库完成删除托盘数据，解除库位锁定
                          //出库任务完成，将出库准备好置为未准备！！！ 
                            var Md = new LTWMSService.ApplicationService.WmsServer.Model.ReceiveMachineStatus();
                            Md.dev_info = new List<LTWMSService.ApplicationService.WmsServer.Model.DevInfo>();
                            Md.dev_info.Add(new LTWMSService.ApplicationService.WmsServer.Model.DevInfo() { dev_id = taskInfo.dest_station, error_code = 0, error_msg = "", status = 0 });
                            this.ReceivePLCStatus(Md, wcssrvguid);
                            /////////////////////////////
                            wh_shelfunits _shelfcell = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid);

                            LTWMSEFModel.SimpleBackValue rtvshelfunits = LTWMSEFModel.SimpleBackValue.False;
                            LTWMSEFModel.SimpleBackValue rtvdisconnected = LTWMSEFModel.SimpleBackValue.True;
                            LTWMSEFModel.SimpleBackValue stockoutdetailtreaymatter = LTWMSEFModel.SimpleBackValue.True;
                            LTWMSEFModel.SimpleBackValue rtvbillstockout = LTWMSEFModel.SimpleBackValue.True;
                            if (_shelfcell != null && _shelfcell.guid != Guid.Empty)
                            {
                                string _depth1_traybarcode = _shelfcell.depth1_traybarcode;
                                //暂存至List 》》LED显示
                                var ListMatterDetail = bll_wh_tray.GetMatterDetailByTrayBarcode(_depth1_traybarcode);
                                if (ListMatterDetail != null && ListMatterDetail.Count > 0)
                                {
                                    string traybarcode = ListMatterDetail[0].traybarcode;
                                    var traymatterSet = TrayOutedSet.Where(w => w.traybarcode == traybarcode).FirstOrDefault();
                                    if (traymatterSet != null && traymatterSet.guid != Guid.Empty)
                                    {
                                        TrayOutedSet.Remove(traymatterSet);
                                    }
                                    TrayOutedSet.Add(ListMatterDetail[0]);
                                }
                                //出库自动解绑托盘与电池的关联关系？
                                // bll_wh_tray.DisConnectedALLMatter(_shelfcell.depth1_traybarcode);
                                //bll_wh_tray.DeleteTrayInfoAndMatterDetails(_shelfcell.depth1_traybarcode);
                                ////修改对应托盘状态
                                //var trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _shelfcell.depth1_traybarcode);
                                //if (trayM != null)
                                //{
                                //    trayM.shelfunits_guid = null;
                                //    trayM.shelfunits_pos = "";
                                //    trayM.status = TrayStatus.OffShelf;
                                //    bll_wh_tray.Update(trayM);
                                //}
                                //重置库位信息
                                _shelfcell.cellstate = ShelfCellState.CanIn;
                                if (taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                                {//如果是出库强制完成，加人工锁
                                    _shelfcell.locktype = ShelfLockType.ManLock;
                                }
                                else
                                {
                                    _shelfcell.locktype = ShelfLockType.Normal;
                                }
                                _shelfcell.depth1_traybarcode = "";
                                _shelfcell.depth2_traybarcode = "";
                                _shelfcell.updatedate = DateTime.Now;
                                _shelfcell.tray_indatetime = null;
                                _shelfcell.tray_outdatetime = DateTime.Now;
                                _shelfcell.special_lock_type = SpecialLockTypeEnum.Normal;
                                //修改数据
                                rtvshelfunits = bll_wh_shelfunits.Update(_shelfcell);
                                //出库完成修改对应出库单的出库数量 检测出库单是否全部出库完成 
                                /************************/
                                /************************/
                                // 修改对应的单据信息 
                                var reMd = bll_bill_task_tray_relation.GetAllQueryOrderby(o => o.createdate
                                , w => w.traybarcode == taskInfo.tray1_barcode, false).FirstOrDefault();
                                if (reMd != null && reMd.guid != Guid.Empty)
                                { //对托盘仅仅解除绑定库位关系
                                    rtvdisconnected = bll_wh_tray.DisConnectedALLMatter(_depth1_traybarcode);
                                }
                                else
                                {//对托盘对应的物料进行删除
                                    rtvdisconnected = bll_wh_tray.DeleteTrayInfoAndMatterDetails(_depth1_traybarcode);
                                }
                                //修改对应出库单据状态
                                if (taskInfo.bills_type == BillsTypeEnum.BillsOut && taskInfo.re_detail_traymatter_guid != null)
                                {//修改对应出库单的托盘出库状态
                                    var stockOutDetailTrayMM = bll_bill_stockout_detail_traymatter.GetFirstDefault(w => w.guid == taskInfo.re_detail_traymatter_guid);
                                    if (stockOutDetailTrayMM != null && stockOutDetailTrayMM.guid != Guid.Empty)
                                    {
                                        stockOutDetailTrayMM.tray_status = LTWMSEFModel.Bills.TrayOutStockStatusEnum.TrayOuted;
                                        stockOutDetailTrayMM.updatedate = DateTime.Now;
                                        stockOutDetailTrayMM.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>出库完成，自动修改为已出库。";
                                        stockoutdetailtreaymatter = bll_bill_stockout_detail_traymatter.Update(stockOutDetailTrayMM);
                                        //自动修改对应出库单据的状态
                                        var objStockOut = bll_bill_stockout.GetFirstDefault(w => w.guid == stockOutDetailTrayMM.stockout_guid);
                                        if (objStockOut != null && objStockOut.guid != Guid.Empty)
                                        {
                                            int _allcount = bll_bill_stockout_detail_traymatter.GetCount(w => w.stockout_guid == objStockOut.guid);
                                            int _outedcount = bll_bill_stockout_detail_traymatter.GetCount(w => w.stockout_guid == objStockOut.guid &&
                                             w.tray_status != LTWMSEFModel.Bills.TrayOutStockStatusEnum.None
                                             && w.tray_status != LTWMSEFModel.Bills.TrayOutStockStatusEnum.WaitOut
                                             );
                                            if (_allcount == _outedcount)
                                            {//已全部出库
                                                objStockOut.updatedate = DateTime.Now;
                                                objStockOut.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>出库完成";
                                                objStockOut.bill_status = LTWMSEFModel.Bills.BillsStatus_Out.Finished;
                                                objStockOut.get_status = LTWMSEFModel.Bills.GetStatus_Out.GetALL;
                                                rtvbillstockout = bll_bill_stockout.Update(objStockOut);
                                            }
                                            else
                                            {//部分出库
                                                if (objStockOut.bill_status == LTWMSEFModel.Bills.BillsStatus_Out.None)
                                                {
                                                    objStockOut.updatedate = DateTime.Now;
                                                    objStockOut.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>正在出库";
                                                    objStockOut.bill_status = LTWMSEFModel.Bills.BillsStatus_Out.Running;
                                                    objStockOut.get_status = LTWMSEFModel.Bills.GetStatus_Out.GetPart;
                                                    rtvbillstockout = bll_bill_stockout.Update(objStockOut);
                                                }
                                            }
                                        }
                                    }
                                }
                                /************************/
                                /************************/
                                /************************/
                                //  OnExecuteLog("出库任务完成 已修改对应的库位信息：[" + _shelfcell.shelfunits_pos + "] 可入库/正常状态", randDiff);
                            }

                            if (rtvshelfunits == LTWMSEFModel.SimpleBackValue.True && rtvdisconnected == LTWMSEFModel.SimpleBackValue.True &&
                                stockoutdetailtreaymatter == LTWMSEFModel.SimpleBackValue.True && rtvbillstockout == LTWMSEFModel.SimpleBackValue.True)
                            {
                                dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                            }
                            else
                            {
                                rtvMsg += "出库任务完成 修改数据失败》》";
                            }
                            //任务完成修改电池出库单为进行中。。。。
                            /* if (!string.IsNullOrWhiteSpace(taskInfo.order))
                             {
                                 //order字段只做电池订单标记，为空则表示不是电池出库任务
                                 var billStockOut = bll_bill_stockout.GetFirstDefault(w => w.bill_status == LTWMSEFModel.Bills.BillsStatus_Out.None
                                   && w.odd_numbers_in == taskInfo.order);
                                 if (billStockOut != null && billStockOut.guid != Guid.Empty)
                                 {
                                     billStockOut.bill_status = LTWMSEFModel.Bills.BillsStatus_Out.Running;
                                     bll_bill_stockout.Update(billStockOut);
                                 }
                             }*/
                        }
                        else if (taskInfo.taskstatus == WcsTaskStatus.Canceled)
                        {//出库任务取消--发送一条警告信息 
                         //任务取消，修改库位状态为，可出库
                         //wh_shelfunits _shelfCell = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid);
                         //string _new_task_guid = "";
                         //if (taskInfo.regenerate_task_queue == true)
                         //{//重新生成出库任务
                         //    _shelfCell.cellstate = ShelfCellState.WaitOut;
                         //    _new_task_guid = bll_hdw_stacker_taskqueue.ReGenerateTask(taskInfo);
                         //}
                         //else
                         //{//不重新生成出库任务
                         //    _shelfCell.cellstate = ShelfCellState.Stored;
                         //}
                         //_shelfCell.updatedate = DateTime.Now;
                         //bll_wh_shelfunits.Update(_shelfCell);
                            if (taskInfo.src_shelfunits_guid != null)
                            {
                                wh_shelfunits _shelfCell = bll_wh_shelfunits.StockOutCanceledHandler(taskInfo);
                                if (_shelfCell != null && _shelfCell.guid != Guid.Empty)
                                {
                                    log_sys_alarm _alarm = new log_sys_alarm();
                                    _alarm.warehouse_guid = taskInfo.warehouse_guid;
                                    _alarm.guid = Guid.NewGuid();
                                    _alarm.log_date = DateTime.Now;
                                    _alarm.log_from = AlarmFrom.Stacker;
                                    _alarm.remark = "出库任务[" + taskInfo.id + "]已取消,对应库位（" +
                                        taskInfo.src_rack + "排" + taskInfo.src_col + "列" + taskInfo.src_row + "层）";
                                    var rtvlogadd = bll_sys_alarm_log.Add(_alarm);
                                    if (_shelfCell.cellstate == ShelfCellState.WaitOut)
                                    {
                                        OnExecuteLog("出库任务取消 重新生成出库任务guid[" + Convert.ToString(taskInfo.new_task_queue_guid)
                                            + "] 已修改对应的库位信息：[" + _shelfCell.shelfunits_pos + "] 等待出库...", randDiff);
                                    }
                                    else
                                    {
                                        OnExecuteLog("出库任务取消 已修改对应的库位信息：[" + _shelfCell.shelfunits_pos + "] 存储状态", randDiff);
                                    }

                                    if (rtvlogadd == LTWMSEFModel.SimpleBackValue.True)
                                    {
                                        dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                                    }
                                }
                            }

                        }
                        else if (taskInfo.taskstatus == WcsTaskStatus.Exception)
                        {
                            //发送一条警告信息
                            log_sys_alarm _alarm = new log_sys_alarm();
                            _alarm.warehouse_guid = taskInfo.warehouse_guid;
                            _alarm.guid = Guid.NewGuid();
                            _alarm.log_date = DateTime.Now;
                            _alarm.log_from = AlarmFrom.Stacker;
                            _alarm.remark = "来自WCS的出库任务异常：目标库位（" +
                                taskInfo.dest_rack + "排" + taskInfo.dest_col + "列" + taskInfo.dest_row + "层）";
                            dealsuccess = bll_sys_alarm_log.Add(_alarm);
                        }
                        else
                        {//其它状态，默认true
                            dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                        }
                        break;
                    case WcsTaskType.MoveTo:
                        //移库
                        if (taskInfo.taskstatus == WcsTaskStatus.Finished || taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                        {//移库完成
                            SpecialLockTypeEnum srcSpecLock = SpecialLockTypeEnum.Normal;
                            wh_shelfunits _shelfU_start = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid);
                            LTWMSEFModel.SimpleBackValue rtvwhtraydis = LTWMSEFModel.SimpleBackValue.False;
                            LTWMSEFModel.SimpleBackValue rtvshelfunits_start = LTWMSEFModel.SimpleBackValue.False;
                            if (_shelfU_start != null && _shelfU_start.guid != Guid.Empty)
                            {//移库起点库位清空
                             //出库自动解绑托盘与电池的关联关系？
                                rtvwhtraydis = bll_wh_tray.DisConnectedALLMatter(_shelfU_start.depth1_traybarcode);
                                //   bll_wh_tray.DeleteTrayInfoAndMatterDetails(_shelfU_start.depth1_traybarcode);
                                srcSpecLock = _shelfU_start.special_lock_type;
                                //重置库位信息
                                _shelfU_start.cellstate = ShelfCellState.CanIn;
                                if (taskInfo.taskstatus == WcsTaskStatus.ForceComplete)
                                {//如果是出库强制完成，加人工锁
                                    _shelfU_start.locktype = ShelfLockType.ManLock;
                                }
                                else
                                {
                                    _shelfU_start.locktype = ShelfLockType.Normal;
                                }
                                _shelfU_start.depth1_traybarcode = "";
                                _shelfU_start.depth2_traybarcode = "";
                                _shelfU_start.updatedate = DateTime.Now;
                                _shelfU_start.tray_indatetime = null;
                                _shelfU_start.tray_outdatetime = DateTime.Now;
                                _shelfU_start.special_lock_type = SpecialLockTypeEnum.Normal;
                                //修改数据
                                rtvshelfunits_start = bll_wh_shelfunits.Update(_shelfU_start);
                            }
                            wh_shelfunits _shelfU_dest = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.dest_shelfunits_guid);
                            //移库终点 库位入库操作
                            LTWMSEFModel.SimpleBackValue rtvshelfunits_dest = LTWMSEFModel.SimpleBackValue.False;
                            LTWMSEFModel.SimpleBackValue rtvwhtrayupdate = LTWMSEFModel.SimpleBackValue.True;
                            if (_shelfU_dest != null && _shelfU_dest.guid != Guid.Empty)
                            {
                                _shelfU_dest.updatedate = DateTime.Now;
                                _shelfU_dest.depth1_traybarcode = taskInfo.tray1_barcode;
                                _shelfU_dest.tray_indatetime = DateTime.Now;
                                _shelfU_dest.cellstate = ShelfCellState.Stored;
                                _shelfU_dest.locktype = ShelfLockType.SysLock;//入库完成，防止获取库位时修改失败 
                                _shelfU_dest.special_lock_type = srcSpecLock;/// SpecialLockTypeEnum.Normal;
                                //修改对应托盘状态
                                var trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == taskInfo.tray1_barcode);
                                if (trayM != null && trayM.guid != Guid.Empty)
                                {
                                    trayM.shelfunits_guid = _shelfU_dest.guid;
                                    trayM.shelfunits_pos = _shelfU_dest.shelfunits_pos;
                                    trayM.status = TrayStatus.OnShelf;
                                    trayM.warehouse_guid = _shelfU_dest.warehouse_guid;
                                    rtvwhtrayupdate = bll_wh_tray.Update(trayM);
                                }
                                //修改数据
                                rtvshelfunits_dest = bll_wh_shelfunits.Update(_shelfU_dest);
                            }

                            if (rtvwhtraydis == LTWMSEFModel.SimpleBackValue.True && rtvshelfunits_start == LTWMSEFModel.SimpleBackValue.True &&
                                rtvshelfunits_dest == LTWMSEFModel.SimpleBackValue.True && rtvwhtrayupdate == LTWMSEFModel.SimpleBackValue.True)
                            {
                                dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                            }
                        }
                        else if (taskInfo.taskstatus == WcsTaskStatus.Canceled)
                        {
                            LTWMSEFModel.SimpleBackValue rtvalarmlogadd = LTWMSEFModel.SimpleBackValue.False;
                            wh_shelfunits _shelfU_start = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid);
                            if (_shelfU_start != null && _shelfU_start.guid != Guid.Empty)
                            {//移库取消操作。。。
                                wh_shelfunits _shelfCell = bll_wh_shelfunits.StockMoveOutCanceledHandler(taskInfo);
                                if (_shelfCell != null)
                                {
                                    log_sys_alarm _alarm = new log_sys_alarm();
                                    _alarm.warehouse_guid = taskInfo.warehouse_guid;
                                    _alarm.guid = Guid.NewGuid();
                                    _alarm.log_date = DateTime.Now;
                                    _alarm.log_from = AlarmFrom.Stacker;
                                    _alarm.remark = "移库任务[" + taskInfo.id + "]已取消,对应库位（" +
                                        taskInfo.src_rack + "排" + taskInfo.src_col + "列" + taskInfo.src_row + "层 => "
                                        + taskInfo.dest_rack + "排" + taskInfo.dest_col + "列" + taskInfo.dest_row + "层）";
                                    rtvalarmlogadd = bll_sys_alarm_log.Add(_alarm);
                                    if (_shelfCell.cellstate == ShelfCellState.WaitOut)
                                    {
                                        OnExecuteLog("移库任务取消 重新生成移库任务guid[" + Convert.ToString(taskInfo.new_task_queue_guid)
                                            + "] 已修改对应的库位信息：[" + _shelfCell.shelfunits_pos + "] 等待移库...", randDiff);
                                    }
                                    else
                                    {
                                        OnExecuteLog("移库任务取消 已修改对应的库位信息：[" + _shelfCell.shelfunits_pos + "] 存储状态", randDiff);
                                    }
                                }
                            }
                            LTWMSEFModel.SimpleBackValue rtvshelfunits = LTWMSEFModel.SimpleBackValue.False;
                            wh_shelfunits _shelfU_dest = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.dest_shelfunits_guid);
                            //移库终点 库位入库操作
                            if (_shelfU_dest != null && _shelfU_dest.guid != Guid.Empty)
                            {
                                _shelfU_dest.cellstate = ShelfCellState.CanIn;
                                _shelfU_dest.updatedate = DateTime.Now;
                                //取消入库加人工锁，防止人为操作失误，实际托盘已入库位（人工锁的库位可以强制出库）
                                _shelfU_dest.locktype = ShelfLockType.ManLock;
                                rtvshelfunits = bll_wh_shelfunits.Update(_shelfU_dest);
                            }
                            if (rtvalarmlogadd == LTWMSEFModel.SimpleBackValue.True && rtvshelfunits == LTWMSEFModel.SimpleBackValue.True)
                            {
                                dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                            }
                        }
                        else if (taskInfo.taskstatus == WcsTaskStatus.Exception)
                        {
                            LTWMSEFModel.SimpleBackValue rtvwhtraydisc = LTWMSEFModel.SimpleBackValue.False;
                            LTWMSEFModel.SimpleBackValue rtvshelfunits_start = LTWMSEFModel.SimpleBackValue.False;
                            wh_shelfunits _shelfU_start = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.src_shelfunits_guid);

                            if (_shelfU_start != null && _shelfU_start.guid != Guid.Empty)
                            {//移库起点库位清空

                                //出库自动解绑托盘与电池的关联关系？
                                rtvwhtraydisc = bll_wh_tray.DisConnectedALLMatter(_shelfU_start.depth1_traybarcode);
                                //   bll_wh_tray.DeleteTrayInfoAndMatterDetails(_shelfU_start.depth1_traybarcode);
                                //重置库位信息
                                _shelfU_start.cellstate = ShelfCellState.CanIn;
                                //加人工锁
                                _shelfU_start.locktype = ShelfLockType.ManLock;

                                _shelfU_start.depth1_traybarcode = "";
                                _shelfU_start.depth2_traybarcode = "";
                                _shelfU_start.updatedate = DateTime.Now;
                                _shelfU_start.tray_indatetime = null;
                                _shelfU_start.tray_outdatetime = DateTime.Now;
                                //修改数据
                                rtvshelfunits_start = bll_wh_shelfunits.Update(_shelfU_start);
                            }
                            LTWMSEFModel.SimpleBackValue rtvshelfunit_dest = LTWMSEFModel.SimpleBackValue.False;
                            wh_shelfunits _shelfU_dest = bll_wh_shelfunits.GetFirstDefault(w => w.guid == taskInfo.dest_shelfunits_guid);
                            //移库终点 库位入库操作
                            if (_shelfU_dest != null && _shelfU_dest.guid != Guid.Empty)
                            {
                                _shelfU_dest.cellstate = ShelfCellState.CanIn;
                                _shelfU_dest.updatedate = DateTime.Now;
                                //取消入库加人工锁，防止人为操作失误，实际托盘已入库位（人工锁的库位可以强制出库）
                                _shelfU_dest.locktype = ShelfLockType.ManLock;
                                rtvshelfunit_dest = bll_wh_shelfunits.Update(_shelfU_dest);
                            }
                            //修改托盘绑定的库位信息

                            //发送一条警告信息
                            log_sys_alarm _alarm = new log_sys_alarm();
                            _alarm.warehouse_guid = taskInfo.warehouse_guid;
                            _alarm.guid = Guid.NewGuid();
                            _alarm.log_date = DateTime.Now;
                            _alarm.log_from = AlarmFrom.Stacker;
                            _alarm.remark = "来自WCS的移库任务异常：锁定起点库位（" + taskInfo.src_rack + "排" + taskInfo.src_col
                                + "列" + taskInfo.src_row + "层）和目标库位（" +
                                taskInfo.dest_rack + "排" + taskInfo.dest_col + "列" + taskInfo.dest_row + "层）";
                            var rtvalarmlogadd = bll_sys_alarm_log.Add(_alarm);

                            if (rtvwhtraydisc == LTWMSEFModel.SimpleBackValue.True && rtvshelfunits_start == LTWMSEFModel.SimpleBackValue.True &&
                                rtvshelfunit_dest == LTWMSEFModel.SimpleBackValue.True && rtvalarmlogadd == LTWMSEFModel.SimpleBackValue.True)
                            {
                                dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                            }
                        }
                        else
                        {//其它状态，默认true
                            dealsuccess = LTWMSEFModel.SimpleBackValue.True;
                        }
                        break;
                }
                //修改任务
                rtvtaskqueue = bll_hdw_stacker_taskqueue.Update(taskInfo);
                // OnExecuteLog("已成功修改任务状态 任务id=" + receiveTaskStatus.task_id + " /状态：" + (int)taskInfo.taskstatus, randDiff);
            }
            else
            {
                rtvMsg = "解析任务完成状态失败 详细==>>" + Newtonsoft.Json.JsonConvert.SerializeObject(receiveTaskStatus);
                // OnExecuteLog("解析任务完成状态失败 详细==>>" + Newtonsoft.Json.JsonConvert.SerializeObject(receiveTaskStatus), randDiff);
            }
            if (rtvtaskqueue == LTWMSEFModel.SimpleBackValue.True && dealsuccess == LTWMSEFModel.SimpleBackValue.True)
            {
                return JsonReturn(true);
            }
            return JsonReturn(false, rtvMsg);

        }
        public void ReceivePLCStatus(ReceiveMachineStatus machineState, Guid wcssrvguid)
        {
            if (machineState != null && machineState.dev_info != null)
            {
                //堆垛机运行状态+输送线1、2、3、4 允许出库状态
                var _list_Stacker = bll_hdw_plc.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.shvwcs_srv_guid == wcssrvguid);
                // && w.type == PLCType.Stacker 
                foreach (var item in machineState.dev_info)
                {
                    bool addnew = false;
                    hdw_plc _plc = new hdw_plc();
                    if (_list_Stacker != null && _list_Stacker.Count > 0)
                    {//数据库有数据判断
                        _plc = _list_Stacker.Where(w => w.number == item.dev_id).FirstOrDefault();
                        if (_plc == null)
                        {
                            //为空则添加
                            addnew = true;
                        }
                    }
                    else
                    {//数据库无数据，直接添加
                        addnew = true;
                    }
                    if (addnew)
                    {
                        var devObj = bll_wh_wcs_device.GetFirstDefault(w => w.wcs_srv_guid == wcssrvguid && w.number == item.dev_id);
                        if (devObj != null && devObj.guid != Guid.Empty)
                        {
                            //添加设备信息至数据库
                            hdw_plc objplc = new hdw_plc();
                            objplc.guid = Guid.NewGuid();
                            objplc.createdate = DateTime.Now;
                            objplc.state = LTWMSEFModel.EntityStatus.Normal;
                            objplc.number = item.dev_id;
                            objplc.shvwcs_srv_guid = wcssrvguid;
                            objplc.u_identification = objplc.shvwcs_srv_guid + "-" + objplc.number;
                            objplc.warehouse_guid = devObj.warehouse_guid;
                            objplc.type = devObj.device_type;
                            objplc.run_status = GetStackerStatus(item.status);
                            bll_hdw_plc.Add(objplc);
                        }
                    }
                    else
                    {
                        //修改
                        //保存堆垛机的状态信息至数据库
                        _plc.run_status = GetStackerStatus(item.status);
                        _plc.updatedate = DateTime.Now;
                        bll_hdw_plc.Update(_plc);
                    }
                }
            }
        }
        public PLCRunStatus GetStackerStatus(int status)
        {
            switch (status)
            {
                case -1:
                    //与PLC断开连接
                    return PLCRunStatus.DisConnected;
                case 1:
                    //正常
                    return PLCRunStatus.Ready;
                case 2:
                    //中断运行
                    return PLCRunStatus.Running;
                case 3:
                    return PLCRunStatus.Warning;
                case 0:
                default:
                    //未启动
                    return PLCRunStatus.None;
            }
        }
    }
}
