using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Hardware
{
    public class hdw_plcBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Hardware.hdw_plc>
    {
        public hdw_plcBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
