using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class TrayModel : BaseModel
    {
        /// <summary>
        /// 托盘条码
        /// </summary>
        [Display(Name = "托盘条码")]
        public string traybarcode { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        [Display(Name = "重量")]
        public decimal weight { get; set; }
        /// <summary>
        /// 空托盘
        /// </summary>
        [Display(Name = "是否空托盘"), DropDownList("YesNoState")]
        public bool emptypallet { get; set; }
        [Display(Name = "电池条码1")]
        public string matter_barcode1 { get; set; }
        [Display(Name = "电池条码2")]
        public string matter_barcode2 { get; set; }
        /// <summary>
        /// 关联物料的种类
        /// </summary>
        [Display(Name = "种类数量")]
        public int totalkind { get; set; }
        /// <summary>
        /// 关联物料的总数量
        /// </summary>
        [Display(Name = "总数量")]
        public int totalnumber { get; set; }
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>           
        [Display(Name = "仓库分区"), DropDownList("WareHouseGuidList2", true)]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        ///托盘具体存放位置--货架仓位guid（关联表：wh_shelfunits）
        /// </summary>  
        public Guid? shelfunits_guid { get; set; }
        /// <summary>
        /// 托盘上架状态(在货架上不能关联绑定物料???)
        /// </summary>
        [Display(Name = "上架状态"), DropDownList("TrayStatus")]
        public TrayStatus status { get; set; }
        /// <summary>
        /// 托盘所在库位（1排-2列-1层）
        /// </summary>
        [Display(Name = "存储库位")]
        public string shelfunits_pos { get; set; }
        [Display(Name = "库位状态"), DropDownList("ShelfCellState")]
        public ShelfCellState? cell_state { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        public List<TrayMatterModel> traymatterList { get; set; }
        /// <summary>
        /// 一个托盘只放一种物料
        /// </summary>
        public TrayMatterModel TrayMatter { get; set; }
        /// <summary>
        /// 托盘是否扫码组盘（不扫描不能入库。。。）
        /// </summary>
        [Display(Name = "是否组盘"), DropDownList("YesNoState")]
        public bool? isscan { get; set; }
        /// <summary>
        /// 组盘时间，组盘超过4小时需重新组盘才能入库
        /// </summary>
        [Display(Name = "组盘时间")]
        public DateTime? scandate { get; set; }

        /// <summary>
        /// 组盘指定分配库位（给对应库位上系统锁）
        ///库位表guid（关联表：wh_shelfunits）
        /// </summary>
        [Display(Name = "指定库位guid")]
        public Guid? dispatch_shelfunits_guid { get; set; }
        /// <summary>
        /// 托盘所在库位（1排-2列-1层）
        /// </summary>
        [Display(Name = "指定库位")]
        public string dispatch_shelfunits_pos { get; set; }


    }
}