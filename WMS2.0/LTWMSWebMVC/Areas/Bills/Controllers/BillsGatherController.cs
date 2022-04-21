using iTextSharp.text;
using iTextSharp.text.pdf;
using LTWMSEFModel.query_model;
using LTWMSService.Bills;
using LTWMSWebMVC.Areas.Bills.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Bills.Controllers
{
    public class BillsGatherController : Controller
    {
        bill_stockinBLL bll_bill_stockin;
        public BillsGatherController(bill_stockinBLL bll_bill_stockin)
        {
            this.bll_bill_stockin = bll_bill_stockin;
        }
        // GET: Bills/BillsGather
        public ActionResult Index(BillsGatherSearch Model)
        {
            int TotalSize = 0;
            int _id = 0;
            Model.PageCont = bll_bill_stockin.BillsInOutGatherPaging(Model.s_keywords, Model.Paging.paging_curr_page, Model.Paging.PageSize, out TotalSize,
                Model.s_date_begin, Model.s_date_end, Model.bills_type).Select(s =>
                {
                    _id++;
                    var Md = MapperConfig.Mapper.Map<BillsGather, BillsGatherModel>(s);
                    Md.Id = _id + (Model.Paging.PageSize * (Model.Paging.paging_curr_page - 1));
                    return Md;
                }).ToList();

            Model.Paging = new PagingModel() { TotalSize = TotalSize };

            return View(Model);
        }

        public ActionResult ExportPDF(string kwd, DateTime? begin, DateTime? end, BillsInOutEnum? bills_type, DateTime? exp_date, int ex_type)
        {

            if (ex_type == 0)
            {//入库
                bills_type = BillsInOutEnum.BillsIn;
            }
            else
            {
                bills_type = BillsInOutEnum.BillsOut;
            }

            int TotalSize = 0;
            int _id = 0;
            var ListGatherData = bll_bill_stockin.BillsInOutGatherPaging(kwd, 1, 5000, out TotalSize,
                begin, end, bills_type).Select(s =>
                {
                    _id++;
                    var Md = MapperConfig.Mapper.Map<BillsGather, BillsGatherModel>(s);
                    Md.Id = _id;// (Model.Paging.PageSize * (Model.Paging.paging_curr_page - 1));
                    return Md;
                }).ToList();

            if (ListGatherData != null && ListGatherData.Count > 0)
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
                string _title = bills_type == BillsInOutEnum.BillsIn ? "生产入库单" : "生产出库单";
                cell = new PdfPCell(new Phrase(_title, fontChinese_11));
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

                //   string str = "";  
                document.Add(table);


                //》》》》》》》》》》》》明细
                table = new PdfPTable(8);
                table.TotalWidth = 550;
                table.LockedWidth = true;

                //明细>>>

                //明细
                cell = new PdfPCell(new Phrase("出/入库单号", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.PaddingBottom = 5;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("出/入库日期", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("品名", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("批号", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("数量", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("检验状态", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("出库人员", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("复核人员", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);

                foreach (var item in ListGatherData)
                {

                    cell = new PdfPCell(new Phrase(item.odd_numbers, fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(item.inout_date.ToString("yyyy/MM/dd"), fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(item.name, fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(item.lot_number, fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(item.in_out_number.ToString(), fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(LTLibrary.EnumHelper.GetEnumDescription(item.test_status), fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("", fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("", fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                }

                document.Add(table);

                /*
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


            */

                /*  table = new PdfPTable(8);
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
           */



                document.NewPage();
                document.Close();
                #endregion
                //System.IO.File.Delete(filePath);

                return File(ms.ToArray(), "application/pdf", _title + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                // return View("Index");
            }
            return null;
        }



    }
}