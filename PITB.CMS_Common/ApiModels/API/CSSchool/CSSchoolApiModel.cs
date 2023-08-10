using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.ApiModels.Custom;


namespace PITB.CMS_Common.ApiModels.API.CSSchool
{
    public class CSSchoolApiModel
    {
        public class Request
        {
            public class AddComplaint
            {
                public string Complainent_Address { get; set; }
                public string Complainent_CNIC { get; set; }
                public string Complainent_Cell { get; set; }
                public string Complainent_Email { get; set; }
                public string Complainent_Name { get; set; }
                public string Complainent_Phone { get; set; }
                public string Complaint_Date_Time { get; set; }
                public int Complaint_ID { get; set; }
                //public int Complaint_Matter_ID { get; set; }
                public string Complaint_Received_Date { get; set; }
                //public int Complaint_To_Id { get; set; }
                //public string Completion_date { get; set; }
                public string Description { get; set; }
                //public string Priority { get; set; }
                //public int Priority_Id { get; set; }
                //public string Status { get; set; }
                //public string Status_For_Completion { get; set; }

                public int CategoryId1 { get; set; }

                public string CategoryId2 { get; set; }
                public int CategoryId3 { get; set; }
                //public string Subject { get; set; }
                //public int User_ID { get; set; }

            }
        }

        public class Response
        {
            public class AddComplaint : ApiStatus
            {
                public int ComplaintId { get; set; }

                public string ComplaintIdStr { get; set; }

                public AddComplaint(int complaintId, string complaintIdStr)
                {
                    ComplaintId = complaintId;
                    ComplaintIdStr = ComplaintIdStr;
                    this.SetSuccess();
                }

            }
        }
    }
}