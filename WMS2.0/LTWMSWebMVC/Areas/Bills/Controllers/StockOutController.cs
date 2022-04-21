using LTWMSWebMVC.Areas.Bills.Data;
using LTWMSEFModel.Bills;
using LTWMSService.Basic;
using LTWMSService.Bills;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using LTWMSService.Warehouse;
using LTWMSService.Stock;
using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LTWMSWebMVC.Areas.Bills.Controllers
{
    public class StockOutController : BaseController
    {
        bill_stockoutBLL bll_bill_stockout;
        bill_stockinBLL bll_bill_stockin;
        sys_number_ruleBLL bll_sys_number_rule;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        sys_control_dicBLL bll_sys_control_dic;
        wh_shelfunitsBLL bll_wh_shelfunits;
        bill_stockout_detailBLL bll_bill_stockout_detail;
        stk_matterBLL bll_stk_matter;
        bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter;
        wh_traymatterBLL bll_wh_traymatter;
        bill_task_tray_relationBLL bll_bill_task_tray_relation;
        wh_trayBLL bll_wh_tray;
        //hdw_agv_task_mainBLL bll_hdw_agv_task_main;
        //hdw_agv_taskqueueBLL bll_hdw_agv_taskqueue;
        public StockOutController(bill_stockoutBLL bll_bill_stockout, bill_stockinBLL bll_bill_stockin, sys_number_ruleBLL bll_sys_number_rule,
            hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue, sys_control_dicBLL bll_sys_control_dic, wh_shelfunitsBLL bll_wh_shelfunits,
             bill_stockout_detailBLL bll_bill_stockout_detail, stk_matterBLL bll_stk_matter, bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter
        , wh_traymatterBLL bll_wh_traymatter, bill_task_tray_relationBLL bll_bill_task_tray_relation, wh_trayBLL bll_wh_tray)
        {
            this.bll_bill_stockout_detail_traymatter = bll_bill_stockout_detail_traymatter;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_bill_stockout_detail = bll_bill_stockout_detail;
            this.bll_sys_control_dic = bll_sys_control_dic;
            //this.bll_hdw_agv_taskqueue = bll_hdw_agv_taskqueue;
            //this.bll_hdw_agv_task_main = bll_hdw_agv_task_main;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_bill_stockout = bll_bill_stockout;
            this.bll_bill_stockin = bll_bill_stockin;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_sys_number_rule = bll_sys_number_rule;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_bill_task_tray_relation = bll_bill_task_tray_relation;
            this.bll_wh_tray = bll_wh_tray;
            //设置出库单
            //  ListDataManager.setBillInOddNumberGuidList(bll_bill_stockin);
            //设置Agv终点
            ListDataManager.setAgvDestinationList();
            ListDataManager.SetALLMatterList(bll_stk_matter);
        }
        public ActionResult DispatchTrayIndex(DispatchTraySearch Model)
        {
            //分配指定的托盘进行下架
            var detailTrayMatter = bll_bill_stockout_detail.GetFirstDefault(w => w.guid == Model.billstockout_detail_guid);
            if (detailTrayMatter != null && detailTrayMatter.guid != Guid.Empty)
            {
                Model.PageCont = bll_wh_tray.GetTrayMatterListOnShelf(detailTrayMatter.matter_code
                , detailTrayMatter.lot_number)
                    .Select(s => MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s)).ToList();
            }
            return PartialView(Model);
        }
        [HttpPost]
        public JsonResult DispatchTrayIndex(DispatchTraySearch Model, string traymatter_chk_guids)
        {
            try
            {
                traymatter_chk_guids = Request.Params["traymatter_chk_guids"];
                var detailTrayMatter = bll_bill_stockout_detail.GetFirstDefault(w => w.guid == Model.billstockout_detail_guid);
                if (detailTrayMatter != null && detailTrayMatter.guid != Guid.Empty)
                {
                    var stockoutMd = bll_bill_stockout.GetFirstDefault(w => w.guid == detailTrayMatter.stockout_guid);
                    if (stockoutMd != null && stockoutMd.guid != Guid.Empty)
                    {
                        List<Guid> lstGuid = LTLibrary.ConvertUtility.ParseToGuids(traymatter_chk_guids);
                        if (lstGuid != null && lstGuid.Count > 0)
                        {
                            var ListShelfOut = bll_wh_shelfunits.GetShelfUnitOutToTaskByGuids(lstGuid);
                            if (ListShelfOut != null && ListShelfOut.Count > 0)
                            {//待出库的仓位信息ListShelfOut  
                                foreach (var item in ListShelfOut)
                                {
                                    var trayMatterList = bll_wh_tray.GetMatterDetailByTrayBarcode(item.depth1_traybarcode);
                                    if (trayMatterList != null && trayMatterList.Count > 0)
                                    {
                                        if ((stockoutMd.stockout_type == StockOutType.SellOut ||
                                        stockoutMd.stockout_type == StockOutType.UseOut)
                                        && trayMatterList[0].test_status != TestStatusEnum.TestOk)
                                        {//非合格状态下 销售领用不能出库
                                            AddJsonError("产品非合格状态不能出库！");
                                            return JsonError();
                                        }
                                        else
                                        {
                                            string xbarcode = trayMatterList[0].x_barcode;
                                            var matterMd = bll_stk_matter.GetFirstDefault(w => w.code == xbarcode);
                                            using (var tran = bll_wh_shelfunits.BeginTransaction())
                                            {
                                                LTWMSEFModel.SimpleBackValue rtv2 = LTWMSEFModel.SimpleBackValue.False;
                                                LTWMSEFModel.SimpleBackValue rtv1 = LTWMSEFModel.SimpleBackValue.False;
                                                LTWMSEFModel.SimpleBackValue rtvtask = LTWMSEFModel.SimpleBackValue.False;
                                                LTWMSEFModel.SimpleBackValue rtvshelfunit = LTWMSEFModel.SimpleBackValue.False;

                                                //  生成对应的出库任务。。。
                                                bill_stockout_detail_traymatter detailTrayM = new bill_stockout_detail_traymatter();
                                                var task = bll_hdw_stacker_taskqueue.AddTaskByShelfUnit(item, "");
                                                if (task != null)
                                                {
                                                    //自动删除取消的出库明细
                                                    string _trayBcd = trayMatterList[0].traybarcode;
                                                    var ListDetTrayM = bll_bill_stockout_detail_traymatter.GetAllQuery(w => w.stockout_detail_guid == detailTrayMatter.guid
                                                      && w.traybarcode == _trayBcd && w.tray_status == TrayOutStockStatusEnum.WaitOut);
                                                    if (ListDetTrayM != null && ListDetTrayM.Count > 0)
                                                    {
                                                        foreach (var itmDTM in ListDetTrayM)
                                                        {
                                                            bll_bill_stockout_detail_traymatter.Delete(itmDTM);
                                                        }
                                                    }
                                                    //  添加出库表数据 
                                                    detailTrayM.createdate = DateTime.Now;
                                                    detailTrayM.effective_date = trayMatterList[0].effective_date;
                                                    detailTrayM.guid = Guid.NewGuid();
                                                    detailTrayM.lot_number = trayMatterList[0].lot_number;
                                                    detailTrayM.matter_code = trayMatterList[0].x_barcode;
                                                    detailTrayM.matter_name = trayMatterList[0].name_list;
                                                    detailTrayM.number = trayMatterList[0].number;
                                                    detailTrayM.out_shelfunits_guid = item.guid;
                                                    detailTrayM.out_shelfunits_pos = item.shelfunits_pos;
                                                    detailTrayM.out_stacker_taskqueue_guid = task.guid;
                                                    detailTrayM.produce_date = trayMatterList[0].producedate;
                                                    detailTrayM.state = LTWMSEFModel.EntityStatus.Normal;
                                                    detailTrayM.stk_matter_guid = matterMd?.guid;
                                                    detailTrayM.stockout_detail_guid = detailTrayMatter.guid;
                                                    detailTrayM.stockout_guid = stockoutMd.guid;
                                                    detailTrayM.test_status = trayMatterList[0].test_status;
                                                    detailTrayM.traybarcode = trayMatterList[0].traybarcode;
                                                    detailTrayM.tray_status = TrayOutStockStatusEnum.WaitOut;
                                                    rtv1 = bll_bill_stockout_detail_traymatter.Add(detailTrayM);
                                                    // //
                                                    task.order = stockoutMd.odd_numbers;
                                                    task.re_detail_traymatter_guid = detailTrayM.guid;
                                                    task.bills_type = LTWMSEFModel.Hardware.BillsTypeEnum.BillsOut;

                                                    //修改对应库位状态
                                                    item.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.WaitOut;
                                                    item.tray_outdatetime = DateTime.Now;
                                                    rtvshelfunit = bll_wh_shelfunits.Update(item);
                                                    //对应库位有历史遗留任务/先删除历史任务再新增 
                                                    var DelList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal
                                                    && (w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut || w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.MoveTo) &&
                                                       w.src_shelfunits_guid == item.guid);
                                                    if (DelList != null && DelList.Count > 0)
                                                    {
                                                        foreach (var itemD in DelList)
                                                        {
                                                            bll_hdw_stacker_taskqueue.Delete(itemD);
                                                        }
                                                    }
                                                    //添加任务
                                                    rtvtask = bll_hdw_stacker_taskqueue.Add(task);
                                                }

                                                if (stockoutMd.stockout_type == StockOutType.CheckOut ||
                                                  stockoutMd.stockout_type == StockOutType.SamplingOut)
                                                {//需要回库
                                                 //删除之前旧数据
                                                    var lstReTsk = bll_bill_task_tray_relation.GetAllQuery(w => w.traybarcode == item.depth1_traybarcode);
                                                    if (lstReTsk != null && lstReTsk.Count > 0)
                                                    {
                                                        foreach (var rtskItm in lstReTsk)
                                                        {
                                                            bll_bill_task_tray_relation.Delete(rtskItm);
                                                        }
                                                    }
                                                    //   盘点是否销售出库、不需要回库，则不添加 订单、任务、托盘 关联表记录
                                                    var taskTrayRelationM = new bill_task_tray_relation();
                                                    taskTrayRelationM.bill_type = ReBillTypeEnum.StockOut;
                                                    taskTrayRelationM.createdate = DateTime.Now;
                                                    taskTrayRelationM.guid = Guid.NewGuid();
                                                    taskTrayRelationM.odd_numbers = stockoutMd.odd_numbers;
                                                    taskTrayRelationM.re_detail_traymatter_guid = detailTrayM.guid;
                                                    taskTrayRelationM.state = LTWMSEFModel.EntityStatus.Normal;
                                                    if (stockoutMd.stockout_type == StockOutType.CheckOut)
                                                    {
                                                        taskTrayRelationM.stockout_type = StockOutType.CheckOut;
                                                    }
                                                    else
                                                    {
                                                        taskTrayRelationM.stockout_type = StockOutType.SamplingOut;
                                                    }
                                                    taskTrayRelationM.traybarcode = item.depth1_traybarcode;

                                                    rtv2 = bll_bill_task_tray_relation.Add(taskTrayRelationM);
                                                }
                                                else
                                                {//不需要回库
                                                    rtv2 = LTWMSEFModel.SimpleBackValue.True;
                                                }

                                                if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True
                                                    && rtvtask == LTWMSEFModel.SimpleBackValue.True && rtvshelfunit == LTWMSEFModel.SimpleBackValue.True)
                                                { 
                                                    tran.Commit();
                                                }
                                                else
                                                {
                                                    AddJsonError("保存数据失败，事务已回滚！！");
                                                    return JsonError();
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                AddUserOperationLog("分配托盘出库："+ string.Join(",", ListShelfOut.Select(w => w.depth1_traybarcode).ToArray()), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                return JsonSuccess();
                            }
                            else
                            {
                                AddJsonError("操作失败！没有查找到对应的可出库库位或库位已设置出库锁定！");
                            }
                        }
                        else
                        {
                            AddJsonError("参数错误>>" + traymatter_chk_guids);
                        }

                    }
                    else
                    {
                        AddJsonError("出库订单记录不存在或已删除！");
                    }
                }
                else
                {
                    AddJsonError("出库订单详细记录不存在或已删除！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        // GET: Bills/StockIn  
        public ActionResult Index(StockOutSearch Model)
        {
            if (Request.HttpMethod == "GET")
            {
                Model.s_bill_status =  LTWMSEFModel.SearchBillsStatus_Out.Running;
            }
            int TotalSize = 0;
            int _id = 0;
            Model.PageCont = bll_bill_stockout.PaginationByLinq(Model.s_keywords, Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, Model.s_bill_status, Model.s_out_date_begin, Model.s_out_date_end).Select(
                        s =>
                        {
                            _id++;
                            var Md = MapperConfig.Mapper.Map<bill_stockout, StockOutModel>(s); 
                            Md.Id = _id + (Model.Paging.PageSize * (Model.Paging.paging_curr_page - 1));
                            Md.List_StockOutDetailModel = bll_bill_stockout_detail.GetAllQuery(w => w.stockout_guid == Md.guid)
                            .Select(s => MapperConfig.Mapper.Map<bill_stockout_detail, StockOutDetailModel>(s)).ToList();
                            return Md;
                        }
                        ).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加出库单";
            StockOutModel Model = new StockOutModel();
            //  Model.odd_numbers = bll_sys_number_rule.GetBillStockOutNum(); ;
            // Model.odd_numbers_in = "";
            Model.out_date = DateTime.Now;
            return PartialView(Model);
        }
        [HttpPost]
        public JsonResult Add(StockOutModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //添加出库订单号、
                    LTWMSEFModel.Bills.bill_stockout stockoutModel = new LTWMSEFModel.Bills.bill_stockout();

                    stockoutModel.bill_status = model.bill_status;
                    stockoutModel.createdate = DateTime.Now;
                    stockoutModel.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    stockoutModel.from = LTWMSEFModel.Bills.BillsFrom.System;
                    stockoutModel.guid = Guid.NewGuid();
                    stockoutModel.odd_numbers = bll_sys_number_rule.GetBillStockOutNum();
                    stockoutModel.odd_numbers_in = model.odd_numbers_in;//管理出库单据
                    stockoutModel.destination = model.destination;
                    stockoutModel.out_date = model.out_date;
                    stockoutModel.operator_user = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    stockoutModel.state = LTWMSEFModel.EntityStatus.Normal;
                    stockoutModel.stockout_type = model.stockout_type;
                    stockoutModel.contact_department = model.contact_department;
                    stockoutModel.customer_name = model.customer_name;
                    stockoutModel.generated_task = model.generated_task;
                    stockoutModel.get_status = GetStatus_Out.None;
                    stockoutModel.memo = model.memo;
                    stockoutModel.project_name = model.project_name;
                    stockoutModel.project_no = model.project_no;
                    stockoutModel.receiver = model.receiver;
                    stockoutModel.total_category = model.total_category;
                    stockoutModel.total_matter = model.total_matter;
                    stockoutModel.total_out = model.total_out;

                    var rtv = bll_bill_stockout.AddIfNotExists(stockoutModel, w => w.odd_numbers);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加出库订单[" + stockoutModel.odd_numbers + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在订单号为：[" + stockoutModel.odd_numbers + "]的记录信息");
                    }
                    else
                    {
                        AddJsonError("保存失败");
                    }


                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }

        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存出库单";
            ViewBag.isUpdate = true;
            var model = bll_bill_stockout.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<bill_stockout, StockOutModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(StockOutModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var stockoutModel = bll_bill_stockout.GetFirstDefault(w => w.guid == model.guid);
                    if (stockoutModel != null && stockoutModel.guid != Guid.Empty)
                    {  // 订单状态不能后退
                        if ((int)stockoutModel.bill_status > (int)model.bill_status)
                        {
                            AddJsonError("订单状态不能由“" + LTLibrary.EnumHelper.GetEnumDescription(stockoutModel.bill_status)
                                + "”修改为“" + LTLibrary.EnumHelper.GetEnumDescription(model.bill_status) + "”");
                            return JsonError();
                        }
                        stockoutModel.bill_status = model.bill_status;
                        stockoutModel.updatedate = DateTime.Now;
                        stockoutModel.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        // stockoutModel.from = LTWMSEFModel.Bills.BillsFrom.System;
                        // stockoutModel.guid = Guid.NewGuid();
                        // stockoutModel.odd_numbers = bll_sys_number_rule.GetBillStockOutNum();
                        //stockoutModel.odd_numbers_in = model.odd_numbers_in;//管理出库单据
                        stockoutModel.destination = model.destination;
                        stockoutModel.out_date = model.out_date;
                        stockoutModel.operator_user = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //  stockoutModel.state = LTWMSEFModel.EntityStatus.Normal;
                        stockoutModel.stockout_type = model.stockout_type;
                        stockoutModel.contact_department = model.contact_department;
                        stockoutModel.customer_name = model.customer_name;
                        stockoutModel.generated_task = model.generated_task;
                        //  stockoutModel.get_status = GetStatus_Out.None;
                        stockoutModel.memo = model.memo;
                        stockoutModel.project_name = model.project_name;
                        stockoutModel.project_no = model.project_no;
                        stockoutModel.receiver = model.receiver;
                        //stockoutModel.total_category = model.total_category;
                        //stockoutModel.total_matter = model.total_matter;
                        //stockoutModel.total_out = model.total_out;

                        var rtv = bll_bill_stockout.Update(stockoutModel);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改出库订单[" + stockoutModel.odd_numbers + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                        {
                            AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                        }
                        else
                        {
                            AddJsonError("保存失败");
                        }
                    }
                    else
                    {
                        AddJsonError("数据库中不存在该条记录或已删除！");
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }
        /// <summary>
        /// 强行结束出库单任务。。。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ForceFinishBillStockOut(StockOutModel Model)
        {
            try
            {
                var stockOut = bll_bill_stockout.GetFirstDefault(w => w.guid == Model.guid);
                if (stockOut != null && stockOut.guid != Guid.Empty)
                {
                    //结束出库单
                    stockOut.bill_status = BillsStatus_Out.Finished;
                    stockOut.updatedate = DateTime.Now;
                    stockOut.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    stockOut.memo += ";[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]操作出库单强行结束。";
                    //并发控制（乐观锁）
                    stockOut.OldRowVersion = Model.OldRowVersion;
                    var rtv = bll_bill_stockout.Update(stockOut);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        ////不需要考虑立库出库任务（强行结束前必须要终止出库单操作）
                        //var agvMain = bll_hdw_agv_task_main.GetFirstDefault(w => w.order == stockOut.odd_numbers_in);
                        //if (agvMain != null && agvMain.guid != Guid.Empty)
                        //{//agv_main任务 结束
                        //    agvMain.task_execute_status = LTWMSEFModel.Hardware.AgvTaskExecuteStatus.Finished;
                        //    agvMain.memo += ";[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]操作任务强行结束。";
                        //    var rtv2 = bll_hdw_agv_task_main.Update(agvMain);
                        //    if (rtv2 != LTWMSEFModel.SimpleBackValue.True)
                        //    {
                        //        WMSFactory.Log.v("[ForceFinishBillStockOut]12335553 >>>保存数据失败。。。");
                        //    }
                        //}
                        ////  agv_taskqueue 全部取消，正在运行中的任务，等待自行结束或操作agv wcs取消/ 完成
                        //var listAgvTaskQueue = bll_hdw_agv_taskqueue.GetAllQuery(w => w.order == stockOut.odd_numbers_in);
                        //if (listAgvTaskQueue != null && listAgvTaskQueue.Count > 0)
                        //{
                        //    foreach (var agvTq in listAgvTaskQueue)
                        //    {
                        //        agvTq.task_status = LTWMSEFModel.Hardware.AgvTaskStatus.CancelHandling;
                        //        agvTq.updatedate = DateTime.Now;
                        //        agvTq.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //    }
                        //    bll_hdw_agv_taskqueue.Update(listAgvTaskQueue);
                        //}
                        AddUserOperationLog("操作出库单[" +
                           stockOut.odd_numbers_in + "]强行结束。", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                    }
                    else
                    {
                        AddJsonError("保存失败");
                    }
                }
                else
                {
                    //参数错误
                    AddJsonError("参数错误，数据库不存在或已删除。");
                }

            }
            catch (Exception ex)
            {
                AddJsonError("重发失败！异常:" + ex.Message);
            }
            return JsonError();

        }

        [HttpPost]
        public JsonResult DeletePost(Guid guid)
        {
            try
            {
                var model = bll_bill_stockout.GetFirstDefault(w => w.guid == guid);
                if (model != null && model.guid != Guid.Empty)
                {
                    //判断订单是否执行，如果执行出库则不能删除！！！！
                    if (model.bill_status == BillsStatus_Out.None)
                    {
                        //var rtv = bll_bill_stockin.Delete(w => w.guid == guid); 
                        var rtv = bll_bill_stockout.Delete(model);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("删除订单[" + model.odd_numbers + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.NotExistOfDelete)
                        {
                            AddJsonError("数据库不存在记录或已删除！");
                        }
                        else
                        {
                            AddJsonError("删除失败！");
                        }
                    }
                    else
                    {
                        AddJsonError("订单[" + model.odd_numbers + "]状态为“" + LTLibrary.EnumHelper.GetEnumDescription(model.bill_status) + "”不能删除！");
                    }
                }
                else
                {
                    AddJsonError("数据库不存在记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }

        public ActionResult DetailIndex(StockOutDetailSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_bill_stockout_detail.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.createdate,
                w => w.stockout_guid == Model.billstockout_guid &&
                (Model.s_keywords == "" || (w.memo ?? "").Contains(Model.s_keywords)
                || (w.lot_number ?? "").Contains(Model.s_keywords) || (w.matter_code ?? "").Contains(Model.s_keywords)
                || (w.matter_name ?? "").Contains(Model.s_keywords))
                        , false).Select(s =>
                        {
                            var M = MapperConfig.Mapper.Map<bill_stockout_detail, StockOutDetailModel>(s);
                            M.List_bill_stockout_detail_traymatterModel =
                            bll_bill_stockout_detail_traymatter.GetAllQuery(w => w.stockout_detail_guid == M.guid)
                            .Select(s => MapperConfig.Mapper.Map<bill_stockout_detail_traymatter, bill_stockout_detail_traymatterModel>(s)).ToList();
                            return M;

                        }).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }

        [HttpGet]
        public ActionResult DetailAdd(Guid billstk_guid)
        {
            ViewBag.SubmitText = "添加";
            StockOutDetailModel Model = new StockOutDetailModel();
            // Model.in_date = DateTime.Now;
            Model.stockout_guid = billstk_guid;
            return PartialView(Model);
        }
        [HttpPost]
        public JsonResult DetailAdd(StockOutDetailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //限制只能添加一条明细
                    if (bll_bill_stockout_detail.GetCount(w => w.stockout_guid == model.stockout_guid) > 0)
                    {
                        AddJsonError("出库订单只能有一种物料明细");
                        return JsonError();
                    }
                    if (model.out_number <= 0)
                    {
                        AddJsonError("出库数量必须大于0");
                        return JsonError();
                    }
                    //判断出库数量是否<=库存
                    decimal rtvNum = bll_wh_traymatter.GetStockNumberByMatterGuid(model.matter_guid, model.lot_number);
                    if (model.out_number > rtvNum)
                    {
                        AddJsonError("出库数量【"+ model.out_number + "】不能大于库存数【"+ (int)rtvNum + "】");
                        return JsonError();
                    }
                    bill_stockout_detail info = new bill_stockout_detail();
                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    // info.effective_date = model.effective_date;
                    info.guid = Guid.NewGuid();

                    info.out_number = model.out_number;
                    info.memo = model.memo;
                    info.lot_number = model.lot_number;

                    var matterObj = bll_stk_matter.GetFirstDefault(w => w.guid == model.matter_guid);
                    info.matter_guid = model.matter_guid;
                    info.state = LTWMSEFModel.EntityStatus.Normal;
                    info.matter_name = matterObj.name;
                    info.matter_code = matterObj.code;
                    //   info.producedate = model.producedate;
                    info.stockout_guid = model.stockout_guid;


                    var rtv = bll_bill_stockout_detail.Add(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加出库订单明细[" + info.matter_code + "][" + info.matter_name + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else
                    {
                        AddJsonError("保存失败");
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }

        [HttpGet]
        public ActionResult DetailUpdate(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_bill_stockout_detail.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<bill_stockout_detail, StockOutDetailModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("DetailAdd", Md);
        }

        [HttpPost]
        public JsonResult DetailUpdate(StockOutDetailModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    bill_stockout_detail info = bll_bill_stockout_detail.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null)
                    {
                        //// 订单状态不能后退
                        //if ((int)info.bill_status > (int)model.bill_status)
                        //{
                        //    AddJsonError("订单状态不能由“" + LTLibrary.EnumHelper.GetEnumDescription(info.bill_status)
                        //        + "”修改为“" + LTLibrary.EnumHelper.GetEnumDescription(model.bill_status) + "”");
                        //    return JsonError();
                        //}

                        ////////////////////////
                        info.updatedate = DateTime.Now;
                        info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        //  info.effective_date = model.effective_date;
                        info.out_number = model.out_number;
                        info.memo = model.memo;
                        info.lot_number = model.lot_number;

                        var matterObj = bll_stk_matter.GetFirstDefault(w => w.guid == model.matter_guid);
                        info.matter_guid = model.matter_guid;
                        //      info.state = LTWMSEFModel.EntityStatus.Normal;
                        info.matter_name = matterObj.name;
                        info.matter_code = matterObj.code;
                        // info.producedate = model.producedate;
                        //  info.stockin_guid = model.stockin_guid;
                        //info.test_status = model.test_status;

                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_bill_stockout_detail.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改出库订单明细[" + info.matter_code + "][" + info.matter_name + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                        {
                            AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                        }
                        else
                        {
                            AddJsonError("保存失败");
                        }
                    }
                    else
                    {
                        AddJsonError("数据库中不存在该条记录或已删除！");
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("保存数据出错！异常：" + ex.Message);
                }
            }
            else
            {
                AddJsonError("数据验证失败！");
            }
            return JsonError();
        }

        /// <summary>
        /// 检查条码类型并返回
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getLotNumberList(Guid matter_guid)
        {
            try
            {
                var srvRtv = bll_wh_traymatter.GetLotNumberListOnShelf(matter_guid);
                return Json(new { success = true, data = srvRtv });
            }
            catch (Exception ex)
            {
                AddJsonError("错误>>" + ex.ToString());
            }
            return JsonError();
        }
        /// <summary>
        /// 通过批次号和物料guid获取对应的库存数
        /// </summary>
        /// <param name="matter_guid"></param>
        /// <param name="lot_number"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getStockNumByLotNumberMatterGuid(Guid matter_guid, string lot_number)
        {
            try
            {
                decimal rtvNum = bll_wh_traymatter.GetStockNumberByMatterGuid(matter_guid, lot_number);
                return Json(new { success = true, data = rtvNum });
            }
            catch (Exception ex)
            {
                AddJsonError("错误>>" + ex.ToString());
            }
            return JsonError();
        }
        [HttpPost]
        public JsonResult DetailDeletePost(Guid guid)
        {
            try
            {
                var model = bll_bill_stockout_detail.GetFirstDefault(w => w.guid == guid);
                if (model != null && model.guid != Guid.Empty)
                {
                    int countTrayM = bll_bill_stockout_detail_traymatter.GetCount(w => w.stockout_detail_guid == model.guid);

                    //判断订单是否执行，如果执行出库则不能删除！！！！
                    if (countTrayM == 0)
                    {
                        var rtv = bll_bill_stockout_detail.Delete(model);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("删除订单明细成功！[" + model.matter_code + "][" + model.matter_name + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.NotExistOfDelete)
                        {
                            AddJsonError("数据库不存在记录或已删除！");
                        }
                        else
                        {
                            AddJsonError("删除失败！");
                        }
                    }
                    else
                    {
                        AddJsonError("订单明细已经绑定出库托盘，不能删除。");
                    }
                }
                else
                {
                    AddJsonError("数据库不存在记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }

        /// <summary>
        /// 导出出库单
        /// </summary>
        /// <param name="billstockin_guid"></param>
        /// <returns></returns>
        public ActionResult ExportPDF(Guid billstockout_guid, DateTime? exp_date)
        {
            //获取桌面路径设为文件下载保存路径
            //   string Fname = Server.MapPath("~/")+ "pdf/证书.pdf";
            var stockOutMd = bll_bill_stockout.GetFirstDefault(w => w.guid == billstockout_guid);
            if (stockOutMd != null && stockOutMd.guid != Guid.Empty)
            {
                if (stockOutMd.stockout_type == StockOutType.SamplingOut)
                {//抽样出库打印不同。。。
                    var ms = new MemoryStream();
                    #region CreatePDF
                    Document document = new Document(PageSize.A4, 5f, 5f, 30f, 0f);
                    //Document document = new Document(PageSize.A4.Rotate(), 0f, 0f, 10f, 0f);(A4纸横线打印)
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    PdfPTable table = new PdfPTable(8);
                    table.SplitLate = false;//设置当前页能放多少放多少
                    table.TotalWidth = 550;
                    table.LockedWidth = true;
                    table.SetWidths(new int[] { 40, 65, 65, 55, 60, 45, 45, 170 });
                    PdfPCell cell;
                    BaseFont bfChinese = BaseFont.CreateFont("C://WINDOWS//Fonts//simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font fontChinese_11 = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_10 = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_bold = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_8 = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    //黑体
                    BaseFont bf_ht = BaseFont.CreateFont("C://WINDOWS//Fonts//simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font ht_7 = new iTextSharp.text.Font(bf_ht, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));

                    cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type) + "单", fontChinese_11));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    //cell = new PdfPCell(new Phrase("定金单", fontChinese_10));
                    //cell.Colspan = 8;
                    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell.Border = Rectangle.NO_BORDER;
                    //table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);
                    if (exp_date != null)
                    {
                        cell = new PdfPCell(new Phrase("打印日期：" + exp_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("打印日期：" + DateTime.Now.ToString("yyyy/MM/dd"), fontChinese));
                    }
                    cell.Colspan = 2;
                    cell.PaddingBottom = 6;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    string str = ""; 

                    document.Add(table);

                    // table.SetWidths(new int[] { 40, 70, 65, 55, 60, 45, 45, 165 });

                    if (stockOutMd != null && stockOutMd.guid != Guid.Empty)
                    {
                        var stockoutDetailMd = bll_bill_stockout_detail.GetFirstDefault(w => w.stockout_guid == stockOutMd.guid);
                        if (stockoutDetailMd != null && stockoutDetailMd.guid != Guid.Empty)
                        {
                            table = new PdfPTable(8);
                            table.TotalWidth = 550;
                            table.LockedWidth = true;

                            cell = new PdfPCell(new Phrase("出库单号", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.odd_numbers, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("出库日期", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.out_date.ToString("yyyy/MM/dd"), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("出库类型", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("批次", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.lot_number, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);


                            cell = new PdfPCell(new Phrase("物料码", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.matter_code, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);


                            cell = new PdfPCell(new Phrase("名称", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.matter_name, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Colspan = 3;
                            cell.Padding = 5;
                            cell.PaddingLeft = 15;
                            table.AddCell(cell);



                            cell = new PdfPCell(new Phrase("出库数量", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.out_number.ToString(), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            /**添加明细**/
                            var lstDetailTrayMatter = bll_bill_stockout_detail_traymatter.GetAllQuery(w => w.stockout_detail_guid == stockoutDetailMd.guid);
                            if (lstDetailTrayMatter != null && lstDetailTrayMatter.Count > 0)
                            {
                                cell = new PdfPCell(new Phrase("托盘条码", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(lstDetailTrayMatter[0].traybarcode, fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);


                                cell = new PdfPCell(new Phrase("生产日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(lstDetailTrayMatter[0].produce_date == null ? "" : lstDetailTrayMatter[0].produce_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);


                                cell = new PdfPCell(new Phrase("有效日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(lstDetailTrayMatter[0].produce_date == null ? "" : lstDetailTrayMatter[0].produce_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);


                                cell = new PdfPCell(new Phrase("检验状态", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(lstDetailTrayMatter[0].test_status), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                            }
                            /**************************************/

                            cell = new PdfPCell(new Phrase("备注", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.memo, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            cell.PaddingLeft = 10;
                            cell.Colspan = 7;
                            table.AddCell(cell);


                            //明细
                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);

                            //、、、、、、、、、、、、、
                            cell = new PdfPCell(new Phrase("", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase("总计：", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.memo, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase("", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            cell.Colspan = 5;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);


                            document.Add(table);
                            /*
                            var lstDetailTrayMatter = bll_bill_stockout_detail_traymatter.GetAllQuery(w => w.stockout_detail_guid == stockoutDetailMd.guid);
                            if (lstDetailTrayMatter != null && lstDetailTrayMatter.Count > 0)
                            {
                                //》》》》》》》》》》》》明细
                                table = new PdfPTable(5);
                                table.TotalWidth = 550;
                                table.LockedWidth = true;

                                //明细>>>

                                //明细
                                cell = new PdfPCell(new Phrase("托盘物料明细>>", fontChinese_8));
                                cell.Colspan = 5;
                                cell.PaddingBottom = 6;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase("托盘条码", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("数量", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("生产日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("有效日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("检验状态", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);

                                foreach (var item in lstDetailTrayMatter)
                                {

                                    cell = new PdfPCell(new Phrase(item.traybarcode, fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.number.ToString(), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.produce_date == null ? "" : item.produce_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.effective_date == null ? "" : item.effective_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(item.test_status), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                }
                                cell = new PdfPCell(new Phrase(" ", fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Colspan = 5;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);


                                cell = new PdfPCell(new Phrase("总计：", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(lstDetailTrayMatter.Sum(w => w.number).ToString(), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Border = Rectangle.BOTTOM_BORDER;

                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(" ", fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Colspan = 3;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);

                                document.Add(table);
                            }
                            */
                            table = new PdfPTable(8);
                            table.TotalWidth = 550;
                            table.LockedWidth = true;


                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);


                            //显示签名。。。。。。。。
                            str = "出库员(签字)：______________            复核员(签字)：______________";
                            cell = new PdfPCell(new Phrase(str, fontChinese_bold));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                            document.Add(table);
                        }

                    }
                    
                    document.NewPage();
                    document.Close();
                    #endregion
                    //System.IO.File.Delete(filePath);

                    return File(ms.ToArray(), "application/pdf", LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type) + "单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                    // return View("Index");
                }
                else
                {//非抽样出库
                    var ms = new MemoryStream();
                    #region CreatePDF
                    Document document = new Document(PageSize.A4, 5f, 5f, 30f, 0f);
                    //Document document = new Document(PageSize.A4.Rotate(), 0f, 0f, 10f, 0f);(A4纸横线打印)
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    PdfPTable table = new PdfPTable(8);
                    table.SplitLate = false;//设置当前页能放多少放多少
                    table.TotalWidth = 550;
                    table.LockedWidth = true;
                    table.SetWidths(new int[] { 40, 65, 65, 55, 60, 45, 45, 170 });
                    PdfPCell cell;
                    BaseFont bfChinese = BaseFont.CreateFont("C://WINDOWS//Fonts//simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font fontChinese_11 = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_10 = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_bold = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese_8 = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));
                    //黑体
                    BaseFont bf_ht = BaseFont.CreateFont("C://WINDOWS//Fonts//simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font ht_7 = new iTextSharp.text.Font(bf_ht, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));

                    cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type) + "单", fontChinese_11));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    //cell = new PdfPCell(new Phrase("定金单", fontChinese_10));
                    //cell.Colspan = 8;
                    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell.Border = Rectangle.NO_BORDER;
                    //table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);
                    if (exp_date != null)
                    {
                        cell = new PdfPCell(new Phrase("打印日期：" + exp_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase("打印日期：" + DateTime.Now.ToString("yyyy/MM/dd"), fontChinese));
                    }
                    cell.Colspan = 2;
                    cell.PaddingBottom = 6;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    string str = "";// "客户： 电话：卡号";
                                    //cell = new PdfPCell(new Phrase(str, fontChinese));
                                    //cell.Colspan = 6;
                                    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    //cell.Border = Rectangle.NO_BORDER;
                                    //cell.Padding = 5;
                                    //cell.PaddingLeft = 0;
                                    //table.AddCell(cell);
                                    //str = "出货：";
                                    //cell = new PdfPCell(new Phrase(str, fontChinese));
                                    //cell.Colspan = 2;
                                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    //cell.Border = Rectangle.NO_BORDER;
                                    //cell.Padding = 5;
                                    //table.AddCell(cell);

                    document.Add(table);

                    // table.SetWidths(new int[] { 40, 70, 65, 55, 60, 45, 45, 165 });

                    if (stockOutMd != null && stockOutMd.guid != Guid.Empty)
                    {
                        var stockoutDetailMd = bll_bill_stockout_detail.GetFirstDefault(w => w.stockout_guid == stockOutMd.guid);
                        if (stockoutDetailMd != null && stockoutDetailMd.guid != Guid.Empty)
                        {
                            table = new PdfPTable(8);
                            table.TotalWidth = 550;
                            table.LockedWidth = true;

                            cell = new PdfPCell(new Phrase("出库单号", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.odd_numbers, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("出库日期", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.out_date.ToString("yyyy/MM/dd"), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("出库类型", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            cell = new PdfPCell(new Phrase("批次", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.lot_number, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);


                            cell = new PdfPCell(new Phrase("物料码", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.matter_code, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);


                            cell = new PdfPCell(new Phrase("名称", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.matter_name, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Colspan = 3;
                            cell.Padding = 5;
                            cell.PaddingLeft = 15;
                            table.AddCell(cell);



                            cell = new PdfPCell(new Phrase("出库数量", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.out_number.ToString(), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            /*
                            cell = new PdfPCell(new Phrase("生产日期", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockoutDetailMd.producedate == null ? "" : stockinDetailMd.producedate.Value.ToString("yyyy/MM/dd"), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);



                            cell = new PdfPCell(new Phrase("有效日期", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockinDetailMd.effective_date == null ? "" : stockinDetailMd.effective_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);



                            cell = new PdfPCell(new Phrase("检验状态", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockinDetailMd.test_status), fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);*/


                            cell = new PdfPCell(new Phrase("备注", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(stockOutMd.memo, fontChinese));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            cell.PaddingLeft = 10;
                            cell.Colspan = 7;
                            table.AddCell(cell);


                            //明细
                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);


                            document.Add(table);

                            var lstDetailTrayMatter = bll_bill_stockout_detail_traymatter.GetAllQuery(w => w.stockout_detail_guid == stockoutDetailMd.guid);
                            if (lstDetailTrayMatter != null && lstDetailTrayMatter.Count > 0)
                            {
                                //》》》》》》》》》》》》明细
                                table = new PdfPTable(5);
                                table.TotalWidth = 550;
                                table.LockedWidth = true;

                                //明细>>>

                                //明细
                                cell = new PdfPCell(new Phrase("托盘物料明细>>", fontChinese_8));
                                cell.Colspan = 5;
                                cell.PaddingBottom = 6;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase("托盘条码", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("数量", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("生产日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("有效日期", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase("检验状态", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);

                                foreach (var item in lstDetailTrayMatter)
                                {

                                    cell = new PdfPCell(new Phrase(item.traybarcode, fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.number.ToString(), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.produce_date == null ? "" : item.produce_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(item.effective_date == null ? "" : item.effective_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(item.test_status), fontChinese));
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Padding = 5;
                                    table.AddCell(cell);
                                }
                                cell = new PdfPCell(new Phrase(" ", fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Colspan = 5;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);


                                cell = new PdfPCell(new Phrase("总计：", fontChinese_bold));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(lstDetailTrayMatter.Sum(w => w.number).ToString(), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Border = Rectangle.BOTTOM_BORDER;

                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(" ", fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.PaddingTop = 10;
                                cell.Colspan = 3;
                                cell.Border = Rectangle.NO_BORDER;
                                table.AddCell(cell);

                                document.Add(table);
                            }

                            table = new PdfPTable(8);
                            table.TotalWidth = 550;
                            table.LockedWidth = true;


                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(" ", fontChinese));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);


                            //显示签名。。。。。。。。
                            str = "出库员(签字)：______________            复核员(签字)：______________";
                            cell = new PdfPCell(new Phrase(str, fontChinese_bold));
                            cell.Colspan = 8;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.NO_BORDER;
                            table.AddCell(cell);
                            document.Add(table);
                        }

                    }
                    /*table = new PdfPTable(8);
                    table.TotalWidth = 550;
                    table.LockedWidth = true;
                    table.SetWidths(new int[] { 40, 70, 65, 55, 60, 45, 45, 165 });

                    cell = new PdfPCell(new Phrase("条码\n单内ID", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("名称\n款号(镶口范围)", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("证书", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("重量", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("参数", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("原价\n成品价", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("优惠", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("单内备注", fontChinese_bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    decimal total_osaled_earnest = 0M;
                    //特殊通用条码List

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 7;
                    cell.Padding = 6;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Padding = 6;
                    table.AddCell(cell);

                    //同一行文字设置不同的字体样式    
                    Phrase phrase = new Phrase();
                    phrase.Add(new Chunk("  实收金额(大写)：", fontChinese));
                    phrase.Add(new Chunk("  我自己写的文字  ", fontChinese_8));
                    cell = new PdfPCell(phrase);
                    cell.Colspan = 7;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 8;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(total_osaled_earnest.ToString("#0.00"), fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    str = "客户须知\n\n";
                    str += "1、定金单作为取货的重要凭证，请妥善保管。结单时，请同时携带定单及定单人身份证件方可取货。\n\n";
                    str += "2、定单有效期30天，请于定金单显示的出货日期起30天内结单。如未能按时结单，则视为合同自动解除，产品将不再保留；所付定金将视为违约金，不予返还。\n\n";
                    str += "3、请核对定单内容后签字确认，定制类定单在定金支付完成后流转到工厂定制，定制期间无法再更改定单内容。产品出货后如非质量问题，定单不予退换。\n\n";
                    str += "4、如过出货日期仍未收到我们的到货通知，请尽快联系我们的客服中心4008800051，查询定单具体情况。\n";
                    cell = new PdfPCell(new Phrase(str, ht_7));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    str = "    客服电话：400-880-0051\n\n";
                    str += "    专业钻石网站:www.zbird.com";
                    cell = new PdfPCell(new Phrase(str, fontChinese));
                    //cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    string SalesEmployeeNo = "12";
                    if (!string.IsNullOrWhiteSpace(SalesEmployeeNo))
                    {
                        SalesEmployeeNo = SalesEmployeeNo.TrimEnd();
                    }
                    string orecev_maker_employeeno = "";
                    if (!string.IsNullOrWhiteSpace("ddddd"))
                    {
                        orecev_maker_employeeno = "asdfsadfsafa";
                    }
                    else
                    {
                        orecev_maker_employeeno = SalesEmployeeNo;
                    }
                    str = @"营业员：" + SalesEmployeeNo + "    收银员：" + orecev_maker_employeeno + "    销售(签字)：______________    顾客(签字)：______________";
                    cell = new PdfPCell(new Phrase(str, fontChinese));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    str = @"★号代表刻爱心符号";
                    cell = new PdfPCell(new Phrase(str, fontChinese));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(" ", fontChinese));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);

                    document.Add(table);

                    //table = new PdfPTable(8);
                    //table.TotalWidth = 550;
                    //table.LockedWidth = true;

                    //cell = new PdfPCell(new Phrase("", fontChinese));
                    //cell.Colspan = 7;
                    //cell.Border = Rectangle.NO_BORDER;
                    //table.AddCell(cell);
                    ////插入Logo图    
                    //string imagePath = Server.MapPath("~/Content/ico/exclamation.png");
                    //iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                    //cell = new PdfPCell(image, true);
                    //cell.Colspan = 1;
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //cell.Border = Rectangle.NO_BORDER;
                    //table.AddCell(cell);
                    //document.Add(table);

                    //iTextSharp.text.Rectangle pageSize = document.PageSize;
                    //document.SetPageSize(pageSize);
                    */
                    document.NewPage();
                    document.Close();
                    #endregion
                    //System.IO.File.Delete(filePath);

                    return File(ms.ToArray(), "application/pdf", LTLibrary.EnumHelper.GetEnumDescription(stockOutMd.stockout_type) + "单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                    // return View("Index");
                }
            }
            return null;
        }
    }
}