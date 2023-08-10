using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using Amazon.IdentityManagement.Model.Internal.MarshallTransformations;
using PITB.CMS.Handler.Business;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.View;
using PITB.CRM_API.Handler.Assignment;
using PITB.CRM_API.Handler.Messages;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Models.DB;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Handlers.FileUpload;
using AssignmentModel = PITB.CRM_API.Models.Custom.AssignmentModel;

namespace PITB.CRM_API.Handlers.Complaint
{
    public class SchoolEducationStatusHandler
    {
        public static ApiStatus ChangeStatus(string username, int complaintId, int statusId, string statusComments, List<Picture> listPictures, Int64 apiRequestId)
        {
            CMS.Models.DB.DbComplaint dbComplaint = CMS.Models.DB.DbComplaint.GetByComplaintId(complaintId);
            int campaignId = (int) dbComplaint.Compaign_Id;
            int categoryId = (int) dbComplaint.Complaint_Category;
            int subcategoryId = (int) dbComplaint.Complaint_SubCategory;
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

                DbUsers dbUser = string.IsNullOrEmpty(username) ?  DbUsers.GetByUserName("SchoolEducationTabletMEA") : DbUsers.GetByUserName(username);
                
                //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId); 
                //float catRetainingHours =
                //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                //CMSCookie cookie = AuthenticationHandler.GetCookie();
                //DbComplaint dbComplaint = DbComplaint.GetListByComplaintId(complaintId).First();

                int userHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
                int userInnerHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);

                CMS.Models.DB.DbSchoolsMapping dbSchools = CMS.Models.DB.DbSchoolsMapping.GetById((int)dbComplaint.TableRowRefId);

            //--------- Old code--------------
               /* List<DbAssignmentMatrix> listDbAssignmentMatrix =
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId);

                for (int i = 0; i < listDbAssignmentMatrix.Count; i++)
                {
                    if (Convert.ToInt32(dbSchools.School_Type) == (int)Config.SchoolType.Secondary)
                    {
                        if (listDbAssignmentMatrix[i].ToSourceId > (int)Config.Hierarchy.District)// means tehsil or uc
                        {
                            listDbAssignmentMatrix[i].ToSourceId = (int)Config.Hierarchy.District;
                        }
                    }
                }*/
            //---------- End Old Code ---------


            //---------- New Custom Code ------------

                int? userCategoryId1 = null;
                int? userCategoryId2 = null;
                int? userCategoryId3 = null;

                List<CMS.Models.Custom.AssignmentModel> assignmentModelList = null;
                //if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint)
                {

                    assignmentModelList =
                        CMS.Handler.Complaint.Assignment.AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory((int)dbComplaint.Compaign_Id,
                            (int)dbComplaint.Complaint_Category, (int)dbComplaint.Complaint_SubCategory, true, dbSchools.School_Type, null);

                    //------ Custom Code -------

                    List<CMS.Models.DB.DbUsers> listDbUsers = CMS.Handler.StakeHolder.UsersHandler.GetUsersHierarchyMapping(Convert.ToInt32(dbComplaint.Compaign_Id));
                    CMS.Config.Hierarchy hierarchyId = (CMS.Config.Hierarchy)assignmentModelList[0].SrcId;
                    int? userHierarchyVal = Convert.ToInt32(assignmentModelList[0].UserSrcId);
                    VmComplaint vmComplaint = new VmComplaint();
                    vmComplaint.Province_Id = dbComplaint.Province_Id;
                    vmComplaint.Division_Id = dbComplaint.Division_Id;
                    vmComplaint.District_Id = dbComplaint.District_Id;
                    vmComplaint.Tehsil_Id = dbComplaint.Tehsil_Id;
                    vmComplaint.UnionCouncil_Id = dbComplaint.UnionCouncil_Id;

                    List<int?> listStatusIds = new List<int?>{(int)Config.ComplaintStatus.PendingReopened};

                    if (listStatusIds.Where(n => n == statusId).FirstOrDefault() != null) // if status exist in 
                    {

                    CMS.Models.Custom.OriginHierarchy originHierarchy =
                            CMS.Handler.Business.BlSchool.EvaluateAssignmentMartix(vmComplaint, listDbUsers,
                                assignmentModelList, dbSchools, hierarchyId, userHierarchyVal, ref userCategoryId1,
                                ref userCategoryId2, ref userCategoryId3, 0, null);
                        paramDict.Add("@UserCategoryId1", userCategoryId1);
                        paramDict.Add("@UserCategoryId2", userCategoryId2);

                        paramDict.Add("@Origin_HierarchyId", originHierarchy.OriginHierarchyId);
                        paramDict.Add("@Origin_UserHierarchyId", originHierarchy.OriginUserHierarchyId);
                        paramDict.Add("@Origin_UserCategoryId1", originHierarchy.OriginUserCategoryId1);
                        paramDict.Add("@Origin_UserCategoryId2", originHierarchy.OriginUserCategoryId2);
                        paramDict.Add("@Is_AssignedToOrigin", originHierarchy.IsAssignedToOrigin);
                    }
                    else
                    {
                        paramDict.Add("@UserCategoryId1", dbComplaint.UserCategoryId1);
                        paramDict.Add("@UserCategoryId2", dbComplaint.UserCategoryId2);

                        paramDict.Add("@Origin_HierarchyId", dbComplaint.Origin_HierarchyId);
                        paramDict.Add("@Origin_UserHierarchyId", dbComplaint.Origin_UserHierarchyId);
                        paramDict.Add("@Origin_UserCategoryId1", dbComplaint.Origin_UserCategoryId1);
                        paramDict.Add("@Origin_UserCategoryId2", dbComplaint.Origin_UserCategoryId2);
                        paramDict.Add("@Is_AssignedToOrigin", dbComplaint.Is_AssignedToOrigin);
                    }
                    //--------------------------
                }


