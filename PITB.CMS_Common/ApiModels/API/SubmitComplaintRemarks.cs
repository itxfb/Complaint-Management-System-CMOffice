using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API
{
    public class SubmitComplaintRemarks
    {
        public string ComplaintId { get; set; }
        public Config.ComplainantRemarkType RemarkType { get; set; }
        public string RemarkStr { get; set; }
    }
}