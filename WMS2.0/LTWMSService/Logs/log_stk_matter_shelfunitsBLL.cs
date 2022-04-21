using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Logs
{
    public class log_stk_matter_shelfunitsBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Logs.log_stk_matter_shelfunits>
    {
        public log_stk_matter_shelfunitsBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
