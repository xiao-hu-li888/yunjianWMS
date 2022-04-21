using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web; 

namespace LTERPWebMVC.Models
{
    public class ResponseJson
    {
        /// <summary>
      /// 消息编码
      /// </summary>
        public ResponseCode code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 分页的总条数
        /// </summary>
        public int totalcount { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }
        public static HttpResponseMessage GetResponJson(ResponseJson response)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("code", response.code);
            hashtable.Add("msg", response.msg);
            if (response.totalcount > 0)
            {
                hashtable.Add("totalcount", response.totalcount);
            }
            if (response.data != null)
            {
                hashtable.Add("data", response.data);
            } 
            var resp = new HttpResponseMessage { Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(hashtable), System.Text.Encoding.UTF8, "application/json") };
            return resp;
        }
        public static string GetResponJsonString(ResponseJson response)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("code", response.code);
            hashtable.Add("msg", response.msg);
            if (response.totalcount > 0)
            {
                hashtable.Add("totalcount", response.totalcount);
            }
            if (response.data != null)
            {
                hashtable.Add("data", response.data);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(hashtable);
        }
    }

    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        success = 1000,
        /// <summary>
        /// 请求失败
        /// </summary>
        faild = 1001,
        /// <summary>
        /// 服务器错误
        /// </summary>
        servererror = 1002,
        /// <summary>
        /// 值为空
        /// </summary>
        emptyvalue = 1003,
        /// <summary>
        /// 参数错误
        /// </summary>
        paramerror = 1004,
        /// <summary>
        /// 未授权连接(非法连接)
        /// </summary>
        unauthorized = 1005,
        /// <summary>
        /// 登录成功
        /// </summary>
        loginsuccess = 1006,
        /// <summary>
        /// 登录失败
        /// </summary>
        loginfaild = 1007,
        /// <summary>
        /// apk有新版本
        /// </summary>
        apknewversion = 1008,
        /// <summary>
        /// apk维持旧版本
        /// </summary>
        apkoldversion = 1009


    }

}