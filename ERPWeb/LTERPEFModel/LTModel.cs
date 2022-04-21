using LTERPEFModel.Basic;
using LTERPEFModel.Contact;
using LTERPEFModel.Employee;
using LTERPEFModel.Logs;
using LTERPEFModel.Project;
using LTERPEFModel.Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPEFModel
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class LTModel : DbContext
    {
        public LTModel()
            : base("name=LTERPModel")
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
            modelBuilder.Entity<log_sys_useroperation>().Property(InstructionsCxl => InstructionsCxl.log_date).HasPrecision(6); 
  
            //设置创建日期/修改日期支持毫秒 
           
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
        /// 管理表自增id(支持并发)
        /// </summary>
        public virtual DbSet<sys_table_id> sys_table_id { get; set; }


        public virtual DbSet<wh_warehouse> wh_warehouse { get; set; }
        public virtual DbSet<pro_project> pro_project { get; set; }
        public virtual DbSet<pro_project_deliver> pro_project_deliver { get; set; }
        public virtual DbSet<pro_project_details> pro_project_details { get; set; }
        public virtual DbSet<pro_project_matter> pro_project_matter { get; set; }
        public virtual DbSet<pro_promatterdesign_detail> pro_promatterdesign_detail { get; set; }

        public virtual DbSet<emp_organization> emp_organization { get; set; }
        public virtual DbSet<emp_employeeInfo> emp_employeeInfo { get; set; }
        public virtual DbSet<emp_employee_organization> emp_employee_organization { get; set; }

        public virtual DbSet<con_customer> con_customer { get; set; }
        public virtual DbSet<con_customer_contact> con_customer_contact { get; set; }
        public virtual DbSet<con_supplier> con_supplier { get; set; }
        public virtual DbSet<con_supplier_contact> con_supplier_contact { get; set; }
    }
}
