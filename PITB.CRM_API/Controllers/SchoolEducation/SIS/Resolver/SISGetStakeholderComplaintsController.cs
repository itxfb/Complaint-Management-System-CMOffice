using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation.SIS.Resolver
{
    public class SISGetStakeholderComplaintsController : ApiController
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
        //public SEStakeholderComplaintResponseModel Post([FromBody]JToken jsonBody/*, [FromUri] int appId = 0*/, [FromUri] int languageId = 1/*, [FromUri] int platformId = 0*/)
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
        //            SEStakeholderComplaintsRequestModel seStakeholderComplaintsReqModel =
        //                (SEStakeholderComplaintsRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(SEStakeholderComplaintsRequestModel));
        //            //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));


        //            DateTime startDate = new DateTime(1970, 1, 1);
        //            DateTime endDate = DateTime.Now;

        //            string from = startDate.ToString("yyyy-MM-dd");
        //            string to = endDate.ToString("yyyy-MM-dd");

        //            return BlSchoolEducationResolver.GetStakeHolderComplaintsServerSideByUserNameDynamic(seStakeholderComplaintsReqModel.UserName, seStakeholderComplaintsReqModel.Statuses, seStakeholderComplaintsReqModel.StartRowIndex, from, to, (Config.Language)languageId/*, (Config.PlatformID)platformId*/);
        //        }
        //        catch (Exception ex)
        //        {
        //            apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
        //            //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
        //            //     ex.Message);
        //            //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "").ToString();
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        SEStakeholderComplaintResponseModel seResponse = new SEStakeholderComplaintResponseModel();
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
