using LTWMSEFModel.Bills;
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
    /// 货架存放容器（根据不同的业务需求可以是单独放托盘或料箱，其中托盘上可以放多个料箱）
    /// </summary>
    public enum TrayTypeEnum
    {
        /// <summary>
        /// 托盘（托盘可以绑定多个料箱）
        /// </summary>
        Tray = 0,
        /// <summary>
        /// 料箱
        /// </summary>
        Box = 1
    }
    /// <summary>
    /// 托盘表
    /// </summary>
    [Table("wh_tray")]
    public class wh_tray : BaseEntity
    {
        /// <summary>
        /// 托盘或料箱（默认托盘,如果只有料箱，默认料箱=托盘） 托盘条码一般T开头，料箱B开头
        /// </summary>
        public TrayTypeEnum tray_type { get; set; }
        /// <summary>
        /// 托盘条码(多个仓库托盘条码必须唯一) 新增修改检查唯一性
        /// </summary>
        [StringLength(100)]
        public string traybarcode { get; set; }
        ///// <summary>
        ///// 电池入库单(电池入库单处理方式不一样)/其它物料入库
        ///// </summary>
        //public BillsProperty bill_property { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal weight { get; set; }
        /// <summary>
        /// 空托盘
        /// </summary>
        public bool emptypallet { get; set; }
        /// <summary>
        /// 关联物料的种类
        /// </summary>
        public int totalkind { get; set; }
        /// <summary>
        /// 关联物料的总数量
        /// </summary>
        public int totalnumber { get; set; }
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        ///托盘具体存放位置--货架仓位guid（关联表：wh_shelfunits）
        /// </summary>  
        public Guid? shelfunits_guid { get; set; }
        /// <summary>
        /// 托盘上架状态(在货架上不能关联绑定物料???)
        /// </summary>
        public TrayStatus status { get; set; }
        /// <summary>
        /// 托盘所在库位（1排-2列-1层）
        /// </summary>
        [StringLength(50)]
        public string shelfunits_pos { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 托盘是否扫码组盘（不扫描不能入库。。。）
        /// </summary>
        public bool? isscan { get; set; }
        /// <summary>
        /// 组盘时间，组盘超过4小时需重新组盘才能入库
        /// </summary>
        public DateTime? scandate { get; set; }
        /**************指定库位***************/
        /// <summary>
        /// 组盘指定分配库位（给对应库位上系统锁）
        ///库位表guid（关联表：wh_shelfunits）
        /// </summary>
        public Guid? dispatch_shelfunits_guid { get; set; }
        /// <summary>
        /// 托盘所在库位（1排-2列-1层）
        /// </summary>
        [StringLength(50)]
        public string dispatch_shelfunits_pos { get; set; }
    }
    /// <summary>
    /// 托盘状态（是否在货架上）
    /// </summary>
    public enum TrayStatus
    {
        /// <summary>
        /// 不在货架上
        /// </summary>
        [Description("未上架")]
        OffShelf = 0,
        /// <summary>
        /// 在货架上
        /// </summary>
        [Description("已上架")]
        OnShelf = 1
    }
}
