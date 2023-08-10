using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Models.Custom.CustomForm;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API.CNFP
{
    public class CNFPApiModel
    {
        public class Request
        {
            public class PostFeedback
            {
                public List<Feedback> listFeedBack { get; set; }

                public class Feedback
                {
                    public int complaintId { get; set; }

                    public int feedbackId { get; set; }

                    public string feedbackVal { get; set; }

                    public string feedbackDateTime { get; set; }

                    public string feedbackCommments { get; set; }

                }

            }
        }

        public class Response
        {
            public class PostFeedback : ApiStatus
            {

                public List<PostFeedbackResponse> listFeedBackResponse { get; set; }

                public PostFeedback()
                {
                    listFeedBackResponse = new List<PostFeedbackResponse>();
                    this.SetSuccess();
                }

                public class PostFeedbackResponse : ApiStatus
                {
                    public int complaintId { get; set; }

                    public PostFeedbackResponse(int _complaintId)
                    {
                        complaintId = _complaintId;
                        this.SetSuccess();
                    }
                }

            }
        }
    }
}