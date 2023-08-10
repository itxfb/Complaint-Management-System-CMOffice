using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PITB.CMS.Models.Custom.Reports
{
    public class PMIUSummaryReport 
    {
        public class OverDueComplaint
        {
            public int CrmId { get; set; }
            public int SrNo { get; set; }
            public string ComplaintNo { get; set; }
            public string Department { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string ResolvingOfficer { get; set; }
            public string Date { get; set; }
            public int DaysOverDue { get; set; }
            public string SchoolName { get; set; }

            //[JsonProperty("")]
            public string Hierarchy1Data { get; set; }
            public string Hierarchy2Data { get; set; }

            public int PMIUId { get; set; }
        }


        public class RegionAndStatusWiseCountTempData
        {
            public int SrNo { get; set; }
            public int Hierarchy1Id { get; set; }
            public int StatusId { get; set; }
            public string StatusName { get; set; }
            public int Total { get; set; }

        }

        public class RegionAndStatusWiseCount
        {
            public int CrmId { get; set; }
            public int SrNo { get; set; }
            public string Hierarchy1Data { get; set; }
            public int Resolved { get; set; }
            public int Closed { get; set; }
            public int Opened { get; set; }
            public int Overdue { get; set; }
            public int Total { get; set; }

            public int PMIUId { get; set; }
        }

        public class TopOverDueComplaintsByOfficer
        {
            public int CrmId { get; set; }
            public int SrNo { get; set; }
            public int UserId { get; set; }
            public string ResolvingOfficer { get; set; }

            //public int Designation { get; set; }
            public int OverdueComplaints { get; set; }
            public string Hierarchy1Data { get; set; }
            public string Hierarchy2Data { get; set; }

            public int PMIUId { get; set; }

        }
    }
}