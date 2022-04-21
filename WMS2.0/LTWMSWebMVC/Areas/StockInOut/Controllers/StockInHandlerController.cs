using LTWMSService.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using LTWMSService.Stock;
using LTWMSService.ApplicationService.StockInOut;
using LTWMSWebMVC.Areas.StockInOut.Data;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSService.ApplicationService;
using LTWMSService.ApplicationService.WmsServer;
using LTWMSService.Hardware;
using LTWMSEFModel.Stock;
using LTWMSService.Warehouse;
using LTWMSEFModel.Warehouse;
using System.Collections;
using Newtonsoft.Json;
using LTLibrary;


namespace LTWMSWebMVC.Areas.StockInOut.Controllers
{
    public class StockInHandlerController : BaseController
    {
        string _messagebox = "";
        stk_matterBLL bll_stk_matter;
        hdw_plcBLL bll_hdw_plc;
        hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue;
        StockInService srv_StockInService;
        WCSService bll_wcsservice;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_shelvesBLL bll_wh_shelves;
        wh_trayBLL bll_wh_tray;
        sys_control_dicBLL bll_sys_control_dic;
        public StockInHandlerController(StockInService srv_StockInService, stk_matterBLL bll_stk_matter,
            WCSService bll_wcsservice, hdw_plcBLL bll_hdw_plc, hdw_stacker_taskqueueBLL bll_hdw_stacker_taskqueue,
            LTWMSService.Warehouse.wh_wcs_deviceBLL bll_wh_wcs_device, wh_wcs_srvBLL bll_wh_wcs_srv, wh_shelvesBLL bll_wh_shelves,
            wh_trayBLL bll_wh_tray, sys_control_dicBLL bll_sys_control_dic)
        {
            this.srv_StockInService = srv_StockInService;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_wcsservice = bll_wcsservice;
            this.bll_hdw_plc = bll_hdw_plc;
            this.bll_hdw_stacker_taskqueue = bll_hdw_stacker_taskqueue;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_shelves = bll_wh_shelves;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_sys_control_dic = bll_sys_control_dic;
        }
        // GET: StockInOut/StockIn
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 检查条码类型并返回
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckBarcodeType(string barcode)
        {
            var srvRtv = srv_StockInService.CheckBarcodeType(barcode);
            return Json(srvRtv);
        }
        /// <summary>
        /// 删除扫码记录
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteSessionMatterList()
        {
            HttpContext.Session["currsession-liststockin-matters"] = null;
            _messagebox = "清除成功";
            return ViewList("");
        }
        /// <summary>
        /// 修改扫码物料数量
        /// </summary>
        /// <param name="addbarcode"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ActionResult UpdateMatterNumber(string addbarcode, string number)
        {
            //addbarcode: bcode,number:t.value
            var Md = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
            if (Md != null && Md.Count > 0)
            {
                var _existObj = Md.FirstOrDefault(w => w.x_barcode == addbarcode);
                if (_existObj != null)
                {
                    int _newV = LTLibrary.ConvertUtility.ToInt(number, -1);
                    if (_newV > 0)
                    {
                        _existObj.number = _newV;
                        _messagebox = "修改数量为" + _existObj.number;
                    }
                }
            }
            return ViewList("");
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="xbarcode"></param>
        /// <param name="medit_number"></param>
        /// <param name="medit_proj_no"></param>
        /// <param name="medit_proj_name"></param>
        /// <param name="medit_cust_name"></param>
        /// <param name="medit_memo"></param>
        /// <returns></returns>
        public ActionResult SaveMatterInfo(string xbarcode, string medit_number, string medit_proj_no
          , string medit_proj_name, string medit_cust_name, string medit_memo)
        {
            var Md = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
            if (Md != null && Md.Count > 0)
            {
                var _existObj = Md.FirstOrDefault(w => w.x_barcode == xbarcode);
                if (_existObj != null)
                {
                    int _newV = LTLibrary.ConvertUtility.ToInt(medit_number, -1);
                    if (_newV > 0)
                    {
                        _existObj.number = _newV;
                    }
                    _existObj.project_no = medit_proj_no;
                    _existObj.project_name = medit_proj_name;
                    _existObj.customer_name = medit_cust_name;
                    _existObj.memo = medit_memo;
                    _messagebox = "保存成功";
                }
            }
            return ViewList("");
        }

        //$.post("@Url.Action("DeleteMatterInfoByBarcode")?r=" + Math.random(), { x_barcode: barcode }
        public ActionResult DeleteMatterInfoByBarcode(string x_barcode)
        {
            var Md = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
            if (Md != null && Md.Count > 0)
            {
                var delObj = Md.FirstOrDefault(w => w.x_barcode == x_barcode);
                if (delObj != null)
                {
                    Md.Remove(delObj);
                    _messagebox = "删除成功";
                }
            }
            return ViewList("");
        }
        /// <summary>
        /// 返回物料扫码详细列表
        /// </summary>
        /// <param name="addbarcode"></param>
        /// <returns></returns>
        public ActionResult ViewList(string addbarcode)
        {
            var Md = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
            try
            {
                if (Md == null)
                {
                    Md = new List<StockInHandlerModel>();
                }
                if (!string.IsNullOrWhiteSpace(addbarcode))
                {
                    var List = LTLibrary.ConvertUtility.ParseToList(addbarcode);
                    foreach (var mm in List)
                    {
                        var _ExistObj = Md.FirstOrDefault(w => w.x_barcode == mm);
                        if (_ExistObj != null)
                        {//只修改数量
                            _ExistObj.number++;
                            _messagebox = "修改数量为" + _ExistObj.number;
                            //   _messagebox = "已扫描";
                        }
                        else
                        {  //删除之前扫的数据，只保留最新扫的数据
                           // Md.RemoveRange(0, Md.Count);
                            var stkHandM = new StockInHandlerModel();
                            // string traybarcode = bll_sys_control_dic.GetValueByType(CommDictType.TrayBarcodeRule, Guid.Empty);
                            // 请求艾华数据
                            //var matterInfoObj = GetMatterInfoByQrbarcode(mm);
                            // var _tranM = Regex.Match(mm, traybarcode);
                            //if (matterInfoObj != null)
                            //{//托盘条码
                            //料箱条码B101... 托盘条码T101 指令C101
                            //正常简短条码：^B[\d]{3,4}$  >>BOX-101
                            //蓝天遗留条码规则：^LT-TP-[\d]+|LTZN[\d]+$
                            //$4=批次 $8=数量  $2=物料条码
                            //艾华批次号  
                            stkHandM.x_barcode = mm;// matterInfoObj.spec_id;// _tranM.Groups[6].Value.Trim();//物料条码 唯一  
                                                    //采购订单号
                                                    //    stkHandM.project_no = matterInfoObj.order_code;// _tranM.Groups[1].Value.Trim();
                                                    //供应商代码
                                                    //  stkHandM.customer_name = matterInfoObj.vendor_code;// _tranM.Groups[2].Value.Trim();
                                                    //  stkHandM.MatterModel = new MatterModel() { code = matterInfoObj.goods_id };//_tranM.Groups[3].Value.Trim()物料号
                                                    //stkHandM.memo = "供应商批号：" + _tranM.Groups[4].Value.Trim() + ",生产日期："
                                                    //    + _tranM.Groups[7].Value.Trim() + ",";

                            stkHandM.MatterModel =
                                MapperConfig.Mapper.Map<LTWMSEFModel.Stock.stk_matter, MatterModel>
                                (bll_stk_matter.GetFirstDefault(w => w.code == mm));
                            stkHandM.number = 1;// Convert.ToDecimal(_tranM.Groups[8].Value.Trim());//默认数量1
                            Md.Insert(0, stkHandM);
                            // }
                            //   Md.Add(stkHandM); 
                            _messagebox = "添加物料成功";
                        }
                    }

                }
                HttpContext.Session["currsession-liststockin-matters"] = Md;
                ViewData["message"] = _messagebox;
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.ToString();
            }
            return PartialView("ViewList", Md);
        }
        public AHMatterInfosModel GetMatterInfoByQrbarcode(string rqbarcodeStr)
        {
            //发送流水至接口系统
            string _sendjson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                shop_code = "1199",//厂别固定为1199
                location_id = "1001",//仓库固定为1001
                carton_code = "1001888888",//固定为1001加6个8       
                qrcode = rqbarcodeStr  //完整的二维码信息 
            });
            WMSFactory.Log.v("发送数据至:(" + WMSFactory.Config.AihuaAnalysisMatterURL + ")>>>条码值【" + _sendjson + "】");
            string resp = HttpRequestHelper.HttpPost(WMSFactory.Config.AihuaAnalysisMatterURL, _sendjson);
            AHMatterInfosModel respM = (AHMatterInfosModel)JsonConvert.DeserializeObject(resp, typeof(AHMatterInfosModel));
            //  AHMatterInfosModel respM = new AHMatterInfosModel() { goods_id="lh1101001", order_code="cgd123", qty=20.06M, spec_id="pc1333212", vendor_code="gys355555" };
            return respM;
        }
        public ActionResult GetRelativeViewList(string traybarcode)
        {
            var lstTrayMatter = bll_wh_tray.GetMatterDetailByTrayBarcode(traybarcode);
            if (lstTrayMatter != null && lstTrayMatter.Count > 0)
            {
                var Md = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
                if (Md == null)
                {
                    Md = new List<StockInHandlerModel>();
                }
                if (Md.Count == 0)
                {//session中不存在对应的记录
                    foreach (var item in lstTrayMatter)
                    {
                        var stkHandM = new StockInHandlerModel();
                        stkHandM.x_barcode = item.x_barcode;//物料条码 唯一
                        stkHandM.MatterModel = MapperConfig.Mapper.Map<LTWMSEFModel.Stock.stk_matter, MatterModel>(bll_stk_matter.GetFirstDefault(w => w.code == item.x_barcode));
                        stkHandM.number = item.number;//默认数量1
                        stkHandM.memo = item.memo;
                        stkHandM.project_name = item.project_name;
                        stkHandM.project_no = item.project_no;
                        stkHandM.customer_name = item.customer_name;
                        //   Md.Add(stkHandM);
                        Md.Insert(0, stkHandM);
                    }
                    HttpContext.Session["currsession-liststockin-matters"] = Md;
                }
            }
            return ViewList("");
        }
        /// <summary>
        /// 通过关键字查询物料信息（查询匹配前50条记录）
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public ActionResult SearchMatterListByKeyWords(string keywords)
        {
            keywords = (keywords ?? "").Trim();
            int TotalSize = 0;
            var aa = bll_stk_matter.Pagination(1
               , 50, out TotalSize, o => o.name,
               w => (keywords == "" || (w.code ?? "").Contains(keywords)
               || (w.code ?? "").Contains(keywords) || (w.name ?? "").Contains(keywords)
               || (w.name_pinyin ?? "").Contains(keywords) || (w.memo ?? "").Contains(keywords)
               || (w.specs ?? "").Contains(keywords) || (w.description ?? "").Contains(keywords)
                || (w.brand_name ?? "").Contains(keywords) || (w.mattertype_name ?? "").Contains(keywords)
               )
                 , true);
            var Model = aa.Select(s => MapperConfig.Mapper.Map<stk_matter, MatterModel>(s)).ToList();

            return PartialView("SearchMatterViewList", Model);
        }
        /// <summary>
        /// 保存托盘条码组盘信息，并生成入库任务
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveBindTrayMatter(string traybarcode, string cmd_code)
        {
            try
            {
               /* 艾华扫码入库需要入库任务完成后才能扫下一个托盘入库
                * 
                * //判断系统是否有未完成的入库任务
                * 
                * 
                bool existsTaskIn = bll_hdw_stacker_taskqueue.ExistsRunningTaskIn();
                if (existsTaskIn)
                {//有执行中的任务，直接返回
                 //return;
                    AddJsonError("有执行中的入库任务！请等待执行完成或操作取消任务。");
                    return JsonError();
                }
               */
                //通过入库指令 wcs+站台 100
                var matchObj = Regex.Match(cmd_code, "^--C11M(?<station>[\\d]+)D(?<wcscode>[\\d]+)--$");
                if (!matchObj.Success)
                {
                    AddJsonError("指令解析失败！");
                    return JsonError();
                }
                int WCSCode = Convert.ToInt32(matchObj.Groups["wcscode"].Value);
                int station = Convert.ToInt32(matchObj.Groups["station"].Value);
                var wcsSrvObj = bll_wh_wcs_srv.GetFirstDefault(w => w.code == WCSCode);
                if (wcsSrvObj == null || wcsSrvObj.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败22！wcs编码错误");
                    return JsonError();
                }
                Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                var wcsdevice = bll_wh_wcs_device.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.device_type == DeviceTypeEnum.Station
                            && w.wcs_srv_guid == wcsSrvObj.guid && w.number == station);
                if (wcsdevice == null || wcsdevice.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败333！站台错误");
                    return JsonError();
                }
                //只入或可入可出的站台配置才能入库，否则提示指令错误，该站台不能入库
                if (wcsdevice.station_mode == StationModeEnum.OutOnly)
                {
                    AddJsonError("站台" + station + "为只出模式不能入库");
                    return JsonError();
                }
                //开启事务
                using (var _tran = srv_StockInService.BeginTran())
                {
                    try
                    {
                        //保存托盘绑定信息
                        var lst = new List<LTWMSService.ApplicationService.Model.MatterBarcode>();
                        //组装物料详细数据
                        var MdList = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
                        if (MdList != null && MdList.Count > 0)
                        {
                            foreach (var item in MdList)
                            {
                                var MattMd = new LTWMSService.ApplicationService.Model.MatterBarcode();
                                MattMd.matter_code = item.MatterModel.code;
                                //MattMd.barcode = item.x_barcode;
                                MattMd.number = item.number;
                                MattMd.memo = item.memo;
                                MattMd.project_name = item.project_name;
                                MattMd.project_no = item.project_no;
                                MattMd.customer_name = item.customer_name;
                                MattMd.matter_name = item.MatterModel.name;
                                lst.Add(MattMd);
                            }
                        }
                        else
                        {
                            //请扫条码
                            AddJsonError("请扫条码");
                            return JsonError();
                        }
                        ///////////////////////////////
                        //var srvRtv = srv_StockInService.SaveBind(traybarcode, lst);
                        var srvRtv = srv_StockInService.SaveBind(traybarcode, lst);

                        //生成入库信息，模拟输送线扫码入库。。。。
                        //  Guid warehouseguid = GetCurrentLoginUser_WareGuid();  
                        var recvStockIn = new LTWMSService.ApplicationService.WmsServer.Model.ReceiveStockIn();
                        recvStockIn.station = station;
                        //recvStockIn.x_1_barcode = traybarcode;
                        recvStockIn.x_1_barcode = traybarcode;
                        var srvRtv2 = bll_wcsservice.ScanOnShelf(recvStockIn,
                             LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName, wcsdevice.wcs_srv_guid);
                        if (srvRtv.success && srvRtv2.success)
                        {
                            // 清除session数据
                            HttpContext.Session["currsession-liststockin-matters"] = null;
                            //AddUserOperationLog("托盘[" + traybarcode + "]绑定信息成功，即将入库...");
                            if (string.IsNullOrWhiteSpace(srvRtv2.result))
                            {
                                AddUserOperationLog("托盘[" + traybarcode + "]绑定信息成功，即将入库...", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            }
                            else
                            {//返回结果不为空
                                AddUserOperationLog(srvRtv2.result, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            }
                            //提交事务
                            _tran.Commit();
                            //return JsonSuccess();
                            string mess = "";
                            if (lst.Count == 0)
                            {
                                mess = "空托盘即将入库";
                            }
                            else
                            {
                                //mess = "物料组盘成功，即将入库";
                                if (string.IsNullOrWhiteSpace(srvRtv2.result))
                                {
                                    mess = "物料即将入库";
                                }
                                else
                                {
                                    mess = srvRtv2.result;
                                }
                            }
                            return Json(new { success = true, data = mess }); ;
                        }
                        else
                        {
                            if (srvRtv.success == false)
                            {
                                AddJsonError(srvRtv.result);
                            }
                            if (srvRtv2.success == false)
                            {
                                AddJsonError(srvRtv2.result);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        _tran.Rollback();
                        AddJsonError("保存数据出错！异常>>" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常11>>" + ex.ToString());
            }
            return JsonError();
        }
        /// <summary>
        /// 设置出库口出库准备好
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetStockOutStationReady(string cmd_code)
        {
            try
            {
                //通过入库指令 wcs+站台 100
                var matchObj = Regex.Match(cmd_code, "^--C12M(?<station>[\\d]+)D(?<wcscode>[\\d]+)--$");
                if (!matchObj.Success)
                {
                    AddJsonError("指令解析失败！");
                    return JsonError();
                }
                int WCSCode = Convert.ToInt32(matchObj.Groups["wcscode"].Value);
                int station = Convert.ToInt32(matchObj.Groups["station"].Value);
                var wcsSrvObj = bll_wh_wcs_srv.GetFirstDefault(w => w.code == WCSCode);
                if (wcsSrvObj == null || wcsSrvObj.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败22！");
                    return JsonError();
                }
                Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                var wcsdevice = bll_wh_wcs_device.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.device_type == DeviceTypeEnum.Station
                            && w.wcs_srv_guid == wcsSrvObj.guid && w.number == station);
                if (wcsdevice == null || wcsdevice.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败333！站台错误");
                    return JsonError();
                }
                //////////////////
                //通过当前仓库+设备编号
                //Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                //  var wcsdevice = bll_wh_wcs_device.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.number == 200);

                var Md = new LTWMSService.ApplicationService.WmsServer.Model.ReceiveMachineStatus();
                Md.dev_info = new List<LTWMSService.ApplicationService.WmsServer.Model.DevInfo>();
                Md.dev_info.Add(new LTWMSService.ApplicationService.WmsServer.Model.DevInfo() { dev_id = station, error_code = 0, error_msg = "", status = 1 });
                bll_wcsservice.ReceivePLCStatus(Md, wcsdevice.wcs_srv_guid);
                AddUserOperationLog("扫码设置出库准备好", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                return JsonSuccess();
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常>>" + ex.ToString());
            }
            return JsonError();
        }
        /// <summary>
        /// 请求空箱出库或最小容量料箱出库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RequestEmptyBoxOut(string cmd_code)
        {
            string stationName = "";
            try
            {
                //通过入库指令 wcs+站台 100
                var matchObj = Regex.Match(cmd_code, "^--C13M(?<station>[\\d]+)D(?<wcscode>[\\d]+)--$");
                if (!matchObj.Success)
                {
                    AddJsonError("指令解析失败！");
                    return JsonError();
                }
                int WCSCode = Convert.ToInt32(matchObj.Groups["wcscode"].Value);
                int station = Convert.ToInt32(matchObj.Groups["station"].Value);
                var wcsSrvObj = bll_wh_wcs_srv.GetFirstDefault(w => w.code == WCSCode);
                if (wcsSrvObj == null || wcsSrvObj.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败22！");
                    return JsonError();
                }
                Guid warehouseguid = GetCurrentLoginUser_WareGuid();
                var wcsdevice = bll_wh_wcs_device.GetFirstDefault(w => w.warehouse_guid == warehouseguid && w.device_type == DeviceTypeEnum.Station
                            && w.wcs_srv_guid == wcsSrvObj.guid && w.number == station);
                if (wcsdevice == null || wcsdevice.guid == Guid.Empty)
                {
                    AddJsonError("指令解析失败333！站台错误");
                    return JsonError();
                }
                stationName = wcsdevice.name;
                ////////////////////////
                //100、200 主要：300、400 出空托盘
                string rtvS = bll_hdw_stacker_taskqueue.SetOffAndEmptyTray(station, GetShelvesOfStation(station, wcsdevice.wcs_srv_guid));
                if (string.IsNullOrWhiteSpace(rtvS))
                {
                    AddUserOperationLog(wcsdevice.name + "站台出空托盘！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    return JsonSuccess();
                }
                else
                {
                    AddUserOperationLog(wcsdevice.name + "站台出空托盘！" + rtvS, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    AddJsonError(rtvS);
                }
            }
            catch (Exception ex)
            {
                AddJsonError("异常，请查看操作日志");
                // AddJsonError((_station / 100) + "站台出空托盘异常！Err：" + ex.Message);
                AddUserOperationLog(stationName + "站台出空托盘异常！Err：=>>" + ex.ToString(), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
            }
            return JsonError();
        }
        /// <summary>
        /// 站台对应的货架
        /// </summary> 
        public List<wh_shelves> GetShelvesOfStation(int station, Guid wcs_srv_guid)
        {
            Hashtable tableShelvesOfStation = HttpContext.Session["currsession-tableShelvesof-station"] as Hashtable;
            if (tableShelvesOfStation == null)
            {
                tableShelvesOfStation = new Hashtable();
            }
            if (!tableShelvesOfStation.ContainsKey(wcs_srv_guid + "-" + station))
            {//根据对应的站台 查找对应的库位
                wh_wcs_device deviceObj2 = bll_wh_wcs_device.GetFirstDefault(w => w.state == LTWMSEFModel.EntityStatus.Normal
                  && w.wcs_srv_guid == wcs_srv_guid && w.number == station && w.device_type == DeviceTypeEnum.Station);
                tableShelvesOfStation.Add(wcs_srv_guid + "-" + station, bll_wh_shelves.GetAllShelvesByStation(deviceObj2));
            }
            HttpContext.Session["currsession-tableShelvesof-station"] = tableShelvesOfStation;
            //对应起点站台可分配的库位
            List<wh_shelves> lstShelves = tableShelvesOfStation[wcs_srv_guid + "-" + station] as List<wh_shelves>;
            return lstShelves;
        }
        /// <summary>
        /// 通过条码获取物料信息
        /// </summary>
        /// <param name="x_barcode"></param>
        /// <returns></returns>
        public JsonResult GetMatterInfoByBarcode(string x_barcode)
        {
            var MdList = HttpContext.Session["currsession-liststockin-matters"] as List<StockInHandlerModel>;
            if (MdList != null && MdList.Count() > 0)
            {
                var MObj = MdList.FirstOrDefault(a => a.x_barcode == x_barcode);
                if (MObj != null)
                {
                    return Json(new { success = true, data = MObj });
                }
            }
            return Json(new { success = false });
        }
    }
}