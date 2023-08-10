using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;


namespace PITB.CMS_Common.Models.View
{
    public class VmComplaintPoliceDetailAgent: VmComplaintDetail
    {
        //public VmComplaintDetailAgent()
        //{
            
        //}

        public bool CanShowFollowUpView { get; set; }

        public bool CanShowFeedbackSubmitView { get; set; }

        public string FeedbackNotSumbmitViewMessage { get; set; }

        public DbComplainantFeedbackLog DbFeedbackLog { get; set; }

        public VmComplaintPoliceDetailAgent()
        {
            
        }

        public VmComplaintPoliceDetailAgent(DbComplaint dbComplaint, VmComplaintDetail vmComplaintDetail)
            : base(vmComplaintDetail)
        {
            
            //List<int?> listca = new List<int?>(){1,2,3,4,5};
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            string permissionStr =
                cookie.ListCampaignPermissions.Where(
                    n =>
                        n.Type_Id == (int) Config.Campaign.Police &&
                        n.Permission_Id == (int) Config.CampaignPermissions.CategoriesForFeedback)
                    .FirstOrDefault()
                    .Permission_Value; //Utility.ConvertCollonFormatToDict() 
            //this.CanShowFollowUpView = !((base.currentStatusStr.Trim() == Config.ClosedVerifiedStatus) || (base.currentStatusId.Equals(((int)Config.ComplaintStatus.Resolved).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedVerified).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedUnverified).ToString())));
            Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(permissionStr);
            bool hasCategoryFound = false;
            this.Is_Anonymous = dbComplaint.Is_Anonymous ?? false;
            foreach (KeyValuePair<string,string> keyVal in dict)
            {
                List<int?> listCatIds = Utility.GetNullableIntList(Utility.GetIntList(keyVal.Value));
                if (keyVal.Key == "1")
                {
                    if (listCatIds.Contains(dbComplaint.Department_Id))
                    {
                        hasCategoryFound = true;
                    }
                }
                else if (keyVal.Key == "2")
                {
                    if (listCatIds.Contains(dbComplaint.Complaint_Category))
                    {
                        hasCategoryFound = true;
                    }
                }
                else if (keyVal.Key == "3")
                {
                    if (listCatIds.Contains(dbComplaint.Complaint_SubCategory))
                    {
                        hasCategoryFound = true;
                    }
                }
            }
            if (!hasCategoryFound)
            {
                CanShowFeedbackSubmitView = false;
                FeedbackNotSumbmitViewMessage = "Complaint feedback cannot be submitted against this category.";
            }
            else if (dbComplaint.Complainant_Feedback_Status_Id == (int)Config.BinaryStatus.Yes)
            {
                CanShowFeedbackSubmitView = false;
                FeedbackNotSumbmitViewMessage = "Complaint feedback has been submitted";
                DbFeedbackLog =
                    DbComplainantFeedbackLog.GetLastComplainantFeedbackLogOfParticularComplaint(dbComplaint.Id);
            }
            else if (dbComplaint.Is_Action_Completed != null && dbComplaint.Is_Action_Completed == true)
            {
                CanShowFeedbackSubmitView = true;
            }
            else
            {
                CanShowFeedbackSubmitView = false;
                FeedbackNotSumbmitViewMessage = "Complaint action is still in progress. So feedback cannot be submitted.";
            }
            this.CanShowFollowUpView = false;
        }

        [Required]
        public string FollowupComments { get; set; }
    }
}