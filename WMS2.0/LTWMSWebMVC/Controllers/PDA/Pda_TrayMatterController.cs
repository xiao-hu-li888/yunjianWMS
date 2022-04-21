using LTWMSWebMVC.Models.Pda;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LTWMSService.Warehouse;
using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Models;
using LTWMSWebMVC.Areas.BasicData.Data;
using System.Web;
using LTWMSService.Bills;
using LTWMSService.Hardware;
using LTWMSService.Basic;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Bills;
using LTWMSService.Stock;

namespace LTWMSWebMVC.Controllers.PDA
{
    /// <summary>
    /// 托盘电池条码绑定控制器
    /// </summary>
    public class Pda_TrayMatterController : ApiBaseController
    {
        LTWMSService.ApplicationService.StockInOut.StockOutService stockoutsrv;
        wh_trayBLL bll_wh_tray;
        wh_traymatterBLL bll_wh_traymatter;
        //     wh_traymatter_logBLL bll_wh_traymatter_log;
        bill_stockinBLL bll_bill_stockin;
        //  hdw_agv_task_mainBLL bll_hdw_agv_task_main;
        //   hdw_agv_taskqueueBLL bll_hdw_agv_taskqueue;
        sys_control_dicBLL bll_sys_control_dic;
        wh_service_statusBLL bll_wh_service_status;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        sys_table_idBLL bll_sys_table_id;
        wh_shelfunitsBLL bll_shelfunits;
        wh_shelvesBLL bll_shelves;
        bill_stockoutBLL bll_bill_stockout;
        bill_stockout_detailBLL bll_bill_stockout_detail;
        bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter;
        sys_number_ruleBLL bll_sys_number_rule;
        stk_matterBLL bll_stk_matter;
        public Pda_TrayMatterController(wh_trayBLL bll_wh_tray, wh_traymatterBLL bll_wh_traymatter
            , bill_stockinBLL bll_bill_stockin, sys_control_dicBLL bll_sys_control_dic, wh_service_statusBLL bll_wh_service_status
            , hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue, sys_table_idBLL bll_sys_table_id, wh_shelfunitsBLL bll_shelfunits, wh_shelvesBLL bll_shelves
            , bill_stockoutBLL bll_bill_stockout, bill_stockout_detailBLL bll_bill_stockout_detail
            , bill_stockout_detail_traymatterBLL bll_bill_stockout_detail_traymatter, sys_number_ruleBLL bll_sys_number_rule,
            stk_matterBLL bll_stk_matter, LTWMSService.ApplicationService.StockInOut.StockOutService stockoutsrv)
        {
            this.bll_stk_matter = bll_stk_matter;
            this.bll_sys_number_rule = bll_sys_number_rule;
            this.bll_bill_stockout = bll_bill_stockout;
            this.bll_bill_stockout_detail = bll_bill_stockout_detail;
            this.bll_bill_stockout_detail_traymatter = bll_bill_stockout_detail_traymatter;

            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_traymatter = bll_wh_traymatter;
            //this.bll_wh_traymatter_log = bll_wh_traymatter_log;
            this.bll_bill_stockin = bll_bill_stockin;
            //  this.bll_hdw_agv_task_main = bll_hdw_agv_task_main;
            // this.bll_hdw_agv_taskqueue = bll_hdw_agv_taskqueue;
            this.bll_sys_control_dic = bll_sys_control_dic;
            this.bll_wh_service_status = bll_wh_service_status;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_sys_table_id = bll_sys_table_id;
            this.bll_shelfunits = bll_shelfunits;
            this.bll_shelves = bll_shelves;
            this.stockoutsrv = stockoutsrv;
        }
        /// <summary>
        /// 保存托盘与电池条码的绑定关系（支持多物料绑定）
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SaveBind()
        {
            //StockInService.SaveBind(); 后期加上pda扫码绑定功能
            return JsonError();
            /* string _traybarcode = HttpContext.Current.Request.Form["traybarcode"];
             //string matter_barcode1 = HttpContext.Current.Request.Form["matter_barcode1"];
             //string matter_barcode2 = HttpContext.Current.Request.Form["matter_barcode2"];
             string json_data_list = HttpContext.Current.Request.Form["data_list"];
             //[{"MatterBarcode":"T01-01-01","Memo":"1","is_check_ok":true},{"MatterBarcode":"T01-01-02","Memo":"3","is_check_ok":true},{"MatterBarcode":"T01-05-04","Memo":"4","is_check_ok":true}]
             AddUserOperationLog("[PDA]收到PDA扫描条码[" + _traybarcode + "],json=>>" + json_data_list);
             MatterInfo[] dataArr = (MatterInfo[])Newtonsoft.Json.JsonConvert.DeserializeObject(json_data_list, typeof(MatterInfo[]));
             LTWMSWebMVC.Models.Pda.TrayMatterModel Model = new LTWMSWebMVC.Models.Pda.TrayMatterModel() { traybarcode = _traybarcode, data = dataArr };
             try
             {
                 if (string.IsNullOrWhiteSpace(Model.traybarcode))
                 {
                     AddJsonError("托盘条码为空");
                     return JsonError();
                 }
                 //if (string.IsNullOrWhiteSpace(Model.matter_barcode1) && string.IsNullOrWhiteSpace(Model.matter_barcode2))
                 //{
                 if (Model.data == null || Model.data.Length == 0)
                 {
                     //空托盘组盘
                     return SaveTrayBind(Model, "", false);
                     // return JsonError();
                 }
                 //如果传过来的条码一样，只保留一个
                 //if (!string.IsNullOrWhiteSpace(Model.matter_barcode1) && Model.matter_barcode1 == Model.matter_barcode2)
                 //{
                 //    Model.matter_barcode2 = "";
                 //}
                 //数据去重,电池条码为空也去除
                 List<MatterInfo> lstD = new List<MatterInfo>();
                 foreach (var item in Model.data)
                 {
                     if (!string.IsNullOrWhiteSpace(item.MatterBarcode) && !lstD.Exists(w => w.MatterBarcode == item.MatterBarcode))
                     {
                         lstD.Add(item);
                     }
                 }
                 Model.data = lstD.ToArray();
                 int _matter_count = 0;//电池条码 数量
                 int _other_count = 0;//其它条码 数量
                 //判断是否有非电池条码
                 foreach (var item in Model.data)
                 {
                     if (System.Text.RegularExpressions.Regex.IsMatch(item.MatterBarcode, regBatteryBarcode))
                     {//非电池条码
                         _matter_count++;
                     }
                     else
                     {
                         _other_count++;
                     }
                 }
                 string _order1 = string.Empty;
                 if (_other_count == 0)
                 {//只有电池条码  正常走流程
                     //判断多个电池条码是否为同一个订单 T01-01-01  
                     foreach (var item in Model.data)
                     {
                         string[] _orderArr1 = (item.MatterBarcode ?? "").Trim().Split(new string[] { "-" }
                    , StringSplitOptions.RemoveEmptyEntries);
                         if (_orderArr1 != null && _orderArr1.Length > 2)
                         {
                             if (string.IsNullOrWhiteSpace(_order1))
                             {
                                 _order1 = _orderArr1[0];
                             }
                             else if (_order1 != _orderArr1[0])
                             {//订单不一致
                                 AddJsonError("托盘绑定的电池条码[" + _order1 + "/" + _orderArr1[0] + "]不属于同一个订单！");
                                 return JsonError();
                             }
                         }
                     }
                 }
                 else
                 {//包含其它条码 
                     if (_matter_count > 0)
                     {//判断是否包含 电池条码
                         AddJsonError("物料条码中不能同时包含电池条码和其它物料条码！");
                         return JsonError();
                     }
                     else
                     {//不包含电池条码
                      // 不生成订单 等其它信息
                     }
                 }

                 //判断电池条码是否与其它托盘有绑定关系？有则提示
                 //var traymatterlist = bll_wh_traymatter.GetAllQuery(w => w.matter_barcode == Model.matter_barcode1 || w.matter_barcode == Model.matter_barcode2);
                 string[] _mArr = Model.data.Select(s => s.MatterBarcode).ToArray();
                 var traymatterlist = bll_wh_traymatter.GetAllQuery(w => _mArr.Contains(w.matter_barcode));
                 if (traymatterlist != null && traymatterlist.Count > 0)
                 {
                     bool flag = false;
                     foreach (var item in traymatterlist)
                     {
                         var _trayMd = bll_wh_tray.GetFirstDefault(w => w.guid == item.tray_guid);
                         if (_trayMd != null && _trayMd.traybarcode != Model.traybarcode)
                         {
                             flag = true;
                             AddJsonError("绑定失败，电池条码：" + item.matter_barcode + " 与托盘：" + _trayMd.traybarcode
                                 + " 已存在绑定关系！托盘在库状态：" + LTLibrary.EnumHelper.GetEnumDescription(_trayMd.status) + "");
                         }
                     }
                     if (flag)
                     {
                         return JsonError();
                     }
                 }
                 if (string.IsNullOrWhiteSpace(_order1))
                 {//非电池条码、其它物料条码 不生成对应的订单信息
                     return SaveTrayBind(Model, "", false);
                 }
                 else
                 {//电池条码 生成订单信息
                     return SaveTrayBind(Model, _order1, true);
                 }

             }
             catch (Exception ex)
             {
                 AddJsonError("保存数据出错！异常信息：" + ex.Message);
                 AddUserOperationLog("[PDA]保存托盘电池条码绑定关系失败！异常：=>>" + ex.ToString());
             }
             return JsonError();*/

        }

