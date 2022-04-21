using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary
{
    public interface IThreadService
    {
        void OnStop();
        void ThreadStart(object threadhandler);
    }
}
