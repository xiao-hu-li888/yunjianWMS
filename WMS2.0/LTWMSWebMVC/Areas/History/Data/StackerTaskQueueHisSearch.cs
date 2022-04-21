using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.History.Data
{
    public class StackerTaskQueueHisSearch
    {
        private string _skeywords;
        [Display(Name = "关键字")]
        public string s_keywords
        {
            get
            {
                return (_skeywords ?? "").Trim();
            }
            set { _skeywords = value; }
        }

        /// <summary>
        /// 出库日期
        /// </summary>
        [Display(Name = "开始日期"), DataType(DataType.Date)]
        public DateTime? s_out_date_begin { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        [Display(Name = "结束日期"), DataType(DataType.Date)]
        public DateTime? s_out_date_end { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [Display(Name = "任务类型"), DropDownList("WcsTaskType", true)]
        public WcsTaskType? s_tasktype { get; set; }
        /// <summary>
        /// 任务完成状态
        /// </summary>
        [Display(Name = "完成状态"), DropDownList("WcsTaskStatusHis", true)]
        public WcsTaskStatus? s_taskstatus { get; set; } 
        public List<StackerTaskQueueHisModel> PageCont { get; set; }
        private PagingModel _paging;
        public PagingModel Paging
        {
            get
            {
                if (_paging == null)
                {
                    return new PagingModel();
                }
                return _paging;
            }
            set
            {
                _paging = value;
            }
        }
    }
}