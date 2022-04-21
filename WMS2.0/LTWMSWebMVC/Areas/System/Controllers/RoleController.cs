using LTWMSWebMVC.Areas.System.Data;
using LTWMSEFModel.Basic;
using LTWMSService.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Warehouse;
using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Areas.Setting.Data;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSWebMVC.App_Start.WebMvCEx;

namespace LTWMSWebMVC.Areas.System.Controllers
{
    public class RoleController : BaseController
    {
        sys_roleBLL bll_role;
        sys_loginroleBLL bll_loginrole;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        public RoleController(sys_roleBLL bll_role, sys_loginroleBLL bll_loginrole, wh_warehouse_typeBLL bll_wh_warehouse_type,
            wh_warehouseBLL bll_wh_warehouse)
        {
            this.bll_role = bll_role;
            this.bll_loginrole = bll_loginrole;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_warehouse = bll_wh_warehouse;
        }
        // GET: System/Role
        public ActionResult Index(sysRoleSearch Model)
        {
            ListDataManager.setWareHouseGuidList2(bll_wh_warehouse, bll_wh_warehouse_type);
            var ListWareHouse = ListProvider.GetList("WareHouseGuidList2");
            Model.PageCont = bll_role.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal)
                .Select(s =>
                {
                    var a = MapperConfig.Mapper.Map<sys_role, sysRoleModel>(s);
                    var wgt= LTLibrary.ConvertUtility.ParseToList(a.warehouse_guid_text);
                    if (wgt != null && wgt.Count > 0)
                    {
                        if (ListWareHouse != null && ListWareHouse.Count() > 0)
                        {
                            var lstWarehouse = ListWareHouse.Where(w => wgt.Contains(w.Value)).Select(s => s.Text).ToList();
                            if (lstWarehouse != null && lstWarehouse.Count > 0)
                            {
                                a.warehouse_permission_text = string.Join(",", lstWarehouse.ToArray());
                            }
                        }
                    }
                    return a;
                }).ToList();
            return View("Index", Model);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new sysRoleModel());
        }
        [HttpPost]
        public JsonResult Add(sysRoleModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    sys_role info = new sys_role();
                    info.active = model.active;
                    info.remark = model.remark;
                    info.rolename = model.rolename;
                    info.state = LTWMSEFModel.EntityStatus.Normal;

                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();

                    var rtv = bll_role.AddIfNotExists(info, w => w.rolename);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加角色信息 guid：[" + info.guid + "]--角色名：[" + info.rolename + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在角色名为：[" + info.rolename + "]的记录信息");
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
            var model = bll_role.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<sys_role, sysRoleModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(sysRoleModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    sys_role info = bll_role.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null)
                    {
                        info.active = model.active;
                        //   info.permissiontext = model.permissiontext;
                        info.remark = model.remark;
                        info.rolename = model.rolename;
                        info.state = LTWMSEFModel.EntityStatus.Normal;
                        info.updatedate = DateTime.Now;
                        info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_role.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改角色信息 guid：[" + info.guid + "]--角色名：[" + info.rolename + "]"
                                , LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
            try
            {
                //如果角色已关联用户则不能删除
                int loginCount = bll_loginrole.GetCount(w => w.roleguid == guid);
                if (loginCount > 0)
                {
                    AddJsonError("该角色已关联登录用户，请先解除关联关系！");
                }
                else
                {
                    //var rtv = bll_role.Delete(w => w.guid == guid);
                    var objdel = bll_role.GetFirstDefault(w => w.guid == guid);
                    if (objdel == null || objdel.guid == Guid.Empty)
                    {
                        AddJsonError("数据库不存在记录或已删除！");
                        return JsonError();
                    }
                    var rtv = bll_role.Delete(objdel);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("删除角色信息guid：[" + guid + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }

        [HttpGet]
        public ActionResult Permission(Guid guid)
        {
            sys_role rol = bll_role.GetFirstDefault(w => w.guid == guid);
            if (rol == null)
                return ErrorView("参数错误");
            var _Md = MapperConfig.Mapper.Map<sys_role, sysRoleModel>(rol);
            _Md.OldRowVersion = rol.rowversion;
            //处理仓库权限
            _Md.list_warehouse_guid_permision = LTLibrary.ConvertUtility.ParseToGuids(_Md.warehouse_guid_text);
            //查询所有仓库列表
            var list_warehouseModel = bll_wh_warehouse.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal)
                 .Select(a => MapperConfig.Mapper.Map<wh_warehouse, WareHouseModel>(a)).ToList();
            //查询所有仓库分区
            var model = bll_wh_warehouse_type.GetAllQueryOrderby(o => o.code, w => w.state == LTWMSEFModel.EntityStatus.Normal, true).Select(a =>
                      MapperConfig.Mapper.Map<wh_warehouse_type, WarehouseTypeModel>(a)).ToList();
            //重组树结构
            List<WarehouseTypeModel> TreeModel = new List<WarehouseTypeModel>();
            if (model != null && model.Count > 0)
            {
                RecursionTree(TreeModel, model, "", list_warehouseModel, _Md.list_warehouse_guid_permision);
            }
            _Md.list_warehouseTypeModel = TreeModel;

            return View(_Md);
        }
        public void RecursionTree(List<WarehouseTypeModel> TreeModel, List<WarehouseTypeModel> source, string parentcode, List<WareHouseModel> list_warehouseModel, List<Guid> permission_warehouseguid)
        {
            List<WarehouseTypeModel> subs = source.Where(w => (w.parent_code ?? "") == parentcode).ToList();

            if (subs != null && subs.Count > 0)
            {
                foreach (var item in subs)
                {
                    TreeModel.Add(item);
                    item.SubItems = new List<WarehouseTypeModel>();
                    //关联仓库列表
                    if (list_warehouseModel != null && list_warehouseModel.Count > 0)
                    {
                        item.ListWareHouse = list_warehouseModel.Where(w => w.warehouse_type_guid == item.guid).Select(s =>
                        {
                            if (permission_warehouseguid != null && permission_warehouseguid.Count > 0)
                            {
                                s.Checked = permission_warehouseguid.Contains(s.guid);
                            }
                            return s;
                        }).ToList();
                    }
                    RecursionTree(item.SubItems, source, item.code, list_warehouseModel, permission_warehouseguid);
                }
            }
        }
        [HttpPost]
        public ActionResult Permission(sysRoleModel model)
        {
            string _errMess = "";
            sys_role rol = bll_role.GetFirstDefault(w => w.guid == model.guid);
            if (rol != null)
            {
                try
                {
                    rol.permissiontext = Request.Form["PermissionText"];
                    rol.warehouse_guid_text = Request.Form["permissionWarehouseText"];
                    //并发控制（乐观锁）
                    rol.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_role.Update(rol);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("修改角色权限guid：[" + rol.guid + "]--角色名：[" + rol.rolename + "]--权限：[" + rol.permissiontext + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return Index(new sysRoleSearch());
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        _errMess += "数据并发异常，请重新加载数据然后再保存。";
                    }
                    else
                    {
                        _errMess += "保存失败";
                    }
                }
                catch (Exception ex)
                {
                    _errMess += "保存数据出错！异常信息：" + ex.Message;
                }
            }
            else
            {
                _errMess += "角色不存在或已删除！";
            }
            return ErrorView(_errMess);
        }
    }
}