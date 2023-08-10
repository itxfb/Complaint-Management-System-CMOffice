using System;
using System.Globalization;
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

namespace PITB.CRM_API.Controllers.LWMC.ResolverEnd
{
    //[Authorize]
    public class LwmcGetStakeholderComplaintsController : ApiController
    {
       
        // POST api/values
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
                    GetStakeholderComplaintPostModel getStakeholderComplaintPostModel =
                        (GetStakeholderComplaintPostModel)JsonConvert.DeserializeObject(actualJson, typeof(GetStakeholderComplaintPostModel));
                    //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    
                    DateTime startDate, endDate;
                    DateTime.TryParseExact(getStakeholderComplaintPostModel.StartDate, "dd/MM/yyyy", null,DateTimeStyles.None, out startDate);
                    DateTime.TryParseExact(getStakeholderComplaintPostModel.EndDate, "dd/MM/yyyy", null, DateTimeStyles.None, out endDate);
                    string from = startDate.ToString("yyyy-MM-dd");
                    string to = endDate.ToString("yyyy-MM-dd");
                    return BlLwmcApiStakeholderHander.GetStakeHolderComplaintsServerSideByUserNameDynamic(getStakeholderComplaintPostModel.fcm_key,getStakeholderComplaintPostModel.UserName, getStakeholderComplaintPostModel.Statuses, getStakeholderComplaintPostModel.StartRowIndex, from, to, (Config.Language)languageId, (Config.PlatformID)platformId);
                    //StakeholderListingLogic.GetListingQuery()
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "").ToString();
                    return null;
                }
            }
            else
            {
                LwmcStakeholderComplaintResponse resp = new LwmcStakeholderComplaintResponse();
                //resp.SetAuthenticationError();
                resp.SetStatus(authModel);
                return resp;
            }
        }
    }
}
