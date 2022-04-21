using LTWMSEFModel.Warehouse;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Setting.Controllers
{
    public class ShelvesWcsDeviceController : BaseController
    {
        wh_warehouseBLL bll_warehouse;
        wh_shelvesBLL bll_shelves;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        wh_shelves_devBLL bll_wh_shelves_dev;
        public ShelvesWcsDeviceController(wh_warehouseBLL bll_warehouse, wh_shelvesBLL bll_shelves, wh_warehouse_typeBLL bll_wh_warehouse_type
            , wh_wcs_srvBLL bll_wh_wcs_srv, wh_wcs_deviceBLL bll_wh_wcs_device, wh_shelves_devBLL bll_wh_shelves_dev)
        {
            this.bll_warehouse = bll_warehouse;
            this.bll_shelves = bll_shelves;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
            this.bll_wh_shelves_dev = bll_wh_shelves_dev;
            //设置仓库列表
            ListDataManager.setWareHouseGuidList2(bll_warehouse, bll_wh_warehouse_type);
        }
        // GET: Setting/ShelvesWcsDevice
        public ActionResult Index(ShelvesSearch Model)
        {//只有立库才需要配置
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
            Model.PageCont = bll_shelves.GetAllQueryOrderby(o => o.rack, w => w.state == LTWMSEFModel.EntityStatus.Normal
            && w.isinitialized == true && w.category == WareHouseCategoriesEnum.AutomatedWarehouse, true)
                .Select(s =>
                {
                    var ShelfModel = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(s);
                    BindShelfWcsDevice(ShelfModel);
                    return ShelfModel;
                }).ToList();
            return View(Model);
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
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
            var Md = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(model);
            Md.OldRowVersion = model.rowversion;
            //加载对应wcs的设备信息

            return PartialView("Update", Md);
        }
        [HttpPost]
        public JsonResult Update(ShelvesModel model)
        {
            //ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    using (var tran = bll_shelves.BeginTransaction())
                    {
                        try
                        {
                            wh_shelves info = bll_shelves.GetFirstDefault(w => w.guid == model.guid);
                            if (info != null)
                            {
                                info.out_logic = model.out_logic;
                                info.wcs_srv_guid = model.wcs_srv_guid;
                                info.stock_distribute = model.stock_distribute;
                                info.updatedate = DateTime.Now;
                                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                info.rack_of_wcs = model.rack_of_wcs;
                                info.columns_reversal_wcs = model.columns_reversal_wcs;
                                info.columns_offset_wcs = model.columns_offset_wcs;
                                info.rows_offset_wcs = model.rows_offset_wcs;
                                
                                //并发控制（乐观锁）
                                info.OldRowVersion = model.OldRowVersion;
                                var rtv = bll_shelves.Update(info);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    ////保存设备关联关系
                                    ////删除设备关联关系
                                    ////重新保存关联关系
                                    /////////////////////////////
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

        public ActionResult DeviceBindView(Guid guid)
        {
            var Model = bll_shelves.GetFirstDefault(w => w.guid == guid);
            if (Model == null)
                return ErrorView("参数错误");
            var DataModel = MapperConfig.Mapper.Map<wh_shelves, ShelvesModel>(Model);
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
            BindShelfWcsDevice(DataModel);
            DataModel.OldRowVersion = Model.rowversion;
            return View(DataModel);
        }
        private void BindShelfWcsDevice(ShelvesModel DataModel)
        {
           // ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
            DataModel.WcsSrvModel = MapperConfig.Mapper.Map<wh_wcs_srv, Data.WcsSrvModel>(bll_wh_wcs_srv.GetFirstDefault(w => w.guid == DataModel.wcs_srv_guid));
            if (DataModel.WcsSrvModel != null && DataModel.WcsSrvModel.guid != Guid.Empty)
            {
                var ListConnects = bll_wh_shelves_dev.GetAllQuery(w => w.shelves_guid == DataModel.guid);
                DataModel.WcsSrvModel.List_wcsDeviceModel = bll_wh_wcs_device.GetAllQueryOrderby(o => o.createdate, w => w.state == LTWMSEFModel.EntityStatus.Normal
               && w.wcs_srv_guid == DataModel.wcs_srv_guid&&w.warehouse_guid==DataModel.warehouse_guid).Select(s =>
               {
                   var Obj = MapperConfig.Mapper.Map<wh_wcs_device, Data.WcsDeviceModel>(s);
                   if (ListConnects.Exists(w => w.wcs_device_guid == Obj.guid))
                   {//货架已关联该设备
                       Obj.IsChecked = true;
                   }
                   else
                   {
                       Obj.IsChecked = false;
                   }
                   return Obj;
               }).ToList();
            }
        }
        [HttpPost]
        public ActionResult DeviceBindView(ShelvesModel model)
        {
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
            string _mess = "";
            try
            {
                var shelfModel = bll_shelves.GetFirstDefault(w => w.guid == model.guid);
                if (shelfModel != null && shelfModel.guid != Guid.Empty)
                {
                    using (var tran = bll_shelves.BeginTransaction())
                    {
                        try
                        {
                            string check_device_guids = Request.Form["chk_guids"];
                            List<Guid> lstGuid = LTLibrary.ConvertUtility.ParseToGuids(check_device_guids);
                            //
                            shelfModel.OldRowVersion = model.OldRowVersion;
                            shelfModel.updatedate = DateTime.Now;
                            shelfModel.updateuser = App_Start.AppCode.CurrentUser.UserName;
                            var rtv = bll_shelves.Update(shelfModel);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                bool flag2 = false;
                                //修改成功，删除对应的绑定信息
                                var ListConnects = bll_wh_shelves_dev.GetAllQuery(w => w.shelves_guid == shelfModel.guid);
                                if (ListConnects != null && ListConnects.Count > 0)
                                {
                                    foreach (var item in ListConnects)
                                    {
                                        bll_wh_shelves_dev.Delete(item);
                                    }
                                }
                                // 重新绑定设备信息
                                if (lstGuid != null && lstGuid.Count > 0)
                                {
                                    List<wh_shelves_dev> lstDev = new List<wh_shelves_dev>();
                                    foreach (var item in lstGuid)
                                    {
                                        var Md = new wh_shelves_dev();
                                        Md.guid = Guid.NewGuid();
                                        Md.shelves_guid = shelfModel.guid;
                                        Md.warehouse_guid = shelfModel.warehouse_guid;
                                        Md.wcs_device_guid = item;
                                        lstDev.Add(Md);
                                    }
                                    //添加绑定信息
                                    var rtv2 = bll_wh_shelves_dev.AddRange(lstDev);
                                    if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                    {
                                        flag2 = true;
                                    }
                                }
                                else
                                {
                                    flag2 = true;
                                }
                                if (flag2)
                                {
                                    tran.Commit();
                                    _mess = "数据保存成功！";
                                }
                                else
                                {
                                    tran.Rollback();
                                    _mess = "操作失败，请重试";
                                }
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                tran.Rollback();
                                _mess = "数据并发异常，请刷新重试！";
                            }
                            else
                            {
                                tran.Rollback();
                                _mess = "修改失败，请重试。";
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            _mess += ";保存失败，异常>>>" + ex.ToString();
                        }
                    }
                }
                else
                {
                    //参数错误
                    _mess = "货架guid参数错误，系统不存在或已删除";
                }
            }
            catch (Exception ex)
            {
                _mess += ";保存失败，异常>>>" + ex.ToString();
            }
            ViewBag.Mess = _mess;
            return DeviceBindView(model.guid);
        }
    }
}