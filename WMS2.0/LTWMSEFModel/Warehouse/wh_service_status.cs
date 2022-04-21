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
    /// wms、wcs等服务系统状态表
    /// </summary>
    [Table("wh_service_status")]
    public class wh_service_status : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        public Guid? wcs_srv_guid { get; set; }
        ///// <summary>
        ///// 编号（1、2、3、4）编号唯一不能重复
        ///// </summary>        
        //public int number { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(255)]
        public string desc { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        [StringLength(30)]
        public string ip { get; set; }
        /// <summary>
        /// 端口（20001）
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// wcs连接状态（实时状态）
        /// </summary>
        public WcsStatus wcs_status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string memo { get; set; }
        /// <summary>
        /// wcs调度系统分类
        /// </summary>
        public WCSType wcstype { get; set; }
    }
    /// <summary>
    /// wcs类型
    /// </summary>
    public enum WCSType
    {
        /// <summary>
        /// 堆垛机调度系统
        /// </summary>
        [Description("堆垛机调度系统")]
        Stacker = 0,
        /// <summary>
        /// agv调度系统
        /// </summary>
        [Description("agv调度系统")]
        Agv = 1,
        /// <summary>
        /// 集装箱调度系统
        /// </summary>
        [Description("集装箱调度系统")]
        Pcs = 2,
        /// <summary>
        /// WMS服务
        /// </summary>
        [Description("WMS服务")]
        WMSWinServer=3,
        /// <summary>
        /// WCS服务
        /// </summary>
        [Description("WCS服务")]
        WCSServer =4,

        /// <summary>
        /// WCS Socket处理接收到的消息服务
        /// </summary>
        SRV_DealReceive = 5,
        /// <summary>
        ///WCS Socket 处理生成消息
        /// </summary>
        SRV_DealSend = 6,
        /// <summary>
        /// WCS Socket处理历史消息
        /// </summary>
        SRV_DealToHistory = 7
    }
    /// <summary>
    /// wcs连接状态
    /// </summary>
    public enum WcsStatus
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        [Description("断开连接")]
        Disconnected = 0,
        /// <summary>
        /// 已连接
        /// </summary>
        [Description("已连接")]
        Connected = 1
    }
}
