using LTLibrary;
using LTWMSService.Stock;
using LTWMSWebMVC.App_Start.AppCode;
using LTWMSWebMVC.App_Start.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.Services
{
    public class InterfaceDealSendService
    {
        LTWMSEFModel.LTModel dbmodel;
        LogToDb logtodb = null;
        stk_inout_recodBLL bll_stk_inout_recod;
        stk_inout_recod_hisBLL bll_stk_inout_recod_his;

        private int _seqqqq;
        public int Seq
        {
            get { return ++_seqqqq; }
        }
        public InterfaceDealSendService()
        {
            dbmodel = new LTWMSEFModel.LTModel();
            logtodb = new LogToDb(new LTWMSService.Logs.log_sys_alarmBLL(dbmodel), new LTWMSService.Logs.log_sys_executeBLL(dbmodel));
            bll_stk_inout_recod = new stk_inout_recodBLL(dbmodel);
            bll_stk_inout_recod_his = new stk_inout_recod_hisBLL(dbmodel);
        }


        /// <summary>
        /// 发送出入库流水
        /// </summary>
        /// <param name="randDiff"></param>
        public void SendInoutRecordCMD()
        {
            var inOutRcdList = bll_stk_inout_recod.GetAllQueryOrderby(o => o.createdate, w => w.is_send == LTWMSEFModel.Stock.IsSendToEnum.None,true);
            if (inOutRcdList != null && inOutRcdList.Count > 0)
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                foreach (var item in inOutRcdList)
                {
                    InoutRecordModel inoutModel = new InoutRecordModel();
                    inoutModel.goods_id = item.goods_id;
                    inoutModel.qty = item.qty;
                    inoutModel.spec_id = item.spec_id;
                    string postUrl = "";
                    if (item.inout_type == LTWMSEFModel.Stock.InOutTypeEnum.In)
                    {//入库流水
                        postUrl = WMSFactory.Config.AihuaStockInURL;
                    }
                    else
                    {//出库流水
                        postUrl = WMSFactory.Config.AihuaStockOutURL;
                    }
                    //发送流水至接口系统
                    string _sendjson = Newtonsoft.Json.JsonConvert.SerializeObject(inoutModel);
                    WMSFactory.Log.v("发送数据至:("+postUrl+")>>>json=【"+ _sendjson + "】");
                    string resp = HttpRequestHelper.HttpPost(postUrl, _sendjson);
                    ReturnBackModel respM = (ReturnBackModel)JsonConvert.DeserializeObject(resp, typeof(ReturnBackModel));
                    if (respM != null && respM.state == 0)
                    {//成功，修改本地数据 
                        item.is_send = LTWMSEFModel.Stock.IsSendToEnum.Sended;
                        item.updatedate = DateTime.Now;
                        bll_stk_inout_recod.Update(item);
                        WMSFactory.Log.v("已成功发送流水至艾华接口系统>>批次[" + item.spec_id + "]/料号[" + item.goods_id + "]/guid[" + item.guid + "]");
                    }
                    else
                    {//发送失败一直发送直到成功！！！！
                        item.error_count += 1;
                        //if (item.error_count >=4)
                        //{//大于=4次发送失败。。。
                        //    item.is_send = LTWMSEFModel.Stock.IsSendToEnum.Failed;
                        //}
                        item.updatedate = DateTime.Now;
                        bll_stk_inout_recod.Update(item);
                    }
                }

            }
        }
        /// <summary>
        /// 将已发送成功的出入库流水归入历史记录表中
        /// </summary>
        public void InoutSendedRecordToHis()
        {
            var inoutRcdList = bll_stk_inout_recod.GetAllQuery(w => w.is_send == LTWMSEFModel.Stock.IsSendToEnum.Sended);
            if (inoutRcdList != null && inoutRcdList.Count > 0)
            {
                //已发送的归入历史，并删除。。。
                foreach (var item in inoutRcdList)
                {
                    var rcdHis = new LTWMSEFModel.Stock.stk_inout_recod_his();
                    rcdHis.createdate = item.createdate;
                    rcdHis.error_count = item.error_count;
                    rcdHis.goods_id = item.goods_id;
                    rcdHis.guid = Guid.NewGuid();
                    rcdHis.inout_type = item.inout_type;
                    rcdHis.is_send = item.is_send;
                    rcdHis.memo = item.memo;
                    rcdHis.qty = item.qty;
                    rcdHis.rowversion = item.rowversion;
                    rcdHis.spec_id = item.spec_id;
                    rcdHis.state = item.state;
                    rcdHis.updatedate = item.updatedate; 
                    var rtv = bll_stk_inout_recod_his.Add(rcdHis);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {//添加成功，删除数据
                        bll_stk_inout_recod.Delete(item);
                    }
                }
            }
        }

        /* /// <summary>
         /// 检查发送批量任务
         /// </summary>
         /// <param name="randDiff"></param>
         public void CheckSendTasks(int randDiff)
         {
             var agvTaskModel = bll_hdw_agv_task_main.GetFirstDefault(w => w.task_execute_status == LTEFModel.Hardware.AgvTaskExecuteStatus.None
             && w.rec_status == LTEFModel.Hardware.AgvReceiveStatus.WaitSend);
             if (agvTaskModel != null && agvTaskModel.guid != Guid.Empty)
             {
                 try
                 {
                     var taskQueueList = bll_hdw_agv_taskqueue.GetAllQueryOrderby(o => o.id, w => w.agv_task_main_guid == agvTaskModel.guid, true);
                     var lstItem = new List<TaskBarcodeModel>();
                     if (taskQueueList != null && taskQueueList.Count > 0)
                     {
                         foreach (var item in taskQueueList)
                         {
                             item.task_status = LTEFModel.Hardware.AgvTaskStatus.IsSend;
                             item.memo += ";[" + DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + "]=>发送任务至AGV";
                             lstItem.Add(new TaskBarcodeModel() { barcode = item.matter_barcode1, task_id = item.id });
                         }
                         var rtv2 = bll_hdw_agv_taskqueue.Update(taskQueueList);
                         if (rtv2 != LTEFModel.SimpleBackValue.True)
                         {//更新数据失败 
                             WMSFactory.Log.v("更新数据失败155556>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv2));
                         }
                         SendTaskModel taskM = new SendTaskModel();
                         taskM.dest_rack = agvTaskModel.dest_rack;
                         taskM.order = agvTaskModel.order;
                         taskM.seq = Seq;
                         taskM.src_rack = agvTaskModel.src_rack;

                         taskM.data = lstItem;
                         string taskJson = Newtonsoft.Json.JsonConvert.SerializeObject(taskM);
                         WMSFactory.Log.v(taskJson);
                         string resp = HttpRequestHelper.HttpPost(WMSFactory.Config.AgvApiSendTask, taskJson);
                         logtodb.DbExecuteLog("发送订单[" + agvTaskModel.order + "]任务至AGV。详细：json==>>>" + taskJson, randDiff);

                         // 界面+重发
                         //解析resp
                         // agv->wms 接收状态
                         //{"state":1 //1：接收信息OK}
                         ComResponseModel respM = (ComResponseModel)Newtonsoft.Json.JsonConvert.DeserializeObject(resp, typeof(ComResponseModel));
                         if (respM != null && respM.state == 1)
                         {
                             //发送接收成功
                             agvTaskModel.rec_status = LTEFModel.Hardware.AgvReceiveStatus.Ok;
                             logtodb.DbExecuteLog("agv返回接收任务成功！订单：" + agvTaskModel.order, randDiff);
                         }
                         else
                         {//发送成功，但返回值解析失败。
                          //{"seq":1,"order":"T01","state":2}
                             agvTaskModel.rec_status = LTEFModel.Hardware.AgvReceiveStatus.SendedError;
                             logtodb.DbExecuteLog("agv返回任务接收失败！解析返回值失败，返回值为：" + resp + "，订单：" + agvTaskModel.order, randDiff);
                         }
                     }
                     else
                     {//没有任务，默认成功且接收成功，为了不入库也能生成搬运任务
                         agvTaskModel.task_execute_status = AgvTaskExecuteStatus.Finished;
                         agvTaskModel.rec_status = LTEFModel.Hardware.AgvReceiveStatus.Ok;
                     }
                     var rtv = bll_hdw_agv_task_main.Update(agvTaskModel);
                     if (rtv != LTEFModel.SimpleBackValue.True)
                     {//更新数据失败
                         WMSFactory.Log.v("更新数据失败15555005>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv));
                     }
                 }
                 catch (Exception ex)
                 {
                     WMSFactory.Log.v(ex);
                 }
             }
             else
             {
                 //如果不存在未发送的主任务，查找子任务是否存在 holding 状态
                 var taskQueue = bll_hdw_agv_taskqueue.GetFirstDefault(w => w.task_status == AgvTaskStatus.Holding);
                 if (taskQueue != null && taskQueue.guid != Guid.Empty)
                 {
                     WMSFactory.Log.v("5225255>>>id:" + taskQueue.order);
                     taskQueue.task_status = LTEFModel.Hardware.AgvTaskStatus.IsSend;
                     taskQueue.memo += ";[" + DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + "]=>发送任务至AGV";
                     var rtv2 = bll_hdw_agv_taskqueue.Update(taskQueue);
                     if (rtv2 != LTEFModel.SimpleBackValue.True)
                     {//更新数据失败 
                         WMSFactory.Log.v("更新数据失败17762252>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv2));
                     }
                     else
                     {//更新成功！
                         var lstItem = new List<TaskBarcodeModel>();
                         SendTaskModel taskM = new SendTaskModel();
                         taskM.dest_rack = taskQueue.dest_point;
                         taskM.order = taskQueue.order;
                         taskM.seq = Seq;
                         taskM.src_rack = taskQueue.src_point;
                         lstItem.Add(new TaskBarcodeModel() { barcode = taskQueue.matter_barcode1, task_id = taskQueue.id });
                         taskM.data = lstItem;
                         string taskJson = Newtonsoft.Json.JsonConvert.SerializeObject(taskM);
                         WMSFactory.Log.v("bbbbbbb99955555>>>" + taskJson);
                         string resp = HttpRequestHelper.HttpPost(WMSFactory.Config.AgvApiSendTask, taskJson);
                         logtodb.DbExecuteLog("发送订单[" + taskQueue.order + "]任务至AGV。详细：json==>>>" + taskJson, randDiff);
                     }
                 }
             }
         }
         */
        public void ResetDbconnection()
        {
            dbmodel = new LTWMSEFModel.LTModel();
        }
        /* /// <summary>
         ///发送当前任务id和电池条码及朝向
         /// </summary>
         /// <param name="randDiff"></param>
         public void GetScanTaskAndSend(int randDiff)
         {
             //查询当前任务状态为 LTEFModel.Hardware.AgvTaskStatus.ScanOk 的数据
             int _total = 0;
             //查询最后一次扫码任务，可能重复扫码，或扫多个码，但只取最后一条
             var agvtaskList = bll_hdw_agv_taskqueue.Pagination(1, 50, out _total, o => o.updatedate, w =>
                    w.task_status == LTEFModel.Hardware.AgvTaskStatus.ScanOk, false);
             if (agvtaskList != null && agvtaskList.Count > 0)
             {
                 // string mess = "";
                 for (int i = 0; i < agvtaskList.Count; i++)
                 {
                     if (i == 0)
                     {//只发送最新的一条记录                    
                         agvtaskList[i].updatedate = DateTime.Now;
                         agvtaskList[i].task_status = LTEFModel.Hardware.AgvTaskStatus.ScanOkSended;
                         agvtaskList[i].memo += ";[" + DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + "]=>已发送电池朝向给AGV";
                         agvtaskList[i].send_count++;
                         var rtv = bll_hdw_agv_taskqueue.Update(agvtaskList[i]);
                         if (rtv != LTEFModel.SimpleBackValue.True)
                         {//更新数据失败
                             WMSFactory.Log.v("更新数据失败155551>>id:" + agvtaskList[i].id + ",返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv));
                         }
                         else
                         {//更新数据成功！
                             WMSFactory.Log.v("6239992成功：" + agvtaskList[i].id);
                             /////////// 发送当前扫码任务（发送当前任务id和电池条码及朝向）至agv
                             SendCurrentTask currTaskM = new SendCurrentTask();
                             currTaskM.barcode = agvtaskList[i].matter_barcode1;
                             currTaskM.direction = agvtaskList[i].direction;
                             currTaskM.seq = Seq;
                             currTaskM.task_id = agvtaskList[i].id;
                             string jsonS = Newtonsoft.Json.JsonConvert.SerializeObject(currTaskM);
                             string resp = HttpRequestHelper.HttpPost(WMSFactory.Config.AgvApiBatteryDirect, jsonS);
                             logtodb.DbExecuteLog("发送当前任务[" + currTaskM.task_id + "]和电池条码[" + currTaskM.barcode
                                 + "]及朝向[" + currTaskM.direction + "]至AGV。详细：json==>>>" + jsonS, randDiff);
                             //不管返回值为空还是不会空，直接标记发送状态为：已发送
                             logtodb.DbExecuteLog("接收返回值：" + resp, randDiff);
                             WMSFactory.Log.v("622212成功：id:" + agvtaskList[i].id + ",接收返回值：" + resp);
                             string mess = "已修改agv任务id[" + agvtaskList[i].id + "] 条码：" + agvtaskList[i].matter_barcode1
                            + " 状态为：" + LTLibrary.EnumHelper.GetEnumDescription(LTEFModel.Hardware.AgvTaskStatus.ScanOkSended);
                             logtodb.DbExecuteLog(mess, randDiff);
                         }
                     }
                     else
                     {//如果存在多条，则修改状态为
                         agvtaskList[i].task_status = LTEFModel.Hardware.AgvTaskStatus.IsSend;
                         agvtaskList[i].memo += ";[" + DateTime.Now.ToString("yyyyMMdd HHmmss.fff") + "]=>修改状态为已发送";
                         agvtaskList[i].updatedate = DateTime.Now;
                         var rtv = bll_hdw_agv_taskqueue.Update(agvtaskList[i]);
                         if (rtv != LTEFModel.SimpleBackValue.True)
                         {//更新数据失败
                             WMSFactory.Log.v("更新数据失败155552>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv));
                         }
                         else
                         {
                             logtodb.DbExecuteLog(";已修改agv任务id[" + agvtaskList[i].id + "] 条码：" + agvtaskList[i].matter_barcode1
                            + " 状态为：" + LTLibrary.EnumHelper.GetEnumDescription(LTEFModel.Hardware.AgvTaskStatus.IsSend), randDiff);
                         }
                         //mess += ";已修改agv任务id[" + agvtaskList[i].id + "] 条码：" + agvtaskList[i].matter_barcode1
                         //    + " 状态为：" + LTLibrary.EnumHelper.GetEnumDescription(LTEFModel.Hardware.AgvTaskStatus.IsSend);
                     }
                 }
                 //bll_hdw_agv_taskqueue.Update(agvtaskList);
                 //logtodb.DbExecuteLog(mess, randDiff);
             }
         }
         /// <summary>
         /// 处理agv任务完成、取消等状态
         /// </summary>
         public void DealAgvTaskFinish(bool isConnectedToAgv)
         {
             var agvTaskList = bll_hdw_agv_taskqueue.GetAllQuery(w => w.task_status == LTEFModel.Hardware.AgvTaskStatus.Finished ||
             w.task_status == LTEFModel.Hardware.AgvTaskStatus.ForceCompleted || w.task_status == LTEFModel.Hardware.AgvTaskStatus.Canceled ||
             w.task_status == LTEFModel.Hardware.AgvTaskStatus.CancelHandling);
             if (agvTaskList != null && agvTaskList.Count > 0)
             {
                 foreach (var item in agvTaskList)
                 {
                     //using (var tran = bll_hdw_agv_task_main.BeginTransaction())
                     //{
                     try
                     {
                         //  WMSFactory.Log.v("023911开始事务：id:" + item.id);
                         if (item.task_status == LTEFModel.Hardware.AgvTaskStatus.Finished ||
                           item.task_status == LTEFModel.Hardware.AgvTaskStatus.ForceCompleted)
                         {//任务已完成(强制完成)
                             int idx = 0;
                             do
                             {
                                 idx++;
                                 if (idx > 3)
                                 {
                                     break;
                                 }
                                 var mainObj = bll_hdw_agv_task_main.GetFirstDefault(w => w.guid == item.agv_task_main_guid);
                                 if (mainObj != null && mainObj.guid != Guid.Empty)
                                 {//  任务完成 修改对应主agv任务数据。。。
                                     mainObj.total_success = mainObj.total_success + 1;
                                     //判断剩余任务是否为0，为0则任务结束
                                     int countlast = bll_hdw_agv_taskqueue.GetCount(w => w.agv_task_main_guid == item.agv_task_main_guid
                                          &&
                                          w.task_status != LTEFModel.Hardware.AgvTaskStatus.Finished
                                          && w.task_status != LTEFModel.Hardware.AgvTaskStatus.ForceCompleted
                                          && w.task_status != LTEFModel.Hardware.AgvTaskStatus.Canceled
                                          && w.task_status != LTEFModel.Hardware.AgvTaskStatus.CancelHandling
                                         );
                                     //任务全部接收完成
                                     if (mainObj.total_success >= mainObj.total_count && countlast == 0)
                                     {//任务已全部执行完成
                                         mainObj.task_execute_status = LTEFModel.Hardware.AgvTaskExecuteStatus.Finished;
                                         //将出库订单修改为正在终止
                                         int idx22 = 0;
                                         do
                                         {
                                             idx22++;
                                             if (idx22 > 3)
                                             {
                                                 break;
                                             }
                                             var BillOut = bll_bill_stockout.GetFirstDefault(w => w.bill_property == LTEFModel.Bills.BillsProperty.Battery
                                   && (w.bill_status == LTEFModel.Bills.BillsStatus_Out.None || w.bill_status == LTEFModel.Bills.BillsStatus_Out.Running ||
                                       w.bill_status == LTEFModel.Bills.BillsStatus_Out.Finishing));
                                             if (BillOut != null && BillOut.guid != Guid.Empty)
                                             {
                                                 BillOut.bill_status = LTEFModel.Bills.BillsStatus_Out.Finished;
                                                 BillOut.updatedate = DateTime.Now;
                                                 var rtv2 = bll_bill_stockout.Update(BillOut);
                                                 if (rtv2 != LTEFModel.SimpleBackValue.True)
                                                 {//更新数据失败
                                                     WMSFactory.Log.v("更新数据失败155553>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv2));
                                                 }
                                                 else
                                                 {//更新成功！！！
                                                     break;
                                                 }
                                             }
                                             else
                                             {
                                                 break;
                                             }
                                             System.Threading.Thread.Sleep(500);
                                         } while (true);
                                     }
                                     else
                                     {
                                         mainObj.task_execute_status = LTEFModel.Hardware.AgvTaskExecuteStatus.Running;
                                     }
                                     var rtv4 = bll_hdw_agv_task_main.Update(mainObj);
                                     if (rtv4 != LTEFModel.SimpleBackValue.True)
                                     {//更新数据失败
                                         WMSFactory.Log.v("更新数据失败177455554>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv4));
                                         //  tran.Rollback();                                    
                                     }
                                     else
                                     {
                                         WMSFactory.Log.v("更新agv主任务成功！订单号：" + mainObj.order);
                                         break;
                                     }
                                 }
                                 else
                                 {
                                     break;
                                 }
                                 System.Threading.Thread.Sleep(500);
                             } while (true);
                         }
                         else if (item.task_status == LTEFModel.Hardware.AgvTaskStatus.Canceled)
                         {//任务取消

                             //任务取消后，再次扫码会自动重新生成一条agv搬运任务。
                         }
                         else if (item.task_status == AgvTaskStatus.CancelHandling)
                         {//wms接收终止插箱指令，结束agv任务
                             item.task_status = AgvTaskStatus.Canceled;
                             //发送取消任务至AGV
                             SendCancelByTaskId taskM = new SendCancelByTaskId();
                             taskM.task_id = item.id.ToString();
                             string taskJson = Newtonsoft.Json.JsonConvert.SerializeObject(taskM);
                             WMSFactory.Log.v(taskJson);
                             try
                             {//不管删除成功没成功都不管
                                 if (isConnectedToAgv)
                                 {//连接成功发送删除，没连上不管了....
                                     string resp = HttpRequestHelper.HttpPost(WMSFactory.Config.AgvApiCancelTask, taskJson);
                                 }
                             }
                             catch (Exception ex)
                             {

                             }
                             logtodb.DbExecuteLog("取消插箱AGV搬运任务 agv任务id[" + taskM.task_id + "]：json==>>>" + taskJson, 0);
                         }
                         //agv任务归入历史。。。  
                         var hisObj = bll_hdw_agv_taskqueue_his.AgvTaskQueueToHisObj(item);
                         var rtv = bll_hdw_agv_taskqueue_his.Add(hisObj);
                         if (rtv != LTEFModel.SimpleBackValue.True)
                         {//更新数据失败
                             WMSFactory.Log.v("add数据失败155541////[" + hisObj.agv_taskqueue_id + "]>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv));
                         }
                         //删除agv任务
                         var rtv3 = bll_hdw_agv_taskqueue.Delete(item);
                         if (rtv3 != LTEFModel.SimpleBackValue.True)
                         {//更新数据失败
                             WMSFactory.Log.v("更新数据失败155542>>返回结果：" + Enum.GetName(typeof(LTEFModel.SimpleBackValue), rtv3));
                         }

                     }
                     catch (Exception ex)
                     {
                         WMSFactory.Log.v("保存数据异常65475596：" + ex.ToString());
                     }
                     //WMSFactory.Log.v("023211131回滚事务：id:" + item.id);
                     //tran.Rollback();
                     //} 

                 }
             }

         }
         */
    }
}