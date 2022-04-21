using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_roleBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_role>
    { 
        public sys_roleBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
