using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Logs
{
    public class log_sys_useroperationBLL : LTERPEFModel.ComDao<LTERPEFModel.Logs.log_sys_useroperation>
    { 
        public log_sys_useroperationBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
