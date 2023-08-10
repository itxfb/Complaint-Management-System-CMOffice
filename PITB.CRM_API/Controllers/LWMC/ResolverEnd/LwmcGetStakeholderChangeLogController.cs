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
using PITB.CMS_Common.Models;

namespace PITB.CRM_API.Controllers.LWMC.ResolverEnd
{
    public class LwmcGetStakeholderChangeLogController : ApiController
    {
        public object Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    RequestComplaintStatusLog submitComplainantLogin = (RequestComplaintStatusLog)JsonConvert.DeserializeObject(actualJson, typeof(RequestComplaintStatusLog));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));  
                    return BlLwmcApiHandler.GetComplaintStatusChangeLog(submitComplainantLogin, (Config.PlatformID)platformId, (Config.Language)languageId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));

                    return new ResponseComplaintStatusChangeLog() { Status = Config.ResponseType.Failure.ToString(), Message = "Server Error" };
                }
            }
            else
            {
                ResponseComplaintStatusChangeLog resp = new ResponseComplaintStatusChangeLog();
                //resp.SetAuthenticationError();
                resp.SetStatus(authModel);
                return resp;
            }
        }


    }
}