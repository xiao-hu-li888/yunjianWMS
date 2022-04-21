using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary.LED
{
    public class LedHelper
    {
        /// <summary>
        /// 设置屏参（注意：只需根据屏的宽高点数的颜色设置一次，发送节目时无需设置）
        /// </summary>
        /// <param name="ip">ip地址(192.168.9.97)</param>
        /// <param name="LedWidth">192(64)</param>
        /// <param name="LedHeight">32</param>
        /// <returns>成功/失败的原因</returns>
        public static string LEDSetting(string ip, int LedWidth, int LedHeight)
        {
            try
            {
                int nResult;
                LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                            //TCP通讯********************************************************************************
                CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
                CommunicationInfo.IpStr = ip;//192.168.9.97 给IpStr赋值LED控制卡的IP
                CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值

                nResult = LedDll.LV_SetBasicInfo(ref CommunicationInfo, 2, LedWidth, LedHeight);//设置屏参，屏的颜色为2即为双基色，192为屏宽点数，32为屏高点数，具体函数参数说明见函数声明注示
                if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
                {
                    return LedDll.LS_GetError(nResult);
                }
                else
                {
                    return "成功";
                }
            }
            catch (Exception ex)
            {

            }
            return "设置失败异常！";
        }
        /// <summary>
        /// 显示单行文本 不移动
        /// </summary>
        /// <param name="ip">ip地址(192.168.9.97)</param>
        /// <param name="LedWidth">192(64)</param>
        /// <param name="LedHeight">32</param>
        /// <param name="text">需要显示的文本</param>
        /// <returns>成功/失败的原因</returns>
        public static string ShowText(string ip, int LedWidth, int LedHeight, string text)
        {
            try
            {
                int nResult;
                LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                            //TCP通讯********************************************************************************
                CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
                CommunicationInfo.IpStr = ip;//给IpStr赋值LED控制卡的IP
                CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值
                int hProgram;//节目句柄
                hProgram = LedDll.LV_CreateProgram(LedWidth, LedHeight, 2);//根据传的参数创建节目句柄，192是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                                           //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败
                nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
                if (nResult != 0)
                {
                    return LedDll.LS_GetError(nResult);
                }
                LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
                AreaRect.left = 0;
                AreaRect.top = 0;
                AreaRect.width = LedWidth;
                AreaRect.height = LedHeight;
                LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
                FontProp.FontName = "宋体";
                FontProp.FontSize = 12;
                FontProp.FontColor = LedDll.COLOR_RED;
                FontProp.FontBold = 0;
                LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);
                LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
                PlayProp.InStyle = 0;
                PlayProp.DelayTime = 3;
                PlayProp.Speed = 4;
                nResult = LedDll.LV_AddSingleLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, text, ref FontProp, ref PlayProp);
                nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
                LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
                if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
                {
                    return LedDll.LS_GetError(nResult);
                }
                else
                {
                    return "成功";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "显示失败异常！";
        }
        /// <summary>
        /// 显示单行文本 移动
        /// </summary>
        /// <param name="ip">ip地址(192.168.9.97)</param>
        /// <param name="LedWidth">192(64)</param>
        /// <param name="LedHeight">32</param>
        /// <param name="text">需要显示的文本</param>
        /// <returns>成功/失败的原因</returns>
        public static string ShowTextMove(string ip, int LedWidth, int LedHeight, string text)
        {
            try
            {
                int nResult;
                LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                            //TCP通讯********************************************************************************
                CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
                CommunicationInfo.IpStr = ip;//给IpStr赋值LED控制卡的IP
                CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值
                int hProgram;//节目句柄
                hProgram = LedDll.LV_CreateProgram(LedWidth, LedHeight, 2);//根据传的参数创建节目句柄，192是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                                           //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败
                nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
                if (nResult != 0)
                {
                    return LedDll.LS_GetError(nResult);
                }
                LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
                AreaRect.left = 0;
                AreaRect.top = 0;
                AreaRect.width = LedWidth;
                AreaRect.height = LedHeight;
                LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
                FontProp.FontName = "宋体";
                FontProp.FontSize = 12;
                FontProp.FontColor = LedDll.COLOR_RED;
                FontProp.FontBold = 0;
                nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, text, ref FontProp, 4);//快速通过字符添加一个单行文本区域，函数见函数声明注示
                nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
                LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
                if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
                {
                    return LedDll.LS_GetError(nResult);
                }
                else
                {
                    return "成功";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "显示失败异常！";
        }
        /// <summary>
        /// 显示多行文本
        /// </summary>
        public static string ShowMuiltyText(string ip, int LedWidth, int LedHeight, string text)
        {
            try
            {
                int nResult;
                LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
                                                                                            //TCP通讯********************************************************************************
                CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
                CommunicationInfo.IpStr = ip;//给IpStr赋值LED控制卡的IP
                CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值
                                                //节目句柄
                int hProgram;
                hProgram = LedDll.LV_CreateProgram(LedWidth, LedHeight, 2);//根据传的参数创建节目句柄，192是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
                                                                           //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败
                nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
                if (nResult != 0)
                {
                    return LedDll.LS_GetError(nResult);
                }
                LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
                AreaRect.left = 0;
                AreaRect.top = 0;
                AreaRect.width = LedWidth;
                AreaRect.height = LedHeight;
                LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);
                LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
                FontProp.FontName = "宋体";
                FontProp.FontSize = 12;
                FontProp.FontColor = LedDll.COLOR_RED;
                FontProp.FontBold = 0;
                LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
                PlayProp.InStyle = 0;
                PlayProp.DelayTime = 3;
                PlayProp.Speed = 4;
                //可以添加多个子项到图文区，如下添加可以选一个或多个添加
                //通过字符串添加一个多行文本到图文区，参数说明见声明注示
                nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, text, ref FontProp, ref PlayProp, 0, 0);
                nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
                LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
                if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
                {
                    return LedDll.LS_GetError(nResult);
                }
                else
                {
                    return "成功";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "显示失败异常！";
        }

        //其它方法待补充...
    }
}
