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
    /// 仓库表
    /// </summary>
    [Table("wh_warehouse")]
    public class wh_warehouse : BaseEntity
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        [StringLength(30)]
        public string name { get; set; }
        /// <summary>
        /// 仓库编号唯一
        /// </summary>
        [StringLength(30)]
        [Required]
        public string code { get; set; }
        /// <summary>
        /// 仓库分类（分区表）表guid（关联表wh_warehouse_type）
        /// </summary>
        public Guid? warehouse_type_guid { get; set; }
        /// <summary>
        /// 仓库分区/分类 名称（末级分类名称）
        /// </summary>
        [StringLength(50)]
        public string warehouse_type_name { get; set; }
        /// <summary>
        /// 仓库分区/分类 名称(所有父类子类名称)
        /// </summary>
        [StringLength(50)]
        public string warehouse_type_name_all { get; set; }
        /// <summary>
        /// 仓库类别（立体库/平库）
        /// </summary>
        public WareHouseCategoriesEnum category { get; set; }
        /// <summary>
        /// 针对单边多排的货物存放策略，一般只有2排、支持任意多排
        /// 0：将最外侧的货架整个存满然后再存内侧，避免频繁的移库动作（一个移库动作最少也要20来秒）
        /// 1：外侧内侧同时存遵循路径最短原则，但存在频繁移库的可能性
        /// </summary>
        public DistributeWayEnum distribute_way { get; set; }
        /// <summary>
        ///仓库地址
        /// </summary>
        [StringLength(50)]
        public string address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(300)]
        public string remark { get; set; }
        /// <summary>
        /// 托盘包含条码【有条码的扫条码，没有条码的直接入库?】
        /// </summary>
        public bool tray_contains_barcode { get; set; } 
    } 
    /// <summary>
    /// 针对单边多排(双伸、3伸)的货物存放策略，一般只有2排、支持任意多排
    /// 0：将最外侧的货架整个存满然后再存内侧，避免频繁的移库动作（一个移库动作最少也要20来秒）
    /// 1：外侧内侧同时存遵循路径最短原则，但存在频繁移库的可能性
    /// </summary>
    public enum DistributeWayEnum
    {
        /// <summary>
        /// 0：将最外侧的货架整个存满然后再存内侧，避免频繁的移库动作（一个移库动作最少也要20来秒）
        /// </summary>
        [Description("外侧存满再存内侧(库存<=50%不需要移库)")]
        SidesToMiddle = 0,
        /// <summary>
        /// 1：外侧内侧同时存遵循路径最短原则，但存在频繁移库的可能性
        /// </summary>
        [Description("多排同时存放(频繁移库)")]
        BothSides = 1
    }
    /// <summary>
    /// 仓库类别（立体库/平库）
    /// </summary>
    public enum WareHouseCategoriesEnum
    {
        /// <summary>
        /// 立库
        /// </summary>
        [Description("立库")]
        AutomatedWarehouse = 0,
        /// <summary>
        /// 平库
        /// </summary>
        [Description("平库")]
        FlatWarehouse = 1
    }
}
