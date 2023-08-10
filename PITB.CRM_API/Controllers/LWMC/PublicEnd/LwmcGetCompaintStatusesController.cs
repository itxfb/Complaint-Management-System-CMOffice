using System;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CRM_API.Controllers
{
    public class LwmcGetCompaintStatusesController : ApiController
    {
     
        // POST: api/GetCompaintStatuses
        public ApiComplaintStatusResponseModel Post([FromBody]JToken jsonBody, [FromUri] int languageId = 0, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                try
                {
                    ApiComplaintStatusesPostedModel submitComplaintModel =
                        (ApiComplaintStatusesPostedModel)
                            JsonConvert.DeserializeObject(actualJson, typeof (ApiComplaintStatusesPostedModel));
                    return BlLwmcApiHandler.GetCurrentStatusesOfComplaints(submitComplaintModel,
                        (Config.Language) languageId);
                }
                catch (Exception ex)
                {
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return null;
                }
            }
            else
            {
                ApiComplaintStatusResponseModel resp = new ApiComplaintStatusResponseModel();
                resp.SetAuthenticationError();
                return resp;
            }
        }

    }
}
