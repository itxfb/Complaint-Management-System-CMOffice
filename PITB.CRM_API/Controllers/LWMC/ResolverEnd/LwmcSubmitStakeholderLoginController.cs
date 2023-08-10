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
using PITB.CMS_Common.Models;

namespace PITB.CRM_API.Controllers.LWMC.ResolverEnd
{
    public class LwmcSubmitStakeholderLoginController : ApiController
    {


        // POST api/<controller>
        public object Post([FromBody] JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SubmitStakeHolderLogin submitStakeholderLogin = JsonConvert.DeserializeObject<SubmitStakeHolderLogin>(actualJson);
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    LwmcResponseStakeholderLogin response = BlLwmcApiHandler.SubmitStakeholderLoginWithPhoneNo(submitStakeholderLogin, (Config.PlatformID)platformId);
                    return response; //JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    LwmcResponseStakeholderLogin response = new LwmcResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                    return response; //JsonConvert.SerializeObject(response);
                }
            }
            else
            {
                LwmcResponseStakeholderLogin resp = new LwmcResponseStakeholderLogin();
                resp.SetStatus(authModel);
                return resp; //JsonConvert.SerializeObject(resp); ;
            }
            /*else
            {
                LwmcResponseStakeholderLogin resp = new LwmcResponseStakeholderLogin();
                resp.SetAuthenticationError();
                return resp;
            }*/
        }
    }
}