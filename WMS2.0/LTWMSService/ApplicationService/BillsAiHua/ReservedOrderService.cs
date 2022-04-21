using LTWMSEFModel.BillsAihua;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.Basic;
using LTWMSService.BillsAihua;
using LTWMSService.Hardware;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService
{
    public class ReservedOrderService : BaseService
    {
        billah_reserved_orderBLL bll_billah_reserved_order;
        billah_reserved_order_detailBLL bll_billah_reserved_order_detail;
        sys_table_idBLL bll_sys_table_id;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        wh_shelfunitsBLL bll_wh_shelfunits;
        public ReservedOrderService(LTWMSEFModel.LTModel dbcontext, billah_reserved_orderBLL bll_billah_reserved_order,
            sys_table_idBLL bll_sys_table_id, hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue, wh_shelfunitsBLL bll_wh_shelfunits
            , billah_reserved_order_detailBLL bll_billah_reserved_order_detail) : base(dbcontext)
        {
            this.bll_billah_reserved_order = bll_billah_reserved_order;
            this.bll_sys_table_id = bll_sys_table_id;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_billah_reserved_order_detail = bll_billah_reserved_order_detail;
        }
        /// <summary>
        /// 结束预留单仅修改状态不操作任务
        /// </summary>
        /// <param name="reservOrderModel"></param>
        /// <returns></returns>
        public ComServiceReturn TerminateReserveBill(billah_reserved_order reservOrderModel)
        {
            string text = "";
            var resmodels = bll_billah_reserved_order.getAllMatterDetails(reservOrderModel);
            if (resmodels != null && resmodels.Count > 0)
            {
                foreach (var item in resmodels)
                {
                    if (item.task_status != LTWMSEFModel.TaskStatusEnum.Finished)
                    {
                        item.task_status = LTWMSEFModel.TaskStatusEnum.Finished;
                        item.memo += ">>[" + DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + "]强制结束任务";
                    }
                }
                var rtv = bll_billah_reserved_order_detail.Update(resmodels);
                if (rtv == LTWMSEFModel.SimpleBackValue.True)
                {
                    return new ComServiceReturn() { success = true, result = text };
                }
                else
                {
                    text = "任务结束保存失败";
                }
            }
            return new ComServiceReturn() { success = false, result = text };
        }
        /// <summary>
        /// 生成预留单出库任务
        /// </summary>
        /// <param name="reservOrderModel"></param>
        /// <returns></returns>
        public ComServiceReturn TrayStartOut(billah_reserved_order reservOrderModel)
        {
            string text = "";
            var resmodels = bll_billah_reserved_order.getAllMatterDetails(reservOrderModel);
            if (resmodels != null && resmodels.Count > 0)
            {
                List<string> lotnumList = resmodels.Select(s => s.spec_id).ToList();
                IQueryable<wh_shelfunits> query = from a in dbcontext.wh_shelfunits
                                                  join b in dbcontext.wh_tray on a.depth1_traybarcode equals b.traybarcode
                                                  where dbcontext.wh_traymatter.Any(w => w.tray_guid == b.guid && lotnumList.Contains(w.lot_number)) && a.state == LTWMSEFModel.EntityStatus.Normal
                                                  && a.locktype == LTWMSEFModel.Warehouse.ShelfLockType.SysLock && a.cellstate == LTWMSEFModel.Warehouse.ShelfCellState.Stored
                                                  orderby a.columns ascending, a.rows ascending
                                                  select a;
                var listShelfUnits = query.AsNoTracking().ToList(); 
                int minId = bll_sys_table_id.GetId(sys_table_idBLL.TableIdType.hdw_stacker_taskqueue, listShelfUnits.Count);
                //生成堆垛机出库任务。。。。。。。。。。。。。。。
                List<hdw_stacker_taskqueue> lsttaskqueue = new List<hdw_stacker_taskqueue>();
                foreach (var item in listShelfUnits)
                {
                    var task = bll_hdw_stacker_taskqueue.AddTaskByShelfUnit(item, item.depth1_traybarcode, minId);
                    minId++;
                    lsttaskqueue.Add(task);
                    //修改对应库位状态
                    item.cellstate = ShelfCellState.WaitOut;
                    item.tray_outdatetime = DateTime.Now;
                    //对应库位有历史遗留任务/先删除历史任务再新增 
                    var DelList = bll_hdw_stacker_taskqueue.GetAllQuery(w => w.state == LTWMSEFModel.EntityStatus.Normal
                        && (w.tasktype == WcsTaskType.StockOut || w.tasktype == WcsTaskType.MoveTo) &&
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
                    //出库单与出库之间关联 
                    /*LTEFModel.Hardware.hdw_task_bill_inout reModel = new LTEFModel.Hardware.hdw_task_bill_inout();
                     reModel.guid = Guid.NewGuid();
                     reModel.billtype = LTEFModel.Hardware.BillType.BillStockOut;
                     reModel.bill_stockinout_guid = bill_stockout_guid;
                     reModel.task_queue_guid = task.guid;
                     lsttaskinoutRe.Add(reModel);
                     */
                }
                LTWMSEFModel.SimpleBackValue rtv1 = LTWMSEFModel.SimpleBackValue.False;
                //添加任务 
                if (lsttaskqueue != null && lsttaskqueue.Count > 0)
                {
                     rtv1 = bll_hdw_stacker_taskqueue.AddRange(lsttaskqueue);
                }
                LTWMSEFModel.SimpleBackValue rtv2 = LTWMSEFModel.SimpleBackValue.False;
                //修改库位状态
                if (listShelfUnits != null && listShelfUnits.Count > 0)
                {
                    rtv2 = bll_wh_shelfunits.Update(listShelfUnits);
                }
                //修改预留单详细信息
                foreach (var item in resmodels)
                {
                    var obj = lsttaskqueue.Where(w => w.tray1_barcode == item.spec_id).FirstOrDefault();
                    if (obj != null && obj.guid != Guid.Empty)
                    {
                        item.task_guid = obj.guid;
                        item.task_status = LTWMSEFModel.TaskStatusEnum.WaitedSend;
                    }
                    else
                    {
                        item.memo = "系统不存在该批次号物料";
                    }
                }
                LTWMSEFModel.SimpleBackValue rtv3 = LTWMSEFModel.SimpleBackValue.False;
                if (resmodels != null && resmodels.Count > 0)
                {
                   rtv3 = bll_billah_reserved_order_detail.Update(resmodels);
                }
                if (listShelfUnits.Count==0||(rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True &&
                    rtv3 == LTWMSEFModel.SimpleBackValue.True))
                {
                    return new ComServiceReturn() { success = true, result = text };
                }
                else
                {
                    text = "保存数据失败";
                }

            }
            else
            {//预留单为空，强制关闭???
                return new ComServiceReturn() { success = true, result = text };
            }
            return new ComServiceReturn() { success = false, result = text };
        }
    }
}
