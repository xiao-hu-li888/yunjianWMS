using LTWMSEFModel.BillsAihua;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class ReservedOrderSearch
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
        ///预留单状态
        /// </summary>
        [Display(Name = "预留单状态"), DropDownList("ReserveBillOutStatus", true)]
        public ReserveBillOutStatus? s_status { get; set; }
        public List<ReservedOrderModel> PageCont { get; set; }
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