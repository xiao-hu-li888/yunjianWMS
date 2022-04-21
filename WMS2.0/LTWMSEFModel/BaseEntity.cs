using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel
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

        /****************扩展字段****************/ 
        /// <summary>
        /// 扩展字段1
        /// </summary>
        [StringLength(255)]
        public string ext_field1 { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        [StringLength(255)]
        public string ext_field2 { get; set; } 
        /// <summary>
        /// 扩展字段3
        /// </summary> 
        public int ext_field3 { get; set; }
        /// <summary>
        /// 扩展字段4
        /// </summary> 
        public int ext_field4 { get; set; } 
    }
    /// <summary>
    /// 任务状态、guid、等信息(单据详细表的出入库执行状态<=>关联任务hdw_stacker_taskqueue表)
    /// </summary>
    public abstract partial class BaseBaseTaskStatusEntity : BaseBaseEntity
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskStatusEnum task_status { get; set; }
        /// <summary>
        /// 任务guid（关联表hdw_stacker_taskqueue）
        /// </summary>
        public Guid? task_guid { get; set; }
       // 托盘号
        //仓位
    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatusEnum
    {
        /// <summary>
        /// 无（单据未处理没有生成出入库任务）
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 待出库（已经生成了任务）
        /// </summary>
        [Description("待出库")]
        WaitedSend = 1,
        /// <summary>
        /// 执行中
        /// </summary>
        [Description("执行中")]
        Running = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Finished = 3,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 4
    }
}
