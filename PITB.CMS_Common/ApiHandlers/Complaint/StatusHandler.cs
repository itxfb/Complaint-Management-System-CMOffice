using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using Amazon.EC2;
using Amazon.IdentityManagement.Model.Internal.MarshallTransformations;
using PITB.CMS_Common.ApiHandlers.Messages;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.FileUpload;

namespace PITB.CMS_Common.ApiHandlers.Complaint
{
    public class StatusHandler
    {
        public static ApiStatus ChangeStatus(string username, int complaintId, int statusId, string statusComments, List<Picture> listPictures, Int64 apiRequestId, bool canSendMessage = true)
        {
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            int campaignId = (int) dbComplaint.Compaign_Id;
            int categoryId = (int) dbComplaint.Complaint_Category;
            int subcategoryId = (int) dbComplaint.Complaint_SubCategory;
            DateTime currentDateTime = DateTime.Now;

            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;

            subcatRetainingHours = DbComplaintType.GetRetainingHoursByTypeId(categoryId);

            if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
            {
                catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId(categoryId);
                //cateogryType = Config.CategoryType.Main;
            }
            else
            {
                catRetainingHours = (float)subcatRetainingHours;
                //cateogryType = Config.CategoryType.Sub;
            }

            DbUsers dbUser = DbUsers.GetByUserName(username);

            //float catRetainingHours =
            //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
            Dictionary<string, object> paramDict = new Dictionary<string, object>();


            List<DbPermissionsAssignment> listDbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campaignId,
                (int)Config.CampaignPermissions.CanResetEscalation);

            bool canResetOnPendingReopen = (listDbPermissionAssignment.Count == 0) ? false : true;

            List<AssignmentModel> assignmentModelList =
                AssignmentHandler.GetAssignmentModelOnStatusChange2(Convert.ToInt32(dbUser.Hierarchy_Id), Convert.ToInt32(dbUser.User_Hierarchy_Id), dbComplaint,
                    statusId, DateTime.Now, DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId, categoryId, subcategoryId), catRetainingHours, canResetOnPendingReopen);

            for (int i = 0; i < 10; i++)
            {
                if (i < assignmentModelList.Count)
                {
                    paramDict.Add("@Dt" + (i + 1), assignmentModelList[i].Dt.ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
                }
                else
                {
                    paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                }
            }
            //
            if (string.IsNullOrEmpty(statusComments))
            {
                statusComments = dbComplaint.StatusChangedComments;
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

            //----- Send message on status change ---------
            if(canSendMessage)
            { 
                new Thread(delegate()
                {
                    TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                        (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
                }).Start();
            }
            /*
            TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                    (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
             */
            return new ApiStatus(Config.ResponseType.Success.ToString(),"Your Complaint status has been changed Successfully");
            //DbComplaint dbComplaint = 
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
    }
}