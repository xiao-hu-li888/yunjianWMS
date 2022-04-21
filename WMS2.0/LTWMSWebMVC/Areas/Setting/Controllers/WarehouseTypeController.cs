using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Setting.Controllers
{
    public class WarehouseTypeController : BaseController
    {
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        CodeHelperService bll_CodeHelperService;
        public WarehouseTypeController(wh_warehouse_typeBLL bll_wh_warehouse_type, CodeHelperService bll_CodeHelperService)
        {
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_CodeHelperService = bll_CodeHelperService;
        }
        // GET: Setting/WarehouseType
        public ActionResult Index()
        {
            //  cfg.CreateMap<wh_warehouse_type, WarehouseTypeModel > ().ReverseMap();
            return View();
        }
        [HttpGet]
        public ActionResult GetTreeView()
        {
            var model = bll_wh_warehouse_type.GetAllQueryOrderby(o => o.code, w => w.state == LTWMSEFModel.EntityStatus.Normal, true).Select(a =>
                      MapperConfig.Mapper.Map<wh_warehouse_type, WarehouseTypeModel>(a)).ToList();
            //if (model == null|| model.Count==0)
            //    return ErrorView("参数错误");  
            //重组树结构
            List<WarehouseTypeModel> TreeModel = new List<WarehouseTypeModel>();
            if (model != null && model.Count > 0)
            {
                RecursionTree(TreeModel, model, "");
            }
            return PartialView("TreeView", TreeModel);
        }
        public void RecursionTree(List<WarehouseTypeModel> TreeModel, List<WarehouseTypeModel> source, string parentcode)
        {
            List<WarehouseTypeModel> subs = source.Where(w => (w.parent_code ?? "") == parentcode).ToList();

            if (subs != null && subs.Count > 0)
            {
                foreach (var item in subs)
                {
                    TreeModel.Add(item);
                    item.SubItems = new List<WarehouseTypeModel>();
                    RecursionTree(item.SubItems, source, item.code);
                }
            }
        }

        [HttpGet]
        public ActionResult GetAddView(string gguid, string p_code)
        {
            var model = bll_wh_warehouse_type.GetFirstDefault(w => w.guid.ToString() == gguid);
            WarehouseTypeModel Md = null;
            if (model != null && model.guid != Guid.Empty)
            {
                ViewBag.isUpdate = true;
                //return ErrorView("参数错误");
                Md = MapperConfig.Mapper.Map<wh_warehouse_type, WarehouseTypeModel>(model);
                Md.OldRowVersion = model.rowversion;
            }
            else
            {
                Md = new WarehouseTypeModel();
                Md.parent_code = p_code;
            }
            return PartialView("Add", Md);
        }
        [HttpPost]
        public ActionResult Update(Data.WarehouseTypeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isUpdate = true;
                    wh_warehouse_type wareType = bll_wh_warehouse_type.GetFirstDefault(w => w.guid == model.guid);
                    if (wareType == null || wareType.guid == Guid.Empty)
                    {
                        isUpdate = false;
                        wareType = new wh_warehouse_type();
                    }
                    wareType.memo = model.memo;
                    wareType.name = model.name;
                    wareType.sort = model.sort;
                    LTWMSEFModel.SimpleBackValue rtv = LTWMSEFModel.SimpleBackValue.False;
                    if (isUpdate)
                    {//修改
                        wareType.updatedate = DateTime.Now;
                        wareType.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //rtv = bll_wh_warehouse_type.Update(wareType); 
                        rtv = bll_wh_warehouse_type.UpdateIfNotExists(wareType, w => w.code);
                    }
                    else
                    {//添加  
                        var rtnCode = bll_CodeHelperService.GetNewCodeByTablePrentCode(CodeTableEnum.wh_warehouse_type, model.parent_code);
                        wareType.code = rtnCode.result;
                        wareType.parent_code = model.parent_code;
                        wareType.code_num = Convert.ToInt32(rtnCode.data);
                        wareType.createdate = DateTime.Now;
                        wareType.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        wareType.guid = Guid.NewGuid();
                        wareType.state = LTWMSEFModel.EntityStatus.Normal;
                        rtv = bll_wh_warehouse_type.AddIfNotExists(wareType, w => w.code);
                    }
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在编码为" + wareType.code + "的记录");
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据。");
                    }
                    else
                    {
                        AddJsonError("修改失败，请重试");
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
        public ActionResult DeleteTreeItem(string gg_guid)
        {
            try
            {
                var Model = bll_wh_warehouse_type.GetFirstDefault(w => w.guid.ToString() == gg_guid);
                if (Model != null && Model.guid != Guid.Empty)
                {
                    //删除当前、及所有子项
                    // AddJsonError("删除1111失败");
                    RecursionDelete(Model.code);
                    return JsonSuccess();
                }
                else
                {//数据库不存在记录
                    AddJsonError("参数错误，数据库不存在或已删除。");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        public void RecursionDelete(string parentcode)
        {
            var lst = bll_wh_warehouse_type.GetAllQuery(w => w.code == parentcode || w.parent_code == parentcode);
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    if (item.code == parentcode)
                    {//删除当前项
                        bll_wh_warehouse_type.Delete(item);
                    }
                    else
                    {//删除子项 及下一级子项
                        //bll_wh_warehouse_type.Delete(item);
                        RecursionDelete(item.code);
                    }
                }

            }
        }
    }
}