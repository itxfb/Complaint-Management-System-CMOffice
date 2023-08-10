using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.Models;

namespace PITB.CRM_API.Controllers.LWMC
{
    public class LwmcSubmitComplainantLoginController : ApiController
    {
        public SyncModel Post([FromBody] JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0, [FromUri] int dbVersionId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();

                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SubmitComplainantLogin submitComplainantLogin = JsonConvert.DeserializeObject<SubmitComplainantLogin>(actualJson);

                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));

                    return BlLwmcApiHandler.SubmitComplainantLogin(submitComplainantLogin, appId, (Config.PlatformID)platformId, (Config.Language)languageId, dbVersionId);
                }
                catch (Exception ex)
                {
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, false, ex.Message.ToString()));
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
