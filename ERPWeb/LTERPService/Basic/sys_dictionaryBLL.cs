using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_dictionaryBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_dictionary>
    {
        public sys_dictionaryBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        public List<LTERPEFModel.Basic.sys_dictionary> GetListByParentID(string parentid)
        {
             return    GetAllQuery(w=>w.parent_id== parentid).OrderBy(a => a.sort).ToList();
        }
    }
}
