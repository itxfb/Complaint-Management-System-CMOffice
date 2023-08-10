using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers
{
    public class GetStakeholderStatusesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public StatusList Get(string userName, int appId, int languageId = 0, int platformId = 0, int appVersionId = 0)
        {
            try
            {
                return BlApiHandler.GetStakeholderValidStatuses(userName, (Config.Language) languageId, (Config.AppID) appId , (Config.PlatformID) platformId, appVersionId);
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