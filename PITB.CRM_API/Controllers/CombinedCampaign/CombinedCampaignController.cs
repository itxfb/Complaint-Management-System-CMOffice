using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.Handler.Business;

namespace PITB.CRM_API.Controllers.CombinedCampaign
{
    [RoutePrefix("api")]
    public class CombinedCampaignController : ApiController
    {
        [HttpPost]
        [Route("CombinedCampaign/SyncCampaignData")]
        public object SyncCampaignData([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0, [FromUri] int dbVersionId = 0, [FromUri] int appVersionId = 0, [FromUri] int mobileVersionId = 0, [FromUri]  int platformId = 0)
        {
            //Request.RequestUri.Scheme==Uri.UriSchemeHttps
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string json = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SubmitSyncComplainant submitSyncComplaint =
                        (SubmitSyncComplainant)JsonConvert.DeserializeObject(json, typeof(SubmitSyncComplainant));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(json, ipAddress, true, null));
                    return BlCombinedCampaign.SyncComplainant(submitSyncComplaint, appId, (Config.Language)languageId/*, (Config.PlatformID)platformId, dbVersionId, appVersionId*/);
                }
                catch (Exception ex)
                {

                    return null;
                    //return new LwmcStakeholderComplaintResponse() { Status = Config.ResponseType.Failure.ToString(), Message = "Server Error" };
                }
            }
            else
            {
                return Utility.GetApiResponse(false, authModel.Status, authModel.Message);

                LwmcStakeholderComplaint resp = new LwmcStakeholderComplaint();
                resp.SetStatus(authModel);
                //resp.SetAuthenticationError();
                return resp;
            }
        }

        [HttpPost]
        [Route("CombinedCampaign/SubmitComplaint")]
        public object SubmitComplaint([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string json = jsonBody.ToString();
                //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                //Int64 apiRequestId = -1;
                try
                {
                    dynamic request = Utility.DeserializeToDynamic(json);
                    //dynamic req2 = JToken.Parse(json);
                    return BlCombinedCampaign.SubmitComplaint(request);
                }
                catch (Exception ex)
                {
                    return Utility.GetApiResponse(false, null, null, null);
                }
            }
            else
            {
                return Utility.GetApiResponse(false, authModel.Status, authModel.Message);
            }
        }

        [HttpPost]
        [Route("CombinedCampaign/GetComplainantComplaints")]
        public object GetComplainantComplaints([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string json = jsonBody.ToString();
                //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                //Int64 apiRequestId = -1;
                try
                {
                    dynamic request = Utility.DeserializeToDynamic(json);
                    //dynamic req2 = JToken.Parse(json);
                    dynamic d = BlCombinedCampaign.GetComplainantComplaints(request);
                    //string s = JsonConvert.SerializeObject(d);
                    return d;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return Utility.GetApiResponse(false, authModel.Status, authModel.Message);
            }
        }


        [HttpPost]
        [Route("CombinedCampaign/SubmitCategories")]
        public object SubmitCategories([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string json = jsonBody.ToString();
                //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                //Int64 apiRequestId = -1;
                try
                {
                    dynamic request = Utility.DeserializeToDynamic(json);
                    //dynamic req2 = JToken.Parse(json);
                    dynamic d = BlCombinedCampaign.SubmitCategories(request);
                    //string s = JsonConvert.SerializeObject(d);
                    return d;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return Utility.GetApiResponse(false, authModel.Status, authModel.Message);
            }
        }
    }
}
