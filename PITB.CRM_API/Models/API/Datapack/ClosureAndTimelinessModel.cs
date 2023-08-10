using PITB.CRM_API.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API.Datapack
{
    public class ClosureAndTimelinessModel
    {
        public string RowLabel { get; set; }
        public string RowId { get; set; }
        public int? ClosedVerified { get; set; }
        public int? PendingFresh { get; set; }
        public int? PendingReopened { get; set; }
        public int? PendingOverdue { get; set; }
        public int? ResolvedUnverified { get; set; }
        public int? ResolvedVerified { get; set; }
        public int? GrandTotal { get; set; }
        public double? ClosureRate { get; set; }
        public double? Timeliness { get; set; }        
        public string HierarchyName { get; set; }
        public List<ClosureAndTimelinessModel> HierarchyList { get; set; }
        public List<ClosureAndTimelinessModel> TopClosureRateHierarchyList { get; set; }
        public List<ClosureAndTimelinessModel> TopTimelinessHierarchyList { get; set; }
        public List<ClosureAndTimelinessModel> WorstClosureRateHierarchyList { get; set; }
        public List<ClosureAndTimelinessModel> WorstTimelinessHierarchyList { get; set; }
    }

    public class ModelResponse: ApiStatus
    {
        public object list;
    }
    public class ClosureComparer : IComparer<ClosureAndTimelinessModel>
    {
        public string orderBy { get; set; }
        public ClosureComparer(string order)
        {
            orderBy = order;
        }
        public int Compare(ClosureAndTimelinessModel x, ClosureAndTimelinessModel y)
        {
            if (orderBy.Equals("ASC"))
            {
                if (x.ClosureRate < y.ClosureRate)
                {
                    return -1;
                }
                else if (x.ClosureRate == y.ClosureRate)
                {
                    return 0;
                }
                else if (x.ClosureRate > y.ClosureRate)
                {
                    return 1;
                }
            }
            else if (orderBy.Equals("DESC"))
            {
                if (x.ClosureRate > y.ClosureRate)
                {
                    return -1;
                }
                else if (x.ClosureRate == y.ClosureRate)
                {
                    return 0;
                }
                else if (x.ClosureRate < y.ClosureRate)
                {
                    return 1;
                }
            }           
            return 1;
        }
    }
    public class TimelinessComparer :IComparer<ClosureAndTimelinessModel>
    {
         public string orderBy { get; set; }
         public TimelinessComparer(string order)
        {
            orderBy = order;
        }
        public int Compare(ClosureAndTimelinessModel x, ClosureAndTimelinessModel y)
        {
            if (orderBy.Equals("ASC"))
            {
                if (x.Timeliness < y.Timeliness)
                {
                    return -1;
                }
                else if (x.Timeliness == y.Timeliness)
                {
                    return 0;
                }
                else if (x.Timeliness > y.Timeliness)
                {
                    return 1;
                }
            }
            else if (orderBy.Equals("DESC"))
            {
                if (x.Timeliness > y.Timeliness)
                {
                    return -1;
                }
                else if (x.Timeliness == y.Timeliness)
                {
                    return 0;
                }
                else if (x.Timeliness < y.Timeliness)
                {
                    return 1;
                }
            }
            return 1;
        }
    }
    public struct TopAndWorstPerformingAEOList
    {
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }

        public List<TopAndWorstPerformingAEO> TopClosure { get; set; }
        public List<TopAndWorstPerformingAEO> WorstClosure { get; set; }
        public List<TopAndWorstPerformingAEO> TopTimeliness { get; set; }
        public List<TopAndWorstPerformingAEO> WorstTimeliness { get; set; } 
    }
    public struct TopAndWorstPerformingAEO
    {
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string MarkazId { get; set; }
        public string MarkazName { get; set; }
        public double? ClosureRate { get; set; }
        public double? Timeliness { get; set; }
    }
    public struct SchoolsRecurringResponse
    {
        public string districtId { get; set; }
        public string districtName { get; set; }
        public List<object> data { get; set; }
    }
}