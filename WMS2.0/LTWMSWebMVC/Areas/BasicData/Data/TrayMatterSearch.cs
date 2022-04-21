using LTWMSEFModel;
using LTWMSEFModel.Bills;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class TrayMatterSearch
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
        [Display(Name = "物料类型"), DropDownList("MatterType_list", true)]
        public int? s_matterType { get; set; }

        [Display(Name = "是否检测"), DropDownList("YesNoState", true)]
        public bool? s_ischecked_ok { get; set; }
        /// <summary>
        /// 仓库guid
        /// </summary>
        [Display(Name = "仓库分区"), DropDownList("WareHouseGuidList2", true)]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        [Display(Name = "开始时间"),DataType(DataType.Date)]
        public DateTime? trayInDate_begin { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        [Display(Name = "结束时间"), DataType(DataType.Date)]
        public DateTime? trayInDate_end { get; set; }
        /// <summary>
        /// 物料排序模式
        /// </summary>
        [Display(Name = "排序"),DropDownList("MatterOrderEnum")]
        public MatterOrderEnum matterOrder { get; set; }
        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// 待检状态锁定出库！！！
        /// </summary>
        [Display(Name = "检验状态"), DropDownList("TestStatusEnum",true)]
        public TestStatusEnum? test_status { get; set; }

        public List<TrayMatterModel> PageCont { get; set; }
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