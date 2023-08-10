using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CRM_API.Controllers
{
    public class PostComplaintRemarksController : ApiController
    {
        // GET: api/PostComplaintRemarks
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PostComplaintRemarks/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PostComplaintRemarks
        public ApiStatus Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SubmitComplaintRemarks submitComplaintRemarks =
                    (SubmitComplaintRemarks)JsonConvert.DeserializeObject(actualJson, typeof(SubmitComplaintRemarks));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.SubmitComplainantRemarks(submitComplaintRemarks);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                return Utility.GetStatus(Config.ResponseType.Failure.ToString(),
                     ex.Message);
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }

            return new ApiStatus("Success", "Remarks has been posted successfully.");
            /*
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                ApiComplaintStatusesPostedModel submitComplaintModel =
                    (ApiComplaintStatusesPostedModel)JsonConvert.DeserializeObject(actualJson, typeof(ApiComplaintStatusesPostedModel));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.GetCurrentStatusesOfComplaints(submitComplaintModel);
            }
            catch (Exception ex)
            {
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return null;
            }*/
        }

        // PUT: api/PostComplaintRemarks/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PostComplaintRemarks/5
        public void Delete(int id)
        {
        }
    }
}
