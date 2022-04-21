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
    /// 货架关联的wcs处理服务（wcs处理服务socket服务端/客户端）
    /// 一个wcs里面对应的堆垛机和出入库口编号必须唯一
    /// </summary>
    [Table("wh_wcs_srv")]
    public class wh_wcs_srv : BaseEntity
    {
        //一个wcs 可以管理多个仓库
        ///// <summary>
        ///// 所属仓库 关联表：wh_warehouse
        ///// </summary> 
        //public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// WCS编号 10/11/12
        /// 入库指令： --C11M100D10--  C11入库指令/M100站台/D10wcs编号
        ///出库准备好：--C12M100D10--  C12出库准备好/M100站台/D10wcs编号
        ///请求空托盘：--C13M100D10--  C13请求空托盘/M100站台/D10wcs编号
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// wcs服务名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }
        /// <summary>
        /// wcs服务类型
        /// </summary>
        public WcsServerType srv_type { get; set; }
        /// <summary>
        /// 唯一标志（添加时判断）ip+port (一个IP、端口 只能启动一个监听不能重复，重复无法启动socket监听,不管是服务端还是做客户端都一样不能重复)
        /// </summary>
        [StringLength(100)]
        public string u_identification { get; set; }
        //通用 入库 出库 逻辑  
        /// <summary>
        /// IP (如果是服务器 启动 ip 端口，如果是客户端 则配置为wcs ip 端口)
        /// </summary>
        [StringLength(30)]
        public string ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int port { get; set; }

    }
    /// <summary>
    /// wcs socket服务处理类型
    /// </summary>
    public enum WcsServerType
    {
        /// <summary>
        /// 服务端（启动监听ip、端口）
        /// </summary>
        [Description("Socket服务端")]
        Server = 0,
        /// <summary>
        /// 客户端（连接远程ip、端口）
        /// </summary>
        [Description("Socket客户端")]
        Client = 1
    }
}
