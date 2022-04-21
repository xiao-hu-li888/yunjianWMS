using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService
{
    public class BaseService
    {
        //执行过程记录至数据库
        public delegate void DelegateExecuteLog(string logs,int randDiff);
        /// <summary>
        /// 记录执行过程事件（保存至数据库）
        /// </summary>
        public event DelegateExecuteLog OnDbExecuteLog;
        //保存日志至文件
        public delegate void DelegateLogV(string logs);
        /// <summary>
        /// 记录日志事件（保存至文件）
        /// </summary>
        public event DelegateLogV OnLogV;
        /*********************/
        protected LTWMSEFModel.LTModel dbcontext;
        public BaseService(LTWMSEFModel.LTModel dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        /// <summary>
        /// 执行过程日志（存数据库）
        /// </summary>
        /// <param name="logs"></param>
        protected void OnExecuteLog(string logs,int randDiff)
        {
            if (OnDbExecuteLog != null)
            {
                OnDbExecuteLog(logs, randDiff);
            }
        }
        /// <summary>
        /// 普通的日志（存txt文件）
        /// </summary>
        /// <param name="logs"></param>
        protected void OnLog(string logs)
        {
            if (OnLogV != null)
            {
                OnLogV(logs);
            }
        }
        public DbContextTransaction BeginTran()
        {
            return dbcontext.Database.BeginTransaction();
        }
        /// <summary>
        /// 返回json对象值
        /// </summary>
        /// <param name="success"></param>
        /// <param name="result"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ComServiceReturn JsonReturn(bool success, string result = null, object data = null)
        {
            return new ComServiceReturn() { success = success, result = result, data = data };
        }
    }
}
