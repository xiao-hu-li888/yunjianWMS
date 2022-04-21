using LTWMSEFModel.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel.Warehouse
{
    /// <summary>
    /// 托盘+物料关联表
    /// </summary>
    [Table("wh_traymatter")]
    public class wh_traymatter : BaseEntity
    {
        /// <summary>
        /// 所属仓库 关联表：wh_warehouse
        /// </summary> 
        public Guid? warehouse_guid { get; set; }
        /// <summary>
        /// 托盘guid（表：wh_tray）
        /// </summary> 
        [Required]
        public Guid tray_guid { get; set; }
        /// <summary>
        /// 托盘条码（冗余字段）
        /// </summary>
        [StringLength(100)]
        public string traybarcode { get; set; }
        /***111****/
        /// <summary>
        /// 存储的条码类型：物料编码 包装条码 料箱条码
        /// </summary>
        public BarcodeStoredTypeEnum barcodetype { get; set; }
        /// <summary>
        /// 物料(stk_matter>code)、包装（bill_stockin_print>print_barcode）、料箱条码(wh_tray>traybarcode)
        /// （根据barcodetype区分）(可为空)
        /// </summary>
        [StringLength(100)]
        public string x_barcode { get; set; }
        /****111****/
        ///// <summary>
        /////物料编码（0101010001）（PrintBarcode包装条码>>>冗余）
        ///// </summary>
        //public string matter_barcode { get; set; }

        /// <summary>
        /// 物料名称/货品名称（一个或多个物料的名称  物料1/物料2/物料3 ...）
        /// </summary>
        [StringLength(4000)]
        public string name_list { get; set; }
        /// <summary>
        /// 入库单号
        /// </summary>
        [StringLength(50)]
        public string odd_numbers_in { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        [StringLength(50)]
        public string lot_number { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public decimal number { get; set; }
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

        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime? effective_date { get; set; }

        /// <summary>
        /// 检验状态（合格、待检、退回）
        /// 待检状态锁定出库！！！
        /// </summary>
        public TestStatusEnum test_status { get; set; }


        /*************项目相关*****************/
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
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string memo { get; set; }
    }
    /// <summary>
    /// 条码存储类型
    /// </summary>
    public enum BarcodeStoredTypeEnum
    { 
        /// <summary>
        /// 物料编码（一种物料数量为1 不含批次等其它信息） 一条记录
        /// </summary>
        MatterCode = 0,
        /// <summary>
        /// 包装条码（包含批次 数量等其它信息，包装条码下存在一种、多种物料/产品信息 数量不定） 一条记录
        /// </summary>
        PrintBarcode = 1,
        /// <summary>
        /// 料箱条码(通过反查wh_tray 表再次 关联 wh_traymatter 查询物料信息)
        /// </summary>
        Box = 2
    }
}
