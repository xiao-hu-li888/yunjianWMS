using DbBackUpService.bak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBackUpService.bakBLL
{
    public class bak_dataBLL : BackUpComDao<bak_data>
    {
        public bak_dataBLL(DbBackUpService.LTBackUpModel dbContext) : base(dbContext)
        {
             
        }  
    }
}
