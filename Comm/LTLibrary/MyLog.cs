using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace LTLibrary
{
    public class MyLog
    {
        private bool _disablelog = false;//默认启用日志
       // private ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        private string logFile;
        private string DirRoot;
        public MyLog(string webroot)
        {
            DirRoot = webroot;// Environment.CurrentDirectory; 
        }
        public void NewDirector()
        {
            logFile = DirRoot + "/Logs/" + DateTime.Now.ToString("yyyyMM")
              + GetWeek(DateTime.Now.Day) + "/" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            CreateDirectory(logFile);

        }
        /// <summary>
        /// 禁用日志
        /// </summary>
        /// <param name="enablefale"></param>
        public MyLog(bool enablefale)
        {
            _disablelog = true;
        }
        private int GetWeek(int day)
        {
            return day % 7 > 0 ? day / 7 + 1 : day / 7;
        }
        public void v(string message, Exception e)
        {
            if (_disablelog)
            {
                return;
            }
            v(message + e.ToString());
        }
        public void v(Exception e)
        {
            if (_disablelog)
            {
                return;
            }
            v(e.ToString());
        }
        object objLock = new object();
        public void v(string info)
        {
            lock (objLock)
            {
                if (_disablelog)
                {
                    return;
                }
                try
                {
                    // LogWriteLock.EnterWriteLock();
                    NewDirector();
                    System.IO.File.AppendAllText(logFile, DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "----->>>" + info + "\r\n");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //finally
            //{
            //    LogWriteLock.ExitWriteLock();
            //}
        }

        private void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}
