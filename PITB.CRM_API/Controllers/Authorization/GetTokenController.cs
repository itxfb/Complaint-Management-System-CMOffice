﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API.Authentication;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.Sync.Request;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.Sync.Response;

namespace PITB.CRM_API.Controllers.Authorization
{
    public class GetTokenController : ApiController
    {
        // GET: api/GetToken
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetToken/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GetToken
        public ApiToken.Response Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                string headerStr = Utility.GetHeadersString(HttpContext.Current.Request);
                SyncRequest.SyncUsersReq syncUserData =
                    (SyncRequest.SyncUsersReq)JsonConvert.DeserializeObject(actualJson, typeof(SyncRequest.SyncUsersReq));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
                SyncResponse.UserDataResponse userDataResponse = new SyncResponse.UserDataResponse();
                userDataResponse.ListUserData = BlSchoolEducation.GetUsersData(syncUserData.campaignId, syncUserData.userType, syncUserData.from, syncUserData.to);
                userDataResponse.Message = Config.ResponseType.Success.ToString();
                userDataResponse.Status = Config.ResponseType.Success.ToString();
                return null;
                //return userDataResponse;
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);

                return null;
                //return new SyncResponse.UserDataResponse { Message = "Server Error", Status = Config.ResponseType.Failure.ToString() };

                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }
            return null;
        }

        // PUT: api/GetToken/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GetToken/5
        public void Delete(int id)
        {
        }
    }
}
