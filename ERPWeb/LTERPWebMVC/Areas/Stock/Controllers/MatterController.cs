using LTERPEFModel.Stock;
using LTERPWebMVC.Areas.Stock.Data;
using LTERPService.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTERPWebMVC.Areas.Stock.Controllers
{
    public class MatterController : BaseController
    {
        stk_matterBLL bll_stk_matter;

        public MatterController(stk_matterBLL bll_stk_matter)
        {
            this.bll_stk_matter = bll_stk_matter;
        }
        // GET: Stock/Matter
        public ActionResult Index(MatterSearch Model)
        {
            //Model.PageCont = bll_stk_matter.GetAllQuery(w => w.state == LTERPEFModel.EntityStatus.Normal)
            //   .Select(s => MapperConfig.Mapper.Map<stk_matter, MatterModel>(s)).ToList();
            //return View("Index", Model);
            int TotalSize = 0;
            Model.PageCont = bll_stk_matter.Pagination(Model.Paging.paging_curr_page
            , Model.Paging.PageSize, out TotalSize, o => o.createdate
            , w => (Model.s_keywords == "" || (w.code ?? "").Trim().Contains(Model.s_keywords)
             || (w.brand_name ?? "").Trim().Contains(Model.s_keywords) || (w.description ?? "").Trim().Contains(Model.s_keywords)
             || (w.specs ?? "").Trim().Contains(Model.s_keywords) || (w.unit_measurement ?? "").Trim().Contains(Model.s_keywords)
             || (w.name ?? "").Trim().Contains(Model.s_keywords) || (w.name_pinyin ?? "").Trim().Contains(Model.s_keywords)
             || (w.mattertype_name ?? "").Trim().Contains(Model.s_keywords)), true)
              .Select(s => MapperConfig.Mapper.Map<stk_matter, MatterModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View("Index", Model);
        }
    }
}