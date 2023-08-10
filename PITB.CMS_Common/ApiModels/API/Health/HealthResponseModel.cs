using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;

namespace PITB.CMS_Common.ApiModels.API.Health
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