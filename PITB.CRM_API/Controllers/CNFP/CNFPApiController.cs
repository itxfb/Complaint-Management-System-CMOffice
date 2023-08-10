using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiModels.API.CNFP;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CRM_API.Controllers.CNFP
{
    [RoutePrefix("api")]
    public class CNFPApiController : ApiController
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

        [HttpPost]
        [Route("CNFP/PostFeedback")]
        // POST api/<controller>
        public object PostFeedback([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                Int64 apiRequestId = -1;
                try
                {
                    CNFPApiModel.Request.PostFeedback postFeedback =
                        (CNFPApiModel.Request.PostFeedback)
                            JsonConvert.DeserializeObject(actualJson, typeof(CNFPApiModel.Request.PostFeedback));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    return BlCNFP.SetCnfpComplainantFeedback(postFeedback);
                    //return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, false, ex.Message.ToString()));
                    CNFPApiModel.Response.PostFeedback respFeedback = new CNFPApiModel.Response.PostFeedback();
                    respFeedback.SetFailure();
                    return respFeedback;
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