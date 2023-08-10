using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.ApiModels.Request
{
    public class ApiReqSyncSEModel
    {
        public class SyncSEData
        {
            public string sec_key { get; set; }

            public string system_type { get; set; }

            public string data_required { get; set; }

            public string start_date { get; set; }

            public string end_date { get; set; }
        }

        public class SyncSEUsersData
        {
            public string sec_key { get; set; }

            public string system_type { get; set; }

            //public string data_required { get; set; }

            public string user_type { get; set; }

            public string user_id { get; set; }

            public string start_date { get; set; }

            public string end_date { get; set; }
        }
    }
}