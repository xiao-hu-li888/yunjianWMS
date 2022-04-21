using LTWMSEFModel.Bills;
using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.RealTime.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class TrayMatterModel : BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        [Display(Name = "仓库分区"), DropDownList("WareHouseGuidList2", true)]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 托盘guid（表：wh_tray）
        /// </summary>  
        public Guid tray_guid { get; set; }
        /// <summary>
        /// 托盘条码（冗余字段）
        /// </summary>
        [Display(Name = "托盘条码")]
        public string traybarcode { get; set; }
        /***111****/
        /// <summary>
        /// 存储的条码类型：物料编码，包装条码（stockid物料编码冗余），整箱打包条码(需要另外建表？以后扩展)
        /// </summary>
        [Display(Name = "条码类型")]
        public BarcodeStoredTypeEnum barcodetype { get; set; }
        ///// <summary>
        ///// 物料条码（根据barcodetype区分）(可为空)
        ///// </summary>
        //[Display(Name = "物料条码")]
        //public string bag_barcode { get; set; }
        /****111****/
        /// <summary>
        ///物料编码（0101010001）（PrintBarcode包装条码>>>冗余）
        /// </summary>
        [Display(Name = "物料编码")]
        public string matter_barcode { get; set; }

        ///// <summary>
        ///// 物料名称/货品名称
        ///// </summary>
        //[Display(Name = "物料名称")]
        //public string name { get; set; }


        /// <summary>
        /// 物料(stk_matter>code)、包装（bill_stockin_print>print_barcode）、料箱条码(wh_tray>traybarcode)
        /// （根据barcodetype区分）(可为空)
        /// </summary>
        [Display(Name = "条码")]
        public string x_barcode { get; set; }
        /// <summary>
        /// 物料名称/货品名称（一个或多个物料的名称  物料1/物料2/物料3 ...）
        /// </summary>
        [Display(Name = "物料名称")]
        public string name_list { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        [StringLength(50)]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 批次（订单号）
        /// </summary>
        [Display(Name = "批次"), Required(AllowEmptyStrings = false)]
        public string lot_number { get; set; }
        /// <summary>
        /// 单个长度
        /// </summary>
        [Display(Name = "单个长度")]
        public decimal single_length { get; set; }
        /// <summary>
        /// 总长度
        /// </summary>
        [Display(Name = "总长度")]
        public decimal total_length { get; set; }

        /// <summary>
        /// 批次（订单号）
        /// </summary>
        [Display(Name = "批次")]
        public string batch { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Display(Name = "数量")]
        public int number { get; set; }
        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        [Display(Name = "单个重量(g)")]
        public decimal single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        [Display(Name = "总重量")]
        public decimal total_weight { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        [Display(Name = "生产日期"), DataType(DataType.Date)]
        public DateTime? producedate { get; set; }

        /// <summary>
        /// 有效日期
        /// </summary>
        [Display(Name = "有效日期"), DataType(DataType.Date)]
        public DateTime? effective_date { get; set; }

        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// 待检状态锁定出库！！！
        /// </summary>
        [Display(Name = "检验状态"), DropDownList("TestStatusEnum")]
        public TestStatusEnum test_status { get; set; }

        /// <summary>
        /// 项目编号（关联表：_project）[直接从erp获取进行中的项目列表？] [{prj_no:'项目编号',prj_name:'项目名称',cust_name:'客户名称',...},...]
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
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }

        /// <summary>
        /// 托盘信息
        /// </summary>
        public TrayModel trayModel { get; set; }
        /// <summary>
        /// 物料信息
        /// </summary>
        public MatterModel MatterModel { get; set; }
        /// <summary>
        /// 库位信息
        /// </summary>
        public ShelfUnitsModel ShelfUnitModel { get; set; }
        /// <summary>
        /// 锁定/解锁
        /// </summary>
        [Display(Name = "锁定/解锁"), DropDownList("LockUnLockEnum")]
        public LockUnLockEnum lock_unlock { get; set; }

        /// <summary>
        /// 待检产品转合格产品是两个不同的物料编码
        /// </summary>
        [Display(Name = "物料编码")]
        public string new_matter_barcode { get; set; }
    }
    public enum LockUnLockEnum
    {
        /// <summary>
        /// 解锁出库
        /// </summary>
        [Description("解锁")]
        Unlock = 0,
        /// <summary>
        /// 锁定出库
        /// </summary>
        [Description("锁定出库")]
        Lock = 1
    }
}








