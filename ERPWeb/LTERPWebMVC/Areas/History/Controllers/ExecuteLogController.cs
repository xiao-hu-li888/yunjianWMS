 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTERPWebMVC.Areas.History.Data;
using LTERPEFModel.Basic;
using LTERPService.Logs;
using LTERPEFModel.Logs;

namespace LTERPWebMVC.Areas.History.Controllers
{
    public class ExecuteLogController : BaseController
    {
        log_sys_executeBLL bll_sys_execute_log;
        public ExecuteLogController(log_sys_executeBLL bll_sys_execute_log)
        {
            this.bll_sys_execute_log = bll_sys_execute_log;
        }
        // GET: History/UserOperationLog
        public ActionResult Index(ExecuteLogSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_sys_execute_log.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.log_date,
                w => (Model.s_keywords == "" || (w.remark ?? "").Contains(Model.s_keywords))
                , false).Select(s => MapperConfig.Mapper.Map<log_sys_execute, ExecuteLogModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        /// <summary>
        /// 删除半年前的日志记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelHalfYearData()
        {
            try
            {
                //var rtv = bll_sys_login.Delete(w => w.guid == guid);
                bool rtv = bll_sys_execute_log.DelHistInfo();
                //if (rtv)
                //{//删除成功
                AddUserOperationLog("[" + LTERPWebMVC.App_Start.AppCode.CurrentUser.UserName + "]删除1周前的运行日志...");
                return JsonSuccess();
                //}
                //else
                //{//删除失败
                //    AddJsonError("删除历史数据失败！"); 
                //}  
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
    }
}