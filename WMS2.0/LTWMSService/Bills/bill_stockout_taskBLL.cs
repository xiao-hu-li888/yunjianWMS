using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockout_taskBLL : LTWMSEFModel.ComDao<bill_stockout_task>
    {
        public bill_stockout_taskBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
