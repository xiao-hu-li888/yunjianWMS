using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel.Stock
{
    /// <summary>
    /// 物料表/货品表(包括采购物料、设计成品等都属于物料)
    /// </summary>
    [Table("stk_matter")]
    public class stk_matter : BaseEntity
    {
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [StringLength(50)]
        public string code { get; set; }
        ///// <summary>
        ///// 编码（01-01-01） -分割符
        ///// </summary>
        //[StringLength(50)]
        //public string code_full { get; set; }
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
        /// 描述(型号)
        /// </summary>
       [StringLength(255)]
        public string description { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public decimal length { get; set; }
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
        public bool? can_mix{get;set;}

        /// <summary>
        /// 可删除（如果从未入库则可以删除，默认创建之后可以删除）
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? can_delete { get; set; }

        /*后期补上
          //基本属性
        /// <summary>
        /// 图号（设计图号,图号应该也是唯一）
        /// </summary>
        [StringLength(50)]
        public string drawing_no { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        [StringLength(50)]
        public string color { get; set; }
        //工艺属性
        /// <summary>
        /// 物料来源
        /// </summary>
        public MatterFromEnum matter_from { get; set; }
        //采购属性
        /// <summary>
        /// 采购批量（采购批量指的是每次采购的数量）
        /// </summary>
        public int purchase_quantity { get; set; }
        /// <summary>
        /// 主供应商
        /// </summary>
        [StringLength(50)]
        public string supplier_main { get; set; }
        //其它属性
        /// <summary>
        /// 成熟度（专用件、企标准件、行业标准件）
        /// </summary>
        public string maturity { get;set;}

         
         */
    }
    /// <summary>
    /// 物料来源
    /// </summary>
    public enum MatterFromEnum
    {
        /// <summary>
        /// 采购件
        /// </summary>
        [Description("采购件")]
        Purchase = 0,
        /// <summary>
        /// 自制件（如果是临时急需用也可采购，只在erp做记录，不改变物料的原属性，否则需要更改）
        /// </summary>
        [Description("自制件")]
        SelfProduced = 1
    }
}
