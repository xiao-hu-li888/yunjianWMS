using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_annexBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_annex>
    { 
        public sys_annexBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        { 
        }
  
    }
}
