using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Complaint.Assignment;
using PITB.CMS.Handler.FileUpload;
using PITB.CMS.Handler.Messages;
using PITB.CMS.Handler.Permission;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Web;
using PITB.CMS.Models.View.Table;
using PITB.CMS.Handler.Business;

namespace PITB.CMS.Handler.Complaint.Status
{
    public class StatusHandler
    {
        public static FileUploadHandler.FileValidationStatus ChangeStatus(VmStatusChange vmStatusChange, HttpFileCollectionBase files)
        {
            FileUploadHandler.FileValidationStatus validationStatus = FileUploadHandler.GetFileValidationStatus(files);

            

            if (validationStatus.ValidationStatus==Config.AttachmentErrorType.NoError)
            {
                int campaignId = (int)vmStatusChange.Compaign_Id;
                int complaintId = vmStatusChange.ComplaintId;
                int categoryId = (int)vmStatusChange.Complaint_Category;
                int subcategoryId = (int)vmStatusChange.Complaint_SubCategory;
                int statusId = (int)vmStatusChange.statusID;
                DateTime currentDateTime = DateTime.Now;

                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;
                //Config.CategoryType cateogryType = Config.CategoryType.Main;

                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId(subcategoryId);

                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)vmStatusChange.Complaint_Category);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                //float catRetainingHours =
                //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                CMSCookie cookie = AuthenticationHandler.GetCookie();
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbComplaint dbComplaint = DbComplaint.GetByComplaintId(db, complaintId);

                if (dbComplaint.Compaign_Id == (int)Config.Campaign.DcoOffice)
                {
                    /*List<string> listPhoneNo = new List<string>()
                    {
                        "03214226005",
                        "03458554214",
                        "03004345032",
                        "03454333358",
                        "03360008755"
                    };*/
                    if (dbComplaint.Person_Contact!=null /*&& listPhoneNo.Contains(dbComplaint.Person_Contact.Trim())*/)
                    {
                        if (statusId == (int) Config.ComplaintStatus.Resolved)
                        {
                            BlComplaints.PushComplaintToCNFP(dbComplaint, db);
                        }
                    }
                }

                List<DbPermissionsAssignment> listDbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campaignId,
                (int)Config.CampaignPermissions.CanResetEscalation);

                bool canResetOnPendingReopen = (listDbPermissionAssignment.Count == 0) ? false : true;

