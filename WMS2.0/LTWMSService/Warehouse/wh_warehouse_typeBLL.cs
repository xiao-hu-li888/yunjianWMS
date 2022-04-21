using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_warehouse_typeBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_warehouse_type>
    {
        public wh_warehouse_typeBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
