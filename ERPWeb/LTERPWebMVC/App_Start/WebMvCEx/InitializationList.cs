using LTERPEFModel.Basic; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LTERPWebMVC.App_Start.WebMvCEx
{
    public class InitializationList
    {
        public InitializationList()
        {

        }
        /// <summary>
        /// 设置字典下拉框
        /// </summary>
        /// <param name="typename">字典类型</param>
        /// <param name="ListName">设置下拉框数据源名称</param>
        public static void SetDictDrpdownList(string DictType, string ListName)
        {
            SetDictDrpdownList(DictType, ListName, false);
        }
        /// <summary>
        /// 设置字典下拉框
        /// </summary>
        /// <param name="typename">字典类型</param>
        /// <param name="ListName">设置下拉框数据源名称</param>
        /// <param name="ValueIsName">值名称是否为同一内容</param>
        public static void SetDictDrpdownList(string DictType, string ListName, bool ValueIsName)
        {
            List<ListItem> list = new List<ListItem>();
            var d = App_Start.AutofacConfig.GetFromFac<LTERPService.Basic.sys_dictionaryBLL>();
            IList<LTERPEFModel.Basic.sys_dictionary> cmplist = d.GetListByParentID(DictType);
            if (ValueIsName)
                SetListToContext(cmplist, b => b.text, c => c.text, ListName);
            else
                SetListToContext(cmplist, b => b.guid.ToString(), c => c.text, ListName);
        }
        /// <summary>
        /// 登录用户
        /// </summary>
        public static void setEmployDropdownlist()
        {
            var d = App_Start.AutofacConfig.GetFromFac<LTERPService.Basic.sys_loginBLL>();
            var EmployList = d.GetAllQuery();
            SetListToContext(EmployList, b => b.guid, c => c.loginname, "ChooseCheckLoginIDList");
        }
        /// <summary>
        /// 登录用户
        /// </summary>
        public static void setEmployDropdownlist(List<LTERPEFModel.Basic.sys_login> emps)
        {
            SetListToContext(emps, b => b.guid, c => c.loginname, "ChooseCheckLoginIDList");
        }

        public static void SetListToContext<T, TValue, TText>(IEnumerable<T> list, Expression<Func<T, TValue>> value, Expression<Func<T, TText>> text, string name, bool isstatic = false)
        {
            if (list == null) return;
            List<ListItem> items = new List<ListItem>();
            foreach (T d in list)
            {
                items.Add(new ListItem(text.Compile().Invoke(d).ToString(), value.Compile().Invoke(d).ToString()));
            }
            if (isstatic)
                ListProvider.AddList(name, items, null);
            else
                ListProvider.AddList(name, items);
        }
        public static void SetListToContext<T, TValue, TText, TColor>(IEnumerable<T> list, Expression<Func<T, TValue>> value, Expression<Func<T, TText>> text, Expression<Func<T, TColor>> color, string name, bool isstatic = false)
        {
            if (list == null) return;
            List<ListItem> items = new List<ListItem>();
            foreach (T d in list)
            {
                items.Add(new ListItem(text.Compile().Invoke(d).ToString(), value.Compile().Invoke(d).ToString(), color.Compile().Invoke(d).ToString()));
            }
            if (isstatic)
                ListProvider.AddList(name, items, null);
            else
                ListProvider.AddList(name, items);
        }
        public string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        public void StaticList()
        {
            ListProvider.AddList("Active", new ListItem[] {new ListItem("启用","True","label label-sm label-success"),
            new ListItem("已禁用","False","label label-sm label-danger")}, null);

            ListProvider.AddList("gender", new ListItem[] { new ListItem("男", "True"), new ListItem("女", "False") }, null);
            ListProvider.AddList("YesNoState", new ListItem[] {new ListItem("是","True","label label-sm label-success"),
            new ListItem("否","False","label label-sm label-danger")}, null);
            ListProvider.AddList("State", new ListItem[] {
                new ListItem("启用","1","label label-sm label-success"),
                new ListItem("已禁用","0","label label-sm label-warning"),
                new ListItem("已删除","3","label label-sm label-danger")
            }, null);

            //MatterType_list  物料分类 电池/其它物料
            //ListProvider.AddList("MatterType_list", new ListItem[] {
            //    new ListItem("电池","1"),
            //    new ListItem("其它物料","2")
            //}, null);
            ////////////////////  
            
        }
    }
}