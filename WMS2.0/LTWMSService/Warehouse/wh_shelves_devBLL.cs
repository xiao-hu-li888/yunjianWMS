using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_shelves_devBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_shelves_dev>
    {
        public wh_shelves_devBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
