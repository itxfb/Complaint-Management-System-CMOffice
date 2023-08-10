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
    public class LwmcSubmitComplaintStatusController : ApiController
    {
       // POST api/<controller>
        public object Post([FromBody]JToken jsonBody, [FromUri] int appId = 2)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SubmitStatusModel submitStatusModel =
                        (SubmitStatusModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitStatusModel));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    return BlLwmcApiStakeholderHander.ChangeStatus(submitStatusModel.UserName, Convert.ToInt32(submitStatusModel.ComplaintId), submitStatusModel.StatusId, submitStatusModel.StatusComments, submitStatusModel.PicturesList, apiRequestId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error");
                    //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                }
            }
            else
            {
                ApiStatus resp = new ApiStatus();
                //resp.SetAuthenticationError();
                resp.SetStatus(authModel);
                return resp;
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}