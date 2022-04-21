using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.WmsServer
{
    public class LEDDisplay
    {
        public delegate void LogDelegate(string logs);
        public event LogDelegate onLogHandler;
        string LedIp1;
        string LedIp2;
        string LedIp3;
        string LedIp4;
        int LedWidth;
        int LedHeight;
        public LEDDisplay(string LedIp1, string LedIp2, string LedIp3, string LedIp4, int LedWidth, int LedHeight)
        {
            this.LedIp1 = LedIp1;
            this.LedIp2 = LedIp2;
            this.LedIp3 = LedIp3;
            this.LedIp4 = LedIp4;
            this.LedWidth = LedWidth;
            this.LedHeight = LedHeight;
        }
        /// <summary>
        /// 1号显示屏输出(南1)
        /// </summary>
        public void LED1_Say(string text)
        {
            //  LTLibrary.LED.LedHelper.LEDSetting(LedIp1, LedWidth, LedHeight);
            //判断是否配置ip
            if (string.IsNullOrWhiteSpace(LedIp1) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            string LogLed = LTLibrary.LED.LedHelper.ShowMuiltyText(LedIp1
                               , LedWidth, LedHeight, text);
            if (!string.IsNullOrWhiteSpace(LogLed) && LogLed != "成功")
            {
                //Services.WinServiceFactory.Log.v("LED显示异常>>> " + LogLed);
                if (onLogHandler != null)
                {
                    onLogHandler("LED显示异常>>> " + LogLed);
                }
            }
        }
        /// <summary>
        /// 2号显示屏输出(北1)
        /// </summary>
        public void LED2_Say(string text)
        {
            //  LTLibrary.LED.LedHelper.LEDSetting(LedIp2, LedWidth, LedHeight);
            //判断是否配置ip
            if (string.IsNullOrWhiteSpace(LedIp2) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            string LogLed = LTLibrary.LED.LedHelper.ShowMuiltyText(LedIp2
                               , LedWidth, LedHeight, text);
            if (!string.IsNullOrWhiteSpace(LogLed) && LogLed != "成功")
            {
                //Services.WinServiceFactory.Log.v("LED显示异常>>> " + LogLed);
                if (onLogHandler != null)
                {
                    onLogHandler("LED显示异常>>> " + LogLed);
                }
            }
        }
        /// <summary>
        /// 3号显示屏输出(北2)
        /// </summary>
        public void LED3_Say(string text)
        {
            //  LTLibrary.LED.LedHelper.LEDSetting(LedIp3, LedWidth, LedHeight);
            //判断是否配置ip
            if (string.IsNullOrWhiteSpace(LedIp3) || string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            string LogLed = LTLibrary.LED.LedHelper.ShowMuiltyText(LedIp3
                               , LedWidth, LedHeight, text);
            if (!string.IsNullOrWhiteSpace(LogLed) && LogLed != "成功")
            {
                //Services.WinServiceFactory.Log.v("LED显示异常>>> " + LogLed);
                if (onLogHandler != null)
                {
                    onLogHandler("LED显示异常>>> " + LogLed);
                }
            }
        }
        /// <summary>
        /// 4号显示屏输出(南1-出口)
        /// </summary>
        public void LED4_Say(string text)
        {    //判断是否配置ip
             //   LTLibrary.LED.LedHelper.LEDSetting(LedIp4, LedWidth, LedHeight);
            if (string.IsNullOrWhiteSpace(LedIp4)||string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            string LogLed = LTLibrary.LED.LedHelper.ShowMuiltyText(LedIp4
                             , LedWidth, LedHeight, text);
            if (!string.IsNullOrWhiteSpace(LogLed) && LogLed != "成功")
            {
                // Services.WinServiceFactory.Log.v("LED显示异常>>> " + LogLed);
                if (onLogHandler != null)
                {
                    onLogHandler("LED显示异常>>> " + LogLed);
                }
            }
        }
    }
}
