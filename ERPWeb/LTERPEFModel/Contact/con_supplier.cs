using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Contact
{
    /// <summary>
    /// 供应商
    /// </summary>
    [Table("con_supplier")]
    public class con_supplier : BaseEntity
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string sup_name { get; set; }
        /// <summary>
        /// 首字母
        /// </summary>
        [StringLength(50)]
        public string name_abb
        {
            get;
            set;
        } 
        /// <summary>
        /// 关联公司类别（字典：sys_dictionary>>CompType 枚举）
        /// </summary>
        public Guid? comptype_guid { get; set; }
        /// <summary>
        /// 公司类别
        /// </summary>
        [StringLength(50)]
        public string comptype_name { get; set; }

        /// <summary>
        /// 关联供应商类型（字典：sys_dictionary>>SupplierType 枚举）
        /// </summary>
        public Guid? suppliertype_guid { get; set; }
        /// <summary>
        /// 供应商类型
        /// </summary>
        [StringLength(50)]
        public string suppliertype_name { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        [StringLength(50)]
        public string province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(50)]
        public string city { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(255)]
        public string address { get; set; }
        /// <summary>
        ///  邮编
        /// </summary>
        [StringLength(10)]
        public string zipcode { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [StringLength(50)]
        public string comp_phone { get; set; }
        /// <summary>
        /// 合作开始时间
        /// </summary> 
        public DateTime? begintime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        public string memo { get; set; }
    }
}
