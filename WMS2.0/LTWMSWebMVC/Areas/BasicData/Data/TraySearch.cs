using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class TraySearch
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
        /// 托盘上架状态(在货架上不能关联绑定物料???)
        /// </summary>
        [Display(Name = "上架状态"), DropDownList("TrayStatus", true)]
        public TrayStatus? s_status { get; set; }
        public List<TrayModel> PageCont { get; set; }
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