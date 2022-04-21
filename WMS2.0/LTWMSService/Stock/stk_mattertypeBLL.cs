using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Stock
{
    public class stk_mattertypeBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Stock.stk_mattertype>
    {
        public stk_mattertypeBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
