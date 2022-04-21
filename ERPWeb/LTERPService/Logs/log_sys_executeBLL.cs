using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Logs
{
    public class log_sys_executeBLL : LTERPEFModel.ComDao<LTERPEFModel.Logs.log_sys_execute>
    {
        public log_sys_executeBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// 删除半年前的历史数据
        /// </summary>
        /// <returns></returns>
        public bool DelHistInfo()
        { 
            DateTime _d =DateTime.Now.AddDays(-7); 
            MySql.Data.MySqlClient.MySqlParameter[] sqlParameters = {
             new MySql.Data.MySqlClient.MySqlParameter{ParameterName="log_date",Value=_d}
            };
            return dbcontext.Database.ExecuteSqlCommand(
                   "delete from sys_execute_log where log_date < @log_date", sqlParameters) > 0 ? true : false;
        }
        /// <summary>
        /// 添加日志，超过长度500 分多次添加
        /// </summary>
        /// <param name="log"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public LTERPEFModel.SimpleBackValue Add(string log, int diff)
        {
            if (string.IsNullOrWhiteSpace(log))
            {
                return LTERPEFModel.SimpleBackValue.False;
            }
            ////判断日志长度，超过500分多次写入
            int _baselength = 250;
            int _count = log.Length / _baselength;
            if (log.Length % _baselength > 0)
            {
                _count++;
            }
            List<LTERPEFModel.Logs.log_sys_execute> loglist = new List<LTERPEFModel.Logs.log_sys_execute>();
            for (int i = 0; i < _count; i++)
            {
                string _rmk ="";
                if (i == _count - 1)
                {//最后一次循环
                    _rmk= "==>>>" + log.Substring(i * _baselength);
                }
                else
                {
                    _rmk = "==>>>" + log.Substring(i * _baselength, _baselength);
                }
                loglist.Add(new LTERPEFModel.Logs.log_sys_execute()
                {
                    log_date = DateTime.Now,
                    remark = _rmk,
                    diff = diff,
                    guid = Guid.NewGuid()
                });
            }
            return AddRange(loglist);
        }

    }
}
