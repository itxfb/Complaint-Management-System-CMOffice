using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PITB.CMS.Models.View.Reports
{
    public class VmHierarchyWiseReport
    {
        public int HierarchyId { get; set; }
        public string HierarchyName { get; set; }
        public string CampaignIds { get; set; }
        public string CampaignName { get; set; }
        public string StatusIds { get; set; }
        public string HierarchyIds { get; set; }
        public Dictionary<string, string> StatusesName { get; set; }
        public string DataInJson { get; set; }

        public DataTable data { get; set; }
    }
    public class VmHealthDistrictWiseReport
    {
        public string SrNo { get; set; }
        public string DistrictName{get;set;}
        public string PendingFresh{get;set;}
        public string Overdue{get;set;}
        public string PendingReopened{get;set;}
        public string Resolved { get; set; }
        public string Total { get; set; }

    }
}