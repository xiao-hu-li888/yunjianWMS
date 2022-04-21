using LTWMSEFModel.Stock;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.Stock;
using LTWMSWebMVC.Areas.BasicData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class MatterTypeController : BaseController
    {
        stk_mattertypeBLL bll_stk_mattertype;
        CodeHelperService bll_CodeHelperService;
        public MatterTypeController(stk_mattertypeBLL bll_stk_mattertype, CodeHelperService bll_CodeHelperService)
        {
            this.bll_stk_mattertype = bll_stk_mattertype;
            this.bll_CodeHelperService = bll_CodeHelperService;
        }
        // GET: BasicData/MatterType
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetTreeView()
        {
            var model = bll_stk_mattertype.GetAllQueryOrderby(o => o.code, w => w.state == LTWMSEFModel.EntityStatus.Normal, true).Select(a =>
                      MapperConfig.Mapper.Map<stk_mattertype, MatterTypeModel>(a)).ToList();
            //if (model == null|| model.Count==0)
            //    return ErrorView("参数错误");  
            //重组树结构
            List<MatterTypeModel> TreeModel = new List<MatterTypeModel>();
            if (model != null && model.Count > 0)
            {
                RecursionTree(TreeModel, model, "");
            }
            return PartialView("TreeView", TreeModel);
        }

        public void RecursionTree(List<MatterTypeModel> TreeModel, List<MatterTypeModel> source, string parentcode)
        {
            List<MatterTypeModel> subs = source.Where(w => (w.parent_code ?? "") == parentcode).ToList();

            if (subs != null && subs.Count > 0)
            {
                foreach (var item in subs)
                {
                    TreeModel.Add(item);
                    item.SubItems = new List<MatterTypeModel>();
                    RecursionTree(item.SubItems, source, item.code);
                }
            }
        }
        [HttpGet]
        public ActionResult GetAddView(string gguid, string p_code)
        {
            var model = bll_stk_mattertype.GetFirstDefault(w => w.guid.ToString() == gguid);
            MatterTypeModel Md = null;
            if (model != null && model.guid != Guid.Empty)
            {
                ViewBag.isUpdate = true;
                //return ErrorView("参数错误");
                Md = MapperConfig.Mapper.Map<stk_mattertype, MatterTypeModel>(model);
                Md.OldRowVersion = model.rowversion;
            }
            else
            {
                Md = new MatterTypeModel();
                Md.parent_code = p_code;
            }
            return PartialView("Add", Md);
        }
        [HttpPost]
        public ActionResult Update(Data.MatterTypeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isUpdate = true;
                    stk_mattertype matterType = bll_stk_mattertype.GetFirstDefault(w => w.guid == model.guid);
                    if (matterType == null || matterType.guid == Guid.Empty)
                    {
                        isUpdate = false;
                        matterType = new stk_mattertype();
                    }
                    matterType.memo = model.memo;
                    matterType.name = model.name;
                    matterType.sort = model.sort;
                    LTWMSEFModel.SimpleBackValue rtv = LTWMSEFModel.SimpleBackValue.False;
                    if (isUpdate)
                    {//修改
                        matterType.updatedate = DateTime.Now;
                        matterType.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //rtv = bll_wh_warehouse_type.Update(wareType); 
                        rtv = bll_stk_mattertype.UpdateIfNotExists(matterType, w => w.code);
                    }
                    else
                    {//添加  
                        var rtnCode = bll_CodeHelperService.GetNewCodeByTablePrentCode(CodeTableEnum.stk_mattertype, model.parent_code);
                        matterType.code = rtnCode.result;
                        matterType.parent_code = model.parent_code;
                        matterType.code_full = "-";
                        matterType.code_num = Convert.ToInt32(rtnCode.data);
                        matterType.createdate = DateTime.Now;
                        matterType.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        matterType.guid = Guid.NewGuid();
                        matterType.state = LTWMSEFModel.EntityStatus.Normal;
                        rtv = bll_stk_mattertype.AddIfNotExists(matterType, w => w.code);
                    }
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在编码为" + matterType.code + "的记录");
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
                var Model = bll_stk_mattertype.GetFirstDefault(w => w.guid.ToString() == gg_guid);
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
            var lst = bll_stk_mattertype.GetAllQuery(w => w.code == parentcode || w.parent_code == parentcode);
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    if (item.code == parentcode)
                    {//删除当前项
                        bll_stk_mattertype.Delete(item);
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