using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;

using PITB.CMS_Common.ApiModels.API.SchoolEducation;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.Resolver;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.Complaint;

namespace PITB.CRM_API.Controllers.SchoolEducation.SIS
{

    [RoutePrefix("api")]
    public class SISApiController : ApiController
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
        [HttpPost]
        [Route("SISGetStakeholderStatuses")]
        public SISApiModel.Response.SEStakeholderStatusesModel SISGetStakeholderStatuses(string userName, int appId = 0, int languageId = 1, int platformId = 0, int appVersionId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if (true)
            {
                try
                {
                    return BlSchoolEducationResolver.GetStakeholderValidStatuses(userName, (Config.Language)languageId, (Config.AppID)appId, (Config.PlatformID)platformId, appVersionId);
                }
                catch (Exception exception)
                {
                    return null;
                }
            }
            else
            {
                SISApiModel.Response.SEStakeholderStatusesModel seResponse = new SISApiModel.Response.SEStakeholderStatusesModel();
                seResponse.Message = "Authentication Error";
                seResponse.Status = Config.ResponseType.Failure.ToString();
                return seResponse;
            }
        }

        [HttpPost]
        [Route("SISGetStakeholderComplaints")]
        public SISApiModel.Response.SEStakeholderComplaintResponseModel SISGetStakeholderComplaints([FromBody]JToken jsonBody/*, [FromUri] int appId = 0*/, [FromUri] int languageId = 1/*, [FromUri] int platformId = 0*/)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if (true)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SISApiModel.Request.SEStakeholderComplaintsRequestModel seStakeholderComplaintsReqModel =
                        (SISApiModel.Request.SEStakeholderComplaintsRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SEStakeholderComplaintsRequestModel));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));


                    DateTime startDate = new DateTime(1970, 1, 1);
                    DateTime endDate = DateTime.Now;

                    string from = startDate.ToString("yyyy-MM-dd");
                    string to = endDate.ToString("yyyy-MM-dd");

                    return BlSchoolEducationResolver.GetStakeHolderComplaintsServerSideByUserNameDynamic(seStakeholderComplaintsReqModel.UserName, seStakeholderComplaintsReqModel.Statuses, seStakeholderComplaintsReqModel.StartRowIndex, from, to, (Config.Language)languageId/*, (Config.PlatformID)platformId*/);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "").ToString();
                    return null;
                }
            }
            else
            {
                SISApiModel.Response.SEStakeholderComplaintResponseModel seResponse = new SISApiModel.Response.SEStakeholderComplaintResponseModel();
                seResponse.Message = "Authentication Error";
                seResponse.Status = Config.ResponseType.Failure.ToString();
                return seResponse;
            }
        }

        [HttpPost]
        [Route("SISStatusChange")]
        public ApiStatus SISStatusChange([FromBody]JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if (true)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SISApiModel.Request.SubmitSchoolEducationStatusModel submitStatusModel =
                        (SISApiModel.Request.SubmitSchoolEducationStatusModel)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SubmitSchoolEducationStatusModel));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return SchoolEducationStatusHandler.ChangeStatus(submitStatusModel.UserName, Convert.ToInt32(submitStatusModel.ComplaintId), submitStatusModel.StatusId, submitStatusModel.StatusComments, submitStatusModel.PicturesList, apiRequestId);
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
                return new ApiStatus(Config.ResponseType.Failure.ToString(), "Authentication Error");
            }
        }

        [HttpPost]
        [Route("SISSubmitStakeholderLogin")]
        public SEResponseStakeholderLogin SISSubmitStakeholderLogin([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if(true)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SISApiModel.Request.SubmitSEStakeholderLogin submitStakeholderLogin =
                        (SISApiModel.Request.SubmitSEStakeholderLogin)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SubmitSEStakeholderLogin));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
                    return BlSchoolEducationResolver.SubmitStakeholderLoginImeiNoRestriction(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return new SEResponseStakeholderLogin(null, new ApiStatus(ex.InnerException.ToString(), "Server Error"));

                    //return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                }
            }
            else
            {
                SEResponseStakeholderLogin seResponse = new SEResponseStakeholderLogin();
                seResponse.Message = "Authentication Error";
                seResponse.Status = Config.ResponseType.Failure.ToString();
                return seResponse;
            }
        }

        [HttpPost]
        [Route("SIS/AEOLogin")]
        public object AEOLogin([FromBody] JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string json = jsonBody.ToString();
                //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                //Int64 apiRequestId = -1;
                try
                {
                    dynamic request = Utility.DeserializeToDynamic(json);
                    return BlSchoolEducation.SubmitStakeholderLogin(request);
                }
                catch (Exception ex)
                {
                    return Utility.GetApiResponse(false, null, null, null);
                }
            }
            else
            {
                return Utility.GetApiResponse(false, authModel.Status, authModel.Message);
            }
        }

        [HttpPost]
        [Route("SISGetSchoolComplaints")]
        public SISApiModel.Response.SEGetComplaintAgainstEmisCodes SISGetSchoolComplaints([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if(true)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SISApiModel.Request.SEGetComplaintAgainstEmisCodes submitStakeholderLogin =
                        (SISApiModel.Request.SEGetComplaintAgainstEmisCodes)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SEGetComplaintAgainstEmisCodes));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
                    return BlSchoolEducation.GetComplaintsAgainstEmisCode(submitStakeholderLogin);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    SISApiModel.Response.SEGetComplaintAgainstEmisCodes resp = new SISApiModel.Response.SEGetComplaintAgainstEmisCodes();
                    resp.Status = ex.InnerException.ToString();
                    resp.Message = "Server Error";
                    //return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                }
            }
            else
            {
                SISApiModel.Response.SEGetComplaintAgainstEmisCodes seResponse = new SISApiModel.Response.SEGetComplaintAgainstEmisCodes();
                seResponse.Message = "Authentication Error";
                seResponse.Status = Config.ResponseType.Failure.ToString();
                return seResponse;
            }
            return null;
        }



        [HttpPost]
        [Route("SISGetUsersInfo")]
        public object SISGetUsersInfo([FromBody]JToken jsonBody, [FromUri] int appId = 0, [FromUri] int platformId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            //if(true)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    SISApiModel.Request.SISGetUsers submitStakeholderLogin =
                        (SISApiModel.Request.SISGetUsers)JsonConvert.DeserializeObject(actualJson, typeof(SISApiModel.Request.SISGetUsers));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    //return BlSchoolEducationResolver.SubmitStakeholderLogin(submitStakeholderLogin, (Config.PlatformID)platformId);
                    return BlSchoolEducationResolver.GetUsers(submitStakeholderLogin, (Config.PlatformID)platformId);
                }
                catch (Exception ex)
                {
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                    //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                    //     ex.Message);
                    return new SEResponseStakeholderLogin(null, new ApiStatus(ex.InnerException.ToString(), "Server Error"));

                    //return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                }
            }
            else
            {
                SEResponseStakeholderLogin seResponse = new SEResponseStakeholderLogin();
                seResponse.Message = "Authentication Error";
                seResponse.Status = Config.ResponseType.Failure.ToString();
                return seResponse;
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