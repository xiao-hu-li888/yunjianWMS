using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockcheckBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockcheck>
    { 
        public bill_stockcheckBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
           
        }
    }
}
