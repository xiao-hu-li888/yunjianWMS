using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Hardware
{
   public class hdw_message_waitedsend_hisBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Hardware.hdw_message_waitedsend_his>
    {
        public hdw_message_waitedsend_hisBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
    }
}
