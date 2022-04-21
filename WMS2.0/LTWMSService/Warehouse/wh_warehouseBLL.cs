using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_warehouseBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_warehouse>
    {
        public wh_warehouseBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
