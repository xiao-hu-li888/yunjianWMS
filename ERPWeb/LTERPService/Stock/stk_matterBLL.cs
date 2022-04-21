using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Stock
{
    public class stk_matterBLL : LTERPEFModel.ComDao<LTERPEFModel.Stock.stk_matter>
    {
        public stk_matterBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
