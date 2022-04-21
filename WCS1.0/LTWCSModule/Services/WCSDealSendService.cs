using LTWCSService.ApplicationService.WcsServer;
using LTWCSService.ApplicationService.WcsServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSModule.Services
{
    public class WCSDealSendService : BaseService
    {

        /// <summary>
        /// Wcs服务运行中
        /// </summary>
        public void WCSStarted()
        {
            //兼容以后写数据库，修改状态
            //wcs服务已启动
            Services.WcsServiceFactory.Log.v("Wcs 服务已启动...");
        }
        /// <summary>
        /// Wcs服务停止
        /// </summary>
        public void WCSStoped()
        {
            Services.WcsServiceFactory.Log.v("Wcs 服务已停止...");
            //var winSrv = bll_wh_wcs.GetFirstDefault(w => w.wcstype == WCSType.WCSWinServer);
            //if (winSrv != null && winSrv.guid != Guid.Empty)
            //{
            //    winSrv.wcs_status = WcsStatus.Disconnected;
            //    winSrv.updatedate = DateTime.Now;
            //    bll_wh_wcs.Update(winSrv);
            //}
        }
        public void AddStateChange(bool status)
        {
            //int randDiff = new Random().Next(1, int.MaxValue);
            if (status)
            {
                Services.WcsServiceFactory.Log.v("与wms连接成功...");
            }
            else
            {
                Services.WcsServiceFactory.Log.v("与wms断开连接...");
            }
        }
        /// <summary>
        /// 每隔3秒推送设备状态
        /// </summary>
        /// <param name="SocketClient"></param>
        public void SendDeviceStatus(LTProtocol.Tcp.Socket_Client SocketClient, StackerService stackerSrv)
        {
            int code = -1;//PLC未连接
            if (stackerSrv != null && stackerSrv.GetConnected())
            {//连接PLC成功，发送设备状态
                code = (int)stackerSrv.GetStatus();
            }
            SendDeviceStatus sendObj = new SendDeviceStatus();
            sendObj.seq = Seq;
            sendObj.dev_info = new List<DevInfo>();
            sendObj.dev_info.Add(new DevInfo() { dev_id = 2001, error_code = 0, error_msg = "", status = code });
            //获取站台出库准备好信号
            if (stackerSrv != null && stackerSrv.GetConnected())
            {
                for (int i = 1; i < 3; i++)
                {
                    var readyState = stackerSrv.GetReadyOutOfStation(i);
                    if (readyState == DeviceStatusEnum.Ready || readyState == DeviceStatusEnum.None)
                    {//只有这准备好、无  两种状态才发送
                        sendObj.dev_info.Add(new DevInfo() { dev_id = i * 100, error_code = 0, error_msg = "", status = (int)readyState });
                    }
                }
            }
            SocketClient.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(sendObj));
        }
        /// <summary>
        /// 发送托盘条码请求入库
        /// </summary>
        /// <param name="SocketClient"></param>
        public void SendTrayBarcode(LTProtocol.Tcp.Socket_Client SocketClient)
        {

        }
    }
}
