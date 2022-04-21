using LTWMSService.ApplicationService.WmsServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.Services
{
    public class BaseService
    {
        //WCS配置信息
        // public Guid WareHouseGuid;
        /// <summary>
        /// wcs服务guid
        /// </summary>
        public Guid Wcs_srv_guid;
        /// <summary>
        /// wcs服务名称
        /// </summary>
        public string Wcs_srv_Name;
        public string Wcs_srv_Ip;
        public int Wcs_srv_Port;
        /////////////
        //public delegate void ResetDbModelDelegate(LTWMSEFModel.LTModel dbmodel);
        ///// <summary>
        ///// 重置DbModel事件
        ///// </summary>
        //public event ResetDbModelDelegate onResetDbModel;
        /// <summary>
        /// 操作用户为win服务
        /// </summary>
        protected string curr_username = "lt-winserver";
        /// <summary>
        /// wcs任务id+1000
        /// </summary>
     //   protected const int  _increatenumber = 1000;//任务id+1000
        private int _seq;
        public int Seq
        {
            get
            {
                if (_seq > 99999999)
                {//重置
                    _seq = 1;
                }
                return ++_seq;
            }
        }
        private LTWMSEFModel.LTModel dbmodel;
        /// <summary>
        /// 报警日志（一般数据量不大，全部保存，提供删除历史数据功能）
        /// </summary>
        private LTWMSService.Logs.log_sys_alarmBLL bll_sys_alarm_log;
        /// <summary>
        /// 执行日志（自动删除，只保留3-7天？）
        /// </summary>
        private LTWMSService.Logs.log_sys_executeBLL bll_sys_execute_log;
        public LTWMSService.Basic.sys_control_dicBLL bll_sys_control_dic;
        public LEDDisplay ledDisplay;
        public BaseService(Guid Wcs_srv_guid, string ServerName,string Wcs_srv_Ip,int Wcs_srv_Port)
        {
            // this.WareHouseGuid = WareHouseGuid;
            this.Wcs_srv_guid = Wcs_srv_guid;
            this.Wcs_srv_Name = ServerName;
            this.Wcs_srv_Ip = Wcs_srv_Ip;
            this.Wcs_srv_Port = Wcs_srv_Port;
            // WinServiceFactory.Log.v(DateTime.Now.ToString()+"初始化baseservice.......");
            //多线程下每个线程只创建一个DbContext
            dbmodel = new LTWMSEFModel.LTModel();
            bll_sys_alarm_log = new LTWMSService.Logs.log_sys_alarmBLL(dbmodel);
            bll_sys_execute_log = new LTWMSService.Logs.log_sys_executeBLL(dbmodel);
            bll_sys_control_dic = new LTWMSService.Basic.sys_control_dicBLL(dbmodel);
            //输出日志 
            // dbmodel.Database.Log = message => Services.WinServiceFactory.Log.v("LT-DBContext==>>" + message);
            ledDisplay = new LEDDisplay(Services.WinServiceFactory.Config.LedIp1, Services.WinServiceFactory.Config.LedIp2, Services.WinServiceFactory.Config.LedIp3, Services.WinServiceFactory.Config.LedIp4,
               Services.WinServiceFactory.Config.LedWidth, Services.WinServiceFactory.Config.LedHeight);
            ledDisplay.onLogHandler += LedDisplay_onLogHandler;
        }
        private void LedDisplay_onLogHandler(string logs)
        {
            Services.WinServiceFactory.Log.v(logs);
        }
        /// <summary>
        /// 获取数据库服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetSQLDateTime()
        {
            return bll_sys_control_dic.getServerDateTime();
        }
        public LTWMSEFModel.LTModel GetDbModel()
        {
            return dbmodel;
        }
        /// <summary>
        /// 错误日志记录至数据库
        /// </summary>
        public void DbExceptionLog(string log, LTWMSEFModel.Logs.AlarmFrom type)
        {
            try
            {
                //判断日志长度，超过500分多次写入
                bll_sys_alarm_log.Add(log, type);
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("DbLog 写日志失败1==>>>" + ex.ToString());
                Services.WinServiceFactory.Log.v(log);
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
                if (string.IsNullOrWhiteSpace(log))
                {//空值直接返回
                    return;
                }
                bll_sys_execute_log.Add(log, diff);
            }
            catch (Exception ex)
            {
                Services.WinServiceFactory.Log.v("DbExecuteLog 写日志失败2==>>>【" + ex.ToString() + "】");
                Services.WinServiceFactory.Log.v("log>>>【" + log + "】");
            }
        }
        /// <summary>
        /// 重置dbContext连接，释放并重新建立连接
        /// </summary>
        public void ResetDbModel()
        {
            //RemoveDBConnection();
            //  dbmodel = null; 
            dbmodel = new LTWMSEFModel.LTModel();
            //if (onResetDbModel != null)
            //{
            //    onResetDbModel(dbmodel);
            //}
        }
        /// <summary>
        /// 释放DbContext对象
        /// </summary>
        //public void RemoveDBConnection()
        //{
        //    try
        //    {
        //        dbmodel.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        Services.WinServiceFactory.Log.v("RemoveDBConnection 释放DbContext对象失败！");
        //        Services.WinServiceFactory.Log.v(ex);
        //    }
        //}

    }
}
