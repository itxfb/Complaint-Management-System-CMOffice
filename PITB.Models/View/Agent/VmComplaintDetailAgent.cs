using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Models.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS_Models.View
{
    public class VmComplaintDetailAgent : VmComplaintDetail
    {
        //public VmComplaintDetailAgent()
        //{
            
        //}

        public bool CanShowFollowUpView { get; set; }

        public VmComplaintDetailAgent()
        {
            VmStatusChange = new VmStatusChange();

        }

        public VmComplaintDetailAgent(VmComplaintDetail vmComplaintDetail): base (vmComplaintDetail)
        {
            this.CanShowFollowUpView = !((base.currentStatusStr.Trim() == Config.ClosedVerifiedStatus) || (base.currentStatusId.Equals(((int)Config.ComplaintStatus.Resolved).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedVerified).ToString()) || base.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedUnverified).ToString())));

            //
            DbComplaint dbComplaint = DbComplaint.GeByComplaintIdAllColumnsIncluded(this.ComplaintId);

            this.VmStatusChange = new VmStatusChange();
            this.VmStatusChange.Compaign_Id = dbComplaint.Compaign_Id;
            this.VmStatusChange.ComplaintId = base.ComplaintId;
            this.VmStatusChange.Complaint_Category = dbComplaint.Complaint_Category;
            this.VmStatusChange.Complaint_SubCategory = dbComplaint.Complaint_SubCategory;

            List<DbStatus> listDbStatus = null;
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (this.Compaign_Id == 83/* && PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.StakeholderStatusesOnStatusChangeView)*/) // status getting from permission
            {
                //DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                //    ((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
                //    ).FirstOrDefault();
              
                //listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
                listDbStatus = DbStatus.GetByStatusIds(new List<int>(){(int)Config.ComplaintStatus.PendingReopened});
            }
            else if (/*vm.Compaign_Id == 83 &&*/ PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder)) // status getting from permission
            {
                //CMSCookie cookie = AuthenticationHandler.GetCookie();
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder
                    ).FirstOrDefault();
                //DbPermissions dbPermission =
                //DbPermissions.GetPermissionsByPermissionAndType(
                // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
                if (!string.IsNullOrEmpty(dbPermissionAssignment.Permission_Value))
                {
                    listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
                }
                else
                {
                    listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
                }
            }
            //else
            //{
            //    listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
            //}


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


            //this.VmCategoryChange.ListOfComplaintTypes = DbComplaintType.GetByCampaignId(Convert.ToInt32(dbComplaint.Compaign_Id));
            //this.VmCategoryChange.ComplaintCategoriesSelectList = this.VmCategoryChange.ListOfComplaintTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_Category.ToString(), Selected = (dbComplaint.Complaint_Category == complaintType.Complaint_Category) ? true : false }).ToList();

            //this.VmCategoryChange.ListOfComplaintSubTypes = DbComplaintSubType.GetByComplaintType(Convert.ToInt32(dbComplaint.Complaint_Category));
            //this.VmCategoryChange.ComplaintSubCategoriesSelectList = this.VmCategoryChange.ListOfComplaintSubTypes.Select(complaintSubType => new SelectListItem() { Text = complaintSubType.Name, Value = complaintSubType.Complaint_SubCategory.ToString(), Selected = (dbComplaint.Complaint_SubCategory == complaintSubType.Complaint_SubCategory) ? true : false }).ToList();

            this.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
            if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
            {
                this.followupComment = dbComplaint.FollowupComment.Trim();
            }
            this.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;

            //this.CNFP_Feedback_Status_Id = dbComplaint.CNFP_Feedback_Id;
            //this.CNFP_Feedback_Status_Value = dbComplaint.CNFP_Feedback_Value.FirstLetterUpperCase();
            //this.CNFP_Feedback_Comments = dbComplaint.CNFP_Feedback_Comments;
            //this.CNFP_Feedback_IsGiven = dbComplaint.CNFP_Is_FeedbackGiven;

            //if (dbComplaint.Latitude != null && dbComplaint.Latitude != 0)
            //{
            //    this.Latitude = dbComplaint.Latitude;
            //    this.Longitude = dbComplaint.Longitude;
            //}
            //this.Latitude = dbComplaint.Latitude;

        }

        [Required]
        public string FollowupComments { get; set; }
        
        public VmStatusChange VmStatusChange { get; set; }

        //public DbUsers dbUserComplaintAgent { get; set; }


        //public VmComplaintDetailAgent GetComplaintDetail(DbComplaint dbComplaint, List<DbDynamicComplaintFields> ListDynamicComplaintFields, VmStakeholderComplaintDetail.DetailType detailType)
        //{
        //    VmComplaintDetailAgent vm = new VmComplaintDetailAgent();
        //    vm.ComplaintId = dbComplaint.Id;
        //    vm.Compaign_Id = dbComplaint.Compaign_Id;
        //    vm.complaintIdStr = vm.Compaign_Id + "-" + vm.ComplaintId;
        //    vm.Complaint_Type = dbComplaint.Complaint_Type;
        //    vm.ComplaintCategoryName = dbComplaint.listCategory.Name;
        //    vm.Complaint_SubCategoryName = dbComplaint.listSubCategory.Name;
        //    //vm.ComplaintCategoryName = dbComplaint.listCategory.FirstOrDefault().Descr;
        //    vm.Province_Name = dbComplaint.listProvince.Province_Name;
        //    vm.District_Name = dbComplaint.listDistrict.District_Name;
        //    vm.Tehsil_Name = (dbComplaint.listTehsil != null) ? dbComplaint.listTehsil.Tehsil_Name : "None";

        //    if (dbComplaint.User == null)
        //    {
        //        vm.Created_By = "Mobile Request";
        //    }
        //    else
        //    {
        //        vm.Created_By = dbComplaint.User.Name;
        //    }
        //    vm.Created_DateTime = dbComplaint.Created_Date;
        //    if (dbComplaint.listUc != null)
        //    {
        //        vm.UnionCouncil_Name = dbComplaint.listUc.Councils_Name;

        //    }

        //    vm.Complaint_Address = dbComplaint.Complaint_Address;
        //    vm.Complaint_Remarks = dbComplaint.Complaint_Remarks;
        //    vm.Agent_Comments = dbComplaint.Agent_Comments;
        //    vm.currentStatusStr = dbComplaint.Complaint_Computed_Status;
        //    vm.currentStatusId = dbComplaint.Complaint_Computed_Status_Id.ToString();
        //    vm.currStatusCommentsStr = dbComplaint.StatusChangedComments;

        //    vm.ListDynamicComplaintFields = ListDynamicComplaintFields;

        //    vm.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
        //    if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
        //    {
        //        vm.followupComment = dbComplaint.FollowupComment.Trim();
        //    }
        //    vm.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;





        //    /*int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
        //    Config.Hierarchy hierarchyId = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
        //    int? userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
        //    if (hierarchyVal < 1)
        //    {
        //        if (dbComplaint.MinSrcId != null)
        //        {
        //            hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint,
        //                (Config.Hierarchy)dbComplaint.MinSrcId);
        //            hierarchyId = (Config.Hierarchy)dbComplaint.MinSrcId;
        //            if (dbComplaint.MinUserSrcId == null)
        //            {
        //                userHierarchyId = 1000;
        //            }
        //            else
        //            {
        //                userHierarchyId = dbComplaint.MinUserSrcId;
        //            }
        //        }
        //        else
        //        {
        //            hierarchyId = Config.Hierarchy.Province;
        //            hierarchyVal = Convert.ToInt32(dbComplaint.Province_Id);
        //        }
        //    }

        //    List<DbUsers> listDbUser = DbUsers.GetByCampIdH_IdUserH_Id2(Convert.ToInt32(dbComplaint.Compaign_Id), hierarchyId,
        //                hierarchyVal, userHierarchyId, Convert.ToInt32(dbComplaint.Complaint_Category), dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);

        //    if (dbComplaint.Dt1 == null && (listDbUser == null || listDbUser.Count == 0))
        //    {
        //        listDbUser = new List<DbUsers>();
        //        listDbUser.Add(DbUsers.GetUser(new AuthenticationHandler().CmsCookie.UserId));
        //    }


        //    if (listDbUser != null && listDbUser.Count > 0)
        //    {
        //        vm.dbUserComplaintAgent = listDbUser.First();
        //        if (vm.dbUserComplaintAgent.Designation == null)
        //        {
        //            vm.dbUserComplaintAgent.Designation = "N/A";
        //            vm.dbUserComplaintAgent.Designation_abbr = "N/A";
        //        }
        //    }
        //    else
        //    {
        //        vm.dbUserComplaintAgent = new DbUsers();
        //        vm.dbUserComplaintAgent.Name = "N/A";
        //        vm.dbUserComplaintAgent.Phone = "N/A";
        //        vm.dbUserComplaintAgent.Designation = "N/A";
        //        vm.dbUserComplaintAgent.Designation_abbr = "N/A";
        //    }
            

        //    if (dbComplaint.ComplaintSrc != null)
        //    {
        //        if (dbComplaint.ComplaintSrc == Convert.ToInt32(Config.ComplaintSource.Agent) && dbComplaint.User != null && !string.IsNullOrEmpty(dbComplaint.User.Name))
        //        {
        //            this.Created_By = dbComplaint.User.Name;
        //        }

        //        else if (dbComplaint.ComplaintSrc == Convert.ToInt32(Config.ComplaintSource.Mobile))
        //        {
        //            this.Created_By = "Mobile";
        //        }
        //    }
        //    this.Created_DateTime = dbComplaint.Created_Date;
        //    if (dbComplaint.listUc != null)
        //    {
        //        this.UnionCouncil_Name = dbComplaint.listUc.Councils_Name;

        //    }

        //    this.Complaint_Address = dbComplaint.Complaint_Address;
        //    this.Complaint_Remarks = dbComplaint.Complaint_Remarks;
        //    this.Agent_Comments = dbComplaint.Agent_Comments;
            
        //    this.Callback_Count = dbComplaint.Callback_Count;
        //    this.Callback_Status = dbComplaint.Callback_Status;
        //    this.Callback_Comment = dbComplaint.Callback_Comment;
        //    */
        //    //this.currentStatusId = (int) dbComplaint.status.Complaint_Status_Id;
        //    //this.currentStatusStr = dbComplaint.status.Status;

        //    vm.VmStatusChange.Compaign_Id = vm.Compaign_Id;
        //    vm.VmStatusChange.ComplaintId = vm.ComplaintId;
        //    vm.VmStatusChange.Complaint_Category = vm.Complaint_Category;
        //    vm.VmStatusChange.Complaint_SubCategory = vm.Complaint_SubCategory;

        //    List<DbStatus> listDbStatus = null;
        //    CMSCookie cookie = AuthenticationHandler.GetCookie();
        //    if (vm.Compaign_Id == 83 && PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.StakeholderStatusesOnStatusChangeView)) // status getting from permission
        //    {
        //        DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
        //            ((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
        //            ).FirstOrDefault();
        //        //DbPermissions dbPermission =
        //        //DbPermissions.GetPermissionsByPermissionAndType(
        //        // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
        //        listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
        //    }
        //    /*else if (vm.Compaign_Id == 83 && PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder)) // status getting from permission
        //    {
        //        //CMSCookie cookie = AuthenticationHandler.GetCookie();
        //        DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
        //            ((int)Config.PermissionsType.User, cookie.UserId, (int)Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder
        //            ).FirstOrDefault();
        //        //DbPermissions dbPermission =
        //        //DbPermissions.GetPermissionsByPermissionAndType(
        //        // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
        //        if (!string.IsNullOrEmpty(dbPermissionAssignment.Permission_Value))
        //        {
        //            listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
        //        }
        //        else
        //        {
        //            listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
        //        }
        //    }*/
        //    else
        //    {
        //        listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
        //    }


        //    vm.VmStatusChange.currentStatusId = dbComplaint.Complaint_Computed_Status_Id;//AssignmentHandler.GetActualComplaintStauts(dbComplaint);
        //    vm.currentStatusStr = dbComplaint.Complaint_Computed_Status; //listDbStatus.Where(n => n.Complaint_Status_Id == this.currentStatusId).First().Status;
        //    vm.currStatusCommentsStr = dbComplaint.StatusChangedComments;
        //    vm.VmStatusChange.ListStatus = GetStatusList(BlStatus.GetFilteredStatusList(listDbStatus, vm.VmStatusChange.currentStatusId));

        //    vm.ListDynamicComplaintFields = ListDynamicComplaintFields;
            

        //    //this.VmCategoryChange.ListOfComplaintTypes = DbComplaintType.GetByCampaignId(Convert.ToInt32(dbComplaint.Compaign_Id));
        //    //this.VmCategoryChange.ComplaintCategoriesSelectList = this.VmCategoryChange.ListOfComplaintTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_Category.ToString(), Selected = (dbComplaint.Complaint_Category == complaintType.Complaint_Category) ? true : false }).ToList();

        //    //this.VmCategoryChange.ListOfComplaintSubTypes = DbComplaintSubType.GetByComplaintType(Convert.ToInt32(dbComplaint.Complaint_Category));
        //    //this.VmCategoryChange.ComplaintSubCategoriesSelectList = this.VmCategoryChange.ListOfComplaintSubTypes.Select(complaintSubType => new SelectListItem() { Text = complaintSubType.Name, Value = complaintSubType.Complaint_SubCategory.ToString(), Selected = (dbComplaint.Complaint_SubCategory == complaintSubType.Complaint_SubCategory) ? true : false }).ToList();

        //    vm.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
        //    if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
        //    {
        //        vm.followupComment = dbComplaint.FollowupComment.Trim();
        //    }
        //    vm.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;
            
        //    //this.CNFP_Feedback_Status_Id = dbComplaint.CNFP_Feedback_Id;
        //    //this.CNFP_Feedback_Status_Value = dbComplaint.CNFP_Feedback_Value.FirstLetterUpperCase();
        //    //this.CNFP_Feedback_Comments = dbComplaint.CNFP_Feedback_Comments;
        //    //this.CNFP_Feedback_IsGiven = dbComplaint.CNFP_Is_FeedbackGiven;

        //    //if (dbComplaint.Latitude != null && dbComplaint.Latitude != 0)
        //    //{
        //    //    this.Latitude = dbComplaint.Latitude;
        //    //    this.Longitude = dbComplaint.Longitude;
        //    //}
        //    //this.Latitude = dbComplaint.Latitude;

        //    return vm;
        //}

        private static List<SelectListItem> GetStatusList(List<DbStatus> listDbStatus)
        {
            return listDbStatus.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status, Selected = n.Complaint_Status_Id == 8 ? true : false }).ToList();
            //List<SelectListItem> listStatus = new List<SelectListItem>();
            //SelectListItem item = null;
            //foreach (DbStatus dbStatus in listDbStatus)
            //{
            //    item = new SelectListItem();
            //    item.Value = dbStatus.Complaint_Status_Id.ToString();
            //    item.Text = dbStatus.Status;
            //    listStatus.Add(item);
            //}
            //return listStatus;

        }
    }
}