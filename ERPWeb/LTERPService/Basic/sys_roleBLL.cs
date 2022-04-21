using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_roleBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_role>
    { 
        public sys_roleBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
