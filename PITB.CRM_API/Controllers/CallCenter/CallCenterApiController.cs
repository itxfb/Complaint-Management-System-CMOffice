using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PITB.CMS_Common.ApiModels.API.CallCenter;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers.CallCenter
{
    [RoutePrefix("api")]
    public class CallCenterApiController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
       [HttpPost]
       [Route("CallCenter/SubmitLandedCallLogs")]
        // POST api/<controller>
        public CallCenterModel.Response.SubmitLandedCallLogs SubmitLandedCallLogs([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    CallCenterModel.Request.SubmitLandedCallLogs submitLandedCallLogs =
                        (CallCenterModel.Request.SubmitLandedCallLogs)
                            JsonConvert.DeserializeObject(actualJson, typeof(CallCenterModel.Request.SubmitLandedCallLogs));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return BlCallCenter.SubmitCallLandedStats(submitLandedCallLogs);
                    //return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                   CallCenterModel.Response.SubmitLandedCallLogs resp = new CallCenterModel.Response.SubmitLandedCallLogs();
                    resp.SetFailure();
                   return resp;
                }
            }
            else
            {
                CallCenterModel.Response.SubmitLandedCallLogs resp = new CallCenterModel.Response.SubmitLandedCallLogs();
                resp.SetAuthenticationError();
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