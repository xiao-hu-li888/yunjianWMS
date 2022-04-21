using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.RealTime.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Setting.Data
{
    public class WcsDeviceModel : BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        [Display(Name = "所属仓库"), DropDownList("WareHouseGuidList2")]
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        [Display(Name = "WCS服务名称"), DropDownList("WCSSrvGuidList")]
        public Guid wcs_srv_guid { get; set; }

        /// <summary>
        /// WCS定义的站台编号（100、200、300 ...）/堆垛机编号（2001、2002、2003 ...）
        /// </summary>
        [Required(ErrorMessage = "WCS定义的站台编号不能为空"), Display(Name = "设备编号")]
        public int number { get; set; }
        /// <summary>
        /// 站台名称（1站台、2站台。。。）/堆垛机名称（1号堆垛机、2号堆垛机、。。。）
        /// </summary>
        [Required(ErrorMessage = "设备名称不能为空"), Display(Name = "名称")]
        public string name { get; set; }
        /// <summary>
        /// 设备类型（堆垛机、站台、输送线 ...）
        /// </summary>
        [Display(Name = "设备类型"), DropDownList("DeviceTypeEnum")]
        public DeviceTypeEnum device_type { get; set; }
        /***********站台配置信息***************/
        /// <summary>
        /// 默认出库口（如果没有指定出库口station=0，默认可以分配的出库口）
        /// </summary>
        [Display(Name = "默认出库口"), DropDownList("YesNoState")]
        public bool? default_out { get; set; }
        /// <summary>
        /// 是否需要扫描码组盘动作(true 需要 false/null 不需要)
        /// </summary>
        [Display(Name = "需要组盘"), DropDownList("YesNoState")]
        public bool? need_scan_bind { get; set; }
        /// <summary>
        /// 站台出入库模式
        /// </summary>
        [Display(Name = "站台出入库模式"), DropDownList("StationModeEnum")]
        public StationModeEnum station_mode { get; set; }
        /********多堆垛机操作同一排货架配置（后期完善功能）******/
        /// <summary>
        /// 操作起始列（数字：1） -- 默认第一列
        /// </summary>
        [Display(Name = "操作起始列")]
        public int stacker_col_start { get; set; }
        /// <summary>
        /// 操作终止列(数字：16) --默认最后一列
        /// </summary>
        [Display(Name = "操作终止列")]
        public int stacker_col_end { get; set; }
        /// <summary>
        /// 堆垛机状态：true 正常 false/null 故障（设置故障状态另一台堆垛机自动接管？？）
        /// </summary>
        [Display(Name = "堆垛机状态")]
        public bool? stacker_status { get; set; }

        ////////////////
        /// <summary>
        /// 关联设备状态
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// PLC设备的实时状态
        /// </summary>
        public DevPlcModel DevPlcModel{get;set;}
    }
}