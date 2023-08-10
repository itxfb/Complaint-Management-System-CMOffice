using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Controllers
{
    public class GetStatusesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public List<DbStatuses> Get(int campaignId, int appId)
        {
            try
            {
                return DbStatuses.GetByCampaignId(campaignId);
            }
            catch (Exception exception)
            {
                return null;
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