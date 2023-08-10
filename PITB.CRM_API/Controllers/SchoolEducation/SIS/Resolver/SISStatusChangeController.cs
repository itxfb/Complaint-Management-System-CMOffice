using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation.SIS.Resolver
{
    public class SISStatusChangeController : ApiController
    {
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
        //public ApiStatus Post([FromBody]JToken jsonBody)
        //{
        //    AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
        //    if (authModel.IsAuthenticated)
        //    //if (true)
        //    {
        //        string actualJson = jsonBody.ToString();
        //        string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
        //        Int64 apiRequestId = -1;
        //        try
        //        {
        //            SubmitSchoolEducationStatusModel submitStatusModel =
        //                (SubmitSchoolEducationStatusModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitSchoolEducationStatusModel));
        //            //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
        //            return SchoolEducationStatusHandler.ChangeStatus(submitStatusModel.UserName, Convert.ToInt32(submitStatusModel.ComplaintId), submitStatusModel.StatusId, submitStatusModel.StatusComments, submitStatusModel.PicturesList, apiRequestId);
        //        }
        //        catch (Exception ex)
        //        {
        //            apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
        //            //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
        //            //     ex.Message);
        //            return new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error");
        //            //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        return new ApiStatus(Config.ResponseType.Failure.ToString(), "Authentication Error");
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