using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_table_idBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_table_id>
    {
        public sys_table_idBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
        /// <summary>
        ///递归次数（超过3次直接抛异常）
        /// </summary>
        int count = 0;
        /// <summary>
        /// 获取表对应的id（集装箱出库任务，一次性需要大量的id）
        /// </summary>
        /// <param name="table">对应的表</param>
        /// <param name="take">一次取多少个id</param>
        /// <returns></returns>
        public int GetId(TableIdType table, int take)
        {
            if (count >= 3)
            {
                count = 0;
                // throw new Exception("获取ID失败");
                throw new Exception("获取ID失败 table:" + table + ",take:" + take);
            }
            count++;
            int _rtv = 0;
            string _table = Enum.GetName(typeof(TableIdType), table);
            var _obj = GetFirstDefault(w => w.table == _table);
            if (_obj != null)
            {
                _rtv = _obj.max_id+1;//当前获取的最小值
                _obj.max_id = _obj.max_id + take;//当前获取的最大值
                
                LTWMSEFModel.SimpleBackValue bv = Update(_obj);
                if (bv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                {//如果出现并发异常，重新获取数据，重新修改一次
                    return GetId(table, take);
                }
                else if (bv == LTWMSEFModel.SimpleBackValue.True)
                {
                    count = 0;
                    return _rtv;
                } 
            }
            else
            {
                int _initv = 0; 
                if (table == TableIdType.hdw_stacker_taskqueue)
                {//堆垛车任务列表
                    _initv = 999; 
                }
                _rtv = _initv + 1;
                int _maxv = _initv + take;
                LTWMSEFModel.SimpleBackValue bv = AddIfNotExists(new LTWMSEFModel.Basic.sys_table_id()
                {
                    guid = Guid.NewGuid(),
                    createdate = DateTime.Now,
                    init_val = _rtv,
                    max_id = _maxv,
                    createuser = "sys",
                    table = _table,
                    state = LTWMSEFModel.EntityStatus.Normal,
                    is_reset = false,
                    reset_maxval = int.MaxValue
                }, a => a.table);
                if (bv == LTWMSEFModel.SimpleBackValue.True)
                {//初始化数据返回1
                    count = 0;
                    return _rtv;
                }
            }
            count = 0;
            return 0;
        }
        /// <summary>
        /// 获取表对应的id
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int GetId(TableIdType table)
        {
            return GetId(table, 1);
        }
        /// <summary>
        /// 需要自增id的表名
        /// </summary>
        public enum TableIdType
        {
            /// <summary>
            /// 堆垛车任务列表
            /// </summary>
            hdw_stacker_taskqueue,
            /// <summary>
            /// agv任务列表
            /// </summary>
            hdw_agv_taskqueue
        }
    }
}
