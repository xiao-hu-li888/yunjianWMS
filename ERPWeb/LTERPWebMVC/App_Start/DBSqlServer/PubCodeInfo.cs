//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LTERPWebMVC.App_Start.DBSqlServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class PubCodeInfo
    {
        public int ID { get; set; }
        public string GUID { get; set; }
        public string PubName { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public string Code { get; set; }
        public string CodeGuid { get; set; }
        public string Remark { get; set; }
        public Nullable<int> OrderNumb { get; set; }
        public Nullable<int> IsGroupbyType { get; set; }
        public Nullable<int> Active { get; set; }
    }
}
