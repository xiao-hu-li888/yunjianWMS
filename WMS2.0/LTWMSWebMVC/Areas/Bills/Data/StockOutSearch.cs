using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LTWMSEFModel;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class StockOutSearch
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
        [Display(Name = "开始日期"),DataType(DataType.Date)]
        public DateTime? s_out_date_begin { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        [Display(Name = "结束日期"),DataType(DataType.Date)]
        public DateTime? s_out_date_end { get; set; }

        /// <summary>
        /// 单据进行状态
        /// </summary>
        [Display(Name = "进行状态"), DropDownList("SearchBillsStatus_Out", true)]
        public SearchBillsStatus_Out? s_bill_status { get; set; }
         
        public List<StockOutModel> PageCont { get; set; }
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