using LTERPWebMVC.App_Start.DBSqlServer;
using LTERPService.Basic;
using LTERPService.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.Stock.Controllers
{
    public class DataImportController : BaseController
    {
        stk_mattertypeBLL bll_stk_mattertype;
        stk_matterBLL bll_stk_matter;
        sys_dictionaryBLL bll_sys_dictionary;
        public DataImportController(stk_matterBLL bll_stk_matter, stk_mattertypeBLL bll_stk_mattertype, sys_dictionaryBLL bll_sys_dictionary)
        {
            this.bll_stk_matter = bll_stk_matter;
            this.bll_stk_mattertype = bll_stk_mattertype;
            this.bll_sys_dictionary = bll_sys_dictionary;
        }
        // GET: Stock/DataImport
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DataImports()
        {
            //ViewBag.SubmitText = "添加";
            return PartialView();
        }
        [HttpPost]
        public JsonResult DataImports2()
        {
            try
            {
                var dbcontext = new LTWmsOld_DBContext();
                LTWmsOldBLL wmsoldbll = new LTWmsOldBLL(dbcontext);
                var lstmatter=wmsoldbll.GetAllMatter();
                var lstMatterC = wmsoldbll.GetAllMatterClass();
                var lstBrand = wmsoldbll.GetAllDictByType( DictType.brand);
                var lstUnit = wmsoldbll.GetAllDictByType(DictType.Unit);
                if (lstMatterC != null && lstMatterC.Count > 0)
                {
                    var lstMt = new List<LTERPEFModel.Stock.stk_mattertype>();
                    foreach (var item in lstMatterC)
                    {
                        var mattertypeM=new LTERPEFModel.Stock.stk_mattertype();
                        mattertypeM.code = item.MatCCode;
                        mattertypeM.createuser = "wms-zhanghui";
                        if (item.CreateDate != null)
                        {
                            mattertypeM.createdate = item.CreateDate.Value;
                        }
                        else
                        {
                            mattertypeM.createdate = DateTime.Now;
                        }
                        mattertypeM.guid =Guid.Parse(item.Guid);
                        mattertypeM.name = item.MatCName;
                        mattertypeM.state = LTERPEFModel.EntityStatus.Normal;
                        mattertypeM.updatedate = DateTime.Now;
                        mattertypeM.updateuser = "erp-admin";

                        lstMt.Add(mattertypeM);
                    }
                    
                    if (lstMt != null && lstMt.Count > 0)
                    {
                        var rtv = bll_stk_mattertype.AddRange(lstMt);
                        //添加品牌
                        if (lstBrand != null && lstBrand.Count > 0) 
                        {
                            foreach (var item in lstBrand)
                            {
                                var dic_brand = new LTERPEFModel.Basic.sys_dictionary();
                                dic_brand.guid = Guid.Parse(item.GUID);
                                dic_brand.desc = item.Remark;
                                dic_brand.parent_id = LTLibrary.EnumHelper.GetFullName(LTERPEFModel.Basic.DictionaryParentEnum.Brand);
                                dic_brand.status = LTERPEFModel.Basic.DicStatus.Normal;
                                dic_brand.sort = item.OrderNumb??0;
                                dic_brand.text = item.PubName; 
                                bll_sys_dictionary.Add(dic_brand);
                            }
                           
                        }
                        //添加单位
                        if (lstUnit != null && lstUnit.Count > 0)
                        {
                            foreach (var item in lstUnit)
                            {
                                var dic_brand = new LTERPEFModel.Basic.sys_dictionary();
                                dic_brand.guid = Guid.Parse(item.GUID);
                                dic_brand.desc = item.Remark;
                                dic_brand.parent_id = LTLibrary.EnumHelper.GetFullName(LTERPEFModel.Basic.DictionaryParentEnum.MatterUnit);
                                dic_brand.status = LTERPEFModel.Basic.DicStatus.Normal;
                                dic_brand.sort = item.OrderNumb ?? 0;
                                dic_brand.text = item.PubName;
                                bll_sys_dictionary.Add(dic_brand);
                            }
                        }
                        //添加物料
                        if (lstmatter!=null&& lstmatter.Count > 0)
                        {
                            var lstMatter = new List<LTERPEFModel.Stock.stk_matter>();
                            foreach (var item in lstmatter)
                            {
                                var _matterM= new LTERPEFModel.Stock.stk_matter();
                                if (!string.IsNullOrWhiteSpace(item.BrandGuid))
                                {
                                    _matterM.brand_guid = Guid.Parse(item.BrandGuid);
                                }
                                if (_matterM.brand_guid != null)
                                {
                                    var brandObj = bll_sys_dictionary.GetFirstDefault(w => w.guid == _matterM.brand_guid);
                                    if (brandObj != null)
                                    {
                                        _matterM.brand_name = brandObj.text;
                                    }
                                }
                                _matterM.code = item.CodeNumb;
                                _matterM.createdate = item.createdate??DateTime.Now;
                                _matterM.createuser = "wms-zhanghui";
                                _matterM.description = item.ModelNumber;
                                _matterM.guid = Guid.Parse(item.guid);
                                if (!string.IsNullOrWhiteSpace(item.MType))
                                {
                                    _matterM.mattertype_guid = Guid.Parse(item.MType);
                                }
                                if (_matterM.mattertype_guid != null)
                                {
                                    var MtypeObj = bll_stk_mattertype.GetFirstDefault(w => w.guid == _matterM.mattertype_guid);
                                    if (MtypeObj != null)
                                    {
                                        _matterM.mattertype_name = MtypeObj.name;
                                    }
                                }
                                _matterM.memo = item.Remark;
                                _matterM.name = item.MName;
                                _matterM.name_pinyin = LTLibrary.ChineseHelper.GetSpellCode(_matterM.name);
                                _matterM.state = LTERPEFModel.EntityStatus.Normal;
                                _matterM.std_price = item.PriceMoney??0;
                                _matterM.std_weight = item.weight??0;
                                if (!string.IsNullOrWhiteSpace(item.Unit))
                                {
                                    _matterM.unit_measurement_guid = Guid.Parse(item.Unit);
                                }
                                if (_matterM.unit_measurement_guid != null)
                                {
                                    var UniObj = bll_sys_dictionary.GetFirstDefault(w => w.guid == _matterM.unit_measurement_guid);
                                    if (UniObj != null)
                                    {
                                        _matterM.unit_measurement = UniObj.text;
                                    }
                                }
                                _matterM.updatedate = item.lastUpdateDate;
                                _matterM.updateuser = "erp-admin";
                                bll_stk_matter.Add(_matterM);
                               
                            }
                           // bll_stk_matter.AddRange(lstMatter);
                        }
                        if (rtv == LTERPEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("导入数据成功 总" + lstMt.Count() + "条");
                            return JsonSuccess();
                        }
                        else
                        {
                            AddJsonError("保存失败");
                        }
                    }
                    else
                    {
                        //数据为空
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }

            return JsonError();
        }
    }
}