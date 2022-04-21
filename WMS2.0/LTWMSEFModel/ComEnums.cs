using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel
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
        /// 物料
        /// </summary>
        Matter = 1
    }
    /// <summary>
    /// 物料排序
    /// </summary>
    public enum MatterOrderEnum
    {
        [Description("入库降序")]
        CreateDateDesc = 0,
        [Description("入库升序")]
        CreateDateAsc = 1,
        [Description("有效期升序")]
        EffectiveDateAcs = 2,
        [Description("有效期降序")]
        EffectiveDateDesc = 3
    }

    /// <summary>
    /// 单据状态（查询用）
    /// </summary>
    public enum SearchBillsStatus_In
    {
        /// <summary>
        /// 进行中（待入库+进行中）
        /// </summary>
        [Description("进行中")]
        Running = 0, 
        /// <summary>
        /// 入库完成
        /// </summary>
        [Description("入库完成")]
        Finished = 1
    }
    /// <summary>
    /// 单据状态（查询用）
    /// </summary>
    public enum SearchBillsStatus_Out
    {
        /// <summary>
        /// 进行中（待入库+进行中）
        /// </summary>
        [Description("进行中")]
        Running = 0,
        /// <summary>
        /// 入库完成
        /// </summary>
        [Description("出库完成")]
        Finished = 1
    }
}
