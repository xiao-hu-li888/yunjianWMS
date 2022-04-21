using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_service_statusBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_service_status>
    {
        public wh_service_statusBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
