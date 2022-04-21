using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel.Warehouse;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using LTWMSService.Basic;
using LTWMSService.Stock;
using LTWMSEFModel.Stock;
using System.Data;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{

    public class TrayMatterController : BaseController
    {
        wh_traymatterBLL bll_wh_traymatter;
        wh_trayBLL bll_wh_tray;
        wh_shelfunitsBLL bll_wh_shelfunits;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        sys_control_dicBLL bll_sys_control_dic;
        stk_matterBLL bll_stk_matter;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        public TrayMatterController(wh_traymatterBLL bll_wh_traymatter, wh_trayBLL bll_wh_tray, wh_shelfunitsBLL bll_wh_shelfunits,
             hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue, sys_control_dicBLL bll_sys_control_dic, stk_matterBLL bll_stk_matter
            , wh_warehouse_typeBLL bll_wh_warehouse_type, wh_warehouseBLL bll_wh_warehouse)
        {
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_sys_control_dic = bll_sys_control_dic;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_wh_warehouse = bll_wh_warehouse;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            // ListDataManager.SetALLMatterList(bll_stk_matter);
            // ListDataManager.setWareHouseGuidList2(bll_wh_warehouse, bll_wh_warehouse_type); 
        }
        /// <summary>
        /// 导出所有数据
        /// </summary>
        /// <returns></returns>
        public FileResult ExportALLData()
        {
            /*string fileName = @"C:\Users\pzxne\Desktop\艾华WMS-双伸库\WMSERP2024\WMS2.0\LTWMSWebMVC\excel-download\202110\20211015141036016.xlsx";
            var table=LTLibrary.ExcelHelper.ExcelToDataTable(fileName);
            var lst= LTLibrary.ConvertUtility.TableToList<wh_traymatter>(table);*/
            int TotalSize = 0;
            List<wh_traymatter> aa = bll_wh_traymatter.PaginationByLinq2("", null, 1
                 , 1000000, out TotalSize, LTWMSEFModel.MatterOrderEnum.CreateDateDesc);
            string _fileName = Server.MapPath("~/") + "excel-download/" + DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            DataTable dt = new DataTable();
            //绑定表头
            dt.Columns.Add(new DataColumn() { Caption = "托盘条码", ColumnName = "traybarcode" });
            dt.Columns.Add(new DataColumn() { Caption = "物料编码", ColumnName = "x_barcode" });
            dt.Columns.Add(new DataColumn() { Caption = "名称", ColumnName = "name_list" });
            dt.Columns.Add(new DataColumn() { Caption = "批次号", ColumnName = "lot_number" });
            dt.Columns.Add(new DataColumn() { Caption = "数量", ColumnName = "number" });
            dt.Columns.Add(new DataColumn() { Caption = "入库时间", ColumnName = "createdate" });
            dt.Columns.Add(new DataColumn() { Caption = "生产日期", ColumnName = "producedate" });
            dt.Columns.Add(new DataColumn() { Caption = "有效日期", ColumnName = "effective_date" });
            dt.Columns.Add(new DataColumn() { Caption = "测试状态", ColumnName = "test_status" });
            if (aa != null && aa.Count > 0)
            {//绑定数据
                foreach (var item in aa)
                {
                    DataRow row = dt.NewRow();
                    row["x_barcode"] = item.x_barcode;
                    row["name_list"] = item.name_list;
                    row["number"] = item.number;
                    row["traybarcode"] = item.traybarcode;
                    row["createdate"] = item.createdate;
                    row["lot_number"] = item.lot_number;
                    row["producedate"] = item.producedate?.ToString("yyyy-MM-dd");
                    row["effective_date"] = item.effective_date?.ToString("yyyy-MM-dd");
                    row["test_status"] = LTLibrary.EnumHelper.GetEnumDescription(item.test_status);
                    dt.Rows.Add(row);
                }
            }
            LTLibrary.ExcelHelper.DataTableToExcel(_fileName, dt);
            return File(_fileName, "application/ms-excel", _fileName.Substring(_fileName.LastIndexOf("/") + 1));
        }
        // GET: BasicData/BatteryBarcodeList
        public ActionResult Index(TrayMatterSearch Model)
        {
            //清理旧数据
            try
            {
                //检查出库后是否存在未删除的数据，存在则删除未删除的数据
                //// bll_wh_tray.CheckOldDataAndDelete();
            }
            catch (Exception ex)
            {
                LTWMSWebMVC.WMSFactory.Log.v(ex.ToString());
            }
            //*************************************/
            ListDataManager.setWareHouseGuidListByPermission(bll_wh_warehouse, bll_wh_warehouse_type, GetLoginRole_WarehouseGuid());
            int TotalSize = 0;
            var matterList= bll_stk_matter.GetAllQuery();
            var aa = bll_wh_traymatter.PaginationByLinq2(Model.s_keywords, Model.warehouse_guid, Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize, Model.matterOrder, Model.trayInDate_begin, Model.trayInDate_end, Model.test_status);
            if (aa != null)
            {
                Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s)).ToList();
                if (Model.PageCont != null && Model.PageCont.Count > 0)
                {
                    foreach (var item in Model.PageCont)
                    {
                        //绑定托盘信息
                        item.trayModel = MapperConfig.Mapper.Map<wh_tray, TrayModel>(bll_wh_tray.GetFirstDefault(w => w.guid == item.tray_guid));
                        if (item.trayModel != null && item.trayModel.shelfunits_guid != null)
                        {
                            var _obj = bll_wh_shelfunits.GetFirstDefault(w => w.guid == item.trayModel.shelfunits_guid);
                            if (_obj != null)
                            {
                                item.trayModel.cell_state = _obj.cellstate;
                                item.warehouse_guid = _obj.warehouse_guid;
                            }
                        }
                        //绑定物料信息
                        item.MatterModel = MapperConfig.Mapper.Map<stk_matter, MatterModel>(matterList.FirstOrDefault(w => w.code == item.x_barcode));
                        if (item.MatterModel == null)
                        {
                            item.MatterModel = new MatterModel();
                        }
                    }
                }
            }
            Model.warehouse_guid = GetCurrentLoginUser_WareGuid();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        public ActionResult View(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            //ViewBag.SubmitText = "保存";
            //ViewBag.isUpdate = true;
            var model = bll_wh_traymatter.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(model);
            Md.trayModel = MapperConfig.Mapper.Map<wh_tray, TrayModel>(bll_wh_tray.GetFirstDefault(w => w.guid == Md.tray_guid));
            Md.OldRowVersion = model.rowversion;
            return PartialView(Md);

        }
        public ActionResult BatteryOut()
        {
            return PartialView(new BatteryOutModel());
        }
        [HttpPost]
        public JsonResult BatteryOut(BatteryOutModel Model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Model.Order))
                {
                    AddJsonError("订单号不能为空！");
                    return JsonError();
                }
                if (Model.Order.IndexOf("-") >= 0)
                {
                    AddJsonError("订单号不能包含符合 '-'");
                    return JsonError();
                }
                ////判断订单是否合法 T01
                if (!Regex.IsMatch(Model.Order, @"^[0-9a-zA-Z]+$"))
                {
                    AddJsonError("入库单号[" + Model.Order + "]格式错误！");
                    return JsonError();
                }

                string _keywords = Model.Order + "-";
                if (!string.IsNullOrWhiteSpace(Model.Cluster))
                {//不为空
                    if (!Regex.IsMatch(Model.Cluster, @"^[0-9a-zA-Z]+$"))
                    {
                        AddJsonError("簇[" + Model.Cluster + "]格式错误！");
                        return JsonError();
                    }
                    _keywords += Model.Cluster + "-";
                    if (!string.IsNullOrWhiteSpace(Model.Number))
                    {
                        if (!Regex.IsMatch(Model.Number, @"^[0-9a-zA-Z]+$"))
                        {
                            AddJsonError("编号[" + Model.Number + "]格式错误！");
                            return JsonError();
                        }
                        _keywords += Model.Number;
                    }
                }
                //AddJsonError(_keywords);

                int _retv = 0;
                List<LTWMSEFModel.Warehouse.wh_shelfunits> listShelfUnits = null;
                bool _is_equals = false;
                if (Model.Cluster != null && Model.Number != null)
                {//如果包含订单-簇-编号 则用相等匹配
                    _is_equals = true;//相等查找 只有一个结果
                    listShelfUnits = bll_wh_shelfunits.GetShelfUnitOutToTaskByOrderCluster(_keywords, true);
                }
                else
                {//如果不包含编号 只包含 订单- 或 订单-簇-  则用模糊匹配
                    listShelfUnits = bll_wh_shelfunits.GetShelfUnitOutToTaskByOrderCluster(_keywords, false);
                }
                //生成堆垛机出库任务。。。。。。。。。。。。。。。
                List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue> lsttaskqueue = new List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>();
                if (listShelfUnits != null && listShelfUnits.Count > 0)
                {// 按订单 簇  从小到大排序  依次出库
                    using (var tran = bll_wh_shelfunits.BeginTransaction())
                    {//开始一个事务
                        List<wh_traymatter> listTrayMatter = null;
                        if (_is_equals)
                        {
                            listTrayMatter = bll_wh_traymatter.GetAllQuery(w => w.x_barcode == _keywords && w.lot_number != null && w.lot_number != "").ToList();
                        }
                        else
                        {
                            listTrayMatter = bll_wh_traymatter.GetAllQuery(w => w.x_barcode.Contains(_keywords) && w.lot_number != null && w.lot_number != "").ToList().OrderBy(o =>
                            {//按簇排序   
                                string[] arr = (o.x_barcode ?? "").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr.Length == 3)
                                {//T03(订单)-1（簇）-1（编号）
                                    return arr[1];
                                }
                                return "";
                            }).ThenBy(o =>
                            {
                                //按编号排序
                                string[] arr = (o.x_barcode ?? "").Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr.Length == 3)
                                {//T03(订单)-1（簇）-1（编号）
                                    return arr[2];
                                }
                                return "";
                            }).ToList();
                        }
                        ////////////////////////////////////////////
                        string matterbarcodes = "";
                        //   List<LTWMSEFModel.Hardware.hdw_task_bill_inout> lsttaskinoutRe = new List<LTWMSEFModel.Hardware.hdw_task_bill_inout>(); 
                        foreach (var item in listShelfUnits)
                        {
                            var task = bll_hdw_stacker_taskqueue.AddTaskByShelfUnit(item, Model.Order);
                            if (task != null)
                            {
                                if (!string.IsNullOrWhiteSpace(task.tray1_matter_barcode1))
                                {
                                    matterbarcodes += "," + task.tray1_matter_barcode1;
                                }
                                if (!string.IsNullOrWhiteSpace(task.tray1_matter_barcode2))
                                {
                                    matterbarcodes += "," + task.tray1_matter_barcode2;
                                }
                                lsttaskqueue.Add(task);
                                //修改对应库位状态
                                item.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.WaitOut;
                                item.tray_outdatetime = DateTime.Now;

                                //对应库位有历史遗留任务/先删除历史任务再新增 
                                var DelList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut &&
                                      w.src_rack == task.src_rack
                                   && w.src_col == task.src_col
                                   && w.src_row == task.src_row);
                                if (DelList != null && DelList.Count > 0)
                                {
                                    foreach (var itemD in DelList)
                                    {
                                        bll_hdw_stacker_taskqueue.Delete(itemD);
                                    }
                                }
                            }
                        }
                        ////  _retv = lsttaskqueue.Count();
                        ///对任务队列进行排序 
                        List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue> orderListTaskQueue = new List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>();
                        foreach (var item in listTrayMatter)
                        {
                            var _objTask = lsttaskqueue.FirstOrDefault(w => w.tray1_matter_barcode1 == item.x_barcode || w.tray1_matter_barcode2 == item.x_barcode);
                            if (!orderListTaskQueue.Contains(_objTask))
                            {
                                _objTask.createdate = DateTime.Now;
                                orderListTaskQueue.Add(_objTask);
                                //休息5毫秒，为了排序
                                // System.Threading.Thread.Sleep(5);
                                Thread.Sleep(5);
                            }
                        }
                        //添加任务 
                        bll_hdw_stacker_taskqueue.AddRange(orderListTaskQueue);
                        //修改库位状态
                        bll_wh_shelfunits.Update(listShelfUnits);
                        tran.Commit();
                        AddUserOperationLog("执行批量出库>>>" + matterbarcodes, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                }
                else
                {
                    //没有可出库的任务
                    AddJsonError("没有查到可出库的电池信息。。。");
                }
                /*
                var rtv = bll_warehouse.AddIfNotExists(info, w => w.code);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]执行批量出库>>>");
                    return JsonSuccess();
                }
                else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                {
                    AddJsonError("数据库已存在仓库编号为：[" + info.code + "]的数据记录");
                }
                else
                {
                    AddJsonError("保存失败");
                }
                */
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }

        /// <summary>
        /// 根据物料托盘表 guid  将对应的物料出库，筛选过滤重复出库任务
        /// </summary>
        /// <param name="guidstr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetMatterOutList(string guidstr)
        {

            try
            {
                List<Guid> lstguids = LTLibrary.ConvertUtility.ParseToGuids(guidstr);
                if (lstguids != null && lstguids.Count > 0)
                {
                    var ListShelfOut = bll_wh_shelfunits.GetShelfUnitOutToTaskByGuids(lstguids);
                    if (ListShelfOut != null && ListShelfOut.Count > 0)
                    {//待出库的仓位信息ListShelfOut
                     // 停止所有堆垛机发送指令 
                        string _issendtostacker = bll_sys_control_dic.GetValueByType(CommDictType.SendTaskToAllStackers, Guid.Empty);
                        //暂停发送任务至堆垛机 
                        bll_sys_control_dic.SetValueByType(CommDictType.SendTaskToAllStackers, "0", Guid.Empty);
                        //休息2秒等待发送任务线程接收到停止发送指令 
                        Thread.Sleep(2000);
                        bool _flag = false;
                        using (var tran = bll_wh_shelfunits.BeginTransaction())
                        {
                            try
                            {
                                //生成堆垛机出库任务。。。。。。。。。。。。。。。
                                List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue> lsttaskqueue = new List<LTWMSEFModel.Hardware.hdw_stacker_taskqueue>();
                                foreach (var item in ListShelfOut)
                                {
                                    var task = bll_hdw_stacker_taskqueue.AddTaskByShelfUnit(item, "");
                                    if (task != null)
                                    {
                                        lsttaskqueue.Add(task);
                                        //修改对应库位状态
                                        item.cellstate = LTWMSEFModel.Warehouse.ShelfCellState.WaitOut;
                                        item.tray_outdatetime = DateTime.Now;
                                        //对应库位有历史遗留任务/先删除历史任务再新增 
                                        //var DelList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut &&
                                        //      w.src_rack == task.src_rack
                                        //   && w.src_col == task.src_col
                                        //   && w.src_row == task.src_row);
                                        var DelList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal && w.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut &&
                                           w.src_shelfunits_guid == item.guid);
                                        if (DelList != null && DelList.Count > 0)
                                        {
                                            foreach (var itemD in DelList)
                                            {
                                                bll_hdw_stacker_taskqueue.Delete(itemD);
                                            }
                                        }
                                    }
                                }
                                var rtv1 = bll_hdw_stacker_taskqueue.AddRange(lsttaskqueue);
                                //修改库位状态
                                var rtv2 = bll_wh_shelfunits.Update(ListShelfOut);
                                if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    tran.Commit();
                                    _flag = true;
                                }
                                else
                                {
                                    tran.Rollback();
                                    AddJsonError("操作失败！请重试");
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                AddJsonError("操作失败！事务回滚。>>>" + ex.ToString());
                                WMSFactory.Log.v(ex);
                            }
                        }
                        // 恢复所有堆垛机发送指令
                        bll_sys_control_dic.SetValueByType(CommDictType.SendTaskToAllStackers, _issendtostacker, Guid.Empty);
                        if (_flag)
                        {
                            AddUserOperationLog("批量操作物料出库！ 库位>>>" + string.Join(",", ListShelfOut.Select(w => w.shelfunits_pos).ToArray()), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                    }
                    else
                    {
                        AddJsonError("操作失败！没有查找到对应的可出库库位或库位已设置出库锁定！");
                    }
                }
                else
                {
                    AddJsonError("参数错误！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }
        /************************************/
        public ActionResult UpdateTestStatus()
        {
            return View(new TrayMatterModel() { guid = Guid.NewGuid() });
        }
        [HttpPost]
        public JsonResult UpdateTestStatus(TrayMatterModel model)
        {
            try
            {
                var lstTrayM = bll_wh_traymatter.GetAllQuery(w => w.lot_number == model.lot_number);
                if (lstTrayM != null && lstTrayM.Count > 0)
                {
                    string _oldtraymattercode = lstTrayM[0].x_barcode;
                    string _matterName = "";
                    if (!string.IsNullOrWhiteSpace(model.new_matter_barcode)&& lstTrayM[0].x_barcode!=model.new_matter_barcode)
                    {//查询对应批次号下的产品编码 (当前批次物料编码和新的产品编码不一致)
                        //检查物料编码是否存在
                        var stkMatterM=  bll_stk_matter.GetFirstDefault(w => w.code == model.new_matter_barcode);
                       
                        if(stkMatterM!=null&& stkMatterM.guid!=Guid.Empty)
                        {
                            _matterName= stkMatterM.name;
                        }
                        else
                        {
                            AddJsonError("物料编码"+model.new_matter_barcode+"不存在");
                            return JsonError();
                        }
                    }
                    using (var tran = bll_hdw_stacker_taskqueue.BeginTransaction())
                    {
                        LTWMSEFModel.SimpleBackValue rtvtraymatter = LTWMSEFModel.SimpleBackValue.False;
                        foreach (var item in lstTrayM)
                        {
                            item.test_status = model.test_status;
                            if (!string.IsNullOrWhiteSpace(model.new_matter_barcode)&&!string.IsNullOrWhiteSpace(_matterName))
                            {//替换原有的物料编码
                                item.x_barcode = model.new_matter_barcode;
                                item.name_list = _matterName;
                            }
                            rtvtraymatter = bll_wh_traymatter.Update(lstTrayM);
                            if (rtvtraymatter != LTWMSEFModel.SimpleBackValue.True)
                            {
                                break;
                            }
                        }
                        if (rtvtraymatter == LTWMSEFModel.SimpleBackValue.True)
                        {
                            tran.Commit();
                            string _addtext = "";
                            if (!string.IsNullOrWhiteSpace(model.new_matter_barcode))
                            {
                                 _addtext = ">>原物料编码：" + _oldtraymattercode + "，新物料编码：" + model.new_matter_barcode;
                            }
                            else
                            {
                                _addtext = ">>物料编码值为空，未修改原来的物料编码【"+ _oldtraymattercode+"】";
                            }
                            AddUserOperationLog("设置检验状态：" + LTLibrary.EnumHelper.GetEnumDescription(model.test_status)
                                + _addtext, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            return JsonSuccess();
                        }
                        else if (rtvtraymatter == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                        {
                            AddJsonError("数据并发错误，请重试！");
                        }
                        else
                        {
                            AddJsonError("修改失败，请重试！");
                        }
                    }
                }
                else
                {
                    AddJsonError("对应的批次号不存在！");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
        }

    }
}