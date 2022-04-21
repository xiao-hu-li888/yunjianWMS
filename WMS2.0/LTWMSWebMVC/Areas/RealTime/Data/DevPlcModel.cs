using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSEFModel.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LTWMSEFModel.Warehouse;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class DevPlcModel : BaseModel
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>  
        [Display(Name = "所属仓库")]
        public Guid? warehouse_guid { get; set; }

        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_shvwcs_srv）
        /// </summary>
        [Display(Name = "WCS服务")]
        public Guid shvwcs_srv_guid { get; set; } 
        /// <summary>
        /// 唯一编号(与wh_shvwcs_station配置表 number字段关联)
        /// </summary>
        [Display(Name = "编号")] 
        public int number { get; set; }
        /// <summary>
        ///  PLC类型
        /// </summary>
        [Display(Name = "PLC类型"), DropDownList("PLCType")]
        public DeviceTypeEnum type { get; set; }  
        /// <summary>
        /// 运行模式（实时状态）
        /// </summary>
        [Display(Name = "实时状态"), DropDownList("PLCRunStatus")]
        public PLCRunStatus run_status { get; set; }
    }
} 
