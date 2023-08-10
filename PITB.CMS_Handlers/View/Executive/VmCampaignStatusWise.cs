using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.View.Executive
{
    public class VmCampaignStatusWise
    {
         public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignLogoSrc { get; set; }

        public List<StatusCountObject> Status { get; set; }
        
    }
    public class StatusCountObject
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int StatusCount { get; set; }
    }
}