using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Logs
{
    public class log_hdw_plcBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Logs.log_hdw_plc>
    { 
        public log_hdw_plcBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
            
        }
    }
}
