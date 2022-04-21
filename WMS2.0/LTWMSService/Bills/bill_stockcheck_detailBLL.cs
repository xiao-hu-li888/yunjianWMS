using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockcheck_detailBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockcheck_detail>
    {
        public bill_stockcheck_detailBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
