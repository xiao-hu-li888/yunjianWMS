using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace LTWMSEFModel.Hardware
{
    /// <summary>
    /// plc状态信息（堆垛机/输送线）
    /// </summary>
    [Table("hdw_plc")]
    public class hdw_plc : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>  
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        public Guid shvwcs_srv_guid { get; set; }
        /// <summary>
        /// 唯一标志（添加时判断）shvwcs_srv_guid+number
        /// </summary>
        [StringLength(100)]
        public string u_identification { get; set; }
        /// <summary>
        /// 唯一编号(与wh_shvwcs_station配置表 number字段关联)
        /// </summary> 
        public int number { get; set; }
        /// <summary>
        ///  PLC类型
        /// </summary>
        public DeviceTypeEnum type { get; set; }
       // /// <summary>
       // /// ip地址
       // /// </summary>
       // [StringLength(20)]
       // public string ip { get; set; }
       // /// <summary>
       // /// 端口
       // /// </summary>
       // public int port { get; set; }
       // /// <summary>
       // /// 连接状态（true：已连接，false/null 离线）
       // /// </summary>
       // [Column(TypeName = "bit")]
       // public bool? connected { get; set; }
       // /// <summary>
       // /// 连接时间/断开连接时间
       // /// </summary>
       // public DateTime? conn_disconnect_time { get; set; }
       // /// <summary>
       // /// 1自动模式/0手动模式
       // /// </summary>
       // [Column(TypeName = "bit")]
       // public bool? automode { get; set; }
       // /// <summary>
       // /// 双机联动 （1双机联动模式,0单机联动）
       // /// </summary>
       // [Column(TypeName = "bit")]
       // public bool? double_linked { get;set;}
       // /// <summary>
       // /// 备注
       // /// </summary>
       //[StringLength(255)]
       // public string memo { get; set; }
        /// <summary>
        /// 运行模式（实时状态）
        /// </summary>
        public PLCRunStatus run_status { get; set; }
    }
    /// <summary>
    /// 运行状态
    /// </summary>
    public enum PLCRunStatus
    {
        /// <summary>
        /// 无状态(断开连接??)
        /// </summary>
        [Description("无")]
        None =0,
        /// <summary>
        /// 空闲/准备好
        /// </summary>
        [Description("准备好")]
        Ready = 1,
        /// <summary>
        /// 运行中/任务中
        /// </summary>
        [Description("运行中")]
        Running = 2,
        /// <summary>
        /// 报警
        /// </summary>
        [Description("报警")]
        Warning =3,
        /// <summary>
        /// 与PLC断开连接
        /// </summary>
        [Description("断开连接")]
        DisConnected=4
    }
    ///// <summary>
    ///// PLC类型
    ///// </summary>
    //public enum PLCType
    //{
    //    /// <summary>
    //    /// 堆垛机（2001、2002、...）
    //    /// </summary>
    //    [Description("堆垛机")]
    //    Stacker = 0,
    //    /// <summary>
    //    /// 站台（100、200、300、...）
    //    /// </summary>
    //    [Description("站台")]
    //    Station = 1
    //}
}
