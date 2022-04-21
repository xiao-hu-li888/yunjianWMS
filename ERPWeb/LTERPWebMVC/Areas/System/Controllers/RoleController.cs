using LTERPWebMVC.Areas.System.Data;
using LTERPEFModel.Basic;
using LTERPService.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.System.Controllers
{
    public class RoleController : BaseController
    {
        sys_roleBLL bll_role;
        sys_loginroleBLL bll_loginrole;
        public RoleController(sys_roleBLL bll_role, sys_loginroleBLL bll_loginrole)
        {
            this.bll_role = bll_role;
            this.bll_loginrole = bll_loginrole;
        }
        // GET: System/Role
        public ActionResult Index(sysRoleSearch Model)
        {
            Model.PageCont = bll_role.GetAllQuery(w => w.state == LTERPEFModel.EntityStatus.Normal)
                .Select(s => MapperConfig.Mapper.Map<sys_role, sysRoleModel>(s)).ToList();
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
                    info.state = LTERPEFModel.EntityStatus.Normal;

                    info.createdate = DateTime.Now;
                    info.createuser = LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();

                    var rtv = bll_role.AddIfNotExists(info, w => w.rolename);
                    if (rtv == LTERPEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加角色信息 guid：[" + info.guid + "]--角色名：[" + info.rolename + "]");
                        return JsonSuccess();
                    }
                    else if (rtv == LTERPEFModel.SimpleBackValue.ExistsOfAdd)
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
                        info.state = LTERPEFModel.EntityStatus.Normal;
                        info.updatedate = DateTime.Now;
                        info.updateuser = LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_role.Update(info);
                        if (rtv == LTERPEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("[" + LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName + "]修改角色信息 guid：[" + info.guid + "]--角色名：[" + info.rolename + "]");
                            return JsonSuccess();
                        }
                        else if (rtv == LTERPEFModel.SimpleBackValue.DbUpdateConcurrencyException)
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
                    if(objdel==null|| objdel.guid == Guid.Empty)
                    {
                        AddJsonError("数据库不存在记录或已删除！");
                        return JsonError();
                    }
                    var rtv = bll_role.Delete(objdel);
                    if (rtv == LTERPEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("[" + LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName + "]删除角色信息guid：[" + guid + "]");
                        return JsonSuccess();
                    }
                    else if (rtv == LTERPEFModel.SimpleBackValue.NotExistOfDelete)
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
            return View(_Md);
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
                    //并发控制（乐观锁）
                    rol.OldRowVersion = model.OldRowVersion;
                    var rtv = bll_role.Update(rol);
                    if (rtv == LTERPEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("[" + LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName + "]修改角色权限guid：[" + rol.guid + "]--角色名：[" + rol.rolename + "]--权限：[" + rol.permissiontext + "]");
                        return Index(new sysRoleSearch());
                    }
                    else if (rtv == LTERPEFModel.SimpleBackValue.DbUpdateConcurrencyException)
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
                    _errMess += "保存数据出错！异常信息：" +ex.Message;
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