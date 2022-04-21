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
    /// 出库指定的托盘或系统自动分配的托盘
    /// </summary>
    [Table("bill_stockout_detail_traymatter")]
    public class bill_stockout_detail_traymatter : BaseEntity
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
        /// 出库单详细表guid（关联表：bill_stockout_detail）
        /// </summary>
        [Required]
        public Guid stockout_detail_guid { get; set; }
        /*******************物料相关**********************/
        /// <summary>
        ///物料表guid（ 关联表：stk_matter）
        /// </summary>
        public Guid? stk_matter_guid { get; set; }
        /// <summary>
        /// 物料条码
        /// </summary>
        [StringLength(100)]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [StringLength(50)]
        public string matter_name { get; set; }

        /// <summary>
        /// 批次（订单号）
        /// </summary>
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal number { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? produce_date { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? effective_date { get; set; }
        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// </summary>
        public TestStatusEnum test_status { get; set; }
        /******************关联任务信息**************************/
        /// <summary>
        /// 托盘条码
        /// </summary>
        [StringLength(100)]
        public string traybarcode { get; set; }
        /// <summary>
        /// 备注（任务失败，自动重新生成任务）
        /// </summary>
        [StringLength(1000)]
        public string memo { get; set; }
        /// <summary>
        /// 托盘出库回库状态
        /// </summary>
        public TrayOutStockStatusEnum tray_status { get; set; }
        /**************出库****************/
        /// <summary>
        /// 出库库位（1-1-1）
        /// </summary>
        [StringLength(30)]
        public string out_shelfunits_pos { get; set; }

        /// <summary>
        /// 出库库位guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? out_shelfunits_guid { get; set; }
        /// <summary>
        /// 任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? out_stacker_taskqueue_guid { get; set; }

        /// <summary>
        /// 托盘出库时间
        /// </summary>
        public DateTime? tray_out_date { get; set; }
        /*************回库***************/
        /// <summary>
        /// 存储库位（1-1-1）
        /// </summary>
        [StringLength(30)]
        public string back_shelfunits_pos { get; set; }
        /// <summary>
        /// 存储库位guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? back_shelfunits_guid { get; set; }

        /// <summary>
        /// 回库任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? back_stacker_taskqueue_guid { get; set; }
        /// <summary>
        /// 托盘回库时间
        /// </summary>
        public DateTime? tray_back_date { get; set; }
    }
    /// <summary>
    /// 托盘出库回库状态
    /// </summary>
    public enum TrayOutStockStatusEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 待出库
        /// </summary>
        [Description("待出库")]
        WaitOut = 1,
        /// <summary>
        /// 已出库
        /// </summary>
        [Description("已出库")]
        TrayOuted = 2,
        /// <summary>
        /// 已组盘（分拣）
        /// </summary>
        [Description("已组盘")]
        TrayBinded = 3,
        /// <summary>
        /// 待回库
        /// </summary>
        [Description("待回库")]
        WaitIn = 4,
        /// <summary>
        /// 已回库
        /// </summary>
        [Description("已回库")]
        Stored = 5,
        /// <summary>
        /// 空托回库（回库取消，对于托盘组盘为空托盘，修改相应数据）
        /// </summary>
        [Description("空托回库")]
        Canceled = 6
    }
}
