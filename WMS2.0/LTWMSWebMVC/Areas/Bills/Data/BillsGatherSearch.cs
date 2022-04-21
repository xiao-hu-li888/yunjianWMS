using LTWMSEFModel.query_model;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Bills.Data
{
    public class BillsGatherSearch: SearchBaseModel
    {
        /// <summary>
        /// 出入库类型
        /// </summary>
        [Display(Name = "出入库类型"), DropDownList("BillsInOutEnum", true)]
        public BillsInOutEnum? bills_type { get; set; }
        public List<BillsGatherModel> PageCont { get; set; } 
    }
}