using DbBackUpService.bak;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBackUpService
{
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class LTBackUpModel : DbContext
    {
        public LTBackUpModel()
            : base("name=LTWMSBackUpModel")
        {
            base.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<LTBackUpModel>(new CreateDatabaseIfNotExists<LTBackUpModel>());
            //Database.SetInitializer<LTModel>(new DropCreateDatabaseAlways<LTModel>());  
            //不初始化数据库
            // Database.SetInitializer<LTModel>(new NullDatabaseInitializer<LTModel>()); 
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /* 
             * mysql 配置 日期支持毫秒级别
             *
            modelBuilder.Entity<bak_data>().Property(prop => prop.createdate).HasPrecision(6); 
            modelBuilder.Entity<bak_data>().Property(prop => prop.updatedate).HasPrecision(6);
             */
        }

        /// <summary>
        /// 编号生成规则
        /// </summary>
        public virtual DbSet<bak_data> bak_data { get; set; }

    }
}
