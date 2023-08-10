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

namespace PITB.CRM_API.Controllers.LWMC
{
    public class LwmcSyncComplainantController : ApiController
    {
        public SyncModel Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0, [FromUri] int dbVersionId = 0, [FromUri] int appVersionId = 0, [FromUri] int mobileVersionId = 0, [FromUri]  int platformId = 0)
        {
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
                    return BlLwmcApiHandler.SyncComplainant(submitSyncComplaint, appId, (Config.Language)languageId, (Config.PlatformID)platformId, dbVersionId, appVersionId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(json, ipAddress, false, ex.Message.ToString()));
                    return new SyncModel() { Status = Config.ResponseType.Failure.ToString(), Message = "Server Error" };
                }
            }
            else
            {
                SyncModel resp = new SyncModel();
                resp.SetAuthenticationError();
                return resp;
            }
        }
    }
}
