using LTWMSWebMVC.Areas.History.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Logs;
using LTWMSEFModel.Logs;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LTWMSWebMVC.Areas.History.Controllers
{
    public class UserOperationLogController : BaseController
    {
        log_sys_useroperationBLL bll_useroperation_log;
        public UserOperationLogController(log_sys_useroperationBLL bll_useroperation_log)
        {
            this.bll_useroperation_log = bll_useroperation_log;
        }
        // GET: History/UserOperationLog
        public ActionResult Index(UserOperationLogSearch Model)
        {
            var loginModel = GetCurrentLoginUser();
            string _userName = loginModel.username;
            if (loginModel.issuperadmin == true)
            {//超级管理员查询所有
                _userName = null;
            }
            DateTime? beginDate = null;
            if (Model.s_out_date_begin != null)
            {
                beginDate = new DateTime(Model.s_out_date_begin.Value.Year,
              Model.s_out_date_begin.Value.Month, Model.s_out_date_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (Model.s_out_date_end != null)
            {
                endDate = new DateTime(Model.s_out_date_end.Value.Year,
                    Model.s_out_date_end.Value.Month, Model.s_out_date_end.Value.Day);
                endDate = endDate.Value.AddDays(1);
            }
            int TotalSize = 0;
            Model.PageCont = bll_useroperation_log.Pagination(Model.Paging.paging_curr_page
                , Model.Paging.PageSize, out TotalSize, o => o.log_date,
                w => (beginDate == null || w.log_date >= beginDate) && (endDate == null || w.log_date <= endDate) && (Model.s_keywords == "" || (w.remark ?? "").Contains(Model.s_keywords)
                || (w.operator_u ?? "").Contains(Model.s_keywords)) && (_userName == null || w.operator_u == _userName)
                , false).Select(s => MapperConfig.Mapper.Map<log_sys_useroperation, UserOperationLogModel>(s)).ToList();
            Model.Paging = new PagingModel() { TotalSize = TotalSize };
            return View(Model);
        }
        public ActionResult ExportPDF(string keywords, DateTime? dt_begin, DateTime? dt_end,DateTime? exp_date)
        {
            //获取桌面路径设为文件下载保存路径
            //   string Fname = Server.MapPath("~/")+ "pdf/证书.pdf";
            DateTime? beginDate = null;
            if (dt_begin != null)
            {
                beginDate = new DateTime(dt_begin.Value.Year,
            dt_begin.Value.Month, dt_begin.Value.Day);
            }
            DateTime? endDate = null;
            if (dt_end != null)
            {
                endDate = new DateTime(dt_end.Value.Year,
                    dt_end.Value.Month, dt_end.Value.Day);
                endDate = endDate.Value.AddDays(1);
            }
            var operationLogMod = bll_useroperation_log.GetAllQueryOrderby(o => o.log_date,
             w => (beginDate == null || w.log_date >= beginDate) && (endDate == null || w.log_date <= endDate) &&
             (keywords == "" || (w.remark ?? "").Contains(keywords) || (w.operator_u ?? "").Contains(keywords))
             , false);

            if (operationLogMod != null && operationLogMod.Count > 0)
            {
                var ms = new MemoryStream();
                #region CreatePDF
                Document document = new Document(PageSize.A4, 5f, 5f, 30f, 0f);
                //Document document = new Document(PageSize.A4.Rotate(), 0f, 0f, 10f, 0f);(A4纸横线打印)
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                PdfPTable table = new PdfPTable(3);
                table.SplitLate = false;//设置当前页能放多少放多少
                table.TotalWidth = 550;
                table.LockedWidth = true;
                table.SetWidths(new int[] { 50, 450, 50 });
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

                cell = new PdfPCell(new Phrase("操作日志", fontChinese_11));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.NO_BORDER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", fontChinese));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.NO_BORDER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", fontChinese));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.NO_BORDER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", fontChinese));
                cell.Colspan = 1;
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


                document.Add(table);

                // table.SetWidths(new int[] { 40, 70, 65, 55, 60, 45, 45, 165 });
                table = new PdfPTable(3);
                table.TotalWidth = 550;
                table.LockedWidth = true;
                table.SetWidths(new int[] { 60, 430, 60 });

                cell = new PdfPCell(new Phrase("时间", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("日志内容", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("操作人", fontChinese_bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);

                foreach (var item in operationLogMod)
                {
                    cell = new PdfPCell(new Phrase(item.log_date.ToString("yyyy/MM/dd HH:mm:ss.fff"), fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(item.remark, fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(item.operator_u, fontChinese));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);

                }
                document.Add(table);

                document.NewPage();
                document.Close();
                #endregion
                //System.IO.File.Delete(filePath);

                return File(ms.ToArray(), "application/pdf", "操作日志" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                // return View("Index");
            }

            return null;
        }
    }
}