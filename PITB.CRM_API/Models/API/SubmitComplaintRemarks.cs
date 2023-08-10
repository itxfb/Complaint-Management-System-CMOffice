using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class SubmitComplaintRemarks
    {
        public string ComplaintId { get; set; }
        public Config.ComplainantRemarkType RemarkType { get; set; }
        public string RemarkStr { get; set; }
    }
}