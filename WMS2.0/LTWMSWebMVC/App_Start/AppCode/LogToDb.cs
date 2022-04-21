using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.AppCode
{
    public class LogToDb
    {
        LTWMSService.Logs.log_sys_alarmBLL bll_sys_alarm_log;
        LTWMSService.Logs.log_sys_executeBLL bll_sys_execute_log;
        public LogToDb(LTWMSService.Logs.log_sys_alarmBLL bll_sys_alarm_log,
            LTWMSService.Logs.log_sys_executeBLL bll_sys_execute_log)
        {
            this.bll_sys_alarm_log = bll_sys_alarm_log;
            this.bll_sys_execute_log = bll_sys_execute_log;
        }
        /// <summary>
        /// 错误日志记录至数据库
        /// </summary>
        public void DbExceptionLog(string log, LTWMSEFModel.Logs.AlarmFrom type)
        {
            try
            {
                bll_sys_alarm_log.Add(log,type);
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v("DbLog 写日志失败111==>>>" + ex.ToString());
                WMSFactory.Log.v("log113>>>" + log);
            }
        }
        /// <summary>
        /// 执行日志记录至数据库
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="diff">辨别不同线程之间的调用</param>
        public void DbExecuteLog(string log, int diff)
        {
            try
            {
                bll_sys_execute_log.Add(log,diff);
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v("DbExecuteLog 写日志失败222==>>>" + ex.ToString());
                WMSFactory.Log.v("log221>>>"+log);
            }
        }
    }
}