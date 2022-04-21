using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Web;

namespace LTWMSWebMVC
{
    /// <summary>
    /// voicehandler 的摘要说明
    /// </summary>
    public class voicehandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        { 
            Thread t = null; 
            context.Response.ContentType = "audio/wav"; 
            context.Response.Clear();
            using (MemoryStream ms = new MemoryStream())
            {
                t = new Thread(() =>
                {
                    SpeechSynthesizer ss = new SpeechSynthesizer();
                    try
                    {
                        //  ss.Rate = -5;
                        ss.Rate = 0;
                        ss.Volume = 100;
                        //ss.SelectVoiceByHints(VoiceGender.Male);
                        //获取所有声音名称。。
                        //var lst= ss.GetInstalledVoices();
                        //ss.SelectVoice(lst[1].VoiceInfo.Name);
                     //   ss.SelectVoice("Microsoft Zira");
                       // ss.SelectVoice("Microsoft Lili");
                       // ss.SelectVoice("Microsoft Kangkang");
                        ss.SetOutputToWaveStream(ms);
                        ss.Speak(context.Request["voice"]);
                    }
                    catch (Exception ex)
                    {
                        ss.Dispose();
                        context.Response.Write(ex.Message);
                        WMSFactory.Log.v("语音合成异常>>>"+ex);
                    }
                });
                t.Start();
                t.Join();
                ms.Position = 0;
                if (ms.Length > 0)
                {
                    ms.WriteTo(context.Response.OutputStream);
                }
              //  context.Response.End();
                context.Response.Flush();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}