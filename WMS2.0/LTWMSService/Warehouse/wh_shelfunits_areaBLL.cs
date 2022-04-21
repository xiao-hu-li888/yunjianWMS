using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
   public class wh_shelfunits_areaBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_shelfunits_area>
    {
        public wh_shelfunits_areaBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
