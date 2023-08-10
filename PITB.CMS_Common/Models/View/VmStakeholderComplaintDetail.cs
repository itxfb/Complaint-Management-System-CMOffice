using Amazon.ElasticTranscoder.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Models.Custom;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Helper;
using System.ComponentModel;

namespace PITB.CMS_Common.Models.View
{
    public class VmStakeholderComplaintDetail
    {
        public enum DetailType
        {
            All = 1,
            AssignedToMe = 2
        }

        public VmStakeholderComplaintDetail()
        {
            VmCategoryChange = new VmCategoryChange();
            VmStatusChange = new VmStatusChange();

        }


        public VmStakeholderComplaintDetail(VmStakeholderComplaintDetail vmStakeholderComplaintDetail)
        {
            this.VmCategoryChange = vmStakeholderComplaintDetail.VmCategoryChange;
            this.VmStatusChange = vmStakeholderComplaintDetail.VmStatusChange;
            this.currDetailType = vmStakeholderComplaintDetail.currDetailType;
            this.vmPersonlInfo = vmStakeholderComplaintDetail.vmPersonlInfo;
            this.vmFileModel = vmStakeholderComplaintDetail.vmFileModel;
            this.ListEscalationModel = vmStakeholderComplaintDetail.ListEscalationModel;
            this.complaintIdStr = vmStakeholderComplaintDetail.complaintIdStr;
            this.MappedComplaintIdStr = vmStakeholderComplaintDetail.MappedComplaintIdStr;
            this.ComplaintId = vmStakeholderComplaintDetail.ComplaintId;
            this.Complaint_Type = vmStakeholderComplaintDetail.Complaint_Type;
            this.DepartmentId = vmStakeholderComplaintDetail.DepartmentId;
            this.DepartmentName = vmStakeholderComplaintDetail.DepartmentName;
            this.Complaint_Category = vmStakeholderComplaintDetail.Complaint_Category;
            this.ComplaintCategoryName = vmStakeholderComplaintDetail.ComplaintCategoryName;
            this.Complaint_SubCategory = vmStakeholderComplaintDetail.Complaint_SubCategory;
            this.Complaint_SubCategoryName = vmStakeholderComplaintDetail.Complaint_SubCategoryName;
            this.Complaint_IdealAction = vmStakeholderComplaintDetail.Complaint_IdealAction;

            this.Compaign_Id = vmStakeholderComplaintDetail.Compaign_Id;
            this.Compain_Name = vmStakeholderComplaintDetail.Compain_Name;

            this.Province_Id = vmStakeholderComplaintDetail.Province_Id;
            this.Province_Name = vmStakeholderComplaintDetail.Province_Name;

            this.Division_Id = vmStakeholderComplaintDetail.Division_Id;
            this.Division_Name = vmStakeholderComplaintDetail.Division_Name;

            this.District_Id = vmStakeholderComplaintDetail.District_Id;
            this.District_Name = vmStakeholderComplaintDetail.District_Name;

            this.Tehsil_Id = vmStakeholderComplaintDetail.Tehsil_Id;
            this.Tehsil_Name = vmStakeholderComplaintDetail.Tehsil_Name;

            this.UnionCouncil_Id = vmStakeholderComplaintDetail.UnionCouncil_Id;
            this.UnionCouncil_Name = vmStakeholderComplaintDetail.UnionCouncil_Name;

            this.LocationName = vmStakeholderComplaintDetail.LocationName;
            this.Complaint_Address = vmStakeholderComplaintDetail.Complaint_Address;

            this.Complaint_Remarks = vmStakeholderComplaintDetail.Complaint_Remarks;

            this.Agent_Comments = vmStakeholderComplaintDetail.Agent_Comments;


            this.currentStatusStr = vmStakeholderComplaintDetail.currentStatusStr;

            this.currStatusCommentsStr = vmStakeholderComplaintDetail.currStatusCommentsStr;


            this.Created_By = vmStakeholderComplaintDetail.Created_By;
            this.Created_DateTime = vmStakeholderComplaintDetail.Created_DateTime;

            this.ListDynamicComplaintFields = vmStakeholderComplaintDetail.ListDynamicComplaintFields;



            this.hasStatusHistory = vmStakeholderComplaintDetail.hasStatusHistory;

            this.hasTransferHistory = vmStakeholderComplaintDetail.hasTransferHistory;

            this.followupCount = vmStakeholderComplaintDetail.followupCount;
            this.followupComment = vmStakeholderComplaintDetail.followupComment;
            this.dbUserStakeholder = vmStakeholderComplaintDetail.dbUserStakeholder;
            this.CNFP_Feedback_Status_Id = vmStakeholderComplaintDetail.CNFP_Feedback_Status_Id;
            this.CNFP_Feedback_Status_Value = vmStakeholderComplaintDetail.CNFP_Feedback_Status_Value.FirstLetterUpperCase();
            this.CNFP_Feedback_Comments = vmStakeholderComplaintDetail.CNFP_Feedback_Comments;
            this.CNFP_Feedback_IsGiven = vmStakeholderComplaintDetail.CNFP_Feedback_IsGiven;
            this.dbComplaint = vmStakeholderComplaintDetail.dbComplaint;

            this.VmDistrictList = vmStakeholderComplaintDetail.VmDistrictList;



            //VmCategoryChange = new VmCategoryChange();
            //VmStatusChange = new VmStatusChange();
        }

