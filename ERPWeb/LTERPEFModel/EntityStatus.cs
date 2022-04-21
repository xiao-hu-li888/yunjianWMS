using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel
{

    /// <summary>
    /// 物料类型
    /// </summary>
    public enum MatterTypeEnum
    {
        /// <summary>
        /// 空托盘
        /// </summary>
        Empty = 0,
        /// <summary>
        /// 电池
        /// </summary>
        Battery = 1,
        /// <summary>
        /// 其它物料
        /// </summary>
        OtherMatter = 2
    }
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
