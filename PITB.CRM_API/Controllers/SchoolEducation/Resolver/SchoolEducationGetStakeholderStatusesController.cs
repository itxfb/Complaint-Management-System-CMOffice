using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;
using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation.Resolver
{
    public class SchoolEducationGetStakeholderStatusesController : ApiController
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
        public SISApiModel.Response.SEStakeholderStatusesModel Post(string userName, int appId = 0, int languageId = 1, int platformId = 0, int appVersionId = 0)
        {
            try
            {
                return BlSchoolEducationResolver.GetStakeholderValidStatuses(userName, (Config.Language)languageId, (Config.AppID)appId, (Config.PlatformID)platformId, appVersionId);
            }
            catch (Exception exception)
            {
                SISApiModel.Response.SEStakeholderStatusesModel se = new SISApiModel.Response.SEStakeholderStatusesModel();
                se.SetFailure();
                return se;
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