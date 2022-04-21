using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Logs
{
    public class log_sys_useroperationBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Logs.log_sys_useroperation>
    {
        public log_sys_useroperationBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }

        /// <summary>
        /// 添加日志，超过长度500 分多次添加
        /// </summary>
        /// <param name="log"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue Add(string log, string createuser, int diff, Guid? warehouseguid = null)
        {
            if (string.IsNullOrWhiteSpace(log))
            {
                return LTWMSEFModel.SimpleBackValue.False;
            }
            ////判断日志长度，超过500分多次写入
            int _baselength = 250;
            int _count = log.Length / _baselength;
            if (log.Length % _baselength > 0)
            {
                _count++;
            }
            List<LTWMSEFModel.Logs.log_sys_useroperation> loglist = new List<LTWMSEFModel.Logs.log_sys_useroperation>();
            for (int i = 0; i < _count; i++)
            {
                string _rmk = "";
                if (i == _count - 1)
                {//最后一次循环
                    //_rmk = "==>>>" + log.Substring(i * _baselength);
                    _rmk = log.Substring(i * _baselength);
                }
                else
                {
                    //_rmk = "==>>>" + log.Substring(i * _baselength, _baselength);
                    _rmk = log.Substring(i * _baselength, _baselength);
                }
                loglist.Add(new
                    LTWMSEFModel.Logs.log_sys_useroperation()
                {
                    warehouse_guid = warehouseguid,
                    log_date = DateTime.Now,
                    remark = _rmk,
                    operator_u = createuser,
                    //  diff = diff,
                    guid = Guid.NewGuid()
                });
            }
            return AddRange(loglist);
        }
    }
}
