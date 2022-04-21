using LTWMSWebMVC.Areas.RealTime.Data; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Logs;
using LTWMSEFModel.Logs;

namespace LTWMSWebMVC.Areas.RealTime.Controllers
{
    public class AlarmLogController : BaseController
    {
        log_sys_alarmBLL bll_alarm_log;
        public AlarmLogController(log_sys_alarmBLL bll_alarm_log)
        {
            this.bll_alarm_log = bll_alarm_log;
        }
        // GET: RealTime/AlarmLog
        public ActionResult Index(AlarmLogSearch Model)
        {
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            int TotalSize = 0;
            Model.PageCont = bll_alarm_log.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.log_date,
                w => w.warehouse_guid== warehouseguid && (Model.s_keywords == "" || (w.remark ?? "").Contains(Model.s_keywords)
                ), false).Select(s => MapperConfig.Mapper.Map<log_sys_alarm, AlarmLogModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };            
            return View(Model);
        }
    }
}