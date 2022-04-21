using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
   public class wh_shelfunits_area_mattersBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_shelfunits_area_matters>
    {
        public wh_shelfunits_area_mattersBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
