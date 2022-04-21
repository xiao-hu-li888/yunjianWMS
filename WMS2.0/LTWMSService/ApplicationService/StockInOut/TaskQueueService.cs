using LTWMSEFModel.Hardware;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.StockInOut
{
    /// <summary>
    /// 任务队列处理服务（重发、强制完成、取消等等）
    /// </summary>
    public class TaskQueueService : BaseService
    {
        hdw_stacker_taskqueueBLL bll_stacker_taskqueue;
        public TaskQueueService(LTWMSEFModel.LTModel dbcontext, hdw_stacker_taskqueueBLL bll_stacker_taskqueue) : base(dbcontext)
        {
            this.bll_stacker_taskqueue = bll_stacker_taskqueue;
        }
        /// <summary>
        /// 重发任务至堆垛机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 
        public ComServiceReturn ReSendToWCS(hdw_stacker_taskqueue info)
        {
            string _mess = "";
            try
            {
                if (info != null && info.guid != Guid.Empty)
                {
                    if (info.taskstatus == WcsTaskStatus.Holding || info.taskstatus == WcsTaskStatus.IsSend || info.taskstatus == WcsTaskStatus.WriteError)
                    {
                        info.taskstatus = WcsTaskStatus.Holding;
                        info.updatedate = DateTime.Now;
                        var rtv = bll_stacker_taskqueue.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            //AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]重发任务[" + info.id + "]至WCS...托盘条码[" +
                            //    info.tray1_barcode + "]");
                            return JsonReturn(true);
                        }
                        else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                        {
                            // AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                            _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据并发异常，请重新加载数据然后再保存。";
                        }
                        else
                        {
                            //  AddJsonError("保存失败");
                            _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存失败";
                        }
                    }
                    else
                    {
                        //状态不对，因为wcs已经返回了状态，所以wcs肯定有任务
                        // AddJsonError("该任务状态下不支持重发！");
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "该任务状态下不支持重发！";
                    }
                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据库中不存在该条记录或已删除！";
                    //AddJsonError("数据库中不存在该条记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存数据出错！异常：" + ex.Message;
                // AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonReturn(false, _mess);
        }
        /// <summary>
        /// 强制完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 
        public ComServiceReturn ForceComplete(hdw_stacker_taskqueue info)
        {
            string _mess = "";
            try
            {
                if (info != null && info.guid != Guid.Empty)
                {/*
                    //判断流程是否可以强制完成
                    int flow = LTLibrary.ConvertUtility.ToInt(info.stacker_number);
                    if (
                        (info.tasktype == WcsTaskType.StockIn || info.tasktype == WcsTaskType.MoveTo)
                        && flow < 8)
                    {//入库、移库必须要大于等于第8步才能强制完成，否则取消或重置库位
                        _mess += "当前流程状态不能强制完成";
                    }
                    else if (info.tasktype == WcsTaskType.StockOut && flow < 5)
                    {
                        _mess += "当前流程状态不能强制完成";
                    }
                    else
                    {*/
                        //////////////////////////
                        if (info.taskstatus == WcsTaskStatus.ExceptionHandling||info.taskstatus== WcsTaskStatus.ExceptionSended
                          || info.taskstatus == WcsTaskStatus.Execute|| info.taskstatus == WcsTaskStatus.ForceCompleteHandling ||
                           info.taskstatus == WcsTaskStatus.ForceCompleteSended
                           || info.taskstatus == WcsTaskStatus.IsSend || info.taskstatus == WcsTaskStatus.Pause
                        )
                        {
                            info.taskstatus = WcsTaskStatus.ForceCompleteHandling;
                            info.memo += ";人工强制完成任务";
                            info.updatedate = DateTime.Now;
                            var rtv = bll_stacker_taskqueue.Update(info);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                //AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]重发任务[" + info.id + "]至WCS...托盘条码[" +
                                //    info.tray1_barcode + "]");
                                return JsonReturn(true);
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                // AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据并发异常，请重新加载数据然后再保存。";
                            }
                            else
                            {
                                //  AddJsonError("保存失败");
                                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存失败";
                            }
                        }
                        else
                        {
                            // MessageBox.Show("错误的状态！", "错误");
                            _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "错误的状态！";
                        }
                  //**  }
                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据库中不存在该条记录或已删除！";
                    //AddJsonError("数据库中不存在该条记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存数据出错！异常：" + ex.Message;
                // AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonReturn(false, _mess);
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 
        public ComServiceReturn TaskCancel(hdw_stacker_taskqueue info)
        {
            string _mess = "";
            try
            {
                if (info != null && info.guid != Guid.Empty)
                {
                    info.taskstatus = WcsTaskStatus.CancelHandling;
                    info.memo += ";人工取消任务";
                    info.updatedate = DateTime.Now;
                    var rtv = bll_stacker_taskqueue.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        //AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]重发任务[" + info.id + "]至WCS...托盘条码[" +
                        //    info.tray1_barcode + "]");
                        return JsonReturn(true);
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        // AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据并发异常，请重新加载数据然后再保存。";
                    }
                    else
                    {
                        //  AddJsonError("保存失败");
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存失败";
                    }
                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据库中不存在该条记录或已删除！";
                    //AddJsonError("数据库中不存在该条记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存数据出错！异常：" + ex.Message;
                // AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonReturn(false, _mess);
        }
        /// <summary>
        /// 移库异常出库（锁定起点-终点库位）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ComServiceReturn TaskMoveTrayOut(hdw_stacker_taskqueue info)
        {
            string _mess = "";
            try
            {
                if (info != null && info.guid != Guid.Empty)
                {
                    info.taskstatus = WcsTaskStatus.ExceptionHandling;
                    info.memo += ";人工修改任务状态【移库异常】";
                    //info.memo += ";移库异常，人工操作移库异常出库。锁定起点【" + info.src_shelfunits_pos + "】终点【" +
                    //    info.dest_shelfunits_pos + "】库位。";
                    info.updatedate = DateTime.Now;
                    var rtv = bll_stacker_taskqueue.Update(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        //AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]重发任务[" + info.id + "]至WCS...托盘条码[" +
                        //    info.tray1_barcode + "]");  
                        return JsonReturn(true);
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        // AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据并发异常，请重新加载数据然后再保存。";
                    }
                    else
                    {
                        //  AddJsonError("保存失败");
                        _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存失败";
                    }

                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据库中不存在该条记录或已删除！";
                    //AddJsonError("数据库中不存在该条记录或已删除！");
                }
            }
            catch (Exception ex)
            {
                _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "保存数据出错！异常：" + ex.Message;
                // AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonReturn(false, _mess);
        }
    }
}
