using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_trayBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_tray>
    {
        LTWMSService.Warehouse.wh_traymatterBLL bll_wh_traymatter;
        LTWMSService.Stock.stk_inout_recodBLL bll_stk_inout_recod;
        public wh_trayBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
            bll_wh_traymatter = new wh_traymatterBLL(dbContext);
            bll_stk_inout_recod = new Stock.stk_inout_recodBLL(dbContext);
        }
        /// <summary>
        /// 检查出库后是否存在未删除的数据，存在则删除未删除的数据
        /// </summary>
        public void CheckOldDataAndDelete()
        {
            /*SELECT * FROM `ltdb-wms-aihua`.wh_tray where traybarcode not in(
             select depth1_traybarcode from `ltdb-wms-aihua`.wh_shelfunits 
            where depth1_traybarcode is not null and depth1_traybarcode<>''
            )
            */
            IQueryable<LTWMSEFModel.Warehouse.wh_tray> query = from a in dbcontext.wh_tray
                                                               where dbcontext.wh_shelfunits.Where(w => w.depth1_traybarcode != null && w.depth1_traybarcode != "")
                                                               .Select(s => s.depth1_traybarcode).Contains(a.traybarcode) == false
                                                               select a;
            var listQ = query.AsNoTracking().ToList();
            if (listQ != null && listQ.Count > 0)
            {//删除未删除的数据
                foreach (var item in listQ)
                {
                    DeleteTrayInfoAndMatterDetails(item.traybarcode);
                }
            }
        }
        public List<LTWMSEFModel.Warehouse.wh_tray> PaginationByLinq(string keywords, int currentpage, int pagesize, out int totalcount, TrayStatus? status)
        {
            //关联查询出两条以上信息
            //IQueryable<LTWMSEFModel.Warehouse.wh_tray> query = from a in dbcontext.wh_tray
            //                                                join b in dbcontext.wh_traymatter
            //                                                on a.guid equals b.tray_guid
            //                                                where a.state != LTWMSEFModel.EntityStatus.Deleted
            //                                               && ((keywords ?? "").Trim() == "" || (a.traybarcode ?? "").ToLower().Contains(keywords.ToLower().Trim())
            //                                               || (b.matter_barcode ?? "").ToLower().Contains(keywords.ToLower().Trim()))
            //                                                select a;
            //修改为any 查询>>>>>>>>>>>>>>>>>
            keywords = (keywords ?? "").ToLower().Trim();//不区分大小写
            IQueryable<LTWMSEFModel.Warehouse.wh_tray> query = from a in dbcontext.wh_tray
                                                               where a.state != LTWMSEFModel.EntityStatus.Deleted
                                                               && (keywords == "" || (a.traybarcode ?? "").ToLower().Equals(keywords)
                                                               || (a.shelfunits_pos ?? "").ToLower().Equals(keywords) ||
                                                                 (a.memo ?? "").ToLower().Contains(keywords)
                                                                || dbcontext.wh_traymatter.Any(w => w.tray_guid == a.guid &&
                                                                   ((w.x_barcode ?? "").ToLower().Equals(keywords)
                                                                   || (w.memo ?? "").ToLower().Contains(keywords)
                                                                   || (w.lot_number ?? "").ToLower().Contains(keywords)
                                                                    || (w.name_list ?? "").ToLower().Contains(keywords)
                                                                   )
                                                                )
                                                                /* || (b.matter_barcode ?? "").ToLower().Contains(keywords)
                                                                 || (b.memo ?? "").ToLower().Contains(keywords)*/
                                                                )
                                                                && (status == null || (int)status.Value == -1 || a.status == status)
                                                               select a;
            totalcount = query.AsNoTracking().Count();
            return query.AsNoTracking().OrderByDescending(o => o.createdate)
                    .Skip(pagesize * (currentpage - 1)).Take(pagesize).ToList() ?? null;
        }
        /// <summary>
        /// 删除托盘信息及绑定的物料信息
        /// </summary>
        /// <param name="traybarcode">托盘条码</param>
        public LTWMSEFModel.SimpleBackValue DeleteTrayInfoAndMatterDetails(string traybarcode)
        {
            if (string.IsNullOrWhiteSpace(traybarcode))
            {
                return LTWMSEFModel.SimpleBackValue.True;
            }
            var trayModel = GetFirstDefault(w => w.traybarcode == traybarcode);
            if (trayModel != null && trayModel.guid != Guid.Empty)
            {
                //  删除托盘与物料绑定关系
                var _taymatterlist = bll_wh_traymatter.GetAllQuery(w => w.tray_guid == trayModel.guid).ToList();
                LTWMSEFModel.SimpleBackValue rtvdeltraymatter = LTWMSEFModel.SimpleBackValue.True;
                if (_taymatterlist != null && _taymatterlist.Count > 0)
                {
                    foreach (var subitem in _taymatterlist)
                    {
                        /*  //添加入库流水 
                          var intouRecod = new LTWMSEFModel.Stock.stk_inout_recod();
                          intouRecod.createdate = DateTime.Now;
                          intouRecod.goods_id = subitem.x_barcode;
                          intouRecod.guid = Guid.NewGuid();
                          intouRecod.inout_type = LTWMSEFModel.Stock.InOutTypeEnum.Out;
                          intouRecod.is_send = LTWMSEFModel.Stock.IsSendToEnum.None;
                          intouRecod.qty = subitem.number;
                          intouRecod.spec_id = subitem.lot_number;
                          intouRecod.state = LTWMSEFModel.EntityStatus.Normal;
                          //dbcontext.stk_inout_recod.Add(intouRecod);
                          //dbcontext.SaveChanges();  
                          bll_stk_inout_recod.Add(intouRecod);*/
                        rtvdeltraymatter = bll_wh_traymatter.Delete(subitem);
                        if (rtvdeltraymatter != LTWMSEFModel.SimpleBackValue.True)
                        {
                            break;
                        }
                    }
                }
                //  删除托盘信息（批次 = 托盘号）
                if (rtvdeltraymatter == LTWMSEFModel.SimpleBackValue.True)
                {
                    return Delete(trayModel);
                }
                else
                {
                    return LTWMSEFModel.SimpleBackValue.False;
                }

            }
            return LTWMSEFModel.SimpleBackValue.True;
        }
        /// <summary>
        /// 解除指定托盘编号与库位的关联关系
        /// </summary>
        /// <param name="traycodeList"></param>
        public LTWMSEFModel.SimpleBackValue DisConnectedALLMatter(string traybarcode)
        {
            if (string.IsNullOrWhiteSpace(traybarcode))
            {
                return LTWMSEFModel.SimpleBackValue.True;
            }
            // 如果是其它物料解绑。。。
            //。。。。。。。。。。。。。。。。。。
            //修改托盘信息
            var trayModel = GetFirstDefault(w => w.traybarcode == traybarcode);
            if (trayModel != null && trayModel.guid != Guid.Empty)
            {
                //trayModel.emptypallet = true;
                trayModel.shelfunits_guid = null;
                trayModel.shelfunits_pos = null;
                trayModel.status = LTWMSEFModel.Warehouse.TrayStatus.OffShelf;
                trayModel.totalkind = 0;
                trayModel.totalnumber = 0;
                trayModel.updatedate = DateTime.Now;
                trayModel.weight = 0;
                trayModel.isscan = false;
                trayModel.scandate = null;
                //删除绑定信息
                /* var _taymatterlist = dbcontext.wh_traymatter.Where(w => w.tray_guid == trayModel.guid).ToList();
                 if (_taymatterlist != null && _taymatterlist.Count > 0)
                 {
                     foreach (var subitem in _taymatterlist)
                     {
                         //添加日志
                         dbcontext.wh_traymatter_log.Add(new LTWMSEFModel.Warehouse.wh_traymatter_log()
                         {
                             guid = Guid.NewGuid(),
                             batch = subitem.batch,
                             bind_type = LTWMSEFModel.Warehouse.TrayMatterBindType.Unbind,
                             log_date = DateTime.Now,
                             matter_barcode = subitem.matter_barcode,
                             memo = "电池下架自动解绑",
                             number = 1,
                             traybarcode = trayModel.traybarcode,
                             warehouse_guid = trayModel.warehouse_guid,
                             //weight = trayModel.weight
                         });
                     }
                     dbcontext.wh_traymatter.RemoveRange(_taymatterlist);
                 }*/
                return Update(trayModel);
            }
            return LTWMSEFModel.SimpleBackValue.True;
        }
        /// <summary>
        /// 通过托盘条码查询对应的物料信息
        /// </summary>
        /// <param name="traybarcode"></param>
        /// <returns></returns>
        public List<LTWMSEFModel.Warehouse.wh_traymatter> GetMatterDetailByTrayBarcode(string traybarcode)
        {
            if (string.IsNullOrWhiteSpace(traybarcode))
            {
                return null;
            }
            IQueryable<LTWMSEFModel.Warehouse.wh_traymatter> query2 = from a in dbcontext.wh_tray
                                                                      join b in dbcontext.wh_traymatter
                                                                      on a.guid equals b.tray_guid
                                                                      where a.traybarcode == traybarcode
                                                                      orderby b.createdate
                                                                      select b;
            return query2.AsNoTracking().ToList();
        }
        public List<wh_traymatter> GetTrayMatterListOnShelf(string matter_code, string lot_number)
        {
            IQueryable<wh_traymatter> query2 = from a in dbcontext.wh_tray
                                               join b in dbcontext.wh_traymatter
                                               on a.guid equals b.tray_guid
                                               where b.x_barcode == matter_code &&
                                               b.lot_number == lot_number && a.status == TrayStatus.OnShelf
                                               && a.shelfunits_guid != null
                                               orderby b.createdate
                                               select b;
            return query2.AsNoTracking().ToList();
        }
    }
}
