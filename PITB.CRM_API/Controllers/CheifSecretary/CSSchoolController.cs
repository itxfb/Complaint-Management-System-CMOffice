using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiHandlers.Authentication;
using PITB.CMS_Common.ApiHandlers.Business.CSSchool;
using PITB.CMS_Common.ApiModels.API.CSSchool;
using PITB.CMS_Common.ApiModels.Authentication;

namespace PITB.CRM_API.Controllers.CheifSecretary
{
    [RoutePrefix("api/CSSchool")]
    public class CSSchoolController : ApiController
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
        [Route("SubmitComplaint")]
        public object SubmitComplaint([FromBody]string jsonBody)
        {
            AuthenticationModel authModel = ApiAuthenticationHandler.GetAuthenticationModel(Request);
            if (authModel.IsAuthenticated)
            {
                string actualJson = jsonBody.ToString();
                string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
                Int64 apiRequestId = -1;
                try
                {
                    CSSchoolApiModel.Request.AddComplaint addComplaint = (CSSchoolApiModel.Request.AddComplaint)JsonConvert.DeserializeObject(actualJson, typeof(CSSchoolApiModel.Request.AddComplaint));
                    //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                    return BlCSSchoolHandler.AddComplaint(addComplaint);
                }
                catch (Exception ex)
                {
                    return null;
                    //return new LwmcStakeholderComplaintResponse() { Status = Config.ResponseType.Failure.ToString(), Message = "Server Error" };
                }
            }
            else
            {
                CSSchoolApiModel.Response.AddComplaint resp = new CSSchoolApiModel.Response.AddComplaint(-1,null);
                resp.SetStatus(authModel);
                //resp.SetAuthenticationError();
                return resp;
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