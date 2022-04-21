using LTWMSEFModel.query_model;
using LTWMSEFModel.Stock;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class InventoryStatisticsController : Controller
    {
        stk_matterBLL bll_stk_matter;
        // GET: BasicData/InventoryStatistics
        wh_trayBLL bll_wh_tray;
        public InventoryStatisticsController(stk_matterBLL bll_stk_matter, wh_trayBLL bll_wh_tray)
        {
            this.bll_wh_tray = bll_wh_tray;
            this.bll_stk_matter = bll_stk_matter;
        }
        public ActionResult Index(InventoryStatisticSearch Model)
        {
            int TotalSize = 0;
            //  var aa = bll_stk_matter.StockInventoryPagination(Model.s_keywords, Model.Paging.paging_curr_page, Model.Paging.PageSize, out TotalSize);
            var matterObj = bll_stk_matter.Pagination(Model.Paging.CurrPage, Model.Paging.PageSize, out TotalSize, o => o.createdate,
                w=> (Model.s_keywords==""
                        ||(w.code??"").Contains(Model.s_keywords)
                        || (w.name ?? "").Contains(Model.s_keywords)
                        ) );

            if (matterObj != null && matterObj.Count > 0)
            {
                int emptycount = bll_wh_tray.GetCount(w=>w.emptypallet==true&&w.status== LTWMSEFModel.Warehouse.TrayStatus.OnShelf&&
                w.shelfunits_guid!=null);
                int traycount = bll_wh_tray.GetCount(w => w.emptypallet == false && w.status == LTWMSEFModel.Warehouse.TrayStatus.OnShelf &&
                 w.shelfunits_guid != null); 
                Model.PageCont = matterObj.Select(s =>
                {
                    var obj = MapperConfig.Mapper.Map<stk_matter, MatterModel>(s);
                    var a = bll_stk_matter.GetLotNumTotalNumByType(LotNumTypeEnum.ALL, obj.code);
                    if (a != null)
                    {
                        obj.lot_number = a.Count();
                        obj.total_number = (int)a.Sum(s => s.total_number);
                    }
                    var b = bll_stk_matter.GetLotNumTotalNumByType(LotNumTypeEnum.Ok, obj.code);
                    if (b != null)
                    {
                        obj.ok_lot_number = b.Count();
                        obj.ok_total_number = (int)b.Sum(s => s.total_number);
                    }
                    var c = bll_stk_matter.GetLotNumTotalNumByType(LotNumTypeEnum.Waited, obj.code);
                    if (c != null)
                    {
                        obj.waited_lot_number = c.Count();
                        obj.waited_total_number = (int)c.Sum(s => s.total_number);
                    }
                    var d = bll_stk_matter.GetLotNumTotalNumByType(LotNumTypeEnum.Bad, obj.code);
                    if (d != null)
                    {
                        obj.bad_lot_number = d.Count();
                        obj.bad_total_number = (int)d.Sum(s => s.total_number);
                    }
                    var e = bll_stk_matter.GetLotNumTotalNumByType(LotNumTypeEnum.Near, obj.code);
                    if (e != null)
                    {
                        obj.near_lot_number = e.Count();
                        obj.near_total_number = (int)e.Sum(s => s.total_number);
                    }
                    obj.empty_count = emptycount;
                    obj.tray_count = traycount;
                    return obj;
                }).ToList();
            }
            else
            {
                Model.PageCont = new List<MatterModel>();
            }
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            /* int TotalSize = 0;
            var aa = bll_stk_matter.Pagination(Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize, o => o.guid,
               w => (Model.s_keywords == "" || (w.code ?? "").Contains(Model.s_keywords)
               || (w.code ?? "").Contains(Model.s_keywords) || (w.name ?? "").Contains(Model.s_keywords)
               || (w.name_pinyin ?? "").Contains(Model.s_keywords) || (w.memo ?? "").Contains(Model.s_keywords)
               )
               && (Model.s_specs == "" || (w.specs ?? "").Contains(Model.s_specs))
               && ((Model.s_mattertype_guid == null || Model.s_mattertype_guid.Value.ToString() == "-1") || w.mattertype_guid == Model.s_mattertype_guid)
                 , true);
            Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<stk_matter, MatterModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };*/

            return View(Model);
        }
    }
}