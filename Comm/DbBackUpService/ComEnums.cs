using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBackUpService
{
    /// <summary>
    /// 物料类型
    /// </summary>
    public enum MatterTypeEnum
    {
        /// <summary>
        /// 空托盘
        /// </summary>
        [Description("空托盘")]
        Empty = 0,
        /// <summary>
        /// 物料
        /// </summary>
        [Description("物料")]
        Matter = 1
    }
    /// <summary>
    /// 生产者处理状态（0未处理，处理完成不再处理）
    /// </summary>
    public enum ProduceDealStatus
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
        Done = 1
    }
}
