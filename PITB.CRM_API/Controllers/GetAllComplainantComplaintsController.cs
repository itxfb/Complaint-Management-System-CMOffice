using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers
{
    public class GetAllComplainantComplaintsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public GetComplainantComplaintModel Get(string cnic, int appId, int languageId = 0, int campaignId = 0)
        {
            //return "value";
            try
            {
                return BlApiHandler.GetComplainantComplaints(cnic, appId, (Config.Language) languageId, campaignId);
            }
            catch (Exception ex)
            {
                return new GetComplainantComplaintModel()
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