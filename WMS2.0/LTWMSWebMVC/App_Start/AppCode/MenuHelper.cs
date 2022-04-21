using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LTWMSWebMVC.App_Start.AppCode
{
    public class MenuHelper
    {
        private static string path = "~/App_Data/MenuPermission.config";
        //判断当前action/controller是否有权限
        public static bool HasPermission(LTWMSService.Basic.sys_loginBLL bll_login, Guid currloginguid, string _controller, string _action)
        {//判断当前用户是否是超级管理员
            if (bll_login.GetAny(w => w.guid == currloginguid && w.issuperadmin == true))
            {
                return true;
            }
            string prem = GetPermission(GetALLMenu(), _controller, _action);
            if (string.IsNullOrWhiteSpace(prem))
            {
                return true;
               // throw new Exception("控制器" + _controller + "中方法" + _action + "未在权限配置文件中配置！"); 
            }
            List<LTWMSEFModel.Basic.sys_role> roleList = bll_login.GetLoginRoles(currloginguid);
            if (roleList != null)
            {
                return IsPremission(roleList, prem);
            }
            return false;
        }
        public static bool IsPremission(List<LTWMSEFModel.Basic.sys_role> roleList, string premission)
        {
            if (premission.Equals("none", StringComparison.CurrentCultureIgnoreCase)) { return true; } 
            string[] pers = premission.Split(',');
            foreach (var s in roleList)
            {
                if (!string.IsNullOrWhiteSpace(s.permissiontext))
                {
                    var r = pers.Intersect(s.permissiontext.Split(','));
                    if (r != null && r.Count() > 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取当前操作对应的权限值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="col"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string GetPermission(MenuData data, string col, string action)
        {
            if (data.Action != null && data.Controller != null
                && data.Action.ToLower() == action.ToLower() && data.Controller.ToLower() == col.ToLower())
            {
                return data.Code;
            }
            if (data.ChildItem != null && data.ChildItem.Count > 0)
            {
                foreach (var d in data.ChildItem)
                {
                    string ret = GetPermission(d, col, action);
                    if (!string.IsNullOrWhiteSpace(ret))
                    {
                        return ret;
                    }
                }
            }
            return string.Empty;
        }
        public static MenuData GetCurrentLoginMenu(LTWMSService.Basic.sys_loginBLL bll_login, Guid currLoginGuid)
        {
            //判断当前用户是否是超级管理员
            if (bll_login.GetAny(w => w.guid == currLoginGuid && w.issuperadmin == true))
            {
                var menu = GetALLMenu();
                CheckALL(menu);
                return menu;
            }
            var lstRole = bll_login.GetLoginRoles(currLoginGuid).Where(w => w.active == true && w.state == LTWMSEFModel.EntityStatus.Normal).ToList();
            if (lstRole != null && lstRole.Count > 0)
            {
                string _permisiontext = string.Join(",", lstRole.Select(w => w.permissiontext).ToArray());
                if (!string.IsNullOrWhiteSpace(_permisiontext))
                {
                    MenuData data = GetALLMenu();
                    UpdateChecked(data, "," + _permisiontext + ",");
                    return data;
                }
            }
            return null;
        }
        private static void CheckALL(MenuData data)
        {
            data.Checked = true;
            if (data.ChildItem.Count > 0)
            {
                foreach (var item in data.ChildItem)
                {
                    item.Checked = true;
                    CheckALL(item);
                }
            }
        }
        public static MenuData CurrentRole(object ds)
        {
            MenuData data = GetALLMenu();
            if (ds == null) return data;
            string strd = ds.ToString();
            if (string.IsNullOrWhiteSpace(strd)) { return data; }
            UpdateChecked(data, "," + strd + ",");
            return data;
        }
        private static void UpdateChecked(MenuData data, string str)
        {
            if (str.IndexOf("," + data.Code + ",") >= 0)
                data.Checked = true;
            if (data.ChildItem.Count > 0)
            {
                foreach (var item in data.ChildItem)
                {
                    if (str.IndexOf("," + item.Code + ",") >= 0)
                        item.Checked = true;
                    UpdateChecked(item, str);
                }
                //data.IsChecked = data.ChildItem.Sum(a => a.IsChecked ? 1 : 0) == data.ChildItem.Count;
            }
        }
        public static string MenuPermissionPath
        {
            get
            {
                string cpath;
                if (HttpContext.Current != null)
                {
                    cpath = HttpContext.Current.Server.MapPath(path);
                }
                else
                {
                    cpath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }
                return cpath;
            }
        }
        public static MenuData GetALLMenu()
        {
            if (System.IO.File.Exists(MenuPermissionPath))
            {
                using (Stream stream = new FileStream(MenuPermissionPath, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MenuData));
                    MenuData menu = (MenuData)serializer.Deserialize(stream);
                    return menu;
                }
            }
            return null;
        }
        //public MenuData GetPermissionMenu()
        //{

        //}
    }
}