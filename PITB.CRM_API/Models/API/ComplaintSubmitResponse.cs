using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HashidsNet;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API
{
    public class ComplaintSubmitResponse : ApiStatus
    {
        //public string Status { get; set; }

        //public string Message { get; set; }

        public string complaint_Id { get; set; }
        public string HashTag
        {
            get
            {
                if (string.IsNullOrEmpty(complaint_Id)) return string.Empty;
                return string.Format("#LWMC_{0}", Utility.EncrpytComplaintId(Convert.ToInt32(complaint_Id)));
            }
        }


        public string complaint_url
        {
            get
            {
                if (string.IsNullOrEmpty(complaint_Id)) return string.Empty;

                return string.Format("https://fixit.punjab.gov.pk/public/detail/{0}",
                   Utility.EncrpytComplaintId(Convert.ToInt32(complaint_Id)));
            }
        }

        public ComplaintSubmitResponse()
        {
        }


        public ComplaintSubmitResponse(string status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public ComplaintSubmitResponse(string status, string message, string complaintId)
        {
            this.Status = status;
            this.Message = message;
            this.complaint_Id = complaintId;
        }
    }


    public class ComplaintSubmitResponseSE : ApiStatus
    {
        //public string Status { get; set; }

        //public string Message { get; set; }

        public string complaint_Id { get; set; }


        public ComplaintSubmitResponseSE(string status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        public ComplaintSubmitResponseSE(string status, string message, string complaintId)
        {
            this.Status = status;
            this.Message = message;
            this.complaint_Id = complaintId;
        }
    }
}