            //--------- End new Custom Code ----------

                List<DbPermissionsAssignment> listDbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions((int)Config.Campaign.FixItLwmc,
                (int)Config.CampaignPermissions.CanResetEscalation);

                List<CMS.Models.DB.DbAssignmentMatrix> listDbAssignmentMatrix = CMS.Models.DB.DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId);
                //bool canResetOnPendingReopen = (listDbPermissionAssignment == null) ? false : true;
                //List<AssignmentModel> 
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

                paramDict.Add("@Status_ChangedBy", dbUser.Id);
                paramDict.Add("@Status_ChangedBy_Name", dbUser.Username);
                paramDict.Add("@StatusChangedDate_Time", currentDateTime);
                paramDict.Add("@StatusChangedBy_RoleId", Convert.ToInt32(dbUser.Role_Id));
                paramDict.Add("@StatusChangedBy_HierarchyId", Convert.ToInt32(dbUser.Hierarchy_Id));
                paramDict.Add("@StatusChangedBy_User_HierarchyId", Convert.ToInt32(dbUser.User_Hierarchy_Id));
                paramDict.Add("@StatusChanged_Comments", statusComments.ToDbObj());
                // ------ adding supporting params for escalation params -----------------

                paramDict.Add("@MaxLevel", assignmentModelList.Count);

                paramDict.Add("@MinSrcId", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMinSrcId(assignmentModelList));
                paramDict.Add("@MaxSrcId", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMaxSrcId(assignmentModelList));

                paramDict.Add("@MinUserSrcId", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMinUserSrcId(assignmentModelList));
                paramDict.Add("@MaxUserSrcId", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMaxUserSrcId(assignmentModelList));


                paramDict.Add("@MinSrcIdDate", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMinDate(assignmentModelList));
                paramDict.Add("@MaxSrcIdDate", PITB.CMS.Handler.Complaint.Assignment.AssignmentHandler.GetMaxDate(assignmentModelList));
                //------------------------------------------------------------------------
                
                DBHelper.GetDataTableByStoredProcedure("[PITB].[Update_Complaints_Status]", paramDict);
                

                DBContextHelperLinq db = new DBContextHelperLinq();

                MakeLastLogOfComplaintStatusInactive(complaintId, db);
                DbComplaintStatusChangeLog dbStatusChangeLog = SaveComplaintStatusInLog(dbUser.Id, complaintId, statusId,
                    currentDateTime, statusComments, db);
                db.SaveChanges();

                int statusLogId = dbStatusChangeLog.Id;
                //----- Save Image in db
                if (listPictures != null)
                {
                    foreach (Picture picture in listPictures)
                    {
                        //FileUploadHandler.StartUploadUtilityPWS(picture.picture, "Image", "image/jpeg", ".jpg", campaignId, complaintId, statusLogId, Config.AttachmentReferenceType.ChangeStatus, apiRequestId);
                        FileUploadModel fileUploadModel = new FileUploadModel(picture.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, statusLogId, Config.AttachmentReferenceType.ChangeStatus, null, apiRequestId);
                        FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                    }
                }

                new Thread(delegate()
                {
                    string url = PITB.CMS.Config.ApiUrlSchoolComplaintSystem + "/api/public_school_complaint_status";
                    CMS.Handler.Complaint.SchoolEducatonStatusHandler.PushStatusToSchoolComplaintSystem(url, complaintId, statusId, PITB.CMS.Utility.GetAlteredStatus(Convert.ToInt32(campaignId), PITB.CMS.Models.DB.DbStatus.GetById(statusId).Status));
                }).Start();


                //----- Send message on status change ---------
                /*
                new Thread(delegate()
                {
                    TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                        (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
                }).Start();
                */
                /*
                TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                        (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
                 */
                return new ApiStatus(Config.ResponseType.Success.ToString(), "Your Complaint status has been changed Successfully");

        }

        private static void MakeLastLogOfComplaintStatusInactive(int complaintId, DBContextHelperLinq db)
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

        private static DbComplaintStatusChangeLog SaveComplaintStatusInLog(int userId, int complaintId, int statusId, DateTime statusSaveDateTime, string comments, DBContextHelperLinq db)
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

    }
}