using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace LTWMSWebMVC.App_Start.WebMvCEx
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownListAttribute : Attribute,IMetadataAware
    {
        public string listname { get; private set; }
        public bool IsSearch { get; set; }
        public DropDownListAttribute(string listname)
        {
            this.listname = listname;
        }
        public DropDownListAttribute(string listname, bool issearch)
        {
            this.listname = listname;
            this.IsSearch = issearch;
        }
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("listname", listname);
            metadata.AdditionalValues.Add("IsSearch", IsSearch);
            metadata.TemplateHint = "LTDropdown";
            
        }
    }
}
