using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSModule.Services
{
    public class BaseService
    {
        private int _seq;
        public int Seq
        {
            get
            {
                if (_seq > 99999999)
                {//重置
                    _seq = 1;
                }
                return ++_seq;
            }
        }
    }
}
