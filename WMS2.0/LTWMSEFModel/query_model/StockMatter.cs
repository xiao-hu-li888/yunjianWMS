using LTWMSEFModel.Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.query_model
{
    /// <summary>
    /// 视图模型：
    /// </summary>
    public class StockMatter : BaseEntity
    {
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [StringLength(100)]
        public string code { get; set; }
        /// <summary>
        /// 编码（01-01-01） -分割符
        /// </summary>
        [StringLength(120)]
        public string code_full { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(100)]
        [Required]
        public string name { get; set; }
        /// <summary>
        /// 助记码（拼音首字母）
        /// </summary>
        [StringLength(100)]
        public string name_pinyin { get; set; }

   
        /// <summary>
        /// 规格
        /// </summary>
        [StringLength(255)]
        public string specs { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public decimal length { get; set; }
        /// <summary>
        /// 描述(型号)
        /// </summary>
        [StringLength(255)]
        public string description { get; set; }
        /// <summary>
        /// 关联品牌（字典：sys_dictionary>>Brand 枚举）
        /// </summary>
        public Guid? brand_guid { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [StringLength(50)]
        public string brand_name { get; set; }
        /// <summary>
        ///物料类型/货品类型-- 关联表：stk_mattertype
        /// </summary>
        public Guid? mattertype_guid { get; set; }
        /// <summary>
        /// 物料类型名称
        /// </summary>
        [StringLength(50)]
        public string mattertype_name { get; set; }
        ///// <summary>
        ///// 物料条码/条形码
        ///// </summary>
        //[StringLength(50)]
        //public string barcode { get; set; }
        /// <summary>
        /// 默认仓库 关联表：wh_warehouse
        /// </summary>
        public Guid? def_warehouse_guid { get; set; }
        /*
         计量单位：wms库存保存最小单位
         计量单位=换算比例*n个换算单位
         计量单位 20（件）=1（换算单位：箱）*20（换算比例） 
         */
        /// <summary>
        /// 关联物料单位（字典：sys_dictionary>>MatterUnit 枚举）
        /// </summary>
        public Guid? unit_measurement_guid { get; set; }
        /// <summary>
        /// 计量单位（件/个/米/kg）
        /// </summary>
        [StringLength(30)]
        public string unit_measurement { get; set; }
        /// <summary>
        /// 换算单位（箱/包/卷/桶）
        /// </summary>
        [StringLength(30)]
        public string unit_convert { get; set; }
        /// <summary>
        /// 换算比例
        /// </summary>
        public decimal convert_ratio { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(2000)]
        public string memo { get; set; }

        /// <summary>
        /// 有效日期（天） 从入库算起，超过日期报警！
        /// </summary>
        public int effective_date { get; set; }
        /// <summary>
        /// 库存上限(0不预警)
        /// </summary>
        public int stock_max { get; set; }
        /// <summary>
        /// 库存下限(0不预警)
        /// </summary>
        public int stock_min { get; set; }

        /// <summary>
        /// 参考价
        /// </summary>
        public decimal std_price { get; set; }
        /// <summary>
        /// 参考重量
        /// </summary>
        public decimal std_weight { get; set; }
        /// <summary>
        /// 一个托盘下是否可以混放/混批
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? can_mix { get; set; }

        ///// <summary>
        ///// 可删除（如果从未入库则可以删除，默认创建之后可以删除）
        ///// </summary>
        //[Column(TypeName = "bit")]
        //public bool? can_delete { get; set; }

        /*******物料补齐信息*******/
        /// <summary>
        /// 颜色
        /// </summary>
        [StringLength(50)]
        public string color { get; set; }
        /// <summary>
        /// 物料来源
        /// </summary>
        public MatterFromEnum matter_from { get; set; }
         


        /// <summary>
        /// 总重量
        /// </summary> 
        public decimal? total_weight { get; set; }
        /// <summary>
        /// 总数量
        /// </summary> 
        public decimal? total_number { get; set; }
    }
}
