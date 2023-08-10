namespace PITB.CMS_Common.Models.View
{
    public class VmAgentComplaintListing
    {
        public string Id { get; set; }

        public string Campaign_Id { get; set; }
        public string ComplaintNo{ get; set; }

        public string Campaign_Name { get; set; }
        public string Person_Name { get; set; }
        public string Created_Date { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public bool Is_Anonymous { get; set; }

        // 
        public string Mobile_No { get; set; }

        public string District_Name { get; set; }

        public string Department_Name { get; set; }

        public string Complaint_SubCategory_Name { get; set; }

        public string Complaint_Category_Name { get; set; }

        public string Complaint_Remarks { get; set; }

        public string Is_Action_Completed { get; set; }

        public string Complainant_Feedback_Total_Count { get; set; }

        public int Total_Rows { get; set; }
    }
}