        public VmFeedbackStatus VmFeedbackStatus { get; set; }
        public VmCategoryChange VmCategoryChange { get; set; }

        public bool CanChangeStatus { get; set; }

        public bool CanChangeCategory { get; set; }
        public VmFileModel vmFileModel { get; set; }
        public VmStatusChange VmStatusChange { get; set; }

        public List<SelectListItem> VmDistrictList { get; set; }
        public DetailType currDetailType { get; set; }

        public VmPersonalInfo vmPersonlInfo { get; set; }

        public DbUsers dbUserStakeholder { get; set; }


        public DbComplaint dbComplaint { get; set; }

        public List<EscalationModel> ListEscalationModel { get; set; }

        public string complaintIdStr { get; set; }
        public int ComplaintId { get; set; }
        public string MappedComplaintIdStr { get; set; }
        public Config.ComplaintType? Complaint_Type { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? Complaint_Category { get; set; }
        public string ComplaintCategoryName { get; set; }
        public int? Complaint_SubCategory { get; set; }
        public string Complaint_SubCategoryName { get; set; }
        public string LocationName { get; set; }
        public string Complaint_IdealAction { get; set; }
        public int? Compaign_Id { get; set; }
        public string Compain_Name { get; set; }

        public int? Province_Id { get; set; }
        public string Province_Name { get; set; }

        public int? Division_Id { get; set; }
        public string Division_Name { get; set; }

        public int? District_Id { get; set; }
        public string District_Name { get; set; }

        public int? Tehsil_Id { get; set; }
        public string Tehsil_Name { get; set; }

        public int? UnionCouncil_Id { get; set; }
        public string UnionCouncil_Name { get; set; }

        public string Complaint_Address { get; set; }

        public string Complaint_Remarks { get; set; }

        public string Agent_Comments { get; set; }


        public string currentStatusStr { get; set; }

        public string currStatusCommentsStr { get; set; }


        public string Created_By { get; set; }
        public DateTime? Created_DateTime { get; set; }

        public List<DbDynamicComplaintFields> ListDynamicComplaintFields { get; set; }

        public bool hasStatusHistory { get; set; }

        public bool hasTransferHistory { get; set; }

        public int followupCount { get; set; }
        public string followupComment { get; set; }

        public int? Callback_Count { get; set; }

        public string Callback_Comment { get; set; }

        public int? Callback_Status { get; set; }

        public int? CNFP_Feedback_Status_Id { get; set; }

        public string CNFP_Feedback_Comments { get; set; }

        public string CNFP_Feedback_Status_Value { get; set; }

        public bool? CNFP_Feedback_IsGiven { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public float ComplaintEscalationTimeInHrs { get; set; }

        public VmStakeholderComplaintDetail GetComplaintDetail(DbComplaint dbComplaint, int userId, List<DbDynamicComplaintFields> ListDynamicComplaintFields, VmStakeholderComplaintDetail.DetailType detailType)
        {
            //VmStakeholderComplaintDetail vm = new VmStakeholderComplaintDetail();
            this.dbComplaint = dbComplaint;

            // if complaint is unsatisfactory closed
            //if (dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
            //{
            //    dbComplaint.SrcId1 = 
            //}

            int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
            Config.Hierarchy hierarchyId = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
            int? userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
            bool isFirstLevel = false;

            //if(dbComplaint.Complaint_Status_Id!=null)
            //{
            //    DbStatus dbStatus = DbStatus.GetById((int)dbComplaint.Complaint_Status_Id);
            //    if(!Convert.ToBoolean(dbStatus.IsEscalatable))
            //    {
            //        isFirstLevel = true;
            //    }
            //}

            //if ((int)hierarchyId < 1)// for overdue complaint
            //{
            //    isFirstLevel = true;
            //}


            //if (isFirstLevel)
            //{
            //    if (dbComplaint.MinSrcId != null)
            //    {
            //        hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint,
            //            (Config.Hierarchy) dbComplaint.MinSrcId);
            //        hierarchyId = (Config.Hierarchy) dbComplaint.MinSrcId;
            //        if (dbComplaint.MinUserSrcId == null)
            //        {
            //            userHierarchyId = 1000;
            //        }
            //        else
            //        {
            //            userHierarchyId = dbComplaint.MinUserSrcId;
            //        }
            //    }
            //    else
            //    {
            //        hierarchyId = Config.Hierarchy.Province;
            //        hierarchyVal = Convert.ToInt32(dbComplaint.Province_Id);
            //    }
            //}


            List<DbUsers> listDbUser = DbUsers.GetResponsibleOfficialList(dbComplaint);
            //List<DbUsers> listDbUser = DbUsers.GetByCampIdH_IdUserH_Id2(Convert.ToInt32(dbComplaint.Compaign_Id), hierarchyId,
            //            hierarchyVal, userHierarchyId, Convert.ToInt32(dbComplaint.Complaint_Category), dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);

            if (dbComplaint.Dt1 == null && (listDbUser == null || listDbUser.Count == 0))
            {
                listDbUser = new List<DbUsers>();
                listDbUser.Add(DbUsers.GetUser(userId));
            }


            if (listDbUser != null && listDbUser.Count > 0)
            {
                this.dbUserStakeholder = listDbUser.FirstOrDefault();
                if (dbUserStakeholder != null)
                {
                    if (this.dbUserStakeholder.Designation == null)
                    {
                        this.dbUserStakeholder.Designation = "N/A";
                        this.dbUserStakeholder.Designation_abbr = "N/A";

                    }
                }
            }
            else
            {
                this.dbUserStakeholder = new DbUsers();
                this.dbUserStakeholder.Name = "N/A";
                this.dbUserStakeholder.Phone = "N/A";
                this.dbUserStakeholder.Designation = "N/A";
                this.dbUserStakeholder.Designation_abbr = "N/A";
            }
            this.currDetailType = detailType;
            this.ComplaintId = dbComplaint.Id;
            this.Compaign_Id = dbComplaint.Compaign_Id;
            this.MappedComplaintIdStr = dbComplaint.Ref_Complaint_Id.ToString();
            this.complaintIdStr = this.Compaign_Id + "-" + this.ComplaintId;
            this.DepartmentId = dbComplaint.Department_Id;
            this.DepartmentName = dbComplaint.Department_Name;
            this.Complaint_Type = dbComplaint.Complaint_Type;
            this.Complaint_Category = dbComplaint.Complaint_Category;
            this.Complaint_SubCategory = dbComplaint.Complaint_SubCategory;
            this.ComplaintCategoryName = dbComplaint.listCategory.Name;
            this.Complaint_SubCategoryName = dbComplaint.listSubCategory.Name;
            this.Complaint_IdealAction = dbComplaint.listSubCategory.Ideal_Action;
            //this.ComplaintCategoryName = dbComplaint.listCategory.FirstOrDefault().Descr;

            this.LocationName = dbComplaint.LocationArea;
            this.Province_Name = dbComplaint.listProvince.Province_Name;
            this.District_Name = dbComplaint.listDistrict.District_Name;
            this.District_Id = dbComplaint.District_Id;
            this.Tehsil_Name = (dbComplaint.listTehsil != null) ? dbComplaint.listTehsil.Tehsil_Name : "None";
            if (dbComplaint.ComplaintSrc != null)
            {
                if (dbComplaint.ComplaintSrc == Convert.ToInt32(Config.ComplaintSource.Agent) && dbComplaint.User != null && !string.IsNullOrEmpty(dbComplaint.User.Name))
                {
                    this.Created_By = dbComplaint.User.Name;
                }

                else if (dbComplaint.ComplaintSrc == Convert.ToInt32(Config.ComplaintSource.Mobile))
                {
                    this.Created_By = "Mobile";
                }
            }
            this.Created_DateTime = dbComplaint.Created_Date;
            if (dbComplaint.listUc != null)
            {
                this.UnionCouncil_Name = dbComplaint.listUc.Councils_Name;

            }

            this.Complaint_Address = dbComplaint.Complaint_Address;
            this.Complaint_Remarks = dbComplaint.Complaint_Remarks;
            this.Agent_Comments = dbComplaint.Agent_Comments;

            this.Callback_Count = dbComplaint.Callback_Count;
            this.Callback_Status = dbComplaint.Callback_Status;
            this.Callback_Comment = dbComplaint.Callback_Comment;
            //this.currentStatusId = (int) dbComplaint.status.Complaint_Status_Id;
            //this.currentStatusStr = dbComplaint.status.Status;

            this.VmStatusChange.Compaign_Id = this.Compaign_Id;
            this.VmStatusChange.ComplaintId = this.ComplaintId;
            this.VmStatusChange.Complaint_Category = this.Complaint_Category;
            this.VmStatusChange.Complaint_SubCategory = this.Complaint_SubCategory;

            List<DbStatus> listDbStatus = null;
            DbUsers dbUser = DbUsers.GetActiveUser(userId);
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (this.currDetailType == VmStakeholderComplaintDetail.DetailType.AssignedToMe && PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.StakeholderStatusesOnStatusChangeView, dbUser.User_Id, dbUser.Role_Id)) // status getting from permission
            {
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, userId, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
                    ).FirstOrDefault();
                //DbPermissions dbPermission =
                //DbPermissions.GetPermissionsByPermissionAndType(
                // (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
                listDbStatus = DbStatus.GetByStatusIds(Utility.GetIntList(dbPermissionAssignment.Permission_Value));
            }
            else if (this.currDetailType == VmStakeholderComplaintDetail.DetailType.All && PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder, dbUser.User_Id, dbUser.Role_Id)) // status getting from permission
            {
                //CMSCookie cookie = AuthenticationHandler.GetCookie();
                DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
                    ((int)Config.PermissionsType.User, userId, (int)Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder
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
            else
            {
                listDbStatus = DbStatus.GetByCampaignId((int)dbComplaint.Compaign_Id);
            }


            this.VmStatusChange.currentStatusId = dbComplaint.Complaint_Computed_Status_Id;//AssignmentHandler.GetActualComplaintStauts(dbComplaint);
            this.currentStatusStr = dbComplaint.Complaint_Computed_Status; //listDbStatus.Where(n => n.Complaint_Status_Id == this.currentStatusId).First().Status;
            this.currStatusCommentsStr = dbComplaint.StatusChangedComments;
            this.VmStatusChange.ListStatus = GetStatusList(BlStatus.GetFilteredStatusList(listDbStatus, this.VmStatusChange.currentStatusId));

            this.ListDynamicComplaintFields = ListDynamicComplaintFields;

            this.VmCategoryChange.Compaign_Id = this.Compaign_Id;
            this.VmCategoryChange.ComplaintId = this.ComplaintId;
            this.VmCategoryChange.Complaint_Category = this.Complaint_Category;
            this.VmCategoryChange.Complaint_SubCategory = this.Complaint_SubCategory;

            if (this.DepartmentId.HasValue && this.DepartmentId.Value > 0)
            {
                this.VmCategoryChange.ListOfComplaintTypes = DbComplaintType.GetByDepartmentAndGroupId(Convert.ToInt32(dbComplaint.Department_Id), null);

            }
            else
            {
                this.VmCategoryChange.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(Convert.ToInt32(dbComplaint.Compaign_Id), null);

            }

            this.VmCategoryChange.ComplaintCategoriesSelectList = this.VmCategoryChange.ListOfComplaintTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_Category.ToString(), Selected = (dbComplaint.Complaint_Category == complaintType.Complaint_Category) ? true : false }).ToList();

