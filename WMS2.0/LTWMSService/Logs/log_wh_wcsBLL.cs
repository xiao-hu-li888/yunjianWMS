using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Logs
{
    public class log_wh_wcsBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Logs.log_wh_wcs>
    {
        public log_wh_wcsBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
