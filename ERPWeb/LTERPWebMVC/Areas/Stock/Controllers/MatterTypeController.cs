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
    public class MatterTypeController : BaseController
    {
        stk_mattertypeBLL bll_stk_mattertype;
        public MatterTypeController(stk_mattertypeBLL bll_stk_mattertype)
        {
            this.bll_stk_mattertype = bll_stk_mattertype;
        }
        // GET: Stock/MatterType
        public ActionResult Index(MatterTypeSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_stk_mattertype.PaginationByLinq(Model.s_keywords, Model.Paging.paging_curr_page
            , Model.Paging.PageSize, out TotalSize)
              .Select(s => MapperConfig.Mapper.Map<stk_mattertype, MatterTypeModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View("Index", Model);
        }
    }
}
