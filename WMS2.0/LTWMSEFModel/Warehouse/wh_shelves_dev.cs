using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Warehouse
{
    /// <summary>
    /// 货架和站台、堆垛机关联表（一个货架至少对应一个出入库站点）
    /// </summary>
    [Table("wh_shelves_dev")]
    public class wh_shelves_dev:BaseBaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 货架guid（关联表：wh_shelves）
        /// </summary>  
        public Guid shelves_guid { get; set; }
        /// <summary>
        ///设备（站台、堆垛机）表guid（关联表：wh_wcs_device）
        /// </summary>
        public Guid wcs_device_guid { get; set; }
    }
}
