using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers
{
    public class IncomingMsgSubmitController : ApiController
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
        public ApiStatus Post([FromBody]JToken jsonBody)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                IncomingMsgGroupModel submitIncomingMsgs =
                    (IncomingMsgGroupModel)JsonConvert.DeserializeObject(actualJson, typeof(IncomingMsgGroupModel));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null,false));
                return BlMsgApiHandler.SubmitIncomingMsgs(submitIncomingMsgs, apiRequestId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                return new ApiStatus(Config.ResponseType.Failure.ToString(),
                     ex.Message);
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
