using LTWMSEFModel.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Stock
{
    public class stk_inout_recod_hisBLL : LTWMSEFModel.ComDao<stk_inout_recod_his>
    {
        public stk_inout_recod_hisBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
