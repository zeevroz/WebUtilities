using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using iText.Kernel.Pdf;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebUtilities.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MergePdfs : ControllerBase
    {
        // GET: <MergePdfs>
        [HttpGet]
        public string Get()
        {
            return "Use Post method.";
        }

        [Serializable]
        public class InsertParams
        {
            public string file { get; set; }
            public int after { get; set; }
        }

        [Serializable]
        public class MergeRequest
        {
            public string src { get; set; }
            public InsertParams[] insert { get; set; }
            public string dst { get; set; }
        }

        // POST api/<MergePdf>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] MergeRequest json)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (string.IsNullOrWhiteSpace(json.src) || string.IsNullOrWhiteSpace(json.dst) || json.insert.Length == 0)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Content = new StringContent("StringContent");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                response.Content.Headers.Add("Error", "Missing parameters.");
                return response;
            }

            using (PdfDocument dstDoc = new PdfDocument(new PdfWriter(json.dst)))
            {
                using (PdfDocument srcDoc = new PdfDocument(new PdfReader(json.src)))
                {

                    int srcNumOfPages = srcDoc.GetNumberOfPages();
                    foreach (var item in json.insert)
                        if (item.after > srcNumOfPages) item.after = srcNumOfPages;


                    for (int i = 1; i <= srcNumOfPages; i++)
                    {
                        srcDoc.CopyPagesTo(i, i, dstDoc);

                        InsertParams[] items = Array.FindAll<InsertParams>(json.insert, v => v.after == i);

                        if (items.Length == 0) continue;

                        foreach (InsertParams item in items)
                        {
                            InsertDocument(dstDoc, item.file);
                        }

                    }

                }

            }

            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StringContent("Success!");

            return response;
        }

        private void InsertDocument(PdfDocument dst, string file)
        {
            PdfDocument insertDoc = new PdfDocument(new PdfReader(file));

            for (int j = 1; j <= insertDoc.GetNumberOfPages(); j++)
            {
                insertDoc.CopyPagesTo(j, j, dst);
            }

            insertDoc.Close();
        }

        //// PUT api/<MergePdf>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<MergePdf>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        private readonly ILogger<MergePdfs> _logger;

        public MergePdfs(ILogger<MergePdfs> logger)
        {
            _logger = logger;
        }
    }
}
