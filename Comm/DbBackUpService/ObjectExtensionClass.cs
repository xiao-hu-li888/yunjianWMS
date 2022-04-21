using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbBackUpService
{
    public static class ObjectExtensionClass
    {
        /// <summary>
        /// 利用反射来判断对象是否包含某个属性
        /// </summary>
        /// <param name="instance">object</param>
        /// <param name="propertyname">需要判断的属性</param>
        /// <returns>是否包含</returns>
        public static bool ContainProperty(this object instance, string propertyname)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyname))
            {
                PropertyInfo _findedPropertyInfo = instance.GetType().GetProperty(propertyname);
                return (_findedPropertyInfo != null);
            }
            return false;
        }

        /// <summary>
        /// 获取对象属性值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object instance, string propertyname)
        {
            if (instance != null && !string.IsNullOrEmpty(propertyname))
            {
                PropertyInfo property = instance.GetType().GetProperty(propertyname);
                if (property == null)
                {
                    return null;
                }
                return property.GetValue(instance, null);
            }
            return null;
        } 
    }
}
