using System;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;

namespace PITB.CRM_API.Controllers
{
    public class LwmcGetAllComplainantComplaintsController : ApiController
    {
        //[HttpGet]
        //public LocationMapping Test()
        //{
        //    LocationMapping mapping;
        //    LocationHandler.IsLocationExistInPolygon(new LocationCoordinate(31.257155, 74.222680), out mapping);
        //    return mapping;
       // }



        [HttpPost]
        public GetComplainantComplaintModel GetMyComplaints([FromBody]JToken data)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    SubmitGetComplaintModel model = JsonConvert.DeserializeObject<SubmitGetComplaintModel>(data.ToString());
                    return BlLwmcApiHandler.GetComplainantComplaints(
                       model);
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
            else
            {
                GetComplainantComplaintModel resp = new GetComplainantComplaintModel();
                resp.SetAuthenticationError();
                return resp;
            }
        }





    }
}