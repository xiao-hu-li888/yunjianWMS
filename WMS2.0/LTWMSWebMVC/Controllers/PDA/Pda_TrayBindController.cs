using LTWMSEFModel.Bills;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService;
using LTWMSService.ApplicationService.Model;
using LTWMSService.ApplicationService.StockInOut;
using LTWMSService.Bills;
using LTWMSService.Warehouse;
using LTWMSWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LTWMSWebMVC.Controllers.PDA
{
    public class Pda_TrayBindController : ApiBaseController
    {
        bill_stockinBLL bll_bill_stockin;
        bill_stockin_detailBLL bll_bill_stockin_detail;
        StockInService srv_StockInService;
        wh_shelfunitsBLL bll_wh_shelfunits;
        bill_stockin_detail_traymatterBLL bll_bill_stockin_detail_traymatter;
        bill_task_tray_relationBLL bll_bill_task_tray_relation;
        wh_trayBLL bll_wh_tray;
        wh_traymatterBLL bll_wh_traymatter;
        bill_stockoutBLL bll_bill_stockout;
        bill_stockout_detailBLL bll_bill_stockout_detail;
        bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter;
        public Pda_TrayBindController(StockInService srv_StockInService, bill_stockinBLL bll_bill_stockin
            , wh_shelfunitsBLL bll_wh_shelfunits, bill_stockin_detailBLL bll_bill_stockin_detail,
            bill_stockin_detail_traymatterBLL bll_bill_stockin_detail_traymatter, wh_traymatterBLL bll_wh_traymatter
            , bill_task_tray_relationBLL bll_bill_task_tray_relation, wh_trayBLL bll_wh_tray,
            bill_stockoutBLL bll_bill_stockout, bill_stockout_detailBLL bll_bill_stockout_detail,
            bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter)
        {
            this.bll_bill_stockin_detail = bll_bill_stockin_detail;
            this.srv_StockInService = srv_StockInService;
            this.bll_bill_stockin = bll_bill_stockin;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_bill_stockin_detail_traymatter = bll_bill_stockin_detail_traymatter;
            this.bll_bill_task_tray_relation = bll_bill_task_tray_relation;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_bill_stockout = bll_bill_stockout;
            this.bll_bill_stockout_detail = bll_bill_stockout_detail;
            this.bll_bill_stockout_detail_traymatter = bll_bill_stockout_detail_traymatter;
        }

        [HttpGet]
        public HttpResponseMessage CheckBarcode(string barcode)
        {
            try
            {
                bool _isTraybarcode = false;
                string _errorMsg = "";
                barcode = Getkeywords(barcode);
                var srvRtv = srv_StockInService.CheckBarcodeType(barcode);
                if (srvRtv.success)
                {
                    int type = LTLibrary.ConvertUtility.ToInt(srvRtv.data);
                    if (type == 1)
                    {//料箱条码
                        _isTraybarcode = true;
                        //_errorMsg = srvRtv.result;
                    }
                    else
                    {//条码错误
                        _errorMsg = "【" + barcode + "】>>" + srvRtv.result;
                    }
                }
                else
                {
                    AddJsonError("检查条码【" + barcode + "】失败，请重扫");
                }
                bill_task_tray_relation reM = null;
                if (_isTraybarcode)
                {//料箱条码，查询对应的关联表数据
                    reM = bll_bill_task_tray_relation.GetFirstDefault(w => w.traybarcode == barcode);
                }
                var trayMd = bll_wh_tray.GetFirstDefault(w => w.traybarcode == barcode);
                wh_traymatter traymatterMd = null;
                if (trayMd != null && trayMd.guid != Guid.Empty)
                {
                    traymatterMd = bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == trayMd.guid);
                }
                return ResponseJson.GetResponJson(new ResponseJson()
                {
                    code = ResponseCode.success,
                    data = new
                    {
                        isTrayBarcode = _isTraybarcode,
                        taskTrayRelation = reM,
                        trayInfo = trayMd,
                        trayMatter = traymatterMd,
                        msg = _errorMsg
                    }
                });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 获取入库订单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBillListOfRunning()
        {
            try
            {
                var ListBillIn = bll_bill_stockin.GetAllQueryOrderby(o => o.createdate, w => w.bill_status != LTWMSEFModel.Bills.BillsStatus.Finished);


                return ResponseJson.GetResponJson(new ResponseJson()
                {
                    code = ResponseCode.success,
                    data = ListBillIn
                });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 组盘+绑定入库单据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveBind()
        {
            try
            {
                string _traybarcode = HttpContext.Current.Request.Form["traybarcode"];
                //盘点条码是否合法
                _traybarcode = Getkeywords(_traybarcode);
                var srvRtv = srv_StockInService.CheckBarcodeType(_traybarcode);
                if (srvRtv.success)
                {
                    int type = LTLibrary.ConvertUtility.ToInt(srvRtv.data);
                    if (type == 1)
                    {//料箱条码
                     // _isTraybarcode = true;
                     //_errorMsg = srvRtv.result;
                    }
                    else
                    {//条码错误
                        AddJsonError("【" + _traybarcode + "】>>" + srvRtv.result);
                        return JsonError();
                    }
                }
                else
                {
                    AddJsonError("检查条码【" + _traybarcode + "】失败，请重扫");
                    return JsonError();
                }
                /**************************************/

                //0：物料，1空托盘
                int trayType = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["traytypeval"]);
                string _bill_oddnumbers = HttpContext.Current.Request.Form["bill_oddnumbers"];
                //新增入库
                if (trayType == 0 && string.IsNullOrWhiteSpace(_bill_oddnumbers))
                {//只有trayType=0 物料才和订单进行绑定，空托盘不绑定订单
                    AddJsonError("请选择订单号或新建订单");
                    return JsonError();
                }
                int _logic_inout = 0;//入库逻辑
                //料箱条码，查询对应的关联表数据
                bill_task_tray_relation tasktrayReMd = bll_bill_task_tray_relation.GetFirstDefault(w => w.traybarcode == _traybarcode);
                if (trayType == 0)
                {//0：物料
                    if (tasktrayReMd != null && tasktrayReMd.guid != Guid.Empty)
                    {
                        if (tasktrayReMd.bill_type == ReBillTypeEnum.StockIn)
                        {//之前的入库
                            if (tasktrayReMd.odd_numbers == _bill_oddnumbers)
                            {//绑定之前的订单

                            }
                            else
                            {//新绑定的订单
                             //删除旧关联的旧订单、并删除关联
                                var stockInDetailMM = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == tasktrayReMd.re_detail_traymatter_guid && w.tray_status == TrayInStockStatusEnum.TrayBinded);
                                if (stockInDetailMM != null && stockInDetailMM.guid != Guid.Empty)
                                {
                                    bll_bill_stockin_detail_traymatter.Delete(stockInDetailMM);
                                }
                                //删除关联关系
                                bll_bill_task_tray_relation.Delete(tasktrayReMd);
                            }
                            // 入库逻辑
                            _logic_inout = 0;
                        }
                        else
                        {//出库单
                            if (tasktrayReMd.stockout_type == StockOutType.SamplingOut ||
                                tasktrayReMd.stockout_type == StockOutType.CheckOut)
                            {//需要回库的关联
                             // 回库逻辑 
                                _logic_inout = 1;
                            }
                            else
                            {//直接出库的关联
                             //出库时没有生产数据
                                /***********************/
                            }
                        }

                    }
                    else
                    {
                        // 入库逻辑
                        _logic_inout = 0;
                    }
                }
                else
                {//1空托盘
                    _logic_inout = 2;
                }
                int _num = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["num"]);
                int _rack = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["rack"]);
                int _colum = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["colum"]);
                int _row = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["row"]);
                LTWMSEFModel.SimpleBackValue rtv1 = LTWMSEFModel.SimpleBackValue.False;
                using (var _tran = srv_StockInService.BeginTran())
                {
                    wh_shelfunits shelfunitM = null;
                    //指定库位 进行锁定 (空托盘入库和物料入库都可以指定库位.)
                    if (_rack > 0 && _colum > 0 && _row > 0)
                    {//排列层都大于0 则锁定库位 shelfunit_guid
                        shelfunitM = bll_wh_shelfunits.CheckAndLockPosByRackColumRow(_rack, _colum, _row, _traybarcode);
                        if (shelfunitM != null && shelfunitM.guid != Guid.Empty)
                        {
                            rtv1 = LTWMSEFModel.SimpleBackValue.True;
                        }
                        else
                        {//指定库位分配失败
                            AddJsonError("指定的库位已被占用，请指定其它库位！");
                            return JsonError();
                        }
                    }
                    else
                    {
                        //不指定排列层
                        rtv1 = LTWMSEFModel.SimpleBackValue.True;
                    }
                    if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                    {
                        wh_tray trayAddition = null;
                        if (shelfunitM != null)
                        {
                            trayAddition = new wh_tray();
                            trayAddition.dispatch_shelfunits_guid = shelfunitM.guid;
                            trayAddition.dispatch_shelfunits_pos = shelfunitM.shelfunits_pos;
                        }
                        if (_logic_inout == 0)
                        {//入库单据
                         //组盘
                            var ListMatterBarcode = new List<MatterBarcode>();
                            var stockInM = bll_bill_stockin.GetFirstDefault(w => w.odd_numbers == _bill_oddnumbers);
                            if (stockInM != null && stockInM.guid != Guid.Empty)
                            {//查询订单详细
                             //  修改入库单为进行中。。。
                                if (stockInM.bill_status == BillsStatus.None)
                                {
                                    stockInM.bill_status = BillsStatus.Running;
                                    stockInM.updatedate = DateTime.Now;
                                    stockInM.memo += "【"+DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"】>>进行中;";
                                    bll_bill_stockin.Update(stockInM);
                                }
                                bill_stockin_detail stockInDetail = bll_bill_stockin_detail.GetFirstDefault(w => w.stockin_guid == stockInM.guid);
                                if (stockInDetail != null && stockInDetail.guid != Guid.Empty)
                                {
                                    var matterM = new MatterBarcode();
                                    matterM.matter_code = stockInDetail.matter_code;
                                    matterM.matter_name = stockInDetail.name;
                                    matterM.number = _num;
                                    matterM.effective_date = stockInDetail.effective_date;
                                    matterM.lot_number = stockInDetail.lot_number;
                                    matterM.memo = stockInDetail.memo;
                                    matterM.odd_numbers_in = _bill_oddnumbers;
                                    matterM.producedate = stockInDetail.producedate;
                                    matterM.test_status = stockInDetail.test_status;

                                    ListMatterBarcode.Add(matterM);

                                    ComServiceReturn stockInSrv = srv_StockInService.SaveBind(_traybarcode, ListMatterBarcode, trayAddition);
                                    if (stockInSrv.success)
                                    {
                                        //删除旧数据
                                        var lstDetailMatter = bll_bill_stockin_detail_traymatter.GetAllQuery(w => w.stockin_detail_guid == stockInDetail.guid
                                         && w.traybarcode == _traybarcode&&w.tray_status!= TrayInStockStatusEnum.Canceled&&
                                         w.tray_status!= TrayInStockStatusEnum.Stored);
                                        if (lstDetailMatter != null && lstDetailMatter.Count > 0)
                                        {
                                            foreach (var item in lstDetailMatter)
                                            {
                                                bll_bill_stockin_detail_traymatter.Delete(item);
                                            }
                                        }
                                        //保存生成入库明细和托盘关联表
                                        var detailTrayMatterM = new bill_stockin_detail_traymatter();
                                        detailTrayMatterM.createdate = DateTime.Now;
                                        detailTrayMatterM.effective_date = stockInDetail.effective_date;
                                        detailTrayMatterM.guid = Guid.NewGuid();
                                        detailTrayMatterM.lot_number = stockInDetail.lot_number;
                                        detailTrayMatterM.matter_code = stockInDetail.matter_code;
                                        detailTrayMatterM.matter_name = stockInDetail.name;
                                        detailTrayMatterM.memo = stockInDetail.memo;
                                        detailTrayMatterM.number = _num;
                                        detailTrayMatterM.produce_date = stockInDetail.producedate;
                                        //   detailTrayMatterM.stacker_taskqueue_guid;
                                        detailTrayMatterM.state = LTWMSEFModel.EntityStatus.Normal;
                                        detailTrayMatterM.stk_matter_guid = stockInDetail.matter_guid;
                                        detailTrayMatterM.stockin_detail_guid = stockInDetail.guid;
                                        detailTrayMatterM.stockin_guid = stockInDetail.stockin_guid;
                                        detailTrayMatterM.test_status = stockInDetail.test_status;
                                        detailTrayMatterM.traybarcode = _traybarcode;
                                        detailTrayMatterM.tray_status = TrayInStockStatusEnum.TrayBinded;
                                        var rtvDt = bll_bill_stockin_detail_traymatter.Add(detailTrayMatterM);
                                        /*********************************/
                                        if (rtvDt == LTWMSEFModel.SimpleBackValue.True)
                                        {
                                            //删除关联关系
                                            var lstTaskTrayRelation = bll_bill_task_tray_relation.GetAllQuery(w => w.traybarcode == _traybarcode
                                           );
                                            if (lstTaskTrayRelation != null && lstTaskTrayRelation.Count > 0)
                                            {
                                                foreach (var item in lstTaskTrayRelation)
                                                {
                                                    var reM = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == item.re_detail_traymatter_guid);
                                                    if (reM != null && reM.guid != Guid.Empty)
                                                    {
                                                        bll_bill_stockin_detail_traymatter.Delete(reM);
                                                    }
                                                    bll_bill_task_tray_relation.Delete(item);
                                                }
                                            }
                                            //添加订单、任务、托盘关联关系
                                            var bttrM = new bill_task_tray_relation();
                                            bttrM.bill_type = ReBillTypeEnum.StockIn;
                                            bttrM.createdate = DateTime.Now;
                                            bttrM.guid = Guid.NewGuid();
                                            bttrM.odd_numbers = stockInM.odd_numbers;
                                            bttrM.re_detail_traymatter_guid = detailTrayMatterM.guid;
                                            bttrM.state = LTWMSEFModel.EntityStatus.Normal;
                                            bttrM.traybarcode = _traybarcode;

                                            var rtvreAdd = bll_bill_task_tray_relation.Add(bttrM);
                                            /**************************/
                                            if (rtvreAdd == LTWMSEFModel.SimpleBackValue.True)
                                            {
                                                _tran.Commit();
                                                return JsonSuccess();
                                            }
                                            else
                                            {
                                                AddJsonError("添加表关联关系失败。。。请重试");
                                            }
                                        }
                                        else
                                        {
                                            AddJsonError("添加入库订单明细和托盘关联绑定记录失败。");
                                        }
                                    }
                                    else
                                    {
                                        AddJsonError(stockInSrv.result);
                                    }
                                }
                                else
                                {
                                    //订单明细为空
                                    AddJsonError("[" + _bill_oddnumbers + "]对应订单明细为空！");
                                }
                            }
                            else
                            {
                                AddJsonError("订单记录不存在或已删除，请选择正确的订单号！");
                            }
                        }
                        else if (_logic_inout == 1)
                        {//查询出库单据
                         //组盘
                            var ListMatterBarcode = new List<MatterBarcode>();
                            var stockOutM = bll_bill_stockout.GetFirstDefault(w => w.odd_numbers == tasktrayReMd.odd_numbers);
                            if (stockOutM != null && stockOutM.guid != Guid.Empty)
                            {//查询订单详细
                                var stockOutDetail = bll_bill_stockout_detail_traymatter.GetFirstDefault(w => w.guid == tasktrayReMd.re_detail_traymatter_guid &&
                                w.traybarcode == _traybarcode);
                                if (stockOutDetail != null && stockOutDetail.guid != Guid.Empty)
                                {
                                    var matterM = new MatterBarcode();
                                    matterM.matter_code = stockOutDetail.matter_code;
                                    matterM.matter_name = stockOutDetail.matter_name;
                                    matterM.number = _num;
                                    matterM.effective_date = stockOutDetail.effective_date;
                                    matterM.lot_number = stockOutDetail.lot_number;
                                    matterM.memo = stockOutDetail.memo;
                                    matterM.odd_numbers_in = _bill_oddnumbers;
                                    matterM.producedate = stockOutDetail.produce_date;
                                    matterM.test_status = stockOutDetail.test_status;
                                    ListMatterBarcode.Add(matterM);

                                    ComServiceReturn stockInSrv = srv_StockInService.SaveBind(_traybarcode, ListMatterBarcode, trayAddition);
                                    if (stockInSrv.success)
                                    {
                                        _tran.Commit();
                                        AddUserOperationLog("托盘条码【" + _traybarcode + "】组盘成功！>>" + stockInSrv.result, "PDA");
                                        return JsonSuccess();
                                    }
                                    else
                                    {
                                        AddJsonError(stockInSrv.result);
                                    }
                                }
                                else
                                {
                                    //订单明细为空
                                    AddJsonError("[" + tasktrayReMd.odd_numbers + "]对应订单明细为空！");
                                }
                            }
                            else
                            {
                                AddJsonError("订单记录不存在或已删除！");
                            }
                        }
                        else if (_logic_inout == 2)
                        {
                            LTWMSEFModel.SimpleBackValue rtvdel = LTWMSEFModel.SimpleBackValue.True;
                            //空托盘组盘入库
                            if (tasktrayReMd != null && tasktrayReMd.guid != Guid.Empty)
                            {// 删除出、入库单关联关系
                                if (tasktrayReMd.bill_type == ReBillTypeEnum.StockIn)
                                {//之前的入库
                                    var stockInDetailMM = bll_bill_stockin_detail_traymatter.GetFirstDefault(w =>
                                    w.guid == tasktrayReMd.re_detail_traymatter_guid
                                    && (w.tray_status == TrayInStockStatusEnum.None || w.tray_status == TrayInStockStatusEnum.TrayBinded)
                                    && w.traybarcode == _traybarcode);
                                    if (stockInDetailMM != null && stockInDetailMM.guid != Guid.Empty)
                                    {
                                        stockInDetailMM.tray_status = TrayInStockStatusEnum.Canceled;
                                        stockInDetailMM.updatedate = DateTime.Now;
                                        stockInDetailMM.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>托盘重新组盘为空托盘，关联入库单明细自动取消";
                                        rtvdel = bll_bill_stockin_detail_traymatter.Update(stockInDetailMM);
                                    }
                                }
                                else
                                {
                                    var stockOutDetail = bll_bill_stockout_detail_traymatter.GetFirstDefault(w => w.guid == tasktrayReMd.re_detail_traymatter_guid &&
                                    w.traybarcode == _traybarcode);
                                    if (stockOutDetail != null && stockOutDetail.guid != Guid.Empty)
                                    {
                                        stockOutDetail.tray_status = TrayOutStockStatusEnum.Canceled;
                                        stockOutDetail.updatedate = DateTime.Now;
                                        stockOutDetail.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>托盘组盘为空托盘，关联出库单明细自动取消";
                                        rtvdel = bll_bill_stockout_detail_traymatter.Update(stockOutDetail);
                                    }
                                }
                                if (rtvdel == LTWMSEFModel.SimpleBackValue.True)
                                {
                                    //删除关联关系
                                    rtvdel = bll_bill_task_tray_relation.Delete(tasktrayReMd);
                                }
                            }
                            if (rtvdel == LTWMSEFModel.SimpleBackValue.True)
                            {
                                ComServiceReturn stockInSrv = srv_StockInService.SaveBind(_traybarcode, null, trayAddition);
                                if (stockInSrv.success)
                                {
                                    _tran.Commit();
                                    AddUserOperationLog("托盘条码【" + _traybarcode + "】组盘成功！>>" + stockInSrv.result, "PDA");
                                    return JsonSuccess();
                                }
                                else
                                {
                                    AddJsonError(stockInSrv.result);
                                }
                            }
                            else
                            {
                                AddJsonError("空托盘组盘，删除相关关联记录失败，请重试！");
                            }
                        }
                    }
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
        /// 根据入库单号获取
        /// </summary>
        /// <param name="odd_number"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMatterInfoByBillInOddnumber(string odd_number)
        {
            try
            {
                //   var ListBillIn = bll_bill_stockin.GetAllQueryOrderby(o => o.createdate, w => w.bill_status != LTWMSEFModel.Bills.BillsStatus.Finished);
                var lstDetails = bll_bill_stockin.getDetailsByStockInOddNumbers(odd_number);
                if (lstDetails != null && lstDetails.Count > 0)
                {//查询对应入库单的已入库数量
                    foreach (var item in lstDetails)
                    {
                        item.total_num = item.in_number;
                        item.remain_num = item.in_number - (int)
                       bll_bill_stockin_detail_traymatter.GetAllQuery(w => w.stockin_detail_guid == item.guid &&
                                                 w.tray_status != TrayInStockStatusEnum.Canceled).Sum(w => w.number);
                        //入库托盘状态不等于“入库取消”
                    }
                }
                return ResponseJson.GetResponJson(new ResponseJson()
                {
                    code = ResponseCode.success,
                    data = lstDetails
                });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 根据出库单获取。。。
        /// </summary>
        /// <param name="odd_number"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMatterInfoByBillOutOddnumber(string odd_number)
        {
            try
            {
                //   var ListBillIn = bll_bill_stockin.GetAllQueryOrderby(o => o.createdate, w => w.bill_status != LTWMSEFModel.Bills.BillsStatus.Finished);
                var lstDetails = bll_bill_stockout.getDetailsByStockOutOddNumbers(odd_number); 
                return ResponseJson.GetResponJson(new ResponseJson()
                {
                    code = ResponseCode.success,
                    data = lstDetails
                });
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