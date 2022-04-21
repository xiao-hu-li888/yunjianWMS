using LTWMSEFModel.Warehouse;
using LTWMSService.Warehouse;
using LTWMSWebMVC.App_Start.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.App_Start.Services
{
    public class InterfaceServerCheckService
    {
        LTWMSEFModel.LTModel dbmodel;
        LogToDb logtodb = null;
        //LTService.Hardware.hdw_agv_taskqueueBLL bll_hdw_agv_taskqueue;
        //LTService.Hardware.hdw_agv_task_mainBLL bll_hdw_agv_task_main;
        wh_service_statusBLL bll_wh_service_status;
        public InterfaceServerCheckService()
        {
            dbmodel = new LTWMSEFModel.LTModel();
            logtodb = new LogToDb(new LTWMSService.Logs.log_sys_alarmBLL(dbmodel), new LTWMSService.Logs.log_sys_executeBLL(dbmodel));
            //  bll_hdw_agv_taskqueue = new LTService.Hardware.hdw_agv_taskqueueBLL(dbmodel);
            //  bll_hdw_agv_task_main = new LTService.Hardware.hdw_agv_task_mainBLL(dbmodel);
            bll_wh_service_status = new wh_service_statusBLL (dbmodel);
        }
        public void ResetDbconnection()
        {
            dbmodel = new LTWMSEFModel.LTModel();
        }
        /// <summary>
        /// 判断agv服务是否可用，如果不可用，则不处理
        /// </summary>
        /// <returns></returns>
        public bool CheckAgvServerConnected()
        {
            return LTLibrary.HttpRequestHelper.CheckPageUrl(WMSFactory.Config.AgvApi_TestConnect);
            //  return LTLibrary.HttpRequestHelper.CheckPageUrl(WMSFactory.Config.AgvServerHttpURL + ":" + WMSFactory.Config.AgvServerPort + "/");
        }
        /// <summary>
        /// 服务状态改变
        /// </summary>
        /// <param name="status"></param>
        public void AddStateChange(LTWMSEFModel.Warehouse.WcsStatus status)
        {
            try
            {
                int randDiff = new Random().Next(1, int.MaxValue);
                if (status == LTWMSEFModel.Warehouse.WcsStatus.Connected)
                {
                    logtodb.DbExecuteLog("艾华接口服务连接成功...", randDiff);
                }
                else
                {
                    logtodb.DbExecuteLog("与艾华接口服务断开连接...", randDiff);
                }
                var _whWcs = bll_wh_service_status.GetFirstDefault(w => w.wcstype ==  LTWMSEFModel.Warehouse.WCSType.Agv);
                if (_whWcs != null)
                {//修改
                    _whWcs.wcs_status = status;
                    _whWcs.ip = WMSFactory.Config.AgvApi_TestConnect;// WMSFactory.Config.AgvServerHttpURL;
                    _whWcs.port = 0;// WMSFactory.Config.AgvServerPort;
                    _whWcs.updatedate = DateTime.Now;
                    bll_wh_service_status.Update(_whWcs);
                }
                else
                {//新增
                    _whWcs = new wh_service_status()
                    {
                        createdate = DateTime.Now,
                        guid = Guid.NewGuid(),
                        createuser = "wms-web",
                        ip = WMSFactory.Config.AgvApi_TestConnect,// WMSFactory.Config.AgvServerHttpURL,
                        port = 0,//WMSFactory.Config.AgvServerPort, 
                        desc = "艾华接口服务",
                        state = LTWMSEFModel.EntityStatus.Normal,
                        wcstype = LTWMSEFModel.Warehouse.WCSType.Agv,
                        wcs_status = status
                    };
                    bll_wh_service_status.Add(_whWcs);
                }
            }
            catch (System.InvalidOperationException inverr)
            {
                ResetDbconnection();
                WMSFactory.Log.v("艾华接口 AddStateChange 异常1111111111"+ inverr.ToString());
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v("艾华接口 AddStateChange 异常>>>" + ex);
            }
        }
    }
}