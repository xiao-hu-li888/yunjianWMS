using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPWebMVC.App_Start.DBSqlServer
{
    /// <summary>
    /// 旧版wms 数据库导入连接（sqlserver）
    /// </summary> 
   // [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    //[DbConfigurationType(typeof(System.Data.Entity.DbConfiguration))]
    public class LTWmsOld_DBContext : DbContext
    {
        public LTWmsOld_DBContext()
           : base("name=LTWMSOldModel")
        { 
            this.Configuration.LazyLoadingEnabled = false;
            ////Database.SetInitializer<LTWmsOld_DBContext>(new CreateDatabaseIfNotExists<LTWmsOld_DBContext>());
            Database.SetInitializer<LTWmsOld_DBContext>(new NullDatabaseInitializer<LTWmsOld_DBContext>());

        }
        //public virtual DbSet<Online> onLine { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
        /// <summary>
        /// 编号生成规则
        /// </summary>
        public virtual DbSet<Matter> Matter { get; set; }
        /// <summary>
        /// 编号生成规则
        /// </summary>
        public virtual DbSet<MatterClass> MatterClass { get; set; }
    }
}
