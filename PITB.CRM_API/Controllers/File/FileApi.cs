using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.File
{
    [RoutePrefix("api")]
    public class FileApiController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("GetFile/{*fileRelativePath}")]
        public HttpResponseMessage GetFile([FromUri]string fileRelativePath = null)
        {
            //Request.RequestUri
            string[] filePathArr = fileRelativePath.Split('/');
            string pathPrefix = "D:/";

            string filePath = pathPrefix + fileRelativePath;
            string fileName = filePathArr.LastOrDefault();

            
            
            //var dataBytes = System.IO.File.ReadAllBytes("D:/PostedImage.jpg");
            var dataBytes = System.IO.File.ReadAllBytes(filePath);
            //adding bytes to memory stream   
            var dataStream = new MemoryStream(dataBytes);
            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            //httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;//"PostedImage.jpg";
            //httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            return httpResponseMessage;
            // End Custom code

            //var stream = new MemoryStream();
            //// processing the stream.

            //var result = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new ByteArrayContent(stream.ToArray())
            //};
            //result.Content.Headers.ContentDisposition =
            //    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            //    {
            //        FileName = "CertificationCard.pdf"
            //    };
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");

            //return result;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}