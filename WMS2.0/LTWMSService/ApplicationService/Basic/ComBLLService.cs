using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.Basic
{
    /// <summary>
    /// 通用类库服务
    /// </summary>
    public class ComBLLService
    {
        /// <summary>
        /// 获取数据库对象模型guid列表
        /// </summary>
        /// <param name="basebaseList"></param>
        /// <returns></returns>
        public static List<Guid> GetBaseBaseGuidList(object basebaseObject)
        {
            //var basebaseList = basebaseObject as List<Object>;
            IEnumerable<object> list = basebaseObject as IEnumerable<object>;
            List<Guid> _bbGuid = new List<Guid>();
            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var _BaseBaseObj = item as LTWMSEFModel.BaseBaseEntity;
                    if (_BaseBaseObj != null)
                    {
                        _bbGuid.Add(_BaseBaseObj.guid);
                    }
                }
                //_bbGuid = basebaseList.Select(s => s.guid).ToList();
            }
            return _bbGuid;
        }
    }
}
