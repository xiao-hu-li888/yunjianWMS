using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class ShelfUnitsModel : BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid warehouse_guid { get; set; }
        /// <summary>
        /// 货架guid（关联表：wh_shelves）
        /// </summary>  
        public Guid shelves_guid { get; set; }
        /// <summary>
        /// 库位（1排-32列-2层-2纵深）
        /// </summary>
        [Display(Name = "库位")]
        public string shelfunits_pos { get; set; }
        /// <summary>
        /// 货架(排)
        /// </summary>
        [Display(Name = "排")]
        public int rack { get; set; }
        /// <summary>
        /// 第几列
        /// </summary>
        [Display(Name = "列")]
        public int columns { get; set; }
        /// <summary>
        /// 第几层
        /// </summary>
        [Display(Name = "层")]
        public int rows { get; set; }
        /// <summary>
        /// 纵深1-托盘条码
        /// </summary>
        [Display(Name = "托盘条码1")]
        public string depth1_traybarcode { get; set; }
        /// <summary>
        /// 纵深2-托盘条码
        /// </summary>
        [Display(Name = "托盘条码2")]
        public string depth2_traybarcode { get; set; }

        /// <summary>
        /// 库位状态
        /// </summary>   
        [Display(Name = "库位状态"), DropDownList("ShelfCellState")]
        public ShelfCellState cellstate { get; set; }
        /// <summary>
        /// 锁类型
        /// </summary>
        [Display(Name = "锁类型"), DropDownList("ShelfLockType")]
        public ShelfLockType locktype { get; set; }

        /// <summary>
        /// 出库完成后停用库位
        /// </summary> 
        public bool StockOutThenStop { get; set; }
        /// <summary>
        /// 出库完成后加人工锁
        /// </summary> 
        public bool StockOutThenManLock { get; set; }
        /********判断托盘出入库时间，最新的为最后变动时间*********/
        /// <summary>
        /// 托盘入库时间
        /// </summary>
        [Display(Name = "入库时间")]
        public DateTime? tray_indatetime { get; set; }
        /// <summary>
        /// 托盘出库时间
        /// </summary>
        [Display(Name = "出库时间")]
        public DateTime? tray_outdatetime { get; set; }
        /// <summary>
        /// 提交类型
        /// </summary>

        public int submit_type { get; set; }
        /// <summary>
        /// 托盘
        /// </summary>
        public TrayModel trayModel { get; set; }
        /// <summary>
        /// 物料类型
        /// </summary>
        public MatterTypeEnum matterType { get; set; }
    }
   
}