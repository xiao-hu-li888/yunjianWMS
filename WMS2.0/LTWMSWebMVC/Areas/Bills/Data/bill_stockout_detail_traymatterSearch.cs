using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class bill_stockout_detail_traymatterSearch
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
        public Guid? billstkout_guid { get; set; }
        public Guid? billstockout_detail_guid { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        [Display(Name = "物料条码")]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string matter_name { get; set; }
       
        public List<bill_stockout_detail_traymatterModel> PageCont { get; set; }
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