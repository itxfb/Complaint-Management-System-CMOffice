using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers
{
    public class SubmitComplainantLoginController : ApiController
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
        public SyncModel Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0,  [FromUri] int dbVersionId=0, [FromUri] int platformId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SubmitComplainantLogin submitComplainantLogin =
                    (SubmitComplainantLogin)JsonConvert.DeserializeObject(actualJson, typeof(SubmitComplainantLogin));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiHandler.SubmitComplainantLogin(submitComplainantLogin, appId, (Config.PlatformID)platformId, (Config.Language)languageId, dbVersionId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return new SyncModel() { Status = Config.ResponseType.Failure.ToString(), Message = "Server Error"};
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