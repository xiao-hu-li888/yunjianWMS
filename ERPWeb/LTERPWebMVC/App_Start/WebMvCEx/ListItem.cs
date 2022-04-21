using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.App_Start.WebMvCEx
{
    public class ListItem
    {  
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        public string Class { get; set; }
        public ListItem(string Text, string Value, string Class)
        {
            this.Text = Text;
            this.Value = Value;
            this.Class = Class;
        }
        public ListItem(string Text, string Value)
        {
            this.Text = Text;
            this.Value = Value;
        }
        public ListItem() { }
    }
}