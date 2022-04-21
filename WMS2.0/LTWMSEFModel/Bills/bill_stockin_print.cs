using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LTWMSEFModel.Bills
{
    /// <summary>
    /// 入库单据条码打印管理（入库单对应的物料可能需要拆分放入多个托盘，需要打印多张条码及对应数量批次号关联）
    /// 包装袋条码（包装条码树结构）
    /// </summary>
    [Table("bill_stockin_print")]
    public class bill_stockin_print : BaseEntity
    {
        /// <summary>
        /// 仓库表 （关联表：wh_warehouse）
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 入库单据guid (关联表：bill_stockin)  如果为空：则不关联入库单，说明是从erp过来的包装条码数据？
        /// </summary> 
        public Guid? stockin_guid { get; set; }

        /// <summary>
        /// 包装条码类型(单品包装、箱包装)
        /// </summary>
        public PrintBarcodeTypeEnum barcode_type { get; set; }
        /// <summary>
        /// 打印条码(P1001自动生成且唯一) 箱包装条码
        /// </summary>
        [StringLength(50)]
        public string print_barcode { get; set; }
        /// <summary>
        /// 包装树-父节点guid（可为空）
        /// </summary>
        public Guid? parent_guid { get; set; }
        /**********包装详细***********/
        /// <summary>
        /// 物料guid（关联表:stk_matter）
        /// </summary> 
        public Guid? matter_guid { get; set; }
        /// <summary>
        /// 物料编码/货品编码（为空自动生成） 分类+（4位编号）编号 010101+0001  条形码
        /// </summary>
        [StringLength(50)]
        public string matter_code { get; set; }
        /// <summary>
        /// 物料名称/货品名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }
        /// <summary>
        /// 批次号（入库批次号>>关联入库单号等信息）
        /// </summary> 
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }


        /// <summary>
        /// 单个重量（单位g）
        /// </summary>
        public decimal single_weight { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal total_weight { get; set; }

        /// <summary>
        /// 单个长度
        /// </summary>
        public decimal single_length { get; set; }
        /// <summary>
        /// 总长度
        /// </summary>
        public decimal total_length { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? producedate { get; set; }
        ///// <summary>
        ///// 已出库数量
        ///// </summary>
        //public int out_number { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
        /// <summary>
        /// 包装袋是否可拆零（true 条码拆零可重复利用,false/null拆包作废，重新入库需重打条码）
        /// </summary>
        [Column(TypeName = "bit")]
        public bool? is_unpack { get; set; }

        /// <summary>
        /// 项目编号（关联表：_project）[直接从erp获取进行中的项目列表？] [{prj_no:'项目编号',prj_name:'项目名称',cust_name:'客户名称',...},...]
        /// </summary>
        [StringLength(50)]
        public string project_no { get; set; }
        /// <summary>
        /// 关联项目
        /// </summary>
        [StringLength(50)]
        public string project_name { get; set; }
        /// <summary>
        /// 关联项目对应的客户
        /// </summary>
        [StringLength(50)]
        public string customer_name { get; set; }

    }
    /// <summary>
    /// 包装条码类型(单品、多品)
    /// </summary>
    public enum PrintBarcodeTypeEnum
    {
        /// <summary>
        /// 单品包装(单品如果parent_guid不为空，单品条码可不打印，只打印主包装条码就可以)
        /// </summary>
        [Description("单品包装")]
        Single = 0,
        /// <summary>
        /// 箱包装（包含单品包装）
        /// </summary>
        [Description("箱包装")]
        Multiple = 1
    }
}
