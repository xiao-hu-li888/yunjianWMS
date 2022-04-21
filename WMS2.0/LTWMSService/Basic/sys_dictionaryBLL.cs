using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_dictionaryBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_dictionary>
    {
        public sys_dictionaryBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        public List<LTWMSEFModel.Basic.sys_dictionary> GetListByParentID(string parentid)
        {
             return    GetAllQuery(w=>w.parent_id== parentid).OrderBy(a => a.sort).ToList();
        }
    }
}
