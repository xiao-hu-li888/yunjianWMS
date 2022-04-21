using LTERPService.Stock;
using LTERPWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LTERPWebMVC.Controllers.MatterApi
{
    public class MatterController : ApiBaseController
    {
        stk_matterBLL bll_stk_matter;
        public MatterController(stk_matterBLL bll_stk_matter)
        {
            this.bll_stk_matter = bll_stk_matter;
        }
        /// <summary>
        /// 获取所有物料信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAllMatter()
        {
            try
            {
              var lstMatter=  bll_stk_matter.GetAllQuery(w => w.state == LTERPEFModel.EntityStatus.Normal);
                //List<BatteryKeyValues> lstData = new List<BatteryKeyValues>();
                //lstData.Add(new BatteryKeyValues() { Text = "尺寸", Val = "100mmx500mm" });
                //lstData.Add(new BatteryKeyValues() { Text = "规格", Val = "AL-1" });
                //lstData.Add(new BatteryKeyValues() { Text = "重量", Val = "15KG" });
                //lstData.Add(new BatteryKeyValues() { Text = "颜色", Val = "黑色" });
                return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = lstMatter });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }

    }
}
