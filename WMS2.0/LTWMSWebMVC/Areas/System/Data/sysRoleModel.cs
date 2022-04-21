using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.System.Data
{
    public class sysRoleModel : BaseModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空"), Display(Name = "角色名称")]
        public string rolename { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string remark { get; set; }
        /// <summary>
        /// 所有权限
        /// </summary>
        [Display(Name = "菜单权限")]
        public string permissiontext { get; set; }
        /// <summary>
        ///  所属仓库 关联表：wh_warehouse
        ///  一个角色可以对应多个仓库
        ///  warehouse_guid,warehouse_guid,warehouse_guid,warehouse_guid .....
        /// </summary>
        [Display(Name = "仓库权限")]
        public string warehouse_guid_text { get; set; }
        /// <summary>
        /// 仓库权限 逗号分割
        /// </summary>
        [Display(Name = "仓库权限")]
        public string warehouse_permission_text { get; set; }
        /// <summary>
        /// 仓库权限guid列表
        /// </summary>
        public List<Guid> list_warehouse_guid_permision { get; set; }
        /// <summary>
        /// 仓库分区列表
        /// </summary>
        public List<WarehouseTypeModel> list_warehouseTypeModel { get; set; }

        /// <summary>
        /// 启用或禁用
        /// </summary>
        [Display(Name = "启用状态"), DropDownList("Active")]
        public bool active { get; set; }
    }
}