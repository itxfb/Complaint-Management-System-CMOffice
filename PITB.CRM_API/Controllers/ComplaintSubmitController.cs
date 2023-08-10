using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers
{
    public class ComplaintSubmitController : ApiController
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

        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage ComplaintSubmit([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            HttpResponseMessage response= new HttpResponseMessage(HttpStatusCode.OK);
            //response.Content ="";

            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SubmitComplaintModel submitComplaintModel =
                    (SubmitComplaintModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitComplaintModel));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                response.Content = new StringContent(JsonConvert.SerializeObject(BlApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId)));
                //return JsonConvert.SerializeObject(BlApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId));
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                response.Content = new StringContent(JsonConvert.SerializeObject(new ComplaintSubmitResponse (Config.ResponseType.Failure.ToString(), "Server Error", "")));
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }
            return response;
        }
        //public ComplaintSubmitResponse Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        //{
        //    string actualJson = jsonBody.ToString();
        //    string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
        //    Int64 apiRequestId = -1;
        //    try
        //    {
        //        SubmitComplaintModel submitComplaintModel =
        //            (SubmitComplaintModel) JsonConvert.DeserializeObject(actualJson, typeof (SubmitComplaintModel));
        //        apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
        //        return BlApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId);
        //    }
        //    catch (Exception ex)
        //    {
        //       apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
        //       //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
        //       //     ex.Message);
        //       return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
        //        //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
        //    }
           
        //}

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