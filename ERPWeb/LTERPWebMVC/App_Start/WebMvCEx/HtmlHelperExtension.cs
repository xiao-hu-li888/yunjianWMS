using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LTERPWebMVC.App_Start.WebMvCEx
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString ListValue(this HtmlHelper helper, string listname, object value)
        {
            if (value != null)
            {
                var listitem = ListProvider.GetList(listname);
                try
                {
                    string val = GetObject(value);
                    if (val.IndexOf(',') > 0)
                    {
                        val = "," + val + ",";
                        var r = listitem.Where(a => val.IndexOf("," + a.Value + ",") >= 0).Select(a => a.Text);
                        string oth = string.Empty;
                        foreach (var d in r)
                        {
                            oth += d + ",";

                        }
                        return new MvcHtmlString(oth.TrimEnd(','));
                    }
                    else
                    {
                        var t = listitem.First(a => a.Value == GetObject(value));
                        if (string.IsNullOrWhiteSpace(t.Class))
                            return new MvcHtmlString(t.Text.Trim());
                        else
                        {
                            return new MvcHtmlString(string.Format("<span class='{0}'>{1}</span>", t.Class, t.Text.Trim()));
                        }
                    }
                }
                catch
                {

                }
            }
            return new MvcHtmlString(""); 
        }

        /// <summary>
        /// 下拉列表
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="listname"></param>
        /// <param name="value"></param>
        /// <param name="issearch"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownList(this HtmlHelper helper, string name, string listname, object value, object issearch)
        {
            var listitem = ListProvider.GetList(listname);
            List<SelectListItem> list = new List<SelectListItem>();
            var resobj = GetObject(value);
            if (issearch != null && ((bool)issearch))
            {
                list.Add(new SelectListItem { Value = "-1", Text = "--全部--" });
            }
            foreach (var item in listitem)
            {
                if (item.Value == resobj)
                {
                    list.Add(new SelectListItem { Selected = true, Value = item.Value, Text = item.Text });
                }
                else
                    list.Add(new SelectListItem { Value = item.Value, Text = item.Text });
            }
             return helper.DropDownList(name, list); 
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, string listname, object value, object issearch)
        {
            var listitem = ListProvider.GetList(listname);
            StringBuilder buid = new StringBuilder();
            int d = 1;
            buid.Append("<div class='lt_checkbox'>");
            string valobj = "," + GetObject(value) + ",";

            string cname = helper.ViewContext.ViewData.ModelMetadata.PropertyName;
            if (issearch != null && ((bool)issearch))
            {
                buid.AppendFormat("<input type='checkbox' value='{0}' {1} name='{2}' id='{3}'/>", -1, "", cname, cname + "0");
                buid.AppendFormat("<label for='{0}'>{1}</label>", cname + "0", "-1");
            }
            foreach (var t in listitem)
            {

                string strname = cname + d.ToString();
                d++;
                bool ischeck = false;

                if (value != null && valobj.IndexOf("," + t.Value + ",") >= 0) { ischeck = true; }
                // buid.Append(helper.CheckBox(name, ischeck, new { value = t.Value, id = strname }).ToHtmlString());
                buid.AppendFormat("<input type='checkbox' value='{0}' {1} name='{2}' id='{3}'/>", t.Value, ischeck ? "checked='checked'" : "", cname, strname);
                buid.AppendFormat("<label for='{0}'>{1}</label>", strname, t.Text);
            }
            buid.Append("</div>");
            return new MvcHtmlString(buid.ToString());
        }
        public static MvcHtmlString RadioList(this HtmlHelper helper, string name, string listname, object value, object issearch)
        {
            var listitem = ListProvider.GetList(listname);
            StringBuilder buid = new StringBuilder();
            int d = 1;
            buid.Append("<div class='lt_radio'>");
            var resobj = GetObject(value);
            string cname = helper.ViewContext.ViewData.ModelMetadata.PropertyName;
            if (issearch != null && ((bool)issearch))
            {
                buid.AppendFormat("<input type='radio' value='{0}' {1} name='{2}' id='{3}'/>", -1, "", cname, cname + "0");
                buid.AppendFormat("<label for='{0}'>{1}</label>", cname + "0", "-1");
            }
            foreach (var t in listitem)
            {
                string strname = cname + d.ToString();
                d++;
                bool ischeck = false;
                if (value != null && t.Value == resobj) { ischeck = true; }
                // buid.Append(helper.CheckBox(name, ischeck, new { value = t.Value, id = strname }).ToHtmlString());
                buid.AppendFormat("<input type='radio' value='{0}' {1} name='{2}' id='{3}'/>", t.Value, ischeck ? "checked='checked'" : "", cname, strname);
                buid.AppendFormat("<label for='{0}'>{1}</label>", strname, t.Text);
            }
            buid.Append("</div>");
            return new MvcHtmlString(buid.ToString());
        }
        public static string GetObject(object value)
        {
            if (value != null)
            {
                string val = value.ToString();
                try
                {


                    if (value.GetType().BaseType.Name == typeof(Enum).Name)
                    {

                        val = ((byte)value + 0).ToString();
                    }

                }
                catch
                {
                    try
                    {
                        val = ((int)value).ToString();
                    }
                    catch { }
                }
                return val;
            }
            return string.Empty;
        }

    }
}