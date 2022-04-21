using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel
{
    public enum SimpleBackValue
    {
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        False = 0,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        True =1,
        /// <summary>
        /// 存在数据(添加前判断某个字段值是否在数据库中已存在)
        /// </summary>
        [Description("存在数据")]
        ExistsOfAdd =2,
        /// <summary>
        /// 批量删除数据-表中无记录
        /// </summary>
        [Description("表中无记录")]
        NotExistOfDelete =3,
        /// <summary>
        /// 并发冲突:数据已删除或已修改
        /// </summary>
        [Description("并发冲突:数据已删除或已修改")]
        DbUpdateConcurrencyException =4
    }
}
