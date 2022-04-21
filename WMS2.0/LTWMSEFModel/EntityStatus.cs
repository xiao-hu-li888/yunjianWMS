using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel
{ 
    public enum EntityStatus
    {
        /// <summary>
        /// 已禁用
        /// </summary>
        [Description("已禁用")]
        Disabled = 0,
        /// <summary>
        /// 正常(启用)
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 已删除(回收站)
        /// </summary>
        [Description("已删除")]
        Deleted = 3
    }
}
