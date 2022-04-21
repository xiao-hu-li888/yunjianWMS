using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_annexBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_annex>
    { 
        public sys_annexBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        { 
        }
  
    }
}
