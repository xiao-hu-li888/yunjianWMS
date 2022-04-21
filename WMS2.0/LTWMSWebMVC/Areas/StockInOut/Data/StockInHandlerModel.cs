using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.StockInOut.Data
{
    public class StockInHandlerModel
    {
        ///// <summary>
        ///// 存储的条码类型：物料编码，包装条码（stockid物料编码冗余），整箱打包条码(需要另外建表？以后扩展)
        ///// </summary>
        //[Display(Name = "条码类型")]
        //public BarcodeStoredTypeEnum barcodetype { get; set; }
        /// <summary>
        /// 物料条码（根据barcodetype区分）(可为空)
        /// </summary>
        [Display(Name = "条码")]
        public string x_barcode { get; set; }
        
        //public List<MatterModel> List_MatterModel { get; set; }
        public MatterModel MatterModel { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Display(Name = "数量")]
        public decimal number { get; set; }
        /// <summary>
        /// 项目号
        /// </summary>
        [Display(Name = "项目号")]
        public string project_no { get; set; }
        /// <summary>
        /// 关联项目
        /// </summary>
        [Display(Name = "项目名称")]
        public string project_name { get; set; }
        /// <summary>
        /// 关联项目对应的客户
        /// </summary>
        [Display(Name = "客户名称")]
        public string customer_name { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
    }
}