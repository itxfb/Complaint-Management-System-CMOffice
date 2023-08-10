using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers
{
    public class PostComplaintController : ApiController
    {
        // GET: api/PostComplaint
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/PostComplaint/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PostComplaint
        public ComplaintSubmitResponse Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {

            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SubmitComplaintModel submitComplaintModel =
                    (SubmitComplaintModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitComplaintModel));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }
           

            //return new ApiStatus("Success", "Remarks has been posted successfully.");
        }

        // PUT: api/PostComplaint/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PostComplaint/5
        public void Delete(int id)
        {
        }
    }
}
