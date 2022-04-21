using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel.Basic;
using LTWMSEFModel.Warehouse;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class WareHouseController : BaseController
    {
        wh_warehouseBLL bll_warehouse;
        wh_shelvesBLL bll_shelves;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        public WareHouseController(wh_warehouseBLL bll_warehouse, wh_shelvesBLL bll_shelves, wh_warehouse_typeBLL bll_wh_warehouse_type)
        {
            this.bll_warehouse = bll_warehouse;
            this.bll_shelves = bll_shelves;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            ListDataManager.setWareHouseTypeGuidList(bll_wh_warehouse_type);
        }
        // GET: BasicData/WareHouse
        public ActionResult Index()
        {
            Data.WareHouseSearch ModelSearch = new Data.WareHouseSearch();
            ModelSearch.PageCont = bll_warehouse.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal)
                .Select(a => MapperConfig.Mapper.Map<wh_warehouse, Data.WareHouseModel>(a)).ToList();
            return View(ModelSearch);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new WareHouseModel());
        }
        [HttpPost]
        public JsonResult Add(WareHouseModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    wh_warehouse info = new wh_warehouse();
                    info.address = model.address;
                    info.code = model.code;
                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();
                    info.name = model.name;
                    info.remark = model.remark;
                    info.distribute_way = model.distribute_way;
                    info.warehouse_type_guid = model.warehouse_type_guid;
                    if (info.warehouse_type_guid != null)
                    {
                        var _wtM = bll_wh_warehouse_type.GetFirstDefault(w => w.guid == info.warehouse_type_guid);
                        if (_wtM != null && _wtM.guid != Guid.Empty)
                        {
                            info.warehouse_type_name = _wtM.name;
                        }
                    }
                    info.category = model.category;
                    info.state = LTWMSEFModel.EntityStatus.Normal;

                    var rtv = bll_warehouse.AddIfNotExists(info, w => w.code);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加仓库信息guid：[" + info.guid + "]--名称：[" + info.name + "]--code：[" + info.code + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在仓库编号为：[" + info.code + "]的数据记录");
                    }
                    else
                    {
                        AddJsonError("保存失败");
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
            var model = bll_warehouse.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_warehouse, WareHouseModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(WareHouseModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    wh_warehouse info = bll_warehouse.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null)
                    {
                        info.updatedate = DateTime.Now;
                        info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        info.address = model.address;
                        info.distribute_way = model.distribute_way;
                        info.warehouse_type_guid = model.warehouse_type_guid;
                        if (info.warehouse_type_guid != null)
                        {
                            var _wtM = bll_wh_warehouse_type.GetFirstDefault(w => w.guid == info.warehouse_type_guid);
                            if (_wtM != null && _wtM.guid != Guid.Empty)
                            {
                                info.warehouse_type_name = _wtM.name;
                            }
                        }

                        info.category = model.category;
                        info.name = model.name;
                        info.remark = model.remark;
                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_warehouse.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改仓库信息guid：[" + info.guid + "]--名称：[" + info.name + "]--code：[" + info.code + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
            //如果有对应的货架信息，先删除货架信息（货架已初始化不能删除），否则不能删除
            try
            {
                using (var tran = bll_warehouse.BeginTransaction())
                {
                    try
                    {
                        var warehouseObj = bll_warehouse.GetFirstDefault(w => w.guid == guid);
                        if (warehouseObj != null)
                        {
                            //判断货架对应的库位是否有已初始化的数据
                            int iniShelfCount = bll_shelves.GetCount(w => w.isinitialized == true && w.warehouse_guid == warehouseObj.guid);
                            if (iniShelfCount > 0)
                            {   // 有：则不能删除
                                AddJsonError("仓库信息下有对应已初始化的货架记录，不能删除！");
                            }
                            else
                            {//没有：删除对应货架信息 / 库位详细
                                var rtv = bll_warehouse.Delete(warehouseObj);
                                // var rtv2 = bll_shelves.Delete(w => w.warehouse_guid == warehouseObj.guid);
                                var dellist = bll_shelves.GetAllQuery(w => w.warehouse_guid == warehouseObj.guid);
                                if (dellist != null && dellist.Count > 0)
                                {
                                    foreach (var item in dellist)
                                    {
                                        bll_shelves.Delete(item);
                                    }
                                }
                                // && rtv2 != LTWMSEFModel.SimpleBackValue.False
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    AddUserOperationLog("删除仓库信息guid：[" + guid + "],名称："+ warehouseObj.name, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                        }
                        else
                        {
                            AddJsonError("数据库不存在记录或已删除！");
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
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
    }
}