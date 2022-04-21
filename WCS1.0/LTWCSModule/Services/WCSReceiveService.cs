using LTLibrary.Wms;
using LTWCSService.ApplicationService.WcsServer;
using LTWCSService.ApplicationService.WcsServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSModule.Services
{
    public class WCSReceiveService : BaseService
    {
        public WCSReceiveService()
        {

        }
        object receiveObj = new object();
        public void ReceiveHandler(string json, LTProtocol.Tcp.Socket_Client socket, StackerService stackerSrv)
        {
            lock (receiveObj)
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                int _cmd = WmsHelper.getCmd(json);
                if (_cmd > 0)
                {
                    switch (_cmd)
                    {
                        case 101://入库
                            ReceiveTaskCMD cmdIn = (ReceiveTaskCMD)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveTaskCMD));
                            if (stackerSrv != null && stackerSrv.GetConnected() && stackerSrv.GetStatus() == DeviceStatusEnum.Ready)
                            {//plc 正常  且状态为准备好 才下发任务 
                               // cmdIn.dest_col = 7 - cmdIn.dest_col;
                                bool writeOk = stackerSrv.WriteTaskIn(cmdIn.task_id,cmdIn.src_rack, cmdIn.src_col, cmdIn.src_row, cmdIn.src_station, cmdIn.dest_rack, cmdIn.dest_col, cmdIn.dest_row, cmdIn.dest_station);
                                if (writeOk)
                                {
                                    //写入成功 开启监视线程 返回任务执行状态
                                    WatchTaskStatus(socket, stackerSrv);
                                    break;
                                }
                            }
                            //写入失败
                            ReturnError(cmdIn, socket, stackerSrv);
                            break;
                        case 102://出库
                            ReceiveTaskCMD cmdOut = (ReceiveTaskCMD)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveTaskCMD));
                            if (stackerSrv != null && stackerSrv.GetConnected() && stackerSrv.GetStatus() == DeviceStatusEnum.Ready)
                            {//plc 正常  且状态为准备好 才下发任务 
                               // cmdOut.src_col=7-cmdOut.src_col;
                                bool writeOk = stackerSrv.WriteTaskOut(cmdOut.task_id, cmdOut.src_rack, cmdOut.src_col, cmdOut.src_row, cmdOut.src_station, cmdOut.dest_rack, cmdOut.dest_col, cmdOut.src_row, cmdOut.dest_station);
                                if (writeOk)
                                { //写入成功 开启监视线程 返回任务执行状态
                                    WatchTaskStatus(socket, stackerSrv);
                                    break;
                                }
                            }
                            //写入失败
                            ReturnError(cmdOut, socket, stackerSrv);
                            break;
                        case 103://移库
                            ReceiveTaskCMD cmdMove = (ReceiveTaskCMD)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveTaskCMD));
                            if (stackerSrv != null && stackerSrv.GetConnected() && stackerSrv.GetStatus() == DeviceStatusEnum.Ready)
                            {//plc 正常  且状态为准备好 才下发任务 
                                bool writeOk = stackerSrv.WriteMove(cmdMove.task_id, cmdMove.src_rack, cmdMove.src_col, cmdMove.src_row, cmdMove.src_station, cmdMove.dest_rack, cmdMove.dest_col, cmdMove.dest_row, cmdMove.dest_station);
                                if (writeOk)
                                {
                                    //写入成功 开启监视线程 返回任务执行状态
                                    WatchTaskStatus(socket, stackerSrv);
                                    break;
                                }
                            }
                            //写入失败
                            ReturnError(cmdMove, socket, stackerSrv);
                            break;
                        case 105:
                            //强制完成/取消
                            ReceiveForceCancel cmdForceCancel = (ReceiveForceCancel)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(ReceiveForceCancel));
                            if (cmdForceCancel != null&& stackerSrv != null)
                            {
                                stackerSrv.ExitCurrTaskWatcher = true;
                                //等待500毫秒刷新周期，wcs处理线程退出循环
                                System.Threading.Thread.Sleep(500);
                                SendTaskStatus taskStatus = new SendTaskStatus();
                                taskStatus.seq = Seq;
                                taskStatus.task_id = cmdForceCancel.task_id;
                                if (cmdForceCancel.type == 0)
                                {//强制完成
                                    taskStatus.task_status = 6;
                                }
                                else //if (cmdForceCancel.type == 1)
                                {//取消
                                    taskStatus.task_status = 5;
                                }
                                socket.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(taskStatus));
                            }
                            break;
                    }
                }
            }
        }
        private async void WatchTaskStatus(LTProtocol.Tcp.Socket_Client socket, StackerService stackerSrv)
        {
            stackerSrv.ExitCurrTaskWatcher = false;
            System.Threading.Thread.Sleep(3000);
            await TaskWatcher(socket, stackerSrv);
        }
        private Task TaskWatcher(LTProtocol.Tcp.Socket_Client socket, StackerService stackerSrv)
        {
            Task tsk = Task.Run(() =>
            {
                int currStatus = 0;
                bool exit = false;
               /// bool IsExecuted = false;
                //重置流程字
                stackerSrv.ResetWrokFlow();
                do
                {
                    try
                    {
                        if (stackerSrv != null && stackerSrv.GetConnected())
                        {
                            int execStatus = stackerSrv.GetTaskExecuteResult();
                            if (currStatus != execStatus)
                            {
                                Services.WcsServiceFactory.Log.v("读取到状态："+execStatus);
                                currStatus = execStatus;
                                if (currStatus == 999)
                                {//任务完成
                                 //退出线程
                                    stackerSrv.ExitCurrTaskWatcher = true;
                                    exit = true;
                                    SendTaskStatus taskStatus = new SendTaskStatus();
                                    taskStatus.seq = Seq;
                                    taskStatus.task_id = stackerSrv.TaskId;
                                    taskStatus.task_status = 3;
                                    socket.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(taskStatus));
                                }
                                else if (currStatus > 0)
                                {//开始执行  !IsExecuted
                                    ///      IsExecuted = true;
                                    SendTaskStatus taskStatus = new SendTaskStatus();
                                    taskStatus.seq = Seq;
                                    taskStatus.task_id = stackerSrv.TaskId;
                                    taskStatus.task_status = 1;
                                    taskStatus.flow = currStatus;// stackerSrv.GetWorkFlow();
                                    socket.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(taskStatus));
                                }
                            }

                        }
                        if (exit || stackerSrv.ExitCurrTaskWatcher)
                        {
                            Services.WcsServiceFactory.Log.v("退出循环1133123");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Services.WcsServiceFactory.Log.v("[" + stackerSrv.TaskId + "]TaskWatcher异常：", ex);
                    }
                    //500毫秒刷新一次 PLC状态
                    System.Threading.Thread.Sleep(300);
                } while (true);
            });
            return tsk;
        }
        /// <summary>
        ///  写入PLC失败
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="socket"></param>
        private void ReturnError(ReceiveTaskCMD cmd, LTProtocol.Tcp.Socket_Client socket, StackerService stackerSrv)
        {
            SendTaskStatus taskStatus = new SendTaskStatus();
            taskStatus.seq = Seq;
            taskStatus.task_id = cmd.task_id;
            taskStatus.task_status = -1;
            taskStatus.task_info = stackerSrv?.errorMsg;
            socket.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(taskStatus));
        }
    }
}
