using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;

namespace PITB.CRM_API.Models.API
{
    public class ResponseComplainantLogin : ApiStatus
    {
        //public DbPersonInformation DbPersonInformation { get; set; }
        public SyncModel SyncModel { get; set; }

        public ResponseComplainantLogin(SyncModel syncModel, string status, string message)
            : base(status, message)
        {
            this.SyncModel = syncModel;
        }
    }
}