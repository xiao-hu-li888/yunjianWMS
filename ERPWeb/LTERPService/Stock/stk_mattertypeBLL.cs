using LTERPEFModel.Stock;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Stock
{
    public class stk_mattertypeBLL : LTERPEFModel.ComDao<stk_mattertype>
    {
        public stk_mattertypeBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        public List<stk_mattertype> PaginationByLinq(string keywords, int currentpage, int pagesize, out int totalcount)
        {  
            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<stk_mattertype> query = from a in dbcontext.stk_mattertype 
                            where a.state != LTERPEFModel.EntityStatus.Deleted
                            && (keywords == "" || (a.code ?? "").Trim().Contains(keywords)
             || (a.name ?? "").Trim().Contains(keywords) || (a.memo ?? "").Trim().Contains(keywords))
                                               select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderBy(o => o.code)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
    }
}
