using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.System.Data;
using LTWMSEFModel.Basic;
using LTWMSService.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace LTWMSWebMVC.Areas.System.Controllers
{
    public class UserController : BaseController
    {
        sys_loginBLL bll_sys_login;
        sys_roleBLL bll_sys_role;
        public UserController(sys_loginBLL bll_sys_login, sys_roleBLL bll_sys_role)
        {
            this.bll_sys_login = bll_sys_login;
            this.bll_sys_role = bll_sys_role;
            //设置角色 
            ListDataManager.setRoleListData(bll_sys_role);
        }
        // GET: System/User
        public ActionResult Index(sysloginSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_sys_login.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.createdate,
                w => (Model.s_keywords == "" || (w.loginname ?? "").Contains(Model.s_keywords)
                || (w.username ?? "").Contains(Model.s_keywords))
                  && (((Model.s_state == null || (int)Model.s_state.Value == -1) && w.state != LTWMSEFModel.EntityStatus.Deleted) || w.state == Model.s_state)
                        , false).Select(s => MapperConfig.Mapper.Map<sys_login, sysloginModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            //绑定角色信息
            if (Model.PageCont != null && Model.PageCont.Count > 0)
            {
                foreach (var item in Model.PageCont)
                {
                    var guidList = bll_sys_login.GetLoginRoles(item.guid).Select(a => a.guid).ToList();
                    item.ex_role = string.Join(",", guidList.ToArray());
                }
            }
            return View(Model);
        }
        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new sysloginModel());
        }
        [HttpPost]
        public JsonResult Add(sysloginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(!string.IsNullOrWhiteSpace(model.password)&&model.password.Length<8)
                    {
                        AddJsonError("密码长度不能小于8");
                        return JsonError();
                    }
                    //判断密码复杂度 包含数字和字母
                    if (!string.IsNullOrWhiteSpace(model.password)&&(!LTLibrary.ConvertUtility.ContainsABC(model.password) ||
                         !LTLibrary.ConvertUtility.ContainsNumber(model.password)))
                    {
                        AddJsonError("密码必须包含字母和数字");
                        return JsonError();
                    }
                    using (var _tran = bll_sys_login.BeginTransaction())
                    {
                        try
                        {
                            sys_login info = new sys_login();
                            info.loginname = model.loginname;
                            info.username = model.username;
                            info.position = model.position;
                            info.mobile = model.mobile;
                            info.gender = model.gender;
                            info.issuperadmin = model.issuperadmin;
                            if (model.state != null)
                            {
                                info.state = model.state.Value;
                            }
                            if (!string.IsNullOrWhiteSpace(model.password))
                            {
                                info.password = bll_sys_login.SHA256(model.password);
                                info.updatedate = DateTime.Now;
                            }
                            else
                            {//默认123456
                                info.password = bll_sys_login.SHA256("123456");
                            }
                            info.createdate = DateTime.Now;
                            info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                            info.guid = Guid.NewGuid();
                            var rtv = bll_sys_login.AddIfNotExists(info, w => w.loginname);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                var rtv2 = bll_sys_login.AddRoles(Request.Form["ex_role"], info.guid);
                                if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    AddUserOperationLog("添加登录用户信息guid：[" + info.guid + "]--登录名：[" + info.loginname + "]--姓名：[" + info.username + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                    _tran.Commit();
                                    return JsonSuccess();
                                }
                                else
                                {
                                    AddJsonError("角色信息添加失败！");
                                }
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                            {
                                AddJsonError("数据库已存在登录名为：[" + info.loginname + "]的记录信息");
                            }
                            else
                            {
                                AddJsonError("保存失败");
                            }
                        }catch(Exception ex)
                        {
                            WMSFactory.Log.v(ex);
                            AddJsonError("异常："+ex.ToString());
                        }
                        _tran.Rollback();
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
            var model = bll_sys_login.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<sys_login, sysloginModel>(model);
            Md.OldRowVersion = model.rowversion;
            var guidList = bll_sys_login.GetLoginRoles(model.guid).Select(a => a.guid).ToList();
            Md.ex_role = string.Join(",", guidList.ToArray());
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(sysloginModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(model.password) && model.password.Length < 8)
                    {
                        AddJsonError("密码长度不能小于8");
                        return JsonError();
                    }
                    //判断密码复杂度 包含数字和字母
                    if (!string.IsNullOrWhiteSpace(model.password) && (!LTLibrary.ConvertUtility.ContainsABC(model.password) ||
                         !LTLibrary.ConvertUtility.ContainsNumber(model.password)))
                    {
                        AddJsonError("密码必须包含字母和数字");
                        return JsonError();
                    }
                    using (var _tran = bll_sys_login.BeginTransaction())
                    {
                        try
                        {
                            sys_login info = bll_sys_login.GetFirstDefault(w => w.guid == model.guid);
                            if (info != null)
                            {
                                //info.updatedate = DateTime.Now;
                                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                info.loginname = model.loginname;
                                info.username = model.username;
                                info.position = model.position;
                                info.mobile = model.mobile;
                                info.gender = model.gender;
                                info.issuperadmin = model.issuperadmin;
                                //并发控制（乐观锁）
                                info.OldRowVersion = model.OldRowVersion;
                                if (model.state != null)
                                {
                                    info.state = model.state.Value;
                                }
                                if (!string.IsNullOrWhiteSpace(model.password))
                                {
                                    info.password = bll_sys_login.SHA256(model.password);
                                    info.updatedate = DateTime.Now;
                                }
                                var rtv = bll_sys_login.Update(info);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    var rtv2 = bll_sys_login.AddRoles(Request.Form["ex_role"], info.guid);
                                    if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                    {
                                        AddUserOperationLog("修改登录用户信息guid：[" + info.guid + "]--登录名：[" + info.loginname + "]--姓名：[" + info.username + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                        _tran.Commit();
                                        return JsonSuccess();
                                    }
                                    else
                                    {
                                        AddJsonError("角色信息添加失败！");
                                    }
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
                        }catch(Exception ex)
                        {
                            WMSFactory.Log.v(ex);
                            AddJsonError("异常："+ex.ToString());
                        }
                        _tran.Rollback();
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
                //var rtv = bll_sys_login.Delete(w => w.guid == guid);
                var delobj = bll_sys_login.GetFirstDefault(w => w.guid == guid);
                if(delobj==null|| delobj.guid==Guid.Empty)
                {
                    AddJsonError("数据库不存在记录或已删除！");
                    return JsonError();
                }
                var rtv=bll_sys_login.Delete(delobj);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    AddUserOperationLog("删除登录用户信息guid：[" + guid + "]--登录名：[" + delobj.loginname + "]--姓名：[" + delobj.username + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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