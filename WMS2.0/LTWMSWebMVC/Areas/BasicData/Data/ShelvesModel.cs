using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.RealTime.Data;
using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class ShelvesModel : BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>
        [Required(ErrorMessage = "请选择所属仓库"), Display(Name = "所属仓库"), DropDownList("WareHouseGuidList2")]
        public Guid warehouse_guid { get; set; }
        /// <summary>
        /// 仓库类别（立体库/平库）
        /// </summary>
        [Display(Name = "仓库类别"),DropDownList("WareHouseCategoriesEnum")]
        public WareHouseCategoriesEnum category { get; set; }
        /// <summary>
        /// 对应wcs中的货架（排）  可以一致 可以不一致，默认值是和rack一致
        /// </summary>
        [Display(Name = "对应wcs货架（排）")]
        public int rack_of_wcs { get; set; }
        /// <summary>
        /// 对应wcs中的列反转(null/false 默认（按wms定义的列顺序） true 列反转)
        /// </summary>
        [Display(Name = "对应wcs列反转"),DropDownList("YesNoState")]
        public bool columns_reversal_wcs { get; set; }

        /// <summary>
        /// 列偏移值（默认0：无偏移  其它值：偏移值） 举例：plc按两排U字形的列定义，即只定义一排。第二排的值通过列反转之后
        /// 需要加上第一排的偏移值（第一排总列数）
        /// </summary>
        [Display(Name = "列偏移量")]
        public int columns_offset_wcs{ get; set; }

        /// <summary>
        /// 层偏移值（默认0：无偏移  其它值：偏移值）
        /// </summary>
        [Display(Name = "层偏移量")]
        public int rows_offset_wcs { get; set; }

        ///// <summary>
        /////多仓库wcs管理 关联表：wh_wcs
        ///// </summary>
        //public Guid? wcs_guid { get; set; }
        /// <summary>
        /// 货架(排)
        /// </summary>
        [Display(Name = "货架(排)")]
        public int rack { get; set; }
        /// <summary>
        /// 列规格(x32列)
        /// </summary>
        [Display(Name = "总列数")]
        public int columns_specs { get; set; }
        /// <summary>
        /// 层规格(x4层)
        /// </summary>
        [Display(Name = "总层数")]
        public int rows_specs { get; set; }  

        /// <summary>
        /// 货架的纵深（从内(靠近堆垛机)到外(外侧)为0、1、2。。。）
        /// 默认值 0 
        /// 通过按货架的纵深位降序分配入库从外到内。
        /// </summary>
        [Display(Name = "货架纵深")]
        public int depth { get; set; }

        /// <summary>
        /// 单边多排库位，同一边标记相同的值
        /// </summary>
       [Display(Name="货架同侧标记"),DropDownList("SameSideMarkEnum")]
        public SameSideMarkEnum same_side_mark { get; set; }

        /// <summary>
        /// 是否已初始化仓位数据--如果已初始化，规格则不能修也不能删除
        /// </summary>
        [Display(Name = "是否初始化"), DropDownList("YesNoState")]
        public bool isinitialized { get; set; }
        /// <summary>
        /// 数据提交类型
        /// </summary>
        public int submit_type{get;set;}
        /// <summary>
        /// 对应的库位信息
        /// </summary>
        public IEnumerable<ShelfUnitsModel> ShelfUnits { get; set; }

        /*********单机跑、双机联动？ 一台堆垛机 多台堆垛机？*******/
        /// <summary>
        ///多仓库wcs管理 关联表：wh_wcs_srv
        /// </summary>
        [Display(Name = "WCS服务名称"),DropDownList("WCSSrvGuidList")]
        public Guid? wcs_srv_guid { get; set; }
        /// <summary>
        /// 出库通用处理逻辑
        /// </summary>
        [Display(Name = "出库处理逻辑"),DropDownList("OutLogicEnum")]
        public OutLogicEnum out_logic { get; set; }
        /*********************************************/

        /// <summary>
        /// 库位分配方式（入库请求库位分配排序）
        /// </summary>
        [Display(Name = "库位分配方式"),DropDownList("StockDistributeEnum")]
        public StockDistributeEnum stock_distribute { get; set; }
        /// <summary>
        /// wcs服务对象
        /// </summary>
        public WcsSrvModel WcsSrvModel { get; set; }
        ///// <summary>
        ///// 已关联的设备guid列表
        ///// </summary>
        //public List<Guid> ListWcsDeviceGuid { get; set; }
    }
}
 