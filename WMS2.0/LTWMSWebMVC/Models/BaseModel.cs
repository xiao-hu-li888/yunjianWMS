using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.App_Start.WebMvCEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC
{
    public class BaseModel
    {
        /// <summary>
        /// GUID
        /// </summary>
        [Key]
        [Display(Name = "GUID")]
        public Guid guid { get; set; }
        /// <summary>
        /// 状态
        /// </summary>          
        [Display(Name = "状态"), DropDownList("State")]
        public LTWMSEFModel.EntityStatus? state { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary> 
        [Display(Name = "创建日期")]
        public DateTime? createdate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Display(Name = "修改日期")]
        public DateTime? updatedate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        public string createuser { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [Display(Name = "修改人")]
        public string updateuser { get; set; }
        /**************并发*****************/
        ///// <summary>
        /////并发乐观锁（该字段无需修改，底层自动+1）
        ///// </summary>
        //[Display(Name = "并发乐观锁（该字段无需修改，底层自动+1）")]
        //public int rowversion { get; set; }
        /// <summary>
        /// 保存编辑界面传来的值（页面:guid+RowVersion加密数据）
        /// </summary> 
        public long? OldRowVersion { get; set; }

        /// <summary>
        /// 出库锁定、指定库位锁定(0正常)
        /// </summary>
        [Display(Name = "特殊锁"), DropDownList("SpecialLockTypeEnum")]
        public SpecialLockTypeEnum special_lock_type { get; set; }

    }
}