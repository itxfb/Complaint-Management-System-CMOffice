using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Handlers.Custom.Reports
{
    public class MainSummaryReport
    {
        public class OverDueComplaint
        {
            private string _date = null;
            public int CrmId { get; set; }
            public int SrNo { get; set; }
            public string ComplaintNo { get; set; }
            public string Department { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string ResolvingOfficer { get; set; }
            public string Date { get { return (_date == null ? _date : Convert.ToDateTime(_date).Date.ToString("yyyy/MM/dd")); } set { _date = value; } }
            public int DaysOverDue { get; set; }
            public string SchoolName { get; set; }
            public string Hierarchy1Data { get; set; }
            public string Hierarchy2Data { get; set; }
            public string EmisCode { get; set; }

        }


        public class RegionAndStatusWiseCountTempData
        {
            public int SrNo { get; set; }
            public int Hierarchy1Id { get; set; }
            public int StatusId { get; set; }
            public string StatusName { get; set; }
            public int Total { get; set; }

        }
        public class CategoryAndStatusWiseCountTempData
        {
            public string CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int StatusId { get; set; }
            public string StatusName { get; set; }
            public int Total { get; set; }

        }
        public class RegionAndStatusWiseCount
        {
            public double _percentage_closed = 0;
            public int? SrNo { get; set; }
            public string Hierarchy1Data { get; set; }
            public int Resolved { get; set; }
            public int Closed { get; set; }
            public int Opened { get; set; }
            public int Overdue { get; set; }
            public int Reopened { get; set; }
            public string PercentageClosed { get { return _percentage_closed.ToString("P0", new System.Globalization.CultureInfo("en-US")); } set { } }
            public double _closure_rate = 0d;
            public string ClosureRate { get { return _closure_rate.ToString("P0", new System.Globalization.CultureInfo("en-US")); } set { } }
            public int Total { get; set; }
        }
        public class CategoryWiseAndStatusWiseCount
        {
            public double _percentage_closed = 0;
            public int? SrNo { get; set; }

            public string CateogoryId { get; set; }
            public string CategoryName { get; set; }
            public int Resolved { get; set; }
            public int Closed { get; set; }
            public int Opened { get; set; }
            public int Overdue { get; set; }
            public int Reopened { get; set; }
            public string PercentageClosed { get { return _percentage_closed.ToString("P0", new System.Globalization.CultureInfo("en-US")); } set { } }
            public double _closure_rate = 0d;
            public string ClosureRate { get { return _closure_rate.ToString("P0", new System.Globalization.CultureInfo("en-US")); } set { } }
            public int Total { get; set; }

        }
        public class ZimmedarShehriRegionAndStatusWiseCount
        {
            public double _percentage_closed = 0;
            public int? SrNo { get; set; }
            public string Hierarchy1Data { get; set; }
            public int Resolved { get; set; }
            public int PendingFresh { get; set; }
            public int PendingReopened { get; set; }
            public int Irrelevant { get; set; }
            public int Forwarded { get; set; }
            public int Overdue { get; set; }
            public string PercentageClosed { get { return _percentage_closed.ToString("P0", new System.Globalization.CultureInfo("en-US")); } set { } }
            public int Total { get; set; }

        }
        public class TopOverDueComplaintsByOfficer
        {
            public int SrNo { get; set; }
            public int UserId { get; set; }
            public string ResolvingOfficer { get; set; }

            //public int Designation { get; set; }
            public int OverdueComplaints { get; set; }
            public string Hierarchy1Data { get; set; }
            public string Hierarchy2Data { get; set; }
            
        }

        public class ComplaintsSummary
        {
            public int SrNo { get; set; }
            public string Summary { get; set; }
            public int Count { get; set; }

        }
    }
    public struct RegionWiseFeedback
    {
        public string Hierarchy1Data { get; set; }
        public int Satisfied { get; set; }
        public int Dissatisfied { get; set; }
        public int NoAnswer { get; set; }
        public int Busy { get; set; }
        public int Congestion { get; set; }
        public int Cancel { get; set; }
        public int NotComplete { get; set; }
        public int NotApplicable { get; set; }
        public int Pending { get; set; }
    }
}