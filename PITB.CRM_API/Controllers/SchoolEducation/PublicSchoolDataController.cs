using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.SchoolEducation;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers.SchoolEducation
{
    [RoutePrefix("api/PublicSchool")]
    public class PublicSchoolDataController : System.Web.Http.ApiController
    {
         [AcceptVerbs("POST", "GET")]
         [Route("GetDistrictData/{appId:int?}")]
        public HttpResponseMessage GetDistrictData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
         {
             AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
             if (authModel.IsAuthenticated)
             {
                     var lObjdata = BlPublicSchools.GetDistrictList();
                     if (lObjdata != null)
                     {
                         HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, lObjdata);
                         return response;
                     }
                     else
                     {
                         HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);
                         return response;
                     }
             }
             else
             {
                 HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NonAuthoritativeInformation);
                 return response;
             }
         }

         [AcceptVerbs("POST", "GET")]
         [Route("GetCategoryData/{appId:int?}")]
         public BlPublicSchools.CategoryModelResponse GetCategoryData([FromBody]JToken jsonBody, [FromUri] int appId = 0)
         {
             //AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(jsonBody.ToString());
             AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
             if (authModel.IsAuthenticated)
             {
                 BlPublicSchools.CategoryModelResponse lObjdata = BlPublicSchools.GetCategoryData();
                     if (lObjdata != null)
                     {
                         lObjdata.Status = Config.ResponseType.Success.ToString();
                         lObjdata.Message = "Success";
                         return lObjdata;
                     }
                     else
                     {
                         BlPublicSchools.CategoryModelResponse categoryModelResponse = new BlPublicSchools.CategoryModelResponse();
                         categoryModelResponse.Status = Config.ResponseType.Failure.ToString();
                         categoryModelResponse.Message = "Server Error";
                         return categoryModelResponse;
                     }
             }
             else    
             {
                 BlPublicSchools.CategoryModelResponse categoryModelResponse = new BlPublicSchools.CategoryModelResponse();
                 categoryModelResponse.Status = Config.ResponseType.Failure.ToString();
                 categoryModelResponse.Message = "Authentication Error";
                 return categoryModelResponse;
             }
         }

         [AcceptVerbs("POST")]
         [Route("PostComplaint")]
         public ComplaintSubmitResponseSE PostComplaint([FromBody]JToken jsonBody, [FromUri] int appId = 0)
         {
             AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
             if (authModel.IsAuthenticated)
             {
                 string actualJson = jsonBody.ToString();
                 string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                 Int64 apiRequestId = -1;
                 try
                 {
                     SubmitSEComplaintModel submitComplaintModel =
                         (SubmitSEComplaintModel)JsonConvert.DeserializeObject(actualJson, typeof(SubmitSEComplaintModel));
                     apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    submitComplaintModel.complaintSrc = (int)Config.ComplaintSource.Public;
                    submitComplaintModel.tagId = "PublicSchools";
                     return BlSchoolEducation.SubmitComplaint(submitComplaintModel, apiRequestId, appId);
                 }
                 catch (Exception ex)
                 {
                     apiRequestId =
                         Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false,
                             ex.Message.ToString()));
                     return new ComplaintSubmitResponseSE(Config.ResponseType.Failure.ToString(), "Server Error", "");
                     //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
                 }
             }
             else
             {
                 ComplaintSubmitResponseSE complaintSubmitModel = new ComplaintSubmitResponseSE(Config.ResponseType.Failure.ToString(), "Authentication Error");
                 //complaintSubmitModel.Status = Config.ResponseType.Failure.ToString();
                 //complaintSubmitModel.Message = "Authentication Error";
                 return complaintSubmitModel;
             }

             //return new ApiStatus("Success", "Remarks has been posted successfully.");
         }


        

    }
}