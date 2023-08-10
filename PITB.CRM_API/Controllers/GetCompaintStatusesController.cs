using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers
{
    public class GetCompaintStatusesController : ApiController
    {
        // GET: api/GetCompaintStatuses
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetCompaintStatuses/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GetCompaintStatuses
        public ApiComplaintStatusResponseModel Post([FromBody]JToken jsonBody, [FromUri] int languageId = 0, [FromUri] int appId = 0)
        {
            //List<int>asd = new List<int>(){1,2,3,4,4};
            //string ddd = JsonConvert.SerializeObject(asd);

            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                ApiComplaintStatusesPostedModel submitComplaintModel =
                    (ApiComplaintStatusesPostedModel)JsonConvert.DeserializeObject(actualJson, typeof(ApiComplaintStatusesPostedModel));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.GetCurrentStatusesOfComplaints(submitComplaintModel, (Config.Language) languageId);
            }
            catch (Exception ex)
            {
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return null;
            }
        }

        // PUT: api/GetCompaintStatuses/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GetCompaintStatuses/5
        public void Delete(int id)
        {
        }
    }
}
