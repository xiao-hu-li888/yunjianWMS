using LTWMSEFModel.Stock;
using LTWMSEFModel.Warehouse;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSWebMVC.Areas.RealTime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class LotNumberLockController : BaseController
    {
        wh_traymatterBLL bll_wh_traymatter;
        wh_trayBLL bll_wh_tray;
        wh_shelfunitsBLL bll_wh_shelfunits;
        stk_matterBLL bll_stk_matter;
        public LotNumberLockController(stk_matterBLL bll_stk_matter, wh_traymatterBLL bll_wh_traymatter, wh_trayBLL bll_wh_tray, wh_shelfunitsBLL bll_wh_shelfunits)
        {
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_stk_matter = bll_stk_matter;
        }
        // GET: BasicData/LotNumberLock
        public ActionResult Index(TrayMatterSearch Model)
        {
            //*************************************/ 
            int TotalSize = 0;
            var aa = bll_wh_traymatter.PaginationByLotNumber(Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize);
            if (aa != null)
            {
                Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s)).ToList();
                if (Model.PageCont != null && Model.PageCont.Count > 0)
                {
                    foreach (var item in Model.PageCont)
                    {
                        //绑定托盘信息
                        item.trayModel = MapperConfig.Mapper.Map<wh_tray, TrayModel>(bll_wh_tray.GetFirstDefault(w => w.guid == item.tray_guid));
                        if (item.trayModel != null && item.trayModel.shelfunits_guid != null)
                        {
                            item.ShelfUnitModel = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(bll_wh_shelfunits.GetFirstDefault(w => w.guid == item.trayModel.shelfunits_guid));
                            if (item.ShelfUnitModel != null)
                            {
                                item.trayModel.cell_state = item.ShelfUnitModel.cellstate;
                                item.warehouse_guid = item.ShelfUnitModel.warehouse_guid;
                            }
                        }
                        ////绑定物料信息
                        item.MatterModel = MapperConfig.Mapper.Map<stk_matter, MatterModel>(bll_stk_matter.GetFirstDefault(w => w.code == item.x_barcode));
                        if (item.MatterModel == null)
                        {
                            item.MatterModel = new MatterModel();
                        }
                    }
                }
            }
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }

        public ActionResult UpdateSpecialLock()
        {
            return View(new TrayMatterModel() { guid=Guid.NewGuid()});
        }
        [HttpPost]
        public JsonResult UpdateSpecialLock(TrayMatterModel model)
        {
            try
            {
                var intcout = bll_wh_traymatter.GetCount(w => w.lot_number == model.lot_number);
                if (intcout > 0)
                {//根据批次号，锁定对应的库位
                    bool islock = model.lock_unlock == LockUnLockEnum.Lock ? true : false;
                    var rtv1 = bll_wh_shelfunits.SpecialLockOutByLotNumber(model.lot_number,islock);
                    if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("根据批次号【"+ model.lot_number + "】锁定对应库位", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv1 == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发错误，请重试！");
                    }
                    else
                    {
                        AddJsonError("修改失败，请重试！");
                    }
                }
                else
                {
                    AddJsonError("对应的批次号不存在！");
                }
                /*
                foreach (var item in lstTrayM)
                {
                    item.test_status = model.test_status;
                }
                var rtv1 = bll_wh_traymatter.Update(lstTrayM);
                if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                {
                    return JsonSuccess();
                }
                else if (rtv1 == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                {
                    AddJsonError("数据并发错误，请重试！");
                }
                else
                {
                    AddJsonError("修改失败，请重试！");
                }*/

            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
    }
}