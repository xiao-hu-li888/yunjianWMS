using LTWMSEFModel.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Warehouse
{
    /// <summary>
    /// 站台配置表（100、200、300 ...）
    /// </summary>
    [Table("wh_wcs_device")]
    public class wh_wcs_device : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        public Guid wcs_srv_guid { get; set; }
        /// <summary>
        /// 唯一标志（添加时判断）shvwcs_srv_guid+number
        /// </summary>
        [StringLength(100)]
        public string u_identification { get; set; }
        /// <summary>
        /// WCS定义的站台编号（100、200、300 ...）/堆垛机编号（2001、2002、2003 ...）
        /// </summary> 
        public int number { get; set; }
        /// <summary>
        /// 站台名称（1站台、2站台。。。）/堆垛机名称（1号堆垛机、2号堆垛机、。。。）
        /// </summary>
        [StringLength(20)]
        public string name { get; set; }
        /// <summary>
        /// 设备类型（堆垛机、站台、输送线 ...）
        /// </summary>
        public DeviceTypeEnum device_type { get; set; }
        /***********站台配置信息***************/
        /// <summary>
        /// 默认出库口（如果没有指定出库口station=0，默认可以分配的出库口）
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? default_out { get; set; }
        /// <summary>
        /// 是否需要扫描组盘动作(true 需要 false/null 不需要)
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? need_scan_bind { get; set; }
        /// <summary>
        /// 站台出入库模式
        /// </summary>
        public StationModeEnum station_mode { get; set; }
        /********多堆垛机操作同一排货架配置（后期完善功能）******/
        /// <summary>
        /// 操作起始列（数字：1） -- 默认第一列
        /// </summary>
        public int stacker_col_start { get; set; }
        /// <summary>
        /// 操作终止列(数字：16) --默认最后一列
        /// </summary>
        public int stacker_col_end { get; set; }
        /// <summary>
        /// 堆垛机状态：true 正常 false/null 故障（设置故障状态另一台堆垛机自动接管？？）
        /// </summary>
        public bool? stacker_status { get; set; }
        /// <summary>
        /// 运行模式（实时状态）
        /// </summary>
        [NotMapped]
        public PLCRunStatus run_status { get; set; }
        ////////////////
    }
    /// <summary>
    /// 设备类型（堆垛机、站台、输送线 ...）
    /// </summary>
    public enum DeviceTypeEnum
    {
        /// <summary>
        ///  站台（100、200、300、...）
        /// </summary>
        [Description("站台")]
        Station = 0,
        /// <summary>
        ///堆垛机（2001、2002、...）
        /// </summary>
        [Description("堆垛机")]
        Stacker = 1
    }
    /// <summary>
    /// 站台出入库模式
    /// </summary>
    public enum StationModeEnum
    {
        /// <summary>
        /// 可入可出
        /// </summary>
        [Description("可入可出")]
        InOut = 0,
        /// <summary>
        /// 只入
        /// </summary>
        [Description("只入")]
        InOnly = 1,
        /// <summary>
        /// 只出
        /// </summary>
        [Description("只出")]
        OutOnly = 2
    }
}
