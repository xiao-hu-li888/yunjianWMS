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
    
    public partial class MatterClass
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string MatCCode { get; set; }
        public string ParentGuid { get; set; }
        public Nullable<byte> bMatCEnd { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string MatCName { get; set; }
        public Nullable<byte> MatCGrade { get; set; }
        public string cBarCode { get; set; }
        public string Remark { get; set; }
        public string ParentP { get; set; }
        public string ShortCode { get; set; }
    }
}
