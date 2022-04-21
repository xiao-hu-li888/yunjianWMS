using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.Areas.Admin.Models
{
    public class HomeModel
    {
        /// <summary>
        /// 菜单（根据权限显示）
        /// </summary>
        public LTERPWebMVC.App_Start.AppCode.MenuData Menu
        {
            get;set;
        }
    }
}