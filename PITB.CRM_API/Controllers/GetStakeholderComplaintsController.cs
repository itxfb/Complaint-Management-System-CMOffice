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
    //[Authorize]
    public class GetStakeholderComplaintsController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public StakeholderComplaintsResponse Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int languageId = 0, [FromUri] int platformId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                GetStakeholderComplaintPostModel getStakeholderComplaintPostModel =
                    (GetStakeholderComplaintPostModel)JsonConvert.DeserializeObject(actualJson, typeof(GetStakeholderComplaintPostModel));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlApiStakeholderHander.GetStakeHolderComplaintsServerSideByUserName(getStakeholderComplaintPostModel.UserName, getStakeholderComplaintPostModel.Statuses, getStakeholderComplaintPostModel.StartRowIndex, (Config.Language)languageId, (Config.PlatformID)platformId);
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

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
