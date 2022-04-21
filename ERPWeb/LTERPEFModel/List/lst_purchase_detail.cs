using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.List
{
    /// <summary>
    /// 采购单详细
    /// </summary>
    [Table("lst_purchase_detail")]
    public class lst_purchase_detail:BaseEntity
    {
        /// <summary>
        /// 关联项目表：pro_project
        /// </summary>
        public Guid project_guid { get; set; }
        /// <summary>
        ///采购单guid（关联表：lst_purchase）
        /// </summary>
        public Guid purchase_guid { get; set; }

        //待补充字段。。。。。
    }
}
