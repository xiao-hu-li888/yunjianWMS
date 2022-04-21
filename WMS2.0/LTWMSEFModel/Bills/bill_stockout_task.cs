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
    /// 出库单据详细表（关联任务表，一个物料可能对应多个出库任务）
    /// bill_stockout （主表）==> bill_stockout_detail(子表) ==>bill_stockout_task(详细表)
    /// </summary>
    [Table("bill_stockout_task")]
    public class bill_stockout_task : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 出库单据guid（关联表：bill_stockout）
        /// </summary>
        [Required]
        public Guid stockout_guid { get; set; }
        /// <summary>
        /// 出库单据子表guid（关联表：bill_stockout_detail）
        /// </summary>
        [Required]
        public Guid stockout_detail_guid { get; set; }

        /// <summary>
        /// 指定扫描的 托盘条码
        /// </summary>
        [StringLength(100)]
        public string traybarcode { get; set; }
        /// <summary>
        /// 是否扫描（true扫描代表已出库）
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_scan { get; set; }
        /// <summary>
        /// 扫描时间
        /// </summary>
        public DateTime? scan_datetime { get; set; }
        /// <summary>
        /// 存储的条码类型：物料编码 包装条码 料箱条码
        /// </summary>
        public BarcodeStoredTypeEnum barcodetype { get; set; }
        /// <summary>
        /// 指定扫描的条码=>>>物料(stk_matter>code)、包装（bill_stockin_print>print_barcode）、料箱条码(wh_tray>traybarcode)
        /// （根据barcodetype区分）(可为空)
        /// </summary>
        [StringLength(50)]
        public string x_barcode { get; set; }
        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary> 
        public Guid? matter_guid { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }
        /// <summary>
        /// 是否指定出库批次（优先出指定批次的物料）
        /// </summary> 
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 计算的出库数量（计算得到要出指定包装条码里面的出库数量）
        /// </summary>
        public int out_number { get; set; }

        /// <summary>
        /// 实际出库数量（如果指定箱包条码库存不足？）
        /// </summary>
        public int out_realnumber { get; set; }
        /// <summary>
        /// 出库长度
        /// </summary>
        public decimal out_length { get; set; }
        /// <summary>
        /// 实际出库长度
        /// </summary>
        public decimal out_reallength { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? producedate { get; set; }

        /// <summary>
        /// 任务guid （关联表：hdw_stacker_taskqueue/hdw_stacker_taskqueue_his）
        /// </summary> 
        public Guid? task_queue_guid { get; set; }

        /// <summary>
        /// 起点:排-列-层-纵深
        /// </summary>
        [StringLength(50)]
        public string src_shelfunits_pos { get; set; }

        /// <summary>
        /// 托盘出库状态
        /// </summary>
        public BillOutTaskStatus outtask_status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

    }

    /// <summary>
    /// 出库运行状态
    /// </summary>
    public enum BillOutTaskStatus
    {
        /// <summary>
        /// 等待出库
        /// </summary>
        [Description("等待出库")]
        WaitOut = 0,
        /// <summary>
        /// 正在出库
        /// </summary>
        [Description("正在出库")]
        Outing = 1,
        /// <summary>
        /// 出库完成
        /// </summary>
        [Description("出库完成")]
        Finished = 2,
        /// <summary>
        /// 出库已取消
        /// </summary>
        [Description("出库已取消")]
        Canceled = 3
    }
}
