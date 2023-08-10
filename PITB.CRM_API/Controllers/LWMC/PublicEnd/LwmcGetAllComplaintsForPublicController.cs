using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CRM_API.Controllers.LWMC
{
    public class LwmcGetAllComplaintsForPublicController : ApiController
    {
        [Route("~/api/LwmcGetAllComplaintsForPublic/GetAll")]
        [HttpPost]
        public GetComplainantComplaintModel GetAll([FromBody]JToken data)
        {
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    string exception = string.Empty;
                  //  BlApiHandler.StoreApiRequestInDb(data.ToString(), ipAddress, true, null);


                    SubmitGetComplaintModel model = JsonConvert.DeserializeObject<SubmitGetComplaintModel>(data.ToString());
                    //BlApiHandler.StoreApiRequestInDb(JsonConvert.SerializeObject(model), ipAddress, true, null);


                    if (string.IsNullOrEmpty(model.StartDate) || string.IsNullOrEmpty(model.EndDate))
                    {
                        return new GetComplainantComplaintModel()
                        {
                            Status = Config.ResponseType.Failure.ToString(),
                            Message = "Please provide date parameters"
                        };
                    }

                    if (!model.ModelIsValid())
                    {
                        return new GetComplainantComplaintModel()
                        {
                            Status = Config.ResponseType.Failure.ToString(),
                            Message = "Please provide complete/valid parameters, see documentation for details"
                        };
                    }
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    return BlLwmcApiHandler.GetComplainantComplaints(
                    model);
                }
                catch (Exception ex)
                {
                  //  Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(data.ToString(), ipAddress,false, (ex.InnerException !=null) ? ex.InnerException.Message : ex.Message));

                    return new GetComplainantComplaintModel()
                    {
                        Status = Config.ResponseType.Failure.ToString(),
                        Message = ex.Message
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

        [Route("~/api/LwmcGetAllComplaintsForPublic/PostMyVote")]

        [HttpPost]
        public ApiStatus PostMyVote([FromBody]JToken postedString)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    IncomingSocialSubmitModel model = JsonConvert.DeserializeObject<IncomingSocialSubmitModel>(postedString.ToString());
                    return BlLwmcApiHandler.SubmitVoteForComplaint(model);
                }
                catch (Exception ex)
                {
                    return new ApiStatus()
                    {
                        Status = Config.ResponseType.Failure.ToString(),
                        Message = ex.Message
                    };
                }
            }
            else
            {
                ApiStatus resp = new ApiStatus();
                resp.SetAuthenticationError();
                return resp;
            }
        }


        [Route("~/api/LwmcGetAllComplaintsForPublic/SharePost")]

        [HttpPost]
        public ApiStatus SharePost([FromBody]JToken postedString)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                try
                {
                    IncomingSocialSubmitModel model = JsonConvert.DeserializeObject<IncomingSocialSubmitModel>(postedString.ToString());
                    return BlLwmcApiHandler.SubmitSharePost(model);
                }
                catch (Exception ex)
                {
                    return new ApiStatus()
                    {
                        Status = Config.ResponseType.Failure.ToString(),
                        Message = ex.Message
                    };
                }
            }
            else
            {
                ApiStatus resp = new ApiStatus();
                resp.SetAuthenticationError();
                return resp;
            }
        }
    }
}
