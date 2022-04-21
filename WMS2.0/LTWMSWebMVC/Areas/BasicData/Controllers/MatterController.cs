using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel.Stock;
using LTWMSService.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSWebMVC.Models.Erp;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class MatterController : BaseController
    {
        stk_matterBLL bll_stk_matter;
        stk_mattertypeBLL bll_stk_mattertype;
        public MatterController(stk_matterBLL bll_stk_matter, stk_mattertypeBLL bll_stk_mattertype)
        {
            this.bll_stk_matter = bll_stk_matter;
            this.bll_stk_mattertype = bll_stk_mattertype;
            ListDataManager.setMatterTypeGuidList(bll_stk_mattertype);
        }
        // GET: BasicData/Matter
        public ActionResult Index(MatterSearch Model)
        {
            int TotalSize = 0;
            var aa = bll_stk_matter.Pagination(Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize, o => o.guid,
               w => (Model.s_keywords == "" || (w.code ?? "").Contains(Model.s_keywords)
               || (w.code ?? "").Contains(Model.s_keywords) || (w.name ?? "").Contains(Model.s_keywords)
               || (w.name_pinyin ?? "").Contains(Model.s_keywords) || (w.memo ?? "").Contains(Model.s_keywords)
               )
               && (Model.s_specs == "" || (w.specs ?? "").Contains(Model.s_specs))
               && ((Model.s_mattertype_guid == null || Model.s_mattertype_guid.Value.ToString() == "-1") || w.mattertype_guid == Model.s_mattertype_guid)
                 , true);
            Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<stk_matter, MatterModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };

            return View("Index", Model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加";
            return PartialView(new MatterModel());
        }
        [HttpPost]
        public JsonResult Add(MatterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    stk_matter info = new stk_matter();
                    info.code = model.code; 
                    info.convert_ratio = model.convert_ratio;
                    info.effective_date = model.effective_date;
                    info.mattertype_guid = model.mattertype_guid;
                    info.mattertype_name = model.mattertype_name;
                    info.memo = model.memo;
                    info.name = model.name;
                    // info.name_pinyin= model.name_pinyin;
                    info.specs = model.specs;
                    info.description = model.description;
                    info.stock_max = model.stock_max;
                    info.stock_min = model.stock_min;
                    info.unit_convert = model.unit_convert;
                    info.unit_measurement = model.unit_measurement;
                    //info.can_delete = true;

                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();
                    info.state = LTWMSEFModel.EntityStatus.Normal;

                    var rtv = bll_stk_matter.AddIfNotExists(info, w => w.code);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加物料信息guid：[" + info.guid + "]--名称：[" + info.name + "]--编码：[" + info.code + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在物料编码为：[" + info.code + "]的记录信息");
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
            var model = bll_stk_matter.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<stk_matter, MatterModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(MatterModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    using (var tran = bll_stk_matter.BeginTransaction())
                    {
                        try
                        {
                            stk_matter info = bll_stk_matter.GetFirstDefault(w => w.guid == model.guid);
                            if (info != null)
                            {
                                //info.barcode = model.barcode;
                                //  info.code = model.code;
                                //info.convert_ratio = model.convert_ratio;
                                //info.effective_date = model.effective_date;
                                //info.mattertype_guid = model.mattertype_guid;
                                info.memo = model.memo;
                                info.name = model.name;
                                info.mattertype_name = model.mattertype_name;
                                info.specs = model.specs;
                                info.stock_max = model.stock_max;
                                info.stock_min = model.stock_min;
                                info.description = model.description;

                                //info.unit_convert = model.unit_convert;
                                info.unit_measurement = model.unit_measurement;

                                info.updatedate = DateTime.Now;
                                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                //并发控制（乐观锁）
                                info.OldRowVersion = model.OldRowVersion;

                                var rtv = bll_stk_matter.Update(info);
                                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    tran.Commit();
                                    AddUserOperationLog("修改物料信息guid：[" + info.guid + "]--名称：[" + info.name + "]--编码：[" + info.code + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                            //tran.Rollback();
                            WMSFactory.Log.v(ex.ToString());
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
                var _Obj = bll_stk_matter.GetFirstDefault(w => w.guid == guid);
                if (_Obj != null&& _Obj.guid!=Guid.Empty)
                {
                    //if (_Obj.can_delete!=true)
                    //{//不能删除
                    //    AddJsonError("基础物料信息不能删除！");
                    //}
                    //else
                    //{ 
                    var rtv = bll_stk_matter.Delete(_Obj);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("删除物料信息guid：[" + guid + "] 名称：" + _Obj.name+",编码："+_Obj.code, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                    // }
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
        /// 同步ERP物料数据信息
        /// </summary>
        /// <returns></returns>
        public JsonResult SynchroERPMatterData()
        {
            try
            {
                //http://localhost:8803/api/matter/getallmatter
                //  string txt = LTLibrary.HttpRequestHelper.HttpGet("http://localhost:8803/api/matter/getallmatter"); 
                //string txt = LTLibrary.HttpRequestHelper.HttpGet("http://localhost:8093/api/matter/getallmatter");
                string txt = LTLibrary.HttpRequestHelper.HttpGet(WMSFactory.Config.ErpMatterUrl);
                var objData = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseBase<MatterModel>>(txt);
                if (objData != null && objData.data != null && objData.data.Count > 0)
                {
                    foreach (var item in objData.data)
                    {
                        stk_matter Md = MapperConfig.Mapper.Map<MatterModel, stk_matter>(item);
                        if (Md != null)
                        {
                            //编码唯一
                            var existObj = bll_stk_matter.GetFirstDefault(w => w.code == Md.code);
                            if (existObj != null)
                            {//修改
                                bll_stk_matter.Update(Md);
                            }
                            else
                            {
                                //新增
                                bll_stk_matter.Add(Md);
                            }
                        }
                        /* var md= new stk_matter(); 
                          md.brand_guid = item.brand_guid;
                          md.brand_name = item.brand_name;
                          md.can_mix = item.can_mix;
                          md.code = item.code;
                          md.convert_ratio = item.convert_ratio;
                          if (item.createdate != null)
                          {
                              md.createdate = item.createdate.Value;
                          }
                          else
                          {
                              md.createdate = DateTime.Now;
                          }
                          md.createuser = item.createuser;
                          md.def_warehouse_guid = item.def_warehouse_guid;
                          md.description = item.description;
                          md.effective_date = item.effective_date;
                          md.guid = item.guid;
                          md.mattertype_guid = item.mattertype_guid;
                          md.mattertype_name = item.mattertype_name;
                          md.memo = item.memo;
                          md.name = item.name;
                          md.name_pinyin = item.name_pinyin;
                          md.OldRowVersion = item.OldRowVersion;
                          md.specs = item.specs;
                          if (item.state != null)
                          {
                              md.state = item.state.Value;
                          }
                          else
                          {
                              md.state = LTWMSEFModel.EntityStatus.Deleted;
                          }
                          md.std_price = item.std_price;
                          md.std_weight = item.std_weight;
                          md.stock_max = item.stock_max;
                          md.stock_min = item.stock_min;
                          md.unit_convert = item.unit_convert;
                          md.unit_measurement = item.unit_measurement;
                          md.unit_measurement_guid = item.unit_measurement_guid;
                          md.updatedate = item.updatedate;
                          md.updateuser = item.updateuser;
                          bll_stk_matter.Add(md);*/
                    }

                    return JsonSuccess();
                }
                else
                {
                    AddJsonError("解析数据失败！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("同步数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 导入物料数据（Excel文件）
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportMatterInfo()
        {
            string _message = "";
            //InportFile1
            HttpPostedFileBase File = Request.Files["InportFile1"];
            if (File.ContentLength > 0)
            {
                var Isxls = LTLibrary.ConvertUtility.GetFileSuffix(File.FileName);
                if (Isxls != ".xls" && Isxls != ".xlsx")
                {
                    //Content("请上传Excel文件");
                    _message = "请上传Excel文件";
                }
                else
                {
                    var FileName = File.FileName;//获取文件夹名称
                    var path = Server.MapPath("~/FileExcel/" + DateTime.Now.ToString("yyyyMM") + "/");
                    //盘点目录是否存在
                    LTLibrary.FileHelper.CreateDirectoryIfNotExists(path);
                    string fileName = path + DateTime.Now.ToString("yyyyMMddHHmmss") + Isxls;
                    File.SaveAs(fileName);//将文件保存到服务器 
                                          //物料编码	名称	类型	规格	型号	准字
                    var taleExcel = LTLibrary.ExcelHelper.ExcelToDataTable(fileName);
                    if (taleExcel != null && taleExcel.Rows.Count > 0)
                    {
                        for (int i = 0; i < taleExcel.Rows.Count; i++)
                        {
                            var matter = new stk_matter();
                            matter.code = Convert.ToString(taleExcel.Rows[i]["物料编码"]);
                            matter.name = Convert.ToString(taleExcel.Rows[i]["名称"]);
                            matter.mattertype_name = Convert.ToString(taleExcel.Rows[i]["类型"]);
                            matter.specs = Convert.ToString(taleExcel.Rows[i]["规格"]);
                            matter.description = Convert.ToString(taleExcel.Rows[i]["最小包装单位"]);
                            //matter.brand_name = Convert.ToString(taleExcel.Rows[i]["准字"]);
                            matter.createdate = DateTime.Now;
                            matter.guid = Guid.NewGuid();
                            matter.state = LTWMSEFModel.EntityStatus.Normal;

                            bll_stk_matter.AddIfNotExists(matter, w => w.code);
                        }
                        AddUserOperationLog("导入物料数据（Excel文件）", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        _message = "上传成功";
                    }
                    else
                    {
                        _message = "数据解析失败";
                    }
                }
            }
            else
            {
                _message = "上传的文件为空";
            }
            ViewData["show_message"] = _message;
            return Index(new MatterSearch());
        }
    }
}