using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.Datapack
{
    public class ClosureAndTimelinessAggregateModel
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
        public double? CurrentMonthAggregateClosureRate { get; set; }
        public double? CurrentMonthAggregateTimeliness { get; set; }
        public double? PreviousMonthAggregateClosureRate { get; set; }
        public double? PreviousMonthAggregateTimeliness { get; set; }
        public double? NextMonthAggregateClosureRate { get; set; }
        public double? NextMonthAggregateTimeliness { get; set; }
        public string HierarchyName { get; set; }
        public List<ClosureAndTimelinessAggregateModel> HierarchyList { get; set; }
    }
}