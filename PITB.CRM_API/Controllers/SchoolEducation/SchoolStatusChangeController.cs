using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.Handler.Complaint;

namespace PITB.CRM_API.Controllers.SchoolEducation
{
    public class SchoolStatusChangeController : ApiController
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
                SISApiModel.Request.SubmitSchoolEducationStatusModel submitStatusModel =
                    (SISApiModel.Request.SubmitSchoolEducationStatusModel)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SubmitSchoolEducationStatusModel));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return SchoolEducationStatusHandler.ChangeStatus(submitStatusModel.UserName, Convert.ToInt32(submitStatusModel.ComplaintId), submitStatusModel.StatusId, submitStatusModel.StatusComments, submitStatusModel.PicturesList, apiRequestId);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                return new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error");
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