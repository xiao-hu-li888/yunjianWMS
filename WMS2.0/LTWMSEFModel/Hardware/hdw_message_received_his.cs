using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Hardware
{
    /// <summary>
    ///  接收到的消息表（WCS、ACS、ERP、Mes等接口对接消息）--历史表
    /// </summary>
    [Table("hdw_message_received_his")]
    public class hdw_message_received_his:BaseEntity
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
}
