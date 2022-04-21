using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer
{
    /// <summary>
    /// S7协议基础类
    /// </summary>
    public class S7BaseService
    {
        public delegate void LogDelegate(string logs);
        public event LogDelegate onLogHandler;
        protected string Ip;
        protected CpuType cputype;
        public S7BaseService(string Ip, CpuType cputype)
        {
            this.Ip = Ip;
            this.cputype = cputype;
        }
        protected bool isConnected = false;
        protected Plc plc;
        protected void LogV(string log)
        {
            if (onLogHandler != null)
            {
                onLogHandler(log);
            }
        }
        bool logerr = false;
       // object conSync = new object();
        /// <summary>
        /// 返回连接状态
        /// </summary>
        /// <returns></returns>
        public bool GetConnected()
        {
            if (plc == null)
                return false;
            lock (plc)
            {
                if (isConnected && plc.IsConnected)
                {
                    return true;
                }
                isConnected = false;
            }
            return false;
        }
        public void Connect()
        {
            try
            {//首次连接/重连 确保之前的连接已关闭
                if (plc != null)
                {
                    plc.Close();
                    isConnected = false;
                }
            }
            catch (Exception ex)
            {

            }
            while (true)
            {
                //连接
                try
                {
                    //plc = new Plc(CpuType.S71200, Ip, 0, 1);
                    plc = new Plc(cputype, Ip, 0, 1);
                    plc.Open();
                    break;
                }
                catch (Exception ex)
                {
                    if (!logerr)
                    {
                        LogV("PLC[" + Ip + "]连接失败:5532071-->>>" + ex.ToString());
                        logerr = true;//输出一次则不再继续输出
                    }
                }
                System.Threading.Thread.Sleep(5000);
            }
            //修改堆垛机连接状态 
            isConnected = true;
            logerr = false;
            LogV("PLC[" + Ip + "]:连接成功");
            //Service.WinServiceFactory.Log.v("堆垛机PLC(" + Service.WinServiceFactory.Config.TransportPLC + "):连接成功");
        }
        public void Close()
        {
            try
            {
                if (plc != null)
                {
                    plc.Close();
                    isConnected = false;
                    LogV("PLC[" + Ip + "]:连接已关闭");
                }
            }
            catch (Exception ex)
            {
                LogV("PLC[" + Ip + "]连接失败:172862601-->>" + ex.ToString());
                //Service.WinServiceFactory.Log.v("关闭堆垛机PLC失败:172201-->>", ex);
            }
        }


        ////////////////
        protected bool WriteDBXX(string dbcmd, int val)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dbcmd))
                {//命令地址为空，默认写入成功
                    return true;
                }
                if (dbcmd.IndexOf("DBD") > 0)
                {//写入双字 32位
                    return WriteDBD(dbcmd, val);
                }
                else
                {//写入字 16位
                    return WriteDBW(dbcmd, val);
                }
            }
            catch (Exception ex)
            {
                LogV("WriteDBXX 异常=>>" + ex.ToString());
            }
            return false;
        }
        /// <summary>
        /// 写入双字（DINT）32位
        /// </summary>
        /// <param name="dbcmd">地址</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        protected bool WriteDBD(string dbcmd, int val)
        {
            if (string.IsNullOrWhiteSpace(dbcmd))
            {//判断命令地址是否为空
                ////命令地址为空，默认写入成功
                return true;
            }
            int idx = 0;
            do
            {
                try
                {
                    if (idx >= 3)
                    {//写3次不成功 直接返回
                        return false;
                    }
                    plc.Write(dbcmd, val);
                    //写完等待20毫秒再去读
                    System.Threading.Thread.Sleep(20);
                    int _idxsub = 0;
                    do
                    {//读取指令异常 3次失败跳出读取
                        try
                        {
                            if (_idxsub >= 3)
                            {//读取3次失败，退出循环
                                break;
                            }
                            if (Convert.ToInt32(plc.Read(dbcmd)) == val)
                            {//写入成功
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogV("WriteDBD 读取指令" + dbcmd + "出错>>>" + ex.ToString());
                        }
                        _idxsub++;
                        System.Threading.Thread.Sleep(20);
                    } while (true);
                }
                catch (Exception ex)
                {
                    LogV("WriteDBW 异常>>>" + ex.ToString());
                }
                idx++;
                LogV("写入" + dbcmd + ":" + val + "失败.第" + idx + "次");
                System.Threading.Thread.Sleep(20);
            } while (true);
            //  return true;
        }
        /// <summary>
        /// 写入字（INT） 16位
        /// </summary>
        /// <param name="dbcmd">地址</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        protected bool WriteDBW(string dbcmd, int val)
        {
            if (string.IsNullOrWhiteSpace(dbcmd))
            {//判断命令地址是否为空
                //命令地址为空，默认写入成功
                return true;
            }
            int idx = 0;
            do
            {
                try
                {
                    if (idx >= 3)
                    {//写3次不成功 直接返回
                        return false;
                    }
                    plc.Write(dbcmd, Convert.ToInt16(val));
                    //写完等待20毫秒再去读
                    System.Threading.Thread.Sleep(20);
                    int _idxsub = 0;
                    do
                    {//读取指令异常 3次失败跳出读取
                        try
                        {
                            if (_idxsub >= 3)
                            {//读取3次失败，退出循环
                                break;
                            }
                            if (Convert.ToInt32(plc.Read(dbcmd)) == val)
                            {//写入成功
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogV("WriteDBD 读取指令" + dbcmd + "出错>>>" + ex.ToString());
                        }
                        _idxsub++;
                        System.Threading.Thread.Sleep(20);
                    } while (true);
                }
                catch (Exception ex)
                {
                    LogV("WriteDBW 异常>>>" + ex.ToString());
                }
                idx++;
                LogV("写入" + dbcmd + ":" + val + "失败.第" + idx + "次");
                System.Threading.Thread.Sleep(20);
            } while (true);
            // return true;
        }
    }
}
