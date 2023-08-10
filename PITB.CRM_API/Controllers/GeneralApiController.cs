using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiModels.Authentication;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Modules.Api.Authentication;
using PITB.CMS_Common.Modules.Api.Response;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace PITB.CRM_API.Controllers
{
    //[RoutePrefix("api")]
    public class GeneralApiController : ApiController
    {
        //[AcceptVerbs("POST")]
        [HttpPost]
        [Route("api/PrivateSchool/SubmitComplaint")]
        [Route("api/PrivateSchool/GetComplaints")]
        [Route("api/AeoApp/SubmitComplaint")]
        [Route("api/AeoApp/GetComplaints")]
        [Route("api/eTransferSIS/GetCategories")]
        [Route("api/eTransferSIS/SubmitComplaint")]
        [Route("api/eTransferSIS/GetComplaints")]
        //[Route("api/PublicUser/Login")]
        //[Route("api/PublicUser/LoadForm")]
        
        public object PostRequest([FromBody] JToken jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string url = Request.RequestUri.LocalPath.ToString();
                //string uri = url.Substring(1, url.Length - 1);
                RouteAttribute[] attrArr = (RouteAttribute[])this.GetType().GetMethod("PostRequest").GetCustomAttributes(typeof(System.Web.Http.RouteAttribute));
                string routeUri = attrArr.Where(n => url.IndexOf(n.Template) != -1).FirstOrDefault().Template;
                //string attr = myAttribute.Template;
                //RouteData routeData = HttpContext.Current.Request.RequestContext.RouteData;
                //object sd = routeData.Values["controller"];
                //object sd2 = routeData.Values["action"];
                //routeData.Values;
                //string CurrentAction = (string)this.RouteData.Values["action"];
                //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                //Int64 apiRequestId = -1;
                try
                {
                    string json = (jsonBody == null) ? "{}": jsonBody.ToString();
                    dynamic request =  Utility.DeserializeToDynamic(json);
                    //Request.GetUrlHelper();
                    request.Request = Request;
                    //url = Request.RequestUri.LocalPath.ToString();
                    //return FunctionInvoker.InvokeFunction(url.Substring(1,url.Length-1), json);
                    return FunctionInvoker.InvokeFunction(routeUri, json, HttpContext.Current.Request);
                    //dynamic req2 = JToken.Parse(json);
                    //return BlCombinedCampaign.SubmitComplaint(request);
                    //return null;
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
        [Route("api/PublicUser/Login")]
        [Route("api/PublicUser/AppStart")]
        [Route("api/PublicUser/GetComplaints")]
        [Route("api/PublicUser/PostSignupForm")]
        [Route("api/PublicUser/PostComplaint")]
        [Route("api/PublicUser/PostStatus")]
        [Route("api/PublicUser/PostChangeTimeAndCategory")]
        //[Route("api/PublicUser/GetUserSignupOTP")]
        public object PostRequestMobile(/*[FromBody] JToken jsonBody*/)
        {
            string url = null;
            try
            {
                dynamic authModel = ApiAuthenticationMobile.GetAuthentication(Request);
                if (authModel.authStatus == 1)
                {
                    url = Request.RequestUri.LocalPath.ToString();
                    //string uri = url.Substring(1, url.Length - 1);
                    RouteAttribute[] attrArr = (RouteAttribute[])this.GetType().GetMethod("PostRequestMobile").GetCustomAttributes(typeof(System.Web.Http.RouteAttribute));
                    string routeUri = attrArr.Where(n => url.IndexOf(n.Template) != -1).FirstOrDefault().Template;

                    CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                    try
                    {
                        //string json = (jsonBody == null) ? "{}" : jsonBody.ToString();
                        //dynamic request = Utility.DeserializeToDynamic(json);
                        //request.Request = Request;
                        return FunctionInvoker.InvokeFunctionWithDynamicParam(routeUri, postForm);
                    }
                    catch (Exception ex)
                    {
                        //return ex.StackTrace.ToString() + " route uri = "+  url;
                        return ApiResponseHandlerMobile.SetServerError(null);
                    }
                }
                else
                {
                    return authModel.authResponse;
                }
            }
            catch(Exception ex)
            {
                //return ex.StackTrace.ToString() + " route uri = " + url;
                return ApiResponseHandlerMobile.SetServerError(null);
            }
        }
    }
}
