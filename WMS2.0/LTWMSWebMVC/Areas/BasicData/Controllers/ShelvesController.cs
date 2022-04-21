using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel.Warehouse;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSWebMVC.Areas.RealTime.Data;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class ShelvesController : BaseController
    {
        wh_warehouseBLL bll_warehouse;
        wh_shelvesBLL bll_shelves;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_shelfunitsBLL bll_shelfunits;
        public ShelvesController(wh_warehouseBLL bll_warehouse, wh_shelvesBLL bll_shelves, wh_warehouse_typeBLL bll_wh_warehouse_type,
            wh_wcs_srvBLL bll_wh_wcs_srv, wh_shelfunitsBLL bll_shelfunits)
        {
            this.bll_warehouse = bll_warehouse;
            this.bll_shelves = bll_shelves;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_shelfunits = bll_shelfunits;
            //设置仓库列表
            ListDataManager.setWareHouseGuidList2(bll_warehouse, bll_wh_warehouse_type);
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
        }
        // GET: BasicData/Shelves
        public ActionResult Index(ShelvesSearch Model)
        {
            Model.PageCont = bll_shelves.GetAllQueryOrderby(o => o.rack, w => w.state == LTWMSEFModel.EntityStatus.Normal, true)
                .Select(s => MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(s)).ToList();

            return View(Model);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new ShelvesModel());
        }
        [HttpPost]
        public JsonResult Add(ShelvesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //判断排列层是否大于0
                    if (model.rack <= 0 || model.columns_specs <= 0 || model.rows_specs <= 0)
                    {
                        AddJsonError("排列层必须大于0");
                        return JsonError();
                    }
                    using (var tran = bll_shelves.BeginTransaction())
                    {
                        try
                        {
                            wh_shelves info = new wh_shelves();
                            info.columns_specs = model.columns_specs;
                            // info.category = model.category;
                            info.createdate = DateTime.Now;
                            info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                            info.guid = Guid.NewGuid();
                            info.isinitialized = null;
                            info.rack = model.rack;
                            info.rack_of_wcs = model.rack;//默认wcs排一致
                            info.depth = model.depth;
                            info.same_side_mark = model.same_side_mark;
                            info.state = LTWMSEFModel.EntityStatus.Normal;
                            info.rows_specs = model.rows_specs;
                            info.warehouse_guid = model.warehouse_guid;
                            var wareModel = bll_warehouse.GetFirstDefault(w => w.guid == info.warehouse_guid);
                            if (wareModel != null && wareModel.guid != Guid.Empty)
                            {//设置仓库类别
                                info.category = wareModel.category;
                            }
                            else
                            {
                                //参数错误
                                AddJsonError("参数错误，仓库信息为空！");
                                return JsonError();
                            }


                            info.u_identification = info.warehouse_guid.ToString("N") + "-" + info.rack;
                            //初始化数据
                            if (model.submit_type == 2 || model.submit_type == 3)
                            {
                                var rtv2 = bll_shelves.InitShelfUnitData(info);
                                if (rtv2 != LTWMSEFModel.SimpleBackValue.True)
                                {
                                    AddJsonError("初始化货架信息失败！");
                                    return JsonError();
                                }
                                else
                                {//初始化数据成功
                                    info.isinitialized = true;
                                }
                            }
                            var rtv = bll_shelves.AddIfNotExists(info, w => w.u_identification);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                AddUserOperationLog("添加货架信息guid：[" + info.guid + "]--排：[" + info.rack + "]--列：[" + info.columns_specs + "]--层[" + info.rows_specs + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                tran.Commit();
                                return JsonSuccess();
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                            {
                                AddJsonError("数据库已存在货架排为：[" + info.rack + "]的记录信息");
                            }
                            else
                            {
                                AddJsonError("保存失败");
                            }
                        }
                        catch (Exception ex)
                        {
                            WMSFactory.Log.v(ex);
                            AddJsonError("异常：" + ex.ToString());
                        }
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }
        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_shelves.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(ShelvesModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.isinitialized == true)
                    {
                        AddJsonError("库位数据已初始化，不能修改");
                        return JsonError();
                    }
                    //判断排列层是否大于0
                    if (model.rack <= 0 || model.columns_specs <= 0 || model.rows_specs <= 0)
                    {
                        AddJsonError("排列层必须大于0");
                        return JsonError();
                    }
                    using (var tran = bll_shelves.BeginTransaction())
                    {
                        try
                        {
                            wh_shelves info = bll_shelves.GetFirstDefault(w => w.guid == model.guid);
                            if (info != null && info.guid != Guid.Empty)
                            {
                                info.rack = model.rack;
                                info.rack_of_wcs = model.rack;
                                info.columns_specs = model.columns_specs;
                                info.rows_specs = model.rows_specs;
                                info.depth = model.depth;
                                info.same_side_mark = model.same_side_mark;
                                info.warehouse_guid = model.warehouse_guid;
                                //info.category = model.category;
                                info.updatedate = DateTime.Now;
                                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                //并发控制（乐观锁）
                                info.OldRowVersion = model.OldRowVersion;
                                //初始化数据
                                if ((info.isinitialized == null || info.isinitialized.Value == false)
                                       && (model.submit_type == 2 || model.submit_type == 3))
                                {
                                    var rtv2 = bll_shelves.InitShelfUnitData(info);
                                    if (rtv2 != LTWMSEFModel.SimpleBackValue.True)
                                    {
                                        AddJsonError("初始化货架信息失败！");
                                        return JsonError();
                                    }
                                    else
                                    {//初始化数据成功
                                        info.isinitialized = true;
                                    }
                                }
                                var rtv = bll_shelves.Update(info);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    AddUserOperationLog("修改货架信息guid：[" + info.guid + "]--排：[" + info.rack + "]--列：[" + info.columns_specs + "]--层[" + info.rows_specs + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                    tran.Commit();
                                    return JsonSuccess();
                                }
                                else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                                {
                                    AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                                }
                                else
                                {
                                    AddJsonError("保存失败");
                                }
                            }
                            else
                            {
                                AddJsonError("数据库中不存在该条记录或已删除！");
                            }
                        }
                        catch (Exception ex)
                        {
                            WMSFactory.Log.v(ex);
                            AddJsonError("异常：" + ex.ToString());
                        }
                        tran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }
        [HttpPost]
        public JsonResult DeletePost(Guid guid)
        {
            try
            {
                var shelfObj = bll_shelves.GetFirstDefault(w => w.guid == guid);
                if (shelfObj != null)
                {
                    if (shelfObj.isinitialized == true)
                    {//初始化的数据不能删除
                        AddJsonError("货架已初始化不能删除！");
                    }
                    else
                    {
                        using (var tran = bll_shelves.BeginTransaction())
                        {
                            try
                            {
                                var rtv = bll_shelves.DeleteWidthShelfUnits(shelfObj);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    AddUserOperationLog("删除仓库信息guid：[" + guid + "],排："+ shelfObj.rack, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                    tran.Commit();
                                    return JsonSuccess();
                                }
                                else if (rtv == LTWMSEFModel.SimpleBackValue.NotExistOfDelete)
                                {
                                    AddJsonError("数据库不存在记录或已删除！");
                                }
                                else
                                {
                                    AddJsonError("删除失败！");
                                }
                            }
                            catch (Exception ex)
                            {
                                WMSFactory.Log.v(ex);
                                AddJsonError("异常：" + ex.ToString());
                            }
                            tran.Rollback();
                        }
                    }
                }
                else
                {
                    AddJsonError("数据库不存在记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 通过货架编号查询所有的库位信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult SeeShelfUnits(Guid guid)
        {

            wh_shelves shelvesObj = bll_shelves.GetFirstDefault(o => o.guid == guid);
            if (shelvesObj == null)
                return ErrorView("货架信息为空");
            var list_shelfunit = bll_shelfunits.GetAllQuery(w => w.shelves_guid == guid);
            var shelvesMod = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(shelvesObj);
            shelvesMod.ShelfUnits = list_shelfunit.Select(a =>
            {
                //MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a)
                var ShelfU = MapperConfig.Mapper.Map<wh_shelfunits, ShelfUnitsModel>(a);
                if (ShelfU.cellstate == ShelfCellState.Stored)
                {//存储中的库位查找 储存类型
                 // ShelfU.matterType = bll_shelfunits.getStoredMatterType(a);
                    ShelfU.matterType = LTWMSEFModel.MatterTypeEnum.Matter;
                }
                return ShelfU;
            }).ToList();

            return View(shelvesMod);
        }
        /// <summary>
        /// 删除选中的库位信息
        /// </summary>
        /// <param name="guids"></param>
        [HttpPost]
        public JsonResult DeleteSelectedShelfStocks(string guids)
        {
            try
            {
                List<Guid> lstGuid = LTLibrary.ConvertUtility.ParseToGuids(guids);
                if (lstGuid != null && lstGuid.Count > 0)
                {
                    //using (var tran = bll_shelfunits.BeginTransaction())
                    //{
                    try
                    {
                        var lstShelfUnits = bll_shelfunits.GetAllQuery(w => lstGuid.Contains(w.guid));
                        if (lstShelfUnits != null && lstShelfUnits.Count > 0)
                        {
                            foreach (var item in lstShelfUnits)
                            {
                                //item.state = LTWMSEFModel.EntityStatus.Deleted;
                                //item.updatedate = DateTime.Now;
                                bll_shelfunits.Delete(item);
                            }
                            //var rtv = bll_shelfunits.Update(lstShelfUnits);
                            //if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            //{//修改成功
                            //   tran.Commit();
                            return JsonSuccess();
                            //}
                            //else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            //{
                            //    AddJsonError("并发异常，请刷新再试");
                            //}
                            //else
                            //{
                            //    AddJsonError("修改失败");
                            //}
                        }
                        else
                        {
                            AddJsonError("未查询到数据库记录");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddJsonError("异常：" + ex.ToString());
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
    }
}