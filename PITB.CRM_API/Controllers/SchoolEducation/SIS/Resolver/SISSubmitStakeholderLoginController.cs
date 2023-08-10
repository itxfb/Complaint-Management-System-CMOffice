using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation.SIS.Resolver
{
    public class SISSubmitStakeholderLoginController : ApiController
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
        //public SEResponseStakeholderLogin Post([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        //{
        //    AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
        //    if (authModel.IsAuthenticated)
        //    //if(true)
        //    {
        //        string actualJson = jsonBody.ToString();
        //        string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
        //        Int64 apiRequestId = -1;
        //        try
        //        {
        //            SubmitSEStakeholderLogin submitStakeholderLogin =
        //                (SubmitSEStakeholderLogin)JsonConvert.DeserializeObject(actualJson, typeof(SubmitSEStakeholderLogin));
        //            apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
        //            //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
        //            return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
        //        }
        //        catch (Exception ex)
        //        {
        //            apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
        //            //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
        //            //     ex.Message);
        //            return new SEResponseStakeholderLogin(null, new ApiStatus(ex.InnerException.ToString(), "Server Error"));
                
        //            //return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
        //        }
        //    }
        //    else
        //    {
        //        SEResponseStakeholderLogin seResponse = new SEResponseStakeholderLogin();
        //        seResponse.Message = "Authentication Error";
        //        seResponse.Status = Config.ResponseType.Failure.ToString();
        //        return seResponse;
        //    }
        //}

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