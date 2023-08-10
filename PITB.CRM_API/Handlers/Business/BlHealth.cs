using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using Amazon.Runtime;
using Facebook;
using PITB.CMS;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using PITB.CRM_API.Handler.Messages;
using PITB.CRM_API.Handlers.FileUpload;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.API.Health;
//using PITB.CRM_API.Models.DB;
//using PITB.CRM_API.Helper.Database;
//using PITB.CRM_API.Models.Custom;
//using PITB.CRM_API.Handler.Assignment;
using System.Data;
using Amazon.S3.Model;
using Amazon.S3;
using Newtonsoft.Json;
using PITB.CRM_API.Handlers.Translation;
//using PITB.CRM_API.Models.DB;
//using DbAssignmentMatrix = PITB.CRM_API.Models.DB.DbAssignmentMatrix;
//using DbAttachments = PITB.CRM_API.Models.DB.DbAttachments;
//using DbComplaintSubType = PITB.CRM_API.Models.DB.DbComplaintSubType;
// DbDistrict = PITB.CRM_API.Models.DB.DbDistrict;
//using DbPersonInformation = PITB.CRM_API.Models.DB.DbPersonInformation;
using PITB.CRM_API.Models.Custom;
using Image = Amazon.EC2.Model.Image;
using PITB.CMS.Handler.Complaint.Status;
using PITB.CMS.Handler.Complaint.Assignment;
using AssignmentModel = PITB.CMS.Models.Custom.AssignmentModel;
using OriginHierarchy = PITB.CMS.Models.Custom.OriginHierarchy;

//using PITB.CRM_API.Handlers.Complaint;


