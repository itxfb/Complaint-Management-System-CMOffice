using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.FixitNew.CFNP
{
    public class RequestCNFPComplaintFeedbackModel 
    {
        public List<CNFPComplaintFeedbackModel> listFeedBack { get; set; }
    }

    public class CNFPComplaintFeedbackModel
    {
        public int complaintId { get; set; }

        public int feedbackId { get; set; }

        public string feedbackVal { get; set; }

        public string feedbackCommments { get; set; }

    }

}