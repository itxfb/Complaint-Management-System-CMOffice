using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models.View.Executive;

namespace PITB.CMS_Common.Models.View.Executive
{
    public class VmDashboard
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string ProvinceName { get; set; }
        //public List<int> ListCampaignIdsForFeedbackView { get; set; }
        public List<VmCampaignWiseData> Campaignlst { get; set; }
    }
    
}