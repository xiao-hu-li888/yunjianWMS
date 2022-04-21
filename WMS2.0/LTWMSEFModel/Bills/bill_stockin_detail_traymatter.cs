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
    /// 入库指定托盘或绑定的托盘
    /// </summary>
    [Table("bill_stockin_detail_traymatter")]
    public class bill_stockin_detail_traymatter : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }

        /// <summary>
        /// 入库单据guid (关联表：bill_stockin)
        /// </summary>
        [Required]
        public Guid stockin_guid { get; set; }

        /// <summary>
        /// 入库单详细表guid（关联表：bill_stockin_detail）
        /// </summary>
        [Required]
        public Guid stockin_detail_guid { get; set; }
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
        /// 存储库位（1-1-1）
        /// </summary>
        [StringLength(30)]
        public string dest_shelfunits_pos { get; set; }
        /// <summary>
        /// 存储库位guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? dest_shelfunits_guid { get; set; }
        /// <summary>
        /// 任务guid （关联表：hdw_stacker_taskqueue）
        /// </summary> 
        public Guid? stacker_taskqueue_guid { get; set; }
        /// <summary>
        /// 备注（任务失败，自动重新生成任务）
        /// </summary>
        [StringLength(1000)]
        public string memo { get; set; }
        /// <summary>
        /// 托盘入库状态
        /// </summary>
        public TrayInStockStatusEnum tray_status { get; set; }
        /// <summary>
        /// 托盘入库时间
        /// </summary>
        public DateTime? tray_in_date { get; set; }
    }
    /// <summary>
    /// 托盘入库状态
    /// </summary>
    public enum TrayInStockStatusEnum
    {
        /// <summary>
        ///  无
        /// </summary>
        [Description("无")] 
         None = 0,
        /// <summary>
        /// 已组盘
        /// </summary>
        [Description("已组盘")]
        TrayBinded = 1,
        /// <summary>
        /// 待入库
        /// </summary>
        [Description("待入库")]
        WaitIn = 2,
        /// <summary>
        /// 已入库
        /// </summary>
        [Description("已入库")]
        Stored = 3,
        /// <summary>
        /// 入库取消
        /// </summary>
        [Description("已取消")]
        Canceled=4
    }
    /// <summary>
    /// 检验状态（合格、待检、退回）
    /// </summary>
    public enum TestStatusEnum
    {
        /// <summary>
        /// 待检
        /// </summary>
        [Description("待检")]
        None = 0,
        /// <summary>
        /// 合格
        /// </summary>
        [Description("合格")]
        TestOk = 1,
        /// <summary>
        /// 不合格
        /// </summary>
        [Description("不合格")]
        TestFail = 2
    }
}
