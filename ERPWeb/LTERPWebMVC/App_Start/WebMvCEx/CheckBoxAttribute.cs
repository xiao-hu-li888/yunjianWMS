using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace LTERPWebMVC.App_Start.WebMvCEx
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckBoxAttribute : Attribute, IMetadataAware
    {
        public string listname { get; private set; }
        public bool IsSearch { get; set; }
        public CheckBoxAttribute(string listname)
        {
            this.listname = listname;
        }
        public CheckBoxAttribute(string listname, bool issearch)
        {
            this.listname = listname;
            this.IsSearch = issearch;
        }
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("listname", listname);
            metadata.AdditionalValues.Add("IsSearch", IsSearch);
            metadata.TemplateHint = "LTCheckBox";

        }
    }
}
