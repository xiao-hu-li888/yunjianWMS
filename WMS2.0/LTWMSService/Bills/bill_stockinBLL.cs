using LTWMSEFModel;
using LTWMSEFModel.Bills;
using LTWMSEFModel.query_model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Bills
{
    public class bill_stockinBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Bills.bill_stockin>
    {
        //private bill_stockoutBLL _bll_bill_stockout;
        //public bill_stockoutBLL bll_bill_stockout
        //{
        //    get
        //    {
        //        if (_bll_bill_stockout==null)
        //        {
        //            _bll_bill_stockout = new bill_stockoutBLL(dbcontext);
        //        }
        //        return _bll_bill_stockout;
        //    } 
        //}
        public bill_stockinBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }

        public List<LTWMSEFModel.Bills.bill_stockin> PaginationByLinq(string keywords, int currentpage, int pagesize, out int totalcount, SearchBillsStatus_In? s_bill_status,
            DateTime? s_in_date_begin, DateTime? s_in_date_end)
        {
            //  keywords = keywords ?? "";
            DateTime? beginDate = null;
            if (s_in_date_begin != null)
            {
                beginDate = new DateTime(s_in_date_begin.Value.Year,
              s_in_date_begin.Value.Month, s_in_date_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (s_in_date_end != null)
            {
                endDate = new DateTime(s_in_date_end.Value.Year,
                    s_in_date_end.Value.Month, s_in_date_end.Value.Day);
                endDate = endDate.Value.AddDays(1);
            }

            //修改为any 查询>>>>>>>>>>>>>>>>>
            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<LTWMSEFModel.Bills.bill_stockin> query = from a in dbcontext.bill_stockin
                                                                where a.state != LTWMSEFModel.EntityStatus.Deleted
                                                              && (beginDate == null || a.in_date >= beginDate) && (endDate == null || a.in_date < endDate)
                                                               && (
                                                                  (keywords == "" || (a.memo ?? "").Contains(keywords)
                                                                || (a.odd_numbers ?? "").Contains(keywords) || (a.operator_user ?? "").Contains(keywords)
                                                                || (a.deliverer ?? "").Contains(keywords) || (a.contact_department ?? "").Contains(keywords)
                                                                || (a.code ?? "").Contains(keywords))
                                                                || dbcontext.bill_stockin_detail.Any(w => w.stockin_guid == a.guid &&
                                                                   ((w.lot_number ?? "").ToLower().Equals(keywords)
                                                                   || (w.memo ?? "").ToLower().Contains(keywords)
                                                                   || (w.matter_code ?? "").ToLower().Equals(keywords)
                                                                    || (w.name ?? "").ToLower().Contains(keywords)
                                                                   )
                                                                )
                                                                )
                                                               && (s_bill_status == null || (int)s_bill_status.Value == -1
                                                               || (s_bill_status == SearchBillsStatus_In.Finished && a.bill_status == BillsStatus.Finished)
                                                               || (s_bill_status == SearchBillsStatus_In.Running && a.bill_status != BillsStatus.Finished))
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
        public List<bill_stockin_detail> getDetailsByStockInOddNumbers(string stockin_oddnumbers)
        {
            IQueryable<bill_stockin_detail> query = from a in dbcontext.bill_stockin_detail
                                                    join b in dbcontext.bill_stockin
                                                    on a.stockin_guid equals b.guid
                                                    where b.odd_numbers == stockin_oddnumbers
                                                    select a;
            return query.AsNoTracking().ToList();
        }
        /// <summary>
        /// 单据汇总分页查询
        /// </summary>
        /// <returns></returns>
        public List<BillsGather> BillsInOutGatherPaging(string keywords, int currentpage, int pagesize, out int totalcount, DateTime? s_date_begin, DateTime? s_date_end,
            BillsInOutEnum? bills_inout)
        {
            DateTime? beginDate = null;
            if (s_date_begin != null)
            {
                beginDate = new DateTime(s_date_begin.Value.Year, s_date_begin.Value.Month, s_date_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (s_date_end != null)
            {
                endDate = new DateTime(s_date_end.Value.Year, s_date_end.Value.Month, s_date_end.Value.Day);
                endDate = endDate.Value.AddDays(1);
            }
            int s_bills_inout = 0;
            //if (bills_inout != null &&(int)bills_inout.Value != -1)
            //{ 
            if (bills_inout == BillsInOutEnum.BillsIn)
            {//查询入库
                s_bills_inout = 1;
            }
            else if (bills_inout == BillsInOutEnum.BillsOut)
            {//查询出库
                s_bills_inout = 2;
            }
            // }
            var query = (from a in dbcontext.bill_stockin
                         join b in dbcontext.bill_stockin_detail
                         on a.guid equals b.stockin_guid
                         where a.bill_status == BillsStatus.Finished
                         && (s_bills_inout == 0 || s_bills_inout == 1)
                          && (beginDate == null || a.in_date >= beginDate) && (endDate == null || a.in_date < endDate)
                          && (keywords == "" || (a.odd_numbers ?? "").Contains(keywords)
                         || (b.matter_code ?? "").Contains(keywords)
                          || (b.name ?? "").Contains(keywords)
                           || (b.lot_number ?? "").Contains(keywords)
                         )
                         select new BillsGather
                         {
                             bills_type = BillsInOutEnum.BillsIn,
                             effective_date = b.effective_date,
                             inout_date = a.in_date,
                             in_out_number = b.in_number,
                             lot_number = b.lot_number,
                             matter_code = b.matter_code,
                             name = b.name,
                             odd_numbers = a.odd_numbers,
                             producedate = b.producedate,
                             test_status = b.test_status,
                             stockin_type = a.stockin_type,
                             stockout_type = StockOutType.UseOut
                         }).Union(
                                from a in dbcontext.bill_stockout
                                join b in dbcontext.bill_stockout_detail
                                on a.guid equals b.stockout_guid
                                join c in
                                (
                                      from m in dbcontext.bill_stockout_detail_traymatter
                                      group m by new { m.stockout_guid } into g
                                      select new
                                      {
                                          stockout_guid = g.Key.stockout_guid,//已占库位
                                          test_status = g.Max(s => s.test_status)
                                      }
                                )
                                on a.guid equals c.stockout_guid
                                where a.bill_status == BillsStatus_Out.Finished
                                 && (s_bills_inout == 0 || s_bills_inout == 2)
                                 && (beginDate == null || a.out_date >= beginDate) && (endDate == null || a.out_date < endDate)
                                 && (keywords == "" || (a.odd_numbers ?? "").Contains(keywords)
                                || (b.matter_code ?? "").Contains(keywords)
                                 || (b.matter_name ?? "").Contains(keywords)
                                  || (b.lot_number ?? "").Contains(keywords)
                                )
                                select new BillsGather
                                {
                                    bills_type = BillsInOutEnum.BillsOut,
                                    effective_date = null,
                                    inout_date = a.out_date,
                                    in_out_number = b.out_number,
                                    lot_number = b.lot_number,
                                    matter_code = b.matter_code,
                                    name = b.matter_name,
                                    odd_numbers = a.odd_numbers,
                                    producedate = null,
                                    test_status = c.test_status,
                                    stockin_type = StockInType.PurchaseIn,
                                    stockout_type = a.stockout_type
                                }
                        );
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.inout_date)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
        /*
        /// <summary>
        /// 通过入库订单号查询对应的入库物料信息
        /// </summary>
        /// <param name="odd_numbers">入库单号（T01）</param>
        /// <param name="totalcount">入库总数</param>
        /// <returns></returns>
        public string[] GetStockDetailByOrderNum(string odd_numbers, out int totalcount)
        {
            List<string> lst = new List<string>();
            if (string.IsNullOrWhiteSpace(odd_numbers))
            {//订单号为空，直接返回空数据
                totalcount = 0;
                return lst.ToArray();
            }
            totalcount = 0;//并不是订单数，而是订单的电池总数。。。 GetCount(w => w.odd_numbers == odd_numbers);
            var query = from a in   dbcontext.wh_traymatter 
                    where a.lot_number== odd_numbers.Trim()  && a.state == LTWMSEFModel.EntityStatus.Normal 
                    select new { a.x_barcode };
            var ListV = query.AsNoTracking().Distinct().ToList();
            if (ListV != null && ListV.Count > 0)
            {
                foreach (var item in ListV)
                {
                    lst.Add(item.x_barcode);
                }
            }
            return lst.ToArray();
        }
        /// <summary>
        /// 可入库订单列表
        /// </summary>
        /// <returns></returns>
        public List<LTWMSEFModel.Bills.bill_stockin> GetBillStockInList()
        {
            IQueryable<LTWMSEFModel.Bills.bill_stockin> query = from a in dbcontext.bill_stockin  
                                              where a.state == LTWMSEFModel.EntityStatus.Normal && a.bill_property == LTWMSEFModel.Bills.BillsProperty.Battery
                                              && dbcontext.bill_stockout.Any(w=>w.state== LTWMSEFModel.EntityStatus.Normal&&
                                              w.bill_status!= LTWMSEFModel.Bills.BillsStatus_Out.Finished
                                              &&w.odd_numbers_in==a.odd_numbers)==false
                                              orderby a.createdate descending
                                              select a;

            return query.AsNoTracking().Take(10).ToList();
        }*/
    }
}
