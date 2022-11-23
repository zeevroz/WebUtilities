using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

//https://www.nuget.org/packages/iTextSharp
//dotnet add package iTextSharp --version 5.5.13.3

namespace WebUtilities.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TocChaptersList : ControllerBase
    {
        // GET: <TocChaptersList>
        [HttpGet]
        public string Get()
        {
            return "Use Post method.";
        }

        // POST api/<TocChaptersList>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] TocRequest json)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (string.IsNullOrWhiteSpace(json.File) || json.Chapters.Count == 0)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Content = new StringContent("StringContent");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                response.Content.Headers.Add("Error", "Missing parameters.");
                return response;
            }

            //string src = Library.SwapSource(json.File);
            string src = json.File;

            if (string.IsNullOrWhiteSpace(src))
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Content = new StringContent("StringContent");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                response.Content.Headers.Add("Error", "Cannot swap source.");
                return response;
            }

            using (PdfDocument srcDoc = new PdfDocument(new PdfReader(src)))
            {
                for (int p = 1; p <= srcDoc.GetNumberOfPages(); p++)
                {
                    //var strategy = new SimpleTextExtractionStrategy();
                    var strategy = new LocationTextExtractionStrategy();
                    strategy.SetRightToLeftRunDirection(true);

                    string text = PdfTextExtractor.GetTextFromPage(srcDoc.GetPage(p), strategy);

                    if (string.IsNullOrWhiteSpace(text))
                        continue;

                    foreach (Chapter c in json.Chapters.FindAll(x => x.PageNum == 0))
                    {
                        if (text.IndexOf(c.Title) < 0)
                            continue;
                        else
                        {
                            c.PageNum = p;
                        }
                    }
                }
            }

            string toc = Legacy.CreateTocPdf(json);

            if (string.IsNullOrWhiteSpace(toc))
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Content = new StringContent("StringContent");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                response.Content.Headers.Add("Error", "Unable to create TOC.");
                return response;
            }

            //using (PdfDocument dstDoc = new PdfDocument(new PdfWriter(json.File)))
            //{
            //    Library.InsertDocument(dstDoc, toc);
            //    Library.InsertDocument(dstDoc, src);
            //}

            //System.IO.File.Delete(toc);
            //System.IO.File.Delete(src);

            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StringContent("Success!");

            return response;
        }

        private void AddActions(string src, TocRequest json)
        {
            using (PdfDocument srcDoc = new PdfDocument(new PdfWriter(src)))
            {
                //for (int p = 1; p <= srcDoc.GetNumberOfPages(); p++)
                //{
                int p = 1;
                    //var strategy = new SimpleTextExtractionStrategy();
                    var strategy = new LocationTextExtractionStrategy();
                    strategy.SetRightToLeftRunDirection(true);

                    string text = PdfTextExtractor.GetTextFromPage(srcDoc.GetPage(p), strategy);

                    //if (string.IsNullOrWhiteSpace(text))
                    //    continue;

                    foreach (Chapter c in json.Chapters.FindAll(x => x.PageNum == 0))
                    {
                        if (text.IndexOf(c.Title) < 0)
                            continue;
                        else
                        {
                            c.PageNum = p;
                        }
                    }
                //}
            }
        }

        private readonly ILogger<TocChaptersList> _logger;

        public TocChaptersList(ILogger<TocChaptersList> logger)
        {
            _logger = logger;
        }
    }
}
