using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel
{ 
    public abstract partial class BaseEntity : BaseBaseEntity
    { 
        /// <summary>
        /// 状态
        /// </summary>          
        public EntityStatus state { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary> 
        [Required]
        public DateTime createdate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        //[Column("eff_ts", Order = 2, TypeName = "datetime2")]
        public DateTime? updatedate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(20)]
        public string createuser { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [StringLength(20)]
        public string updateuser { get; set; }
        /**************并发*****************/
        /// <summary>
        ///并发乐观锁（该字段无需修改，底层自动+1）
        /// </summary>
        [ConcurrencyCheck]
        public long rowversion { get; set; }
        /// <summary>
        /// 保存编辑界面传来的值（页面:guid+RowVersion加密数据）
        /// </summary>
        [NotMapped]
        public long? OldRowVersion { get; set; }
        /**************并发*****************/
    }

    public abstract partial class BaseBaseEntity
    { 
        /// <summary>
        /// GUID
        /// </summary>
        [Key]
        public Guid guid { get; set; } 
    }
}
