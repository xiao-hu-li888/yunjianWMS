using LTWMSWebMVC.Areas.History.Data;
using LTWMSEFModel.Hardware;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.History.Controllers
{
    public class RgvTaskHisController : BaseController
    {
        hdw_stacker_taskqueue_hisBLL bll_stacker_taskqueue_his;
        public RgvTaskHisController(hdw_stacker_taskqueue_hisBLL bll_stacker_taskqueue_his)
        {
            this.bll_stacker_taskqueue_his = bll_stacker_taskqueue_his;
        }
        // GET: History/RgvTaskHis
        public ActionResult Index(StackerTaskQueueHisSearch Model)
        {
            DateTime? beginDate = null;
            if (Model.s_out_date_begin != null)
            {
                beginDate = new DateTime(Model.s_out_date_begin.Value.Year,
              Model.s_out_date_begin.Value.Month, Model.s_out_date_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (Model.s_out_date_end != null)
            {
                endDate = new DateTime(Model.s_out_date_end.Value.Year,
                    Model.s_out_date_end.Value.Month, Model.s_out_date_end.Value.Day);
                endDate= endDate.Value.AddDays(1);
            }
            int TotalSize = 0;
            Model.PageCont = bll_stacker_taskqueue_his.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.endup,
                w => (beginDate == null || w.createdate >= beginDate) && (endDate == null || w.createdate <= endDate) && (Model.s_keywords == "" || (w.dest_shelfunits_pos ?? "").Equals(Model.s_keywords)
                || (w.src_shelfunits_pos ?? "").Equals(Model.s_keywords)|| (w.order ?? "").Contains(Model.s_keywords)
                || (w.tray1_matter_barcode1 ?? "").Contains(Model.s_keywords) || (w.tray1_barcode ?? "").Contains(Model.s_keywords)
                || (w.tray1_matter_barcode2 ?? "").Contains(Model.s_keywords) ||w.taskqueue_id.ToString().Equals(Model.s_keywords))
                  && ( Model.s_taskstatus == null 
                    || (int)Model.s_taskstatus.Value == -1|| w.taskstatus == Model.s_taskstatus)
                      && (Model.s_tasktype == null
                    || (int)Model.s_tasktype.Value == -1 || w.tasktype == Model.s_tasktype)
                        ,false).Select(s => MapperConfig.Mapper.Map<hdw_stacker_taskqueue_his, StackerTaskQueueHisModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize }; 
            return View(Model);
        }
        public ActionResult View(Guid guid)
        { 
            var model = bll_stacker_taskqueue_his.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<hdw_stacker_taskqueue_his, StackerTaskQueueHisModel>(model);
            Md.OldRowVersion = model.rowversion; 
            return PartialView(Md);
        }
    }
}