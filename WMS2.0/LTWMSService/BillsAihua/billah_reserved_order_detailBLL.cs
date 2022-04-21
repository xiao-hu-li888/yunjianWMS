using LTWMSEFModel.BillsAihua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.BillsAihua
{
    public class billah_reserved_order_detailBLL : LTWMSEFModel.ComDao<billah_reserved_order_detail>
    {
        public billah_reserved_order_detailBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
