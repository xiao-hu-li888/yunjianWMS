using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Hardware
{
    /// <summary>
    /// 接收到的消息表（WCS、ACS、ERP、Mes等接口对接消息）
    /// </summary>
    [Table("hdw_message_received")]
    public class hdw_message_received : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary>  
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架关联的wcs表 guid（关联表wh_wcs_srv）
        /// </summary>
        public Guid? wcs_srv_guid { get; set; }
        /// <summary>
        /// json数据
        /// </summary>
        [Column(TypeName = "text")]
        public string json_data { get; set; }
        /// <summary>
        /// 消息处理状态
        /// </summary>
        public InterfaceMessageDealStatus deal_status { get; set; }
        /// <summary>
        /// 失败次数（失败超过N次，系统提示。。。正常情况不会失败）
        /// 一般都是料筐信息的传递，如果处理失败接着处理下一个消息。
        /// </summary>
        public int error_count { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? handle_date { get; set; }
        /// <summary>
        /// 接口消息类型（wcs、acs、erp、mess等等）
        /// </summary>
        public InterfaceMessageTypeEnum message_type { get; set; }
        /// <summary>
        /// 处理失败异常消息
        /// </summary>
        [Column(TypeName = "text")]
        public string memo { get; set; }
    }

    /// <summary>
    /// 消息处理状态
    /// </summary>
    public enum InterfaceMessageDealStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        None = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Done = 1,
        /// <summary>
        /// 处理失败（处理失败的料筐返回null数据给输送线？ 输送线控制去人工做处理。。。？？）
        /// </summary>
        [Description("处理失败")]
        Failed = 2
    }

    /// <summary>
    /// 接口消息类型（wcs、acs、erp、mess等等）
    /// </summary>
    public enum InterfaceMessageTypeEnum
    {
        /// <summary>
        /// WCS系统接口
        /// </summary>
        [Description("WCS")]
        WCS = 0,
        /// <summary>
        /// Agv调度系统接口
        /// </summary>
        [Description("ACS")]
        ACS = 1,
        /// <summary>
        /// ERP系统接口
        /// </summary>
        [Description("ERP系统接口")]
        ERP = 2,
        /// <summary>
        /// MES系统接口
        /// </summary>
        [Description("MES系统接口")]
        MES = 3,
        /// <summary>
        /// SAP系统接口
        /// </summary>
        [Description("SAP系统接口")]
        SAP = 4
    }
}