            this.VmCategoryChange.ListOfComplaintSubTypes = DbComplaintSubType.GetByComplaintType(Convert.ToInt32(dbComplaint.Complaint_Category));
            this.VmCategoryChange.ComplaintSubCategoriesSelectList = this.VmCategoryChange.ListOfComplaintSubTypes.Select(complaintSubType => new SelectListItem() { Text = complaintSubType.Name, Value = complaintSubType.Complaint_SubCategory.ToString(), Selected = (dbComplaint.Complaint_SubCategory == complaintSubType.Complaint_SubCategory) ? true : false }).ToList();

            this.VmCategoryChange.VmDepartmentList = DbDepartment.GetByCampaignId(Convert.ToInt32(dbComplaint.Compaign_Id)).Select(department => new SelectListItem() { Text = department.Name, Value = department.Id.ToString(), Selected = (dbComplaint.Department_Id.ToString() == department.Id.ToString()) ? true : false }).ToList();





            this.followupCount = Convert.ToInt32(dbComplaint.FollowupCount);
            if (!string.IsNullOrEmpty(dbComplaint.FollowupComment))
            {
                this.followupComment = dbComplaint.FollowupComment.Trim();
            }
            this.followupComment = (string.IsNullOrEmpty(dbComplaint.FollowupComment)) ? "None" : dbComplaint.FollowupComment;
            this.CNFP_Feedback_Status_Id = dbComplaint.CNFP_Feedback_Id;
            this.CNFP_Feedback_Status_Value = dbComplaint.CNFP_Feedback_Value.FirstLetterUpperCase();
            this.CNFP_Feedback_Comments = dbComplaint.CNFP_Feedback_Comments;
            this.CNFP_Feedback_IsGiven = dbComplaint.CNFP_Is_FeedbackGiven;

