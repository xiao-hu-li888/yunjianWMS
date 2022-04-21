using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class WareHouseModel : BaseModel
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        [Required(ErrorMessage = "仓库名称不能为空"), Display(Name = "仓库名称")]
        public string name { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [Required(ErrorMessage = "仓库编号唯一不能为空"), Display(Name = "仓库编号唯一")]
        public string code { get; set; }
        /// <summary>
        /// 仓库分区表guid（关联表wh_warehouse_type）
        /// </summary>
        [Display(Name = "仓库分区"), DropDownList("WareHouseTypeGuidList")]
        public Guid? warehouse_type_guid { get; set; }
        ///// <summary>
        ///// 仓库分区/分类 名称（末级分类名称）
        ///// </summary>
        //[Display(Name = "仓库分区名称")]
        //public string warehouse_type_name { get; set; }
        /// <summary>
        /// 仓库类别（立体库/平库）
        /// </summary>
        [Display(Name = "仓库类别"), DropDownList("WareHouseCategoriesEnum")]
        public WareHouseCategoriesEnum category { get; set; }

        /// <summary>
        /// 针对单边多排的货物存放策略，一般只有2排、支持任意多排
        /// 0：将最外侧的货架整个存满然后再存内侧，避免频繁的移库动作（一个移库动作最少也要20来秒）
        /// 1：外侧内侧同时存遵循路径最短原则，但存在频繁移库的可能性
        /// </summary>
        [Display(Name = "存货策略"), DropDownList("DistributeWayEnum")]
        public DistributeWayEnum distribute_way { get; set; }

        /// <summary>
        ///仓库地址
        /// </summary>
        [Display(Name = "仓库地址")]
        public string address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string remark { get; set; }
        /// <summary>
        /// 是否选中(角色权限分配)
        /// </summary>
        public bool Checked { get; set; }
    }
}