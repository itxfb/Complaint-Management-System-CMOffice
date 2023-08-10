using System;
using System.Collections.Generic;

namespace PITB.CMS_Common.Models.ApiModels.Request
{
    public class RequestModel
    {
        public class PoliceDashboardCount
        {
            public string request_userName { get; set; }
            public string request_password { get; set; }
            public string request_startDate { get; set; }
            public string request_endDate { get; set; }
            public string request_complaintType { get; set; }
        }
        public class PLRADashboardCount
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class CNFP
        {
            public class PostComplaints
            {
                public string accessToken { get; set; }
                public List<Complaint> complaintData { get; set; }

                public PostComplaints(string _accessToken)
                {
                    complaintData = new List<Complaint>();
                    accessToken = _accessToken;
                }

                public class Complaint
                {
                    public string Complaint_ID { get; set; }

                    public string Person_Name { get; set; }

                    public string Person_Contact { get; set; }

                    public string Person_CNIC { get; set; }

                    public string Category { get; set; }

                    public string SubCategory { get; set; }

                    public string Complaint_Date { get; set; }

                    public string District { get; set; }

                    public Complaint(DbComplaint dbComplaint)
                    {
                        this.Complaint_ID = dbComplaint.Id.ToString();
                        this.Person_Name = dbComplaint.Person_Name;
                        this.Person_Contact = dbComplaint.Person_Contact;
                        this.Person_CNIC = dbComplaint.Person_Cnic;
                        this.Category = dbComplaint.Complaint_Category_Name;
                        this.SubCategory = dbComplaint.Complaint_SubCategory_Name;
                        this.Complaint_Date = Utility.GetDateTimeStr((DateTime)dbComplaint.Created_Date, "yyyy-MM-dd");
                        this.District = dbComplaint.District_Name;
                    }
                }
            }  
        }
    }
}