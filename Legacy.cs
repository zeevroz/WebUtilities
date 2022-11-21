using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;

namespace WebUtilities
{
    public static class Legacy
    {
        public static string CreateTocPdf(TocRequest json)
        {
            Document document = new Document(PageSize.A4);

            string file = Path.Combine(Directory.GetCurrentDirectory(), "Tmp", "toc" + DateTime.Now.ToString("yyyyMMHHmmss") + ".pdf");
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(file, FileMode.CreateNew));

            document.Open();

            string ARIALUNI_TFF = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "Alef-Regular.ttf");

            BaseFont baseFont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font headerFont = new Font(baseFont, 16, Font.BOLD);
            Font paragraphFont = new Font(baseFont, 12, Font.NORMAL);

            #region Chunk
            //iTextSharp.text.pdf.PdfContentByte cb = new iTextSharp.text.pdf.PdfContentByte(writer);
            //iTextSharp.text.pdf.ColumnText ct = new iTextSharp.text.pdf.ColumnText(cb);                    

            //ColumnText ct = new ColumnText(writer.DirectContent);
            //ct.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            //ct.SetSimpleColumn(100, 100, 500, 800, 24, Element.ALIGN_RIGHT);

            //foreach (Chapter entry in json.Chapters)
            //{
            //    Chunk c = new Chunk(entry.Title, hebrewFont);

            //    //PdfAction action = PdfAction.GotoLocalPage(entry.PageNum);
            //    //c.SetAction(action);

            //    ct.AddElement(c);
            //}

            //ct.Go();
            #endregion

            PdfPTable table = new PdfPTable(1) { RunDirection = PdfWriter.RUN_DIRECTION_RTL };

            PdfPCell cell = new PdfPCell(new Phrase("תוכן העניינים", headerFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BorderWidth = 0;

            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(2) { SpacingBefore = 20f, RunDirection = PdfWriter.RUN_DIRECTION_RTL };

            foreach (Chapter entry in json.Chapters)
            {
                Chunk chunk = new Chunk(entry.Title, paragraphFont);

                //if (entry.PageNum > 1)
                //{
                //    PdfAction action = PdfAction.GotoLocalPage(entry.PageNum, new PdfDestination(PdfDestination.FIT), writer);
                //    c.SetAction(action);
                //}

                cell = new PdfPCell(new Phrase(chunk)) {HorizontalAlignment = Element.ALIGN_LEFT, BorderWidth = 0};
                table.AddCell(cell);

                table.AddCell(new PdfPCell(new Phrase(entry.PageNum.ToString(), paragraphFont)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                table.CompleteRow();
            }

            document.Add(table);
            
            document.Close();

            return file;
        }


    }
}
