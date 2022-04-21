using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Setting.Data
{
    public class WcsSrvModel : BaseModel
    {
        ///// <summary>
        ///// 所属仓库 关联表：wh_warehouse
        ///// </summary> 
        //[Display(Name = "所属仓库"), DropDownList("WareHouseGuidList2")]
        //public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// WCS编号 10/11/12
        /// 入库指令： --C11M100D10--  C11入库指令/M100站台/D10wcs编号
        ///出库准备好：--C12M100D10--  C12出库准备好/M100站台/D10wcs编号
        ///请求空托盘：--C13M100D10--  C13请求空托盘/M100站台/D10wcs编号
        /// </summary>
        [Required(ErrorMessage = "WCS编号不能为空"), Display(Name = "WCS编号(扫码指令关联)")]
        public int code { get; set; }
        /// <summary>
        /// wcs服务名称
        /// </summary>
        [Required(ErrorMessage = "WCS服务名称不能为空"), Display(Name = "WCS服务名称")]
        public string name { get; set; }
        /// <summary>
        /// wcs服务类型
        /// </summary>
        [Display(Name = "WCS服务类型"), DropDownList("WcsServerType")]
        public WcsServerType srv_type { get; set; }

        //通用 入库 出库 逻辑  
        /// <summary>
        /// IP (如果是服务器 启动 ip 端口，如果是客户端 则配置为wcs ip 端口)
        /// </summary>
        [Required(ErrorMessage = "IP不能为空"), Display(Name = "IP")]
        public string ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [Required(ErrorMessage = "端口不能为空"), Display(Name = "端口")]
        public int port { get; set; }
        /// <summary>
        /// 设备列表
        /// </summary>
        public List<WcsDeviceModel> List_wcsDeviceModel { get; set; }

    }
}