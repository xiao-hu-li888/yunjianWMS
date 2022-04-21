using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockin_detailBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockin_detail>
    {
        public bill_stockin_detailBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
