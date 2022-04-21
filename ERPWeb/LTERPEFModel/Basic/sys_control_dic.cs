using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Basic
{
    /// <summary>
    /// IIS/桌面程序 与windows服务之间的控制与数据交互
    /// </summary>
    [Table("sys_control_dic")]
    public class sys_control_dic : BaseEntity
    {
        /// <summary>
        /// 字典键(系统设定，不能修改)
        /// </summary>
        [StringLength(50)]
        public string key { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [StringLength(255)]
        public string value { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50)]
        public string desc { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int sort { get; set; }
    }
}
