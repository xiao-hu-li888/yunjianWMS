using LTWMSWebMVC.Areas.Bills.Data;
using LTWMSEFModel.Bills;
using LTWMSService.Bills;
using System;
using System.Linq;
using System.Web.Mvc;
using LTWMSService.Basic;
using LTWMSService.Stock;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LTWMSService.Warehouse;

namespace LTWMSWebMVC.Areas.Bills.Controllers
{
    public class StockInController : BaseController
    {
        bill_stockinBLL bll_bill_stockin;
        sys_number_ruleBLL bll_sys_number_rule;
        bill_stockin_detailBLL bll_bill_stockin_detail;
        stk_matterBLL bll_stk_matter;
        bill_stockin_detail_traymatterBLL bll_bill_stockin_detail_traymatter;
        wh_traymatterBLL bll_wh_traymatter;
        wh_trayBLL bll_wh_tray;
        bill_task_tray_relationBLL bll_bill_task_tray_relation;
        public StockInController(bill_stockinBLL bll_bill_stockin, sys_number_ruleBLL bll_sys_number_rule,
            bill_stockin_detailBLL bll_bill_stockin_detail, stk_matterBLL bll_stk_matter,
            bill_stockin_detail_traymatterBLL bll_bill_stockin_detail_traymatter, wh_traymatterBLL bll_wh_traymatter,
             bill_task_tray_relationBLL bll_bill_task_tray_relation, wh_trayBLL bll_wh_tray)
        {
            this.bll_wh_tray = bll_wh_tray;
            this.bll_bill_task_tray_relation = bll_bill_task_tray_relation;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_bill_stockin = bll_bill_stockin;
            this.bll_sys_number_rule = bll_sys_number_rule;
            this.bll_bill_stockin_detail = bll_bill_stockin_detail;
            this.bll_bill_stockin_detail_traymatter = bll_bill_stockin_detail_traymatter;
            ListDataManager.SetALLMatterList(bll_stk_matter);
        }
        // GET: Bills/StockIn 
        public ActionResult Index(StockInSearch Model)
        {
            if (Request.HttpMethod == "GET")
            {
                Model.s_bill_status = LTWMSEFModel.SearchBillsStatus_In.Running;
            }
            //DateTime? beginDate = null;
            //if (Model.s_in_date_begin != null)
            //{
            //    beginDate = new DateTime(Model.s_in_date_begin.Value.Year,
            //  Model.s_in_date_begin.Value.Month, Model.s_in_date_begin.Value.Day);
            //}
            //DateTime? endDate = null;
            //if (Model.s_in_date_end != null)
            //{
            //    endDate = new DateTime(Model.s_in_date_end.Value.Year,
            //        Model.s_in_date_end.Value.Month, Model.s_in_date_end.Value.Day);
            //}
            int TotalSize = 0;
            int _id = 0;
            Model.PageCont = bll_bill_stockin.PaginationByLinq(Model.s_keywords, Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, Model.s_bill_status, Model.s_in_date_begin, Model.s_in_date_end).Select(s =>
                           {
                               _id++;
                               var Md = MapperConfig.Mapper.Map<bill_stockin, StockInModel>(s);
                               Md.Id = _id + (Model.Paging.PageSize * (Model.Paging.paging_curr_page - 1));
                               Md.List_StockInDetailModel = bll_bill_stockin_detail.GetAllQuery(w => w.stockin_guid == Md.guid)
                               .Select(s => MapperConfig.Mapper.Map<bill_stockin_detail, StockInDetailModel>(s)).ToList();
                               return Md;
                           }).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        public ActionResult DetailIndex(StockInDetailSearch Model)
        {
            int TotalSize = 0;
            Model.PageCont = bll_bill_stockin_detail.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.createdate,
                w => w.stockin_guid == Model.billstockin_guid &&
                (Model.s_keywords == "" || (w.memo ?? "").Contains(Model.s_keywords)
                || (w.lot_number ?? "").Contains(Model.s_keywords) || (w.matter_code ?? "").Contains(Model.s_keywords)
                || (w.name ?? "").Contains(Model.s_keywords))
                        , false).Select(s =>
                        {
                            var M = MapperConfig.Mapper.Map<bill_stockin_detail, StockInDetailModel>(s);
                            M.List_bill_stockin_detail_traymatterModel = bll_bill_stockin_detail_traymatter.GetAllQueryOrderby(o => o.createdate,
                                w => w.stockin_detail_guid == M.guid).Select(s =>
                                    MapperConfig.Mapper.Map<bill_stockin_detail_traymatter, bill_stockin_detail_traymatterModel>(s)).ToList();
                            return M;
                        }).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        [HttpGet]
        public ActionResult DetailAdd(Guid billstk_guid)
        {
            ViewBag.SubmitText = "添加";
            StockInDetailModel Model = new StockInDetailModel();
            // Model.in_date = DateTime.Now;
            Model.stockin_guid = billstk_guid;
            return PartialView(Model);
        }
        [HttpPost]
        public JsonResult DetailAdd(StockInDetailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //限制只能添加一条明细
                    //if (bll_bill_stockin_detail.GetCount(w => w.stockin_guid == model.stockin_guid) > 10)
                    //{
                    //    AddJsonError("入库订单只能有一种物料明细");
                    //    return JsonError();
                    //}
                    if (model.in_number <= 0)
                    {
                        AddJsonError("入库数量必须大于0");
                        return JsonError();
                    }
                    bill_stockin_detail info = new bill_stockin_detail();
                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.effective_date = model.effective_date;
                    info.guid = Guid.NewGuid();

                    info.in_number = model.in_number;
                    info.memo = model.memo;
                    info.lot_number = model.lot_number;

                    var matterObj = bll_stk_matter.GetFirstDefault(w => w.guid == model.matter_guid);
                    info.matter_guid = model.matter_guid;
                    info.state = LTWMSEFModel.EntityStatus.Normal;
                    info.name = matterObj.name;
                    info.matter_code = matterObj.code;
                    info.producedate = model.producedate;
                    info.stockin_guid = model.stockin_guid;
                    info.test_status = model.test_status;

                    var rtv = bll_bill_stockin_detail.Add(info);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加入库订单明细[" + info.matter_code + "][" + info.name + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
        public ActionResult Add()
        {
            ViewBag.SubmitText = "添加入库单";
            StockInModel Model = new StockInModel();
            Model.in_date = DateTime.Now;
            return PartialView(Model);
        }
        [HttpPost]
        public JsonResult Add(StockInModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    /* if (bll_bill_stockin.GetCount(w => w.bill_status != BillsStatus.Finished) > 0)
                     {
                         AddJsonError("有未结束的入库单，请先结束订单再操作！");
                         return JsonError();
                     }*/

                    //if (model.odd_numbers.IndexOf("-") >= 0)
                    //{
                    //    AddJsonError("订单号不能包含符合 '-'");
                    //    return JsonError();
                    //} 
                    ////判断订单是否合法 T01
                    /* if (!Regex.IsMatch(model.odd_numbers, @"^[0-9a-zA-Z]+$"))
                     {
                         AddJsonError("入库单号[" + model.odd_numbers + "]格式错误！请输入正确的格式如：T+数字");
                         return JsonError();
                     }*/
                    /////////
                    bill_stockin info = new bill_stockin();
                    //info.bill_property = BillsProperty.Battery;
                    info.bill_status = model.bill_status;
                    info.from = BillsFrom.System;
                    info.get_status = GetStatus.None;

                    info.in_date = model.in_date;
                    info.memo = model.memo;
                    info.odd_numbers = bll_sys_number_rule.GetBillStockInNum();
                    info.stockin_type = model.stockin_type;
                    info.total_matter = model.total_matter;
                    info.state = LTWMSEFModel.EntityStatus.Normal;
                    info.createdate = DateTime.Now;
                    info.createuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                    info.guid = Guid.NewGuid();
                    var rtv = bll_bill_stockin.AddIfNotExists(info, w => w.odd_numbers);
                    if (rtv == LTWMSEFModel.SimpleBackValue.True)
                    {
                        AddUserOperationLog("添加入库订单[" + info.odd_numbers + "]成功！类型：" + LTLibrary.EnumHelper.GetEnumDescription(info.stockin_type), LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                        return JsonSuccess();
                    }
                    else if (rtv == LTWMSEFModel.SimpleBackValue.ExistsOfAdd)
                    {
                        AddJsonError("数据库已存在订单号为：[" + info.odd_numbers + "]的记录信息");
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
            ViewBag.SubmitText = "保存入库单";
            ViewBag.isUpdate = true;
            var model = bll_bill_stockin.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<bill_stockin, StockInModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("Add", Md);
        }
        [HttpPost]
        public JsonResult Update(StockInModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    bill_stockin info = bll_bill_stockin.GetFirstDefault(w => w.guid == model.guid);
                    if (info != null)
                    {
                        // 订单状态不能后退
                        if ((int)info.bill_status > (int)model.bill_status)
                        {
                            AddJsonError("订单状态不能由“" + LTLibrary.EnumHelper.GetEnumDescription(info.bill_status)
                                + "”修改为“" + LTLibrary.EnumHelper.GetEnumDescription(model.bill_status) + "”");
                            return JsonError();
                        }

                        ////////////////////////
                        info.updatedate = DateTime.Now;
                        info.updateuser = LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName;
                        info.bill_status = model.bill_status;
                        info.in_date = model.in_date;
                        info.memo = model.memo;
                        info.total_matter = model.total_matter;
                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_bill_stockin.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改入库订单[" + info.odd_numbers + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
        [HttpPost]
        public JsonResult DeletePost(Guid guid)
        {
            try
            {
                var model = bll_bill_stockin.GetFirstDefault(w => w.guid == guid);
                if (model != null)
                {
                    //判断订单是否执行，如果执行入库则不能删除！！！！
                    if (model.bill_status == BillsStatus.None)
                    {
                        //var rtv = bll_bill_stockin.Delete(w => w.guid == guid); 
                        var rtv = bll_bill_stockin.Delete(model);
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

        [HttpGet]
        public ActionResult DetailUpdate(Guid guid)
        {
            //SetDropList(null, null, 0, 0);
            ViewBag.SubmitText = "保存";
            ViewBag.isUpdate = true;
            var model = bll_bill_stockin_detail.GetFirstDefault(w => w.guid == guid);
            if (model == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<bill_stockin_detail, StockInDetailModel>(model);
            Md.OldRowVersion = model.rowversion;
            return PartialView("DetailAdd", Md);
        }

        [HttpPost]
        public JsonResult DetailUpdate(StockInDetailModel model)
        {
            ViewBag.isUpdate = true;
            if (ModelState.IsValid)
            {
                try
                {
                    bill_stockin_detail info = bll_bill_stockin_detail.GetFirstDefault(w => w.guid == model.guid);
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
                        info.effective_date = model.effective_date;
                        info.in_number = model.in_number;
                        info.memo = model.memo;
                        info.lot_number = model.lot_number;

                        var matterObj = bll_stk_matter.GetFirstDefault(w => w.guid == model.matter_guid);
                        info.matter_guid = model.matter_guid;
                        //      info.state = LTWMSEFModel.EntityStatus.Normal;
                        info.name = matterObj.name;
                        info.matter_code = matterObj.code;
                        info.producedate = model.producedate;
                        //  info.stockin_guid = model.stockin_guid;
                        info.test_status = model.test_status;

                        //并发控制（乐观锁）
                        info.OldRowVersion = model.OldRowVersion;

                        var rtv = bll_bill_stockin_detail.Update(info);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("修改入库订单明细[" + info.matter_code + "][" + info.name + "]成功！", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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


        [HttpPost]
        public JsonResult DetailDeletePost(Guid guid)
        {
            try
            {
                var model = bll_bill_stockin_detail.GetFirstDefault(w => w.guid == guid);
                if (model != null && model.guid != Guid.Empty)
                {
                    int countTrayM = bll_bill_stockin_detail_traymatter.GetCount(w => w.stockin_detail_guid == model.guid);

                    //判断订单是否执行，如果执行入库则不能删除！！！！
                    if (countTrayM == 0)
                    {
                        var rtv = bll_bill_stockin_detail.Delete(model);
                        if (rtv == LTWMSEFModel.SimpleBackValue.True)
                        {
                            AddUserOperationLog("删除订单明细成功！[" + model.matter_code + "][" + model.name + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
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
                        AddJsonError("订单明细已经绑定入库托盘，不能删除。");
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

        public ActionResult ExportPDF(Guid billstockin_guid, DateTime? exp_date)
        {

            //获取桌面路径设为文件下载保存路径
            //   string Fname = Server.MapPath("~/")+ "pdf/证书.pdf";
            var stockInMd = bll_bill_stockin.GetFirstDefault(w => w.guid == billstockin_guid);
            if (stockInMd != null && stockInMd.guid != Guid.Empty)
            {
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

                cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockInMd.stockin_type) + "单", fontChinese_11));
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

                if (stockInMd != null && stockInMd.guid != Guid.Empty)
                {
                    var stockinDetailMd = bll_bill_stockin_detail.GetFirstDefault(w => w.stockin_guid == stockInMd.guid);
                    if (stockinDetailMd != null && stockinDetailMd.guid != Guid.Empty)
                    {
                        table = new PdfPTable(8);
                        table.TotalWidth = 550;
                        table.LockedWidth = true;

                        cell = new PdfPCell(new Phrase("入库单号", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockInMd.odd_numbers, fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("入库日期", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockInMd.in_date.ToString("yyyy/MM/dd"), fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("入库类型", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(stockInMd.stockin_type), fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("批次", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockinDetailMd.lot_number, fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Phrase("物料码", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockinDetailMd.matter_code, fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Phrase("名称", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockinDetailMd.name, fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 3;
                        cell.Padding = 5;
                        cell.PaddingLeft = 15;
                        table.AddCell(cell);



                        cell = new PdfPCell(new Phrase("入库数量", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockinDetailMd.in_number.ToString(), fontChinese));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Phrase("生产日期", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockinDetailMd.producedate == null ? "" : stockinDetailMd.producedate.Value.ToString("yyyy/MM/dd"), fontChinese));
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
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        cell.Colspan = 2;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Phrase("备注", fontChinese_bold));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(stockInMd.memo, fontChinese));
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

                        var lstDetailTrayMatter = bll_bill_stockin_detail_traymatter.GetAllQuery(w => w.stockin_detail_guid == stockinDetailMd.guid);
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
                            cell = new PdfPCell(new Phrase("入库时间", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase("存储库位", fontChinese_bold));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase("托盘状态", fontChinese_bold));
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
                                cell = new PdfPCell(new Phrase(item.tray_in_date == null ? "" : item.tray_in_date.Value.ToString("yyyy/MM/dd"), fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(item.dest_shelfunits_pos, fontChinese));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Padding = 5;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(item.tray_status), fontChinese));
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
                        str = "入库员(签字)：______________            复核员(签字)：______________";
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

                return File(ms.ToArray(), "application/pdf", LTLibrary.EnumHelper.GetEnumDescription(stockInMd.stockin_type) + "单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                // return View("Index");
            }
            return null;
        }

        [HttpGet]
        public ActionResult UpdateDetail(Guid guid)
        {
            var detailTrayMatter = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == guid);
            if (detailTrayMatter == null)
                return ErrorView("参数错误");
            var Md = MapperConfig.Mapper.Map<bill_stockin_detail_traymatter, bill_stockin_detail_traymatterModel>(detailTrayMatter);

            Md.OldRowVersion = detailTrayMatter.rowversion;
            return PartialView(Md);
        }
        [HttpPost]
        public JsonResult UpdateDetail(bill_stockin_detail_traymatterModel Model)
        {
            try
            {
                var detailTrayM = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == Model.guid);
                if (detailTrayM != null && detailTrayM.guid != Guid.Empty)
                {
                    bool commit = false;
                    using (var tran = bll_bill_stockin.BeginTransaction())
                    {
                        try
                        {
                            string _oddnumbers = "";
                            var objstockin = bll_bill_stockin.GetFirstDefault(w => w.guid == detailTrayM.stockin_guid);
                            if (objstockin != null && objstockin.guid != Guid.Empty)
                            {
                                _oddnumbers = objstockin.odd_numbers;
                            }
                            //并发控制（乐观锁）
                            int oldnum = (int)detailTrayM.number;
                            detailTrayM.number = Model.number;
                            detailTrayM.OldRowVersion = Model.OldRowVersion;
                            var rtv = bll_bill_stockin_detail_traymatter.Update(detailTrayM);
                            //修改托盘数据
                            LTWMSEFModel.SimpleBackValue rtv2 = LTWMSEFModel.SimpleBackValue.True;
                            var trayMatterObj = bll_wh_traymatter.GetFirstDefault(w => w.traybarcode == detailTrayM.traybarcode);
                            if (trayMatterObj != null && trayMatterObj.guid != Guid.Empty)
                            {
                                string txtMsg = "修改数量：由【" + (int)trayMatterObj.number + "】改为【" + Model.number + "】";
                                trayMatterObj.memo += "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]【" +
                                    LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName + "】>>" + txtMsg;
                                trayMatterObj.number = Model.number;
                                rtv2 = bll_wh_traymatter.Update(trayMatterObj);
                                AddUserOperationLog("修改托盘【" + trayMatterObj.traybarcode + "】信息>>" + txtMsg, LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                            }

                            if (rtv == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {
                                AddUserOperationLog("修改订单【" + _oddnumbers + "】 【" + detailTrayM.matter_code + "/" + detailTrayM.matter_name + "】批号【" + detailTrayM.lot_number
                                    + "】 >>关联托盘【" + detailTrayM.traybarcode + "】修改数量：由【" + oldnum + "】改为【" + Model.number + "】", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                commit = true;
                                return JsonSuccess();
                            }
                            else if (rtv == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException ||
                                rtv2 == LTWMSEFModel.SimpleBackValue.DbUpdateConcurrencyException)
                            {
                                AddJsonError("数据并发异常，请重新加载数据然后再保存。");
                            }
                            else
                            {
                                AddJsonError("保存失败");
                            }
                        }
                        finally
                        {
                            if (commit)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
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
            return JsonError();
        }
        [HttpPost]
        public JsonResult DeleteDetailPost(Guid guid)
        {
            try
            {
                var stockindetailtraym = bll_bill_stockin_detail_traymatter.GetFirstDefault(w => w.guid == guid);
                if (stockindetailtraym != null && stockindetailtraym.guid != Guid.Empty)
                {
                    bool commit = false;
                    using (var tran = bll_bill_stockin.BeginTransaction())
                    {
                        try
                        {
                            var rtv = bll_bill_stockin_detail_traymatter.Delete(stockindetailtraym);
                            //查找关联关系并删除 
                            LTWMSEFModel.SimpleBackValue rtvDeleteRelation = LTWMSEFModel.SimpleBackValue.True;
                            var relationObj = bll_bill_task_tray_relation.GetFirstDefault(w => w.traybarcode == stockindetailtraym.traybarcode);
                            if (relationObj != null && relationObj.guid != Guid.Empty)
                            {
                                rtvDeleteRelation = bll_bill_task_tray_relation.Delete(relationObj);
                            }
                            //判断托盘是否上架，没上架删除关联关系，判断入库单的状态
                            LTWMSEFModel.SimpleBackValue rtvupdatetray = LTWMSEFModel.SimpleBackValue.True;
                            LTWMSEFModel.SimpleBackValue rtvtraymodelupdate = LTWMSEFModel.SimpleBackValue.True;
                            if (stockindetailtraym.tray_status != TrayInStockStatusEnum.Stored)
                            {
                                var trayModel = bll_wh_tray.GetFirstDefault(w => w.traybarcode == stockindetailtraym.traybarcode);
                                if (trayModel != null && trayModel.guid != Guid.Empty)
                                {
                                    //取消托盘组盘
                                    trayModel.isscan = null;
                                    trayModel.scandate = null;
                                    rtvupdatetray =bll_wh_tray.Update(trayModel);
                                    //判断有没有入库任务
                                    if (trayModel.status == LTWMSEFModel.Warehouse.TrayStatus.OffShelf)
                                    {
                                        var traymatterModel = bll_wh_traymatter.GetFirstDefault(w => w.tray_guid == trayModel.guid);
                                        if (traymatterModel != null && traymatterModel.guid != Guid.Empty)
                                        {
                                            rtvtraymodelupdate = bll_wh_traymatter.Delete(traymatterModel);
                                        }
                                    }
                                }
                            }
                            if (rtv == LTWMSEFModel.SimpleBackValue.True && rtvDeleteRelation == LTWMSEFModel.SimpleBackValue.True&&
                                rtvupdatetray== LTWMSEFModel.SimpleBackValue.True&& rtvtraymodelupdate == LTWMSEFModel.SimpleBackValue.True)
                            {
                                string oddNumbers = "";
                                var stockInModel = bll_bill_stockin.GetFirstDefault(w => w.guid == stockindetailtraym.stockin_guid);
                                if (stockInModel != null && stockInModel.guid != Guid.Empty)
                                {
                                    oddNumbers = stockInModel.odd_numbers;
                                }
                                AddUserOperationLog("删除订单【" + oddNumbers + "】明细>>删除托盘【" + stockindetailtraym.traybarcode
                                    + "】绑定信息成功！[" + stockindetailtraym.matter_code + "/" + stockindetailtraym.matter_code
                                    + "] 批次[" + stockindetailtraym.lot_number + "] 数量[" + stockindetailtraym.number + "]", LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                                // return JsonSuccess();
                                commit = true;
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
                        finally
                        {
                            if (commit)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                    }
                    if (commit)
                    {
                        return JsonSuccess();
                    }
                }
                else
                {
                    AddJsonError("数据不存在或已删除！");
                }
                return JsonSuccess();
            }
            catch (Exception ex)
            {
                AddJsonError("删除数据异常！详细信息:" + ex.Message);
            }
            return JsonError();
        }

    }
}