using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LTERPWebMVC.App_Start.AppCode
{
    [Serializable]
    public class MenuData
    {
        /// <summary>
        /// 权限码
        /// </summary>
        [XmlAttribute]
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// Area名称
        /// </summary>
        [XmlAttribute]
        public string AreaName { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        [XmlAttribute]
        public string Controller { get; set; }
        /// <summary>
        /// 动作名称
        /// </summary>
        [XmlAttribute]
        public string Action { get; set; }
        /// <summary>
        /// 是否在菜单中显示
        /// </summary>
        [XmlAttribute]
        public bool ShowOnMenu { get; set; }
        [XmlAttribute]
        public string Class { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [XmlAttribute]
        public string Desc { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        [NonSerialized, XmlIgnore]
        public bool Checked = false;

        private List<MenuData> _childItem;
        [XmlElement]
        public List<MenuData> ChildItem
        {
            get
            {
                if (_childItem == null) _childItem = new List<MenuData>();
                return _childItem;
            }
            set
            {
                _childItem = value;
            }
        }
    }
}