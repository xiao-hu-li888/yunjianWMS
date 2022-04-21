using LTWMSEFModel;
using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockoutBLL : LTWMSEFModel.ComDao<bill_stockout>
    {
        public bill_stockoutBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }

        public List<bill_stockout> PaginationByLinq(string keywords, int currentpage, int pagesize, out int totalcount, SearchBillsStatus_Out? s_bill_status,
           DateTime? s_out_date_begin, DateTime? s_out_date_end)
        {
         //   keywords = keywords ?? "";
            DateTime? beginDate = null;
            if (s_out_date_begin != null)
            {
                beginDate = new DateTime(s_out_date_begin.Value.Year,
              s_out_date_begin.Value.Month, s_out_date_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (s_out_date_end != null)
            {
                endDate = new DateTime(s_out_date_end.Value.Year,
                    s_out_date_end.Value.Month, s_out_date_end.Value.Day);
                endDate=endDate.Value.AddDays(1);
            }
            //修改为any 查询>>>>>>>>>>>>>>>>>
            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<bill_stockout> query = from a in dbcontext.bill_stockout
                                                                where a.state != LTWMSEFModel.EntityStatus.Deleted
                                                              && (beginDate == null || a.out_date >= beginDate) && (endDate == null || a.out_date <endDate)
                                                               && (
                                                                  (keywords == "" || (a.memo ?? "").Contains(keywords)
                                                                || (a.odd_numbers ?? "").Contains(keywords) 
                                                                || (a.operator_user ?? "").Contains(keywords)
                                                                ||  (a.contact_department ?? "").Contains(keywords)
                                                                || (a.code ?? "").Contains(keywords))
                                                                || dbcontext.bill_stockout_detail.Any(w => w.stockout_guid == a.guid &&
                                                                   ((w.lot_number ?? "").ToLower().Equals(keywords)
                                                                   || (w.memo ?? "").ToLower().Contains(keywords)
                                                                   || (w.matter_code ?? "").ToLower().Equals(keywords)
                                                                    || (w.matter_name ?? "").ToLower().Contains(keywords)
                                                                   )
                                                                )
                                                                )
                                                               && (s_bill_status == null || (int)s_bill_status.Value == -1 ||
                                                               (s_bill_status== SearchBillsStatus_Out.Finished&& a.bill_status == BillsStatus_Out.Finished)
                                                               ||(s_bill_status== SearchBillsStatus_Out.Running&&a.bill_status!= BillsStatus_Out.Finished)
                                                               )
                                                                select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
        /// <summary>
        /// 通过入库订单号查询对应的详细列表
        /// </summary>
        /// <param name="stockin_oddnumbers"></param>
        /// <returns></returns>
        public List<bill_stockout_detail> getDetailsByStockOutOddNumbers(string stockin_oddnumbers)
        {
            IQueryable<bill_stockout_detail> query = from a in dbcontext.bill_stockout_detail
                                                    join b in dbcontext.bill_stockout
                                                    on a.stockout_guid equals b.guid
                                                    where b.odd_numbers == stockin_oddnumbers
                                                    select a;
            return query.AsNoTracking().ToList();
        }
    }
}
