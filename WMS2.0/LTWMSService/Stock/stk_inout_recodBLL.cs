using LTWMSEFModel.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Stock
{
    public class stk_inout_recodBLL : LTWMSEFModel.ComDao<stk_inout_recod>
    {
        public stk_inout_recodBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
