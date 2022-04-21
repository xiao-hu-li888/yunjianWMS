using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer
{
    /// <summary>
    /// 堆垛机处理服务
    /// </summary>
    public class StackerService : S7BaseService
    {
        /**************PLC变量配置************/
        StackerConfig stackerConfig;
        /***********PLC变量配置*************/
        /// <summary>
        /// 退出当前监视线程
        /// </summary>
        public bool ExitCurrTaskWatcher;
        /// <summary>
        /// 任务编号
        /// </summary>
        public int TaskId;
        public StackerService(string Ip, CpuType cputype, StackerConfig stackerConfig) : base(Ip, cputype)
        {
            /* string dbStackerStatus,string dbTaskStatus,string dbFlow, string dbBoot, string dbTaskId, string dbSrcRack,
            string dbSrcCol, string dbSrcRow, string dbSrcStation , string dbDestRack, string dbDestCol, string dbDestRow,
            string dbDestStation */
            this.stackerConfig = stackerConfig;
            //this.dbStackerStatus = dbStackerStatus;
            //this.dbTaskStatus = dbTaskStatus;
            //this.dbFlow = dbFlow;
            //this.dbBoot = dbBoot;
            //this.dbTaskId = dbTaskId;
            //this.dbSrcRack = dbSrcRack;
            //this.dbSrcCol = dbSrcCol;
            //this.dbSrcRow = dbSrcRow;
            //this.dbSrcStation = dbSrcStation;
            //this.dbDestRack = dbDestRack;
            //this.dbDestCol = dbDestCol;
            //this.dbDestRow = dbDestRow;
            //this.dbDestStation = dbDestStation;
        }
        /**垂直库
        / *************堆垛机 出库、入库、移库 等操作*************** /
        /// <summary>
        ///INT(16位) 堆垛机状态(1自动准备好 2运行 3故障)  状态为1可以下发启动命令
        /// </summary>
        string dbStackerStatus = "DB80.DBW0";
        /// <summary>
        ///INT(16位) 任务完成（1任务完成 0未完成）  每次任务完成后为1
        /// </summary>
        string dbTaskStatus = "DB80.DBW2";
        /// <summary>
        /// INT(16位) 流程字（1货叉归中 2去取货点 3取货伸叉 4 取货抬起 5取货缩回 6去放货点 7放货伸叉 8放货下降 9放货缩回）
        /// </summary>
        string dbFlow = "DB80.DBW4";

        /// <summary>
        ///INT(16位) 启动标志（0默认未启动 1入库 2出库 3站内中转）
        /// </summary>
        string dbBoot = "DB80.DBW50";
        /// <summary>
        ///DINT(32位) 任务号（不等于0）
        /// </summary>
        string dbTaskId = "DB80.DBD52";
        /// <summary>
        ///INT（16位） 起点排
        /// </summary>
        string dbSrcRack = "DB80.DBW56";
        /// <summary>
        ///INT（16位） 起点列
        /// </summary>
        string dbSrcCol = "DB80.DBW58";
        /// <summary>
        ///INT（16位） 起点层
        /// </summary>
        string dbSrcRow = "DB80.DBW60";
        /// <summary>
        /// 起点站台（扩展用）
        /// </summary>
        string dbSrcStation = ""; 
        /// <summary>
        ///INT（16位） 终点排
        /// </summary>
        string dbDestRack = "DB80.DBW62";
        /// <summary>
        ///INT（16位） 终点列
        /// </summary>
        string dbDestCol = "DB80.DBW64";
        /// <summary>
        ///INT（16位） 终点层
        /// </summary>
        string dbDestRow = "DB80.DBW66";
        /// <summary>
        /// 终点站台（纵深1、2） 扩展用
        /// </summary>
        string dbDestStation = "";
        */
        /*
        //展示库
        /*************堆垛机 出库、入库、移库 等操作*************** /
        /// <summary>
        ///INT(16位) 堆垛机状态(1自动准备好 2运行 3故障)  状态为1可以下发启动命令
        /// </summary>
        string dbStackerStatus = "DB1.DBB650.0";//DB1.DBW0
        /// <summary>
        ///INT(16位) 任务完成（1任务完成 0未完成）  每次任务完成后为1
        /// </summary>
        string dbTaskStatus = "DB1.DBW1502";//DB1.DBW2
        /// <summary>
        /// INT(16位) 流程字（1货叉归中 2去取货点 3取货伸叉 4 取货抬起 5取货缩回 6去放货点 7放货伸叉 8放货下降 9放货缩回）
        /// </summary>
        string dbFlow = "DB1.DBW612";//DB1.DBW4

        /// <summary>
        ///INT(16位) 启动标志（0默认未启动 1入库 2出库 3站内中转）
        /// </summary>
        string dbBoot = "DB1.DBW1550";//DB1.DBW50
        /// <summary>
        ///DINT(32位) 任务号（不等于0）
        /// </summary>
        string dbTaskId = "DB1.DBD1552";//DB1.DBD52
        /// <summary>
        ///INT（16位） 起点排
        /// </summary>
        string dbSrcRack = "DB1.DBW204";//DB1.DBW56
        /// <summary>
        ///INT（16位） 起点列
        /// </summary>
        string dbSrcCol = "DB1.DBW200";//DB1.DBW58
        /// <summary>
        ///INT（16位） 起点层
        /// </summary>
        string dbSrcRow = "DB1.DBW202";//DB1.DBW60
        /// <summary>
        /// 起点站台（扩展用）
        /// </summary>
        string dbSrcStation = "";

        /// <summary>
        ///INT（16位） 终点排
        /// </summary>
        string dbDestRack = "DB1.DBW210";//DB1.DBW62
        /// <summary>
        ///INT（16位） 终点列
        /// </summary>
        string dbDestCol = "DB1.DBW206";//DB1.DBW64
        /// <summary>
        ///INT（16位） 终点层
        /// </summary>
        string dbDestRow = "DB1.DBW208";//DB1.DBW66
        /// <summary>
        /// 终点站台（纵深1、2） 扩展用
        /// </summary>
        string dbDestStation = "";
        */
        //  object statusSync = new object();
        /// <summary>
        /// 获取堆垛机的实时状态
        /// </summary>
        /// <returns></returns>
        public DeviceStatusEnum GetStatus()
        {//堆垛机状态
            if (plc == null)
                return DeviceStatusEnum.None;
            lock (plc)
            {
                if (isConnected)
                {
                    int _idx = 0;
                    do
                    {
                        try
                        {
                            if (_idx >= 3)
                            {//状态读取3次失败，则返回默认值
                                break;
                            }
                            //Convert.ToBoolean(plc.Read("DB200.DBX1.6")) 
                            var _v = Convert.ToInt32(plc.Read(stackerConfig.dbStackerStatus));
                            switch (_v)
                            {
                                case 1:
                                    return DeviceStatusEnum.Ready;
                                case 2:
                                    return DeviceStatusEnum.Running;
                                case 3:
                                    return DeviceStatusEnum.Warnning;
                                case 0:
                                default:
                                    return DeviceStatusEnum.None;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogV("获取堆垛机实时状态异常>>>" + ex.ToString());
                        }
                        _idx++;
                        System.Threading.Thread.Sleep(50);
                    } while (true);
                }
            }
            return DeviceStatusEnum.None;
        }
        /// <summary>
        /// 根据站台编号 1、2、3、4 获取对应的出库准备好信息
        /// </summary>
        /// <param name="station">1、2、3、4</param>
        /// <returns></returns>
        public DeviceStatusEnum GetReadyOutOfStation(int station)
        {//获取出库口 出库准备好 
            if (plc == null)
                return DeviceStatusEnum.Warnning;
            lock (plc)
            {
                if (isConnected)
                {
                    int _idx = 0;
                    do
                    {
                        try
                        {
                            if (_idx >= 3)
                            {//状态读取3次失败，则返回默认值
                                break;
                            }
                            //Convert.ToBoolean(plc.Read("DB200.DBX1.6")) 
                            var _v = 0;
                            if (station == 1)
                            {
                                if (!string.IsNullOrWhiteSpace(stackerConfig.dbReady100))
                                {
                                    _v = Convert.ToInt32(plc.Read(stackerConfig.dbReady100));
                                }
                                else
                                {//通过警告判断没有配置，则不发送状态信息
                                    return DeviceStatusEnum.Warnning;
                                }
                            }
                            else if (station == 2)
                            {
                                if (!string.IsNullOrWhiteSpace(stackerConfig.dbReady200))
                                {
                                    _v = Convert.ToInt32(plc.Read(stackerConfig.dbReady200));
                                }
                                else
                                {//通过警告判断没有配置，则不发送状态信息
                                    return DeviceStatusEnum.Warnning;
                                }
                            }
                            else if (station == 3)
                            {
                                if (!string.IsNullOrWhiteSpace(stackerConfig.dbReady300))
                                {
                                    _v = Convert.ToInt32(plc.Read(stackerConfig.dbReady300));
                                }
                                else
                                {//通过警告判断没有配置，则不发送状态信息
                                    return DeviceStatusEnum.Warnning;
                                }
                            }
                            else if (station == 4)
                            {
                                if (!string.IsNullOrWhiteSpace(stackerConfig.dbReady400))
                                {
                                    _v = Convert.ToInt32(plc.Read(stackerConfig.dbReady400));
                                }
                                else
                                {//通过警告判断没有配置，则不发送状态信息
                                    return DeviceStatusEnum.Warnning;
                                }
                            }
                            else
                            {
                                return DeviceStatusEnum.Warnning;
                            }
                            if (_v == 1)
                            {
                                return DeviceStatusEnum.Ready;
                            }
                            return DeviceStatusEnum.None;
                        }
                        catch (Exception ex)
                        {
                            LogV("获取出库准备好状态异常>>>" + ex.ToString());
                        }
                        _idx++;
                        System.Threading.Thread.Sleep(50);
                    } while (true);
                }
            }
            return DeviceStatusEnum.Warnning;
        }
        /// <summary>
        /// 任务写入成功时间
        /// </summary>
        public DateTime? StartDate;
        /// <summary>
        /// 监控任务的工作流程字
        /// </summary>
        private int work_flow;
        public void ResetWrokFlow()
        {
            work_flow = 0;
        }
        ///// <summary>
        ///// 获取工作流
        ///// </summary>
        ///// <returns></returns>
        //public int GetWorkFlow()
        //{
        //    return Convert.ToInt32(plc.Read(stackerConfig.dbFlow));
        //}
        //  object execstatusSync = new object();
        /// <summary>
        /// 获取堆垛机任务执行结果(0未执行 1-9正在执行 999执行完成)
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public int GetTaskExecuteResult()
        {
            if (plc == null)
                return 0;
            lock (plc)
            {
                if (isConnected)
                {
                    try
                    {
                        if (Convert.ToInt32(plc.Read(stackerConfig.dbTaskId)) == this.TaskId)
                        {//如果当前执行中的任务ID=taskid，则判断执行结果
                         //启动时间不为空，且启动时间5秒后读到值成功 才有效（一开始写成功，plc清除上一次成功状态还需要时间）
                            if (Convert.ToInt32(plc.Read(stackerConfig.dbTaskStatus)) == 1 &&
                                work_flow >= 9)
                            // Convert.ToInt32(plc.Read(stackerConfig.dbFlow)) >= 9)
                            {//执行成功  >> 流程==10 & 任务完成状态为1
                                if (this.StartDate != null && LTLibrary.ConvertUtility.DiffSeconds(this.StartDate.Value, DateTime.Now) > 10)
                                // && Convert.ToInt32(plc.Read(stackerConfig.dbTaskStatus)) == 1)
                                {
                                    return 999;
                                }
                            }
                            else
                            {//正在执行中
                                int flw = Convert.ToInt32(plc.Read(stackerConfig.dbFlow));
                                if (flw >= 9)
                                {
                                    work_flow = flw;
                                }
                                return flw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogV("获取堆垛机执行结果异常>>>" + ex.ToString());
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 写任务失败错误提示信息
        /// </summary>
        public string errorMsg;
        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="srcRack"></param>
        /// <param name="srcCol"></param>
        /// <param name="srcRow"></param>
        /// <param name="srcStation"></param>
        /// <param name="destRack"></param>
        /// <param name="destCol"></param>
        /// <param name="destRow"></param>
        /// <param name="destStation"></param>
        /// <returns></returns>
        public bool WriteTaskIn(int taskId, int srcRack, int srcCol, int srcRow, int srcStation, int destRack, int destCol, int destRow, int destStation)
        {
            if (plc == null)
                return false;
            lock (plc)
            {   //入库起点站台指定给 起点排
                //srcRack = srcStation;
                // 1入库 
                if (isConnected)
                {
                    errorMsg = "";
                    return WriteTask(taskId, srcRack, srcCol, srcRow, srcStation, destRack, destCol, destRow, destStation, 1);
                }
            }
            return false;
        }
        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="srcRack"></param>
        /// <param name="srcCol"></param>
        /// <param name="srcRow"></param>
        /// <param name="srcStation"></param>
        /// <param name="destRack"></param>
        /// <param name="destCol"></param>
        /// <param name="destRow"></param>
        /// <param name="destStation"></param>
        /// <returns></returns>
        public bool WriteTaskOut(int taskId, int srcRack, int srcCol, int srcRow, int srcStation, int destRack, int destCol, int destRow, int destStation)
        {
            if (plc == null)
                return false;
            lock (plc)
            {
                //出库终点站台 指定给终点排
                //  destRack = destStation;
                //2出库 
                if (isConnected)
                {
                    errorMsg = "";
                    return WriteTask(taskId, srcRack, srcCol, srcRow, srcStation, destRack, destCol, destRow, destStation, 2);
                }
            }
            return false;
        }
        /// <summary>
        /// 移库
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="srcRack"></param>
        /// <param name="srcCol"></param>
        /// <param name="srcRow"></param>
        /// <param name="srcStation"></param>
        /// <param name="destRack"></param>
        /// <param name="destCol"></param>
        /// <param name="destRow"></param>
        /// <param name="destStation"></param>
        /// <returns></returns>
        public bool WriteMove(int taskId, int srcRack, int srcCol, int srcRow, int srcStation, int destRack, int destCol, int destRow, int destStation)
        {
            if (plc == null)
                return false;
            lock (plc)
            {

                //3站内中转
                if (isConnected)
                {
                    errorMsg = "";
                    return WriteTask(taskId, srcRack, srcCol, srcRow, srcStation, destRack, destCol, destRow, destStation, 3);
                }
            }
            return false;
        }
        private bool WriteTask(int taskId, int srcRack, int srcCol, int srcRow, int srcStation, int destRack, int destCol, int destRow, int destStation, int boot)
        {
            if (!WriteDBXX(stackerConfig.dbTaskId, taskId))
            {
                errorMsg = "写任务号失败>>dbTaskId:" + stackerConfig.dbTaskId + ",值:" + taskId;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbSrcRack, srcRack))
            {
                errorMsg = "写起点排失败>>dbSrcRack:" + stackerConfig.dbSrcRack + ",值:" + srcRack;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbSrcCol, srcCol))
            {
                errorMsg = "写起点列失败>>dbSrcCol:" + stackerConfig.dbSrcCol + ",值:" + srcCol;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbSrcRow, srcRow))
            {
                errorMsg = "写起点层失败>>dbSrcRow:" + stackerConfig.dbSrcRow + ",值:" + srcRow;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbSrcStation, srcStation))
            {
                errorMsg = "写起点站台失败>>dbSrcStation:" + stackerConfig.dbSrcStation + ",值:" + srcStation;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbDestRack, destRack))
            {
                errorMsg = "写终点排失败>>dbDestRack:" + stackerConfig.dbDestRack + ",值:" + destRack;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbDestCol, destCol))
            {
                errorMsg = "写终点列失败>>dbDestCol:" + stackerConfig.dbDestCol + ",值:" + destCol;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbDestRow, destRow))
            {
                errorMsg = "写终点层失败>>dbDestRow:" + stackerConfig.dbDestRow + ",值:" + destRow;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbDestStation, destStation))
            {
                errorMsg = "写终点站台失败>>dbDestStation:" + stackerConfig.dbDestStation + ",值:" + destStation;
                return false;
            }
            if (!WriteDBXX(stackerConfig.dbBoot, boot))
            {
                errorMsg = "写启动标志失败>>dbBoot:" + stackerConfig.dbBoot + ",值:" + boot;
                return false;
            }
            //清除完成状态、写失败没关系PLC还会进行一次清除操作。
            WriteDBXX(stackerConfig.dbTaskStatus, 0);
            //任务写入成功，给对象赋值任务号
            this.TaskId = taskId;
            this.StartDate = DateTime.Now;//每次任务写入成功开始重新赋值
            return true;
        }

    }
}
