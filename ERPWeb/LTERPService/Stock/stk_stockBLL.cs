using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Stock
{
    public class stk_stockBLL : LTERPEFModel.ComDao<LTERPEFModel.Stock.stk_stock>
    {
        public stk_stockBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
