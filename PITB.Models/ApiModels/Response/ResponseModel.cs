using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.ApiModels.Response
{
    public class ResponseModel
    {
        public class PoliceDashboardCount
        {
            public string response_category { get; set; }
            public string response_count { get; set; }
            public string response_remarks { get; set; }
            public string response_resultCode { get; set; }
        }
        public class PLRADashboardCount
        {
            public string response_category { get; set; }
            public string response_count { get; set; }
            public string response_remarks { get; set; }
            public string response_resultCode { get; set; }
        }
        public class CNFP
        {
            public class PostComplaintsResponse
            {
                public string Status { get; set; }

                public string Message { get; set; }
            }


        }
    }
}