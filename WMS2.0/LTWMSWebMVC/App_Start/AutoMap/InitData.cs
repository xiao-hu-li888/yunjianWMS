using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.AutoMap
{
    /// <summary>
    /// 初始化测试数据。。。
    /// </summary>
    public class InitData
    {
        LTWMSService.Basic.sys_table_idBLL bll_sys_table_id;
        LTWMSService.Basic.sys_loginBLL bll_sys_login;
        LTWMSService.Hardware.hdw_stacker_taskqueueBLL bll_stacker_taskqueue;
        LTWMSService.Logs.log_sys_alarmBLL bll_alarm_log;
      //  LTWMSService.Hardware.hdw_agv_task_mainBLL bll_hdw_agv_task_main;
       // LTWMSService.Hardware.hdw_agv_taskqueueBLL bll_hdw_agv_taskqueue;
        public InitData(LTWMSService.Basic.sys_loginBLL bll_sys_login, LTWMSService.Hardware.hdw_stacker_taskqueueBLL bll_stacker_taskqueue,
             LTWMSService.Logs.log_sys_alarmBLL bll_alarm_log, LTWMSService.Basic.sys_table_idBLL bll_sys_table_id)
        {
            this.bll_sys_login = bll_sys_login;
            this.bll_stacker_taskqueue = bll_stacker_taskqueue;
            this.bll_alarm_log = bll_alarm_log;
            //this.bll_hdw_agv_taskqueue = bll_hdw_agv_taskqueue;
            //this.bll_hdw_agv_task_main = bll_hdw_agv_task_main;
            this.bll_sys_table_id = bll_sys_table_id;
        }
        public void AddAlarmData()
        {
            for (int i = 0; i < 50; i++)
            {
                bll_alarm_log.Add(new LTWMSEFModel.Logs.log_sys_alarm()
                {
                    guid = Guid.NewGuid(),
                    is_popup = null,
                    log_date = DateTime.Now,
                    log_from = LTWMSEFModel.Logs.AlarmFrom.Stacker,
                    remark = "测试报警日志！！！测试报警日志！！！测试报警日志！！！测试报警日志！！！测试报警日志！！！测试报警日志！！！测试" + (i + 1)
                });
            }

        }
        /// <summary>
        /// 初始化货架信息
        /// </summary>
        public void InitShelfInfo()
        {

        }
        /// <summary>
        /// 添加几条测试任务
        /// </summary>
        public void AddAgvTaskQueue()
        {
            //入库
            AddAgvTaskQueue2(LTWMSEFModel.Hardware.WcsTaskType.StockIn);
            //出库
            AddAgvTaskQueue2(LTWMSEFModel.Hardware.WcsTaskType.StockOut);
        }
        private void AddAgvTaskQueue2(LTWMSEFModel.Hardware.WcsTaskType tasktype)
        {
            var taskQueue = new LTWMSEFModel.Hardware.hdw_stacker_taskqueue();
            taskQueue.createdate = DateTime.Now;

            taskQueue.guid = Guid.NewGuid();
            taskQueue.order = "T01";

            taskQueue.stacker_number = "1";
            taskQueue.startup = DateTime.Now;
            taskQueue.state = LTWMSEFModel.EntityStatus.Normal;

            if (tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockIn)
            {
                taskQueue.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.Holding;
                taskQueue.tray1_barcode = "151";
                taskQueue.dest_col = 1;
                taskQueue.dest_rack = 1;
                taskQueue.dest_row = 1;
                taskQueue.dest_shelfunits_guid = Guid.NewGuid();
                taskQueue.dest_shelfunits_pos = "1-1-1";
                taskQueue.dest_station = 1;
                taskQueue.tasktype = LTWMSEFModel.Hardware.WcsTaskType.StockIn;
            }
            else
            {
                taskQueue.taskstatus = LTWMSEFModel.Hardware.WcsTaskStatus.IsSend;
                taskQueue.tray1_barcode = "101";
                taskQueue.src_col = 2;
                taskQueue.src_rack = 2;
                taskQueue.src_row = 2;
                taskQueue.src_shelfunits_guid = Guid.NewGuid();
                taskQueue.src_shelfunits_pos = "2-2-2";
                taskQueue.tasktype = LTWMSEFModel.Hardware.WcsTaskType.StockOut;
            }

            taskQueue.tray1_matter_barcode1 = "T01-01-01";
            taskQueue.tray1_matter_barcode2 = "T01-01-02";
            bll_stacker_taskqueue.Add(taskQueue);
        }
        /// <summary>
        /// 添加admin账号
        /// </summary>
        public void AddLoginAdmin()
        {
            bll_sys_login.AddLogin(new LTWMSEFModel.Basic.sys_login()
            {
                createdate = DateTime.Now,
                createuser = "sys",
                gender = true,
                guid = Guid.NewGuid(),
                issuperadmin = true,
                username = "管理员",
                loginname = "admin",
                rowversion = 1,
                state = LTWMSEFModel.EntityStatus.Normal
            }, "123456");
        }
        public void AddAGVTaskInfo()
        {
            //给agv添加30条记录
            //T07-1-15
            // T07-4-15
         /*   var taskMain = bll_hdw_agv_task_main.GetFirstDefault(w => w.order == "T07");
            if (taskMain == null || taskMain.guid == Guid.Empty)
            {
                taskMain = new LTWMSEFModel.Hardware.hdw_agv_task_main();
                taskMain.createdate = DateTime.Now;
                taskMain.dest_rack = "R5";
                taskMain.guid = Guid.NewGuid();
                taskMain.order = "T07";
                taskMain.rec_status = LTWMSEFModel.Hardware.AgvReceiveStatus.WaitSend;
                taskMain.rowversion = 0;
                taskMain.src_rack = "P1";
                taskMain.state = LTWMSEFModel.EntityStatus.Normal;
                taskMain.task_execute_status = LTWMSEFModel.Hardware.AgvTaskExecuteStatus.None;
                taskMain.total_count = 30;
                taskMain.total_success = 0;
                bll_hdw_agv_task_main.Add(taskMain);
            }
            else
            {
                taskMain.task_execute_status = LTWMSEFModel.Hardware.AgvTaskExecuteStatus.None;
                taskMain.rec_status = LTWMSEFModel.Hardware.AgvReceiveStatus.WaitSend;
                taskMain.total_count = 30;
                taskMain.total_success = 0;
                bll_hdw_agv_task_main.Update(taskMain);
            }
            var dellist = bll_hdw_agv_taskqueue.GetAllQuery();
            if (dellist != null && dellist.Count > 0)
            {
                foreach (var item in dellist)
                {
                    bll_hdw_agv_taskqueue.Delete(item);
                }
            }
            List<LTWMSEFModel.Hardware.hdw_agv_taskqueue> lst1 = new List<LTWMSEFModel.Hardware.hdw_agv_taskqueue>();
            for (int i = 0; i < 15; i++)
            {
                var MM = new LTWMSEFModel.Hardware.hdw_agv_taskqueue();
                MM.agv_task_main_guid = taskMain.guid;
                MM.createdate = DateTime.Now;
                MM.id = bll_sys_table_id.GetId( LTWMSService.Basic.sys_table_idBLL.TableIdType.hdw_agv_taskqueue);
                MM.dest_point = "R5";
                MM.src_point = "P1";
                MM.guid = Guid.NewGuid();
                MM.matter_barcode1 = "T07-1-"+(i+1);
                MM.order = "T07";
                MM.task_status = LTWMSEFModel.Hardware.AgvTaskStatus.Holding;
                MM.state = LTWMSEFModel.EntityStatus.Normal; 
                lst1.Add(MM);
            }
            bll_hdw_agv_taskqueue.AddRange(lst1);
            ////////////
            List<LTWMSEFModel.Hardware.hdw_agv_taskqueue> lst2 = new List<LTWMSEFModel.Hardware.hdw_agv_taskqueue>();
            for (int i = 0; i < 15; i++)
            {
                var MM = new LTWMSEFModel.Hardware.hdw_agv_taskqueue();
                MM.agv_task_main_guid = taskMain.guid;
                MM.createdate = DateTime.Now;
                MM.dest_point = "R5";
                MM.src_point = "P1";
                MM.id = bll_sys_table_id.GetId(LTWMSService.Basic.sys_table_idBLL.TableIdType.hdw_agv_taskqueue);
                MM.guid = Guid.NewGuid();
                MM.matter_barcode1 = "T07-4-" + (i + 1);
                MM.order = "T07";
                MM.task_status = LTWMSEFModel.Hardware.AgvTaskStatus.Holding;
                MM.state = LTWMSEFModel.EntityStatus.Normal;
                lst2.Add(MM);
            }
            bll_hdw_agv_taskqueue.AddRange(lst2);*/
        }
    }
}