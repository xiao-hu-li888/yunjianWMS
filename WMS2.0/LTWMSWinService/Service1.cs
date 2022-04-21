using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSWinService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        LTWMSModule.WMSServiceServer wmsSrv;
        protected override void OnStart(string[] args)
        {
            wmsSrv = new LTWMSModule.WMSServiceServer();
            wmsSrv.Start();
        }

        protected override void OnStop()
        {
            wmsSrv.Stop();
        }
    }
}
