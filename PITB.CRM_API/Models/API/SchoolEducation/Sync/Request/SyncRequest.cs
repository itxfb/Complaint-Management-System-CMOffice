using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.SchoolEducation.Sync.Request
{
    public class SyncRequest
    {
        public class SyncUsersReq
        {
            public int campaignId { get; set; }
            public string userType { get; set; }
            public DateTime from { get; set; }
            public DateTime to { get; set; }
        }
    }
}