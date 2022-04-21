using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer
{
    /// <summary>
    /// 堆垛机配置参数
    /// </summary>
    public class StackerConfig
    {
        /// <summary>
        ///INT(16位) 堆垛机状态(1自动准备好 2运行 3故障)  状态为1可以下发启动命令
        ///DB80.DBW0
        /// </summary>
      public  string dbStackerStatus;
        /// <summary>
        ///INT(16位) 任务完成（1任务完成 0未完成）  每次任务完成后为1
        ///DB80.DBW2
        /// </summary>
        public string dbTaskStatus;
        /// <summary>
        /// INT(16位) 流程字（1货叉归中 2去取货点 3取货伸叉 4 取货抬起 5取货缩回 6去放货点 7放货伸叉 8放货下降 9放货缩回）
        /// DB80.DBW4
        /// </summary>
        public string dbFlow;

        /// <summary>
        ///INT(16位) 启动标志（0默认未启动 1入库 2出库 3站内中转）
        ///DB80.DBW50
        /// </summary>
        public string dbBoot;
        /// <summary>
        ///DINT(32位) 任务号（不等于0）
        ///DB80.DBD52
        /// </summary>
        public string dbTaskId;
        /// <summary>
        ///INT（16位） 起点排
        ///DB80.DBW56
        /// </summary>
        public string dbSrcRack;
        /// <summary>
        ///INT（16位） 起点列
        ///DB80.DBW58
        /// </summary>
        public string dbSrcCol;
        /// <summary>
        ///INT（16位） 起点层
        ///DB80.DBW60
        /// </summary>
        public string dbSrcRow;
        /// <summary>
        /// 起点站台（扩展用）
        /// </summary>
        public string dbSrcStation;
        /// <summary>
        ///INT（16位） 终点排
        ///DB80.DBW62
        /// </summary>
        public string dbDestRack;
        /// <summary>
        ///INT（16位） 终点列
        ///DB80.DBW64
        /// </summary>
        public string dbDestCol;
        /// <summary>
        ///INT（16位） 终点层
        ///DB80.DBW66
        /// </summary>
        public string dbDestRow;
        /// <summary>
        /// 终点站台（纵深1、2） 扩展用
        /// </summary>
        public string dbDestStation;
         
        /// <summary>
        /// 100出库准备好
        /// </summary>
        public string dbReady100; 
        /// <summary>
        /// 200出库准备好
        /// </summary>
        public string dbReady200; 
        /// <summary>
        /// 300出库准备好
        /// </summary>
        public string dbReady300; 
        /// <summary>
        /// 400出库准备好
        /// </summary>
        public string dbReady400; 
    }
}
