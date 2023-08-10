using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API.SchoolEducation.PMIU
{
    public class PMIUReportsRequestModel : ApiStatus
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public int HierarchyId { get; set; }
        
        public string PmiuIds { get; set; }
    }
}