            if (dbComplaint.Latitude != null && dbComplaint.Latitude != 0)
            {
                this.Latitude = dbComplaint.Latitude;
                this.Longitude = dbComplaint.Longitude;
            }
            this.Latitude = dbComplaint.Latitude;

            return this;
        }


        public static List<DbStatus> GetListDbStatusesAgainstComplaintDetail(dynamic d)
        {

            List<DbPermissionsAssignment> listDbPermissionAssignment = d.listDbPermissionAssignment;
            List<DbStatus> listDbStatuses = d.listDbStatuses;
            VmStakeholderComplaintDetail.DetailType detailType = d.detailType;
            int campaignId = d.campaignId;
            string srcTag = d.srcTag;
            int userId = -1;

            if (d.srcTag == "web")
            {
                userId = AuthenticationHandler.GetCookie().UserId;
            }
            else
            {
                DbUsers dbUser = d.dbUser;
                userId = dbUser.User_Id;
            }



            List<DbStatus> listDbStatus = null;
            //DbUsers dbUser = DbUsers.GetActiveUser(userId);
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (detailType == VmStakeholderComplaintDetail.DetailType.AssignedToMe) // status getting from permission
            {

                DbPermissionsAssignment dbPermissionAssignment = listDbPermissionAssignment.Where(n => n.Type == (int?)Config.PermissionsType.User
                                                                && n.Type_Id == (int?)userId && n.Permission_Id == (int)Config.Permissions.StakeholderStatusesOnStatusChangeView).FirstOrDefault();
                if (dbPermissionAssignment != null)
                {
                    List<int> listInt = Utility.GetIntList(dbPermissionAssignment.Permission_Value);
                    listDbStatus = listDbStatuses.Where(n => listInt.Contains(n.Complaint_Status_Id)).ToList();
                }
            }
            else if (detailType == VmStakeholderComplaintDetail.DetailType.All) // status getting from permission
            {
                DbPermissionsAssignment dbPermissionAssignment = listDbPermissionAssignment.Where(n => n.Type == (int?)Config.PermissionsType.User
                                                                && n.Type_Id == (int?)userId && n.Permission_Id == (int)Config.Permissions.ShowStatusChangeInComplaintsAllStakeholder).FirstOrDefault();
                if (dbPermissionAssignment != null)
                {
                    List<int> listInt = Utility.GetIntList(dbPermissionAssignment.Permission_Value);
                    listDbStatus = listDbStatuses.Where(n => listInt.Contains(n.Complaint_Status_Id)).ToList();
                }
            }
            else
            {
                //listDbStatus = DbStatus.GetByCampaignId(campaignId);
                listDbStatus = listDbStatus.Where(
                        n => Utility.GetIntList(n.Campaigns).ToList().Any(p => p == campaignId)).ToList();
            }
            return listDbStatus;
        }


