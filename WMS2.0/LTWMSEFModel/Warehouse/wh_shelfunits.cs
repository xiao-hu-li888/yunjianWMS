using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Warehouse
{
    /// <summary>
    ///货架对应仓位详细表
    /// </summary>
    [Table("wh_shelfunits")]
    public class wh_shelfunits : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>
        [Required]
        public Guid warehouse_guid { get; set; }
        /// <summary>
        /// 货架guid（关联表：wh_shelves）
        /// </summary> 
        [Required]
        public Guid shelves_guid { get; set; }
        /// <summary>
        /// 库位（1排-32列-2层-2纵深）
        /// </summary>
        [StringLength(30)]
        public string shelfunits_pos { get; set; }
        /// <summary>
        /// 货架(排)
        /// </summary>
        public int rack { get; set; }
        /// <summary>
        /// 第几列
        /// </summary>
        public int columns { get; set; }
        /// <summary>
        /// 第几层
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 纵深1-托盘条码
        /// </summary>
        [StringLength(50)]
        public string depth1_traybarcode { get; set; }
        /// <summary>
        /// 纵深2-托盘条码
        /// </summary>
        [StringLength(50)]
        public string depth2_traybarcode { get; set; }
        /// <summary>
        /// 货架的纵深（从内(靠近堆垛机)到外(外侧)依次为0、1、2。。。）
        /// 默认值 0 
        /// 通过按货架的纵深depth降序分配入库从外到内。
        /// </summary>
        public int depth { get; set; }
        /// <summary>
        /// 单边多排库位，同一边标记相同的值
        /// </summary>
        public SameSideMarkEnum same_side_mark { get; set; }
        /// <summary>
        /// 库位状态
        /// </summary>   
        public ShelfCellState cellstate { get; set; }
        /// <summary>
        /// 锁类型
        /// </summary>
        public ShelfLockType locktype { get; set; }
        /// <summary>
        /// 出库锁定、指定库位锁定(0正常)
        /// </summary>
        public SpecialLockTypeEnum special_lock_type { get; set; }
        /// <summary>
        /// 出库完成后停用库位
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? stockout_thenstop { get; set; }
        /// <summary>
        /// 出库完成后加人工锁
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? stockout_thenmanlock { get; set; }
        /********判断托盘出入库时间，最新的为最后变动时间*********/
        /// <summary>
        /// 托盘入库时间
        /// </summary>
        public DateTime? tray_indatetime { get; set; }
        /// <summary>
        /// 托盘出库时间
        /// </summary>
        public DateTime? tray_outdatetime { get; set; }
        /********库位分区扩展字段*********/
        /// <summary>
        ///分区表guid(关联表：wh_shelfunits_area)
        /// </summary>
        public Guid? shelfunits_area_guid { get; set; }
        /// <summary>
        /// 分区编码
        /// </summary>
        [StringLength(50)]
        public string area_code { get; set; }

    }
    /// <summary>
    /// 出库锁定、指定库位锁定(0正常)
    /// </summary>
    public enum SpecialLockTypeEnum
    {
        /// <summary>
        /// 默认（正常）
        /// </summary>
        [Description("正常")]
        Normal = 0,
        /// <summary>
        /// 出库锁定(锁定后不能出库、或出库中不能继续出库)
        /// （锁定出库）
        /// </summary>
        [Description("出库锁定")]
        StockOutLock = 1,
        /// <summary>
        /// 指定入库锁定(锁定入库)
        /// </summary>
        [Description("指定入库锁定")]
        DispatchLock = 2
    }
    /// <summary>
    /// 锁类型
    /// </summary>
    public enum ShelfLockType
    {
        /*
         人工锁/系统锁不能入库，可以执行出库
         */
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 0,
        /// <summary>
        /// 人工锁（空库位才能上锁，任务强制完成或取消防止人为操作失误而加的锁）
        /// </summary>
        [Description("人工锁")]
        ManLock = 1,
        /// <summary>
        /// 系统锁（入库开始至出库结束，期间一直为系统锁）
        /// </summary>
        [Description("系统锁")]
        SysLock = 2
    }
    /// <summary>
    /// 库位状态 
    /// </summary>
    public enum ShelfCellState
    {
        /// <summary>
        /// 可入库（空库位）
        /// </summary>
        [Description("可入库")]
        CanIn = 0,
        /// <summary>
        /// 入库中
        /// </summary>
        [Description("入库中")]
        TrayIn = 1,
        /// <summary>
        /// 存储中
        /// </summary>
        [Description("存储中")]
        Stored = 2,
        /// <summary>
        /// 等待出库
        /// </summary>
        [Description("等待出库")]
        WaitOut = 3,
        /// <summary>
        /// 出库中
        /// </summary>
        [Description("出库中")]
        TrayOut = 4
    }
}
