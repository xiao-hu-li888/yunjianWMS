using LTWMSWebMVC.Areas.Bills.Data;
using LTWMSWebMVC.Models;
using LTWMSEFModel.Bills;
using LTWMSService.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LTWMSWebMVC.Controllers.PDA
{
    public class Pda_BillsController : ApiBaseController
    {
        bill_stockinBLL bll_bill_stockin;
        public Pda_BillsController(bill_stockinBLL bll_bill_stockin)
        {
            this.bll_bill_stockin = bll_bill_stockin;
        }
        /// <summary>
        /// 获取入库单据列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyws"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBillsStockInList(string pageindex, string pagesize, string keyws)
        {
            try
            {
                keyws = Getkeywords(keyws);
                int _indx = Convert.ToInt32(pageindex);
                int _pagesize = Convert.ToInt32(pagesize);
                int totalcount = 0;
                var DataList = bll_bill_stockin.Pagination(_indx
                     , _pagesize, out totalcount, o => o.createdate,
                     w => (keyws == "" || (w.memo ?? "").Contains(keyws)
                     || (w.odd_numbers ?? "").Contains(keyws) || (w.operator_user ?? "").Contains(keyws)
                     || (w.deliverer ?? "").Contains(keyws) || (w.contact_department ?? "").Contains(keyws)
                     || (w.code ?? "").Contains(keyws)), false)
                     .Select(s => MapperConfig.Mapper.Map<bill_stockin, StockInModel>(s)).ToList();
                return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = DataList, totalcount = totalcount });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 查询入库单
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBillsStockIn(string guid)
        {
            try
            {
                Guid _GUID = Guid.Parse(guid);
                var obj = bll_bill_stockin.GetFirstDefault(w => w.guid == _GUID);
                if (obj != null)
                {
                    var objModel = MapperConfig.Mapper.Map<bill_stockin, StockInModel>(obj);
                    objModel.OldRowVersion = obj.rowversion;
                    return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = objModel });
                }
                else
                {
                    AddJsonError("参数错误。。。");
                }
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 保存入库订单状态为入库完成（已结束）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveBillStockIn()
        {
            string _order = "";
            try
            {
                Guid _GUID = Guid.Parse( HttpContext.Current.Request.Form["guid"]);
                //旧版本号、防止并发
                int _OldRowVersion =LTLibrary.ConvertUtility.ToInt( HttpContext.Current.Request.Form["oldrowversion"]); 
                bill_stockin obj = bll_bill_stockin.GetFirstDefault(w => w.guid == _GUID);
                if (obj != null)
                {
                    _order = obj.odd_numbers;
                    obj.bill_status = BillsStatus.Finished;
                    //并发控制（乐观锁）
                    obj.OldRowVersion = _OldRowVersion;
                    var rtv = bll_bill_stockin.Update(obj);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {//修改成功 
                        AddUserOperationLog("修改订单状态为"+LTLibrary.EnumHelper.GetEnumDescription(obj.bill_status),"PDA"); 
                        return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success}); 
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据然后再试。");
                    }
                    else
                    {
                        AddJsonError("保存失败");
                    } 
                }
                else
                {
                    AddJsonError("参数错误。。。");
                } 
            }
            catch (Exception ex)
            {
                AddJsonError("修改入库订单["+ _order + "]状态为入库完成失败！异常：=>>" + ex.ToString());
                AddUserOperationLog("修改入库订单[" + _order + "]状态为入库完成失败！异常：=>>" + ex.ToString(), "PDA");
            }
            return JsonError();
        }
    }
}
