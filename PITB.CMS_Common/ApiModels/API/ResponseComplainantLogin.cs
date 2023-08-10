using PITB.CMS_Common.ApiModels.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PITB.CMS_Common.ApiModels.API
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