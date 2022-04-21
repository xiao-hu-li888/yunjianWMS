using LTWMSEFModel.Bills;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.Basic;
using LTWMSService.Bills;
using LTWMSService.Hardware;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.StockInOut
{
    /// <summary>
    /// 出库逻辑（复用）
    /// </summary>
    public class StockOutService : BaseService
    {
        sys_number_ruleBLL bll_sys_number_rule;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        bill_stockoutBLL bll_bill_stockout;
        wh_traymatterBLL bll_wh_traymatter;
        stk_matterBLL bll_stk_matter;
        bill_stockout_detailBLL bll_bill_stockout_detail;
        bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter;
        wh_shelfunitsBLL bll_shelfunits;
        bill_task_tray_relationBLL bll_bill_task_tray_relation;
        public StockOutService(LTWMSEFModel.LTModel dbcontext, hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue
            , sys_number_ruleBLL bll_sys_number_rule, bill_stockoutBLL bll_bill_stockout, wh_traymatterBLL bll_wh_traymatter
            , stk_matterBLL bll_stk_matter, bill_stockout_detailBLL bll_bill_stockout_detail
            , bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter
            , wh_shelfunitsBLL bll_shelfunits, bill_task_tray_relationBLL bll_bill_task_tray_relation) : base(dbcontext)
        {
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_sys_number_rule = bll_sys_number_rule;
            this.bll_bill_stockout = bll_bill_stockout;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_bill_stockout_detail = bll_bill_stockout_detail;
            this.bll_bill_stockout_detail_traymatter = bll_bill_stockout_detail_traymatter;
            this.bll_shelfunits = bll_shelfunits;
            this.bll_bill_task_tray_relation = bll_bill_task_tray_relation;
        }
        public ComServiceReturn OffShelfByShelfUnit(wh_shelfunits info, wh_tray _trayM, StockOutType stockouttype)
        {
            string _mess = "";
            try
            {

                if (info.state == LTWMSEFModel.EntityStatus.Disabled
                    || info.special_lock_type == SpecialLockTypeEnum.StockOutLock)
                {
                    if (info.state == LTWMSEFModel.EntityStatus.Disabled)
                    {
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "操作失败！该库位已禁用。";
                    }
                    else
                    {
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "操作失败！该库位已锁定。";
                    }
                    return JsonReturn(false, _mess);
                }
                info.updatedate = DateTime.Now;
                info.updateuser = "PDA";// LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                        //强制出库  
                if (info.cellstate == ShelfCellState.Stored
                || info.locktype == ShelfLockType.ManLock)
                {//存储状态、人工锁状态 强制出库 
                    info.cellstate = ShelfCellState.WaitOut;//强制出库 
                    info.locktype = ShelfLockType.SysLock;
                    info.tray_outdatetime = DateTime.Now;
                    // 生成对应的出库任务
                    //  var rtv3 = bll_hdw_stacker_taskqueue.CreateTaskByShelfUnits(info);

                    //自动生成对应的出库单据（其它出库！！！）
                    var task = bll_hdw_stacker_taskqueue.AddTaskByShelfUnit(info, null);
                    task.sort = 999;
                    LTWMSEFModel.SimpleBackValue rtvBills = LTWMSEFModel.SimpleBackValue.False;
                    if (!_trayM.emptypallet)
                    {
                        //非空托盘生成对应的出库单据
                        // 1.. 生成出库主单据
                        var stockOutM = new bill_stockout();
                        stockOutM.bill_status = LTWMSEFModel.Bills.BillsStatus_Out.Running;
                        stockOutM.createdate = DateTime.Now;
                        stockOutM.createuser = "PDA";
                        stockOutM.from = LTWMSEFModel.Bills.BillsFrom.System;
                        stockOutM.guid = Guid.NewGuid();
                        stockOutM.odd_numbers = bll_sys_number_rule.GetBillStockOutNum();
                        //stockOutM.odd_numbers_in = model.odd_numbers_in;//管理出库单据
                        //stockOutM.destination = model.destination;
                        stockOutM.out_date = DateTime.Now;// model.out_date;
                        stockOutM.operator_user = "PDA";
                        stockOutM.state = LTWMSEFModel.EntityStatus.Normal;
                        stockOutM.stockout_type = stockouttype;/// LTWMSEFModel.Bills.StockOutType.OtherOut;// model.stockout_type;
                         // stockOutM.contact_department = model.contact_department;
                         // stockOutM.customer_name = model.customer_name;
                         // stockOutM.generated_task = model.generated_task;
                        stockOutM.get_status = LTWMSEFModel.Bills.GetStatus_Out.None;// GetStatus_Out.None;
                        stockOutM.memo = "操作pda出库，自动生成出库单";// model.memo;
                                                           //   stockOutM.project_name = model.project_name;
                                                           //   stockOutM.project_no = model.project_no;
                                                           // stockOutM.receiver = model.receiver;
                                                           //  stockOutM.total_category = model.total_category;
                                                           //   stockOutM.total_matter = model.total_matter;
                                                           //  stockOutM.total_out = model.total_out;

                        var rtvbillout = bll_bill_stockout.AddIfNotExists(stockOutM, w => w.odd_numbers);

                        // 2..  生成出库子单据 
                        var traymatterMMd = bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == _trayM.guid);
                        bill_stockout_detail outdetailMd = new bill_stockout_detail();
                        outdetailMd.createdate = DateTime.Now;
                        outdetailMd.createuser = "PDA";
                        // outdetailMd.effective_date = model.effective_date;
                        outdetailMd.guid = Guid.NewGuid();
                        outdetailMd.out_number = (int)traymatterMMd.number;// model.out_number;
                        outdetailMd.memo = "操作pda自动生成";// model.memo;
                        outdetailMd.lot_number = traymatterMMd.lot_number;// model.lot_number;

                        var matterObj = bll_stk_matter.GetFirstDefault(w => w.code == traymatterMMd.x_barcode);
                        outdetailMd.matter_guid = matterObj.guid;
                        outdetailMd.state = LTWMSEFModel.EntityStatus.Normal;
                        outdetailMd.matter_name = matterObj.name;
                        outdetailMd.matter_code = matterObj.code;
                        //   outdetailMd.producedate = model.producedate;
                        outdetailMd.stockout_guid = stockOutM.guid;

                        var rtvbillstockoutdetail = bll_bill_stockout_detail.Add(outdetailMd);

                        // 3.. 生成出库库位绑定信息
                        //  添加出库表数据 
                        bill_stockout_detail_traymatter detailTrayM = new bill_stockout_detail_traymatter();
                        detailTrayM.createdate = DateTime.Now;
                        detailTrayM.effective_date = traymatterMMd.effective_date;
                        detailTrayM.guid = Guid.NewGuid();
                        detailTrayM.lot_number = traymatterMMd.lot_number;
                        detailTrayM.matter_code = traymatterMMd.x_barcode;
                        detailTrayM.matter_name = traymatterMMd.name_list;
                        detailTrayM.number = traymatterMMd.number;
                        detailTrayM.out_shelfunits_guid = info.guid;
                        detailTrayM.out_shelfunits_pos = info.shelfunits_pos;
                        detailTrayM.out_stacker_taskqueue_guid = task.guid;
                        detailTrayM.produce_date = traymatterMMd.producedate;
                        detailTrayM.state = LTWMSEFModel.EntityStatus.Normal;
                        detailTrayM.stk_matter_guid = matterObj?.guid;
                        detailTrayM.stockout_detail_guid = outdetailMd.guid;
                        detailTrayM.stockout_guid = outdetailMd.stockout_guid;
                        detailTrayM.test_status = traymatterMMd.test_status;
                        detailTrayM.traybarcode = traymatterMMd.traybarcode;
                        detailTrayM.tray_status = TrayOutStockStatusEnum.WaitOut;
                        var rtvoutdetail_traymatter = bll_bill_stockout_detail_traymatter.Add(detailTrayM);
                        //添加盘点回库的关联关系表数据
                        LTWMSEFModel.SimpleBackValue rtvtaskrelation = LTWMSEFModel.SimpleBackValue.False;
                        if(stockOutM.stockout_type== StockOutType.CheckOut)
                        {//出库单据类型为盘点，添加关联绑定记录，回库时查询关联关系已确定回库对应的单据类型
                            var taskTrayRelationM= new bill_task_tray_relation();
                            taskTrayRelationM.bill_type = ReBillTypeEnum.StockOut;
                            taskTrayRelationM.stockout_type = StockOutType.CheckOut;
                            taskTrayRelationM.createdate = DateTime.Now;
                            taskTrayRelationM.guid = Guid.NewGuid();
                            taskTrayRelationM.odd_numbers = stockOutM.odd_numbers;
                            taskTrayRelationM.re_detail_traymatter_guid = detailTrayM.guid;
                            taskTrayRelationM.state = LTWMSEFModel.EntityStatus.Normal;                            
                            taskTrayRelationM.traybarcode= detailTrayM.traybarcode;
                            rtvtaskrelation= bll_bill_task_tray_relation.Add(taskTrayRelationM);
                        }
                        else
                        {
                            rtvtaskrelation = LTWMSEFModel.SimpleBackValue.True;
                        }
                        if (rtvbillout == LTWMSEFModel.SimpleBackValue.True && rtvbillstockoutdetail == LTWMSEFModel.SimpleBackValue.True
                            && rtvoutdetail_traymatter == LTWMSEFModel.SimpleBackValue.True&& rtvtaskrelation== LTWMSEFModel.SimpleBackValue.True)
                        {
                            task.re_detail_traymatter_guid = detailTrayM.guid; 
                            task.bills_type = BillsTypeEnum.BillsOut;
                            task.order = stockOutM.odd_numbers;

                            rtvBills = LTWMSEFModel.SimpleBackValue.True;
                        }
                    }
                    else
                    {
                        rtvBills = LTWMSEFModel.SimpleBackValue.True;
                    }
                    if (rtvBills == LTWMSEFModel.SimpleBackValue.True)
                    {
                        //添加任务
                        var rtv3 = bll_hdw_stacker_taskqueue.Add(task);
                        // var rtv3 = bll_hdw_stacker_taskqueue.CreateTaskByShelfUnitsWithSort(info, 999);
                        if (rtv3 == LTWMSEFModel.SimpleBackValue.True)
                        {
                            //并发控制（乐观锁）
                            info.OldRowVersion = info.rowversion;
                            var rtv = bll_shelfunits.Update(info);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                // AddUserOperationLog("[PDA]操作托盘[" + _trayM.traybarcode + "]出库已生成出库任务，待出库库位：" + info.shelfunits_pos);
                                return JsonReturn(true);
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                //  AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据并发异常，请重新加载数据然后再保存。";
                            }
                            else
                            {
                                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存失败";
                                //  AddJsonError("保存失败");
                            }
                        }
                        else
                        {
                            _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "添加出库任务出错，请重试！";
                            //  _tran.Rollback();
                            // AddJsonError("添加出库任务出错，请重试！");
                            //  return JsonError();
                        }
                    }
                    else
                    {
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "生成对应的出库单据失败，请重试！";
                        // AddJsonError("生成对应的出库单据失败，请重试！");
                    }
                }
                else if (info.cellstate == ShelfCellState.WaitOut)
                {//如果是出库则设置优先级
                 //将对应的出库任务优先级设置为最大 100
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "操作失败，库位为待出库状态，托盘即将出库";
                    /* var _task_out = bll_hdw_stacker_taskqueue.GetFirstDefault(w => w.src_rack == info.rack
                     && w.src_col == info.columns && w.src_row == info.rows);
                     if (_task_out != null)
                     {
                         //查询到出库任务
                         _task_out.sort = 999;
                         var _rtvtask = bll_hdw_stacker_taskqueue.Update(_task_out);
                         if (_rtvtask == LTWMSEFModel.SimpleBackValue.True)
                         {
                             // _tran.Commit();
                             return JsonReturn(true);
                         }
                         else
                         {
                             _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "修改对应的出库任务失败。。。";
                             // AddJsonError("修改对应的出库任务失败。。。");
                         }
                     }*/
                }
                else
                {
                    string _cellTypeStr = "";
                    if (info.cellstate == ShelfCellState.CanIn)
                    {
                        _cellTypeStr = "可入库";
                    }
                    else if (info.cellstate == ShelfCellState.TrayIn)
                    {
                        _cellTypeStr = "入库中";
                    }
                    else if (info.cellstate == ShelfCellState.TrayOut)
                    {
                        _cellTypeStr = "出库中";
                    }
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "该库位状态为\"" + _cellTypeStr + "\"，不能进行该操作！(人工锁状态下可以强制出库)";
                    //  _tran.Rollback();
                    //  AddJsonError("该库位状态为\"" + _cellTypeStr + "\"，不能进行该操作！(人工锁状态下可以强制出库)");
                    //return JsonError();
                }

                //  }

            }
            catch (Exception ex)
            {
                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据保存失败！请重试...>>>" + ex.ToString();
                //WMSFactory.Log.v(ex);
                //AddJsonError("异常：" + ex.ToString());
            }
            return JsonReturn(false, _mess);
        }
    }
}