                List<AssignmentModel> assignmentModelList =
                    AssignmentHandler.GetAssignmentModelOnStatusChange2(Convert.ToInt32(cookie.Hierarchy_Id), Convert.ToInt32(cookie.User_Hierarchy_Id), dbComplaint,
                        statusId, DateTime.Now, DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId), catRetainingHours, canResetOnPendingReopen);

                for (int i = 0; i < 10; i++)
                {
                    if (i < assignmentModelList.Count)
                    {
                        paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                        //paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                    }
                    else
                    {
                        paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                        paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                        //paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                    }
                }

                paramDict.Add("@ComplaintId", complaintId.ToDbObj());
                paramDict.Add("@StatusId", statusId.ToDbObj());

                paramDict.Add("@Status_ChangedBy", cookie.UserId);
                paramDict.Add("@Status_ChangedBy_Name", cookie.UserName);
                paramDict.Add("@StatusChangedDate_Time", currentDateTime);
                paramDict.Add("@StatusChangedBy_RoleId", Convert.ToInt32(cookie.Role));
                paramDict.Add("@StatusChangedBy_HierarchyId", Convert.ToInt32(cookie.Hierarchy_Id));
                paramDict.Add("@StatusChangedBy_User_HierarchyId", Convert.ToInt32(cookie.User_Hierarchy_Id));
                paramDict.Add("@StatusChanged_Comments", vmStatusChange.statusChangeComments.ToDbObj());

                // ------ adding custom params -----------------

                paramDict.Add("@MaxLevel", assignmentModelList.Count);

                paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
                paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

                paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
                paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


                paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
                paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));

                OriginHierarchy originHierarchy = OriginHierarchy.GetOrigin(assignmentModelList);
                paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);

                // ----------- end adding custom params --------

                DBHelper.GetDataTableByStoredProcedure("[PITB].[Update_Complaints_Status]", paramDict)
                    .ToList<VmAgentComplaintListing>();


                //db = new DBContextHelperLinq();

                MakeLastLogOfComplaintStatusInactive(complaintId, db);
                DbComplaintStatusChangeLog dbStatusChangeLog = SaveComplaintStatusInLog(complaintId, statusId,
                    currentDateTime, vmStatusChange.statusChangeComments, db);

                BlNotification.ResetNotification(campaignId, Config.NotificationType.Complaint, Config.NotificationSubType.Launch, complaintId, Config.NotificationStatus.Send);

                db.SaveChanges();

                int statusLogId = dbStatusChangeLog.Id;
                FileUploadHandler.UploadMultipleFiles(files, Config.AttachmentReferenceType.ChangeStatus, Utility.GetComplaintIdStr(campaignId, complaintId),
                    statusLogId, Config.TAG_COMPLAINT_STATUS_CHANGE);

                validationStatus.ValidationMessage = "Complaint " + vmStatusChange.Compaign_Id + "-" + vmStatusChange.ComplaintId +
                       " status changed successfully!! ";


                // Send message on status change
                if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
                {
                    if (statusId == 8 || statusId == 25)
                    {
                        TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                            (int) dbComplaint.Compaign_Id, dbComplaint.Id, (int) dbComplaint.Complaint_Category,
                            statusId,
                            vmStatusChange.statusChangeComments);
                    }
                }
                return validationStatus;
            }
            else
            {
                return validationStatus;
            }

        }


        // Preparing data for status 
        public static Dictionary<string, object> PrepareDataForStatusChange(Dictionary<string, object> dictParam/*VmStatusChange vmStatusChange,int statusId, string statusComments, DbComplaint dbComplaint, DbUsers dbUser, HttpFileCollectionBase files*/)
        {
            HttpFileCollectionBase file = (HttpFileCollectionBase) dictParam["files"];
            FileUploadHandler.FileValidationStatus validationStatus = FileUploadHandler.GetFileValidationStatus(file);
            DbComplaint dbComplaint = (DbComplaint) dictParam["dbComplaint"];

            Dictionary<string,object> dictData = new Dictionary<string, object>();
            //-------- Start -------

            dictData.Add("files", file);
            dictData.Add("validationStatus", validationStatus);
            
            //HttpFileCollectionBase files = (HttpFileCollectionBase)dictData["files"];
            //List<AssignmentModel> assignmentModelList = (List<AssignmentModel>)dictParams["assignmentModelList"];
            //int complaintId = (int)dictParams["complaintId"];
            //int campaignId = (int)dictParams["campaignId"];
            //int categoryId = (int)dictParams["categoryId"];
            //int statusId = (int)dictParams["statusId"];
            //string personContact = (string)dictParams["personContact"];

            //int userId = (int)dictParams["userId"];
            //int userName = (int)dictParams["userName"];
            //DateTime currentDateTime = (DateTime)dictParams["currentDateTime"];
            //int roleId = (int)dictParams["roleId"];
            //int hierarchyId = (int)dictParams["hierarchyId"];
            //int userHierarchyId = (int)dictParams["userHierarchyId"];

            //string statusChangeComments = (string)dictParams["statusChangeComments"];

            //-------- End --------

            if (validationStatus.ValidationStatus == Config.AttachmentErrorType.NoError)
            {
                int campaignId = (int) dbComplaint.Compaign_Id; //(int)dictParam["campaignId"];
                int complaintId = dbComplaint.Id; //(int) dictParam["complaintId"];
                int categoryId = (int) dbComplaint.Complaint_Category;// (int) dictParam["categoryId"];
                int subcategoryId = (int) dbComplaint.Complaint_SubCategory; //(int) dictParam["subcategoryId"];
                int statusId = (int) dictParam["statusId"];
                DateTime currentDateTime = DateTime.Now;




                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;
                //Config.CategoryType cateogryType = Config.CategoryType.Main;

                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId(subcategoryId);

                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours =
                        DbComplaintType.GetRetainingHoursByTypeId((int)categoryId);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float) subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                //float catRetainingHours =
                //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                CMSCookie cookie = AuthenticationHandler.GetCookie();
                DBContextHelperLinq db = new DBContextHelperLinq();
                //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(db, complaintId);

                if (dbComplaint.Compaign_Id == (int) Config.Campaign.DcoOffice)
                {
                    /*List<string> listPhoneNo = new List<string>()
                    {
                        "03214226005",
                        "03458554214",
                        "03004345032",
                        "03454333358",
                        "03360008755"
                    };*/
                    if (dbComplaint.Person_Contact != null
                        /*&& listPhoneNo.Contains(dbComplaint.Person_Contact.Trim())*/)
                    {
                        if (statusId == (int) Config.ComplaintStatus.Resolved)
                        {
                            BlComplaints.PushComplaintToCNFP(dbComplaint, db);
                        }
                    }
                }

                List<DbPermissionsAssignment> listDbPermissionAssignment =
                    DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int) Config.PermissionsType.Campaign, campaignId,
                        (int) Config.CampaignPermissions.CanResetEscalation);

                bool canResetOnPendingReopen = (listDbPermissionAssignment.Count == 0) ? false : true;

                List<AssignmentModel> assignmentModelList =
                    AssignmentHandler.GetAssignmentModelOnStatusChange2(Convert.ToInt32(cookie.Hierarchy_Id),
                        Convert.ToInt32(cookie.User_Hierarchy_Id), dbComplaint,
                        statusId, DateTime.Now,
                        DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId),
                        catRetainingHours, canResetOnPendingReopen);


                DbUsers dbUsers = (DbUsers) dictParam["dbUser"];
                
                //------ pushing data in dictionary
                dictData.Add("assignmentModelList", assignmentModelList);
                dictData.Add("complaintId", complaintId);
                dictData.Add("campaignId", campaignId);
                dictData.Add("categoryId", categoryId);
                dictData.Add("statusId", statusId);
                //dictData.Add("personContact", personContact);
                dictData.Add("userId", dbUsers.User_Id);
                dictData.Add("userName", dbUsers.Username);
                dictData.Add("currentDateTime", currentDateTime);
                dictData.Add("roleId", Convert.ToInt32(dbUsers.Role_Id));
                dictData.Add("hierarchyId", Convert.ToInt32( dbUsers.Hierarchy_Id));
                dictData.Add("userHierarchyId", Convert.ToInt32(dbUsers.User_Hierarchy_Id));
                dictData.Add("statusChangeComments", dictParam["statusComments"]);
                dictData.Add("personContact", dbComplaint.Person_Contact);
                
      
                // ------ end pushing data in dictionary
                return dictData;
            }
            else
            {
                return dictData;
            }

        }


        public static void UpdateStatusInDb(Dictionary<string,object> dictParams )
        {
            List<AssignmentModel> assignmentModelList = (List<AssignmentModel>) dictParams["assignmentModelList"];
            HttpFileCollectionBase files = (HttpFileCollectionBase)dictParams["files"];

            int complaintId = (int)dictParams["complaintId"];
            int campaignId = (int)dictParams["campaignId"];
            int categoryId = (int)dictParams["categoryId"];
            int statusId = (int)dictParams["statusId"];
            string personContact = (string)dictParams["personContact"];

            int userId = (int)dictParams["userId"];
            string userName = (string)dictParams["userName"];
            DateTime currentDateTime = (DateTime)dictParams["currentDateTime"];
            int roleId = (int)dictParams["roleId"];
            int hierarchyId = (int)dictParams["hierarchyId"];
            int userHierarchyId = (int)dictParams["userHierarchyId"];

            string statusChangeComments = (string)dictParams["statusChangeComments"];

            FileUploadHandler.FileValidationStatus validationStatus = (FileUploadHandler.FileValidationStatus)dictParams["validationStatus"];

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            for (int i = 0; i < 10; i++)
            {
                if (i < assignmentModelList.Count)
                {
                    paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                    //paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                }
                else
                {
                    paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                    //paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                }
            }

            paramDict.Add("@ComplaintId", complaintId.ToDbObj());
            paramDict.Add("@StatusId", statusId.ToDbObj());

            paramDict.Add("@Status_ChangedBy", userId);
            paramDict.Add("@Status_ChangedBy_Name", userName);
            paramDict.Add("@StatusChangedDate_Time", currentDateTime);
            paramDict.Add("@StatusChangedBy_RoleId", Convert.ToInt32(roleId));
            paramDict.Add("@StatusChangedBy_HierarchyId", Convert.ToInt32(hierarchyId));
            paramDict.Add("@StatusChangedBy_User_HierarchyId", Convert.ToInt32(userHierarchyId));
            paramDict.Add("@StatusChanged_Comments", statusChangeComments.ToDbObj());

            // ------ adding supporting params for escalation params -----------------

            paramDict.Add("@MaxLevel", assignmentModelList.Count);

            paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
            paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

            paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
            paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


            paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
            paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));

            OriginHierarchy originHierarchy = OriginHierarchy.GetOrigin(assignmentModelList);
            paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
            paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
            paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
            paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
            paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);

            // ----------- end adding custom params --------

            DBHelper.GetDataTableByStoredProcedure("[PITB].[Update_Complaints_Status]", paramDict)
                .ToList<VmAgentComplaintListing>();


            //db = new DBContextHelperLinq();
            DBContextHelperLinq db = new DBContextHelperLinq();
            MakeLastLogOfComplaintStatusInactive(complaintId, db);
            DbComplaintStatusChangeLog dbStatusChangeLog = SaveComplaintStatusInLog(complaintId, statusId,
                currentDateTime, statusChangeComments, db);

            BlNotification.ResetNotification(campaignId, Config.NotificationType.Complaint, Config.NotificationSubType.Launch, complaintId, Config.NotificationStatus.Send);

            db.SaveChanges();

            int statusLogId = dbStatusChangeLog.Id;
            FileUploadHandler.UploadMultipleFiles(files, Config.AttachmentReferenceType.ChangeStatus, Utility.GetComplaintIdStr(campaignId, complaintId),
                statusLogId, Config.TAG_COMPLAINT_STATUS_CHANGE);

            validationStatus.ValidationMessage = "Complaint " + campaignId + "-" + complaintId +
                   " status changed successfully!! ";


            // Send message on status change
            if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
            {
                if (statusId == 8)
                {
                    TextMessageHandler.SendMessageOnStatusChange(personContact,
                        (int)campaignId, complaintId, (int)categoryId,
                        statusId,
                        statusChangeComments);
                }
            }
        }

        public static bool ChangeStatus(int userId, int complaintId, int statusId, string statusComments/*, List<Picture> listPictures, Int64 apiRequestId*/)
        {
            CMS.Models.DB.DbComplaint dbComplaint = CMS.Models.DB.DbComplaint.GetByComplaintId(complaintId);
            int campaignId = (int)dbComplaint.Compaign_Id;
            int categoryId = (int)dbComplaint.Complaint_Category;
            int subcategoryId = (int)dbComplaint.Complaint_SubCategory;
            DateTime currentDateTime = DateTime.Now;

            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;

            subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId(subcategoryId);

            if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
            {
                catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)categoryId);
                //cateogryType = Config.CategoryType.Main;
            }
            else
            {
                catRetainingHours = (float)subcatRetainingHours;
                //cateogryType = Config.CategoryType.Sub;
            }

            //DbUsers dbUser = string.IsNullOrEmpty(username) ? DbUsers.GetByUserName("SchoolEducationTabletMEA") : DbUsers.GetByUserName(username);
            DbUsers dbUser = DbUsers.GetActiveUser(userId);

            //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId); 
            //float catRetainingHours =
            //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            //DbComplaint dbComplaint = DbComplaint.GetListByComplaintId(complaintId).First();

            int userHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
            int userInnerHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);

            //CMS.Models.DB.DbSchoolsMapping dbSchools = CMS.Models.DB.DbSchoolsMapping.GetById((int)dbComplaint.TableRowRefId);

     

            //---------- New Custom Code ------------

            int? userCategoryId1 = null;
            int? userCategoryId2 = null;
            int? userCategoryId3 = null;

            List<CMS.Models.Custom.AssignmentModel> assignmentModelList = null;
            //if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint)
            {

                assignmentModelList =
                    CMS.Handler.Complaint.Assignment.AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory((int)dbComplaint.Compaign_Id,
                        (int)dbComplaint.Complaint_Category, (int)dbComplaint.Complaint_SubCategory, true, null, null);


                

                //------ Custom Code -------

                //List<CMS.Models.DB.DbUsers> listDbUsers = CMS.Handler.StakeHolder.UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(dbComplaint.Compaign_Id));
                //CMS.Config.Hierarchy hierarchyId = (CMS.Config.Hierarchy)assignmentModelList[0].SrcId;
                //int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);
                //VmComplaint vmComplaint = new VmComplaint();
                //vmComplaint.Province_Id = dbComplaint.Province_Id;
                //vmComplaint.Division_Id = dbComplaint.Division_Id;
                //vmComplaint.District_Id = dbComplaint.District_Id;
                //vmComplaint.Tehsil_Id = dbComplaint.Tehsil_Id;
                //vmComplaint.UnionCouncil_Id = dbComplaint.UnionCouncil_Id;

                //List<int?> listStatusIds = new List<int?> { (int)Config.ComplaintStatus.PendingReopened };

                //if (listStatusIds.Where(n => n == statusId).FirstOrDefault() != null) // if status exist in 
                //{

                //    OriginHierarchy originHierarchy =
                //        CMS.Handler.Business.BlSchool.EvaluateAssignmentMartix(vmComplaint, listDbUsers,
                //            assignmentModelList, dbSchools, hierarchyId, userHierarchyVal, ref userCategoryId1,
                //            ref userCategoryId2, ref userCategoryId3, 0, null);
                //    paramDict.Add("@UserCategoryId1", userCategoryId1);
                //    paramDict.Add("@UserCategoryId2", userCategoryId2);

                //    paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                //    paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                //    paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                //    paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                //    paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);
                //}
                //else
                {
                    paramDict.Add("@UserCategoryId1", dbComplaint.UserCategoryId1);
                    paramDict.Add("@UserCategoryId2", dbComplaint.UserCategoryId2);

                    //paramDict.Add("@Origin_HierarchyId", dbComplaint.Origin_HierarchyId);
                    //paramDict.Add("@Origin_UserHierarchyId", dbComplaint.Origin_UserHierarchyId);
                    //paramDict.Add("@Origin_UserCategoryId1", dbComplaint.Origin_UserCategoryId1);
                    //paramDict.Add("@Origin_UserCategoryId2", dbComplaint.Origin_UserCategoryId2);
                    //paramDict.Add("@Is_AssignedToOrigin", dbComplaint.Is_AssignedToOrigin);
                }
                //--------------------------
            }


            //--------- End new Custom Code ----------

            //List<DbPermissionsAssignment> listDbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions((int)Config.Campaign.FixItLwmc,
            //(int)Config.CampaignPermissions.CanResetEscalation);

            //bool canResetOnPendingReopen = (listDbPermissionAssignment == null) ? false : true;
            //List<AssignmentModel> 

            List<DbAssignmentMatrix> listDbAssignmentMatrix = DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId);

            assignmentModelList =
            CMS.Handler.Complaint.Assignment.AssignmentHandler.GetAssignmentModelOnStatusChange2(userHierarchyId, userInnerHierarchyId, dbComplaint,
                statusId, DateTime.Now, listDbAssignmentMatrix /*assignmentModelList*/, catRetainingHours, true);

            for (int i = 0; i < 10; i++)
            {
                if (i < assignmentModelList.Count)
                {
                    paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                    //paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                }
                else
                {
                    paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                }
            }

            paramDict.Add("@ComplaintId", complaintId.ToDbObj());
            paramDict.Add("@StatusId", statusId.ToDbObj());

            paramDict.Add("@Status_ChangedBy", dbUser.User_Id);
            paramDict.Add("@Status_ChangedBy_Name", dbUser.Username);
            paramDict.Add("@StatusChangedDate_Time", currentDateTime);
            paramDict.Add("@StatusChangedBy_RoleId", Convert.ToInt32(dbUser.Role_Id));
            paramDict.Add("@StatusChangedBy_HierarchyId", Convert.ToInt32(dbUser.Hierarchy_Id));
            paramDict.Add("@StatusChangedBy_User_HierarchyId", Convert.ToInt32(dbUser.User_Hierarchy_Id));
            paramDict.Add("@StatusChanged_Comments", statusComments.ToDbObj());

            // ------ adding supporting params for escalation params -----------------

            paramDict.Add("@MaxLevel", assignmentModelList.Count);

            paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList));
            paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList));

            paramDict.Add("@MinUserSrcId", AssignmentHandler.GetMinUserSrcId(assignmentModelList));
            paramDict.Add("@MaxUserSrcId", AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


            paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList));
            paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList));

            OriginHierarchy originHierarchy = OriginHierarchy.GetOrigin(assignmentModelList);
            paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
            paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
            paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
            paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
            paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);

            // ----------- end adding custom params --------

            DBHelper.GetDataTableByStoredProcedure("[PITB].[Update_Complaints_Status]", paramDict);
            

            DBContextHelperLinq db = new DBContextHelperLinq();

            MakeLastLogOfComplaintStatusInactive(complaintId, db);
            DbComplaintStatusChangeLog dbStatusChangeLog = SaveComplaintStatusInLog(dbUser.User_Id, complaintId, statusId,
                currentDateTime, statusComments, db);
            db.SaveChanges();

            int statusLogId = dbStatusChangeLog.Id;
            //----- Save Image in db
            //if (listPictures != null)
            //{
            //    foreach (Picture picture in listPictures)
            //    {
            //        FileUploadHandler.StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", campaignId, complaintId, statusLogId, Config.AttachmentReferenceType.ChangeStatus, apiRequestId);
            //    }
            //}

            //new Thread(delegate()
            //{
            //    string url = PITB.CMS.Config.ApiUrlSchoolComplaintSystem + "/api/public_school_complaint_status";
            //    CMS.Handler.Complaint.SchoolEducatonStatusHandler.PushStatusToSchoolComplaintSystem(url, complaintId, statusId, PITB.CMS.Utility.GetAlteredStatus(Convert.ToInt32(campaignId), PITB.CMS.Models.DB.DbStatus.GetById(statusId).Status));
            //}).Start();


            //return new ApiStatus(Config.ResponseType.Success.ToString(), "Your Complaint status has been changed Successfully");
            return true;
        }


        public static void MakeLastLogOfComplaintStatusInactive(int complaintId, DBContextHelperLinq db)
        {
            //DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaintStatusChangeLog statusChangeLog = DbComplaintStatusChangeLog.GetLastStatusChangeOfParticularComplaint(complaintId, db);
            if (statusChangeLog != null)
            {
                statusChangeLog.IsCurrentlyActive = false;
                db.DbComplaintStatusChangeLog.Add(statusChangeLog);
                db.Entry(statusChangeLog).State = EntityState.Modified;
                //db.SaveChanges();
            }
        }

        public static DbComplaintStatusChangeLog SaveComplaintStatusInLog(int complaintId, int statusId, DateTime statusSaveDateTime, string comments, DBContextHelperLinq db)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbComplaintStatusChangeLog dbStatusChangeLog = new DbComplaintStatusChangeLog();
            dbStatusChangeLog.StatusChangedByUserId = cookie.UserId;
            dbStatusChangeLog.Complaint_Id = complaintId;
            dbStatusChangeLog.StatusId = statusId;
            dbStatusChangeLog.StatusChangeDateTime = statusSaveDateTime;
            dbStatusChangeLog.Comments = comments;
            dbStatusChangeLog.IsCurrentlyActive = true;
            db.DbComplaintStatusChangeLog.Add(dbStatusChangeLog);
            return dbStatusChangeLog;
        }

        public static DbComplaintStatusChangeLog SaveComplaintStatusInLog(int userId, int complaintId, int statusId, DateTime statusSaveDateTime, string comments, DBContextHelperLinq db)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbComplaintStatusChangeLog dbStatusChangeLog = new DbComplaintStatusChangeLog();
            dbStatusChangeLog.StatusChangedByUserId = userId;
            dbStatusChangeLog.Complaint_Id = complaintId;
            dbStatusChangeLog.StatusId = statusId;
            dbStatusChangeLog.StatusChangeDateTime = statusSaveDateTime;
            dbStatusChangeLog.Comments = comments;
            dbStatusChangeLog.IsCurrentlyActive = true;
            db.DbComplaintStatusChangeLog.Add(dbStatusChangeLog);
            //db.SaveChanges();
            return dbStatusChangeLog;
        }

        public static List<VmTableStatusHistory> GetComplaintStatusChangeHistoryTableList(int complaintId)
        {
            List<DbComplaintStatusChangeLog> listComplaintsStatusChangeLogs = DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintId(Convert.ToInt32(complaintId));

            List<VmTableStatusHistory> listTableStatusLogs = new List<VmTableStatusHistory>();
            VmTableStatusHistory tempTableStatusHistory = null;
            DbStatus dbStatus = null;
            DbUsers dbUser = null;
            foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintsStatusChangeLogs)
            {
                //
                dbStatus = DbStatus.GetById((int)dbStatusLog.StatusId);
                dbUser = DbUsers.GetUser((int)dbStatusLog.StatusChangedByUserId);
                
                tempTableStatusHistory = new VmTableStatusHistory();
                tempTableStatusHistory.Id = dbStatusLog.Id.ToString();
                tempTableStatusHistory.UserId = dbUser.User_Id.ToString();
                tempTableStatusHistory.UserName = dbUser.Name;
                if (dbUser.Role_Id == Config.Roles.Stakeholder)
                {
                    tempTableStatusHistory.UserHierarchy = ((Config.Hierarchy)(Convert.ToInt32(dbUser.Hierarchy_Id))).ToString();
                    tempTableStatusHistory.UserHierarchyValue = BlHierarchy.GetRegionValueAgainstHierarchy((Config.Hierarchy)(Convert.ToInt32(dbUser.Hierarchy_Id)), DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser));
                }
                else if (dbUser.Role_Id == Config.Roles.Agent || dbUser.Role_Id==Config.Roles.AgentSuperVisor)
                {
                    tempTableStatusHistory.UserHierarchy = "Agent";
                    tempTableStatusHistory.UserHierarchyValue = "Agent";
                }
                
                tempTableStatusHistory.Status = dbStatus.Status;
                tempTableStatusHistory.StatusChangeDateTime =  Utility.ConvertDateTo_DD_MMMM_YY_H_MM_tt(dbStatusLog.StatusChangeDateTime);
                tempTableStatusHistory.Comments = dbStatusLog.Comments;
                tempTableStatusHistory.IsCurrentlyActive = dbStatusLog.IsCurrentlyActive.ToString();
                listTableStatusLogs.Add(tempTableStatusHistory);


                if (dbUser.Campaign_Id == (int) Config.Campaign.SchoolEducationEnhanced)
                {
                    if (tempTableStatusHistory.UserHierarchy == Config.Hierarchy.UnionCouncil.ToString())
                    {
                        tempTableStatusHistory.UserHierarchy = Config.SchoolUcLvl;
                    }
                }
                
                
                //tempTableStatusHistory.ComplaintId = complaintId;

                //
                /*

                tempComplaintAssignedTo = new VmTableTransferHistory();
                tempComplaintAssignedTo.UserName = dbComplaintTransferLog.User.Name;

                tempComplaintAssignedTo.AssignedFromMedium = ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium)).ToString();
                tempComplaintAssignedTo.AssignedFromMediumValue = BlHierarchy.GetRegionValueAgainstHierarchy((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedFromMedium), (int)dbComplaintTransferLog.AssignedFromMediumValue);

                tempComplaintAssignedTo.AssignedToMedium = ((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium)).ToString();
                tempComplaintAssignedTo.AssignedToMediumValue = BlHierarchy.GetRegionValueAgainstHierarchy((Config.Hierarchy)((int)dbComplaintTransferLog.AssignedToMedium), (int)dbComplaintTransferLog.AssignedToMediumValue);

                tempComplaintAssignedTo.AssignedDate = Utility.ConvertDateTo_DD_MMMM_YY_H_MM_tt(dbComplaintTransferLog.AssignmentDateTime.Value);
                tempComplaintAssignedTo.Comment = dbComplaintTransferLog.Comments;

                tempComplaintAssignedTo.IsValid = Utility.GetYesNoFromBool((bool)dbComplaintTransferLog.IsCurrentlyActive);
                */
                //listTableAssignedTo.Add(tempComplaintAssignedTo);
            }
            return listTableStatusLogs;
        }

        public static void ResetComplaintStatus(DBContextHelperLinq db, DbComplaint dbComplaint)
        {
            //db.DbComplaints.Attach(dbComplaint);
            dbComplaint.IsTransferred = true;
            db.Entry(dbComplaint).Property(n => n.IsTransferred).IsModified = true;

            dbComplaint.Status_ChangedBy = null;
            db.Entry(dbComplaint).Property(n => n.Status_ChangedBy).IsModified = true;

            dbComplaint.Status_ChangedBy_Name = null;
            db.Entry(dbComplaint).Property(n => n.Status_ChangedBy_Name).IsModified = true;

            dbComplaint.StatusChangedDate_Time = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedDate_Time).IsModified = true;

            dbComplaint.StatusChangedBy_RoleId = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedBy_RoleId).IsModified = true;

            dbComplaint.StatusChangedBy_HierarchyId = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedBy_HierarchyId).IsModified = true;

            dbComplaint.StatusChangedComments = null;
            db.Entry(dbComplaint).Property(n => n.StatusChangedComments).IsModified = true;

            // Change Status Of Complaint To Pending Fresh
            dbComplaint.Complaint_Status_Id = (int)Config.ComplaintStatus.PendingFresh;
            db.Entry(dbComplaint).Property(n => n.Complaint_Status_Id).IsModified = true;

            dbComplaint.Complaint_Status = Config.StatusDict[(int)Config.ComplaintStatus.PendingFresh];
            db.Entry(dbComplaint).Property(n => n.Complaint_Status).IsModified = true;

            DbStatus dbStatus = DbStatus.GetById((int)dbComplaint.Complaint_Status_Id);
            dbComplaint.Escalation_Status = dbStatus.EscalationStatus;
            db.Entry(dbComplaint).Property(n => n.Escalation_Status).IsModified = true;
        }
    }
}