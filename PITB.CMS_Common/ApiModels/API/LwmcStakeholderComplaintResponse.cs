using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Web;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.ApiModels.API
{
    public class LwmcStakeholderComplaintResponse : ApiStatus
    {
        public LwmcStakeholderComplaintResponse()
        {
            ListStakeholderComplaint = new List<LwmcStakeholderComplaint>();
        }
        public List<LwmcStakeholderComplaint> ListStakeholderComplaint { get; set; }

        public int Total_Rows { get; set; }




    }
    public class LwmcStakeholderComplaint : ApiStatus
    {
        public LwmcStakeholderComplaint()
        {
            Resolver = new LwmcResolver();
        }
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
        [JsonIgnore]

        public string Complaint_Status_Id { get; set; }
        [JsonIgnore]

        public string Complaint_Status { get; set; }

        public string Complaint_Computed_Status_Id { get; set; }

        public string Complaint_Computed_Status { get; set; }

        public string Complaint_Computed_Hierarchy { get; set; }

        public bool CanUpdateStatus { get; set; }
        public string Complaint_Remarks { get; set; }

        public int Complainant_Remark_Id { get; set; }

        public string Complainant_Remark_Str { get; set; }

        public int Total_Rows { get; set; }
        //[JsonIgnore]
        public string Stakeholder_Comments { get; set; }

        public string Computed_Remaining_Time_To_Escalate { get; set; }

        public int ReopenedCount { get; set; }

        public List<LogHistory> ListLogHistory { get; set; }
        public List<DbAttachments> ListAttachments { get; set; }
        public LwmcResolver Resolver { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationArea { get; set; }
        public string Facebook_PostID { get; set; }

        public string HashTag
        {
            get { return string.Format("#LWMC_{0}", Utility.EncrpytComplaintId(Complaint_Id)); }
        }

        public string Facebook_Post_Url
        {
            get
            {
                
                try
                {
                    //https://m.facebook.com/graphsearch/str/Lwmc_dppzp/keywords_top?tsid=123123&source=typeahead
                    return string.Format("https://m.facebook.com/graphsearch/str/{0}/keywords_top?tsid=123123&source=typeahead", HashTag.Replace("#", ""));
                    
//                    return string.Format("https://web.facebook.com/search/top/?q=%23{0}", HashTag.Replace("#", ""));
                }
                catch (Exception)
                {

                    return string.Empty;
                }

            }
        }

        public string Resolved_Complaint_Url
        {
            get
            {
                if (Complaint_Computed_Status_Id == Convert.ToInt32(Config.ComplaintStatus.ResolvedUnverifiedEscalatable).ToString()
                    || Complaint_Computed_Status_Id == Convert.ToInt32(Config.ComplaintStatus.ResolvedVerified).ToString())
                {
                    return string.Format("https://fixit.punjab.gov.pk/public/resolved/{0}", Utility.EncrpytComplaintId(Complaint_Id));
                }
                return string.Empty;

            }
        }



    }

    public class LogHistory
    {
        public int LogId { get; set; }
        public int ComplaintId { get; set; }
        public string StatusChangeDateTime { get; set; }
        public string StatusChangeComments { get; set; }

        public string StatusChangedByUserName { get; set; }

        public string StatusChangedByUserContact { get; set; }
        public string StatusChangedByUserHierarchy { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }

        public List<DbAttachments> ListAttachments { get; set; }
    }

    public class LwmcResolver
    {
        public string Stakeholder_Comments { get; set; }
        public string Complaint_Status_Id { get; set; }

        public string Complaint_Status { get; set; }
        public List<DbAttachments> ListAttachments { get; set; }

    }
}