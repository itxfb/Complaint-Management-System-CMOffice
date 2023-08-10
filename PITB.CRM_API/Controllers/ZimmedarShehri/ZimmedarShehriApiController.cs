using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint.Status;

namespace PITB.CRM_API.Controllers.ZimmedarShehri
{
    [RoutePrefix("api")]
    public class ZimmedarShehriApiController : ApiController
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

        [HttpPost]
        [Route("ZimmedarShehri/Login")]
        // POST api/<controller>
        public ZimmedarShehriModel.LoginResponseModel Login([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    ZimmedarShehriModel.LoginRequest submitStakeholderLoginRequest =
                        (ZimmedarShehriModel.LoginRequest)
                            JsonConvert.DeserializeObject(actualJson, typeof(ZimmedarShehriModel.LoginRequest));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return BlZimmedarShehri.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLoginRequest,
                        (Config.PlatformID) platformId);
                    //return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    return new ZimmedarShehriModel.LoginResponseModel(null,
                        new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                }
            }
            else
            {
                return new ZimmedarShehriModel.LoginResponseModel(null,
                     new ApiStatus(Config.ResponseType.Failure.ToString(), "Authentication Error"));
            }
        }





        [HttpPost]
        [Route("ZimmedarShehri/ChangeStatus")]
        // POST api/<controller>
        public ApiStatus ChangeStatus([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                   ZimmedarShehriModel.StatusRequestModel submitStatusModel =
                  (ZimmedarShehriModel.StatusRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(ZimmedarShehriModel.StatusRequestModel));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return StatusHandler.ChangeStatus(submitStatusModel.UserName, Convert.ToInt32(submitStatusModel.ComplaintId), submitStatusModel.StatusId, submitStatusModel.StatusComments, submitStatusModel.PicturesList, apiRequestId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error");
                    //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                }
            }
            else
            {
                return new ZimmedarShehriModel.LoginResponseModel(null,
                     new ApiStatus(Config.ResponseType.Failure.ToString(), "Authentication Error"));
            }
        }


        [HttpPost]
        [Route("ZimmedarShehri/GetComplaints")]
        // POST api/<controller>
        public StakeholderComplaintsResponse GetComplaints([FromBody]JToken jsonBody, [FromUri] int languageId = 1, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    ZimmedarShehriModel.ComplaintsRequestModel complaintReqModel =
                        (ZimmedarShehriModel.ComplaintsRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(ZimmedarShehriModel.ComplaintsRequestModel));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));


                    DateTime startDate = new DateTime(1970, 1, 1);
                    DateTime endDate = DateTime.Now;

                    string from = startDate.ToString("yyyy-MM-dd");
                    string to = endDate.ToString("yyyy-MM-dd");

                    return BlApiStakeholderHander.GetStakeHolderComplaintsServerSideByUserName(complaintReqModel.UserName, complaintReqModel.Statuses, complaintReqModel.StartRowIndex, (Config.Language)languageId, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    StakeholderComplaintsResponse stakeholderResponse = new StakeholderComplaintsResponse();
                    stakeholderResponse.Status = Config.ResponseType.Failure.ToString();
                    stakeholderResponse.Message = "Server Error";
                    return stakeholderResponse;
                }
            }
            else
            {
                StakeholderComplaintsResponse stakeholderResponse = new StakeholderComplaintsResponse();
                stakeholderResponse.Status = Config.ResponseType.Failure.ToString();
                stakeholderResponse.Message = "Authentication Error";
                return stakeholderResponse;
            }
        }



        [HttpPost]
        [Route("ZimmedarShehri/GetStatuses")]
        // POST api/<controller>
        public ZimmedarShehriModel.StatusesModelResponse GetStatuses(string userName, int appId = 0, int languageId = 1, int platformId = 0, int appVersionId = 0)
        {

            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {

                try
                {
                    return BlZimmedarShehri.GetStakeholderValidStatuses(userName, (Config.Language)languageId, (Config.AppID)appId, (Config.PlatformID)platformId, appVersionId);
                }
                catch (Exception exception)
                {
                    ZimmedarShehriModel.StatusesModelResponse statusResponse = new ZimmedarShehriModel.StatusesModelResponse();
                    statusResponse.Status = Config.ResponseType.Failure.ToString();
                    statusResponse.Message = "Server Error";
                    return statusResponse;
                }
            }
            else
            {
                ZimmedarShehriModel.StatusesModelResponse statusResponse = new ZimmedarShehriModel.StatusesModelResponse();
                statusResponse.Status = Config.ResponseType.Failure.ToString();
                statusResponse.Message = "Authentication Error";
                return statusResponse;
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