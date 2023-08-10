using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.Custom;

namespace PITB.CRM_API.Models.API.Health
{
    public class HealthResponseModel
    {
        public class SubmitComplaint : ApiStatus
        {
            //public string Status { get; set; }

            //public string Message { get; set; }

            public string complaint_Id { get; set; }


            public SubmitComplaint(string status, string message)
            {
                this.Status = status;
                this.Message = message;
            }

            public SubmitComplaint(string status, string message, string complaintId)
            {
                this.Status = status;
                this.Message = message;
                this.complaint_Id = complaintId;
            }
        }
    }
}