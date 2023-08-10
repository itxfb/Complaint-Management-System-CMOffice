using System;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.Resolver;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;

namespace PITB.CRM_API.Controllers.SchoolEducation.Resolver
{
    public class SchoolEducationSubmitStakeholderLoginController : ApiController
    {
        // GET api/<controller>
        public SEResponseStakeholderLogin Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SISApiModel.Request.SubmitSEStakeholderLogin submitStakeholderLogin =
                    (SISApiModel.Request.SubmitSEStakeholderLogin)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SubmitSEStakeholderLogin));
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
                return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }
        }
    }
}