using LTLibrary;
using LTWMSEFModel.Warehouse;
using LTWMSModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSModule.ThreadSrv
{
    public class WMSSrv_DealReceive : IThreadService
    {
        public Guid Wcs_srv_guid;
        public string ServerName;
        Srv_DealReceive wcsdsServer;
        /// <summary>
        /// 服务终止线程退出标志
        /// </summary>
        private bool exitServer;
        public void OnStop()
        {
            exitServer = true;
            wcsdsServer.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Disconnected);
        }
        List<wh_warehouse> ListWareHouse;
        public void ThreadStart(object threadhandler)
        {
            exitServer = false;
            ThreadHandler thd = threadhandler as ThreadHandler;
            Services.WinServiceFactory.Log.v("启动线程：【" + thd.Name + "】");
            wcsdsServer = new Srv_DealReceive(thd.Wcs_srv_guid, thd.Name, thd.Ip, LTLibrary.ConvertUtility.ToInt(thd.Port));

            Wcs_srv_guid = thd.Wcs_srv_guid;
            ServerName = thd.Name;
            //根据wcs服务查找所有对应的仓库信息
            ListWareHouse = wcsdsServer.GetAllWareHouseByWcsSrvGuid(Wcs_srv_guid);
            while (true)
            {
                try
                {
                    if (exitServer)
                    {
                        break;
                    }
                    try
                    {
                        if (thd.Exit)
                        {
                            break;
                        }
                        wcsdsServer.AddStateChange(LTWMSEFModel.Warehouse.WcsStatus.Connected);
                        while (true)
                        {//递归执行任务 
                            try
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                if (thd.Exit)
                                {
                                    break;
                                }
                                foreach (wh_warehouse item in ListWareHouse)
                                {
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    if (thd.Exit)
                                    {
                                        break;
                                    }
                                    thd.lastBeginDate = DateTime.Now;
                                    /////////////////////// 
                                    wcsdsServer.HandlerReceivedMessage(item);
                                    /////////////////////// 
                                    if (exitServer)
                                    {
                                        break;
                                    }
                                    thd.lastEndDate = DateTime.Now;
                                    System.Threading.Thread.Sleep(30);
                                }
                            }
                            catch (System.InvalidOperationException inverr)
                            {
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.ResetDbModel();
                                Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:30555sljd11110", inverr);
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:30555sljd11110=>>" + inverr.ToString(), 0);
                            }
                            catch (Exception ex)
                            {
                                //wcsdsServer.ResetDbModel();
                                Services.WinServiceFactory.Log.v("异常:30455131lsdd2", ex);
                                if (exitServer)
                                {
                                    break;
                                }
                                wcsdsServer.DbExecuteLog("异常:30455131lsdd2=>>" + ex.ToString(), 0);
                            }

                            // System.Threading.Thread.Sleep(1000);
                            if (exitServer)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep(WinServiceFactory.Config.RefreshCycle);
                        }
                    }
                    catch (System.InvalidOperationException inverr)
                    {
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.ResetDbModel();
                        Services.WinServiceFactory.Log.v("数据库连接异常，已重新初始化dbContext:kkkk12333", inverr);
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.DbExecuteLog("数据库连接异常，已重新初始化dbContext:kkkk12333" + inverr.ToString(), 0);
                    }
                    catch (Exception ex)
                    {
                        Services.WinServiceFactory.Log.v("[" + thd.Name + "]异常ddd12336:", ex);
                        if (exitServer)
                        {
                            break;
                        }
                        wcsdsServer.DbExecuteLog("[" + thd.Name + "]异常ddd12336:" + ex.ToString(), 0);
                    }
                    if (exitServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception exx)
                {
                    Services.WinServiceFactory.Log.v("[" + thd.Name + "]异常212222222222221d7:", exx);
                }
            }
            thd.Exit = false;
            //意外退出
            thd.ThreadReboot = true;
        }
    }
}
