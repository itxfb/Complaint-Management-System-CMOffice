using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API.FixitNew.CFNP;
using PITB.CMS_Common.ApiHandlers.Business.FixitNew;
using PITB.CMS_Common.ApiHandlers.Business;

namespace PITB.CRM_API.Controllers.FixitNew.CFMP
{
    public class PostFixitComplaintFeedbackCFNPController : ApiController
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
        public ApiStatus Post([FromBody]JToken jsonBody/*, [FromUri] int appId = 0*/, [FromUri] int languageId = 1/*, [FromUri] int platformId = 0*/)
        {
            string actualJson = jsonBody.ToString();
            string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                RequestCNFPComplaintFeedbackModel reqCnfpFeedbackModel =
                    (RequestCNFPComplaintFeedbackModel)JsonConvert.DeserializeObject(actualJson, typeof(RequestCNFPComplaintFeedbackModel));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));


                //reqCnfpModel.FromDate = DateTime.ParseExact(reqCnfpModel.from,"yyyy-MM-dd", CultureInfo.InvariantCulture) ;
                //reqCnfpModel.ToDate = DateTime.ParseExact(reqCnfpModel.to, "yyyy-MM-dd", CultureInfo.InvariantCulture);



                //string from = startDate.ToString("yyyy-MM-dd");
                //string to = endDate.ToString("yyyy-MM-dd");

                return BlFixitNew.SetCnfpComplainantFeedback(reqCnfpFeedbackModel);
            }
            catch (Exception ex)
            {
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return Utility.GetStatusJsonString(Config.ResponseType.Failure.ToString(),
                //     ex.Message);
                //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "").ToString();
                return new ApiStatus(Config.ResponseType.Failure.ToString(),ex.Message);
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