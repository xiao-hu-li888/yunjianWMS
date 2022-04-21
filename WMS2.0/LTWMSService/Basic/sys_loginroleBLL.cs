using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_loginroleBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_loginrole>
    {
        public sys_loginroleBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
