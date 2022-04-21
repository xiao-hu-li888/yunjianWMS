using LTWMSWebMVC.App_Start;
using LTWMSWebMVC.App_Start.AppCode;
using LTWMSWebMVC.Models; 
using LTWMSService.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LTWMSWebMVC.Controllers
{
    public class ApiBaseController : ApiController
    {
        log_sys_useroperationBLL bll_useroperation_log;
        public LogToDb LogBLL;
        public ApiBaseController()
        {
            this.bll_useroperation_log = AutofacConfig.GetFromFac<log_sys_useroperationBLL>();
            LogBLL = new LogToDb(AutofacConfig.GetFromFac<log_sys_alarmBLL>(), AutofacConfig.GetFromFac<log_sys_executeBLL>());
            //输出日志 
            //  AutofacConfig.GetFromFac<LTWMSEFModel.LTModel>().Database.Log = message => LTWMSWebMVC.WMSFactory.Log.v("LT-DBContext==>>" + message);
        }
        public string Getkeywords(string keywords)
        { 
            return (keywords ?? "").Trim(); 
        }
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="log"></param>
        public void AddUserOperationLog(string log,string createuser)
        {
            try
            {
                bll_useroperation_log.Add(log, createuser, 0);
                // bll_sys_execute_log.Add(log,diff);
                //bll_useroperation_log.Add(new LTWMSEFModel.Logs.log_sys_useroperation()
                //{ 
                //    guid = Guid.NewGuid(),
                //    log_date = DateTime.Now,
                //    operator_u = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName,
                //    remark = log
                //});
            }
            catch (Exception ex)
            {
                LTWMSWebMVC.WMSFactory.Log.v(ex);
            }
        }
        public HttpResponseMessage JsonSuccess()
        {
            //Hashtable hashtable = new Hashtable();
            //hashtable.Add("success", true); 
            //var resp = new HttpResponseMessage { Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(hashtable)
            //    , System.Text.Encoding.UTF8, "application/json") };
            //return resp;
            return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success });
        }
        public void AddJsonError(string error)
        {
            ModelState.AddModelError("errorMess", error);
        }
        public HttpResponseMessage JsonError()
        {
            var errmess = ModelState.SelectMany(x => x.Value.Errors.Select(er => er.ErrorMessage)).ToList(); 
            //Hashtable hashtable = new Hashtable();
            //hashtable.Add("success", false);
            //hashtable.Add("errors", errmess);
            //var resp = new HttpResponseMessage { Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(hashtable)
            //    , System.Text.Encoding.UTF8, "application/json") }; 
            return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.servererror, msg =string.Join(";", errmess )});
        }
    }
}
