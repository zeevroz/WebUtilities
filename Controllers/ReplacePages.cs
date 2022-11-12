using iText.Forms.Xfdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace WebUtilities.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReplacePages : ControllerBase
    {
        // GET: <ReplacePages?file="c:\tmp\split.pdf">
        [HttpGet]
        public MergeRequest Get([FromQuery] string file)
        {
            string newSrc = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_Copy" + Path.GetExtension(file));

            if (System.IO.File.Exists(newSrc))
            {
                System.IO.File.Delete(newSrc);
            }

            System.IO.File.Copy(file, newSrc);

            MergeRequest result = new MergeRequest(newSrc, file);

            using (PdfDocument srcDoc = new PdfDocument(new PdfReader(newSrc)))
            {

                for (int i = 1; i <= srcDoc.GetNumberOfPages(); i++)
                {
                    PdfPage p = srcDoc.GetPage(i);
                    //var strategy = new SimpleTextExtractionStrategy();
                    var strategy = new LocationTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(p, strategy);

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    //if (!text.StartsWith("~pdf~"))
                    //{
                    //    continue;
                    //}

                    int sidx = text.IndexOf("pdf@");

                    if (sidx < 0)
                    {
                        continue;
                    }

                    int eidx = text.IndexOf("#/pdf");

                    if (eidx < 0)
                    {
                        continue;
                    }

                    string ifile = text.Substring(sidx + 4, eidx - sidx - 4);

                    if (!System.IO.File.Exists(ifile))
                    {
                        continue;
                    }

                    result.insert.Add(new InsertParams(ifile, i.ToString()));

                }

            }

            if (Replace(result))
            {
                System.IO.File.Delete(newSrc);
                System.IO.File.Copy(file, newSrc);
                EnumeratePages(newSrc, file);
                System.IO.File.Delete(newSrc);
                result.status = "Success";
            }
            else
                result.status = "Error";

            return result;
        }

        private bool Replace(MergeRequest json)
        {
            using (PdfDocument dstDoc = new PdfDocument(new PdfWriter(json.dst)))
            {
                using (PdfDocument srcDoc = new PdfDocument(new PdfReader(json.src)))
                {

                    for (int i = 1; i <= srcDoc.GetNumberOfPages(); i++)
                    {
                        if (!Library.InsertItems(dstDoc, Array.FindAll<InsertParams>(json.insert.ToArray(), v => v.after == i.ToString())))
                            srcDoc.CopyPagesTo(i, i, dstDoc);
                    }
                }
            }

            

            return true;
        }

        private bool EnumeratePages(string newSrc, string file)
        {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(newSrc), new PdfWriter(file)))//"c:\\tmp\\pnumbers.pdf"
            {
                using (Document doc = new Document(pdfDoc))
                {

                    int numberOfPages = pdfDoc.GetNumberOfPages();

                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        doc.ShowTextAligned(new Paragraph("page " + i + " of " + numberOfPages), 0, 0, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                    }
                }
            }

            return true;
        }



    }
}
