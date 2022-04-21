using LTWMSEFModel.BillsAihua;
using LTWMSService.ApplicationService;
using LTWMSService.BillsAihua;
using LTWMSWebMVC.Areas.Bills.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Bills.Controllers
{
    public class ReserveController : BaseController
    {
        billah_reserved_orderBLL bll_billah_reserved_order;
        billah_reserved_order_detailBLL bll_billah_reserved_order_detail;
        ReservedOrderService bll_ReservedOrderService;
        public ReserveController(billah_reserved_orderBLL bll_billah_reserved_order
            , billah_reserved_order_detailBLL bll_billah_reserved_order_detail, ReservedOrderService bll_ReservedOrderService)
        {
            this.bll_billah_reserved_order = bll_billah_reserved_order;
            this.bll_billah_reserved_order_detail = bll_billah_reserved_order_detail;
            this.bll_ReservedOrderService = bll_ReservedOrderService;
        }
        // GET: Bills/Reserve  
        public ActionResult Index(ReservedOrderSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_billah_reserved_order.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.createdate, w =>
                  (
                     Model.s_keywords == "" || (w.yl_id ?? "").Contains(Model.s_keywords)
                     || (w.memo ?? "").Contains(Model.s_keywords)
                  )
                     && (
                      Model.s_status == null || (int)Model.s_status.Value == -1 || w.bill_out_status == Model.s_status
                     )
                 , false)
                .Select(s => MapperConfig.Mapper.Map<billah_reserved_order, ReservedOrderModel>(s)).ToList();

            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_billah_reserved_order.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<billah_reserved_order, ReservedOrderModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Update", Md);
        }
        /// <summary>
        /// 强制结束预留单（只修改预留单状态不对出库任务进行更改）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TerminateBill(ReservedOrderModel model)
        {
            try
            {
                billah_reserved_order info = bll_billah_reserved_order.GetFirstDefault(w => w.guid == model.guid);
                //并发控制（乐观锁） 
                info.OldRowVersion = model.OldRowVersion;
                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                //预留单开始出库，生成对应的出库任务 
                using (var _tran = bll_ReservedOrderService.BeginTran())
                {
                    ComServiceReturn rtv = bll_ReservedOrderService.TerminateReserveBill(info);
                    if (rtv.success)
                    {//操作成功
                        info.bill_out_status = ReserveBillOutStatus.Finished;
                        info.updatedate = DateTime.Now;
                        info.memo += ">>[" + DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + "]强制结束";
                        var rtv2 = bll_billah_reserved_order.Update(info);
                        if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                        {
                            _tran.Commit();
                            AddUserOperationLog("强制结束预留单[" + info.yl_id + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else
                        {
                            AddJsonError("修改数据失败.");
                        }
                    }
                    else
                    {
                        AddJsonError(rtv.result);
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 开始出库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StartOut(ReservedOrderModel model)
        {

            try
            {
                //盘点是否存在未完成的预留单
                var runObj = bll_billah_reserved_order.GetFirstDefault(w => w.bill_out_status == ReserveBillOutStatus.Running);
                if (runObj != null && runObj.guid != Guid.Empty)
                {
                    AddJsonError("系统存在未完成的预留单【"+ runObj.yl_id+ "】，请先处理完再操作。");
                    return JsonError();
                }

                /////////////////////
                billah_reserved_order info = bll_billah_reserved_order.GetFirstDefault(w => w.guid == model.guid);
                //并发控制（乐观锁） 
                info.OldRowVersion = model.OldRowVersion;
                info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                //预留单开始出库，生成对应的出库任务 
                using (var _tran = bll_ReservedOrderService.BeginTran())
                {
                    ComServiceReturn rtv = bll_ReservedOrderService.TrayStartOut(info);
                    if (rtv.success)
                    {//操作成功
                        info.bill_out_status = ReserveBillOutStatus.Running;
                        info.updatedate = DateTime.Now;
                        var rtv2 = bll_billah_reserved_order.Update(info);
                        if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                        {
                            _tran.Commit();
                            AddUserOperationLog("操作预留单出库[" + info.yl_id + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else
                        {
                            AddJsonError("修改数据失败.");
                        }
                    }
                    else
                    {
                        AddJsonError(rtv.result);
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /********************预留单明细*********************/
        [HttpGet]
        public ActionResult ReserveDetailsIndex(ReservedOrderDetailSearch Model)
        {
            Model.PageCont = bll_billah_reserved_order_detail.GetAllQueryOrderby(o => o.goods_id, w => w.reserved_order_guid == Model.reserve_guid, false)
                .Select(s => MapperConfig.Mapper.Map<billah_reserved_order_detail, ReservedOrderDetailModel>(s)).ToList();

            return View("ReserveDetailsIndex", Model);
        }
    }
}