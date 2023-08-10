using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;
using PITB.CMS_Common.ApiModels.API;
using System;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation
{
    public class GetMeaStatusesController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public GetComplaintStatuses Get()
        {
            //return "value";
            try
            {
                return BlSchoolEducation.GetMeaStatuses();
            }
            catch (Exception ex)
            {
                return new GetComplaintStatuses()
                {
                    Status = Config.ResponseType.Failure.ToString(),
                    Message = ex.Message.ToString()
                };
            }
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
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