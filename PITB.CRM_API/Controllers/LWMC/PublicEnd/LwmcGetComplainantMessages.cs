using System;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;

namespace PITB.CRM_API.Controllers.LWMC.PublicEnd
{
    public class LwmcGetComplainantMessagesController : ApiController
    {
        //[HttpGet]
        //public LocationMapping Test()
        //{
        //    LocationMapping mapping;
        //    LocationHandler.IsLocationExistInPolygon(new LocationCoordinate(31.257155, 74.222680), out mapping);
        //    return mapping;
        // }



        [HttpPost]
        public LwmcResponse.ComplaintantMessages Post([FromBody] JToken data)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    SubmitGetComplaintModel model = JsonConvert.DeserializeObject<SubmitGetComplaintModel>(data.ToString());
                    return BlLwmcApiHandler.GetComplainantMessages();
                }
                catch (Exception ex)
                {
                    LwmcResponse.ComplaintantMessages resp = new LwmcResponse.ComplaintantMessages();
                    resp.SetFailure();
                    return resp;
                }
            }
            else
            {
                LwmcResponse.ComplaintantMessages resp = new LwmcResponse.ComplaintantMessages();
                resp.SetAuthenticationError();
                return resp;
            }
        }
    }
}