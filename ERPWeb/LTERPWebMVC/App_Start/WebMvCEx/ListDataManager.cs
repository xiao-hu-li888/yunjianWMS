using LTERPWebMVC.App_Start.WebMvCEx;
using LTERPService.Basic; 
using LTERPService.Stock; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC
{
    public class ListDataManager
    {
        /// <summary>
        /// 查询设置所有可用角色
        /// </summary>
        public static void setRoleListData(sys_roleBLL bll_sys_role)
        {
            var d = bll_sys_role.GetAllQuery(w
                => w.state == LTERPEFModel.EntityStatus.Normal);
            List<LTERPWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTERPWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTERPWebMVC.App_Start.WebMvCEx.ListItem { Text = s.rolename, Value = s.guid.ToString() });
            }
            ListProvider.AddList("RoleList", list);
        }
        

        /// <summary>
        /// 查询设置物料类型
        /// </summary>
        public static void setMatterTypeGuidList(stk_mattertypeBLL bll_mattertype)
        {
            var d = bll_mattertype.GetAllQuery(w
                => w.state == LTERPEFModel.EntityStatus.Normal);
            List<LTERPWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTERPWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTERPWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
            }
            ListProvider.AddList("MatterTypeGuidList", list);
        }

      
        /// <summary>
        /// 查询设置Agv终点
        /// </summary>
        public static void setAgvDestinationList()
        {
            List<string> lstDest = new List<string>();
            string ListAgvDest = WMSFactory.Config.AgvDestinationList;
            if (!string.IsNullOrWhiteSpace(ListAgvDest))
            {
                string[] arrList = ListAgvDest.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                lstDest.AddRange(arrList);
            }
            List<LTERPWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTERPWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in lstDest)
            {
                list.Add(new LTERPWebMVC.App_Start.WebMvCEx.ListItem { Text = s, Value = s });
            }
            ListProvider.AddList("AgvDestiNation", list);
        }
    }
}