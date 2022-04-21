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
    public class Socket_Server
    { //接收绑定事件
        public delegate void ReceiveDelegate(Socket_Server socket, string socketKey, string json);
        public event ReceiveDelegate onReceiveHandler;
        public delegate void LogDelegate(string log);
        public event LogDelegate onLogHandler;
        public delegate void onDisConnectDelegate(string socketKey);
        public event onDisConnectDelegate onDisConnectHandler;
        /// <summary>
        /// Socket服务
        /// </summary>
        public Socket socketSrv;
        private string _serverip = "";
        private int _port = 0;
        public bool IsStartListen = false;//是否启动
        /// <summary>
        /// 退出socket所有服务线程
        /// </summary>
        private bool ExitSocketClientServer;
        /**************Log********************/
        public void LogV(string log)
        {
            if (onLogHandler != null)
            {
                onLogHandler(log);
            }
        }
        //---------------------------------------------- 
        List<ClientSocketObj> clientConnectionItems = new List<ClientSocketObj>();
        List<Thread> threadList = new List<Thread>();
        private SocketClientEncoding encoding { get; set; }
        public Socket_Server(string ServerIp, int Port, SocketClientEncoding encoding)
        {
            this.encoding = encoding;
            _serverip = ServerIp;
            _port = Port;
        }
        Thread mainTrd;
        Thread failResend;
        public void StartListen(Socket LastSocketSrv)
        {
            if (LastSocketSrv != null)
            {//关闭上一次的Socket服务
                try
                {
                    LastSocketSrv.Close();
                }
                catch (Exception ex)
                {
                    LogV("关闭上一次打开的Socket服务失败！异常69444190002：" + ex.Message);
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
            if (mainTrd != null)
            {
                try
                {
                    mainTrd.Abort();
                    mainTrd.Join();
                }
                catch (Exception ex)
                {
                    LogV("终止Socket_Server--->>>RunAccept线程异常:" + ex.Message);
                }
            }
            IPAddress ip = IPAddress.Parse(_serverip);
            IPEndPoint ipe = new IPEndPoint(ip, _port);
            //先关闭打开的socket
            if (socketSrv != null)
            {
                try
                {
                    socketSrv.Close();
                    //socketSrv.Dispose();
                }
                catch (Exception ex)
                {
                    LogV("启动SocketServer前关闭已打开的端口异常：5599921111>>" + ex.ToString());
                }
            }
            socketSrv = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                try
                {
                    socketSrv.Bind(ipe);
                    socketSrv.Listen(10);
                    break;
                }
                catch (Exception ex)
                {
                    LogV(ex.ToString());
                }
                System.Threading.Thread.Sleep(3000);
            }
            IsStartListen = true;
            mainTrd = new Thread(RunAccept);
            mainTrd.IsBackground = true;
            mainTrd.Start(socketSrv);
            //失败重发线程启动
            failResend = new Thread(ReSendAll);
            failResend.IsBackground = true;
            failResend.Start();
        }
        private void RunAccept(object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                try
                {
                    try
                    {
                        if (ExitSocketClientServer)
                        {
                            break;
                        }
                        Socket connection = socket.Accept();
                        string remoteEndPoint = connection.RemoteEndPoint.ToString();
                        //Console.WriteLine("成功与" + remoteEndPoint + "客户端建立连接！\t\n");
                        var _exObj = clientConnectionItems.Find(w => w.key == remoteEndPoint);
                        if (_exObj != null)
                        {
                            clientConnectionItems.Remove(_exObj);
                        }
                        if (ExitSocketClientServer)
                        {
                            break;
                        }
                        ////(remoteEndPoint, connection);
                        clientConnectionItems.Add(new ClientSocketObj() { key = remoteEndPoint, socket = connection });
                        Thread thread = new Thread(new ParameterizedThreadStart(recv));
                        thread.IsBackground = true;
                        thread.Start(connection);
                        threadList.Add(thread); 
                    }
                    catch (Exception ex)
                    {
                        LogV(ex.ToString());
                        break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            CloseALL();
            IsStartListen = false;
        }
        /// <summary>
        /// 接收客户端发来的信息，客户端套接字对象
        /// </summary>
        /// <param name="socketclientpara"></param>    
        private void recv(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;
            // int _FNW = 0;
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
                    while ((len = socketServer.Receive(buffer)) > 0)
                    {
                        if (ExitSocketClientServer)
                        {
                            break;
                        }
                        //if (_FNW > 0)
                        //    _FNW = 0;
                        for (int i = 0; i < len; i++)
                        {
                            if (ExitSocketClientServer)
                            {
                                break;
                            }
                            lstArr.Add(buffer[i]);
                        }
                        //处理粘包/拆包数据 
                        lstArr = getRemaining(socketServer.RemoteEndPoint.ToString(), lstArr);
                    }
                    if (len == 0)
                    {
                        try
                        {
                            LogV(socketServer.RemoteEndPoint + "远程连接关闭");
                            //clientConnectionItems.Remove(socketServer.RemoteEndPoint.ToString());
                            var obj = clientConnectionItems.Find(w => w.key == socketServer.RemoteEndPoint.ToString());
                            if (obj != null)
                            {
                                clientConnectionItems.Remove(obj);
                            }
                            onDisConnectHandler(socketServer.RemoteEndPoint.ToString());
                            //关闭socket
                            socketServer.Shutdown(SocketShutdown.Both);
                            socketServer.Disconnect(false);
                            socketServer.Close();
                            socketServer.Dispose();
                            if (ExitSocketClientServer)
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogV("socket-srv-1:" + ex.ToString());
                        }
                        break;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        //异常终止接收数据
                        LogV(ex.ToString());
                        var obj = clientConnectionItems.Find(w => w.key == socketServer.RemoteEndPoint.ToString());
                        if (obj != null)
                        {
                            clientConnectionItems.Remove(obj);
                        }
                        onDisConnectHandler(socketServer.RemoteEndPoint.ToString());
                        //提示套接字监听异常   
                        socketServer.Shutdown(SocketShutdown.Both);
                        socketServer.Disconnect(false);
                        socketServer.Close();
                        socketServer.Dispose();
                    }
                    catch (Exception ex2)
                    {
                        LogV("socket-srv-1:" + ex2.ToString());
                    }
                    break;
                }
            }
        }

        private List<byte> getRemaining(string socketKey, List<byte> lstArr)
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
                {
                    //_str = System.Text.Encoding.UTF8.GetString(lstArr.GetRange(i, _lasindex - i + 1).ToArray());
                    //////_str = System.Text.Encoding.UTF8.GetString(lstArr.GetRange(i + 1, _lasindex - i - 1).ToArray());  //去掉02 和03
                    //////break;
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
            if (onReceiveHandler != null)
            {
                onReceiveHandler(this, socketKey, _str);
            }
            ////////////
            return getRemaining(socketKey, newbyte);
        }
        private List<string> Send_JsonData = new List<string>();
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
                    if (!IsStartListen)
                    {//监听断开 退出线程
                        break;
                    }
                    if (Send_JsonData != null && Send_JsonData.Count > 0)
                    {//有发送失败的数据，则继续发送 
                        if (clientConnectionItems == null || clientConnectionItems.Count == 0)
                        {
                            return;
                        }
                        lock (Send_JsonData)
                        {
                            List<string> _templst = new List<string>();
                            LogV("失败重发315===>>>Send_JsonData.Count=" + Send_JsonData.Count);
                            bool _send = true;
                            for (int i = 0; i < Send_JsonData.Count; i++)
                            {
                                if (ExitSocketClientServer)
                                {
                                    break;
                                }
                                LogV("WMS-->>>WCS 失败重发3666。。。" + Send_JsonData[i]);
                                if (_send && !Send(Send_JsonData[i]))
                                {
                                    _send = false;
                                    //_templst.Add(Send_JsonData[i]);
                                }
                                if (!_send)
                                {//发送失败后不再继续发送,保存待发送
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
        private object sendalllock = new object();
        /// <summary>
        /// 给客户端发送数据
        /// </summary>
        /// <param name="json"></param>
        public bool SendALL(string json)
        {
            lock (sendalllock)
            {//锁定socket
                if (clientConnectionItems == null || clientConnectionItems.Count == 0)
                {//如果客户端没有连接，则保存信息待发送
                    //Send_JsonData.Add(json);
                    return false;
                }
                return Send(json);
                /*  if (!Send(json))
                  {
                    //  Send_JsonData.Add(json);
                  }*/
            } 
        }
        private bool Send(string json)
        {
            try
            {
                byte[] buffer = GetWrappedTextArray(json);
                int success = 0;
                foreach (var item in clientConnectionItems)
                {
                    try
                    {
                        item.socket.Send(buffer, buffer.Length, SocketFlags.None);
                        LogV("发送数据：socket-server33215>>[" + item.key + "]====>>>" + json);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        LogV("socket-server 数据发送失败111:[" + item.key + "]" + json + "====>>>" + ex.ToString());
                    }
                }
                if (success > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogV("socket-server 数据发送失败222：" + json + "==>>>" + ex.ToString());
            }
            return false;
        }
        /*
        public void SendALL(string json)
        {
            try
            {
                if (clientConnectionItems != null && clientConnectionItems.Count > 0)
                { 
                    byte[] buffer = GetWrappedTextArray(json); 
                    foreach (var item in clientConnectionItems)
                    {
                        try
                        {
                            item.Value.Send(buffer, buffer.Length, SocketFlags.None);
                            LogV("socket-server3321>>" + item.Key + "====>>>" + json);
                        }
                        catch (Exception ex)
                        {
                            LogV(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogV(ex.ToString());
            }
        }
        */
        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void CloseALL()
        {
            try
            {
                LogV("CloseALL-......");
                if (clientConnectionItems != null && clientConnectionItems.Count > 0)
                {
                    foreach (var item in clientConnectionItems)
                    {
                        try
                        {
                            item.socket.Shutdown(SocketShutdown.Both);
                            item.socket.Disconnect(false);
                            item.socket.Close();
                            item.socket.Dispose();
                        }
                        catch (Exception ex)
                        {
                            LogV("关闭连接" + item.key + "出错：" + ex.Message);
                        }
                        // clientConnectionItems.Remove(item.key);
                    }
                    clientConnectionItems.Clear();
                }
                if (socketSrv != null)
                {
                    try
                    {
                        socketSrv.Shutdown(SocketShutdown.Both);
                        socketSrv.Disconnect(false);
                        socketSrv.Close();
                        socketSrv.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogV("关闭连接socketSrv出错：" + ex.Message);
                    }
                }
                //关闭所有线程
                if (threadList != null && threadList.Count > 0)
                {
                    foreach (var item in threadList)
                    {
                        try
                        {
                            item.Abort();
                            item.Join();
                        }
                        catch (Exception ex)
                        {
                            LogV("终止Socket_Server--->>>RunAccept线程异常:" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogV(ex.ToString());
            }
            
        }
        /// <summary>
        /// 0x02{}0x03
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private byte[] GetWrappedTextArray(string text)
        {
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
        public bool IsSocketConnected(Socket client)
        {
            bool blockingState = false;
            try
            {
                blockingState = client.Blocking;
            }
            catch (Exception ex)
            {

            }
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                int cnt = client.Send(tmp, 1, SocketFlags.None);
                return true;
            }
            catch (SocketException ex)
            {
                if (ex.NativeErrorCode.Equals(10035))
                {//仍然连接，但是被阻止
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                try
                {
                    client.Blocking = blockingState;
                }
                catch (Exception ex)
                {

                }
            }
            return false;
        }
        /// <summary>
        /// 返回客户端连接
        /// </summary>
        /// <returns></returns>
        public string GetClients()
        {
            string str = "";
            if (clientConnectionItems != null && clientConnectionItems.Count > 0)
            {
                List<string> delLis = new List<string>();
                //保留最后一个socket 其它一律关闭连接并删除
                //   ClientSocketObj lastObj = clientConnectionItems[clientConnectionItems.Count - 1];
                while (clientConnectionItems.Count > 1)
                {
                    try
                    {
                        ClientSocketObj fistObj = clientConnectionItems[0];
                        clientConnectionItems.RemoveAt(0);
                        fistObj.socket.Shutdown(SocketShutdown.Both);
                        fistObj.socket.Disconnect(false);
                        fistObj.socket.Close();
                        fistObj.socket.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogV(ex.ToString());
                    }
                }
                //
                //为以后兼容多客户端保留。。。。。。。。
                //List<ClientSocketObj> lstC = new List<ClientSocketObj>(); 
                //lstC.Add(lastObj);
                foreach (var item in clientConnectionItems)
                {
                    try
                    {
                        //检测连通性 
                        // byte[] buffer = GetWrappedTextArray("");
                        // item.Value.Send(buffer, buffer.Length, SocketFlags.None);  
                        /***********************/
                        if (IsSocketConnected(item.socket))
                        {
                            str += "WCS远程已连接：" + item.key + ";";
                        }
                        else
                        {//断开删除连接
                            if (!delLis.Contains(item.key))
                            {
                                delLis.Add(item.key);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogV("ex1:>>" + ex.ToString());
                        //  delLis.Add(item.Key);
                        if (!delLis.Contains(item.key))
                        {
                            delLis.Add(item.key);
                        }
                    }
                }
                if (delLis.Count > 0)
                {
                    foreach (var item in delLis)
                    {
                        try
                        {
                            var sok = clientConnectionItems.Find(w => w.key == item);
                            //删除断开的连接
                            if (sok != null)
                            {
                                clientConnectionItems.Remove(sok);
                                sok.socket.Shutdown(SocketShutdown.Both);
                                sok.socket.Disconnect(false);
                                sok.socket.Close();
                                sok.socket.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogV("ex3:>>" + ex.ToString());
                        }
                    }
                }
            }
            return str;
        }

        public void Abort(Socket LastSocketSrv)
        {
            try
            {
                lock (this)
                {
                    ExitSocketClientServer = true;
                    CloseALL();
                    if (LastSocketSrv != null)
                    {//关闭上一次的Socket服务
                        try
                        {
                            LastSocketSrv.Close();
                        }
                        catch (Exception ex)
                        {
                            LogV("关闭上一次打开的Socket服务失败！异常6948888542252：" + ex.Message);
                        }
                    }
                    try
                    {
                        if (mainTrd != null)
                        {
                            mainTrd.Abort();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        if (failResend != null)
                        {
                            failResend.Abort();
                        }
                    }
                    catch (Exception ex)
                    {

                    } 
                }
            }
            catch (Exception ex)
            {
                LogV("Socket_Client--Abort 终止线程异常>>>" + ex.ToString());
            }
        }
    }
    /*
    public class ClientSocketObj
    {
        /// <summary>
        /// ip+端口
        /// </summary>
        public string key;
        /// <summary>
        /// 连接socket
        /// </summary>
        public Socket socket;
    }*/
}
