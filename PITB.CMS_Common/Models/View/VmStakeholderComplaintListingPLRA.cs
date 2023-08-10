using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View
{
    public class VmStakeholderComplaintListingPLRA
    {
        public string ComplaintId { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Division { get; set; }

        public string District { get; set; }

        public string Center { get; set; }
        public string Additional_Center { get; set; }

        public string Detail { get; set; }

        public string WorkCode { get; set; }

        public string Created_Date { get; set; }

        public int Total_Rows { get; set; }
 
    }
}