        public HttpResponseMessage SaveTrayBind(LTWMSWebMVC.Models.Pda.TrayMatterModel Model, string _order1, bool createorder)
        {
            bool empty = true;
            if (Model.data != null && Model.data.Length > 0)
            {//非空托盘
                empty = false;
            }
            using (var _tran = bll_wh_tray.BeginTransaction())
            {
                try
                {
                    var _trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == Model.traybarcode);
                    LTWMSEFModel.SimpleBackValue rtv1;
                    if (_trayM == null)
                    {//系统没有则新增一条记录
                        _trayM = new LTWMSEFModel.Warehouse.wh_tray();
                        _trayM.traybarcode = Model.traybarcode;
                        _trayM.createdate = DateTime.Now;
                        _trayM.createuser = "pda-user";
                        _trayM.emptypallet = empty;
                        _trayM.guid = Guid.NewGuid();
                        _trayM.state = LTWMSEFModel.EntityStatus.Normal;
                        _trayM.status = LTWMSEFModel.Warehouse.TrayStatus.OffShelf;
                        _trayM.isscan = true;
                        _trayM.scandate = bll_sys_control_dic.getServerDateTime();
                        rtv1 = bll_wh_tray.Add(_trayM);
                    }
                    else
                    {
                        _trayM.updatedate = DateTime.Now;
                        _trayM.emptypallet = empty;
                        _trayM.isscan = true;
                        _trayM.scandate = bll_sys_control_dic.getServerDateTime();
                        rtv1 = bll_wh_tray.Update(_trayM);
                    }
                    //托盘电池绑定表
                    // 删除托盘与电池条码的绑定关系
                    //bll_wh_traymatter.Delete(w => w.tray_guid == _trayM.guid);
                    var dellist = bll_wh_traymatter.GetAllQuery(w => w.tray_guid == _trayM.guid);
                    if (dellist != null && dellist.Count > 0)
                    {
                        foreach (var item in dellist)
                        {
                            bll_wh_traymatter.Delete(item);
                        }
                    }
                    List<wh_traymatter> lstTrayMatt = new List<wh_traymatter>();
                    foreach (var item in Model.data)
                    {
                        wh_traymatter _tray = new wh_traymatter();
                        _tray.createdate = DateTime.Now;
                        _tray.createuser = "pda-user";
                        _tray.guid = Guid.NewGuid();
                        _tray.state = LTWMSEFModel.EntityStatus.Normal;
                        _tray.tray_guid = _trayM.guid;
                        _tray.lot_number = _order1;

                        _tray.x_barcode = item.MatterBarcode;
                        //   _tray.is_check_ok = item.is_check_ok;
                        _tray.memo = item.Memo;
                        lstTrayMatt.Add(_tray);
                    }
                    //保存新数据 
                    if (lstTrayMatt.Count > 0)
                    {//添加电池与托盘的绑定关系
                        var rtv2 = bll_wh_traymatter.AddRange(lstTrayMatt);
                        if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                        {
                            if (!string.IsNullOrWhiteSpace(_order1) && createorder)
                            {//如果是电池，则创建入库单
                             //查询入库订单是否存在，不存在自动添加，以后每绑定一次修改一次订单信息 
                                var StockIn = bll_bill_stockin.GetFirstDefault(w => w.bill_property == LTWMSEFModel.Bills.BillsProperty.Battery
                                  && w.odd_numbers == _order1);
                                if (StockIn != null && StockIn.guid != Guid.Empty)
                                {//存在订单 
                                    StockIn.updatedate = DateTime.Now;
                                    StockIn.updateuser = "pda-user";
                                    StockIn.bill_status = LTWMSEFModel.Bills.BillsStatus.Finished;
                                    //查询修改电池数量
                                    StockIn.total_get = bll_wh_traymatter.GetCount(w => w.lot_number == _order1);
                                    bll_bill_stockin.Update(StockIn);
                                }
                                else
                                {//不存在订单，自动添加一条记录
                                    var billStockIn = new LTWMSEFModel.Bills.bill_stockin();
                                    billStockIn.bill_property = LTWMSEFModel.Bills.BillsProperty.Battery;
                                    billStockIn.bill_status = LTWMSEFModel.Bills.BillsStatus.Finished;
                                    billStockIn.createdate = DateTime.Now;
                                    billStockIn.createuser = "pda-user";
                                    billStockIn.from = LTWMSEFModel.Bills.BillsFrom.System;
                                    billStockIn.get_status = LTWMSEFModel.Bills.GetStatus.GetPart;
                                    billStockIn.guid = Guid.NewGuid();
                                    billStockIn.in_date = DateTime.Now;
                                    billStockIn.memo = "PDA扫码绑定自动添加订单。";
                                    billStockIn.odd_numbers = _order1;
                                    billStockIn.state = LTWMSEFModel.EntityStatus.Normal;
                                    billStockIn.stockin_type = LTWMSEFModel.Bills.StockInType.OtherIn;
                                    billStockIn.total_get = bll_wh_traymatter.GetCount(w => w.lot_number == _order1);
                                    bll_bill_stockin.Add(billStockIn);
                                }
                                //******************************************************************** 

                            }
                            _tran.Commit();
                            string[] _mArr = Model.data.Select(s => s.MatterBarcode).ToArray();
                            AddUserOperationLog("已成功保存托盘[" + _trayM.guid + "][" + _trayM.traybarcode + "],物料条码[" + string.Join(",", _mArr) + "]绑定关系","PDA");
                            return JsonSuccess();
                        }
                        else
                        {
                            _tran.Rollback();
                            AddJsonError("数据保存失败！请重试...");
                            return JsonError();
                        }
                    }
                    else
                    {
                        //空托盘组盘
                        if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                        {
                            _tran.Commit();
                            AddUserOperationLog("已成功保存托盘[" + _trayM.guid + "][" + _trayM.traybarcode + "]信息：空托盘！", "PDA");
                            return JsonSuccess();
                        }
                        else
                        {
                            _tran.Rollback();
                            AddJsonError("数据保存失败！请重试...");
                            return JsonError();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddJsonError("异常：" + ex.ToString());
                    _tran.Rollback();
                    WMSFactory.Log.v(ex.ToString());
                }
            }
            return JsonError();
        }

        /// <summary>
        /// 通过托盘条码查询对应绑定的电池条码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetMatterBarcodesByTraybarcode(string barcode)
        {
            try
            {
                TrayMatterBarcodeModel traymattModel = new TrayMatterBarcodeModel();
                traymattModel.traybarcode = barcode;
                var trayModel = bll_wh_tray.GetFirstDefault(w => w.traybarcode == barcode);
                if (trayModel != null)
                {//存在
                    var taymatterList = bll_wh_traymatter.GetAllQuery(w => w.tray_guid == trayModel.guid);
                    if (taymatterList != null && taymatterList.Count > 0)
                    {
                        traymattModel.matterbarcode = new List<string>();
                        foreach (var item in taymatterList)
                        {
                            traymattModel.matterbarcode.Add(item.x_barcode);
                        }
                    }
                }
                //托盘数据不存在，添加绑定时自动添加。
                return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = traymattModel });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 查询托盘列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyws"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTrayList(string pageindex, string pagesize, string keyws)
        {
            try
            {
                keyws = Getkeywords(keyws);
                int _indx = Convert.ToInt32(pageindex);
                int _pagesize = Convert.ToInt32(pagesize);
                int totalcount = 0;
                var trayList = bll_wh_tray.PaginationByLinq(keyws, _indx, _pagesize, out totalcount, null).Select(
                    s => MapperConfig.Mapper.Map<wh_tray, TrayModel>(s)).ToList();

                if (trayList != null && trayList.Count > 0)
                {
                    foreach (var item in trayList)
                    {
                        item.traymatterList = bll_wh_traymatter.GetAllQueryOrderby(o => o.createdate, w => w.tray_guid == item.guid, true)
                            .Select(s => MapperConfig.Mapper.Map<wh_traymatter, LTWMSWebMVC.Areas.BasicData.Data.TrayMatterModel>(s)).ToList();
                    }
                }
                return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = trayList, totalcount = totalcount });
            }
            catch (Exception ex)
            {
                WMSFactory.Log.v(ex);
                AddJsonError(ex.Message);
            }
            return JsonError();
        }
        //^[\da-zA-Z]+-[\da-zA-Z]+-[\da-zA-Z]+$
        string regBatteryBarcode = WMSFactory.Config.RegexBatteryBarcode;//"^T[\\d]+-[\\d]+-[\\d]+$";//匹配电池条码T01-01-01
        int randDiff;


