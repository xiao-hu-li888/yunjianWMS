using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Bills
{
    /// <summary>
    /// 盘点方式（盲盘）
    /// 库存盘点详细表（通过盘点方式：库位盘点/物料盘点 两种方式最终生产盘点单）
    /// 物料盘点：最终也是通过查找在库物料所在的库位，进行库位盘点。
    /// 盘点的最终目的是将托盘绑定物料关系表（wh_traymatter）详细信息进行复核的一个操作，防止漏扫条码或放错托盘等导致物料信息不准确
    /// </summary>
    [Table("bill_stockcheck_detail")]
    public class bill_stockcheck_detail : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        ///盘点表guid（关联表:bill_stockcheck）
        /// </summary>
        [Required]
        public Guid stockcheck_guid { get; set; }

        /// <summary>
        ///托盘具体存放位置--货架仓位guid（关联表：wh_shelfunits）
        /// </summary>  
        public Guid? shelfunits_guid { get; set; }
        /// <summary>
        /// 托盘所在库位（1排-2列-1层）
        /// </summary>
        [StringLength(50)]
        public string shelfunits_pos { get; set; }
        /// <summary>
        /// 托盘条码1(料箱条码)
        /// </summary>
        [StringLength(50)]
        public string traybarcode_1 { get; set; }
        /// <summary>
        /// 托盘条码2(料箱条码)
        /// </summary>
        [StringLength(50)]
        public string traybarcode_2 { get; set; }
        /// <summary>
        /// 出库运行状态
        /// </summary>
        public BillOutTaskStatus outtask_status { get; set; }

        /*****************原有记录（托盘绑定表wh_traymatter）*****************/
        /// <summary>
        /// 存储的条码类型：物料编码 包装条码 料箱条码
        /// </summary>
        public BarcodeStoredTypeEnum barcodetype { get; set; }
        /// <summary>
        /// 物料(stk_matter>code)、包装（bill_stockin_print>print_barcode）、料箱条码(wh_tray>traybarcode)
        /// （根据barcodetype区分）(可为空)
        /// </summary>
        [StringLength(50)]
        public string x_barcode { get; set; }

        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary>
        public Guid? matter_guid { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(50)]
        public string matter_name { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int old_number { get; set; }
        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        public decimal old_single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal old_total_weight { get; set; }
        /// <summary>
        /// 单个长度
        /// </summary>
        public decimal old_single_length { get; set; }
        /// <summary>
        /// 总长度
        /// </summary>
        public decimal old_total_length { get; set; }

        /******************盘点结果（盘点记录）***********************/
        /// <summary>
        /// 总数量
        /// </summary>
        public int checking_number { get; set; }
        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        public decimal checking_single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal checking_total_weight { get; set; }
        /// <summary>
        /// 单个长度
        /// </summary>
        public decimal checking_single_length { get; set; }
        /// <summary>
        /// 总长度
        /// </summary>
        public decimal checking_total_length { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary>
        public DateTime? checking_date { get; set; }
        /// <summary>
        /// 盘点人
        /// </summary>
       [StringLength(50)]
        public string checking_user { get; set; }

        /// <summary>
        /// 盘点结果(无差异/有差异)
        /// </summary>
        public CheckResult checking_result { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
        /// <summary>
        /// 是否新增(盘点时多出来的物料？？)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_newadded { get; set; }
    }

    ///// <summary>
    ///// 出库状态
    ///// </summary>
    //public enum TrayOutStatus
    //{
    //    /// <summary>
    //    /// 待出库
    //    /// </summary>
    //    [Description("待出库")]
    //    WaitOut = 0,
    //    /// <summary>
    //    /// 出库中
    //    /// </summary>
    //    [Description("出库中")]
    //    TrayOut = 1,
    //    /// <summary>
    //    /// 已下架
    //    /// </summary>
    //    [Description("已下架")]
    //    OffTheShelf = 2
    //}
}
