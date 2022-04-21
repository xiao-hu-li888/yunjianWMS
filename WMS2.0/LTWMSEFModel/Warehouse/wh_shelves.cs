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
    /// 货架规格主表
    /// </summary>
    [Table("wh_shelves")]
    public class wh_shelves : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>
        [Required]
        public Guid warehouse_guid { get; set; }
        /// <summary>
        /// 仓库类别（立体库/平库）
        /// </summary>
        public WareHouseCategoriesEnum category { get; set; }

        /// <summary>
        /// 唯一标志（添加时判断）warehouse_guid+rack
        /// </summary>
        [StringLength(50)]
        public string u_identification { get; set; }

        /// <summary>
        /// 货架(排)
        /// </summary>
        public int rack { get; set; }
        /// <summary>
        /// 对应wcs中的货架（排）  可以一致 可以不一致，默认值是和rack一致
        /// </summary>
        public int rack_of_wcs { get; set; }
        /// <summary>
        /// 对应wcs中的列反转(null/false 默认（按wms定义的列顺序） true 列反转)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? columns_reversal_wcs { get; set; }
        /// <summary>
        /// 列偏移值（默认0：无偏移  其它值：偏移值） 举例：plc按两排U字形的列定义，即只定义一排。第二排的值通过列反转之后
        /// 需要加上第一排的偏移值（第一排总列数）
        /// </summary>
        public int columns_offset_wcs { get; set; }
        /// <summary>
        /// 列规格(x32列)
        /// </summary>
        public int columns_specs { get; set; }
        /// <summary>
        /// 层偏移值（默认0：无偏移  其它值：偏移值）
        /// </summary>
        public int rows_offset_wcs { get; set; }
        /// <summary>
        /// 层规格(x4层)
        /// </summary>
        public int rows_specs { get; set; }
        /// <summary>
        /// 货架的纵深（从内(靠近堆垛机)到外(外侧)为0、1、2。。。）
        /// 默认值 0 
        /// 通过按货架的纵深位降序分配入库从外到内。
        /// </summary>
        public int depth { get; set; }
        /// <summary>
        /// 单边多排库位，同一边标记相同的值
        /// </summary>
        public SameSideMarkEnum same_side_mark { get; set; }
        /// <summary>
        /// 是否已初始化仓位数据--如果已初始化，规格则不能修也不能删除
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? isinitialized { get; set; }
        /*********单机跑、双机联动？ 一台堆垛机 多台堆垛机？*******/
        /// <summary>
        ///多仓库wcs管理 关联表：wh_wcs_srv
        /// </summary>
        public Guid? wcs_srv_guid { get; set; }
        /// <summary>
        /// 出库通用处理逻辑
        /// </summary>
        public OutLogicEnum out_logic { get; set; }
        /*********************************************/

        /// <summary>
        /// 库位分配方式（入库请求库位分配排序）
        /// </summary>
        public StockDistributeEnum stock_distribute { get; set; }
    }
    /// <summary>
    /// 单边多排库位，同一边标记相同的值
    /// 多巷道定义多个面
    /// </summary>
    public enum SameSideMarkEnum
    {
        /// <summary>
        /// A面
        /// </summary>
        [Description("A面")] 
         SideA =0,
        /// <summary>
        /// B面
        /// </summary>
        [Description("B面")]
        SideB =1,
        /// <summary>
        /// C面
        /// </summary>
        [Description("C面")]
        SideC =2,
        /// <summary>
        /// D面
        /// </summary>
        [Description("D面")]
        SideD =3
    }
    /// <summary>
    /// 出库通用处理逻辑
    /// </summary>
    public enum OutLogicEnum
    {
        /// <summary>
        /// 通用处理逻辑
        /// </summary>
        [Description("通用处理逻辑")]
        Default = 0
    }
    /// <summary>
    /// 库位请求分配方式（从下至上分配）
    /// </summary>
    public enum StockDistributeEnum
    {
        /// <summary>
        /// 从下至上分配-默认分配方式(下层全部分配完，然后再分配上层，依次类推)
        /// </summary>
        [Description("从下往上（默认）")]
        LowerToUpper = 0,
        /// <summary>
        /// 从左至右分配（按列满往右分配） --从下至上
        /// </summary>
        [Description("从左往右")]
        LeftToRigth = 1,
        /// <summary>
        /// 从右至左分配（按列满往左分配）--从下至上
        /// </summary>
        [Description("从右往左")]
        RightToLeft = 2,
        ///// <summary>
        ///// 从中间至两边
        ///// </summary>
        //[Description("从中间往两边")]
        //MiddleToBothSides = 3,
        ///// <summary>
        ///// 从两边至中间
        ///// </summary>
        //[Description("从两边到中间")]
        //BothSidesToMiddle = 4,
        ///// <summary>
        /////  连续占位（测试可能用到）
        ///// </summary>
        //[Description("连续占位")]
        //Continuity = 5,
        /// <summary>
        /// 随机分配（测试可能用到）
        /// </summary>
        [Description("随机分配")]
        Random = 6
    }

}
