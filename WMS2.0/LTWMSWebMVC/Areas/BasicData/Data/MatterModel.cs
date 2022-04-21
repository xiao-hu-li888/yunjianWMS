using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class MatterModel:BaseModel
    {
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [Display(Name = "物料编码"),Required(AllowEmptyStrings =false)]
        public string code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称"),Required(AllowEmptyStrings =false)]
        public string name { get; set; }
        /// <summary>
        /// 助记码（拼音首字母）
        /// </summary> 
        public string name_pinyin { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [Display(Name = "规格")]
        public string specs { get; set; }
        /// <summary>
        /// 描述(型号)
        /// </summary>
        [Display(Name = "最小包装单位")]
        public string description { get; set; }
        /// <summary>
        /// 关联品牌（字典：sys_dictionary>>Brand 枚举）
        /// </summary>
        public Guid? brand_guid { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [Display(Name = "品牌名称")]
        public string brand_name { get; set; }
        /// <summary>
        ///物料类型/货品类型-- 关联表：stk_mattertype
        /// </summary>
        public Guid? mattertype_guid { get; set; }
        /// <summary>
        /// 物料类型名称
        /// </summary>
        [Display(Name = "物料类型")]
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
        [Display(Name = "计量单位")]
        public string unit_measurement { get; set; }
        /// <summary>
        /// 换算单位（箱/包/卷/桶）
        /// </summary>
        [Display(Name = "换算单位")]
        public string unit_convert { get; set; }
        /// <summary>
        /// 换算比例
        /// </summary>
        [Display(Name = "换算比例")]
        public decimal convert_ratio { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }

        /// <summary>
        /// 有效日期（天） 从入库算起，超过日期报警！
        /// </summary>
        [Display(Name = "有效日期")]
        public int effective_date { get; set; }
        /// <summary>
        /// 库存上限(0不预警)
        /// </summary>
        [Display(Name = "库存上限")]
        public int stock_max { get; set; }
        /// <summary>
        /// 库存下限(0不预警)
        /// </summary>
        [Display(Name = "库存下限")]
        public int stock_min { get; set; }

        /// <summary>
        /// 参考价
        /// </summary>
        [Display(Name = "参考价")]
        public decimal std_price { get; set; }
        /// <summary>
        /// 参考重量
        /// </summary>
        [Display(Name = "参考重量")]
        public decimal std_weight { get; set; }
        /// <summary>
        /// 一个托盘下是否可以混放/混批
        /// </summary>
        [Display(Name = "是否可以混放")]
        public bool? can_mix { get; set; }

        ///// <summary>
        ///// 可删除（如果从未入库则可以删除，默认创建之后可以删除）
        ///// </summary>
        //[Display(Name = "可删除")]
        //public bool? can_delete { get; set; }
        /*
        /// <summary>
        /// 物料编码/货品编码（为空自动生成）
        /// </summary>
        [Display(Name = "物料编码")]
        public string code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [Display(Name = "物料名称")]
        public string name { get; set; }
        /// <summary>
        /// 助记码（拼音首字母）
        /// </summary> 
        public string name_pinyin { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        [Display(Name = "规格型号")]
        public string specs { get; set; }
        /// <summary>
        ///物料类型/货品类型-- 关联表：stk_mattertype
        /// </summary>
        [Display(Name = "物料类型")]
        public Guid mattertype_guid { get; set; }
        /// <summary>
        /// 物料条码/条形码
        /// </summary>
        [Display(Name = "物料条码")]
        public string barcode { get; set; }
        /// <summary>
        /// 默认仓库 关联表：wh_warehouse
        /// </summary>
        [Display(Name = "默认仓库")]
        public Guid? def_warehouse_guid { get; set; }
        /*
         计量单位：wms库存保存最小单位
         计量单位=换算比例*n个换算单位
         计量单位 20（件）=1（换算单位：箱）*20（换算比例） 
         * /
        /// <summary>
        /// 计量单位（件/个/米/kg）
        /// </summary>
        [Display(Name = "计量单位(件/个)")]
        public string unit_measurement { get; set; }
        /// <summary>
        /// 换算单位（箱/包/卷/桶）
        /// </summary>
        [Display(Name = "换算单位(箱/包)")]
        public string unit_convert { get; set; }
        /// <summary>
        /// 换算比例
        /// </summary>
        [Display(Name = "换算比例")]
        public decimal convert_ratio { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }

        /// <summary>
        /// 有效日期（天） 从入库算起，超过日期报警！
        /// </summary>
        [Display(Name = "有效日期（天）")]
        public int effective_date { get; set; }
        /// <summary>
        /// 库存上限(0不预警)
        /// </summary>
        [Display(Name = "库存上限")]
        public int stock_max { get; set; }
        /// <summary>
        /// 库存下限(0不预警)
        /// </summary>
        [Display(Name = "库存下限")]
        public int stock_min { get; set; }

        /// <summary>
        /// 一个托盘下是否可以混放/混批
        /// </summary>
        [Display(Name = "是否可混放")]
        public bool? can_mix { get; set; }

        /// <summary>
        /// 可删除（如果从未入库则可以删除，默认创建之后可以删除）
        /// </summary> 
        [Display(Name = "是否可删除")]
        public bool can_delete { get; set; }*/
        /// <summary>
        /// 已入库批次数量
        /// </summary>
       [Display(Name ="批次总数")]
        public int lot_number { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        [Display(Name = "总重量")]
        public decimal total_weight { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        [Display(Name = "总数量")]
        public int total_number { get; set; }


        /// <summary>
        /// 合格批次数
        /// </summary>
       [Display(Name = "合格批次数")]
        public int ok_lot_number { get; set; }
        /// <summary>
        /// 合格总数
        /// </summary>
        [Display(Name = "合格总数")]
        public int ok_total_number { get; set; }


        /// <summary>
        /// 待检批次数
        /// </summary>
        [Display(Name = "待检批次数")]
        public int waited_lot_number { get; set; }
        /// <summary>
        /// 待检总数
        /// </summary>
        [Display(Name = "待检总数")]
        public int waited_total_number { get; set; }

        /// <summary>
        /// 不合格批次数
        /// </summary>
        [Display(Name = "不合格批次数")]
        public int bad_lot_number { get; set; }
        /// <summary>
        /// 不合格总数
        /// </summary>
        [Display(Name = "不合格总数")]
        public int bad_total_number { get; set; }

        
        /// <summary>
        /// 临近有效期批次数
        /// </summary>
        [Display(Name = "临近有效期批次数")]
        public int near_lot_number { get; set; }
        /// <summary>
        /// 临近有效期总数
        /// </summary>
        [Display(Name = "临近有效期总数")]
        public int near_total_number { get; set; }

        /// <summary>
        /// 空托盘数量
        /// </summary>
        public int empty_count { get; set; }
        /// <summary>
        /// 托盘数量
        /// </summary>
        public int tray_count { get; set; }
    }
}