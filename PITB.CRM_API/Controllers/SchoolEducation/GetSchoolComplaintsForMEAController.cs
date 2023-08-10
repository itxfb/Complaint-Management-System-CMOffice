using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;

namespace PITB.CRM_API.Controllers
{
    public class GetSchoolComplaintsForMEAController : ApiController
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
        public GetSchoolEducationComplaintModel Post([FromBody] JToken jsonBody)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId;
            try
            {
                SchoolApiModel submitComplaintModel =
                    (SchoolApiModel)JsonConvert.DeserializeObject(actualJson, typeof(SchoolApiModel));

                return BlSchoolEducation.GetSchoolComplaintsForMea(submitComplaintModel);
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                //return BlLwmcApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId);
            }
            catch (Exception ex)
            {
                return new GetSchoolEducationComplaintModel()
                {
                    Status = Config.ResponseType.Failure.ToString(),
                    Message = ex.Message.ToString()
                };
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