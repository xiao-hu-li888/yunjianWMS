using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_loginroleBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_loginrole>
    {
        public sys_loginroleBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
