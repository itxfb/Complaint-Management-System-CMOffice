using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;

namespace PITB.CRM_API.Controllers
{
    public class SyncController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public SyncModel Get(int id, int appId=0)
        {
            string submitComplaintModel = JsonConvert.SerializeObject(new SubmitComplaintModel());

            int currAppId = Convert.ToInt32(appId);
            SyncModel syncModel = null;
            if (currAppId == 0)
            {
                syncModel = BlApiHandler.GetSyncModelAgaistCampaignId(id);
            }
            else
            {
                syncModel = BlApiHandler.GetSyncModelAgaistCampaignIdWithUcAndWards(id, currAppId);
            }
            return syncModel;

            //string response = JsonConvert.SerializeObject(syncModel);
            //return response;
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