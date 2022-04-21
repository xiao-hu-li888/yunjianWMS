using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Hardware
{
   public class hdw_message_waitedsendBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Hardware.hdw_message_waitedsend>
    {
        public hdw_message_waitedsendBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
