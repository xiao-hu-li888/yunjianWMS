using LTWMSEFModel.Warehouse;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Setting.Controllers
{
    public class WCSSrvController : BaseController
    {
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        public WCSSrvController(wh_wcs_srvBLL bll_wh_wcs_srv, wh_warehouse_typeBLL bll_wh_warehouse_type, wh_warehouseBLL bll_wh_warehouse,
            wh_wcs_deviceBLL bll_wh_wcs_device)
        {
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_warehouse = bll_wh_warehouse;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
            ListDataManager.setWareHouseGuidList2(bll_wh_warehouse, bll_wh_warehouse_type);
            //  ListDataManager.setWareHouseGuidList(bll_wh_warehouse);
            ListDataManager.setWcsSrvGuidList(bll_wh_wcs_srv);
        }
        public ActionResult Index(WcsSrvSearch Model)
        {
            Model.PageCont = bll_wh_wcs_srv.GetAllQueryOrderby(o => o.createdate, w => w.state == LTWMSEFModel.EntityStatus.Normal, false)
                .Select(s => MapperConfig.Mapper.Map<wh_wcs_srv, WcsSrvModel>(s)).ToList();
            if (Model.PageCont != null && Model.PageCont.Count > 0)
            {
                foreach (var item in Model.PageCont)
                {
                    item.List_wcsDeviceModel = bll_wh_wcs_device.GetAllQueryOrderby(o => o.warehouse_guid, w => w.wcs_srv_guid == item.guid)
                        .Select(s => MapperConfig.Mapper.Map<wh_wcs_device, WcsDeviceModel>(s)).ToList();
                }
            }
            return View("Index", Model);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new WcsSrvModel());
        }
        [HttpPost]
        public JsonResult Add(WcsSrvModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    wh_wcs_srv info = new wh_wcs_srv();
                    info.createdate = DateTime.Now;
                    info.createuser = App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();
                    info.ip = model.ip;
                    info.code = model.code;
                    info.name = model.name;
                    info.port = model.port;
                    info.srv_type = model.srv_type;
                    info.state = LTWMSEFModel.EntityStatus.Normal;
                    info.u_identification = info.ip + "-" + info.port;
                    //  info.warehouse_guid = model.warehouse_guid;
                    //判断系统中是否存在code
                    if (bll_wh_wcs_srv.GetAny(w => w.code == info.code))
                    {
                        AddJsonError("WCS编号" + info.code + "已存在，请另外输入一个编号！");
                        return JsonError();
                    }
                    var rtv = bll_wh_wcs_srv.AddIfNotExists(info, w => w.u_identification);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加Wcs服务配置信息guid=" + info.guid + "，名称：" + info.name + ",ip:" + info.ip + ",端口:" + info.port, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在ip:" + info.ip + ",端口:" + info.port + " 的记录");
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
            var model = bll_wh_wcs_srv.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_wcs_srv, WcsSrvModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(WcsSrvModel model)
        {
            //ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    wh_wcs_srv info = bll_wh_wcs_srv.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null && info.guid != Guid.Empty)
                    {
                        //判断系统中是否存在code
                        if (bll_wh_wcs_srv.GetAny(w => w.code == model.code && w.guid != info.guid))
                        {
                            AddJsonError("WCS编号" + model.code + "已存在，请另外输入一个编号！");
                            return JsonError();
                        }
                        using (var tran = bll_wh_wcs_srv.BeginTransaction())
                        {
                            try
                            {
                                info.ip = model.ip;
                                info.name = model.name;
                                info.port = model.port;
                                info.srv_type = model.srv_type;
                                info.code = model.code;
                                info.state = LTWMSEFModel.EntityStatus.Normal;
                                info.u_identification = info.ip + "-" + info.port;
                                //  info.warehouse_guid = model.warehouse_guid;

                                info.updatedate = DateTime.Now;
                                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                //并发控制（乐观锁）
                                info.OldRowVersion = model.OldRowVersion;
                                var rtv = bll_wh_wcs_srv.UpdateIfNotExists(info, w => w.u_identification);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    bool _flag = false;
                                    //修改设备guid
                                    var listDevice = bll_wh_wcs_device.GetAllQuery(w => w.wcs_srv_guid == info.guid);
                                    if (listDevice != null && listDevice.Count > 0)
                                    {
                                        foreach (var item in listDevice)
                                        {
                                            //    item.warehouse_guid = info.warehouse_guid;
                                            item.updatedate = DateTime.Now;
                                            item.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                        }
                                        var rtv2 = bll_wh_wcs_device.Update(listDevice);
                                        if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                        {//修改成功
                                            _flag = true;
                                        }
                                    }
                                    else
                                    {
                                        _flag = true;
                                    }
                                    if (_flag)
                                    {
                                        tran.Commit();
                                        AddUserOperationLog("修改Wcs服务配置信息guid=" + info.guid + "，名称：" + info.name + ",ip:" + info.ip + ",端口:" + info.port, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                        return JsonSuccess();
                                    }
                                    else
                                    {
                                        tran.Rollback();
                                        AddJsonError("修改失败，请重试");
                                    }
                                }
                                else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                                {
                                    tran.Rollback();
                                    AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                                }
                                else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                                {
                                    tran.Rollback();
                                    AddJsonError("数据库已存在ip:" + info.ip + ",端口:" + info.port + " 的记录");
                                }
                                else
                                {
                                    tran.Rollback();
                                    AddJsonError("保存失败");
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                AddJsonError("异常>>>" + ex.ToString());
                            }
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
            try
            {
                //var rtv = bll_role.Delete(w => w.guid == guid);
                var objdel = bll_wh_wcs_srv.GetFirstDefault(w => w.guid == guid);
                if (objdel == null || objdel.guid == Guid.Empty)
                {
                    AddJsonError("数据库不存在记录或已删除！");
                    return JsonError();
                }
                var rtv = bll_wh_wcs_srv.Delete(objdel);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    AddUserOperationLog("成功删除Wcs服务配置 >>>信息guid=" + objdel.guid + "，名称：" + objdel.name + ",ip:" + objdel.ip + ",端口:" + objdel.port, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }

        /*********站台************/
        public ActionResult StationIndex(WcsDeviceSearch Model)
        {
            Model.PageCont = bll_wh_wcs_device.GetAllQueryOrderby(o => o.createdate, w => w.state == LTWMSEFModel.EntityStatus.Normal
            && w.wcs_srv_guid == Model.wcssrv_guid, false)
                .Select(s => MapperConfig.Mapper.Map<wh_wcs_device, WcsDeviceModel>(s)).ToList();

            return View("StationIndex", Model);
        }
        [HttpGet]
        public ActionResult StationAdd(Guid wcssrv_guid)
        {
            ViewBag.SubmitText = "添加";
            var Model = new WcsDeviceModel();
            Model.wcs_srv_guid = wcssrv_guid;
            //var Md = bll_wh_wcs_srv.GetFirstDefault(w => w.guid == Model.wcs_srv_guid);
            //if (Md != null && Md.guid != Guid.Empty)
            //{
            //  //  Model.warehouse_guid = Md.warehouse_guid;
            //}
            return PartialView("StationAdd", Model);
        }
        [HttpPost]
        public JsonResult StationAdd(WcsDeviceModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    wh_wcs_device info = new wh_wcs_device();
                    info.createdate = DateTime.Now;
                    info.createuser = App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();
                    info.default_out = model.default_out;
                    info.need_scan_bind = model.need_scan_bind;
                    info.name = model.name;
                    info.device_type = model.device_type;
                    info.number = model.number;
                    info.state = LTWMSEFModel.EntityStatus.Normal;
                    info.station_mode = model.station_mode;

                    info.wcs_srv_guid = model.wcs_srv_guid;
                    info.warehouse_guid = model.warehouse_guid;
                    info.u_identification = info.wcs_srv_guid + "-" + info.number;

                    var rtv = bll_wh_wcs_device.AddIfNotExists(info, w => w.u_identification);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加站台配置信息guid=" + info.guid + "，名称：" + info.name + ",编号:" + info.number, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在编号为" + info.number + "的记录");
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
        public ActionResult StationUpdate(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_wh_wcs_device.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_wcs_device, WcsDeviceModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("StationAdd", Md);
        }
        [HttpPost]
        public JsonResult StationUpdate(WcsDeviceModel model)
        {
            //ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    wh_wcs_device info = bll_wh_wcs_device.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null && info.guid != Guid.Empty)
                    {
                        info.default_out = model.default_out;
                        info.need_scan_bind = model.need_scan_bind;
                        info.name = model.name;
                        info.device_type = model.device_type;
                        info.number = model.number;
                        info.state = LTWMSEFModel.EntityStatus.Normal;
                        info.station_mode = model.station_mode;

                        //  info.wcs_srv_guid = model.wcs_srv_guid;
                        info.warehouse_guid = model.warehouse_guid;
                        info.u_identification = info.wcs_srv_guid + "-" + info.number;

                        info.updatedate = DateTime.Now;
                        info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;
                        var rtv = bll_wh_wcs_device.UpdateIfNotExists(info, w => w.u_identification);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改站台配置信息guid=" + info.guid + "，名称：" + info.name + ",编号:" + info.number, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                        {
                            AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                        {
                            AddJsonError("数据库已存在编号为" + info.number + "的记录");
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
        public JsonResult StationDeletePost(Guid guid)
        {
            try
            {
                //var rtv = bll_role.Delete(w => w.guid == guid);
                var objdel = bll_wh_wcs_device.GetFirstDefault(w => w.guid == guid);
                if (objdel == null || objdel.guid == Guid.Empty)
                {
                    AddJsonError("数据库不存在记录或已删除！");
                    return JsonError();
                }
                var rtv = bll_wh_wcs_device.Delete(objdel);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    AddUserOperationLog("成功删除站台配置信息 >>> guid=" + objdel.guid + "，名称：" + objdel.name + ",编号:" + objdel.number, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
    }
}