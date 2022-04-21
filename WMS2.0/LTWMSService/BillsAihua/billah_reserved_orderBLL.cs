using LTWMSEFModel.BillsAihua;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.BillsAihua
{
    public class billah_reserved_orderBLL : LTWMSEFModel.ComDao<billah_reserved_order>
    {
        public billah_reserved_orderBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        } 
        /// <summary>
        /// 获取预留单对应的所有明细
        /// </summary>
        /// <param name="resvoder_model"></param>
        /// <returns></returns>
        public List<billah_reserved_order_detail> getAllMatterDetails(billah_reserved_order resvoder_model)
        {
            IQueryable<billah_reserved_order_detail> query = from a in dbcontext.billah_reserved_order_detail
                                          join b in dbcontext.billah_reserved_order
                                          on a.reserved_order_guid equals b.guid
                                          where b.yl_id == resvoder_model.yl_id
                                          select a;
            return query.AsNoTracking().ToList();
        }
    }
}
