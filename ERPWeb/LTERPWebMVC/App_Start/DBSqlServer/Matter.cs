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
    
    public partial class Matter
    {
        public int ID { get; set; }
        public string guid { get; set; }
        public string CodeNumb { get; set; }
        public string MName { get; set; }
        public string MType { get; set; }
        public string Unit { get; set; }
        public Nullable<int> Stockinventory { get; set; }
        public string state { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<int> stock { get; set; }
        public Nullable<decimal> PriceMoney { get; set; }
        public string ModelNumber { get; set; }
        public Nullable<decimal> weight { get; set; }
        public Nullable<System.DateTime> lastUpdateDate { get; set; }
        public byte[] SamllImage { get; set; }
        public string MattCCode { get; set; }
        public string BarCode { get; set; }
        public string BrandGuid { get; set; }
        public string Remark { get; set; }
    }
}
