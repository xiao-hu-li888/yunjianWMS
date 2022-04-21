using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSEFModel.Warehouse;
using LTWMSService.Bills;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Stock;
using LTWMSEFModel.Stock;

namespace LTWMSWebMVC.Areas.BasicData.Controllers
{
    public class TrayListController : BaseController
    {
        wh_trayBLL bll_wh_tray;
        wh_traymatterBLL bll_wh_traymatter;
        bill_stockinBLL bll_bill_stockin;
        stk_matterBLL bll_stk_matter;
        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        public TrayListController(wh_trayBLL bll_wh_tray, wh_traymatterBLL bll_wh_traymatter, bill_stockinBLL bll_bill_stockin,
            stk_matterBLL bll_stk_matter, wh_warehouseBLL bll_wh_warehouse, wh_warehouse_typeBLL bll_wh_warehouse_type)
        {
            this.bll_wh_tray = bll_wh_tray;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_bill_stockin = bll_bill_stockin;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_wh_warehouse = bll_wh_warehouse;
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            ListDataManager.setWareHouseGuidList2(bll_wh_warehouse, bll_wh_warehouse_type);
        }
        // GET: BasicData/TrayList
        public ActionResult Index(TraySearch Model)
        {
            int TotalSize = 0;
            var aa = bll_wh_tray.PaginationByLinq(Model.s_keywords, Model.Paging.paging_curr_page
               , Model.Paging.PageSize, out TotalSize, Model.s_status);
            if (aa != null)
            {
                Model.PageCont = aa.Select(s => MapperConfig.Mapper.Map<wh_tray, TrayModel>(s)).ToList();
            }
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            if (Model.PageCont != null && Model.PageCont.Count > 0)
            {
                foreach (var item in Model.PageCont)
                {
                    item.traymatterList = bll_wh_traymatter.GetAllQueryOrderby(o => o.createdate, w => w.tray_guid == item.guid, true)
                        .Select(s =>
                        {
                            var a = MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s);
                            a.MatterModel = MapperConfig.Mapper.Map<stk_matter, MatterModel>(bll_stk_matter.GetFirstDefault(w => w.code == a.x_barcode));
                            return a;
                        }).ToList();
                    if (item.traymatterList == null || item.traymatterList.Count == 0)
                    {
                        item.traymatterList = new List<TrayMatterModel>();
                        item.traymatterList.Add(new TrayMatterModel());
                    }
                }
            }
            return View(Model);
        }
        [HttpGet]
        public ActionResult Update(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_wh_tray.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<wh_tray, TrayModel>(model);
            /* Md.traymatterList = bll_wh_traymatter.GetAllQueryOrderby(o => o.createdate, w => w.tray_guid == Md.guid, true)
                         .Select(s =>
                         {
                             //  s.OldRowVersion = s.rowversion;
                             return MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(s);
                         }).ToList();*/
            //Md.OldRowVersion = model.rowversion;
            var TrayMatterModel= bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == Md.guid);          
            if (TrayMatterModel != null && TrayMatterModel.guid != Guid.Empty)
            {
                Md.TrayMatter = MapperConfig.Mapper.Map<wh_traymatter, TrayMatterModel>(TrayMatterModel);
                Md.OldRowVersion = TrayMatterModel.rowversion;
            }
            //if (Md.traymatterList != null && Md.traymatterList.Count > 0)
            //{
            //    for (int i = 0; i < Md.traymatterList.Count; i++)
            //    {
            //        if (i == 0)
            //        {
            //            Md.matter_barcode1 = Md.traymatterList[i].matter_barcode;
            //        }
            //        else
            //        {
            //            Md.matter_barcode2 = Md.traymatterList[i].matter_barcode;
            //        }
            //    }
            //}
            return PartialView("Add", Md);
        }
        string regBatteryBarcode = WMSFactory.Config.RegexBatteryBarcode;// "^T[\\d]+-[\\d]+-[\\d]+$";//匹配电池条码T01-01-01
        [HttpPost]
        public JsonResult Update(TrayModel Model)
        {
            //////////////////
            try
            {
                var trayMatterMd = bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == Model.guid);
                if (trayMatterMd != null && trayMatterMd.guid != Guid.Empty)
                {
                    int _oldNum = (int)trayMatterMd.number;
                    trayMatterMd.number = Model.TrayMatter.number;
                    trayMatterMd.updatedate = DateTime.Now;
                    trayMatterMd.memo += "【" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "】>>数量由" + _oldNum + "改为" + trayMatterMd.number + "。";
                    //并发控制（乐观锁）
                    trayMatterMd.OldRowVersion = Model.OldRowVersion;
                    var rtv = bll_wh_traymatter.Update(trayMatterMd);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("托盘【" + trayMatterMd.traybarcode + "】" + trayMatterMd.x_barcode + "/" + trayMatterMd.name_list
                            + ", 批次" + trayMatterMd.lot_number + " ,数量由" + _oldNum + "改为" + trayMatterMd.number, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                    {
                        AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                    }
                    else
                    {
                        AddJsonError("修改失败，请重试");
                    }
                }
                else
                {
                    AddJsonError("参数错误。托盘条码不存在或已删除");
                }
            }
            catch (Exception ex)
            {
                AddJsonError("保存数据出错！异常：" + ex.Message);
            }
            return JsonError();
            /////////////////////////
            /*  ///////之前电池条码只有两个的操作修改逻辑
              if (ModelState.IsValid)
              {
                  try
                  {
                      //判断电池条码是否有格式错误！！！
                      if (!string.IsNullOrWhiteSpace(Model.matter_barcode1) && !Regex.IsMatch(Model.matter_barcode1, regBatteryBarcode))
                      {
                          AddJsonError("电池条码[" + Model.matter_barcode1 + "]格式错误！");
                          return JsonError();
                      }
                      if (!string.IsNullOrWhiteSpace(Model.matter_barcode2) && !Regex.IsMatch(Model.matter_barcode2, regBatteryBarcode))
                      {
                          AddJsonError("电池条码[" + Model.matter_barcode2 + "]格式错误！");
                          return JsonError();
                      }
                      //如果传过来的条码一样，只保留一个
                      if (!string.IsNullOrWhiteSpace(Model.matter_barcode1) && Model.matter_barcode1 == Model.matter_barcode2)
                      {
                          Model.matter_barcode2 = "";
                      }
                      //判断两个电池条码是否为同一个订单 T01-01-01
                      string _order1 = "";
                      string _order2 = "";
                      string[] _orderArr1 = (Model.matter_barcode1 ?? "").Trim().Split(new string[] { "-" }
                      , StringSplitOptions.RemoveEmptyEntries);
                      string[] _orderArr2 = (Model.matter_barcode2 ?? "").Trim().Split(new string[] { "-" }
                      , StringSplitOptions.RemoveEmptyEntries);
                      if (_orderArr1 != null && _orderArr1.Length >= 3)
                      {
                          _order1 = _orderArr1[0];
                      }
                      if (_orderArr2 != null && _orderArr2.Length >= 3)
                      {
                          _order2 = _orderArr2[0];
                      }
                      if (!string.IsNullOrWhiteSpace(_order1) && !string.IsNullOrWhiteSpace(_order2) &&
                          _order1 != _order2)
                      {
                          AddJsonError("托盘绑定的电池条码不属于同一个订单！");
                          return JsonError();
                      }
                      //判断电池条码是否与其它托盘有绑定关系？有则提示
                      var traymatterlist = bll_wh_traymatter.GetAllQuery(w => w.x_barcode == Model.matter_barcode1 || w.x_barcode == Model.matter_barcode2);
                      if (traymatterlist != null && traymatterlist.Count > 0)
                      {
                          bool flag = false;
                          foreach (var item in traymatterlist)
                          {
                              var _trayMd = bll_wh_tray.GetFirstDefault(w => w.guid == item.tray_guid);
                              if (_trayMd != null && _trayMd.traybarcode != Model.traybarcode)
                              {
                                  flag = true;
                                  AddJsonError("绑定失败，电池条码：" + item.x_barcode + " 与托盘：" + _trayMd.traybarcode
                                      + " 已存在绑定关系！托盘在库状态：" + LTLibrary.EnumHelper.GetEnumDescription(_trayMd.status) + "");
                              }
                          }
                          if (flag)
                          {
                              return JsonError();
                          }
                      }
                      using (var _tran = bll_wh_tray.BeginTransaction())
                      {
                          try
                          {
                              var _trayM = bll_wh_tray.GetFirstDefault(w => w.guid == Model.guid);
                              //托盘记录肯定有，没有则提示数据已删除。。。，不提供托盘添加功能，扫码绑定自动添加托盘，没必要手动添加托盘信息。。。
                              if (_trayM != null && _trayM.guid != Guid.Empty)
                              {
                                  _trayM.updatedate = DateTime.Now;
                                  if (string.IsNullOrWhiteSpace(Model.matter_barcode1) &&
                                      string.IsNullOrWhiteSpace(Model.matter_barcode2))
                                  {//条码都为空 =>>空托盘
                                      _trayM.emptypallet = true;
                                  }
                                  else
                                  {//非空托盘
                                      _trayM.emptypallet = false;
                                  }
                                  //并发控制（乐观锁）
                                  _trayM.OldRowVersion = Model.OldRowVersion;
                                  LTWMSEFModel.SimpleBackValue rtv1 = bll_wh_tray.Update(_trayM);
                                  if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                                  {//修改数据成功
                                   //托盘电池绑定表
                                   // 删除托盘与电池条码的绑定关系
                                   // bll_wh_traymatter.Delete(w => w.tray_guid == _trayM.guid);
                                      var dellist = bll_wh_traymatter.GetAllQuery(w => w.tray_guid == _trayM.guid);
                                      if (dellist != null && dellist.Count > 0)
                                      {
                                          foreach (var item in dellist)
                                          {
                                              bll_wh_traymatter.Delete(item);
                                          }
                                      }
                                      List<wh_traymatter> lstTrayMatt = new List<wh_traymatter>();
                                      //保存新数据
                                      for (int i = 0; i < 2; i++)
                                      {
                                          if ((i == 0 && !string.IsNullOrWhiteSpace(Model.matter_barcode1)) ||
                                            (i == 1 && !string.IsNullOrWhiteSpace(Model.matter_barcode2)))
                                          {//1
                                              wh_traymatter _tray = new wh_traymatter();
                                              _tray.createdate = DateTime.Now;
                                              _tray.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                              _tray.guid = Guid.NewGuid();
                                              _tray.state = LTWMSEFModel.EntityStatus.Normal;
                                              _tray.tray_guid = _trayM.guid;
                                              if (i == 0)
                                              {
                                                  _tray.lot_number = _order1;
                                                  _tray.x_barcode = Model.matter_barcode1;
                                              }
                                              else
                                              {
                                                  _tray.lot_number = _order2;
                                                  _tray.x_barcode = Model.matter_barcode2;
                                              }
                                              lstTrayMatt.Add(_tray);
                                          }
                                      }
                                      if (lstTrayMatt.Count > 0)
                                      {//添加电池与托盘的绑定关系
                                          var rtv2 = bll_wh_traymatter.AddRange(lstTrayMatt);
                                          if (rtv2 == LTWMSEFModel.SimpleBackValue.True)
                                          {
                                              //查询入库订单是否存在，不存在自动添加，以后每绑定一次修改一次订单信息
                                              string _odero = string.IsNullOrWhiteSpace(_order1) ? _order2 : _order1;
                                              var StockIn = bll_bill_stockin.GetFirstDefault(w => w.bill_property == LTWMSEFModel.Bills.BillsProperty.Battery
                                                && w.odd_numbers == _odero);
                                              if (StockIn != null && StockIn.guid != Guid.Empty)
                                              {//存在订单 
                                                  StockIn.updatedate = DateTime.Now;
                                                  StockIn.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                                  //查询修改电池数量
                                                  StockIn.total_get = bll_wh_traymatter.GetCount(w => w.lot_number == _odero);
                                                  bll_bill_stockin.Update(StockIn);
                                              }
                                              else
                                              {//不存在订单，自动添加一条记录
                                                  var billStockIn = new LTWMSEFModel.Bills.bill_stockin();
                                                  billStockIn.bill_property = LTWMSEFModel.Bills.BillsProperty.Battery;
                                                  billStockIn.bill_status = LTWMSEFModel.Bills.BillsStatus.Running;
                                                  billStockIn.createdate = DateTime.Now;
                                                  billStockIn.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                                                  billStockIn.from = LTWMSEFModel.Bills.BillsFrom.System;
                                                  billStockIn.get_status = LTWMSEFModel.Bills.GetStatus.GetPart;
                                                  billStockIn.guid = Guid.NewGuid();
                                                  billStockIn.in_date = DateTime.Now;
                                                  billStockIn.memo = "界面修改电池条码自动添加订单。";
                                                  billStockIn.odd_numbers = _odero;
                                                  billStockIn.state = LTWMSEFModel.EntityStatus.Normal;
                                                  billStockIn.stockin_type = LTWMSEFModel.Bills.StockInType.OtherIn;
                                                  billStockIn.total_get = bll_wh_traymatter.GetCount(w => w.lot_number == _odero);
                                                  bll_bill_stockin.Add(billStockIn);
                                              }
                                              //******************************************************************** 
                                              _tran.Commit();
                                              AddUserOperationLog("已成功修改托盘[" + Model.traybarcode + "]与电池条码[" + Model.matter_barcode1 + "/" +
                                    Model.matter_barcode2 + "]的绑定关系", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                              return JsonSuccess();
                                          }
                                          else
                                          {
                                              _tran.Rollback();
                                              AddJsonError("保存托盘与电池条码绑定关系失败！请重试...");
                                              return JsonError();
                                          }
                                      }
                                      else
                                      {
                                          _tran.Commit();
                                          AddUserOperationLog("已成功修改托盘[" + Model.traybarcode + "]与电池条码的绑定关系==>>>电池条码为空...", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                          return JsonSuccess();
                                      }
                                  }
                                  else if (rtv1 == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                                  {//并发异常
                                      AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                                  }
                                  else
                                  {
                                      AddJsonError("保存失败");
                                  }
                              }
                              else
                              {
                                  AddJsonError("数据不存在或已删除，请关闭窗口刷新重试！");
                              }
                          }
                          catch (Exception ex)
                          {
                              AddJsonError("异常：" + ex.ToString());
                          }
                          _tran.Rollback();
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
              return JsonError();*/
        }
        [HttpPost]
        public JsonResult DeletePost(Guid guid)
        {
            try
            {/*
                var shelfObj = bll_shelves.GetFirstDefault(w => w.guid == guid);
                if (shelfObj != null)
                {
                    if (shelfObj.isinitialized == true)
                    {//初始化的数据不能删除
                        AddJsonError("货架已初始化不能删除！");
                    }
                    else
                    {
                        using (var tran = bll_shelves.BeginTransaction())
                        {
                            var rtv = bll_shelves.DeleteWidthShelfUnits(shelfObj);
                            if (rtv == LTWMSEFModel.SimpleBackValue.True)
                            {
                                AddUserOperationLog("[" + LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "]删除仓库信息guid：[" + guid + "]");
                                tran.Commit();
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
                    }
                }
                else
                {
                    AddJsonError("数据库不存在记录或已删除！");
                }*/
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }
    }
}