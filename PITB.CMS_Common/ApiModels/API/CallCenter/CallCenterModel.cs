using PITB.CMS_Common.ApiModels.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.CallCenter
{
    public class CallCenterModel
    {
        public class Request
        {
            public class SubmitLandedCallLogs
            {

                public List<Log> ListLogs { get; set; }

                public class Log
                {
                    public int CampaignId { get; set; }

                    public string PhoneNo { get; set; }

                    public DateTime? LandedDateTime { get; set; }

                    public string Session_id { get; set; }
                }
            }
        }

        public class Response
        {
            public class SubmitLandedCallLogs : ApiStatus
            {
                public int UpdatedRecords { get; set; }
            }
        }
    }
}