using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Logs
{
    public class log_wh_traymatterBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Logs.log_wh_traymatter>
    {
        public log_wh_traymatterBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
