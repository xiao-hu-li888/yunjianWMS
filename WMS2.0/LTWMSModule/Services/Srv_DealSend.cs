using LTWMSEFModel.Hardware;
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
    public class Srv_DealSend : BaseService
    {
        hdw_plcBLL bll_hdw_plc;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        hdw_stacker_taskqueue_hisBLL bll_hdw_stacker_taskqueue_his;
        // LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        wh_shelfunitsBLL bll_wh_shelfunits;
        wh_service_statusBLL bll_wh_service_status;
        LTWMSService.ApplicationService.WmsServer.WCSService bll_wcsservice;
        wh_trayBLL bll_wh_tray;
        sys_table_idBLL bll_sys_table_id;
        log_sys_alarmBLL bll_sys_alarm_log;
        LTWMSService.Bills.bill_stockoutBLL bll_bill_stockout;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        wh_shelvesBLL bll_wh_shelves;
        StockInService srv_StockInService;
        stk_matterBLL bll_stk_matter;
        wh_traymatterBLL bll_wh_traymatter;


        hdw_message_waitedsendBLL bll_hdw_message_waitedsend;

        // wh_service_statusBLL bll_wh_service_status;
        public Srv_DealSend(Guid Wcs_srv_guid, string Wcs_srv_Name, string wcs_srv_ip, int wcs_srv_port) : base(Wcs_srv_guid, Wcs_srv_Name, wcs_srv_ip, wcs_srv_port)
        {
            CreateBLL(GetDbModel());
        }
        public void CreateBLL(LTWMSEFModel.LTModel dbmodel)
        {
            bll_hdw_message_waitedsend = new hdw_message_waitedsendBLL(dbmodel);
            // bll_wh_service_status = new wh_service_statusBLL(dbmodel);
            // bll_hdw_plc = new hdw_plcBLL(dbmodel);
            //   bll_sys_control_dic = new LTWMSService.Basic.sys_control_dicBLL(dbmodel);
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
            bll_stk_matter = new stk_matterBLL(dbmodel);
            bll_wh_wcs_device = new wh_wcs_deviceBLL(dbmodel);
            bll_wh_traymatter = new wh_traymatterBLL(dbmodel);


            bll_wcsservice = new LTWMSService.ApplicationService.WmsServer.WCSService(dbmodel, bll_wh_tray, bll_wh_shelfunits,
              bll_hdw_stacker_taskqueue, bll_sys_control_dic, bll_sys_alarm_log, bll_sys_table_id, bll_hdw_plc, bll_bill_stockout, bll_wh_wcs_device
              , bll_wh_shelves);
            bll_wcsservice.SetLedObj(ledDisplay);
            srv_StockInService = new StockInService(dbmodel, bll_sys_control_dic, bll_stk_matter, bll_wh_traymatter, bll_wh_tray);
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
                w.wcstype == WCSType.SRV_DealSend);
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
                        wcstype = WCSType.SRV_DealSend,
                        wcs_status = status
                    };
                    bll_wh_service_status.Add(_whWcs);
                }
            }
        }
        public List<wh_warehouse> GetAllWareHouseByWcsSrvGuid(Guid Wcs_srv_guid)
        {
            return bll_wh_shelves.getAllWareHouseByWcsSrvGuid(Wcs_srv_guid);
        }
        /// <summary>
        /// 处理生成待发送数据
        /// </summary>
        /// <param name="warehouse"></param>
        public void DealSendHandler(wh_warehouse warehouse)
        {
            var _whWcs = bll_wh_service_status.GetFirstDefault(w => w.wcs_srv_guid == Wcs_srv_guid &&
                w.wcstype == LTWMSEFModel.Warehouse.WCSType.WCSServer);
            if (_whWcs != null && _whWcs.guid != Guid.Empty && _whWcs.wcs_status == WcsStatus.Connected)
            {
                ///检测任务表，发送出入库任务
                CheckOverTaskQueue(warehouse);
                // 检查入库未分配库位，分配库位
                CheckWaiteDispatchStockCell(warehouse);
                //防止并发导致未解锁库位，查询并解锁syslock
                CheckSysLockShelfUnit_Free(warehouse);
                //检查强制完成、取消任务。。。
                CheckForeceCancelTaskHandler(warehouse);
            }
        }



        /// <summary>
        /// 堆垛机对应的货架
        /// </summary>
        System.Collections.Hashtable tableStackerShelves = new System.Collections.Hashtable();
        /// <summary>
        /// 堆垛机对应的站台
        /// </summary>
        System.Collections.Hashtable tableStationOfStacker = new System.Collections.Hashtable();
        /// <summary>
        /// 保留所有堆垛机上一次执行的对应任务信息
        /// </summary>
        // private hdw_stacker_taskqueue lastTaskinfo;
        System.Collections.Hashtable LastTableOfTaskQueue = new System.Collections.Hashtable();

        /// <summary>
        /// 检测任务表，发送出入库任务
        /// </summary> 
        public void CheckOverTaskQueue(wh_warehouse warehouse)
        {
            //判断是否发送任务至所有堆垛机
            string _issendtoAllstacker = bll_sys_control_dic.GetValueByType(CommDictType.SendTaskToAllStackers, Guid.Empty);
            if (_issendtoAllstacker != "1")
            {//如果设置为不下发任务，则不发送任务至堆垛机，直接返回 
             //  WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>不发送任务至所有堆垛机，直接返回 ...");
                return;
            }
            //判断是否发送任务至堆垛机
            string _issendtostacker = bll_sys_control_dic.GetValueByType(CommDictType.SendToStacker, warehouse.guid);
            if (_issendtostacker != "1")
            {//如果设置为不下发任务，则不发送任务至堆垛机，直接返回 
             //   WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>不发送任务至堆垛机，直接返回");
                return;
            }
            if (!bll_hdw_stacker_taskqueue.GetAny(w => w.taskstatus == WcsTaskStatus.Holding && w.warehouse_guid == warehouse.guid))
            {//检测是否有任务，没有直接返回
             //没有任务线程休眠5秒 
             // WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>检测没有待发送任务直接返回");
                return;
            }
            int randDiff = new Random().Next(1, int.MaxValue);
            // WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>循环检测出库任务...");
            // 其它物料可以从1、2包括入库口3、4出？？？ 
            List<hdw_plc> lstPLC = bll_hdw_plc.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.warehouse_guid == warehouse.guid
            && w.shvwcs_srv_guid == Wcs_srv_guid);
            if (lstPLC == null || lstPLC.Count == 0)
            {//plc、输送线等设备状态为空
             //  WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>plc、输送线等设备状态为空");
                return;
            }
            //循环wcs控制的所有堆垛机
            var StackerList = lstPLC.Where(w => w.type == DeviceTypeEnum.Stacker).ToList();
            foreach (hdw_plc _stacker in StackerList)
            {
                //**********************第一排********************** 
                //// 0= 未启动  1 = 准备状态	2=运行  3 堆垛机警告 
                if (_stacker.run_status != PLCRunStatus.Ready)
                {//堆垛机未准备好，直接返回 
                 // WinServiceFactory.Log.v("【" + Wcs_srv_Name + "】>>>堆垛机[" + _stacker.number + "]未准备好，继续下一个堆垛机任务分配");
                    continue;
                }
                //通过堆垛机查找对应的处理货架信息
                if (!tableStackerShelves.ContainsKey(_stacker.u_identification))
                {
                    var lstPlcObj = new List<hdw_plc>();
                    lstPlcObj.Add(_stacker);
                    List<wh_shelves> lstShelves = bll_wh_shelves.GetShelvesByStacker(lstPlcObj);
                    tableStackerShelves.Add(_stacker.u_identification, lstShelves);
                }
                List<wh_shelves> whshelvesList = tableStackerShelves[_stacker.u_identification] as List<wh_shelves>;
                //单机两头各2个出入库口
                int runningCount = bll_hdw_stacker_taskqueue.GetRunningTaskInout(_stacker, whshelvesList);
                if (runningCount > 0)
                {//有执行中的任务，直接返回
                    //return;
                    continue;
                }
                //通过堆垛机查找对应的站台
                if (!tableStationOfStacker.ContainsKey(_stacker.u_identification))
                {
                    //后期改成堆垛机绑定站台（前期暂时保持货架对应站台。。。）
                    List<wh_wcs_device> listStation = bll_wh_wcs_device.GetALLStationByStacker(whshelvesList);
                    tableStationOfStacker.Add(_stacker.u_identification, listStation);
                }
                //根据堆垛机对应的站台，匹配出出库准备好状态
                List<wh_wcs_device> stationList = tableStationOfStacker[_stacker.u_identification] as List<wh_wcs_device>;
                if (stationList != null && stationList.Count > 0)
                {
                    foreach (var item in stationList)
                    {
                        if (item.station_mode == StationModeEnum.InOnly)
                        {//只入模式，不能出库 
                            item.run_status = PLCRunStatus.None;
                        }
                        else
                        {
                            var obj = lstPLC.Where(w => w.u_identification == item.u_identification).FirstOrDefault();
                            if (obj != null && obj.guid != Guid.Empty)
                            {
                                item.run_status = obj.run_status;
                            }
                            else
                            {//未找到默认为none
                                item.run_status = PLCRunStatus.None;
                            }
                        }
                    }
                }
                #region 出入库逻辑（效率）
                hdw_stacker_taskqueue WaiteSendTask = null;
                //查找优先级最高的出库任务（order=999）
                //hdw_stacker_taskqueue PriorityTask = bll_hdw_stacker_taskqueue.GetPriorityTaskInfo(Transport1, Transport2, Transport3, Transport4,
                //    bll_wcsservice.transportNumber1, bll_wcsservice.transportNumber2, bll_wcsservice.transportNumber3, bll_wcsservice.transportNumber4);
                hdw_stacker_taskqueue PriorityTask = bll_hdw_stacker_taskqueue.GetPriorityTaskInfo(stationList, whshelvesList);
                if (PriorityTask != null && PriorityTask.guid != Guid.Empty)
                {//查找最大优先级的任务，优先执行
                    WaiteSendTask = PriorityTask;
                }
                else
                { //按照入库优先>100>0                    
                    List<hdw_stacker_taskqueue> TaskInList = bll_hdw_stacker_taskqueue.GetALLWaitedTaskIn(whshelvesList);
                    if (TaskInList != null && TaskInList.Count > 0)
                    {//有入库
                     //取第一个入库任务。。。。
                        hdw_stacker_taskqueue taskIn = TaskInList[0];
                        hdw_stacker_taskqueue lastTaskinfo = null;
                        if (LastTableOfTaskQueue.ContainsKey(_stacker.u_identification))
                        {
                            lastTaskinfo = LastTableOfTaskQueue[_stacker.u_identification] as hdw_stacker_taskqueue;
                        }
                        if (lastTaskinfo == null || lastTaskinfo.tasktype == WcsTaskType.StockIn)
                        { //上一个任务为空或者是入库 （堆垛机位置在中间）
                            WaiteSendTask = GetTaskInOrOppositeOut(taskIn, stationList, whshelvesList);
                        }
                        else
                        {//出库（堆垛机位置在两头，两头对立面可以带入库或另一头入库可以带出库） 
                            if (stationList != null && stationList.Count > 0)
                            {// 判断上次出库对立面是否有入库任务
                                wh_wcs_device wcsStation = null;
                                //  var lstOrderStation = stationList.OrderBy(o => o.number).ToList();
                                if (lastTaskinfo.dest_station % 2 == 0)
                                {//入库站台取模=0 ，则对立面-1
                                    wcsStation = stationList.Where(w => w.number < lastTaskinfo.dest_station).OrderByDescending(o => o.number).FirstOrDefault();
                                }
                                else
                                {//如果取模>0, 则对立面+1
                                    wcsStation = stationList.Where(w => w.number > lastTaskinfo.dest_station).OrderBy(o => o.number).FirstOrDefault();
                                }
                                //判断上一次出库任务，对立面是否有入库任务
                                hdw_stacker_taskqueue taskOppositeIn = bll_hdw_stacker_taskqueue.GetOppositeTaskIn(wcsStation, whshelvesList);
                                if (taskOppositeIn != null && taskOppositeIn.guid != Guid.Empty)
                                {// 有 =》顺带入库
                                    WaiteSendTask = taskOppositeIn;
                                }
                            }
                            if (WaiteSendTask == null || WaiteSendTask.guid == Guid.Empty)
                            {// 没有 =》
                             //  =》判断自身是否有入库
                                var selfStation = stationList.Where(w => w.number == lastTaskinfo.dest_station).FirstOrDefault();
                                hdw_stacker_taskqueue taskOppositeIn2 = bll_hdw_stacker_taskqueue.GetOppositeTaskIn(selfStation, whshelvesList);
                                if (taskOppositeIn2 != null && taskOppositeIn2.guid != Guid.Empty)
                                {// 有 =》顺带入库  
                                    WaiteSendTask = taskOppositeIn2;
                                }
                                else
                                {
                                    //没有>>> 优先级入库或入库对立面出库
                                    WaiteSendTask = GetTaskInOrOppositeOut(taskIn, stationList, whshelvesList);
                                }
                            }
                        }
                    }
                    else
                    {//没有入库
                     // 按出库优先级出库
                     //出库任务 判断对应是否允许出库。。。只出允许出库的任务！！！ 
                        WaiteSendTask = bll_hdw_stacker_taskqueue.GetTaskDefaultTaskOut(stationList, whshelvesList);
                    }
                }
                if (WaiteSendTask != null && WaiteSendTask.guid != Guid.Empty)
                {// 下发任务。。。     
                    hdw_stacker_taskqueue waitesendtaskReal = null;//实际发送的任务（可能是出库、入库、移库）
                    wh_shelfunits _shelfU_inoutmov = null;
                    if (WaiteSendTask.tasktype == WcsTaskType.StockIn)
                    {//入库 
                        _shelfU_inoutmov = bll_wh_shelfunits.GetFirstDefault(w => w.guid == WaiteSendTask.dest_shelfunits_guid);
                    }
                    else if (WaiteSendTask.tasktype == WcsTaskType.StockOut)
                    {//出库 
                        _shelfU_inoutmov = bll_wh_shelfunits.GetFirstDefault(w => w.guid == WaiteSendTask.src_shelfunits_guid);
                    }
                    //else  先不考虑移库，移库直接发指令。。。
                    // 移库 需要考虑 起点干涉（干涉先移库）  和 终点干涉 （干涉先移库）  ，起点和终点都干涉 先移库，比较复杂。。。
                    //{//移库
                    //    DbExecuteLog("【" + Wcs_srv_Name + "】已下发移库任务至堆垛机，任务id[" + WaiteSendTask.id + "]", randDiff);
                    //}
                    var _exist_shelfunit_array = bll_wh_shelfunits.getAllBlocksShelfUnitOrderByDepth(_shelfU_inoutmov);
                    if (_exist_shelfunit_array != null && _exist_shelfunit_array.Count > 0)
                    {//入库存在阻碍，先移库，单排多列大于2列可能需要多次移库，每次移完库会再检测一遍
                     //发送移库指令 
                     //待移库库位可能存在多线程并发操作冲突，需要处理。。
                        CheckAddMoveReturnType checkMove = null;
                        bool commit = false;
                        using (var tran = bll_hdw_stacker_taskqueue.BeginTransaction())
                        {//防止并发冲突（并发占用入库库位），添加移库任务时增加事务处理
                            try
                            {
                                checkMove = bll_hdw_stacker_taskqueue.CheckAddTaskMoveByShelfUnit(_exist_shelfunit_array[0], whshelvesList);
                                if (checkMove.checkResult == CheckBlockResultEnum.Blocked)
                                {
                                    if (checkMove.stacker_queue != null && checkMove.stacker_queue.guid != Guid.Empty)
                                    {//事务提交
                                     // _tran.Commit();
                                        commit = true;
                                    }
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
                        if (checkMove.checkResult == CheckBlockResultEnum.CanInOut)
                        {
                            waitesendtaskReal = WaiteSendTask;
                        }
                        else
                        {
                            if (checkMove.stacker_queue != null && checkMove.stacker_queue.guid != Guid.Empty)
                            {//可以是移库也可以是出库任务
                                waitesendtaskReal = checkMove.stacker_queue;
                                if (waitesendtaskReal.tasktype == WcsTaskType.StockOut)
                                {//出库分配出库站台=待出库站台
                                    //暂时只用待发送任务的目标站台（大概率只有出库任务存在干涉库位移库和出库操作）
                                    //，如果是入库则值为0（入库会过滤存在干涉的库位），基本不存在入库的可能性。
                                    //后期再优化按出库站台的准备状态出库且WaiteSendTask=入库的时候，出库站台不能等于入库站台
                                    //。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。
                                    waitesendtaskReal.dest_station = WaiteSendTask.dest_station;
                                }
                            }
                        }
                    }
                    else
                    {
                        waitesendtaskReal = WaiteSendTask;
                    }

                    if (waitesendtaskReal != null && waitesendtaskReal.guid != Guid.Empty)
                    {
                        SendTaskQueueCMD(_stacker.u_identification, waitesendtaskReal, whshelvesList);
                        if (waitesendtaskReal.tasktype == WcsTaskType.StockIn)
                        {
                            string txt = "物料即将入库至" + waitesendtaskReal.dest_shelfunits_pos;
                            // ledDisplay.LED3_Say(txt);
                            DbExecuteLog("【" + Wcs_srv_Name + "】已下发入库任务至堆垛机，任务id[" + waitesendtaskReal.id + "]", randDiff);
                        }
                        else if (waitesendtaskReal.tasktype == WcsTaskType.StockOut)
                        {
                            if (WaiteSendTask.guid == waitesendtaskReal.guid)
                            {
                                string txt = "物料即将从" + waitesendtaskReal.src_shelfunits_pos + "出库";
                                //   ledDisplay.LED4_Say(txt);
                                DbExecuteLog("【" + Wcs_srv_Name + "】已下发出库任务至堆垛机，任务id[" + waitesendtaskReal.id + "]", randDiff);
                            }
                            else
                            {
                                if (WaiteSendTask.tasktype == WcsTaskType.StockIn)
                                {
                                    string txt = "入库存在阻挡，优先执行出库";
                                    //  ledDisplay.LED3_Say(txt);
                                    string txt2 = "物料即将从" + waitesendtaskReal.src_shelfunits_pos + "出库";
                                    //   ledDisplay.LED4_Say(txt2);
                                    DbExecuteLog("【" + Wcs_srv_Name + "】任务id[" + WaiteSendTask.id + "][" + WaiteSendTask.dest_shelfunits_pos + "]入库任务存在阻挡，优先执行出库任务[" +
                           waitesendtaskReal.src_shelfunits_pos + "]=>[" + waitesendtaskReal.dest_station + "]，任务id["
                           + waitesendtaskReal.id + "]", randDiff);
                                }
                                else
                                {
                                    string txt = "物料即将从" + waitesendtaskReal.src_shelfunits_pos + "出库";
                                    //     ledDisplay.LED4_Say(txt);
                                    DbExecuteLog("【" + Wcs_srv_Name + "】任务id[" + WaiteSendTask.id + "][" + WaiteSendTask.src_shelfunits_pos + "]出库任务存在阻挡，优先执行出库任务[" +
                          waitesendtaskReal.src_shelfunits_pos + "]=>[" + waitesendtaskReal.dest_station + "]，任务id["
                          + waitesendtaskReal.id + "]", randDiff);
                                }

                            }
                        }
                        else
                        {//移库 //执行移库(可以是新生成的也可以是原有的移库任务)
                            if (WaiteSendTask.tasktype == WcsTaskType.StockIn)
                            {
                                string txt = "入库存在阻挡，优先执行移库";
                                //  ledDisplay.LED3_Say(txt);
                                string txt2 = "物料即将移库至" + waitesendtaskReal.dest_shelfunits_pos;
                                //   ledDisplay.LED4_Say(txt2);
                                DbExecuteLog("【" + Wcs_srv_Name + "】任务id[" + WaiteSendTask.id + "][" + WaiteSendTask.dest_shelfunits_pos + "]入库任务存在阻挡，生成移库任务[" +
                                        waitesendtaskReal.src_shelfunits_pos + "]=>[" + waitesendtaskReal.dest_shelfunits_pos + "]，任务id["
                                        + waitesendtaskReal.id + "]", randDiff);
                            }
                            else
                            {
                                string txt = "物料即将移库至" + waitesendtaskReal.dest_shelfunits_pos;
                                //  ledDisplay.LED4_Say(txt);
                                DbExecuteLog("【" + Wcs_srv_Name + "】任务id[" + WaiteSendTask.id + "][" + WaiteSendTask.src_shelfunits_pos + "]出库任务存在阻挡，生成移库任务[" +
                                    waitesendtaskReal.src_shelfunits_pos + "]=>[" + waitesendtaskReal.dest_shelfunits_pos + "]，任务id["
                                    + waitesendtaskReal.id + "]", randDiff);
                            }
                        }
                    }
                }
                #endregion
                //因为库位不长，暂不考虑入库带出库时，出库当前位置至出库取料点的距离远大于当前位置至入库口取料距离时 需要优先入库的情形！！！
            }
        }
        /// <summary>
        /// 获取优先级入库任务、或者对立面的出库任务
        /// </summary>
        /// <returns></returns>
        private hdw_stacker_taskqueue GetTaskInOrOppositeOut(hdw_stacker_taskqueue taskIn, List<wh_wcs_device> stationList
            , List<wh_shelves> whshelvesList)
        {
            if (stationList != null && stationList.Count > 0)
            {
                wh_wcs_device wcsStation = null;
                //  var lstOrderStation = stationList.OrderBy(o => o.number).ToList();
                if (taskIn.src_station % 2 == 0)
                {//入库站台取模=0 ，则对立面-1
                    wcsStation = stationList.Where(w => w.number < taskIn.src_station).OrderByDescending(o => o.number).FirstOrDefault();
                }
                else
                {//如果取模>0, 则对立面+1
                    wcsStation = stationList.Where(w => w.number > taskIn.src_station).OrderBy(o => o.number).FirstOrDefault();
                }
                //判断对立面是否允许出库、不允许直接入库
                if (wcsStation != null && wcsStation.run_status == PLCRunStatus.Ready)
                {
                    hdw_stacker_taskqueue taskOut = bll_hdw_stacker_taskqueue.GetOppositeTaskOut(wcsStation, whshelvesList);
                    if (taskOut != null && taskOut.guid != Guid.Empty)
                    {// 有 =》顺带出库 
                        return taskOut;
                    }
                }
            }
            //没有=》直接入库>>
            return taskIn;
        }
        //发送入库或出库命令
        private void SendTaskQueueCMD(string stacker_identify, hdw_stacker_taskqueue _TaskM, List<wh_shelves> whshelvesList)
        {
            if (_TaskM == null || Guid.Empty == _TaskM.guid)
            {
                return;
            }
            bool commit = false;
            using (var _tran = bll_hdw_stacker_taskqueue.BeginTransaction())
            {
                try
                {
                    _TaskM.taskstatus = WcsTaskStatus.IsSend;
                    _TaskM.updatedate = DateTime.Now;//下发时间   
                    _TaskM.memo += DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "=>>下发任务;";
                    //任务开始时间 取当前时间，因为是根据堆垛机准备好状态且一次下发一条任务
                    _TaskM.startup = DateTime.Now;
                    var rtvtaskqueue = bll_hdw_stacker_taskqueue.Update(_TaskM);
                    //保留上一次执行的任务信息
                    if (!LastTableOfTaskQueue.ContainsKey(stacker_identify))
                    {
                        if (_TaskM.tasktype != WcsTaskType.MoveTo)
                        {//移库不添加
                            LastTableOfTaskQueue.Add(stacker_identify, _TaskM);
                        }
                    }
                    else
                    {
                        if (_TaskM.tasktype != WcsTaskType.MoveTo)
                        {
                            LastTableOfTaskQueue[stacker_identify] = _TaskM;
                        }
                        else
                        {//移库置为空（堆垛机移库完成在巷道中间位置）
                            LastTableOfTaskQueue[stacker_identify] = null;
                        }
                    }
                    //lastTaskinfo = _TaskM;//保存当前下发的任务
                    LTWMSEFModel.SimpleBackValue rtvshelfunits = LTWMSEFModel.SimpleBackValue.False;
                    wh_shelves shelvesM = null;
                    wh_shelves shelf_src = null;
                    wh_shelves shelf_dest = null;
                    //修改对应库位状态
                    if (_TaskM.tasktype == WcsTaskType.StockIn)
                    {//将库位状态标记为入库中
                        var shelfM = bll_wh_shelfunits.GetFirstDefault(w => w.guid == _TaskM.dest_shelfunits_guid);
                        shelvesM = whshelvesList.Where(w => w.guid == shelfM.shelves_guid).FirstOrDefault();
                        shelfM.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.TrayIn;
                        rtvshelfunits = bll_wh_shelfunits.Update(shelfM);
                    }
                    else if (_TaskM.tasktype == WcsTaskType.StockOut)
                    {//将库位状态标记为出库中
                        var shelfM = bll_wh_shelfunits.GetFirstDefault(w => w.guid == _TaskM.src_shelfunits_guid);
                        shelvesM = whshelvesList.Where(w => w.guid == shelfM.shelves_guid).FirstOrDefault();
                        shelfM.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.TrayOut;
                        rtvshelfunits = bll_wh_shelfunits.Update(shelfM);
                    }
                    else if (_TaskM.tasktype == WcsTaskType.MoveTo)
                    {//移库需要起点排，和终点排
                     //起点
                        var shelfM_s = bll_wh_shelfunits.GetFirstDefault(w => w.guid == _TaskM.src_shelfunits_guid);
                        shelf_src = whshelvesList.Where(w => w.guid == shelfM_s.shelves_guid).FirstOrDefault();
                        /*   shelfM_s.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.TrayOut;
                           bll_wh_shelfunits.Update(shelfM_s);*/
                        //终点
                        var shelfM_d = bll_wh_shelfunits.GetFirstDefault(w => w.guid == _TaskM.dest_shelfunits_guid);
                        shelf_dest = whshelvesList.Where(w => w.guid == shelfM_d.shelves_guid).FirstOrDefault();
                        /* shelfM_d.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.TrayIn;
                         bll_wh_shelfunits.Update(shelfM_d);*/
                        rtvshelfunits = LTWMSEFModel.SimpleBackValue.True;
                    }
                    //**********
                    SendTaskCMD _sendCMD = new SendTaskCMD();
                    _sendCMD.seq = LTLibrary.ConvertUtility.ToInt(Seq);
                    _sendCMD.task_id = _TaskM.id;// + _increatenumber;//任务id加 1000 减1000 
                    _sendCMD.dest_rack = _TaskM.dest_rack;
                    _sendCMD.dest_col = _TaskM.dest_col;
                    _sendCMD.dest_row = _TaskM.dest_row;
                    _sendCMD.dest_station = _TaskM.dest_station;//纵深（纵深1/纵深2）  
                    _sendCMD.src_rack = _TaskM.src_rack;
                    _sendCMD.src_col = _TaskM.src_col;
                    _sendCMD.src_row = _TaskM.src_row;
                    _sendCMD.src_station = _TaskM.src_station;
                    switch (_TaskM.tasktype)
                    {
                        case WcsTaskType.StockIn:
                            //入库 起点站台=起点排
                            _sendCMD.src_rack = _sendCMD.src_station;// / 100;
                            _sendCMD.cmd = SendCMDEnum.CMDIn;
                            if (shelvesM != null)
                            {
                                //排转换
                                _sendCMD.dest_rack = shelvesM.rack_of_wcs;
                                //列反转
                                if (shelvesM.columns_reversal_wcs == true)
                                {
                                    _sendCMD.dest_col = shelvesM.columns_specs - _sendCMD.dest_col + 1;
                                }
                                //列偏移
                                _sendCMD.dest_col += shelvesM.columns_offset_wcs;
                                //层偏移
                                _sendCMD.dest_row += shelvesM.rows_offset_wcs;
                            }
                            break;
                        case WcsTaskType.StockOut:
                            //出库 终点排=终点站台
                            _sendCMD.dest_rack = _sendCMD.dest_station;// / 100;
                            _sendCMD.cmd = SendCMDEnum.CMDOut;
                            if (shelvesM != null)
                            {
                                //排转换
                                _sendCMD.src_rack = shelvesM.rack_of_wcs;
                                //列反转
                                if (shelvesM.columns_reversal_wcs == true)
                                {
                                    _sendCMD.src_col = shelvesM.columns_specs - _sendCMD.src_col + 1;
                                }
                                //列偏移
                                _sendCMD.src_col += shelvesM.columns_offset_wcs;
                                //层偏移
                                _sendCMD.src_row += shelvesM.rows_offset_wcs;
                            }
                            break;
                        case WcsTaskType.MoveTo:
                            _sendCMD.cmd = SendCMDEnum.CMDMove;
                            if (shelf_src != null)
                            {//起点
                             //排转换
                                _sendCMD.src_rack = shelf_src.rack_of_wcs;
                                //列反转 
                                if (shelf_src.columns_reversal_wcs == true)
                                {
                                    _sendCMD.src_col = shelf_src.columns_specs - _sendCMD.src_col + 1;
                                }
                                //列偏移
                                _sendCMD.src_col += shelf_src.columns_offset_wcs;
                                //层偏移
                                _sendCMD.src_row += shelf_src.rows_offset_wcs;
                            }
                            if (shelf_dest != null)
                            {//终点
                             //排转换
                                _sendCMD.dest_rack = shelf_dest.rack_of_wcs;
                                //列反转
                                if (shelf_dest.columns_reversal_wcs == true)
                                {
                                    _sendCMD.dest_col = shelf_dest.columns_specs - _sendCMD.dest_col + 1;
                                }
                                //列偏移
                                _sendCMD.dest_col += shelf_dest.columns_offset_wcs;
                                //层偏移
                                _sendCMD.dest_row += shelf_dest.rows_offset_wcs;
                            }
                            break;
                    }
                    string sendJson = Newtonsoft.Json.JsonConvert.SerializeObject(_sendCMD);
                    /* //做客户端：不为空则发送
                     SocketClient?.SendMessage(sendJson);
                     //做服务端：不为空则发送
                     SocketServer?.SendALL(sendJson);*/
                    //保存数据至待发送表                
                    var rtvWaitedSend = SaveWaitedSendMessage(sendJson);

                    /********************/
                    if (rtvtaskqueue == LTWMSEFModel.SimpleBackValue.True && rtvshelfunits == LTWMSEFModel.SimpleBackValue.True
                        && rtvWaitedSend == LTWMSEFModel.SimpleBackValue.True)
                    {
                        //tran.Commit();
                        commit = true;
                    }
                    else
                    {
                        WinServiceFactory.Log.v("处理出入库任务失败33115Q52>>数据保存异常...");
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

        #region
        /* /// <summary>
         ///将取消或者完成的任务归入历史表中
         /// </summary>
         public void SetTaskToHistory(wh_warehouse warehouse)
         {
             List<hdw_stacker_taskqueue> lstTaskqueue = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.warehouse_guid == warehouse.guid &&
             (w.taskstatus == WcsTaskStatus.Canceled || w.taskstatus == WcsTaskStatus.Finished
             || w.taskstatus == WcsTaskStatus.ForceComplete || w.taskstatus == WcsTaskStatus.Exception));
             if (lstTaskqueue != null && lstTaskqueue.Count > 0)
             {

                 foreach (var item in lstTaskqueue)
                 {
                     //using (System.Data.Entity.DbContextTransaction tran = bll_hdw_stacker_taskqueue.BeginTransaction())
                     //{
                     try
                     {
                         var _taskqueue_history = new hdw_stacker_taskqueue_his();
                         _taskqueue_history.createdate = item.createdate;
                         _taskqueue_history.dest_shelfunits_guid = item.dest_shelfunits_guid;
                         _taskqueue_history.createuser = item.createuser;
                         _taskqueue_history.dest_col = item.dest_col;
                         _taskqueue_history.dest_rack = item.dest_rack;
                         _taskqueue_history.dest_row = item.dest_row;
                         _taskqueue_history.dest_station = item.dest_station;
                         _taskqueue_history.endup = item.endup;
                         _taskqueue_history.guid = Guid.NewGuid();
                         _taskqueue_history.is_emptypallet = item.is_emptypallet;
                         _taskqueue_history.memo = item.memo;
                         _taskqueue_history.order = item.order;
                         _taskqueue_history.rowversion = item.rowversion;
                         _taskqueue_history.shelves_guid = item.shelves_guid;
                         _taskqueue_history.src_shelfunits_guid = item.src_shelfunits_guid;
                         _taskqueue_history.taskqueue_id = item.id;
                         _taskqueue_history.taskqueue_guid = item.guid;
                         _taskqueue_history.tray1_matter_barcode1 = item.tray1_matter_barcode1;
                         _taskqueue_history.tray1_matter_barcode2 = item.tray1_matter_barcode2;
                         _taskqueue_history.tray2_matter_barcode1 = item.tray2_matter_barcode1;
                         _taskqueue_history.tray2_matter_barcode2 = item.tray2_matter_barcode2;
                         _taskqueue_history.updateuser = item.updateuser;
                         _taskqueue_history.sort = item.sort;
                         _taskqueue_history.src_col = item.src_col;
                         _taskqueue_history.src_rack = item.src_rack;
                         _taskqueue_history.src_row = item.src_row;
                         _taskqueue_history.src_station = item.src_station;
                         _taskqueue_history.startup = item.startup;
                         _taskqueue_history.state = item.state;
                         _taskqueue_history.taskstatus = item.taskstatus;
                         _taskqueue_history.tasktype = item.tasktype;
                         _taskqueue_history.tray1_barcode = item.tray1_barcode;
                         _taskqueue_history.tray2_barcode = item.tray2_barcode;
                         _taskqueue_history.updatedate = item.updatedate;
                         _taskqueue_history.warehouse_guid = item.warehouse_guid;
                         _taskqueue_history.src_shelfunits_pos = item.src_shelfunits_pos;
                         _taskqueue_history.dest_shelfunits_pos = item.dest_shelfunits_pos;
                         _taskqueue_history.matterbarcode_list = item.matterbarcode_list;
                         _taskqueue_history.re_detail_traymatter_guid = item.re_detail_traymatter_guid;
                         _taskqueue_history.bills_type = item.bills_type;
                         var rtv = bll_hdw_stacker_taskqueue_his.Add(_taskqueue_history);
                         if (rtv != LTWMSEFModel.SimpleBackValue.True)
                         {
                             Services.WinServiceFactory.Log.v("8777777520修改数据失败:" + Enum.GetName(typeof(LTWMSEFModel.SimpleBackValue), rtv));
                         }
                         else
                         {//添加成功，删除
                             bll_hdw_stacker_taskqueue.Delete(item);
                         }
                         //  tran.Commit();
                     }
                     catch (Exception ex)
                     {
                         //  tran.Rollback();
                         WinServiceFactory.Log.v(ex);
                     }
                     //} 
                 }
             }
         }
         */
        #endregion


        /// <summary>
        /// 站台对应的货架
        /// </summary>
        System.Collections.Hashtable tableShelvesOfStation = new System.Collections.Hashtable();
        /// <summary>
        /// 检查入库未分配库位，分配库位
        /// </summary>
        public void CheckWaiteDispatchStockCell(wh_warehouse warehouse)
        {
            //检查入库未分配库位，分配库位
            List<hdw_stacker_taskqueue> waitdispList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.warehouse_guid == warehouse.guid
            && w.state == LTWMSEFModel.EntityStatus.Normal && w.taskstatus == WcsTaskStatus.WaiteDispatchStockCell
            && w.tasktype == WcsTaskType.StockIn);
            if (waitdispList != null && waitdispList.Count > 0)
            {
                foreach (var w_item in waitdispList)
                {
                    bool commit = false;
                    using (var tran = bll_wh_shelfunits.BeginTransaction())
                    {
                        try
                        {
                            if (!tableShelvesOfStation.ContainsKey(Wcs_srv_guid + "-" + w_item.src_station))
                            {//根据对应的站台 查找对应的库位
                                wh_wcs_device deviceObj2 = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                                  && w.wcs_srv_guid == Wcs_srv_guid && w.number == w_item.src_station && w.device_type == DeviceTypeEnum.Station);
                                tableShelvesOfStation.Add(Wcs_srv_guid + "-" + w_item.src_station, bll_wh_shelves.GetAllShelvesByStation(deviceObj2));
                            }
                            //对应起点站台可分配的库位
                            List<wh_shelves> lstShelves = tableShelvesOfStation[Wcs_srv_guid + "-" + w_item.src_station] as List<wh_shelves>;
                            wh_shelfunits _shelfC = bll_wh_shelfunits.GetStoreShelfUnitsAndLock(lstShelves);
                            if (_shelfC != null && Guid.Empty != _shelfC.guid)
                            {
                                //修改任务为已分配
                                w_item.taskstatus = WcsTaskStatus.Holding;
                                w_item.warehouse_guid = _shelfC.warehouse_guid;
                                w_item.shelves_guid = _shelfC.shelves_guid;
                                w_item.dest_shelfunits_guid = _shelfC.guid;
                                w_item.dest_shelfunits_pos = _shelfC.shelfunits_pos;
                                w_item.dest_rack = _shelfC.rack;
                                w_item.dest_col = _shelfC.columns;
                                w_item.dest_row = _shelfC.rows;
                                w_item.updatedate = DateTime.Now;
                                w_item.updateuser = curr_username;
                                w_item.memo = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "=>>分配库位：[" + _shelfC.shelfunits_pos + "];";
                                var rtvtaskqueue = bll_hdw_stacker_taskqueue.Update(w_item);
                                if (rtvtaskqueue == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    //_tran.Commit();
                                    commit = true;
                                }
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
        /// <summary>
        /// 防止并发导致未解锁库位，查询并解锁syslock
        /// 入库库位存在内侧阻挡库位或分配入库库位存在外侧出库任务时 锁定 库位为ShelfLockType.SysLock
        /// </summary>
        public void CheckSysLockShelfUnit_Free(wh_warehouse warehouse)
        {
            //表 GetStoreShelfUnitsAndLock
            //_shelfUnitsObj.locktype = ShelfLockType.SysLock
            //解锁库位SysLock 为可入库，不然库位可能由于并发未修改
            var sysLockList = bll_wh_shelfunits.GetAllQuery(w => w.warehouse_guid == warehouse.guid && w.cellstate == ShelfCellState.CanIn && w.locktype == ShelfLockType.SysLock);
            if (sysLockList != null && sysLockList.Count > 0)
            {
                foreach (var item in sysLockList)
                {
                    //  bll_wh_shelfunits.GetAllQuery(w=>w.warehouse_guid==warehouse.guid&&w.rack== item.rack)
                    var listShelves = bll_wh_shelves.GetRelatedShelves(item);
                    var shelfGuids = listShelves.Select(s => s.guid).ToList();
                    int _count = bll_wh_shelfunits.GetCount(w => shelfGuids.Contains(w.shelves_guid)
                     && w.warehouse_guid == warehouse.guid && w.same_side_mark == item.same_side_mark && w.columns == item.columns
                     && w.rows == item.rows && w.cellstate != ShelfCellState.CanIn);
                    if (_count == 0)
                    {//可解锁
                        item.locktype = ShelfLockType.Normal;//解锁
                        bll_wh_shelfunits.Update(item);
                    }
                }
            }
            //预留单处理（后期跟mess对接）
            /* var reservModel = bll_billah_reserved_order.GetFirstDefault(w => w.bill_out_status == LTWMSEFModel.BillsAihua.ReserveBillOutStatus.Running);
             if (reservModel != null && reservModel.guid != Guid.Empty)
             {
                 //查找对应明细 跟踪对应的任务状态。。。
                 var rsvDetails = bll_billah_reserved_order.getAllMatterDetails(reservModel);
                 int _notexistcount = 0;
                 int _finishedcount = 0;
                 if (rsvDetails != null && rsvDetails.Count > 0)
                 {
                     foreach (var item in rsvDetails)
                     {
                         if (item.task_guid != null)
                         {
                             if (item.task_status == LTWMSEFModel.TaskStatusEnum.Finished)
                             {
                                 _finishedcount++;
                             }
                             else
                             {
                                 var _status = bll_hdw_stacker_taskqueue.GetTaskRunStatus(item.task_guid.Value);
                                 if (item.task_status != _status)
                                 {//执行或强制完成进行修改 
                                     item.task_status = _status;
                                     bll_billah_reserved_order_detail.Update(item);
                                 }
                             }
                         }
                         else
                         {
                             _notexistcount++;
                         }
                     }
                 }

                 if (rsvDetails.Count == (_notexistcount + _finishedcount))
                 {//  结束预留单
                     reservModel.bill_out_status = LTWMSEFModel.BillsAihua.ReserveBillOutStatus.Finished;
                     reservModel.updatedate = DateTime.Now;
                     reservModel.memo += ">>[" + DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + "]自动结束";
                     reservModel.total_success = _finishedcount;
                     bll_billah_reserved_order.Update(reservModel);
                 }
                 else
                 {
                     if (reservModel.total_success != _finishedcount)
                     {
                         //如果总数量有变化则修改
                         reservModel.total_success = _finishedcount;
                         bll_billah_reserved_order.Update(reservModel);
                     }
                 }
             }
             */
        }
        public void CheckForeceCancelTaskHandler(wh_warehouse warehouse)
        {
            var listTask = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.warehouse_guid == warehouse.guid &&
            (w.taskstatus == WcsTaskStatus.ForceCompleteHandling || w.taskstatus == WcsTaskStatus.ExceptionHandling
             || w.taskstatus == WcsTaskStatus.CancelHandling));
            if (listTask != null && listTask.Count > 0)
            {
                foreach (var item in listTask)
                {
                    bool commit = false;
                    using (var _tran = bll_hdw_stacker_taskqueue.BeginTransaction())
                    {
                        try
                        {
                            SendForceCancelCMD cmdforcecancel = new SendForceCancelCMD();
                            cmdforcecancel.seq = Seq;
                            cmdforcecancel.task_id = item.id;
                            if (item.taskstatus == WcsTaskStatus.ForceCompleteHandling)
                            {//强制完成
                                cmdforcecancel.type = 0;
                                item.taskstatus = WcsTaskStatus.ForceCompleteSended;
                            }
                            else if (item.taskstatus == WcsTaskStatus.CancelHandling)
                            {//取消
                                cmdforcecancel.type = 1;
                                item.taskstatus = WcsTaskStatus.CancelSended;
                            }
                            else if (item.taskstatus == WcsTaskStatus.ExceptionHandling)
                            {
                                cmdforcecancel.type = 2;
                                item.taskstatus = WcsTaskStatus.ExceptionSended;
                            }
                            var rtv = bll_hdw_stacker_taskqueue.Update(item);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                string sendJson = Newtonsoft.Json.JsonConvert.SerializeObject(cmdforcecancel);
                                var rtvWaitedSend = SaveWaitedSendMessage(sendJson);
                                if (rtvWaitedSend == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    // tran.Commit();
                                    commit = true;
                                }
                            }
                            /*if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                string sendJson = Newtonsoft.Json.JsonConvert.SerializeObject(cmdforcecancel);
                                //做客户端：不为空则发送
                                SocketClient?.SendMessage(sendJson);
                                //做服务端：不为空则发送
                                SocketServer?.SendALL(sendJson);
                            }*/
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
    }
}
