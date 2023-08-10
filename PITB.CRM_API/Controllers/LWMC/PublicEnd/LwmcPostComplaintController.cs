using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business;
using PITB.CMS_Common.ApiHandlers.Business.LWMC;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiModels.Authentication;

namespace PITB.CRM_API.Controllers.LWMC
{
    public class LwmcPostComplaintController : ApiController
    {

        public ComplaintSubmitResponse Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId;
                try
                {
                    List<string> listMobileNoToBlock = new List<string>()
                    {
                        "35102296695",
                        "03174833701"
                    };
                    
                   
                    SubmitLWMCComplaintModel submitComplaintModel =
                        (SubmitLWMCComplaintModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitLWMCComplaintModel));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    if (submitComplaintModel.ModelIsValid()) //IMEI Check disabled since 20220322
                    {
                        // Check blocked no
                        if (listMobileNoToBlock.Contains(submitComplaintModel.personContactNumber.Trim()))
                        {
                            apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, "Mobile No = " + submitComplaintModel.personContactNumber + " Blocked"));
                            return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
                        }

                        if (string.IsNullOrEmpty(submitComplaintModel.lattitude) || submitComplaintModel.lattitude == "0" ||
                            string.IsNullOrEmpty(submitComplaintModel.longitude) || submitComplaintModel.longitude == "0")
                        {
                            return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Please turn on your location to submit complaint.", "");
                        
                        }

                        return BlLwmcApiHandler.SubmitComplaint(submitComplaintModel, apiRequestId, appId);


                    }
                    else
                    {
                        return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Please provide complete/valid parameters, consult documentation for detail", "");
                    }
                }
                catch (Exception ex)
                {
                   // System.IO.File.AppendAllText("C:\\Apps\\error.txt",JsonConvert.SerializeObject(ex));
                    apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message));
                    return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
                    //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                }
            }
            else
            {
                ComplaintSubmitResponse resp = new ComplaintSubmitResponse();
                resp.SetAuthenticationError();
                return resp;
            }


            //return new ApiStatus("Success", "Remarks has been posted successfully.");
        }


    }
}
