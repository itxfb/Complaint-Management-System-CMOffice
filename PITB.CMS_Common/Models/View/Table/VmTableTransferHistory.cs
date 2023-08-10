using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Table
{
    public class VmTableTransferHistory
    {
        public string ComplaintId { get; set; }
        public string UserName { get; set; }
        public string AssignedFromMedium { get; set; }
        public string AssignedFromMediumValue { get; set; }
        public string AssignedToMedium { get; set; }
        public string AssignedToMediumValue { get; set; }
        public string AssignedDate { get; set; }

        public string Comment { get; set; }

        public string IsValid { get; set; }
    }
}