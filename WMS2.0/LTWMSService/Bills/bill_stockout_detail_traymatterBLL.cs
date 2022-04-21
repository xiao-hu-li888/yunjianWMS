using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockout_detail_traymatterBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockout_detail_traymatter>
    {
        public bill_stockout_detail_traymatterBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
