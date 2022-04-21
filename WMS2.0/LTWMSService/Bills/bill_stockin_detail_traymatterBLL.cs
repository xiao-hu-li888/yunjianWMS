using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockin_detail_traymatterBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockin_detail_traymatter>
    {
        public bill_stockin_detail_traymatterBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