namespace PITB.CRM_API.Handlers.Business
{
    public class BlHealth
    {
        public static HealthResponseModel.SubmitComplaint SubmitComplaint(HealthRequestModel.SubmitComplaint submitComplaintModel, Int64 apiRequestId/*, int appId*/)
        {
            try
            {
                //Dictionary<int, int> dictCampMap = new Dictionary<int, int>()
                //{
                //   {1, 68},
                //   {2, 72},
                //   {3, 73},
                //   {4, 74},
                //   {6, 69},
                //};

                List<int?> listModuleIds = new List<int?> { (int)CMS.Config.CrmModule.CampaignIds, (int)CMS.Config.CrmModule.StatusIds };
                List<DbCrmIdsMappingToOtherSystem> listMap = DbCrmIdsMappingToOtherSystem.Get(listModuleIds, (int)CMS.Config.OtherSystemId.Health);

                Dictionary<int, int> dictCampMap = DbCrmIdsMappingToOtherSystem.GetDictionaryOtsToCrm(listMap.Where(n => n.Crm_Module_Tag == "Module::CampaignIds").ToList());

                Dictionary<int, int> dictStatusMap = DbCrmIdsMappingToOtherSystem.GetDictionaryOtsToCrm(listMap.Where(n => n.Crm_Module_Tag == "Module::StatusIds").ToList());

                //statusId = dictStatusIds[statusId];


                //1 Pending
                //2 Resolved
                //3 Un-Traceable
                //4 Not Applicable
                //5 Pending (Re-opened)
                //6 Satisfactory Closed
                //7 Closed (Chronic)

                //Dictionary<int, int> dictStatusMap = new Dictionary<int, int>()
                //{
                //   {1, 1}, // pending
                //   {2, 8}, // resolved
                //   {3, 17}, // untracable
                //   {4, 4}, // Not Applicable
                //   {5, 7}, // Pending (Re-opened)
                //   {6, 11}, // Satisfactory Closed
                //   {7, 20}  // Closed (Chronic)
                //};
                submitComplaintModel.statusId = dictStatusMap[submitComplaintModel.statusId];
                submitComplaintModel.campaignID = dictCampMap[submitComplaintModel.campaignID];

                int provinceId = 0, divisionId = 0, districtId = 0, tehsilId = 0, ucId = 0, wardId = 0;
                //Config.AppID app = (Config.AppID)appId;
                DateTime nowDate = DateTime.Now;

                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

                DbPersonInformation dbPersonInfo = DbPersonInformation.GetByCnic(submitComplaintModel.cnic);

                int? personId = 0;
                if (dbPersonInfo != null) personId = dbPersonInfo.Person_id;

                // complaint Info
                paramDict.Add("@Id", -1);
                paramDict.Add("@Person_Id", personId.ToDbObj());
                if (submitComplaintModel.departmentId!=null)
                {
                    submitComplaintModel.departmentId =
                        DbHealthDepartments.GetBy(Convert.ToInt32(submitComplaintModel.departmentId)).DbDepartment.Id;
                }
                submitComplaintModel.categoryID = DbHealthComplaintCategories.GetBy(submitComplaintModel.categoryID).DbComplaintType.Complaint_Category;
                submitComplaintModel.subCategoryID = DbHealthComplaintSubCategories.GetBy(submitComplaintModel.subCategoryID).DbComplaintSubType.Complaint_SubCategory;


                submitComplaintModel.districtID = DbHealthDistricts.GetBy(submitComplaintModel.districtID).DbDistrict.District_Id;
                submitComplaintModel.tehsilID = DbHealthTehsil.GetBy(submitComplaintModel.tehsilID).DbTehsil.Tehsil_Id;

                tehsilId = submitComplaintModel.tehsilID;
                districtId = submitComplaintModel.districtID;
                divisionId = Convert.ToInt32(DbDistrict.GetById((int)districtId).Division_Id);
                provinceId = Convert.ToInt32(PITB.CMS.Models.DB.DbDivision.GetById(divisionId).Province_Id);
                

                // Complaint Info
                paramDict.Add("@DepartmentId", submitComplaintModel.departmentId.ToDbObj()); //submitComplaintModel.departmentId.ToDbObj());
                paramDict.Add("@Complaint_Type", Convert.ToInt32(submitComplaintModel.complaintType));
                paramDict.Add("@Complaint_Category", submitComplaintModel.categoryID.ToDbObj());//submitComplaintModel.categoryID.ToDbObj());
                paramDict.Add("@Complaint_SubCategory", submitComplaintModel.subCategoryID.ToDbObj()); //submitComplaintModel.subCategoryID.ToDbObj());
                paramDict.Add("@Compaign_Id", submitComplaintModel.campaignID.ToDbObj());
                /*if (app == Config.AppID.Awazekhalq)  // Hardcoding for Bahawalpur
                {
                    provinceId = 1;
                    divisionId = 6;
                    districtId = 8;

                    paramDict.Add("@Province_Id", 1); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", 6);
                    //DbDistrict.GetById((int) submitComplaintModel.districtID).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", 8); //submitComplaintModel.districtID.ToDbObj());
                }
                else if (app == Config.AppID.SmartLahore)*/
                {

                    //provinceId = submitComplaintModel.pr;
                    



                    paramDict.Add("@Province_Id", provinceId.ToDbObj()); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", divisionId.ToDbObj());

                    paramDict.Add("@District_Id", submitComplaintModel.districtID.ToDbObj()); //submitComplaintModel.districtID.ToDbObj());
                }
                //tehsilId = submitComplaintModel.tehsilID;
                //ucId = null;//submitComplaintModel.ucID;
                //wardId = null;//submitComplaintModel.wardID;


                paramDict.Add("@Tehsil_Id", submitComplaintModel.tehsilID.ToDbObj());
                paramDict.Add("@UnionCouncil_Id", ((int?)(null)).ToDbObj());
                paramDict.Add("@Ward_Id", ((int?)(null)).ToDbObj());
                paramDict.Add("@Complaint_Remarks", submitComplaintModel.comment.ToDbObj());
                paramDict.Add("@Agent_Comments", (null as object).ToDbObj());
                paramDict.Add("@ComplaintSrc", Convert.ToInt32(submitComplaintModel.complaintSrcId).ToDbObj()); //((int)Config.ComplaintSource.Mobile).ToDbObj());
                paramDict.Add("@HelplineSrc", Convert.ToInt32(submitComplaintModel.helplineSrc).ToDbObj());


                paramDict.Add("@Agent_Id", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Address", (null as object).ToDbObj());
                paramDict.Add("@Business_Address", (null as object).ToDbObj());

                paramDict.Add("@Complaint_Status_Id", submitComplaintModel.statusId); //Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
                //paramDict.Add("@Created_Date", nowDate.ToDbObj());
                paramDict.Add("@Created_Date", submitComplaintModel.createdDateTime.ToDbObj());
                paramDict.Add("@Created_By", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
                paramDict.Add("@Completed_Date", (null as object).ToDbObj());
                //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
                paramDict.Add("@Updated_By", (null as object).ToDbObj());
                paramDict.Add("@Is_Deleted", false);
                paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
                paramDict.Add("@Deleted_By", (null as object).ToDbObj());
                paramDict.Add("@Latitude", submitComplaintModel.lattitude.ToDbObj());
                paramDict.Add("@Longitude", submitComplaintModel.longitude.ToDbObj());

                paramDict.Add("@IsComplaintEditing", false);




                // Personal Info
                /*
                paramDict.Add("@p_Person_id", personId.ToDbObj());
                paramDict.Add("@Person_Name", submitComplaintModel.personName.ToDbObj());
                paramDict.Add("@Person_Father_Name", (null as object).ToDbObj());
                paramDict.Add("@Cnic_No", submitComplaintModel.cnic.ToDbObj());
                paramDict.Add("@Gender", (1).ToDbObj());
                paramDict.Add("@Mobile_No", submitComplaintModel.personContactNumber.ToDbObj());
                paramDict.Add("@Secondary_Mobile_No", (null as object).ToDbObj());
                paramDict.Add("@LandLine_No", (null as object).ToDbObj());
                paramDict.Add("@Person_Address", (null as object).ToDbObj());
                paramDict.Add("@Email", (null as object).ToDbObj());
                paramDict.Add("@Nearest_Place", (null as object).ToDbObj());
                paramDict.Add("@p_Province_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Division_Id", (null as object).ToDbObj());
                paramDict.Add("@p_District_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Tehsil_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Town_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Uc_Id", (null as object).ToDbObj());
                paramDict.Add("@p_Created_By", (null as object).ToDbObj());
                paramDict.Add("@p_Updated_By", (null as object).ToDbObj());
                */

                paramDict.Add("@p_Person_id", personId.ToDbObj());
                paramDict.Add("@Person_Name", submitComplaintModel.personName.ToDbObj());
                paramDict.Add("@Person_Father_Name", (null as object).ToDbObj());
                paramDict.Add("@Cnic_No", submitComplaintModel.cnic.ToDbObj());
                paramDict.Add("@Gender", (1).ToDbObj());
                paramDict.Add("@Mobile_No", submitComplaintModel.personContactNumber.ToDbObj());
                paramDict.Add("@Secondary_Mobile_No", (null as object).ToDbObj());
                paramDict.Add("@LandLine_No", (null as object).ToDbObj());
                paramDict.Add("@Person_Address", (null as object).ToDbObj());
                paramDict.Add("@Email", (null as object).ToDbObj());
                paramDict.Add("@Nearest_Place", (null as object).ToDbObj());
                paramDict.Add("@p_Province_Id", (provinceId).ToDbObj());
                paramDict.Add("@p_Division_Id", (divisionId).ToDbObj());
                paramDict.Add("@p_District_Id", (districtId).ToDbObj());
                paramDict.Add("@p_Tehsil_Id", (tehsilId).ToDbObj());
                paramDict.Add("@p_Town_Id", (tehsilId).ToDbObj());
                paramDict.Add("@p_Uc_Id", (ucId).ToDbObj());
                paramDict.Add("@p_Created_By", (null as object).ToDbObj());
                paramDict.Add("@p_Updated_By", (null as object).ToDbObj());

                paramDict.Add("@IsProfileEditing", false);

                float catRetainingHours = 0;
                float? subcatRetainingHours = 0;

                //AssignmentMatrix
                List<AssignmentModel> assignmentModelList = null;

                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)submitComplaintModel.subCategoryID);
                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)submitComplaintModel.categoryID);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                //catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int) submitComplaintModel.categoryID);
                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)submitComplaintModel.campaignID, submitComplaintModel.categoryID, submitComplaintModel.subCategoryID), catRetainingHours)/*nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)submitComplaintModel.campaignID, submitComplaintModel.categoryID, submitComplaintModel.subCategoryID), catRetainingHours*/);
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
                paramDict.Add("@Ref_Complaint_Src", CMS.Config.OtherSystemId.Health);
                paramDict.Add("@Ref_Complaint_Id", submitComplaintModel.refComplaintId);

                DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
                //Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
                string[] complaintIdStrArr = dt.Rows[0][1].ToString().Split('-');
                int campaignId = Convert.ToInt32(complaintIdStrArr[0]);
                int complaintId = Convert.ToInt32(complaintIdStrArr[1]);
                
                string complaintIdStr = dt.Rows[0][1].ToString();

                //SaveMobileRequest(submitComplaintModel, Convert.ToInt32(complaintIdStr.Split('-')[1]), apiRequestId);
                if (submitComplaintModel.PicturesList != null)
                {
                    foreach (Picture picture in submitComplaintModel.PicturesList)
                    {
                        FileUploadModel fileUploadModel = new FileUploadModel(picture.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, null, apiRequestId);
                        FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                        //FileUploadHandler.StartUploadUtilityPWS(picture.picture, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, apiRequestId);
                        //StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                    }
                }
                /*
                if (!string.IsNullOrEmpty(submitComplaintModel.video))
                {
                    StartUploadUtility(submitComplaintModel.video, "Video", "application/octet-stream",
                        submitComplaintModel.videoFileExtension, complaintIdStr, Config.FileType.Video, apiRequestId);
                }*/
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString());
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(),  dt.Rows[0][1].ToString());
                //return "Your Complaint Id = " + dt.Rows[0][1].ToString();
                // Send message


                //new Thread(delegate()
                //{
                //    TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                //    submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                //    submitComplaintModel.categoryID);
                //}).Start();

                //new Thread(delegate()
                //{
                //    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId, submitComplaintModel.personContactNumber);
                //}).Start();

                // Commenting send message done
                /*TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    submitComplaintModel.categoryID);

                TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId);
                */
                return new HealthResponseModel.SubmitComplaint(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString(), dt.Rows[0][1].ToString().Split('-')[1]);
            }
            catch (Exception ex)
            {
                //return Utility.GetStatus(Config.ResponseType.Failure.ToString(), ex.Message);
                //return Config.ResponseType.Failure.ToString()+" Exception = "+ex.Message;
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(JsonConvert.SerializeObject(submitComplaintModel), "", false, ex.Message));

                return new HealthResponseModel.SubmitComplaint(Config.ResponseType.Failure.ToString(), "Server Error", "");
            }
        }


        public static ApiStatus ChangeStatus(PITB.CMS.Models.DB.DbUsers dbUser, int complaintId, DateTime createdDateTime, int statusId, string statusComments, List<Picture> listPictures, Int64 apiRequestId, bool canSendMessage = false)
        {
            //userId = 31939;
            List<int?> listModuleIds = new List<int?> {(int) CMS.Config.CrmModule.StatusIds};
            List<DbCrmIdsMappingToOtherSystem> listMap = DbCrmIdsMappingToOtherSystem.Get(listModuleIds, (int)CMS.Config.OtherSystemId.Health);

            //Dictionary<int, int> dictCampIds = DbCrmIdsMappingToOtherSystem.GetDictionaryOtsToCrm(listMap.Where(n => n.Crm_Module_Tag == "Module::CampaignIds").ToList());

            Dictionary<int, int> dictStatusMap = DbCrmIdsMappingToOtherSystem.GetDictionaryOtsToCrm(listMap.Where(n => n.Crm_Module_Tag == "Module::StatusIds").ToList());

            statusId = dictStatusMap[statusId];

            PITB.CMS.Models.DB.DbComplaint dbComplaint = PITB.CMS.Models.DB.DbComplaint.GetByComplaintId(complaintId);
            int campaignId = (int)dbComplaint.Compaign_Id;
            int categoryId = (int)dbComplaint.Complaint_Category;
            int subcategoryId = (int)dbComplaint.Complaint_SubCategory;
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

            //PITB.CMS.Models.DB.DbUsers dbUser = PITB.CMS.Models.DB.DbUsers.GetByUserName(username);

            //float catRetainingHours =
            //    DbComplaintType.GetRetainingHoursByTypeId((int) vmStakeholderComplaint.Complaint_Category);
            Dictionary<string, object> paramDict = new Dictionary<string, object>();


            List<PITB.CRM_API.Models.DB.DbPermissionsAssignment> listDbPermissionAssignment = PITB.CRM_API.Models.DB.DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campaignId,
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
                    //paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
                }
                else
                {
                    paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                    //paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                }
            }
            //
            if (string.IsNullOrEmpty(statusComments))
            {
                statusComments = dbComplaint.StatusChangedComments;
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


            DBContextHelperLinq db = new PITB.CMS.Helper.Database.DBContextHelperLinq();

            StatusHandler.MakeLastLogOfComplaintStatusInactive(complaintId, db);
            DbComplaintStatusChangeLog dbStatusChangeLog = StatusHandler.SaveComplaintStatusInLog(dbUser.User_Id, complaintId, statusId,
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
            if (canSendMessage)
            {
                new Thread(delegate()
                {
                    PITB.CMS.Handler.Messages.TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                        (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
                }).Start();
            }
            /*
            TextMessageHandler.SendMessageOnStatusChange(dbComplaint.Person_Contact,
                    (int)dbComplaint.Compaign_Id, dbComplaint.Id, (int)dbComplaint.Complaint_Category, statusId, statusComments);
             */
            return new ApiStatus(Config.ResponseType.Success.ToString(), "Your Complaint status has been changed Successfully");
            //DbComplaint dbComplaint = 
        }

        public static void SaveMobileRequest(SubmitComplaintModel submitComplaintModel, int complaintId, Int64 apiRequestId)
        {
            DateTime requestDateTime = submitComplaintModel.date;
            requestDateTime = requestDateTime + submitComplaintModel.time;

            DBContextHelperLinq db = new DBContextHelperLinq();
            DbMobileRequest dbMobileRequest = new DbMobileRequest
            {
                ComplaintId = complaintId,
                Latitude =
                    (string.IsNullOrEmpty(submitComplaintModel.lattitude))
                        ? (double?)null
                        : double.Parse(submitComplaintModel.lattitude),
                Longitude =
                    (string.IsNullOrEmpty(submitComplaintModel.longitude))
                        ? (double?)null
                        : double.Parse(submitComplaintModel.longitude),
                RequestType = (int)Config.MobileUserRequestType.AddComplaint,
                RequestTypeId = complaintId,
                Imei = submitComplaintModel.imei_number,
                CreatedDateTime = requestDateTime,
                ApiRequestId = apiRequestId
            };
            db.DbMobileRequest.Add(dbMobileRequest);
            db.SaveChanges();
            //db.db
        }

        //public static void StartUploadUtility(string base64String, string filePrefix, string contentType, string fileExtension, string complaintIdStr, Config.FileType fileType, Int64 apiRequestId)
        //{
        //    Config.AmazonConfig amazonConfig = Utility.GetAmazonConfigModel();
        //    AmazonS3Client client = Utility.GetS3Client(amazonConfig.AmazonKeyId, amazonConfig.AmazonSecretKey);
        //    string fileName = Utility.GetUniqueFileName(filePrefix, complaintIdStr, fileExtension);
        //    int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
        //    UploadAmazon(client, base64String, contentType, fileExtension, amazonConfig.AmazonBucket, fileName, complaintId, amazonConfig.AmazonUrlPrefix + fileName, fileType, apiRequestId);

        //}

        //private static void UploadAmazon(AmazonS3Client client, string base64String, string contentType, string fileExtension, string bucket, string filename, int complaintId, string completeUrl, Config.FileType fileType, Int64 apiRequestId)
        //{
        //    try
        //    {
        //        // Initialization.
        //        string path = string.Empty;
        //        //AmazonS3Client client = GetS3Client();

        //        // Loading image.
        //        byte[] bytes = Convert.FromBase64String(base64String);

        //        // Setting.
        //        PutObjectRequest request = new PutObjectRequest
        //        {
        //            BucketName = bucket,
        //            CannedACL = S3CannedACL.PublicRead,
        //            Key = string.Format("{0}", filename),
        //            ContentType = contentType
        //        };


        //        // Setting.
        //        //request.AddHeader(AmazonSettings.AMAZON_PUBLIC_HEADER, AmazonSettings.AMAZON_PUBLIC_VALUE);

        //        // Uploading.
        //        using (var ms = new MemoryStream(bytes))
        //        {
        //            request.InputStream = ms;
        //            client.PutObject(request);
        //        }
        //        StoreImageUrlInDb(completeUrl, complaintId, apiRequestId, contentType, fileExtension, fileType);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Info.
        //        throw ex;
        //    }
        //}

        //private static void StoreImageUrlInDb(string url, int complaintId, Int64 apiRequestId, string contentType, string fileExtension, Config.FileType fileType)
        //{
        //    DBContextHelperLinq db = new DBContextHelperLinq();
        //    DbAttachments dbAttachments = new DbAttachments();
        //    dbAttachments.Source_Id = (int)Config.AttachmentSaveType.WebServer;
        //    dbAttachments.Source_Url = url;
        //    dbAttachments.Complaint_Id = complaintId;
        //    dbAttachments.ApiRequestId = apiRequestId;
        //    dbAttachments.ReferenceType = (int)Config.AttachmentReferenceType.Add;
        //    dbAttachments.ReferenceTypeId = complaintId;
        //    dbAttachments.FileName = Config.FileName;
        //    dbAttachments.FileExtension = fileExtension;
        //    dbAttachments.FileContentType = contentType;
        //    dbAttachments.FileType = (int)fileType;
        //    db.DbAttachments.Add(dbAttachments);
        //    db.SaveChanges();
        //    //db.db
        //}
    }
}