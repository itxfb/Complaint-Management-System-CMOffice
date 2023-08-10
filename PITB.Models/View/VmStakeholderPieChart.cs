using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.View
{
    public class VmStakeholderPieChart
    {
        public string HierarchyId { get; set; }
        public List<VmStatusWiseCount> ListStatusWiseCount { get; set; }

        public List<string> AllStatusColorList { get; set; }
    }

    public class VmStatusWiseCount
    {
        public string name { get; set; }

        public double y { get; set; }
    }

    public class VmStakeholderBarChartUserInfo
    {
        public string UserName {get; set;}
        public List<VmStatusWiseCount> ListStatusWiseCount {get; set;}
    }
}