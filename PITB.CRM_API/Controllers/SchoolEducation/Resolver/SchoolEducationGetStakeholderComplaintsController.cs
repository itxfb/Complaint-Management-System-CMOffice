using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers.SchoolEducation.Resolver
{
    public class SchoolEducationGetStakeholderComplaintsController : ApiController
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
        public SISApiModel.Response.SEStakeholderComplaintResponseModel Post([FromBody]JToken jsonBody/*, [FromUri] int appId = 0*/, [FromUri] int languageId = 1/*, [FromUri] int platformId = 0*/)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                SISApiModel.Request.SEStakeholderComplaintsRequestModel seStakeholderComplaintsReqModel =
                    (SISApiModel.Request.SEStakeholderComplaintsRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SEStakeholderComplaintsRequestModel));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));


                DateTime startDate = new DateTime(1970,1,1);
                DateTime endDate = DateTime.Now;
           
                string from = startDate.ToString("yyyy-MM-dd");
                string to = endDate.ToString("yyyy-MM-dd");

                return BlSchoolEducationResolver.GetStakeHolderComplaintsServerSideByUserNameDynamic(seStakeholderComplaintsReqModel.UserName, seStakeholderComplaintsReqModel.Statuses, seStakeholderComplaintsReqModel.StartRowIndex, from, to, (Config.Language)languageId/*, (Config.PlatformID)platformId*/);
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