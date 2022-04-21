using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class ShelfUnitsSearch
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
        [Display(Name = "排")]
        public int? rack { get; set; }
        [Display(Name = "列")]
        public int? column { get; set; }
        [Display(Name = "层")]
        public int? row { get; set; }
        [Display(Name = "库位状态"), DropDownList("ShelfCellState", true)]
        public LTWMSEFModel.Warehouse.ShelfCellState? cell_state { get; set; }
        [Display(Name = "锁类型"), DropDownList("ShelfLockType", true)]
        public LTWMSEFModel.Warehouse.ShelfLockType? lock_type { get; set; }
        /// <summary>
        /// 出库锁定、指定库位锁定(0正常)
        /// </summary>
        [Display(Name = "特殊锁"), DropDownList("SpecialLockTypeEnum",true)]
        public SpecialLockTypeEnum? special_lock_type { get; set; }
        public List<RealTime.Data.ShelfUnitsModel> PageCont { get; set; }
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