        private static List<SelectListItem> GetStatusList(List<DbStatus> listDbStatus)
        {
            return listDbStatus.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status, Selected = n.Complaint_Status_Id == 8 ? true : false }).ToList();
            //List<SelectListItem> listStatus = new List<SelectListItem>();
            //SelectListItem item = null;
            //foreach (DbStatus dbStatus in listDbStatus)
            //{
            //    item = new SelectListItem();
            //    item.Value = dbStatus.Complaint_Status_Id.ToString(u);
            //    item.Text = dbStatus.Status;
            //    listStatus.Add(item);
            //}
            //return listStatus;

        }

        //private static List<SelectListItem> GetStatusList(List<DbStatus> listDbStatus)
        //{
        //    SelectListItem item = null;
        //    List<SelectListItem> listStatus = new List<SelectListItem>();
        //    foreach (DbStatus dbStatus in listDbStatus)
        //    {
        //        if (statusToRemove != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.PendingFresh) != dbStatus.Complaint_Status_Id && Convert.ToInt32(Config.ComplaintStatus.UnsatisfactoryClosed) != dbStatus.Complaint_Status_Id)
        //        {
        //            item = new SelectListItem();
        //            item.Value = dbStatus.Complaint_Status_Id.ToString();
        //            item.Text = dbStatus.Status;
        //            listStatus.Add(item);
        //        }
        //    }
        //    return listStatus;
        //}


    }

    public class VmStatusChange
    {
        public int? Compaign_Id { get; set; }

        public int ComplaintId { get; set; }

        public int? Complaint_Category { get; set; }

        public int? Complaint_SubCategory { get; set; }

        //public VmFileModel vmFileModel { get; set; }

        public int currentStatusId { get; set; }



        [Required]
        [Display(Name = "Status")]
        public int? statusID { get; set; }

        public List<SelectListItem> ListStatus { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(1000, ErrorMessage = "Comment should be maximum 1000 characters in length")]
        public string statusChangeComments { get; set; }

        public string returnUrl { get; set; }
        public VmFeedbackStatus VmFeedbackStatus { get; set; }
        public VmStatusChange()
        {
            VmFeedbackStatus = new VmFeedbackStatus();
        }

        public static VmStatusChange GetModel(CustomForm.Post postedForm)
        {
            VmStatusChange vmStatusChange = new VmStatusChange();
            vmStatusChange.ComplaintId = postedForm.GetElementValue(string.Format("VmStatusChange.ComplaintId")).CastObj<int>();
            vmStatusChange.Compaign_Id = postedForm.GetElementValue(string.Format("VmStatusChange.Compaign_Id")).CastObj<int>();
            vmStatusChange.Complaint_Category = postedForm.GetElementValue(string.Format("VmStatusChange.Complaint_Category")).CastObj<int>();
            vmStatusChange.Complaint_SubCategory = postedForm.GetElementValue(string.Format("VmStatusChange.Complaint_SubCategory")).CastObj<int>();
            vmStatusChange.statusID = postedForm.GetElementValue(string.Format("VmStatusChange.statusID")).CastObj<int>();
            vmStatusChange.statusChangeComments = postedForm.GetElementValue(string.Format("VmStatusChange.statusChangeComments")).CastObj<string>();
            vmStatusChange.VmFeedbackStatus.feedbackStatusID = postedForm.GetElementValue(string.Format("VmStatusChange.VmFeedbackStatus.feedbackStatusID")).CastObj<int>();
            vmStatusChange.VmFeedbackStatus.feedbackStatusChangeComments = postedForm.GetElementValue(string.Format("VmStatusChange.VmFeedbackStatus.feedbackStatusChangeComments")).CastObj<string>();
            return vmStatusChange;
        }
    }

    public class VmCategoryChange
    {

        public int? Compaign_Id { get; set; }

        public int? ComplaintId { get; set; }

        public int? Complaint_Category { get; set; }

        public int? Complaint_SubCategory { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        public List<DbComplaintType> ListOfComplaintTypes { get; set; }

        [Required]
        [Display(Name = "Complaint category")]
        public int? selectedComplaintCategory { get; set; }

        public List<SelectListItem> ComplaintCategoriesSelectList
        {
            get;
            set;
            /*get
            {
                return ListOfComplaintTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_Category.ToString() }).ToList();
            }*/
        }

        public List<SelectListItem> VmDepartmentList { get; set; }
        public List<DbComplaintSubType> ListOfComplaintSubTypes { get; set; }

        [Required]
        [Display(Name = "Complaint subcategory")]
        public int? selectedComplaintSubCategory { get; set; }

        public List<SelectListItem> ComplaintSubCategoriesSelectList
        {
            get;
            set; /*get
            {
                return ListOfComplaintSubTypes.Select(complaintType => new SelectListItem() { Text = complaintType.Name, Value = complaintType.Complaint_SubCategory.ToString() }).ToList();
            }*/
        }

    }
}