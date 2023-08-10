using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Common.Models.View
{
    public class VmComplaintDetailPublicUser : VmComplaintDetail
    {

        public bool CanShowFollowUpView { get; set; }

        public VmComplaintDetailPublicUser()
        {
            VmStatusChange = new VmStatusChange();

        }

        public VmComplaintDetailPublicUser(VmComplaintDetail vmComplaintDetail) : base(vmComplaintDetail)
        {
            this.CanShowFollowUpView = !((base.currentStatusStr.Trim() == Config.ClosedVerifiedStatus) || (base.currentStatusId.Equals(((int)Config.ComplaintStatus.Resolved).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedVerified).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedUnverified).ToString())));

            //
            DbComplaint dbComplaint = DbComplaint.GeByComplaintIdAllColumnsIncluded(this.ComplaintId);

            this.VmStatusChange = new VmStatusChange();
            this.VmStatusChange.Compaign_Id = dbComplaint.Compaign_Id;
            this.VmStatusChange.ComplaintId = base.ComplaintId;
            
            this.VmStatusChange.Complaint_Category = dbComplaint.Complaint_Category;
            this.VmStatusChange.Complaint_SubCategory = dbComplaint.Complaint_SubCategory;
            this.Is_Anonymous = dbComplaint.Is_Anonymous ?? false;

            List<DbStatus> listDbStatus = null;
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (this.Compaign_Id == 83) // status getting from permission
            {
                listDbStatus = DbStatus.GetByStatusIds(new List<int>() { (int)Config.ComplaintStatus.PendingReopened });
            }
            else if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder,cookie.UserId,cookie.Role)) // status getting from permission
            {
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder
                    ).FirstOrDefault();

                if (!string.IsNullOrEmpty(dbPermissionAssignment.Permission_Value))
                {
                    listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
                }
                else
                {
                    listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
                }
            }

            this.VmStatusChange.currentStatusId = dbComplaint.Complaint_Computed_Status_Id;//AssignmentHandler.GetActualComplaintStauts(dbComplaint);
            this.currentStatusStr = dbComplaint.Complaint_Computed_Status; //listDbStatus.Where(n => n.Complaint_Status_Id == this.currentStatusId).First().Status;
            this.currStatusCommentsStr = dbComplaint.StatusChangedComments;
            if (listDbStatus != null)
            {
                this.VmStatusChange.ListStatus =
                    GetStatusList(BlStatus.GetFilteredStatusList(listDbStatus, this.VmStatusChange.currentStatusId));
            }
            else
            {
                this.VmStatusChange.ListStatus = null;
            }
            this.ListDynamicComplaintFields = ListDynamicComplaintFields;

            this.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
            if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
            {
                this.followupComment = dbComplaint.FollowupComment.Trim();
            }
            this.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;
        }

        [Required]
        public string FollowupComments { get; set; }

        public VmFileModel vmFileModel { get; set; }
        public VmStatusChange VmStatusChange { get; set; }

        private static List<SelectListItem> GetStatusList(List<DbStatus> listDbStatus)
        {
            return listDbStatus.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status, Selected = n.Complaint_Status_Id == 8 ? true : false }).ToList();
        }
    }
}