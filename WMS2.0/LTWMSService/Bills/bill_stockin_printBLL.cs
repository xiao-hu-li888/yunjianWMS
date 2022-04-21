using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockin_printBLL : LTWMSEFModel.ComDao<bill_stockin_print>
    {
        public bill_stockin_printBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
