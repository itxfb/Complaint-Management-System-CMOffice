using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CRM_API.Controllers
{
    public class LwmcPostComplaintRemarksController : ApiController
    {
      

        // POST: api/PostComplaintRemarks
        public ApiStatus Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SubmitComplaintRemarks submitComplaintRemarks =
                        (SubmitComplaintRemarks)JsonConvert.DeserializeObject(actualJson, typeof(SubmitComplaintRemarks));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return BlLwmcApiHandler.SubmitComplainantRemarks(submitComplaintRemarks);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    return Utility.GetStatus(Config.ResponseType.Failure.ToString(),
                         ex.Message);
                    //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                }
            }
            else
            {
                ApiStatus resp = new ApiStatus();
                resp.SetAuthenticationError();
                return resp;
            }
            
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

        
    }
}
