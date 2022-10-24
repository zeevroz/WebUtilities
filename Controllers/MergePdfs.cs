using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using iText.Kernel.Pdf;

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

        // POST api/<MergePdf>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] MergeRequest json)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (string.IsNullOrWhiteSpace(json.src) || string.IsNullOrWhiteSpace(json.dst) || json.insert.Count == 0)
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

                    #region Normalize "after"
                    foreach (var item in json.insert)
                    {
                        if (int.TryParse(item.after, out int result))
                        {
                            if (result == 0) item.after = "start";
                            else if (result > srcNumOfPages) item.after = "end";
                        }
                        else if (string.IsNullOrWhiteSpace(item.after))
                        {
                            item.after = "end";
                        }
                        else if (item.after == "start")
                        {

                        }
                        else if (item.after == "end")
                        {

                        }
                        else
                        {
                            item.after = "end";
                        }
                    }
                    #endregion

                    InsertParams[] items = Array.FindAll<InsertParams>(json.insert.ToArray(), v => v.after == "start");

                    Library.InsertItems(dstDoc, items);

                    for (int i = 1; i <= srcNumOfPages; i++)
                    {
                        srcDoc.CopyPagesTo(i, i, dstDoc);

                        items = Array.FindAll<InsertParams>(json.insert.ToArray(), v => v.after == i.ToString());

                        Library.InsertItems(dstDoc, items);
                    }

                    items = Array.FindAll<InsertParams>(json.insert.ToArray(), v => v.after == "end");

                    Library.InsertItems(dstDoc, items);
                }

            }

            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new StringContent("Success!");

            return response;
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
