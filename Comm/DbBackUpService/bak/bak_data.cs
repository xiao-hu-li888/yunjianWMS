using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBackUpService.bak
{
     [Table("bak_data")]
    public class bak_data:BaseEntity
    {
        /// <summary>
        /// 表名
        /// </summary>
        [StringLength(100)]
        public string table_name { get; set; }
        /// <summary>
        /// 将数据库表序列化成json格式保存
        /// </summary>
        [Column(TypeName = "text")]
        public string json_data { get; set; }

    }
}
