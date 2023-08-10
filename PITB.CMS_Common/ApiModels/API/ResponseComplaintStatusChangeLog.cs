using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiModels.API
{
    public class ResponseComplaintStatusChangeLog : ApiStatus
    {
        public int TotalLogs { get; set; }
        public ResponseComplaintStatusChangeLog()
        {
            StatusChangeLogs = new List<StatusChangeLog>();
        }


        public string Status { get; set; }

        public string Message { get; set; }

        public List<StatusChangeLog> StatusChangeLogs { get; set; }

        public class StatusChangeLog
        {
            public int LogId { get; set; }
            public int ComplaintId { get; set; }
            public string StatusChangeDateTime { get; set; }
            public string StatusChangeComments { get; set; }
            public string StatusChangedByUserHierarchy { get; set; }
            public string Status { get; set; }
            public int StatusId { get; set; }
        }

    }
}