using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Custom;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers
{
    public class ReplyUpdateGroupController : ApiController
    {
        // GET: api/ReplyUpdateGroup
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ReplyUpdateGroup/5
        public ApiStatus Get(int id)
        {
            ApiStatus apiStatus = BlMsgApiHandler.SubmitSentMessageStatus(id);
            return apiStatus;
        }

        // POST: api/ReplyUpdateGroup
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ReplyUpdateGroup/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ReplyUpdateGroup/5
        public void Delete(int id)
        {
        }
    }
}
