using Newtonsoft.Json.Linq;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers
{
    public class ReplyMsgController : ApiController
    {
        // GET: api/ReplyMsg
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ReplyMsg/5
        public ReplyGroupModel Get(int id)
        {
            ReplyGroupModel replyGroupModel = BlMsgApiHandler.GetReplyMessages(id);
            return replyGroupModel;
        }

        // POST: api/ReplyMsg
        public void Post([FromBody]JToken jsonBody)
        {
        }

        // PUT: api/ReplyMsg/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ReplyMsg/5
        public void Delete(int id)
        {
        }
    }
}
