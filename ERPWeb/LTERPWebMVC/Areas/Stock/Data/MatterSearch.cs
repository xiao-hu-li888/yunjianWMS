﻿using LTERPWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.Areas.Stock.Data
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