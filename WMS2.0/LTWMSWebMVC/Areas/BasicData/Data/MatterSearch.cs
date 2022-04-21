using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class MatterSearch
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
        private string _s_specs;
        /// <summary>
        /// 规格型号
        /// </summary>
        [Display(Name = "规格型号")]
        public string s_specs
        {
            get
            {
                return (_s_specs ?? "").Trim();
            }
            set { _s_specs = value; }
        }

        /// <summary>
        ///物料类型/货品类型-- 关联表：stk_mattertype
        /// </summary>
        [Display(Name = "物料类型"), DropDownList("MatterTypeGuidList", true)]
        public Guid? s_mattertype_guid { get; set; }
          
        public List<MatterModel> PageCont { get; set; }
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