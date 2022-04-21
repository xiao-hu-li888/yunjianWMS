using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace LTERPWebMVC.App_Start.WebMvCEx
{
    public class ListProvider
    {  //源
        private static Dictionary<string, ListData> listItems = new Dictionary<string, ListData>();
        private static string currName = "LT_HCont_";
        private static object obj = new object();
        private static bool isBegin = false;
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void LoadIoc()
        {
            if (isBegin) return;
            InitializationList list =App_Start.AutofacConfig.GetFromFac<InitializationList>();
            if (list != null)
                list.StaticList();
            isBegin = true;
        }
        /// <summary>
        /// 添加数据至当前请求上下文中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>

        public static void AddList(string name, IEnumerable<ListItem> list)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Remove(currName + name);
                HttpContext.Current.Items.Add(currName + name, list);
            }
        }
        /// <summary>
        /// 添加数据至缓存上下文，所有线程共享存储数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        /// <param name="expireTime">数据过期时间，为null则不过期</param>
        public static void AddList(string name, IEnumerable<ListItem> list, DateTime? expireTime)
        {
            lock (obj)
            {
                listItems.Remove(name);
                listItems.Add(name, new ListData(list, expireTime));
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetList(string name)
        {
            if (!isBegin) { LoadIoc(); }
            //优先上下文
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items.Contains(currName + name))
                    return HttpContext.Current.Items[currName + name] as IEnumerable<ListItem>;
            }
            lock (obj)
            {
                ListData data;
                if (listItems.TryGetValue(name, out data))
                {
                    if (data.ExpireTime != null && data.ExpireTime < DateTime.Now)
                    {
                        listItems.Remove(name);
                    }
                    return data.List;
                }
                return new ListItem[0];
            }
        }
    }
}