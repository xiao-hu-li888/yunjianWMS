using LTWMSEFModel.query_model;
using LTWMSService.Basic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Stock
{
    public class stk_matterBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Stock.stk_matter>
    {
        sys_control_dicBLL bll_sys_control_dic;
        public stk_matterBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
           
        }
        /// <summary>
        /// 库存统计分页
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="currentpage"></param>
        /// <param name="pagesize"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<StockMatter> StockInventoryPagination(string keywords, int currentpage, int pagesize, out int totalcount)
        {
            #region
            /*
             var query =
                       from item in
                           (
                               from parent in etcDataContext.SYS_Structures
                               where parent.ParentNode == "root"
                               join child in etcDataContext.SYS_Structures
                               on parent.Name equals child.ParentNode into childAll
                               from childItem in childAll.DefaultIfEmpty()
                               select new
                               {
                                   parent.ID,
                                   parent.Name,
                                   childItem.ParentNode
                               }
                           )
                       group item by item.Name into groupChild                      
                       select new StructureRootDTO
                       {
                           ID = groupChild.Max(o => o.ID),
                           Name = groupChild.Max(o => o.Name),
                           ChildParentNodeName = groupChild.Max(o => o.ParentNode)
                       };
                return query.OrderBy(o => o.ID).ToList();
             */
            #endregion
            IQueryable<StockMatter> query = from b in dbcontext.stk_matter
                                            join d in (
                                                    from a in dbcontext.wh_traymatter
                                                    join m in dbcontext.wh_tray
                                                    on a.tray_guid equals m.guid
                                                    where a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                                                    && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf
                                                    group a by new { a.x_barcode } into g
                                                    select new
                                                    {
                                                        matterBarcode = g.Key.x_barcode,//已占库位
                                                        totalnum = g.Sum(s => s.number),
                                                        totalweight = g.Sum(s => s.total_weight)
                                                    }
                                            )
                                            on b.code equals d.matterBarcode
                                            into newTb
                                            from c in newTb.DefaultIfEmpty()
                                            where (keywords == null
                                                || (b.code ?? "").Trim().Contains(keywords) || (b.name ?? "").Trim().Contains(keywords)
                                                || (b.description ?? "").Trim().Contains(keywords) || (b.memo ?? "").Trim().Contains(keywords)
                                                )
                                            select new StockMatter
                                            {
                                                brand_guid = b.brand_guid,
                                                brand_name = b.brand_name,
                                                can_mix = b.can_mix,
                                                code = b.code,
                                                code_full = b.code_full,
                                                color = b.color,
                                                convert_ratio = b.convert_ratio,
                                                createdate = b.createdate,
                                                createuser = b.createuser,
                                                def_warehouse_guid = b.def_warehouse_guid,
                                                description = b.description,
                                                effective_date = b.effective_date,
                                                guid = b.guid,
                                                length = b.length,
                                                mattertype_guid = b.mattertype_guid,
                                                mattertype_name = b.mattertype_name,
                                                matter_from = b.matter_from,
                                                memo = b.memo,
                                                name = b.name,
                                                name_pinyin = b.name_pinyin,
                                                rowversion = b.rowversion,
                                                specs = b.specs,
                                                state = b.state,
                                                std_price = b.std_price,
                                                std_weight = b.std_weight,
                                                stock_max = b.stock_max,
                                                stock_min = b.stock_min,
                                                unit_convert = b.unit_convert,
                                                unit_measurement = b.unit_measurement,
                                                unit_measurement_guid = b.unit_measurement_guid,
                                                updatedate = b.updatedate,
                                                updateuser = b.updateuser,
                                                total_number = c.totalnum,
                                                total_weight = c.totalweight
                                            };

            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking()
                .OrderByDescending(o => o.total_weight)
                .ThenByDescending(o => o.total_number)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="lotNumType">类型</param>
        /// <param name="mattercode">物料条码</param>
        public List<TrayMatterLotTotal> GetLotNumTotalNumByType(LotNumTypeEnum lotNumType,string mattercode)
        {
            if(lotNumType== LotNumTypeEnum.ALL)
            {
                var query = from a in dbcontext.wh_traymatter
                            join m in dbcontext.wh_tray
                            on a.tray_guid equals m.guid
                            where a.x_barcode==mattercode&&a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                            && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf
                            group a by new { a.lot_number } into g
                            select new TrayMatterLotTotal
                            {
                                lot_number = g.Key.lot_number,                               
                                total_number= g.Sum(s => s.number)
                            };
                return query.AsNoTracking().ToList();
            }
            else if(lotNumType== LotNumTypeEnum.Bad)
            {
                var query = from a in dbcontext.wh_traymatter
                            join m in dbcontext.wh_tray
                            on a.tray_guid equals m.guid
                            where a.x_barcode == mattercode && a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                            && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf&&a.test_status== LTWMSEFModel.Bills.TestStatusEnum.TestFail
                            group a by new { a.lot_number } into g
                            select new TrayMatterLotTotal
                            {
                                lot_number = g.Key.lot_number,
                                total_number = g.Sum(s => s.number)
                            };
                return query.AsNoTracking().ToList();
            }
            else if(lotNumType== LotNumTypeEnum.Near)
            {
                if (bll_sys_control_dic == null)
                {
                    bll_sys_control_dic = new sys_control_dicBLL(dbcontext);
                }
                string neartearStr= bll_sys_control_dic.GetValueByType(CommDictType.NearTerm, Guid.Empty);
                if (string.IsNullOrWhiteSpace(neartearStr))
                {
                    neartearStr = "30";
                    bll_sys_control_dic.SetValueByType(CommDictType.NearTerm, neartearStr, Guid.Empty);
                }
               int neartermV= LTLibrary.ConvertUtility.ToInt( neartearStr);
               DateTime dateBg=  DateTime.Now.AddDays(neartermV);
                var query = from a in dbcontext.wh_traymatter
                            join m in dbcontext.wh_tray
                            on a.tray_guid equals m.guid
                            where a.x_barcode == mattercode && a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                            && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf &&
                            (a.effective_date==null||a.effective_date< dateBg)
                            group a by new { a.lot_number } into g
                            select new TrayMatterLotTotal
                            {
                                lot_number = g.Key.lot_number,
                                total_number = g.Sum(s => s.number)
                            };
                return query.AsNoTracking().ToList();
            }
            else if(lotNumType== LotNumTypeEnum.Ok)
            {
                var query = from a in dbcontext.wh_traymatter
                            join m in dbcontext.wh_tray
                            on a.tray_guid equals m.guid
                            where a.x_barcode == mattercode && a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                            && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf && a.test_status == LTWMSEFModel.Bills.TestStatusEnum.TestOk
                            group a by new { a.lot_number } into g
                            select new TrayMatterLotTotal
                            {
                                lot_number = g.Key.lot_number,
                                total_number = g.Sum(s => s.number)
                            };
                return query.AsNoTracking().ToList();
            }
            else if(lotNumType== LotNumTypeEnum.Waited)
            {
                var query = from a in dbcontext.wh_traymatter
                            join m in dbcontext.wh_tray
                            on a.tray_guid equals m.guid
                            where a.x_barcode == mattercode && a.state == LTWMSEFModel.EntityStatus.Normal && m.shelfunits_guid != null
                            && m.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf && a.test_status == LTWMSEFModel.Bills.TestStatusEnum.None
                            group a by new { a.lot_number } into g
                            select new TrayMatterLotTotal
                            {
                                lot_number = g.Key.lot_number,
                                total_number = g.Sum(s => s.number)
                            };
                return query.AsNoTracking().ToList();

            }
            return null;
        }
    }
    public class TrayMatterLotTotal
    {
        /// <summary>
        /// 批次号
        /// </summary>
       public string lot_number { get; set; }
        /// <summary>
        /// 批次号下的总数量
        /// </summary>
       public decimal total_number { get; set; }
    }
    public enum LotNumTypeEnum
    {
        /// <summary>
        /// 所有批次和数量
        /// </summary>
        ALL=0,
        /// <summary>
        /// 合格
        /// </summary>
        Ok=1,
        /// <summary>
        /// 待检
        /// </summary>
        Waited=2,
        /// <summary>
        /// 不合格
        /// </summary>
        Bad=3,
        /// <summary>
        /// 临近有效期（1个月？2个月）
        /// </summary>
        Near=4
    }
}
