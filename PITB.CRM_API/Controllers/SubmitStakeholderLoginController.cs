using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CRM_API.Controllers
{
    public class SubmitStakeholderLoginController : ApiController
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
        public ResponseStakeholderLogin Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SubmitStakeHolderLogin submitStakeholderLogin =
                    (SubmitStakeHolderLogin)JsonConvert.DeserializeObject(actualJson, typeof(SubmitStakeHolderLogin));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID) platformId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return new ResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
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