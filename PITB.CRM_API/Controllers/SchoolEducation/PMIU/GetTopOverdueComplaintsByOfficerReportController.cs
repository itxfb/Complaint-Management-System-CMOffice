using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common;
using PITB.CMS_Common.ApiModels.API.SchoolEducation.PMIU;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.Custom.Reports;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PITB.CRM_API.Controllers.SchoolEducation.PMIU
{
    public class GetTopOverdueComplaintsByOfficerReportController : ApiController
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
        public List<PMIUSummaryReport.OverDueComplaint> Post([FromBody]JToken jsonBody, [FromUri] int appId = 0)
        {

            string actualJson = jsonBody.ToString();
            //string ipAddress = Utility.GetClientIpAddress(HttpContext.Current.Request);
            Int64 apiRequestId = -1;
            try
            {
                PMIUReportsRequestModel pmiuReportModel =
                    (PMIUReportsRequestModel)JsonConvert.DeserializeObject(actualJson, typeof(PMIUReportsRequestModel));
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, true, null));
                return BlPMIUReports.GetOverDueComplaintSummary(pmiuReportModel.StartDate, pmiuReportModel.EndDate, ((int)Config.Campaign.SchoolEducationEnhanced).ToString(),
                    pmiuReportModel.HierarchyId, 20, pmiuReportModel.PmiuIds, "1,2,3,6,7,11", (int)Config.SummaryReportType.Specific);
            }
            catch (Exception ex)
            {
                return new List<PMIUSummaryReport.OverDueComplaint>();
                //apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(actualJson, ipAddress, false, ex.Message.ToString()));
                //return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
                //return Config.ResponseType.Failure.ToString() +" Exception = "+ ex.Message;
            }


            //return new ApiStatus("Success", "Remarks has been posted successfully.");
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