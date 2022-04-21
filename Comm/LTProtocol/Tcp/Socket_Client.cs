//using LTLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LTProtocol.Tcp
{
    //39.108.248.231 20002
    public class Socket_Client
    {
        //接收绑定事件
        public delegate void ReceiveDelegate(Socket_Client socket, string json);
        public event ReceiveDelegate onReceiveHandler;
        public delegate void DisConnectDelegate(string ErrorMessage);
        public event DisConnectDelegate onDisConnectHandler;
        public delegate void LogDelegate(string log);
        public event LogDelegate onLogHandler;
        //---------------------------------------------- 
        /// <summary>
        /// 退出socket所有服务线程
        /// </summary>
        private bool ExitSocketClientServer;
        public bool IsConnected = false;//已建立连接
        private Socket socket;
        private string _serverip = "";
        private int _port = 0;
        private SocketClientEncoding encoding { get; set; }
        public Socket_Client(string ServerIp, int Port, SocketClientEncoding encoding)
        {
            this.encoding = encoding;
            _serverip = ServerIp;
            _port = Port;
        }
        public void LogV(string log)
        {
            if (onLogHandler != null)
            {
                onLogHandler(log);
            }
        }
        Thread recTh;
        Thread failResend;
        Thread heatBeatTh;
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            if (heatBeatTh != null)
            {
                try
                {
                    heatBeatTh.Abort();
                    heatBeatTh.Join();
                }
                catch (Exception ex)
                {
                    LogV("终止心跳包异常：" + ex.ToString());
                }
            }
            if (failResend != null)
            {
                try
                {
                    failResend.Abort();
                    failResend.Join();
                }
                catch (Exception ex)
                {
                    LogV("终止Socket_Server--->>>ReSendAll线程异常:" + ex.Message);
                }
            }
            if (recTh != null)
            {
                try
                {
                    recTh.Abort();
                    recTh.Join();
                }
                catch (Exception ex)
                {
                    LogV("终止Socket_Client--->>>RecMsg接收线程异常:" + ex.Message);
                }
            }
            IPAddress ipAddr = IPAddress.Parse(_serverip);
            //LogV("error:92103 _serverip==>>>" + _serverip);
            //LogV("error:92104 _port==>>>" + _port);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, _port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool logout = false;//如果持续重连没有成功，则不输出日志
            while (true)
            {
                try
                {
                    //如果连接不成功则，一直尝试连接。。。直到连接成功为止
                    socket.Connect(endPoint);  //与远程主机建立连接  
                    break;
                }
                catch (Exception ex)
                {
                    if (!logout)
                    {
                        LogV("【连接超时：" + ex.ToString() + "】");
                        logout = true;
                    }
                }
                System.Threading.Thread.Sleep(5000);
            }
            IsConnected = true;
            LogV("成功连接到:" + socket.RemoteEndPoint);
            recTh = new Thread(RecMsg);
            recTh.IsBackground = true;
            recTh.Start();
            //重发失败数据线程 
            failResend = new Thread(ReSendAll);
            failResend.IsBackground = true;
            failResend.Start();
            //发送心跳包
            heatBeatTh = new Thread(HeatBeating);
            heatBeatTh.IsBackground = true;
            heatBeatTh.Start();
        }

        private Object errorlock = new object();
        public void onErrorRestart(string errormessage)
        {
            lock (errorlock)
            {
                LogV("onErrorRestart[133101]：" + errormessage);
                if (IsConnected)
                {
                    try
                    {
                        IsConnected = false;
                        //关闭socket
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Disconnect(false);
                        socket.Close();
                        socket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogV("关闭socket异常：" + ex.Message);
                    }
                    finally
                    {
                        if (onDisConnectHandler != null)
                        {
                            onDisConnectHandler(errormessage);
                        }
                    }
                }
            }
        }
        private void RecMsg()
        {
            while (true)
            {
                try
                {
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    List<byte> lstArr = new List<byte>();
                    byte[] buffer = new Byte[1024];
                    int len = 0;
                    while ((len = socket.Receive(buffer)) > 0)
                    {
                        if (ExitSocketClientServer)
                        {
                            break;
                        }
                        for (int i = 0; i < len; i++)
                        {
                            if (ExitSocketClientServer)
                            {
                                break;
                            }
                            lstArr.Add(buffer[i]);
                        }
                        //处理粘包/拆包数据 
                        lstArr = getRemaining(lstArr);
                    }
                    if (len == 0)
                    {//远程关闭连接 
                        onErrorRestart("远程连接已关闭");
                        break;
                    }
                }
                catch (Exception ex)
                { //异常终止接收数据  
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    onErrorRestart("RecMsg接收数据出现未知异常，终止连接：" + ex.Message);
                    break;
                }
                if (ExitSocketClientServer)
                {
                    break;
                }
                System.Threading.Thread.Sleep(1000);
                LogV("RecMsg>>>>>意外跳出循环...");
            }
        }
        private List<byte> getRemaining(List<byte> lstArr)
        {
            if (lstArr.Count == 0)
                return lstArr;
            List<byte> newbyte = new List<byte>();
            int _lasindex = 0;
            for (int i = 0; i < lstArr.Count; i++)
            {
                if (lstArr[i] == 3 && i > 0 && lstArr[i - 1] == 125)
                {////结束字符 //125 	 0x7D 	 }
                    _lasindex = i;
                    break;
                }
            }
            if (_lasindex == 0)
                return lstArr;
            string _str = "";
            //截取字符串
            for (int i = 0; i < lstArr.Count; i++)
            {
                if (lstArr[i] == 2 && lstArr.Count > (i + 1) && lstArr[i + 1] == 123)
                {//去掉02 和03
                    if (encoding == SocketClientEncoding.GB2312)
                    {
                        _str = Encoding.GetEncoding("GB2312").GetString(lstArr.GetRange(i + 1, _lasindex - i - 1).ToArray());
                    }
                    else
                    {
                        _str = System.Text.Encoding.UTF8.GetString(lstArr.GetRange(i + 1, _lasindex - i - 1).ToArray());
                    }
                    break;
                }
            }
            if ((lstArr.Count - _lasindex - 1) > 0)
            {
                newbyte.AddRange(lstArr.GetRange(_lasindex + 1, lstArr.Count - _lasindex - 1));
            }
            //处理数据  
            // LogV(_str);
            if (onReceiveHandler != null)
            {
                onReceiveHandler(this, _str);
            }
            return getRemaining(newbyte);
        }
        private List<string> Send_JsonData = new List<string>();
        private object sendSync=new object();
        /// <summary>
        /// 心跳包
        /// </summary>
        private void HeatBeating()
        {
            byte[] buffer = GetWrappedTextArray("{100}");
            while (true)
            {
                try
                {
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    try
                    {
                        if (!IsConnected)
                        {
                            break;
                        }
                        if (socket != null)
                        {
                            // bool blockingState = socket.Blocking;
                            try
                            {
                                byte[] tmp = new byte[1]; 
                                lock (sendSync)
                                {
                                    //临时禁用心跳
                                  /*  socket.Send(buffer, buffer.Length, SocketFlags.None);*/
                                } 
                            }
                            catch (SocketException e)
                            {
                                if (ExitSocketClientServer)
                                {
                                    break;
                                }
                                if (e.NativeErrorCode.Equals(10035))
                                {// 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                                 //return true;
                                }
                                else
                                {  //  return false;
                                    onErrorRestart("心跳失败、断开重连.。。。");
                                }
                            }
                            finally
                            {
                                // socket.Blocking = blockingState;    // 恢复状态
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ExitSocketClientServer)
                        {
                            break;
                        }
                        LogV("心跳包发送失败：==>>" + ex);
                    }
                    //每隔5秒发送心跳包
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    LogV("心跳包发送失败22：==>>" + ex);
                }
            }
        }

        /// <summary>
        /// 终止所有启动的线程
        /// </summary>
        public void Abort()
        {
            try
            {
                //lock (this)
                //{
                    ExitSocketClientServer = true;
                    if (recTh != null)
                    {
                        recTh.Abort();
                    }
                    if (failResend != null)
                    {
                        failResend.Abort();
                    }
                    if (heatBeatTh != null)
                    {
                        heatBeatTh.Abort();
                    }
                //}
            }
            catch (Exception ex)
            {
                LogV("Socket_Client--Abort 终止线程异常>>>" + ex.ToString());
            }
        }
        /// <summary>
        /// 重发失败数据
        /// </summary>
        private void ReSendAll()
        {
            while (true)
            {
                try
                {
                    if (ExitSocketClientServer)
                    {
                        break;
                    }
                    if (!IsConnected)
                    {
                        break;
                    }
                    if (Send_JsonData != null && Send_JsonData.Count > 0)
                    {//有发送失败的数据，则继续发送
                        lock (Send_JsonData)
                        {
                            List<string> _templst = new List<string>();
                            LogV("1332111失败重发===>>>Send_JsonData.Count=" + Send_JsonData.Count);
                            for (int i = 0; i < Send_JsonData.Count; i++)
                            {
                                if (ExitSocketClientServer)
                                {
                                    break;
                                }
                                LogV("3333231WMS-->>>WCS 失败重发。。。" + Send_JsonData[i]);
                                if (!Send(Send_JsonData[i]))
                                {
                                    _templst.Add(Send_JsonData[i]);
                                }
                            }
                            Send_JsonData.Clear();
                            if (_templst.Count > 0)
                            {//如果还是有发送失败的，则继续发送
                                Send_JsonData.AddRange(_templst.ToArray());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogV("[ReSendAll]失败重发失败：==>>" + ex);
                }
                if (ExitSocketClientServer)
                {
                    break;
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
        private object sendmesslock = new object();
        public void SendMessage(string json)
        {
            lock (sendmesslock)
            {//锁定 
                if (!Send(json))
                {
                    lock (Send_JsonData)
                    {
                        Send_JsonData.Add(json);
                    }
                    LogV("发送失败。。。保存待发送 Send_JsonData.count:" + Send_JsonData.Count + ", json:" + json);
                }
            }
        }
        /// <summary>
        /// 发送消息,成功返回true，失败返回false
        /// </summary>
        /// <param name="mess"></param>
        private bool Send(string json)
        {
            try
            {
                if (IsConnected)
                {
                    byte[] buffer = GetWrappedTextArray(json);
                    lock (sendSync)
                    {
                        socket.Send(buffer, buffer.Length, SocketFlags.None);
                    }
                    LogV("WMS>>>WCS====>>>" + json);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogV("调用 Send 发送数据异常：" + ex.Message);
                onErrorRestart("RecMsg接收数据出现未知异常，终止连接：" + ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 0x02{}0x03
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private byte[] GetWrappedTextArray(string text)
        {
            //   Encoding.GetEncoding("BG2312").GetString(new byte[1] { 2 }); 
            byte[] oldbytes;
            if (encoding == SocketClientEncoding.GB2312)
            {
                oldbytes = Encoding.GetEncoding("GB2312").GetBytes(text);
            }
            else
            {
                oldbytes = Encoding.UTF8.GetBytes(text);
            }
            byte[] newbytes = new byte[oldbytes.Length + 2];
            newbytes[0] = 2;
            oldbytes.CopyTo(newbytes, 1);
            newbytes[newbytes.Length - 1] = 3;
            return newbytes;
        }
    }
    public enum SocketClientEncoding
    {
        GB2312,
        UTF8
    }
}
