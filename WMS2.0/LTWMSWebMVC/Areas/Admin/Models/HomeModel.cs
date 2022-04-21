using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Admin.Models
{
    public class HomeModel
    {
        /// <summary>
        /// 密码过期提示（默认：false 不提示）
        /// </summary>
        public int PassWordExpiration { get; set; }
        /// <summary>
        /// 下发任务至所有堆垛机
        /// </summary>
        public bool SendTaskToAllStackers { get; set; }
        /// <summary>
        /// 仓库分区表guid（关联表wh_warehouse_type）
        /// </summary>
        [Display(Name = "仓库分区"), DropDownList("WareHouseGuidList2")]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 菜单（根据权限显示）
        /// </summary>
        public LTWMSWebMVC.App_Start.AppCode.MenuData Menu
        {
            get;set;
        }
    }
}