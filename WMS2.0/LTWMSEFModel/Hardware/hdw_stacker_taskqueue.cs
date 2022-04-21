using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LTWMSEFModel.Hardware
{
    /// <summary>
    /// 任务队列（上架/下架/移库）
    /// </summary>
    [Table("hdw_stacker_taskqueue")]
    public class hdw_stacker_taskqueue : BaseEntity
    {
        /// <summary>
        /// id（由sys_table_id表管理id自增）
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架guid关联对应的wcs调度 （关联表：wh_shelves）
        /// </summary>  
        public Guid? shelves_guid { get; set; }
        /// <summary>
        ///移动目标库位 货架guid关联对应的wcs调度 （关联表：wh_shelves）
        /// </summary>  
        public Guid? shelves_guid_end { get; set; }
        /// <summary>
        /// 关联 wh_shelfunits仓位详细表--出库、移库起点仓位
        /// </summary> 
        public Guid? src_shelfunits_guid { get; set; }
        /// <summary>
        /// 关联 wh_shelfunits仓位详细表--入库、移库终点仓位
        /// </summary> 
        public Guid? dest_shelfunits_guid { get; set; }
        /*************出入库**************/
        /// <summary>
        /// 起点:排-列-层-纵深
        /// </summary>
        [StringLength(50)]
        public string src_shelfunits_pos { get; set; }
        /// <summary>
        ///终点:排-列-层-纵深
        /// </summary>
        [StringLength(50)]
        public string dest_shelfunits_pos { get; set; }
        /// <summary>
        /// 起点站台([A]或B取料口，用数字1、2代替)  【纵深1/纵深2】
        /// </summary>
        public int src_station { get; set; }
        /// <summary>
        /// 货架(排) 
        /// </summary>
        public int src_rack { get; set; }
        /// <summary>
        /// 起始--列
        /// </summary>
        public int src_col { get; set; }
        /// <summary>
        /// 起始--层
        /// </summary>
        public int src_row { get; set; }
        ///// <summary>
        ///// 取货/放货纵深【起始--纵深 如果纵深为1则出库只出一半，如果纵深为2则全部出库】
        ///// </summary>
        //public int src_depth { get; set; }
        ///// <summary>
        /////目标--排
        ///// </summary>
        /// <summary>
        /// 终点站台（纵深1/纵深2）
        /// </summary>
        public int dest_station { get; set; }
        public int dest_rack { get; set; }
        /// <summary>
        ///目标--列
        /// </summary>
        public int dest_col { get; set; }
        /// <summary>
        ///目标--层
        /// </summary>
        public int dest_row { get; set; }
        ///// <summary>
        /////目标-- 纵深
        ///// </summary>
        //public int dest_depth { get; set; }
        //托盘信息******************************* 
        /// <summary>
        /// 纵深1--托盘条码
        /// </summary>
        [StringLength(50)]
        public string tray1_barcode { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray1_matter_barcode1 { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray1_matter_barcode2 { get; set; }

        /// <summary>
        /// 物料信息集合(T01-1-1,T01-1-2,,,,,,)
        /// </summary>
        [Column(TypeName = "text")]
        public string matterbarcode_list { get; set; }
        /// <summary>
        ///纵深2--托盘条码
        /// </summary>
        [StringLength(50)]
        public string tray2_barcode { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray2_matter_barcode1 { get; set; }
        /// <summary>
        ///  物料条码（电池条码）
        /// </summary>
        [StringLength(50)]
        public string tray2_matter_barcode2 { get; set; }

        /******************************************/
        /// <summary>
        /// 执行任务的堆垛机编号
        /// </summary>
        [StringLength(30)]
        public string stacker_number { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public WcsTaskType tasktype { get; set; }
        /// <summary>
        /// 任务完成状态
        /// </summary>
        public WcsTaskStatus taskstatus { get; set; }

        /// <summary>
        /// 排序（插队,默认0，值越大优先级越大）
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime? startup { get; set; }
        /// <summary>
        /// 任务结束时间
        /// </summary>
        public DateTime? endup { get; set; }
        /// <summary>
        /// 任务信息
        /// </summary>
        [StringLength(1000)]
        public string memo { get; set; }
        /********************************************************/

        /// <summary>
        /// 是否空托盘
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_emptypallet { get; set; }
        /// <summary>
        ///界面操作：取消任务后重新生成出库任务标志(true:重新生成任务，false/nul不重新生成)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? regenerate_task_queue { get; set; }
        /// <summary>
        /// 新出库任务关联guid
        /// </summary>
        public Guid? new_task_queue_guid { get; set; }

        /***********发送成功失败重发**************/
        /// <summary>
        /// 发送成功/失败
        /// </summary>
        public bool send_ok { get; set; }
        /// <summary>
        /// 发送次数（默认重发3次？2次？超过失败不继续重发）
        /// </summary>
        public int send_count { get; set; }
        /************单据相关--任务执行完成后根据单据类型查找对应的表修改状态***********/
        /// <summary>
        /// 单号
        /// </summary>
        [StringLength(50)]
        public string order { get; set; }
        /// <summary>
        /// 单据类型 
        /// </summary>
        public BillsTypeEnum bills_type { get; set; }
        /// <summary>
        /// 与对应单据关联的详细绑定表的guid
        /// 入库单据：bill_stockin_detail_traymatter
        /// 出库单据：bill_stockout_detail_traymatter
        /// 盘点单据：
        /// </summary>
        public Guid? re_detail_traymatter_guid { get; set; }
        /********************************/
    }
    /// <summary>
    /// 单据类型（根据单据类型查找对应的表）
    /// </summary>
    public enum BillsTypeEnum
    {
        /// <summary>
        /// 默认(无)
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 盘点(非出库单的盘点单)
        /// </summary>
        [Description("盘点单")]
        Check = 1,
        /// <summary>
        /// 入库单
        /// </summary>
        [Description("入库单")]
        BillsIn = 2,
        /// <summary>
        /// 出库单（包含盘点单类型出库）
        /// </summary>
        [Description("出库单")]
        BillsOut = 3
    }
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum WcsTaskType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        StockIn = 0,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        StockOut = 1,
        /// <summary>
        /// 移库
        /// </summary>
        [Description("移库")]
        MoveTo = 2
    }

    /// <summary>
    /// 任务完成状态
    /// </summary>
    public enum WcsTaskStatus
    {
        /// <summary>
        /// 任务未下发(WMS未处理)
        /// </summary>
        [Description("未下发")]
        Holding = 0,
        /// <summary>
        /// 任务已下发(WCS未处理)
        /// </summary>
        [Description("已下发")]
        IsSend = 1,
        /// <summary>
        /// 任务暂停
        /// </summary>
        [Description("暂停")]
        Pause = 2,
        /// <summary>
        /// 任务已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 3,
        /// <summary>
        /// 任务异常
        /// </summary>
        [Description("异常")]
        Exception = 4,
        /// <summary>
        /// 任务已完成
        /// </summary>
        [Description("已完成")]
        Finished = 5,
        /// <summary>
        /// 任务强制完成(人工已完成操作，未经过调度堆垛机完成)
        /// </summary>
        [Description("强制完成")]
        ForceComplete = 6,
        /// <summary>
        ///任务执行中
        /// </summary>
        [Description("执行中")]
        Execute = 7,
        /// <summary>
        /// 正在取消任务(wms操作wcs取消任务)
        /// </summary>
        [Description("正在取消")]
        CancelHandling = 8,
        /// <summary>
        /// 正在强制完成(wms操作wcs强制完成任务)
        /// </summary>
        [Description("正在强制完成")]
        ForceCompleteHandling = 9,
        /// <summary>
        /// 已发送取消指令
        /// </summary>
        [Description("取消指令已发送")]
        CancelSended = 10,
        /// <summary>
        /// 已发送强制完成指令
        /// </summary>
        [Description("强制完成指令已发送")]
        ForceCompleteSended = 11,
        /// <summary>
        ///未分配库位(等待派发入库库位)
        /// </summary>
        [Description("等待分配库位")]
        WaiteDispatchStockCell = 12,
        /// <summary>
        /// 任务写入失败（WCS写PLC失败）
        /// </summary>
        [Description("写入PLC失败")]
        WriteError = 13,
        /// <summary>
        /// 正在发送任务异常指令
        /// </summary>
        [Description("正在发送异常指令")]
        ExceptionHandling = 14,
        /// <summary>
        /// 已发送任务异常指令
        /// </summary>
        [Description("已发送任务异常指令")]
        ExceptionSended = 15
    }
}
