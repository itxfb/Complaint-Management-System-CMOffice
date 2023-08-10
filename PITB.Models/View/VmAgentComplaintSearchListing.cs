using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.View
{
    public class VmAgentComplaintSearchListing
    {
        public string Id { get; set; }
        public string ComplaintNo { get; set; }

        public string Campaign_Name { get; set; }

        public string ComplaintType { get; set; }

        public string Person_Name { get; set; }
        public string Created_Date { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }

        public int Total_Rows { get; set; }
    }
}