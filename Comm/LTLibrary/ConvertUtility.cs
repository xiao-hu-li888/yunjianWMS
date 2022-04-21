using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LTLibrary
{
    public class ConvertUtility
    {

        /// <summary>
        /// 判断是否包含字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsABC(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            return Regex.Matches(str, "[a-zA-Z]").Count > 0;
        }
        /// <summary>
        /// 判断是否包含数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ContainsNumber (string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            return Regex.Matches(str, "[0-9]").Count > 0;
        }
        /// <summary>
        /// 获取文件的后缀名，带点号
        /// 返回小写带点的后缀名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileSuffix(string filename)
        { 
            if (string.IsNullOrWhiteSpace(filename))
            {
                return "";
            }
            string _suffix = filename.Substring(filename.LastIndexOf("."));
            return _suffix.ToLower();
        }
        /// <summary>
        /// 对象或List对象转成Table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回单行或多行数据集</returns>
        public static DataTable ObjectToTable(object obj)
        {
            try
            {
                Type t;
                if (obj.GetType().IsGenericType)
                {
                    t = obj.GetType().GetGenericTypeDefinition();
                }
                else
                {
                    t = obj.GetType();
                }
                if (t == typeof(List<>) ||
                    t == typeof(IEnumerable<>))
                {
                    DataTable dt = new DataTable();
                    IEnumerable<object> lstenum = obj as IEnumerable<object>;
                    if (lstenum.Count() > 0)
                    {
                        var ob1 = lstenum.GetEnumerator();
                        ob1.MoveNext();
                        foreach (var item in ob1.Current.GetType().GetProperties())
                        {
                            dt.Columns.Add(new DataColumn() { ColumnName = item.Name });
                        }
                        //数据
                        foreach (var item in lstenum)
                        {
                            DataRow row = dt.NewRow();
                            foreach (var sub in item.GetType().GetProperties())
                            {
                                row[sub.Name] = sub.GetValue(item, null);
                            }
                            dt.Rows.Add(row);
                        }
                        return dt;
                    }
                }
                else if (t == typeof(DataTable))
                {
                    return (DataTable)obj;
                }
                else   //(t==typeof(Object))
                {
                    DataTable dt = new DataTable();
                    foreach (var item in obj.GetType().GetProperties())
                    {
                        dt.Columns.Add(new DataColumn() { ColumnName = item.Name });
                    }
                    DataRow row = dt.NewRow();
                    foreach (var item in obj.GetType().GetProperties())
                    {
                        row[item.Name] = item.GetValue(obj, null);
                    }
                    dt.Rows.Add(row);
                    return dt;
                }

            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// Table表转List列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> TableToList<T>(DataTable table)
        {
            List<T> lstData = new List<T>();
            if (table != null && table.Rows.Count > 0)
            {
                //数据
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var _row = table.Rows[i];
                    T obj = Activator.CreateInstance<T>();
                    //foreach (DataColumn column in _row.Table.Columns)
                    //{ 
                    for (int j = 0; j < _row.Table.Columns.Count; j++)
                    {
                        try
                        {
                            DataColumn column = _row.Table.Columns[j];
                            PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                            if (prop != null)
                            {
                                object value = _row[column.ColumnName];
                                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    var valWithRealType = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                                    prop.SetValue(obj, valWithRealType, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw;    
                        }
                    }
                    lstData.Add(obj);
                }
            }
            return lstData;
        }
        /// <summary>
        /// 返回对应字符宽度的数字，不足用0补齐
        /// </summary>
        /// <param name="num"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string getNumberWidthString(int num, int width)
        {
            if (width <= 0)
            {
                return num.ToString();
            }
            int _len = num.ToString().Length;//字符串的长度 
            if (width <= _len)
            {//宽度小于或等于字符串的长度
                return num.ToString();
            }
            //补齐字符串
            int _sps_len = (width - _len);//空格的长度
            string _space = "";
            for (int i = 0; i < _sps_len; i++)
            {
                _space += "0";
            }
            return _space + num.ToString();
        }
        /// <summary>
        /// 转换成Guid值列表
        /// </summary>
        /// <param name="guidstrs">guid1,guid2,guid3,guid4</param>
        /// <returns></returns>
        public static List<Guid> ParseToGuids(string guidstrs)
        {
            if (string.IsNullOrWhiteSpace(guidstrs))
            {
                return null;
            }
            List<Guid> listGuid = new List<Guid>();
            string[] arr = guidstrs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arr != null && arr.Length > 0)
            {
                foreach (string item in arr)
                {
                    try
                    {
                        listGuid.Add(Guid.Parse(item));
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return listGuid;
        }
        /// <summary>
        /// 转换成Guid值列表
        /// </summary>
        /// <param name="guidstrs">guid1,guid2,guid3,guid4</param>
        /// <returns></returns>
        public static List<string> ParseToList(string guidstrs)
        {
            if (string.IsNullOrWhiteSpace(guidstrs))
            {
                return null;
            }
            List<string> listGuid = new List<string>();
            string[] arr = guidstrs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arr != null && arr.Length > 0)
            {
                foreach (string item in arr)
                {
                    try
                    {
                        listGuid.Add(item);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return listGuid;
        }
        /// <summary>
        /// 总秒数转天:小时:分钟:秒
        /// </summary>
        /// <param name="seconds">总秒数</param>
        /// <param name="format">0=>时：分：秒 1=>1:1:1</param>
        /// <returns></returns>
        public static string SecondToDate(int seconds, int format = 0)
        {
            int hour = seconds / 3600;
            int modSec = seconds % 3600;
            int min = modSec / 60;
            int sec = modSec % 60;
            if (format == 0)
            {
                return (hour > 0 ? (hour + "时:") : "") + ((hour > 0 || min > 0) ? (min + "分:") : "") + sec + "秒";
            }
            return (hour > 0 ? (hour + ":") : "") + ((hour > 0 || min > 0) ? (min + ":") : "") + sec;
        }

        /// <summary>
        /// 相差天数
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static long DiffDays(DateTime? time1, DateTime? time2)
        {
            if (time1 == null || time2 == null)
            {
                return 0;
            } 
            return (time2.Value - time1.Value).Days;    //min=90
        }

        /// <summary>
        /// 相差秒数
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static long DiffSeconds(DateTime? time1, DateTime? time2)
        {
            if(time1==null||time2==null)
            {
                return 0;
            }
            return (time2.Value.Ticks - time1.Value.Ticks) / 10000000;    //min=90
        }
        /// <summary>
        /// 相差毫秒数
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static long DiffMilliseconds(DateTime time1, DateTime time2)
        {
            //return (time2 - time1).Milliseconds;
            return (time2.Ticks - time1.Ticks) / 10000;
        }
        public static decimal ToDecimal(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                return Convert.ToDecimal(obj.ToString().Trim());
            }
            catch { }
            return 0;
        }
        public static decimal ToDecimal(object obj, decimal defaultvalue)
        {
            if (obj == null)
                return defaultvalue;
            try
            {
                return Convert.ToDecimal(obj.ToString().Trim());
            }
            catch { }
            return defaultvalue;
        }
        /// <summary>
        /// 转换为int类型,转换失败默认返回0
        /// </summary>
        /// <param name="obj">转换对象</param> 
        /// <returns></returns>
        public static int ToInt(object obj)
        {
            if (obj == null)
                return 0;
            try
            {
                return Convert.ToInt32(obj.ToString().Trim());
            }
            catch
            {

            }
            return 0;
        }
        /// <summary>
        /// 转换Int，失败返回defaultvalue
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="defaultvalue">返回设置的默认值</param>
        /// <returns></returns>
        public static int ToInt(object obj, int defaultvalue)
        {
            if (obj == null)
                return defaultvalue;
            try
            {
                return Convert.ToInt32(obj.ToString().Trim());
            }
            catch
            {

            }
            return defaultvalue;
        }
        public static DateTime? ToDateTime(object obj)
        {
            try
            {
                return Convert.ToDateTime(obj.ToString().Trim());
            }
            catch
            {

            }
            return null;
        }
        public static string ToString(object obj)
        {
            if (obj == null)
                return string.Empty;
            try
            {
                return Convert.ToString(obj);
            }
            catch { }
            return string.Empty;
        }
        public static string ToDateString(DateTime? obj)
        {
            if (obj == null)
                return string.Empty;
            try
            {
                return obj.Value.ToString("yyyy/MM/dd");
            }
            catch { }
            return string.Empty;
        }
        public static string ToDateTimeString(DateTime? obj)
        {
            if (obj == null)
                return string.Empty;
            try
            {
                return obj.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch { }
            return string.Empty;
        }
        public static bool ToBoolean(object obj)
        {
            if (obj == null)
                return false;
            try
            {
                return Convert.ToBoolean(obj.ToString().Trim());
            }
            catch { }
            return false;
        }


    }
}
