using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Campaign
{
    public class VmAddEditCampaign
    {
        public VmCampaign VmCampaign;
    }

    public class VmCampaign
    {
        public string CampaignName { get; set; }

        public string CampaignNumber { get; set; }

        public int IsMessageEnabled { get; set; }

        public string LayoutImageUrl { get; set; }

        public string UrlSuffix { get; set; }

        public int IsCustomUrlAllowed { get; set; }
    }
}