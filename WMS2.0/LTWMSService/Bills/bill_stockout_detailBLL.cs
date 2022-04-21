using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockout_detailBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockout_detail>
    {
        public bill_stockout_detailBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
