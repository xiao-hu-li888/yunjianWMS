using LTWMSEFModel.Basic;
using LTWMSEFModel.Bills;
using LTWMSEFModel.BillsAihua;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Logs;
using LTWMSEFModel.Stock;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSEFModel
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class LTModel : DbContext
    {
        public LTModel()
            : base("name=LTWMSModel")
        {
            base.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<LTModel>(new CreateDatabaseIfNotExists<LTModel>());
            //Database.SetInitializer<LTModel>(new DropCreateDatabaseAlways<LTModel>());  
            //不初始化数据库
            // Database.SetInitializer<LTModel>(new NullDatabaseInitializer<LTModel>()); 
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //配置日志-日期支持毫秒
            modelBuilder.Entity<log_sys_execute>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);
            modelBuilder.Entity<log_sys_alarm>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);
            modelBuilder.Entity<log_sys_useroperation>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);

            modelBuilder.Entity<log_hdw_plc>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);

            modelBuilder.Entity<log_stk_matter_shelfunits>().Property(InstructionsCxl => InstructionsCxl.date_time).HasPrecision(6);
            modelBuilder.Entity<log_stk_stock_account>().Property(InstructionsCxl => InstructionsCxl.account_date).HasPrecision(6);
            modelBuilder.Entity<log_wh_traymatter>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);
            modelBuilder.Entity<log_wh_wcs>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6);

            //设置创建日期/修改日期支持毫秒

            modelBuilder.Entity<bill_stockcheck>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<bill_stockcheck>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<bill_stockin>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<bill_stockin>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<bill_stockin_print>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<bill_stockin_print>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<bill_stockout>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<bill_stockout>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<bill_stockout_task>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<bill_stockout_task>().Property(prop => prop.updatedate).HasPrecision(6);

            modelBuilder.Entity<hdw_plc>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_plc>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue_his>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue_his>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<stk_matter>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<stk_matter>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<stk_mattertype>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<stk_mattertype>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<stk_stock>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<stk_stock>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<sys_annex>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<sys_annex>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<sys_login>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<sys_login>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<sys_number_rule>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<sys_number_rule>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<sys_role>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<sys_role>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<sys_table_id>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<sys_table_id>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_shelfunits>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_shelfunits>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_shelves>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_shelves>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_tray>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_tray>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_traymatter>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_traymatter>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_warehouse>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_warehouse>().Property(prop => prop.updatedate).HasPrecision(6);
            modelBuilder.Entity<wh_service_status>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<wh_service_status>().Property(prop => prop.updatedate).HasPrecision(6);



            modelBuilder.Entity<bill_stockin>().Property(prop => prop.in_date).HasPrecision(6);
            modelBuilder.Entity<bill_stockout>().Property(prop => prop.out_date).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue>().Property(prop => prop.startup).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue>().Property(prop => prop.endup).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue_his>().Property(prop => prop.startup).HasPrecision(6);
            modelBuilder.Entity<hdw_stacker_taskqueue_his>().Property(prop => prop.endup).HasPrecision(6);


            modelBuilder.Entity<hdw_message_received>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_message_received_his>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_message_waitedsend>().Property(prop => prop.createdate).HasPrecision(6);
            modelBuilder.Entity<hdw_message_waitedsend_his>().Property(prop => prop.createdate).HasPrecision(6);

        }


        /// <summary>
        /// 编号生成规则
        /// </summary>
        public virtual DbSet<Basic.sys_number_rule> sys_number_rule { get; set; }
        /// <summary>
        /// 系统执行日志，只保留最近3-7天的日志？？
        /// </summary>
        public virtual DbSet<log_sys_execute> log_sys_execute { get; set; }
        /// <summary>
        /// 系统报警日志（包括：异常、硬件等运行警告日志）
        /// </summary>
        public virtual DbSet<log_sys_alarm> log_sys_alarm { get; set; }
        /// <summary>
        /// 系统附件表（保存所有图片/文档/附件）
        /// </summary>
        public virtual DbSet<Basic.sys_annex> sys_annex { get; set; }
        /// <summary>
        ///  IIS/桌面程序 与windows服务之间的控制与数据交互
        /// </summary>
        public virtual DbSet<Basic.sys_control_dic> sys_control_dic { get; set; }
        /// <summary>
        /// 系统字典表
        /// </summary>
        public virtual DbSet<Basic.sys_dictionary> sys_dictionary { get; set; }
        /// <summary>
        /// 登录表
        /// </summary>
        public virtual DbSet<Basic.sys_login> sys_login { get; set; }
        /// <summary>
        /// 登录与角色关联表
        /// </summary>
        public virtual DbSet<Basic.sys_loginrole> sys_loginrole { get; set; }
        /// <summary>
        /// 角色表
        /// </summary>
        public virtual DbSet<Basic.sys_role> sys_role { get; set; }
        /// <summary>
        /// 用户操作日志
        /// </summary>
        public virtual DbSet<log_sys_useroperation> log_sys_useroperation { get; set; }

        /// <summary>
        /// 库存盘点表
        /// </summary>
        public virtual DbSet<Bills.bill_stockcheck> bill_stockcheck { get; set; }
        /// <summary>
        /// 库存盘点详细表
        /// </summary>
        public virtual DbSet<Bills.bill_stockcheck_detail> bill_stockcheck_detail { get; set; }
        /// <summary>
        /// 入库单据(收货)
        /// </summary>
        public virtual DbSet<Bills.bill_stockin> bill_stockin { get; set; }
        /// <summary>
        /// 入库单据详细表
        /// </summary>
        public virtual DbSet<Bills.bill_stockin_detail> bill_stockin_detail { get; set; }
        /// <summary>
        /// 出库单据（出货）
        /// </summary>
        public virtual DbSet<Bills.bill_stockout> bill_stockout { get; set; }
        /// <summary>
        /// 出库单据详细
        /// </summary>
        public virtual DbSet<Bills.bill_stockout_detail> bill_stockout_detail { get; set; }

        /// <summary>
        /// plc状态信息（堆垛机/输送线）
        /// </summary>
        public virtual DbSet<Hardware.hdw_plc> hdw_plc { get; set; }
        /// <summary>
        /// plc 运行日志
        /// </summary>
        public virtual DbSet<log_hdw_plc> log_hdw_plc { get; set; }
        /// <summary>
        /// 任务队列（上架/下架/移库）
        /// </summary>
        public virtual DbSet<Hardware.hdw_stacker_taskqueue> hdw_stacker_taskqueue { get; set; }
        /// <summary>
        /// 任务历史记录表
        /// </summary>
        public virtual DbSet<Hardware.hdw_stacker_taskqueue_his> hdw_stacker_taskqueue_his { get; set; }
        ///// <summary>
        ///// 出入库单据和出入库历史记录关联表
        ///// </summary>
        //public virtual DbSet<Hardware.hdw_task_bill_inout> hdw_task_bill_inout { get; set; }

        /// <summary>
        /// 物料表/货品表
        /// </summary>
        public virtual DbSet<Stock.stk_matter> stk_matter { get; set; }
        /// <summary>
        /// 物料分类
        /// </summary>
        public virtual DbSet<Stock.stk_mattertype> stk_mattertype { get; set; }
        /// <summary>
        /// 实时库存总表
        /// </summary>
        public virtual DbSet<Stock.stk_stock> stk_stock { get; set; }
        /// <summary>
        /// 库存流水账（入库/出库/盘盈/盘亏/报损）
        /// </summary>
        public virtual DbSet<log_stk_stock_account> log_stk_stock_account { get; set; }
        /// <summary>
        /// 物料上下架历史记录表
        /// </summary>
        public virtual DbSet<log_stk_matter_shelfunits> log_stk_matter_shelfunits { get; set; }
        /// <summary>
        /// 货架对应仓位详细表
        /// </summary>
        public virtual DbSet<Warehouse.wh_shelfunits> wh_shelfunits { get; set; }
        /// <summary>
        /// 货架规格主表
        /// </summary>
        public virtual DbSet<Warehouse.wh_shelves> wh_shelves { get; set; }
        /// <summary>
        /// 托盘表
        /// </summary>
        public virtual DbSet<Warehouse.wh_tray> wh_tray { get; set; }
        /// <summary>
        /// 托盘+物料关联表
        /// </summary>

        public virtual DbSet<Warehouse.wh_traymatter> wh_traymatter { get; set; }
        /// <summary>
        /// 托盘物料绑定/解绑记录表
        /// </summary>
        public virtual DbSet<log_wh_traymatter> log_wh_traymatter { get; set; }
        /// <summary>
        /// 仓库表
        /// </summary>
        public virtual DbSet<Warehouse.wh_warehouse> wh_warehouse { get; set; }
        /// <summary>
        /// wcs调度系统配置表
        /// </summary>
        public virtual DbSet<wh_service_status> wh_service_status { get; set; }
        /// <summary>
        /// wcs连接日志记录表
        /// </summary>
        public virtual DbSet<log_wh_wcs> log_wh_wcs { get; set; }
        /// <summary>
        /// 入库单据条码打印管理（入库单对应的物料可能需要拆分放入多个托盘，需要打印多张条码及对应数量批次号关联）
        /// </summary>
        public virtual DbSet<Bills.bill_stockin_print> bill_stockin_print { get; set; }
        /// <summary>
        /// 出库单据详细表（关联任务表，一个物料可能对应多个出库任务）
        /// bill_stockout （主表）==> bill_stockout_detail(子表) ==>bill_stockout_task(详细表)
        /// </summary>
        public virtual DbSet<Bills.bill_stockout_task> bill_stockout_task { get; set; }

        /// <summary>
        /// 管理表自增id(支持并发)
        /// </summary>
        public virtual DbSet<sys_table_id> sys_table_id { get; set; }
        /// <summary>
        /// 仓库分类（分区域，支持父子多级）
        /// </summary>
        public virtual DbSet<wh_warehouse_type> wh_warehouse_type { get; set; }
        /// <summary>
        /// 货架关联wcs处理服务
        /// </summary>
        public virtual DbSet<wh_wcs_srv> wh_wcs_srv { get; set; }
        /// <summary>
        /// 站台配置表
        /// </summary>
        public virtual DbSet<wh_wcs_device> wh_wcs_device { get; set; }
        /// <summary>
        /// 货架和站台、堆垛机关联表
        /// </summary>
        public virtual DbSet<wh_shelves_dev> wh_shelves_dev { get; set; }
        ///// <summary>
        ///// 包装条码详细物料明细
        ///// </summary>
        //public virtual DbSet<bill_stockin_print_detail> bill_stockin_print_detail { get; set; }
        /// <summary>
        /// 预留单
        /// </summary>
        public virtual DbSet<billah_reserved_order> billah_reserved_order { get; set; }
        /// <summary>
        /// 预留单明细
        /// </summary>
        public virtual DbSet<billah_reserved_order_detail> billah_reserved_order_detail { get; set; }
        /// <summary>
        ///  出入库流水
        /// </summary>
        public virtual DbSet<stk_inout_recod> stk_inout_recod { get; set; }
        /// <summary>
        ///  出入库流水--历史表
        /// </summary>
        public virtual DbSet<stk_inout_recod_his> stk_inout_recod_his { get; set; }

        /// <summary>
        /// 库位分区存放（特殊要求，一般库位不分区）
        /// </summary>
        public virtual DbSet<wh_shelfunits_area> wh_shelfunits_area { get; set; }
        /// <summary>
        /// 分区下关联存储的物料信息
        /// </summary>
        public virtual DbSet<wh_shelfunits_area_matters> wh_shelfunits_area_matters { get; set; }

        /// <summary>
        /// 接收到的消息表（WCS、ACS、ERP、Mes等接口对接消息）
        /// </summary>
        public virtual DbSet<hdw_message_received> hdw_message_received { get; set; }
        /// <summary>
        ///  接收到的消息表（WCS、ACS、ERP、Mes等接口对接消息）--历史表
        /// </summary>
        public virtual DbSet<hdw_message_received_his> hdw_message_received_his { get; set; }
        /// <summary>
        /// 待发送消息表（WCS、ACS、ERP、Mes等接口对接消息）
        /// </summary>
        public virtual DbSet<hdw_message_waitedsend> hdw_message_waitedsend { get; set; }
        /// <summary>
        /// 待发送消息表（WCS、ACS、ERP、Mes等接口对接消息）--历史表
        /// </summary>
        public virtual DbSet<hdw_message_waitedsend_his> hdw_message_waitedsend_his { get; set; }
        /// <summary>
        /// 入库指定托盘或绑定的托盘
        /// </summary>
        public virtual DbSet<bill_stockin_detail_traymatter> bill_stockin_detail_traymatter { get; set; }
        /// <summary>
        /// 出库指定的托盘或系统自动分配的托盘
        /// </summary>
        public virtual DbSet<bill_stockout_detail_traymatter> bill_stockout_detail_traymatter { get; set; }
        /// <summary>
        /// 单据（入库、出库、盘点。。。）与托盘、任务的关联关系表
        /// 用于在托盘入库、出库流转、盘点回库等，将对应的信息进行关联绑定
        /// </summary>
        public virtual DbSet<bill_task_tray_relation> bill_task_tray_relation { get; set; }
    }
}
