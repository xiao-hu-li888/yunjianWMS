using LTWMSEFModel;
using LTWMSEFModel.Bills;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_traymatterBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_traymatter>
    {
        public wh_traymatterBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// 通过物料获取在库批次号列表
        /// </summary>
        /// <param name="matterGuid"></param>
        /// <returns></returns>
        public List<string> GetLotNumberListOnShelf(Guid matterGuid)
        {
            IQueryable<wh_traymatter> query = from a in dbcontext.wh_traymatter
                                 join b in dbcontext.stk_matter
                                 on a.x_barcode equals b.code
                                 join c in dbcontext.wh_tray
                                 on a.tray_guid equals c.guid
                                 where c.shelfunits_guid!=null&& c.status== TrayStatus.OnShelf
                                 &&b.guid==matterGuid
                                 select a;
           return query.AsNoTracking().Select(s => s.lot_number).Distinct().ToList();
        }
        public decimal GetStockNumberByMatterGuid(Guid matterGuid,string lotNumber)
        {
            IQueryable<wh_traymatter> query = from a in dbcontext.wh_traymatter
                                              join b in dbcontext.stk_matter
                                              on a.x_barcode equals b.code
                                              join c in dbcontext.wh_tray
                                              on a.tray_guid equals c.guid
                                              where c.shelfunits_guid != null && c.status == TrayStatus.OnShelf
                                              && b.guid == matterGuid &&a.lot_number== lotNumber
                                              select a;
            return query.AsNoTracking().Sum(s => s.number);
        }
        public List<LTWMSEFModel.Warehouse.wh_traymatter> PaginationByLinq(string keywords, int currentpage, int pagesize, out int totalcount,
           bool? s_ischecked_ok, int? s_matterType)
        {
            /*
                  from a in dbcontext.wh_tray
                  join b in dbcontext.wh_traymatter
                  on a.guid equals b.tray_guid
             */
            //    new ListItem("电池","1"),  new ListItem("其它物料", "2")
            if (s_matterType == null || s_matterType.Value == -1)
            {
                s_matterType = null;
            }
            else
            {
                s_matterType = s_matterType.Value;
            }

            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query = from a in dbcontext.wh_traymatter
                                                                     join b in dbcontext.wh_tray
                                                                      on a.tray_guid equals b.guid
                                                                     where b.state != LTWMSEFModel.EntityStatus.Deleted
                                                                     && (keywords == "" || (b.traybarcode ?? "").ToLower().Contains(keywords) || (b.shelfunits_pos ?? "").ToLower().Contains(keywords) ||
                                                                      (b.memo ?? "").ToLower().Contains(keywords) || (a.x_barcode ?? "").ToLower().Contains(keywords)
                                                                      || (a.memo ?? "").ToLower().Contains(keywords))
                                                                      //  &&(s_ischecked_ok == null||(s_ischecked_ok==true&& a.is_check_ok==true)||(s_ischecked_ok==false&&(a.is_check_ok==null||a.is_check_ok==false)))
                                                                      && (s_matterType == null || (s_matterType == 1 && a.lot_number != null && a.lot_number != "") || (s_matterType == 2 && (a.lot_number == null || a.lot_number == "")))
                                                                     select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
        /// <summary>
        /// 根据关键字、仓库 查询物料信息
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="warehouseguid"></param>
        /// <param name="currentpage"></param>
        /// <param name="pagesize"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<LTWMSEFModel.Warehouse.wh_traymatter> PaginationByLinq2(string keywords, Guid? warehouseguid
            , int currentpage, int pagesize, out int totalcount, MatterOrderEnum matterOrder
            , DateTime? TrayInDate_begin = null, DateTime? TrayInDate_end = null, TestStatusEnum? test_status = null
            )
        {
            /*
           * 正常可用
           * IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query = from a in dbcontext.wh_traymatter
                                                                    join b in dbcontext.wh_tray
                                                                     on a.tray_guid equals b.guid
                                                                     join c in dbcontext.stk_matter
                                                                     on a.x_barcode equals c.code
                                                                    where (warehouseguid==null||b.warehouse_guid==warehouseguid)&&b.state != LTWMSEFModel.EntityStatus.Deleted
                                                                    && (keywords == "" || (b.traybarcode ?? "").ToLower().Contains(keywords) || (b.shelfunits_pos ?? "").ToLower().Contains(keywords) ||
                                                                     (b.memo ?? "").ToLower().Contains(keywords) || (a.x_barcode ?? "").ToLower().Contains(keywords)
                                                                     || (a.project_name ?? "").ToLower().Contains(keywords) || (a.project_no ?? "").ToLower().Contains(keywords)
                                                                     || (a.customer_name ?? "").ToLower().Contains(keywords) || (a.memo ?? "").ToLower().Contains(keywords)
                                                                     || (a.lot_number ?? "").ToLower().Contains(keywords) || (c.name ?? "").ToLower().Contains(keywords)
                                                                      || (c.name_pinyin ?? "").ToLower().Contains(keywords) || (c.brand_name ?? "").ToLower().Contains(keywords)
                                                                       || (c.description ?? "").ToLower().Contains(keywords) || (c.mattertype_name ?? "").ToLower().Contains(keywords)
                                                                        || (c.specs ?? "").ToLower().Contains(keywords)  
                                                                     )
                                                                    && b.shelfunits_guid!=null
                                                                    select a;*/

            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query;
            DateTime? beginDate = null;
            if (TrayInDate_begin != null)
            {
                beginDate = new DateTime(TrayInDate_begin.Value.Year, TrayInDate_begin.Value.Month, TrayInDate_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (TrayInDate_end != null)
            {
                endDate = new DateTime(TrayInDate_end.Value.Year, TrayInDate_end.Value.Month, TrayInDate_end.Value.Day);
                endDate = endDate.Value.AddDays(1);
            }
            query = from a in dbcontext.wh_traymatter
                    join b in dbcontext.wh_tray
                     on a.tray_guid equals b.guid
                    //join c in dbcontext.wh_shelfunits //加上库位过滤，查询变慢。。。
                    //on b.traybarcode equals c.depth1_traybarcode
                    where (beginDate == null || a.createdate >= beginDate) && (endDate == null || a.createdate <= endDate)
                    && (warehouseguid == null || b.warehouse_guid == warehouseguid) && b.state != LTWMSEFModel.EntityStatus.Deleted
                    && (keywords == "" || (b.traybarcode ?? "").ToLower().Contains(keywords)
                    || (b.shelfunits_pos ?? "").ToLower().Contains(keywords) ||
                     (b.memo ?? "").ToLower().Contains(keywords) 
                     || (a.x_barcode ?? "").ToLower().Equals(keywords)
                     || (a.project_name ?? "").ToLower().Contains(keywords) || (a.project_no ?? "").ToLower().Contains(keywords)
                     || (a.customer_name ?? "").ToLower().Contains(keywords) || (a.memo ?? "").ToLower().Contains(keywords)
                     || (a.lot_number ?? "").ToLower().Equals(keywords) || (a.name_list ?? "").ToLower().Contains(keywords)
                     )
                    && b.shelfunits_guid != null
                    && (test_status == null || (int)test_status.Value == -1 || a.test_status == test_status)
                    select a;

            totalcount = query.AsNoTracking().Count();
             if(matterOrder== MatterOrderEnum.CreateDateAsc)
            {
                return query.AsNoTracking().OrderBy(o => o.createdate)
                       .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
            }
            else if(matterOrder== MatterOrderEnum.EffectiveDateAcs)
            {
                return query.AsNoTracking().OrderBy(o => o.effective_date)
                       .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
            }
            else if(matterOrder== MatterOrderEnum.EffectiveDateDesc)
            {
                return query.AsNoTracking().OrderByDescending(o => o.effective_date)
                       .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
            }
            else
            {
                return query.AsNoTracking().OrderByDescending(o => o.createdate)
                       .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
            }
        }
        /****************根据批次号查询锁定的物料信息******************/
        /// <summary>
        /// 根据批次号查询锁定的物料信息
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="currentpage"></param>
        /// <param name="pagesize"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<LTWMSEFModel.Warehouse.wh_traymatter> PaginationByLotNumber(int currentpage, int pagesize, out int totalcount)
        {  
            IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query = from a in dbcontext.wh_traymatter
                                                                     join b in dbcontext.wh_tray
                                                                      on a.tray_guid equals b.guid
                                                                     where dbcontext.wh_shelfunits.Any(w=>w.guid==b.shelfunits_guid
                                                                     &&w.special_lock_type== LTWMSEFModel.Warehouse.SpecialLockTypeEnum.StockOutLock) 
                                                                     &&b.state != LTWMSEFModel.EntityStatus.Deleted
                                                                       && b.shelfunits_guid != null
                                                                     select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }

    }
}
