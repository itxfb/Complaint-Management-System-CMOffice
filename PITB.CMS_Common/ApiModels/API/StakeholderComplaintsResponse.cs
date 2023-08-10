using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.AWSSupport.Model;
using Microsoft.SqlServer.Server;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.ApiModels.API
{

    public class StakeholderComplaintsResponse : ApiStatus
    {
        public List<StakeholderComplaint> ListStakeholderComplaint { get; set; }

        public int Total_Rows { get; set; }
    }

    public class StakeholderComplaint
    {
        public int Complaint_Id { get; set; }

        public string Campaign_Name { get; set; }

        public string Person_Name { get; set; }

        public string Person_Cnic { get; set; }

        public string Person_Contact { get; set; }

        public string District_Name { get; set; }

        public string Tehsil_Name { get; set; }

        public string UnionCouncil_Name { get; set; }

        public string Created_Date { get; set; }

        public string Complaint_Category_Name { get; set; }

        public string Complaint_SubCategory_Name { get; set; }

        public string Complaint_Computed_Status_Id { get; set; }

        public string Complaint_Computed_Status { get; set; }

        public string Complaint_Computed_Hierarchy { get; set; }

        public string Complaint_Remarks { get; set; }

        public int Complainant_Remark_Id { get; set; }

        public string Complainant_Remark_Str { get; set; }

        public int Total_Rows { get; set; }

        public List<DbAttachments> ListAttachments { get; set; }
   
    }
}