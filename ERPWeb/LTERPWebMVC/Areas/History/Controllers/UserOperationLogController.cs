using LTERPWebMVC.Areas.History.Data; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTERPService.Logs;
using LTERPEFModel.Logs;

namespace LTERPWebMVC.Areas.History.Controllers
{
    public class UserOperationLogController : BaseController
    {
        log_sys_useroperationBLL bll_useroperation_log;
        public UserOperationLogController(log_sys_useroperationBLL bll_useroperation_log)
        {
            this.bll_useroperation_log = bll_useroperation_log;
        }
        // GET: History/UserOperationLog
        public ActionResult Index(UserOperationLogSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_useroperation_log.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.log_date,
                w => (Model.s_keywords == "" || (w.remark ?? "").Contains(Model.s_keywords)
                || (w.operator_u ?? "").Contains(Model.s_keywords)) 
                , false).Select(s => MapperConfig.Mapper.Map<log_sys_useroperation, UserOperationLogModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
    }
}