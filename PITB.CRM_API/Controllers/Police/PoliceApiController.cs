using System;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom.Police;

namespace PITB.CRM_API.Controllers.Police
{
    [RoutePrefix("api")]
    public class PoliceApiController : ApiController
    {
        [HttpPost]
        [Route("Police/AddComplaint")]
        public object AddComplaint([FromBody] JToken jsonBody)
        {

            //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    PoliceModel.AddComplaint addComplaintModel =
                        (PoliceModel.AddComplaint)
                            JsonConvert.DeserializeObject(actualJson, typeof(PoliceModel.AddComplaint));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    Config.CommandMessage response =
                         BlPolice.AddComplaint(addComplaintModel);
                    return response; //JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    ApiStatus response = new LwmcResponseStakeholderLogin(null,
                        new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                    return response;
                }
            }
            else
            {
                ApiStatus resp = new LwmcResponseStakeholderLogin();
                resp.SetStatus(authModel);
                return resp; //JsonConvert.SerializeObject(resp); ;
            }
        }

        [HttpPost]
        [Route("Police/PostAction")]
        public object PostAction([FromBody] JToken jsonBody)
        {

            //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    PoliceModel.AddAction addComplaintAction =
                        (PoliceModel.AddAction)
                            JsonConvert.DeserializeObject(actualJson, typeof(PoliceModel.AddAction), Utility.GetJsonSettings());
                    Config.CommandMessage response = BlPolice.SaveComplaintAction(addComplaintAction /*new CustomForm.Post(Request)*/);
                    return response; //JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    ApiStatus response = new LwmcResponseStakeholderLogin(null,
                        new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                    return response;
                }
            }
            else
            {
                ApiStatus resp = new LwmcResponseStakeholderLogin();
                resp.SetStatus(authModel);
                return resp; //JsonConvert.SerializeObject(resp); ;
            }
        }

        [HttpPost]
        [Route("Police/PostFeedback")]
        public object PostFeedback([FromBody] JToken jsonBody)
        {

            //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    PoliceModel.AddFeedback addComplaintFeedback =
                        (PoliceModel.AddFeedback)
                            JsonConvert.DeserializeObject(actualJson, typeof(PoliceModel.AddFeedback), Utility.GetJsonSettings());
                    Config.CommandMessage response = BlPolice.SubmitComplaintFeedback(addComplaintFeedback /*new CustomForm.Post(Request)*/);
                    return response; //JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    ApiStatus response = new LwmcResponseStakeholderLogin(null,
                        new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                    return response;
                }
            }
            else
            {
                ApiStatus resp = new LwmcResponseStakeholderLogin();
                resp.SetStatus(authModel);
                return resp; //JsonConvert.SerializeObject(resp); ;
            }
        }




        [HttpPost]
        [Route("Police/ChangeComplaintDistrict")]
        public object ChangeComplaintDistrict([FromBody] JToken jsonBody)
        {

            //DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    PoliceModel.AddComplaint addComplaintModel =
                        (PoliceModel.AddComplaint)
                            JsonConvert.DeserializeObject(actualJson, typeof(PoliceModel.AddComplaint));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    DbApiRequestLogs.Save(new DbApiRequestLogs(HttpContext.Current.Request, Request, true, null));
                    Config.CommandMessage response =
                        BlPolice.AddComplaint(addComplaintModel);
                    return response; //JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    apiRequestId =
                        Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                            ex.Message.ToString()));
                    ApiStatus response = new LwmcResponseStakeholderLogin(null,
                        new ApiStatus(Config.ResponseType.Failure.ToString(), "Server Error"));
                    return response;
                }
            }
            else
            {
                ApiStatus resp = new LwmcResponseStakeholderLogin();
                resp.SetStatus(authModel);
                return resp; //JsonConvert.SerializeObject(resp); ;
            }
        }
    }
}
