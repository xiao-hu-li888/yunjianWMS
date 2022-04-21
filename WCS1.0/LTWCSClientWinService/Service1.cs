using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSClientWinService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        LTWCSModule.WCSServiceClient wcsModule;
        protected override void OnStart(string[] args)
        {
            //启动wcs 调度服务
            if (wcsModule == null)
            {
                wcsModule = new LTWCSModule.WCSServiceClient();
            }
            wcsModule.Start();
        }

        protected override void OnStop()
        {//关闭wcs 调度服务
            wcsModule.Stop();
        }
    }
}
