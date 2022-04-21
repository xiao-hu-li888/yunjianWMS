using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Stock
{
    public class stk_stockBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Stock.stk_stock>
    {
        public stk_stockBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
