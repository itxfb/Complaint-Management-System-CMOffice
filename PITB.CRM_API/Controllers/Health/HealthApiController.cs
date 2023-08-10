using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PITB.CMS_Common.ApiModels.API.Health;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CRM_API.Controllers.Health
{
    public class HealthApiController : ApiController
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
        [Route("Health/AddComplaint")]
        // POST api/<controller>
        public HealthResponseModel.SubmitComplaint AddComplaint([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    HealthRequestModel.SubmitComplaint submitComplaintRequest =
                        (HealthRequestModel.SubmitComplaint)
                            JsonConvert.DeserializeObject(actualJson, typeof(HealthRequestModel.SubmitComplaint));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return BlHealth.SubmitComplaint(submitComplaintRequest, apiRequestId);
                    //return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    return new HealthResponseModel.SubmitComplaint(Config.ResponseType.Failure.ToString(), "Server Error");
                }
            }
            else
            {
                return new HealthResponseModel.SubmitComplaint(Config.ResponseType.Failure.ToString(), "Authentication Error");
            }
        }

        [HttpPost]
        [Route("Health/ChangeStatus")]
        public ApiStatus ChangeStatus([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    HealthRequestModel.SubmitStatus submitStatusModel =
                        (HealthRequestModel.SubmitStatus)JsonConvert.DeserializeObject(actualJson, typeof(HealthRequestModel.SubmitStatus));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbUsers dbUser = DbUsers.GetActiveUser(31939);
                    return BlHealth.ChangeStatus(dbUser, Convert.ToInt32(submitStatusModel.complaintId), submitStatusModel.createdDateTime, submitStatusModel.statusId, submitStatusModel.statusComments, submitStatusModel.picturesList, apiRequestId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return new ApiStatus(Config.ResponseType.Failure.ToString() + ex.StackTrace.ToString(), "Server Error");
                    //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                }
            }
            else
            {
                return new HealthResponseModel.SubmitComplaint(Config.ResponseType.Failure.ToString(), "Authentication Error");
            }
        }


        // POST api/<controller>
        public void Post([FromBody]string value)
        {
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