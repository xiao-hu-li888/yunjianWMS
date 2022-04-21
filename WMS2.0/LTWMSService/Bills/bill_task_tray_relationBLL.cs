using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_task_tray_relationBLL : LTWMSEFModel.ComDao<bill_task_tray_relation>
    {
        public bill_task_tray_relationBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
    }
}
