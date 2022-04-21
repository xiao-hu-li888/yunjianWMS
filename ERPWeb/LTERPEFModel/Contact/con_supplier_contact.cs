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
    /// 供应商联系人
    /// </summary>
    [Table("con_supplier_contact")]
    public class con_supplier_contact : BaseEntity
    {
        /// <summary>
        /// 关联供应商guid（表：con_supplier）
        /// </summary>
        public Guid supplier_guid { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required, StringLength(50)]
        public string name { get; set; }
        /// <summary>
        /// 姓名首字母
        /// </summary>
        [StringLength(50)]
        public string name_abb { get; set; }
        /// <summary>
        /// 性别(true: 男 false/null:女)
        /// </summary> 
        [Column(TypeName = "bit")]
        public bool? gender { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        [StringLength(50)]
        public string engname { get; set; }
        /// <summary>
        /// 公司电话
        /// </summary>
        [StringLength(100)]
        public string comp_phone { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [StringLength(50)]
        public string mobile { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [StringLength(255)]
        public string email { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(255)]
        public string post { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        public string memo { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(255)]
        public string address { get; set; }
    }
}