        /// <summary>
        /// 清除扫码数据
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage removescandata()
        {
            try
            {
                //清除扫码数据
                //返回后清除记录。。。
                bll_sys_control_dic.SetValueByType(CommDictType.ScanResultText, "", Guid.Empty);
                bll_sys_control_dic.SetValueByType(CommDictType.ScanBatteryBarcode, "", Guid.Empty);
                return JsonSuccess();
            }
            catch (Exception ex)
            {
                AddJsonError("清除扫码数据出错！异常信息：" + ex.Message);
                AddUserOperationLog("清除扫码数据失败！异常：=>>" + ex.ToString(), "PDA");
            }
            return JsonError();
        }
        /// <summary>
        /// 放一个空托盘
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SetOffAnTray()
        {
            int _station = LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Form["station"]);
            try
            {
                //100、200 主要：300、400 出空托盘
                string rtvS = "";// bll_hdw_stacker_taskqueue.SetOffAndEmptyTray(_station);
                if (string.IsNullOrWhiteSpace(rtvS))
                {
                    AddUserOperationLog( (_station / 100) + "站台出空托盘！", "PDA");
                    return JsonSuccess();
                }
                else
                {
                    AddUserOperationLog( (_station / 100) + "站台出空托盘！" + rtvS, "PDA");
                    AddJsonError(rtvS);
                }
            }
            catch (Exception ex)
            {
                AddJsonError((_station / 100) + "站台出空托盘异常！Err：" + ex.Message);
                AddUserOperationLog((_station / 100) + "站台出空托盘异常！Err：=>>" + ex.ToString(), "PDA");
            }
            return JsonError();
        }

        /// <summary>
        /// 根据批次号查询最小数量的托盘信息，进行盘点出库
        /// </summary>
        /// <param name="lotnumber"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetTrayMatterByLotNum(string lotnumber)
        {
            try
            {
                lotnumber = (lotnumber ?? "").Trim(); 
                wh_traymatter model = bll_wh_traymatter.GetAllQueryOrderby(
                    o => o.number, w => w.lot_number == lotnumber, true).FirstOrDefault();
                if (model != null && model.guid != Guid.Empty)
                {
                    wh_traymatter trayMd = model;
                    MatterScanModel data = new MatterScanModel();
                    data.MatterBarcode = trayMd.x_barcode;
                    data.effective_date = trayMd.effective_date == null ? "" : trayMd.effective_date.Value.ToString("yyyy-MM-dd");
                    data.lot_number = trayMd.lot_number;
                    data.Matter_Name = trayMd.name_list;
                    data.number = trayMd.number;
                    data.producedate = trayMd.producedate == null ? "" : trayMd.producedate.Value.ToString("yyyy-MM-dd");
                    switch (trayMd.test_status)
                    {
                        case TestStatusEnum.None:
                            data.test_status = "待检";
                            break;
                        case TestStatusEnum.TestFail:
                            data.test_status = "不合格";
                            break;
                        case TestStatusEnum.TestOk:
                            data.test_status = "合格";
                            break;
                    }
                    data.TrayMatter_Guid = trayMd.guid;
                    //   data.is_check_ok = model.is_check_ok == true ? true : false;
                    data.Memo = trayMd.memo;
                    var _trayM = bll_wh_tray.GetFirstDefault(w => w.guid == trayMd.tray_guid);
                    if (_trayM != null && _trayM.guid != Guid.Empty)
                    {
                        data.ShelfUnit_Pos = _trayM.shelfunits_pos;
                        data.traybarcode = _trayM.traybarcode;
                    }
                    if (string.IsNullOrWhiteSpace(data.ShelfUnit_Pos))
                    {
                        data.ShelfUnit_Pos = "未上架";
                    }
                    return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = data });
                }
                else
                {//空托盘出库 
                    AddJsonError("没有查找到对应批次物料信息"); 
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
        /// 通过物料条码查找备注/是否检测等信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage getMatterInfoByCode(string barcode)
        {
            try
            {
                string _oldBarcode = barcode;
                barcode = (barcode ?? "").Trim();
                //判断是否包含-
                if (barcode.IndexOf("-") > 0)
                {//库位转托盘。。。
                    var shelfU = bll_shelfunits.GetFirstDefault(w => w.shelfunits_pos == barcode);
                    if (shelfU != null && shelfU.guid != Guid.Empty)
                    {
                        barcode = shelfU.depth1_traybarcode;
                    }
                }
                var model = bll_wh_tray.GetMatterDetailByTrayBarcode(barcode);
                if (model != null && model.Count > 0)
                {
                    wh_traymatter trayMd = model[0];
                    MatterScanModel data = new MatterScanModel();
                    data.MatterBarcode = trayMd.x_barcode;
                    data.effective_date = trayMd.effective_date == null ? "" : trayMd.effective_date.Value.ToString("yyyy-MM-dd");
                    data.lot_number = trayMd.lot_number;
                    data.Matter_Name = trayMd.name_list;
                    data.number = trayMd.number;
                    data.producedate = trayMd.producedate == null ? "" : trayMd.producedate.Value.ToString("yyyy-MM-dd");
                    switch (trayMd.test_status)
                    {
                        case TestStatusEnum.None:
                            data.test_status = "待检";
                            break;
                        case TestStatusEnum.TestFail:
                            data.test_status = "不合格";
                            break;
                        case TestStatusEnum.TestOk:
                            data.test_status = "合格";
                            break;
                    }
                    data.TrayMatter_Guid = trayMd.guid;
                    //   data.is_check_ok = model.is_check_ok == true ? true : false;
                    data.Memo = trayMd.memo;
                    var _trayM = bll_wh_tray.GetFirstDefault(w => w.guid == trayMd.tray_guid);
                    if (_trayM != null && _trayM.guid != Guid.Empty)
                    {
                        data.ShelfUnit_Pos = _trayM.shelfunits_pos;
                        data.traybarcode = _trayM.traybarcode;
                    }
                    if (string.IsNullOrWhiteSpace(data.ShelfUnit_Pos))
                    {
                        data.ShelfUnit_Pos = "未上架";
                    }
                    return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = data });
                }
                else
                {//空托盘出库
                    var trayMM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == barcode);
                    if (trayMM != null && trayMM.guid != Guid.Empty)
                    {
                        MatterScanModel data = new MatterScanModel();
                        data.MatterBarcode = "";
                        data.effective_date = null;
                        data.lot_number = "";
                        data.Matter_Name = "";
                        data.number = 0;
                        data.producedate = null;
                        data.TrayMatter_Guid = null;
                        data.Memo = "空托盘";
                        if (trayMM != null && trayMM.guid != Guid.Empty)
                        {
                            data.ShelfUnit_Pos = trayMM.shelfunits_pos;
                            data.traybarcode = trayMM.traybarcode;
                        }
                        if (string.IsNullOrWhiteSpace(data.ShelfUnit_Pos))
                        {
                            data.ShelfUnit_Pos = "未上架";
                        }
                        return ResponseJson.GetResponJson(new ResponseJson() { code = ResponseCode.success, data = data });
                    }
                    else
                    {
                        AddJsonError("条码错误或对应的库位没有托盘或托盘不在立库中");
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
        /// 通过物料条码查询对应托盘，并下架
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage OffShelfByMatterBarcode()
        {
            string _barcode = "";
            try
            {
                _barcode = HttpContext.Current.Request.Form["barcode"];
                _barcode = (_barcode ?? "").Trim();
                //判断是否包含-
                if (_barcode.IndexOf("-") > 0)
                {//库位转托盘。。。
                    var shelfU = bll_shelfunits.GetFirstDefault(w => w.shelfunits_pos == _barcode);
                    if (shelfU != null && shelfU.guid != Guid.Empty)
                    {
                        _barcode = shelfU.depth1_traybarcode;
                    }
                }
                var _trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == _barcode && w.shelfunits_guid != null);
                if (_trayM != null && _trayM.guid != Guid.Empty)
                {//通过库位guid 管理数据
                 //托盘强制出库
                    using (var _tran = bll_shelfunits.BeginTransaction())
                    {
                        var info = bll_shelfunits.GetFirstDefault(w => w.guid == _trayM.shelfunits_guid);
                        if (info != null && info.guid != Guid.Empty)
                        {
                           var rtvsrv= stockoutsrv.OffShelfByShelfUnit(info, _trayM, StockOutType.OtherOut);
                            if (rtvsrv.success)
                            {
                                _tran.Commit();
                                AddUserOperationLog("操作托盘[" + _trayM.traybarcode + "]出库已生成出库任务，待出库库位：" + info.shelfunits_pos,"PDA");
                                return JsonSuccess();
                            }
                            else
                            {
                                AddJsonError(rtvsrv.result);
                            }
                        }
                        else
                        {
                            AddJsonError("数据库中不存在对应库位信息！shelfunits_guid=" + _trayM.shelfunits_guid);
                        }
                    }

                }
                else
                {
                    if (_barcode.IndexOf("-") > 0)
                    {
                        AddJsonError("库位中不存在托盘");
                    }
                    else
                    {
                        AddJsonError("托盘不在立库中");
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("[PDA]通过托盘条码[" + _barcode + "]下架托盘异常！Err：" + ex.Message);
                AddUserOperationLog("[PDA]通过托盘条码[" + _barcode + "]下架托盘异常！Err：" + ex.Message,"PDA");
            }
            return JsonError();
        }
        /*********************/
        /// <summary>
        /// 通过批次号下架最小数据托盘,进行盘点出库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage OffShelfByLotNumbers()
        {
            string lotnumber = "";
            try
            {
                lotnumber = HttpContext.Current.Request.Form["lotnumber"];
                lotnumber = (lotnumber ?? "").Trim(); 
                wh_traymatter model = bll_wh_traymatter.GetAllQueryOrderby(
                    o => o.number, w => w.lot_number == lotnumber, true).FirstOrDefault();
                if (model != null && model.guid != Guid.Empty)
                {

                    var _trayM = bll_wh_tray.GetFirstDefault(w => w.guid == model.tray_guid && w.shelfunits_guid != null);
                    if (_trayM != null && _trayM.guid != Guid.Empty)
                    {//通过库位guid 管理数据
                     //托盘强制出库
                        using (var _tran = bll_shelfunits.BeginTransaction())
                        {
                            var info = bll_shelfunits.GetFirstDefault(w => w.guid == _trayM.shelfunits_guid);
                            if (info != null && info.guid != Guid.Empty)
                            {
                                var rtvsrv = stockoutsrv.OffShelfByShelfUnit(info, _trayM, StockOutType.CheckOut);
                                if (rtvsrv.success)
                                {
                                    _tran.Commit();
                                    AddUserOperationLog("操作托盘[" + _trayM.traybarcode + "]出库已生成出库任务，待出库库位：" + info.shelfunits_pos, "PDA");
                                    return JsonSuccess();
                                }
                                else
                                {
                                    AddJsonError(rtvsrv.result);
                                }
                            }
                            else
                            {
                                AddJsonError("数据库中不存在对应库位信息！shelfunits_guid=" + _trayM.shelfunits_guid);
                            }
                        }

                    }
                    else
                    {
                        AddJsonError("托盘不在立库中");
                    }
                }
                else
                {
                    AddJsonError("没有查找到对应批次物料信息");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("[PDA]通过批次号[" + lotnumber + "]下架最小数量托盘异常！Err：" + ex.Message);
                AddUserOperationLog("通过批次号[" + lotnumber + "]下架最小数量托盘异常！Err：" + ex.Message, "PDA");
            }
            return JsonError();
        }
    }
}
