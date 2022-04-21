using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Areas.Setting.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.Admin.Models
{
    public class ServiceStatusModel : BaseModel
    {
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        public Guid? wcs_srv_guid { get; set; }
        public WcsSrvModel WcsSrvModel {get;set;}
        /// <summary>
        /// 编号（1、2、3、4）编号唯一不能重复
        /// </summary>        
        [Display(Name = "编号")]
        public int number { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        [Display(Name = "IP")]
        public string ip { get; set; }
        /// <summary>
        /// 端口（20001）
        /// </summary>
        [Display(Name = "端口")]
        public int port { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(255)]
        public string desc { get; set; }
        /// <summary>
        /// wcs连接状态（实时状态）
        /// </summary>
        [Display(Name = "实时状态")]
        public WcsStatus wcs_status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        /// <summary>
        /// wcs调度系统分类
        /// </summary> 
        public WCSType wcstype { get; set; }
    }
}