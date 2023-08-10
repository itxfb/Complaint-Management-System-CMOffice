using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using Facebook;
using System.Dynamic;
using Newtonsoft.Json;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.ApiHandlers.Location;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Handler.API;
using PITB.CMS_Common.ApiHandlers.Translation;
using PITB.CMS_Common.ApiHandlers.Social;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Messages;

namespace PITB.CMS_Common.ApiHandlers.Business.LWMC
{
    public class BlLwmcApiHandler
    {
        public static SyncModel SyncComplainant(SubmitSyncComplainant submitSyncComplainant, int appId, Config.Language language, Config.PlatformID platformId, int dbVersionId, int appVersionId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(submitSyncComplainant.Cnic, submitSyncComplainant.MobileNo, db);
            SyncModel syncModel = null;

            int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID)appId);

            string status, message;

            // platformId Check


            // if (dbPersonalInfo != null) // username and cnic present
            {
                // if (dbPersonalInfo.Imei_No == null || dbPersonalInfo.Imei_No == submitSyncComplainant.ImeiNo)
                {

                    status = Config.ResponseType.Success.ToString();
                    message = "Successfully Synced";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = GetSyncModelAgaistCnicAndAppId(submitSyncComplainant.Cnic, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }

                    if (dbPersonalInfo != null)
                    {
                        DbUsersVersionMapping.Update_AddVersion(Config.UserType.Complainant, dbPersonalInfo.Person_id, (int)platformId, appId, appVersionId);
                    }

                    //TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                //else // wrong mobile
                //{
                //    status = Config.ResponseType.Failure.ToString();
                //    message = "Sync Unsuccessful Already Registered with another imei";
                //}
            }
            //   else // if username not present
            //   {
            //       status = Config.ResponseType.Failure.ToString();
            //       message = "Sync Unsuccessful Mobile no or cnic incorrect";
            //   }
            if (syncModel == null)
            {
                syncModel = new SyncModel();
            }

            syncModel.Status = status;
            syncModel.Message = message;
            syncModel.DbVersionId = currDbVersion;
            return syncModel;
        }
        public static SyncModel GetSyncModelAgaistCnicAndAppId(string cnic, int appId, Config.Language language) // SmartApp
        {
            List<Config.AppConfig> listAppCampaignConfig = Config.LwmcAppConfigurations;
            List<Config.CampaignConfig> listCampaignConfig = Utility.GetCampaignConfigList(listAppCampaignConfig, (Config.AppID)appId);

            List<Config.Campaign> listCampaignIds = listCampaignConfig.Select(n => n.CampaignId).ToList();
            SyncModel syncModel = new SyncModel
            {
                ListCategory = new List<DbComplaintType>(),
                ListSubCategory = new List<DbComplaintSubType>()
            };
            List<DbComplaintType> listComplaintTypeTemp = null;
            //int districtId = Config.CampDistDictionary[listCampaignIds[0]];


            foreach (var campaign in listCampaignIds)
            {
                var campaignId = (int)campaign;
                listComplaintTypeTemp = DbComplaintType.GetByCampaignId(campaignId);

                syncModel.ListCategory.AddRange(listComplaintTypeTemp);
                syncModel.ListSubCategory.AddRange(
                    DbComplaintSubType.GetByComplaintTypes(
                        listComplaintTypeTemp.Select(n => n.Complaint_Category).ToList()));
            }



            //DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnic(cnic);

            //DbProvince dbProvince = DbProvince.GetById(Convert.ToInt32(dbPersonalInfo.Province_Id));
            //syncModel.ListProvince = (dbProvince != null) ? new List<DbProvince>() { dbProvince } : new List<DbProvince>(); //DbProvince.GetAllProvincesList();
            ////syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            //DbDistrict dbDistrict = DbDistrict.GetById(Convert.ToInt32(dbPersonalInfo.District_Id));
            //syncModel.ListDistrict = (dbDistrict != null) ? new List<DbDistrict>() { dbDistrict } : new List<DbDistrict>();
            ////syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetById(districtId) }; // hardcoding for bahawalpur 

            //DbTehsil dbTehsil = DbTehsil.GetById(Convert.ToInt32(dbPersonalInfo.Tehsil_Id));
            //syncModel.ListTehsil = (dbTehsil != null) ? new List<DbTehsil>() { dbTehsil } : new List<DbTehsil>();
            ////syncModel.ListTehsil = DbTehsil.GetTehsilList(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id);

            ////int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId((int) listCampaignConfig[0].CampaignId);

            //DbUnionCouncils dbUnionCouncil = DbUnionCouncils.GetById(Convert.ToInt32(dbPersonalInfo.Uc_Id));
            //syncModel.ListUnionCouncils = (dbTehsil != null) ? new List<DbUnionCouncils>() { dbUnionCouncil } : new List<DbUnionCouncils>();

            //DbWards dbWards = DbWards.GetByWardId(Convert.ToInt32(dbPersonalInfo.Ward_Id));
            //syncModel.ListWards = (dbWards != null) ? new List<DbWards>() { dbWards } : new List<DbWards>();

            //  syncModel.ListSubCategory = DbComplaintSubType.PopulateImageInBase64FromComplaintsSubtype(syncModel.ListSubCategory);


            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());

            /*  Enable them when these fields also need to be translated
             * 
            syncModel.ListProvince.GetTranslatedList("Province_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListDistrict.GetTranslatedList("District_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListTehsil.GetTranslatedList("Tehsil_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListUnionCouncils.GetTranslatedList("Councils_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListWards.GetTranslatedList("Wards_UrduName", translationDict, Config.Language.Urdu);
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Name", translationDict, language);
             *
             */

            syncModel.ListCategory.GetTranslatedList("Category_UrduName", "Name", translationDict, Config.Language.Urdu);

            //          syncModel.ListSubCategory.GetTranslatedList<DbComplaintSubType>("Name", translationDict, language);
            syncModel.ListSubCategory.GetTranslatedList("SubCategory_UrduName", "Name", translationDict, Config.Language.Urdu);


            return syncModel;
        }
        public static ComplaintSubmitResponse SubmitComplaint(SubmitLWMCComplaintModel submitComplaintModel, Int64 apiRequestId, int appId)
        {
            try
            {


                LocationCoordinate complaintLocation = new LocationCoordinate(
                                                        Convert.ToDouble(submitComplaintModel.lattitude),
                                                        Convert.ToDouble(submitComplaintModel.longitude)
                                                        );

                int provinceId = -1, divisionId = -1, districtId = -1, tehsilId = -1, ucId = -1, wardId = -1;
                LocationMapping mapping;
                if (LocationHandler.IsLocationExistInPolygon(complaintLocation, Config.Hierarchy.UnionCouncil,
                    out mapping))
                {

                    List<LocationMapping> allDistrictDivisionTehsilTownsList =
                        LocationHandler.GetAllAboveHirerchyByHirerchyIdAndType(mapping);
                    provinceId =
                        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Province)
                            .HirerchyTypeID;
                    divisionId =
                        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Division)
                            .HirerchyTypeID;
                    districtId =
                        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.District)
                            .HirerchyTypeID;
                    tehsilId =
                        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Tehsil)
                            .HirerchyTypeID;
                    ucId =
                        allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.UnionCouncil)
                            .HirerchyTypeID;
                }
                else
                {
                    return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "We are sorry, LWMC does not operate in this area.", "");
                }

                submitComplaintModel.provinceID = provinceId;
                submitComplaintModel.districtID = districtId;
                submitComplaintModel.tehsilID = tehsilId;
                submitComplaintModel.ucID = ucId;
                submitComplaintModel.wardID = wardId;


                Config.AppID app = (Config.AppID)appId;
                DateTime nowDate = DateTime.Now;

                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                DbPersonInformation dbPersonInfo = DbPersonInformation.GetByCnic
                    (
                        submitComplaintModel.cnic,
                        submitComplaintModel.userId,
                        submitComplaintModel.userProvider
                     );

                //if (string.IsNullOrEmpty(submitComplaintModel.userId) &&
                //    string.IsNullOrEmpty(submitComplaintModel.userProvider))
                //{
                //    dbPersonInfo = DbPersonInformation.GetByCnic(submitComplaintModel.cnic);

                //}
                //else
                //{
                //    int dbPersonId = DbPersonInformation.GetPersonIdByUserId(submitComplaintModel.userId,
                //        submitComplaintModel.userProvider);
                //    dbPersonInfo = DbPersonInformation.GetPersonInformationByPersonId(dbPersonId);

                //}


                int? personId = 0;
                if (dbPersonInfo != null) personId = dbPersonInfo.Person_id;

                // complaint Info
                paramDict.Add("@Id", -1);
                paramDict.Add("@Person_Id", personId.ToDbObj());



                // Complaint Info
                paramDict.Add("@DepartmentId", submitComplaintModel.departmentId.ToDbObj());
                paramDict.Add("@Complaint_Type", Config.ComplaintType.Complaint);
                paramDict.Add("@Complaint_Category", submitComplaintModel.categoryID.ToDbObj());
                paramDict.Add("@Complaint_SubCategory", submitComplaintModel.subCategoryID.ToDbObj());
                paramDict.Add("@Compaign_Id", submitComplaintModel.campaignID.ToDbObj());



                // if (app == Config.AppID.FixitLwmc)
                {

                    // provinceId = submitComplaintModel.provinceID;
                    //  divisionId = Convert.ToInt32(DbDistrict.GetById((int)submitComplaintModel.districtID).Division_Id);
                    //districtId = submitComplaintModel.districtID;

                    paramDict.Add("@Province_Id", submitComplaintModel.provinceID.ToDbObj()); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", divisionId);

                    paramDict.Add("@District_Id", submitComplaintModel.districtID.ToDbObj()); //submitComplaintModel.districtID.ToDbObj());
                }



                tehsilId = submitComplaintModel.tehsilID;
                ucId = submitComplaintModel.ucID;
                wardId = submitComplaintModel.wardID;


                paramDict.Add("@Tehsil_Id", submitComplaintModel.tehsilID.ToDbObj());
                paramDict.Add("@UnionCouncil_Id", submitComplaintModel.ucID.ToDbObj());
                paramDict.Add("@Ward_Id", submitComplaintModel.wardID.ToDbObj());
                paramDict.Add("@Complaint_Remarks", submitComplaintModel.comment.ToDbObj());
                paramDict.Add("@Agent_Comments", (null as object).ToDbObj());
                paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.Mobile).ToDbObj());



                paramDict.Add("@Agent_Id", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Address", (null as object).ToDbObj());
                paramDict.Add("@Business_Address", (null as object).ToDbObj());

                paramDict.Add("@Complaint_Status_Id", Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
                paramDict.Add("@Created_Date", nowDate.ToDbObj());
                paramDict.Add("@Created_By", (null as object).ToDbObj());
                paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
                paramDict.Add("@Completed_Date", (null as object).ToDbObj());
                //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
                paramDict.Add("@Updated_By", (null as object).ToDbObj());
                paramDict.Add("@Is_Deleted", false);
                paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
                paramDict.Add("@Deleted_By", (null as object).ToDbObj());

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
                paramDict.Add("@p_Person_External_User_Id", submitComplaintModel.userId);
                paramDict.Add("@p_Person_External_Provider", submitComplaintModel.userProvider);
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

                if (!(string.IsNullOrEmpty(submitComplaintModel.lattitude) &&
                    string.IsNullOrEmpty(submitComplaintModel.lattitude)))
                {
                    paramDict.Add("@Latitude", Convert.ToDouble(submitComplaintModel.lattitude));
                    paramDict.Add("@Longitude", Convert.ToDouble(submitComplaintModel.longitude));

                }

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
                assignmentModelList = AssignmentHandler.GetAssignmentModel(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)submitComplaintModel.campaignID, submitComplaintModel.categoryID, submitComplaintModel.subCategoryID), catRetainingHours);
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
                paramDict.Add("@LocationArea", submitComplaintModel.locationArea.ToDbObj());
                DbUnionCouncils dbUc = DbUnionCouncils.GetById(ucId);
                if (dbUc != null)
                {
                    paramDict.Add("@RefField1", dbUc.UcNo);
                }
                //DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Add_Complaints_Crm_Lwmc]", paramDict);
                DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Add_Complaints_Crm]", paramDict);

                //Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
                string[] complaintIdStrArr = dt.Rows[0][1].ToString().Split('-');
                int campaignId = Convert.ToInt32(complaintIdStrArr[0]);
                int complaintId = Convert.ToInt32(complaintIdStrArr[1]);
                string complaintIdStr = dt.Rows[0][1].ToString();

                BlLwmcApiHandler.SaveMobileRequest(submitComplaintModel, Convert.ToInt32(complaintIdStr.Split('-')[1]), apiRequestId);
                if (submitComplaintModel.PicturesList != null)
                {
                    foreach (Picture picture in submitComplaintModel.PicturesList)
                    {
                        FileUploadModel fileUploadModel = new FileUploadModel(picture.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.AttachmentType.File), apiRequestId);
                        FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);

                        //BlApiHandler.StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                    }
                }
                if (!string.IsNullOrEmpty(submitComplaintModel.video))
                {
                    FileUploadModel fileUploadModel = new FileUploadModel(submitComplaintModel.video, Config.AttachmentSaveType.WebServer, "Video", "application/octet-stream", submitComplaintModel.videoFileExtension, campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.AttachmentType.Video), apiRequestId);
                    FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                    //BlApiHandler.StartUploadUtility(submitComplaintModel.video, "Video", "application/octet-stream",
                    //    submitComplaintModel.videoFileExtension, complaintIdStr, Config.FileType.Video, apiRequestId);
                }
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString());
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(),  dt.Rows[0][1].ToString());
                //return "Your Complaint Id = " + dt.Rows[0][1].ToString();
                // Send message

                // string encryptedComplaintNo = UtilityExtensions.Encrypt(dt.Rows[0][1].ToString().Split('-')[1]);
                new Thread(delegate ()
                {
                    DbCampaign dbCampaign = DbCampaign.GetById(submitComplaintModel.campaignID);
                    DbComplaintType dbComplaintType = DbComplaintType.GetById(submitComplaintModel.categoryID);

                    string sms = "Your complaint has been submitted in campaign (" + dbCampaign.Campaign_Name + ")\n\nCategory : " +
                       dbComplaintType.Name + "\nComplaint Id : " + submitComplaintModel.campaignID + "-" + complaintId;
                    TextMessageHandler.SendMessageToPhoneNo(
                        submitComplaintModel.personContactNumber, sms);
                    //PITB.CMS.Handler.Messages.TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    // submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    // submitComplaintModel.categoryID);
                }).Start();

                new Thread(delegate ()
                {
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId));
                }).Start();

                // Send notification to users
                DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
                int hierarchyVal = DbComplaint.GetHierarchyIdValueAgainstComplaint(dbComplaint);
                Config.Hierarchy hierarchyId = (Config.Hierarchy)dbComplaint.Complaint_Computed_Hierarchy_Id;
                int? userHierarchyId = dbComplaint.Complaint_Computed_User_Hierarchy_Id;
                List<DbUsers> listDbUser = DbUsers.GetByCampIdH_IdUserH_Id2(Convert.ToInt32(dbComplaint.Compaign_Id), hierarchyId,
                        hierarchyVal, userHierarchyId, Convert.ToInt32(dbComplaint.Complaint_Category), dbComplaint.UserCategoryId1, dbComplaint.UserCategoryId2);
                DbUserWiseDevices dbUserwiseDevices = null;
                foreach (DbUsers dbUsers in listDbUser)
                {
                    string url = "https://fcm.googleapis.com/fcm/send";//Config.DictUrl["Police-PostAction"];
                    Dictionary<string, string> headersDict = new Dictionary<string, string>();
                    //headersDict.Add("Authorization", "key=AAAAtDjHYG4:APA91bE271IXsJUSKRltY0b9GxPZhrxtE41aEeYPrSwSJwOMMCnC0mKgci_emVCETbiT7ex2ZF6m87VFvyBX5bW2zOZ6x9LssO9-IN5aUvVQw7Umochi4Z3I-gFsZma_cuzWu7yqDYJA");
                    headersDict.Add("Authorization", "key=AAAAgS9ba9A:APA91bEzNf1wSrC05L--l77gkZgQIu238vwGzbQbpnX4y5MHR_M0SKiToiUkOErd5mzxsM3C_yjrdWgQ_RZo2i0F8VuRK6pMTRZf9YIM-HJoXo3Aw8XuT5iItVjz5Wz3BWgi3Mno21eO");
                    //headersDict.Add("SystemUserName", "Police");
                    //headersDict.Add("Password", "p@4o!li#0c@e");
                    //headersDict.Add("Username", "P#0el@9ic$e");
                    LwmcModel.Api.Request.GoogleNotification googleNotificationModel = new LwmcModel.Api.Request.GoogleNotification();
                    dbUserwiseDevices = dbUsers.ListDbUserWiseDevices.Where(n => n.Is_Active == true).FirstOrDefault();
                    if (dbUserwiseDevices != null)
                    {
                        try
                        {
                            new Thread(delegate ()
                            {
                                googleNotificationModel.to = dbUserwiseDevices.Device_Id;
                                //googleNotificationModel.to =
                                //    "fGMmmrRtibQ:APA91bHBbBJdCSO_cT0XuXACEeplssOO_jn9cuoymb-YM4iZzux6ybwq2k8Uamhhesgyx3pvWvFYTtZA_OU0eIpzrbGVlCR0eXL4eZw5sFYLF9nqxk3J15Rn9JZfyfUKka-D-4ZFfHlN";
                                googleNotificationModel.data.detail = "A new complaint has been added";
                                googleNotificationModel.data.title = "A new complaint has been assigned to you.";
                                googleNotificationModel.data.message = "Complaint " + submitComplaintModel.campaignID + "-" + complaintId + " has been assigned in Clean Lahore. Tap to view details.";
                                string response =
                                    APIHelper.HttpClientGetResponse<string, LwmcModel.Api.Request.GoogleNotification>(url,
                                        googleNotificationModel, headersDict);
                                string resp = response;
                            }).Start();
                           
                        }
                        catch (Exception e)
                        {
                           
                        }
                        
                    }
                }
                // End Send Notification to users

                // Commenting send message done
                /*TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    submitComplaintModel.categoryID);

                TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId);
                */
                return new ComplaintSubmitResponse(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString(), dt.Rows[0][1].ToString().Split('-')[1]);
            }
            catch (Exception ex)
            {
                //return Utility.GetStatus(Config.ResponseType.Failure.ToString(), ex.Message);
                //return Config.ResponseType.Failure.ToString()+" Exception = "+ex.Message;
               // System.IO.File.AppendAllText("C:\\Apps\\error.txt", JsonConvert.SerializeObject(ex));

                return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
            }
        }


        public static void SaveMobileRequest(SubmitLWMCComplaintModel submitComplaintModel, int complaintId, Int64 apiRequestId)
        {
            DateTime requestDateTime = submitComplaintModel.complaintDate;
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
        public static ApiComplaintStatusResponseModel GetCurrentStatusesOfComplaints(ApiComplaintStatusesPostedModel postedStatusModel, Config.Language language)
        {
            ApiComplaintStatusResponseModel responseModel = new ApiComplaintStatusResponseModel();
            List<ApiComplaintStatusModel> listComplaintStatusModel = new List<ApiComplaintStatusModel>();
            List<DbComplaint> listComplaintStatus = DbComplaint.GetByComplaintIds(postedStatusModel.ListComplaintId);
            List<int> listChangeStatusLogIds;
            List<DbAttachments> listAttachments = new List<DbAttachments>();
            DbComplaint complaintModel = null;
            foreach (int complaintId in postedStatusModel.ListComplaintId)
            {
                complaintModel = listComplaintStatus.Where(n => n.Id == complaintId).FirstOrDefault();
                if (complaintModel != null)
                {
                    listAttachments.Clear();
                    listChangeStatusLogIds = DbComplaintStatusChangeLog.GetActiveStatusChangeLogIdsListAgainstComplaintId(complaintId);
                    foreach (int changeLogId in listChangeStatusLogIds)
                    {
                        listAttachments.AddRange(DbAttachments.GetByRefAndComplaintId(complaintId, Config.AttachmentReferenceType.ChangeStatus, changeLogId));
                    }

                    listComplaintStatusModel.Add(new ApiComplaintStatusModel()
                    {
                        ComplaintId = complaintId,
                        //StatusId = (complaintModel.Complaint_Computed_Status_Id==(int)Config.ComplaintStatus.PendingFresh || complaintModel.Complaint_Computed_Status_Id==(int)Config.ComplaintStatus.PendingReopened ) ? 1 : 2,
                        StatusId = (int)complaintModel.Complaint_Computed_Status_Id,
                        StatusMessage = complaintModel.Complaint_Computed_Status,
                        ListPicturesUrl = listAttachments.Select(n => new Picture() { picture = n.Source_Url }).ToList()
                    });
                }
            }
            responseModel.ListApiStatusModel = listComplaintStatusModel;
            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
            responseModel.ListApiStatusModel.GetTranslatedList<ApiComplaintStatusModel>("StatusMessage", translationDict, language);
            responseModel.SetSuccess();
            return responseModel;
        }
        public static ApiStatus SubmitComplainantRemarks(SubmitComplaintRemarks submitComplaintRemarks)
        {
            try
            {
                // Start blocking person
                List<string> listMobileNoToBlock = new List<string>()
                    {
                        "35102296695",
                        "03174833701"
                    };


                // End blocking person
                using (DBContextHelperLinq db = new DBContextHelperLinq())
                {
                    DbComplaint dbComplaint = DbComplaint.GetByComplaintId(Convert.ToInt32(submitComplaintRemarks.ComplaintId), db);

                    if (listMobileNoToBlock.Contains(dbComplaint.Person_Contact.Trim()))
                    {
                        db.Dispose();
                        return new ApiStatus(Config.ResponseType.Failure.ToString(), Config.ResponseType.Failure.ToString());
                    }

                    db.DbComplaints.Attach(dbComplaint);



                    dbComplaint.Complainant_Remark_Id = (int)submitComplaintRemarks.RemarkType;
                    db.Entry(dbComplaint).Property(x => x.Complainant_Remark_Id).IsModified = true;

                    dbComplaint.Complainant_Remark_Str = submitComplaintRemarks.RemarkStr;
                    db.Entry(dbComplaint).Property(x => x.Complainant_Remark_Str).IsModified = true;

                    db.SaveChanges();
                }



                return new ApiStatus(Config.ResponseType.Success.ToString(), Config.ResponseType.Success.ToString());
            }
            catch (Exception ex)
            {
                return new ApiStatus(Config.ResponseType.Failure.ToString(), Config.ResponseType.Failure.ToString());
                throw;
            }
        }
        /// <summary>
        /// This method returns all complaints when cnic,cell,externalUserId and externalProvider provided null and 
        /// Also same method will return user created complaints on basis of user parameters 
        /// </summary>
        /// <param name="cnic">CNIC of Mobile User</param>
        /// <param name="cell">Cell number of Mobile User</param>
        /// <param name="appId">Application ID of mobile</param>
        /// <param name="language">Language ID 1 for English or 2 for Urdu</param>
        /// <param name="campaignId">Campaign ID of your campaign </param>
        /// <param name="from">Page offset from where you want to select rows</param>
        /// <param name="to">Page offset till where you want to end rows</param>
        /// <param name="filterType">Most Recent / Popular based on votes </param>
        /// <param name="lat">Current position of user Latitude</param>
        /// <param name="lng">Current position of user Longitude</param>
        /// <param name="externalUserId">External User ID i.e Facebook ID</param>
        /// <param name="externalProvider">External User provider i.e "Facebook"</param>
        /// <returns>GetComplainantComplaintModel</returns>
        public static GetComplainantComplaintModel GetComplainantComplaints(SubmitGetComplaintModel model)
        {
            List<int> statusIds = new List<int>();
            if (model.ComplaintStatus > 0)
            {
                switch (model.ComplaintStatus)
                {
                    case (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable:
                        statusIds.Add((int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable);
                        statusIds.Add((int)Config.ComplaintStatus.ResolvedVerified);
                        break;
                    case (int)Config.ComplaintStatus.PendingFresh:
                        statusIds.Add((int)Config.ComplaintStatus.PendingFresh);
                        statusIds.Add((int)Config.ComplaintStatus.PendingReopened);

                        break;

                    default:
                        statusIds.Add(model.ComplaintStatus);

                        break;
                }

            }
            DateTime startDate, endDate;
            DateTime.TryParseExact(model.StartDate, "dd/MM/yyyy", null, DateTimeStyles.None, out startDate);
            DateTime.TryParseExact(model.EndDate, "dd/MM/yyyy", null, DateTimeStyles.None, out endDate);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));

            /* ----------------------------------
                Initialize GetComplainantComplaintModel
            ------------------------------------*/
            GetComplainantComplaintModel complaintsModel = new GetComplainantComplaintModel
            {
                ListComplaint = new List<ComplainantComplaintModel>(),
                Status = Config.ResponseType.Success.ToString(),
                Message = Config.ResponseType.Success.ToString()

            };

            /* ----------------------------------------------------------------
             * 
             *  Load complaints 
             *  ----------------
             *  If user provides its identity i.e cnic or user Id 
             *      then load user created complaints
             *  else load all complaints 
             *      Calculating distance of all complaints in database
             *      Loading nearest complaints to user
             *      
             * ----------------------------------------------------------------*/


            #region Get Complaints

            List<DbComplaint> listDbComplaints = null;
            dynamic tempComplaintModel = null;
            if (model.CnicOrSocialPresent == Config.CnicOrSocialPresent.None)
            {

                if (model.Filter == Config.FilterTypeApi.Nearest)
                {
                    Dictionary<int, LocationCoordinate> complaintDictionary =
                    DbComplaint.GetAllComplaintsOfCampaignWithCoordinates(model.CampaignId, startDate, endDate);
                    Dictionary<int, double> complaintIdWithDistance = new Dictionary<int, double>();

                    foreach (KeyValuePair<int, LocationCoordinate> complaintWithlocationCoordinate in complaintDictionary)
                    {
                        complaintIdWithDistance.Add(
                            complaintWithlocationCoordinate.Key,
                            GetDistanceBetweenTwoPoints(
                                model.Latitude,
                                model.Longitude,
                                complaintWithlocationCoordinate.Value.Lt,
                                complaintWithlocationCoordinate.Value.Lg));
                    }

                    //List<LocationCoordinate> listOfRadiusPoints = LocationHandler.GetRadiusPoints(lat, lng, 0.5);

                    /*
                     * ***************************************************************
                     *  Order the complaints ascending with shortest distance first
                     * ***************************************************************
                     */

                    List<int> orderedComplaints = complaintIdWithDistance
                        //  .Where(m => m.Value <= Config.RadiusRangeInKiloMeters)
                        .OrderBy(m => m.Value) //ordering on calculated distance
                        .Select(m => m.Key)
                        .Skip(model.From)
                        .Take(model.To)
                        .ToList();


                    listDbComplaints = DbComplaint.GetListByComplaintIds(orderedComplaints, statusIds,
                        (byte)model.Filter, startDate, endDate);




                    //IMPORTANT !!!
                    //Sorting complaints list as ordered complaints list
                    listDbComplaints = listDbComplaints.OrderBy(d => orderedComplaints.IndexOf(d.Id)).ToList();

                    //Setting count
                    complaintsModel.TotalComplaints = complaintIdWithDistance.Count;
                }
                else
                {
                    tempComplaintModel = DbComplaint.GetListByCampaignIdPaging(model.CampaignId, model.From, model.To, statusIds,
                      (byte)model.Filter, startDate, endDate);

                    listDbComplaints = tempComplaintModel.listDbComplaint;
                    complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;
                    //complaintsModel.TotalComplaints = DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, startDate, endDate);
                }

            }
            else
            {
                int personId = -1;
                switch (model.CnicOrSocialPresent)
                {
                    case Config.CnicOrSocialPresent.OnlyCnic:
                    case Config.CnicOrSocialPresent.CnicAndSocial:

                        tempComplaintModel = DbComplaint.GetListByPersonCnicPaging(model.Cnic, model.CampaignId,
                            model.From, model.To, statusIds, (byte)model.Filter, startDate, endDate);

                        listDbComplaints = tempComplaintModel.listDbComplaint;
                        complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;

                        //complaintsModel.TotalComplaints = string.IsNullOrEmpty(model.Cnic)
                        //   ? DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, startDate, endDate)
                        //   : DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, model.Cnic, model.Cell, startDate, endDate);

                        break;
                    case Config.CnicOrSocialPresent.OnlySocial:
                        personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider);
                        tempComplaintModel = DbComplaint.GetListByPersonUserIdAndProviderPaging(model.UserId, model.UserProvider, model.CampaignId, model.From, model.To, statusIds, (byte)model.Filter);
                        listDbComplaints = tempComplaintModel.listDbComplaint;
                        complaintsModel.TotalComplaints = tempComplaintModel.TotalComplaints;
                        //complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
                        break;

                        //case Config.CnicOrSocialPresent.CnicAndSocial:
                        //    personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider, model.Cnic);
                        //    listDbComplaints = DbComplaint.GetListByPersonId(personId, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
                        //    complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
                        //    break;

                }
            }
            //if (string.IsNullOrEmpty(model.Cnic) && string.IsNullOrEmpty(model.Cell) & string.IsNullOrEmpty(model.UserId) && string.IsNullOrEmpty(model.UserProvider))
            //{
            //    Dictionary<int, LocationCoordinate> complaintDictionary = DbComplaint.GetAllComplaintsOfCampaignWithCoordinates(model.CampaignId);
            //    Dictionary<int, double> complaintIdWithDistance = new Dictionary<int, double>();

            //    foreach (KeyValuePair<int, LocationCoordinate> complaintWithlocationCoordinate in complaintDictionary)
            //    {
            //        complaintIdWithDistance.Add(
            //            complaintWithlocationCoordinate.Key,
            //            GetDistanceBetweenTwoPoints(
            //                                        model.Latitude,
            //                                        model.Longitude,
            //                                        complaintWithlocationCoordinate.Value.Lt,
            //                                        complaintWithlocationCoordinate.Value.Lg));
            //    }

            //    //List<LocationCoordinate> listOfRadiusPoints = LocationHandler.GetRadiusPoints(lat, lng, 0.5);

            //    /*
            //     * ***************************************************************
            //     *  Order the complaints ascending with shortest distance first
            //     * ***************************************************************
            //     */

            //    List<int> orderedComplaints = complaintIdWithDistance
            //        //  .Where(m => m.Value <= Config.RadiusRangeInKiloMeters)
            //                                .OrderBy(m => m.Value) //ordering on calculated distance
            //                                .Select(m => m.Key)
            //                                .Skip(model.From)
            //                                .Take(model.To)
            //                                .ToList();


            //    listDbComplaints = DbComplaint.GetListByComplaintIds(orderedComplaints, model.ComplaintStatus, (byte)model.Filter);
            //    complaintsModel.TotalComplaints = complaintIdWithDistance.Count;
            //}
            //else
            //{



            //    if (!string.IsNullOrEmpty(model.UserId) && !string.IsNullOrEmpty(model.UserProvider) &&
            //        !string.IsNullOrEmpty(model.Cnic))
            //    {
            //        DbPersonInformation dbPerson = DbPersonInformation.GetByCnic(model.Cnic, model.UserId, model.UserProvider);
            //        listDbComplaints = DbComplaint.GetListByPersonId(dbPerson.Person_id, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);

            //    }

            //    else if (string.IsNullOrEmpty(model.UserId) && string.IsNullOrEmpty(model.UserProvider))
            //    {
            //        listDbComplaints = DbComplaint.GetListByPersonCnicPaging(model.Cnic, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
            //        complaintsModel.TotalComplaints = string.IsNullOrEmpty(model.Cnic)
            //            ? DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId)
            //            : DbComplaint.GetAllComplaintsCountInCampaign(model.CampaignId, model.Cnic, model.Cell);
            //    }
            //    else
            //    {

            //        int personId = DbPersonInformation.GetPersonIdByUserId(model.UserId, model.UserProvider);
            //        listDbComplaints = DbComplaint.GetListByPersonId(personId, model.CampaignId, model.From, model.To, model.ComplaintStatus, (byte)model.Filter);
            //        complaintsModel.TotalComplaints = DbComplaint.GetCountOfComplaintsByPersonId(personId);
            //    }
            //}

            #endregion




            List<Picture> listStatusPicturesUrl = new List<Picture>();
            List<Picture> listComplaintPicturesUrl = new List<Picture>();
            List<Video> listComplaintVideoUrl = new List<Video>();
            List<int> listChangeStatusLogIds = null;
            List<DbAttachments> listDbAttachment = new List<DbAttachments>();

            // Get status History
            List<DbComplaintStatusChangeLog> listStatusChangeLogs = DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintIds(Utility.GetNullableIntList(listDbComplaints.Select(n => n.Id).ToList()));

            // End Get status History

            List<DbTranslationMapping> translationList = DbTranslationMapping.GetMappedTranslation(model.CampaignId);

            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetMappedTranslation(model.CampaignId));
            foreach (DbComplaint dbComplaint in listDbComplaints)
            {
                try
                {
                    #region Attachements

                    if (!string.IsNullOrEmpty(dbComplaint.Person_External_Provider))
                    {
                        dbComplaint.Person_External_Provider = dbComplaint.Person_External_Provider.Trim();
                    }


                    listChangeStatusLogIds = DbComplaintStatusChangeLog.LwmcGetStatusChangeLogIdsListAgainstComplaintId(dbComplaint.Id, (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable);
                    foreach (int changeLogId in listChangeStatusLogIds) //  get all attachment against change log ids
                    {
                        listDbAttachment.AddRange(DbAttachments.GetByRefAndComplaintId(dbComplaint.Id, Config.AttachmentReferenceType.ChangeStatus, changeLogId));
                    }

                    if (listDbAttachment != null)
                    {
                        listStatusPicturesUrl = listDbAttachment.Select(n => new Picture()
                        {
                            picture = n.Source_Url
                        }).ToList();
                    }


                    listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.File);

                    if (listDbAttachment != null)
                    {
                        listComplaintPicturesUrl = listDbAttachment.Select(n => new Picture()
                        {
                            picture = n.Source_Url
                        }).ToList();
                    }

                    listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.AttachmentType.Video);

                    if (listDbAttachment != null)
                    {
                        listComplaintVideoUrl = listDbAttachment.Select(n => new Video()
                        {
                            video = n.Source_Url
                        }).ToList();
                    }
                    #endregion


                    #region Votes
                    List<DbComplaintVote> listComplaintVotes = DbComplaintVote.GetVotesByComplaintId(Convert.ToInt32(dbComplaint.Id));
                    Config.UserVote userVote = GetUserVoteFromAllVotes(listComplaintVotes, model.Cnic, model.Cell, model.UserId, model.UserProvider);
                    #endregion



                    #region Complaint Model Stuff


                    ComplainantComplaintModel complaintModel = new ComplainantComplaintModel();

                    complaintModel.provinceName = dbComplaint.Province_Name;
                    complaintModel.districtName = dbComplaint.District_Name;
                    complaintModel.tehsilName = dbComplaint.Tehsil_Name;
                    complaintModel.ucName = dbComplaint.UnionCouncil_Name;
                    complaintModel.LocationArea = dbComplaint.LocationArea;

                    complaintModel.ExternalUserId = dbComplaint.Person_External_User_Id;
                    complaintModel.ExternalProvider = (string.IsNullOrEmpty(dbComplaint.Person_External_Provider))
                                                ? Config.ExternalProvider.None :
                                                (Config.ExternalProvider)Enum.Parse(typeof(Config.ExternalProvider), dbComplaint.Person_External_Provider, true);


                    complaintModel.campaignName = DbCampaign.GetById(Convert.ToInt32(dbComplaint.Compaign_Id)).Campaign_Name;
                    complaintModel.complaintID = dbComplaint.Id;
                    complaintModel.campaignID = Convert.ToInt32(dbComplaint.Compaign_Id);
                    complaintModel.categoryID = Convert.ToInt32(dbComplaint.Complaint_Category);

                    complaintModel.cnic = dbComplaint.Person_Cnic;
                    complaintModel.comment = dbComplaint.Complaint_Remarks;
                    complaintModel.date = Convert.ToDateTime(dbComplaint.Created_Date).ToString("dd/MM/yyyy hh:mm tt");
                    complaintModel.departmentId = Convert.ToInt32(dbComplaint.Department_Id);
                    complaintModel.districtID = Convert.ToInt32(dbComplaint.District_Id);

                    complaintModel.ListPicturesComplaintsUrl = listComplaintPicturesUrl;
                    complaintModel.ListPicturesStatusUrl = listStatusPicturesUrl;
                    complaintModel.ListVideoComplaintsUrl = listComplaintVideoUrl;
                    complaintModel.personContactNumber = dbComplaint.Person_Contact;
                    complaintModel.personName = dbComplaint.Person_Name;
                    complaintModel.provinceID = Convert.ToInt32(dbComplaint.Province_Id);
                    complaintModel.categoryName = dbComplaint.Complaint_Category_Name;
                    complaintModel.categoryNameUrdu = translationList.Where(w => w.Type_Id == complaintModel.categoryID)?.FirstOrDefault()?.UrduMappedString;
                    complaintModel.subCategoryID = Convert.ToInt32(dbComplaint.Complaint_SubCategory);
                    complaintModel.subCategoryName = dbComplaint.Complaint_SubCategory_Name;
                    complaintModel.subCategoryNameUrdu = translationList.Where(w => w.Type_Id == complaintModel.categoryID && w.SubType_Id == complaintModel.subCategoryID)?.FirstOrDefault()?.UrduMappedString; ;

                    complaintModel.tehsilID = Convert.ToInt32(dbComplaint.Tehsil_Id);

                    complaintModel.ucID = Convert.ToInt32(dbComplaint.UnionCouncil_Id);
                    complaintModel.wardID = Convert.ToInt32(dbComplaint.Ward_Id);

                    complaintModel.statusStr = dbComplaint.Complaint_Computed_Status;
                    complaintModel.Latitude = dbComplaint.Latitude;
                    complaintModel.Longitude = dbComplaint.Longitude;
                    complaintModel.UpVotes = Convert.ToInt32(dbComplaint.Vote_Up_Count);
                    complaintModel.DownVotes = Convert.ToInt32(dbComplaint.Vote_Down_Count);
                    complaintModel.UserVote = userVote;
                    complaintModel.ComplainantRemarks = dbComplaint.Complainant_Remark_Str;
                    complaintModel.StakeHolderRemarks = dbComplaint.StatusChangedComments;
                    complaintModel.StakeHolderRemarksDate = (dbComplaint.StatusChangedDate_Time == null) ? string.Empty : dbComplaint.StatusChangedDate_Time.ToString();
                    complaintModel.ComplainantSatisfactionStatus = (dbComplaint.Complainant_Remark_Id == null) ? (int)Config.ComplainantRemarkType.None : (int)((Config.ComplainantRemarkType)Convert.ToInt32(dbComplaint.Complainant_Remark_Id));
                    complaintModel.DistanceFromYourLocation = (dbComplaint.Latitude.HasValue) ? GetDistanceBetweenTwoPoints(model.Latitude, model.Longitude, dbComplaint.Latitude.Value, dbComplaint.Longitude.Value, 'K') : -1;
                    complaintModel.ComputedRemainingTotalTime = dbComplaint.Computed_Remaining_Total_Time;


                    #endregion


                    #region Status Id

                    switch (dbComplaint.Complaint_Computed_Status_Id)
                    {
                        case (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable:
                        case (int)Config.ComplaintStatus.ResolvedVerified:

                            complaintModel.statusId = (int)Config.ComplaintStatus.ResolvedUnverifiedEscalatable;

                            break;
                        case (int)Config.ComplaintStatus.PendingFresh:
                        case (int)Config.ComplaintStatus.PendingReopened:

                            complaintModel.statusId = (int)Config.ComplaintStatus.PendingFresh;

                            break;
                        default:
                            complaintModel.statusId = Convert.ToInt32(dbComplaint.Complaint_Computed_Status_Id);

                            break;
                    }


                    #endregion


                    #region OldCommentedStuff
                    //complaintModel.categoryName = DbComplaintType.GetById(complaintModel.categoryID);



                    //(complaintModel.categoryID != 0) ? DbComplaintType.GetById(complaintModel.categoryID).Name : "";
                    //(complaintModel.subCategoryID != 0) ? DbComplaintSubType.GetById(complaintModel.subCategoryID).Name : "";





                    //(complaintModel.provinceID != 0) ? DbProvince.GetById(complaintModel.provinceID).Province_Name : "";
                    //(complaintModel.districtID != 0) ? DbDistrict.GetById(complaintModel.districtID).District_Name : "";
                    //(complaintModel.tehsilID != 0) ? DbTehsil.GetById(complaintModel.tehsilID).Tehsil_Name : "";
                    #endregion


                    #region Translations
                    complaintModel.GetTranslatedModel<ComplainantComplaintModel>("campaignName", translationDict, (Config.Language)model.LanguageId);

                    complaintModel.GetTranslatedModel<ComplainantComplaintModel>("categoryName", translationDict, (Config.Language)model.LanguageId);
                    complaintModel.GetTranslatedModel<ComplainantComplaintModel>("subCategoryName", translationDict, (Config.Language)model.LanguageId);

                    complaintModel.GetTranslatedModel<ComplainantComplaintModel>("statusStr", translationDict, (Config.Language)model.LanguageId);
                    #endregion

                    // Add Resolve History
                    // Populate History
                    DbComplaintStatusChangeLog dbStatusLog = listStatusChangeLogs.Where(n => n.Complaint_Id == dbComplaint.Id).OrderByDescending(n => n.StatusChangeDateTime).FirstOrDefault();
                    //foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintStatusChangeLogs)
                    if (dbStatusLog != null)
                    {
                        //complaintModel.ListLogHistory = new List<LogHistory>();
                        LogHistory logHistory = new LogHistory();
                        logHistory.ComplaintId = Convert.ToInt32(dbStatusLog.Complaint_Id);

                        logHistory.StatusChangeComments = dbStatusLog.Comments;
                        logHistory.StatusChangeDateTime = dbStatusLog.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                        logHistory.StatusId = Convert.ToByte(dbStatusLog.StatusId);
                        logHistory.StatusChangedByUserName = dbStatusLog.ChangedBy.Name;
                        logHistory.StatusChangedByUserContact = dbStatusLog.ChangedBy.Phone;
                        logHistory.StatusChangedByUserHierarchy = ((Config.LwmcResolverHirarchyId)Convert.ToByte(dbStatusLog.ChangedBy.User_Hierarchy_Id)).ToString();
                        logHistory.Status = ((Config.ComplaintStatus)logHistory.StatusId).GetDisplayName();
                        logHistory.ListAttachments = dbStatusLog.ListDbAttachments;
                        //complaintModel.ListLogHistory.Add(logHistory);
                        complaintModel.LastStatus = logHistory;
                    }
                    //List<DbComplaintStatusChangeLog> listComplaintStatusChangeLogs = listStatusChangeLogs.Where(n => n.Complaint_Id == dbComplaint.Id).OrderByDescending(n=>n.StatusChangeDateTime).ToList();
                    //foreach (DbComplaintStatusChangeLog dbStatusLog in listComplaintStatusChangeLogs)
                    //{
                    //    complaintModel.ListLogHistory = new List<LogHistory>();
                    //    LogHistory logHistory = new LogHistory();
                    //    logHistory.ComplaintId = Convert.ToInt32(dbStatusLog.Complaint_Id);

                    //    logHistory.StatusChangeComments = dbStatusLog.Comments;
                    //    logHistory.StatusChangeDateTime = dbStatusLog.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                    //    logHistory.StatusId = Convert.ToByte(dbStatusLog.StatusId);
                    //    logHistory.StatusChangedByUserName = dbStatusLog.ChangedBy.Name;
                    //    logHistory.StatusChangedByUserContact = dbStatusLog.ChangedBy.Phone;
                    //    logHistory.StatusChangedByUserHierarchy = ((Config.LwmcResolverHirarchyId)Convert.ToByte(dbStatusLog.ChangedBy.User_Hierarchy_Id)).ToString();
                    //    logHistory.Status = ((Config.ComplaintStatus)logHistory.StatusId).GetDisplayName();
                    //    logHistory.ListAttachments = dbStatusLog.ListDbAttachments;
                    //    complaintModel.ListLogHistory.Add(logHistory);

                    //}
                    // End Populate History
                    // End Add Resolve History
                    complaintsModel.ListComplaint.Add(complaintModel);
                }
                catch (Exception ex)
                {

                }
            }
            complaintsModel.SetSuccess();
            return complaintsModel;
        }

        public static LwmcResponse.ComplaintantMessages GetComplainantMessages()
        {
            LwmcResponse.ComplaintantMessages complaintMsgResp = new LwmcResponse.ComplaintantMessages();
            complaintMsgResp.ListMessages = DbCampaignMessages.GetByCampaignId((int)Config.Campaign.FixItLwmc).Select(n => new LwmcResponse.ComplaintantMessages.MessageModel { MessageStr = n.Message_Body, OrderId = Convert.ToInt32(n.Order_Id) }).ToList();
            complaintMsgResp.SetSuccess();
            return complaintMsgResp;
        }

        public static SyncModel SubmitComplainantLogin(SubmitComplainantLogin submitComplainantLogin, int appId, Config.PlatformID platformId, Config.Language language, int dbVersionId)
        {
            //string autoGeneratedCnic = null;
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
             * 
             * 
             *  WHEN USER SUBMITTED INCOMPLETE DATA THEN THROW ERROR
             *  
             * 
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

            if (!submitComplainantLogin.ModelIsValid())
            {
                return new SyncModel()
                {
                    Message = "Please provide valid/complete parameters, see documentation for details",
                    Status = Config.ResponseType.Failure.ToString(),
                };
            }



            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = null;
            /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
             - 
             - 
             -  If user using external login then search based on its 
             -  UserId and UserProvider value
             -  
             - 
             - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
            if (submitComplainantLogin.IsExternalLogin)
            {
                DbPersonInformation dbPersonInfoTemp;
                //There are two cases

                /**
                 * 1- Where User logged in with CNIC and then Logged in with Facebook
                 * 2- User Just logged in with Facebook
                 * 
                 * 
                 */
                dbPersonInfoTemp = string.IsNullOrEmpty(submitComplainantLogin.Cnic) ? DbPersonInformation.GetByExternalUserId(submitComplainantLogin.UserId, submitComplainantLogin.UserProvider) : DbPersonInformation.GetByCnic(submitComplainantLogin.Cnic);
                //dbPersonInfoTemp = DbPersonInformation.GetByCnic(submitComplainantLogin.Cnic, submitComplainantLogin.UserId, submitComplainantLogin.UserProvider, db);


                if (dbPersonInfoTemp == null)
                {

                    DbPersonInformation newPersonInformation = new DbPersonInformation()
                    {
                        Cnic_No = Config.FacebookPrefixCnic,
                        External_User_ID = submitComplainantLogin.UserId,
                        External_Provider = submitComplainantLogin.UserProvider,
                        Imei_No = submitComplainantLogin.ImeiNo,
                        Created_Date = DateTime.Now,
                        Person_Name = submitComplainantLogin.FullName,
                        Email = submitComplainantLogin.Email
                    };
                    SetLocationAttributesInDbPersonObject(newPersonInformation, new LocationCoordinate(submitComplainantLogin.Latitude, submitComplainantLogin.Longitude));
                    DbPersonInformation.InsertDbPersonInformation(newPersonInformation);


                }
                else
                {


                    if (string.IsNullOrEmpty(submitComplainantLogin.Cnic))
                    {
                        // User has logged in with Facebook only 
                        dbPersonInfoTemp.External_User_ID = submitComplainantLogin.UserId;
                        dbPersonInfoTemp.External_Provider = submitComplainantLogin.UserProvider;
                    }
                    else
                    {
                        dbPersonInfoTemp.Cnic_No = submitComplainantLogin.Cnic;
                    }

                    dbPersonInfoTemp.Mobile_No = submitComplainantLogin.MobileNo;
                    dbPersonInfoTemp.Person_Name = submitComplainantLogin.FullName;


                    db.DbPersonalInfo.Attach(dbPersonInfoTemp);

                    db.Entry(dbPersonInfoTemp).Property(x => x.Cnic_No).IsModified = true;
                    db.Entry(dbPersonInfoTemp).Property(x => x.External_User_ID).IsModified = true;
                    db.Entry(dbPersonInfoTemp).Property(x => x.External_Provider).IsModified = true;
                    db.Entry(dbPersonInfoTemp).Property(x => x.Mobile_No).IsModified = true;
                    db.Entry(dbPersonInfoTemp).Property(x => x.Person_Name).IsModified = true;
                    db.SaveChanges();
                }


                dbPersonalInfo = dbPersonInfoTemp;




                //dbPersonalInfo = DbPersonInformation.GetByExternalUserId(submitComplainantLogin.UserId,submitComplainantLogin.UserProvider, db);
            }
            else
            {
                //dbPersonalInfo = DbPersonInformation.GetByCnic(submitComplainantLogin.Cnic);
                dbPersonalInfo = DbPersonInformation.GetListByMobileNo(submitComplainantLogin.MobileNo).FirstOrDefault();

                if (dbPersonalInfo == null) // if user does not exist
                {
                    decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
                    string cnicStr = cnic.ToString();

                    //cnicStr = CMS.Utility.PaddLeft(cnicStr, CMS.Config.CnicLength, '0');
                    submitComplainantLogin.Cnic = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
                    //autoGeneratedCnic = submitComplainantLogin.Cnic;

                    //vmPersonalInfo.Cnic_No = cnicStr;
                    //dbPersonalInfo.Cnic_No = 

                    DbPersonInformation newPersonInformation = new DbPersonInformation()
                    {
                        Person_Name = submitComplainantLogin.PersonName,
                        Cnic_No = submitComplainantLogin.Cnic,
                        Mobile_No = submitComplainantLogin.MobileNo,
                        Imei_No = submitComplainantLogin.ImeiNo,
                        Created_Date = DateTime.Now,

                    };



                    SetLocationAttributesInDbPersonObject(newPersonInformation, new LocationCoordinate(submitComplainantLogin.Latitude, submitComplainantLogin.Longitude));
                    DbPersonInformation.InsertDbPersonInformation(newPersonInformation);
                    dbPersonalInfo = newPersonInformation;
                }
                else
                {
                    //When user is already registered than update its value in case of internal login
                    //autoGeneratedCnic = dbPersonalInfo.Cnic_No;
                    submitComplainantLogin.Cnic = dbPersonalInfo.Cnic_No;

                    dbPersonalInfo.Person_Name = submitComplainantLogin.PersonName;
                    dbPersonalInfo.Mobile_No = submitComplainantLogin.MobileNo;
                    db.DbPersonalInfo.Attach(dbPersonalInfo);
                    db.Entry(dbPersonalInfo).Property(x => x.Mobile_No).IsModified = true;
                    db.Entry(dbPersonalInfo).Property(x => x.Person_Name).IsModified = true;
                    db.SaveChanges();
                }

            }




            SyncModel syncModel = null;

            int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID)appId);

            string status, message;
            #region Old Code


            /*
            // For IOS platform
            if (platformId == Config.PlatformID.IOS)
            {
                string cnic = "3333335555555";
                string mobileNo = "03332222225";

                dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(cnic, mobileNo, db);
                dbPersonalInfo.Imei_No = submitComplainantLogin.ImeiNo;
                db.DbPersonalInfo.Attach(dbPersonalInfo);
                db.Entry(dbPersonalInfo).Property(x => x.Imei_No).IsModified = true;
                db.SaveChanges();
                status = Config.ResponseType.Success.ToString();
                message = "User has successfully logged-in and IMEI registered successfully.";

                syncModel = dbVersionId < currDbVersion ? BlApiHandler.GetSyncModelAgaistCnicAndAppId(cnic, appId, language) : new SyncModel();
                // TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);

                syncModel.Status = status;
                syncModel.Message = message;
                syncModel.DbVersionId = currDbVersion;

                return syncModel;
            }
            // End For IOS platform
            */



            // When user information is not available it means he is appearing for first time

            //if (dbPersonalInfo.Imei_No == null) // if imei not registered then register it
            //{
            //    // DbUsers dbUser = DbUsers.GetByUserName(submitStakeHolderLogin.Username, db);
            //    dbPersonalInfo.Imei_No = submitComplainantLogin.ImeiNo;
            //    db.DbPersonalInfo.Attach(dbPersonalInfo);
            //    db.Entry(dbPersonalInfo).Property(x => x.Imei_No).IsModified = true;
            //    db.SaveChanges();
            //    status = Config.ResponseType.Success.ToString();
            //    message = "User has successfully logged-in and IMEI registered successfully.";
            //    if (dbVersionId < currDbVersion)
            //    {
            //        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(submitComplainantLogin.Cnic, appId,
            //            language);
            //    }
            //    else
            //    {
            //        syncModel = new SyncModel();
            //    }
            //    //  TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
            //}



            // If user credentials is correct and imei not registered then register imei
            //  if (dbPersonalInfo.Imei_No != submitComplainantLogin.ImeiNo)

            // {
            //  dbPersonalInfo.Imei_No = submitComplainantLogin.ImeiNo;
            //  db.DbPersonalInfo.Attach(dbPersonalInfo);
            //   db.Entry(dbPersonalInfo).Property(x => x.Imei_No).IsModified = true;
            //   db.SaveChanges();

            #endregion
            status = Config.ResponseType.Success.ToString();
            message = "User has successfully logged-in and IMEI registered successfully.";
            if (dbVersionId < currDbVersion)
            {
                syncModel = BlLwmcApiHandler.GetSyncModelAgaistCnicAndAppId(
                    submitComplainantLogin.IsExternalLogin,
                    submitComplainantLogin.Cnic,
                    submitComplainantLogin.UserId,
                    submitComplainantLogin.UserProvider,
                    appId,
                    language);
            }
            else
            {
                syncModel = new SyncModel();
            }

            // }
            //  else
            {
                //     status = Config.ResponseType.Success.ToString();
                //      message = "User has successfully logged-in and IMEI registered successfully.";
            }
            if (syncModel == null)
            {
                syncModel = new SyncModel();
            }

            syncModel.Status = status;
            syncModel.Message = message;
            syncModel.DbVersionId = currDbVersion;
            syncModel.Cnic = submitComplainantLogin.Cnic;

            return syncModel;
        }

        public static SubmitVoteResponse SubmitVoteForComplaint(IncomingSocialSubmitModel submitModel)
        {

            SubmitVoteResponse status = new SubmitVoteResponse() { Status = Config.ResponseType.Success.ToString() };
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {

                DbComplaintVote userVote = db.DbComplaintVotes.FirstOrDefault(m => m.Complaint_ID == submitModel.ComplaintId && m.Voted_By == submitModel.UserId);

                //int userVotes = db.DbComplaintVotes.Count(m => m.Complaint_ID == submitModel.ComplaintId && m.Voted_By == submitModel.UserId);
                if (userVote == null)
                {
                    DbComplaintVote vote = new DbComplaintVote
                    {
                        Complaint_ID = submitModel.ComplaintId,
                        Is_Positive_Vote = (byte)submitModel.UserVote,
                        Vote_DateTime = DateTime.Now,
                        Voted_By = submitModel.UserId,
                        Voted_By_Provider = submitModel.ExternalProvider.ToString(),
                        First_Name = submitModel.FirstName,
                        Last_Name = submitModel.LastName,
                        Cnic = submitModel.CnicNo,
                        Cell_Number = submitModel.ContactNo


                    };

                    db.DbComplaintVotes.Add(vote);
                    db.SaveChanges();
                    status.Message = ApiStatusMessages.VoteSubmitted;
                }
                else
                {
                    userVote.Is_Positive_Vote = (byte)submitModel.UserVote;
                    userVote.Updated_Date_Time = DateTime.Now;
                    db.Entry(userVote).State = EntityState.Modified;
                    db.SaveChanges();
                    status.Message = submitModel.UserVote == Config.UserVote.NoVote ? ApiStatusMessages.NoVote : ApiStatusMessages.VoteSubmitted;
                }

                status.TotalUpVotes = db.DbComplaintVotes.AsNoTracking().Count(m => m.Complaint_ID == submitModel.ComplaintId && m.Is_Positive_Vote == (byte)Config.UserVote.UpVote);
                status.TotalDownVotes = db.DbComplaintVotes.AsNoTracking().Count(m => m.Complaint_ID == submitModel.ComplaintId && m.Is_Positive_Vote == (byte)Config.UserVote.DownVote);
                UpdateVoteCountOfComplaint(submitModel.ComplaintId, status.TotalUpVotes, status.TotalDownVotes);
                status.UserVote = submitModel.UserVote;
            }


            return status;
        }
        public static ApiStatus SubmitSharePost(IncomingSocialSubmitModel submitModel)
        {

            ApiStatus status = new ApiStatus() { Status = Config.ResponseType.Success.ToString() };
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {

                DbSocialSharing sharing = db.DbSocialSharings.FirstOrDefault(m => m.Complaint_Id == submitModel.ComplaintId && m.Post_Id == submitModel.PostId && m.User_Id == submitModel.UserId);

                if (sharing == null)
                {
                    DbSocialSharing complaintShare = new DbSocialSharing()
                    {
                        Complaint_Id = submitModel.ComplaintId,
                        Provider = submitModel.UserProvider,
                        First_Name = submitModel.FirstName,
                        Last_Name = submitModel.LastName,
                        Post_Id = submitModel.PostId,
                        User_Id = submitModel.UserId,
                        Created_DateTime = DateTime.Now

                    };
                    db.DbSocialSharings.Add(complaintShare);
                    db.SaveChanges();
                    status.Message = ApiStatusMessages.PostShared;

                }
                else
                {
                    status.Message = ApiStatusMessages.PostAlreadyShared;
                }
            }
            return status;
        }

        public static void UpdateVoteCountOfComplaint(long complaintId, int upVoteCount, int downVoteCount)
        {
            Dictionary<string, object> paramz = new Dictionary<string, object>()
            {
                {"@Complaint_ID",complaintId},
                {"@VoteUpCount",upVoteCount},
                {"@VoteDownCount",downVoteCount}
            };

            DBHelper.CrudOperation("[PITB].[Update_Complaint_Vote_Counts]", paramz);
        }

        public static double GetDistanceBetweenTwoPoints(double lat1, double lon1, double lat2, double lon2, char unit = 'T')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {

                case 'T': //Meters -- > default
                    return dist * 1609.344;
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        private static void SetLocationAttributesInDbPersonObject(DbPersonInformation dbPersonInformation,
            LocationCoordinate complainantLocation)
        {
            LocationMapping mapping;

            if (LocationHandler.IsLocationExistInPolygon(complainantLocation, Config.Hierarchy.UnionCouncil,
                   out mapping))
            {

                List<LocationMapping> allDistrictDivisionTehsilTownsList =
                    LocationHandler.GetAllAboveHirerchyByHirerchyIdAndType(mapping);
                dbPersonInformation.Province_Id =
                    allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Province)
                        .HirerchyTypeID;
                dbPersonInformation.Division_Id =
                   allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Division)
                       .HirerchyTypeID;
                dbPersonInformation.District_Id =
                   allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.District)
                       .HirerchyTypeID;
                dbPersonInformation.Tehsil_Id =
                   allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.Tehsil)
                       .HirerchyTypeID;
                dbPersonInformation.Uc_Id =
                   allDistrictDivisionTehsilTownsList.First(m => m.Hierarchy == Config.Hierarchy.UnionCouncil)
                       .HirerchyTypeID;

            }
        }

        public static Config.UserVote GetUserVoteFromAllVotes(List<DbComplaintVote> listComplaintVotes, string cnic, string cell, string externalUserId, string externalProvider)
        {
            Config.UserVote userVote = Config.UserVote.NoVote;
            if (listComplaintVotes != null)
                if (listComplaintVotes.Count > 0)
                {
                    Func<DbComplaintVote, bool> whereExpression;
                    if (string.IsNullOrEmpty(externalUserId) && string.IsNullOrEmpty(externalProvider))
                    {
                        whereExpression = m => m.Cnic == cnic && m.Cell_Number == cell;
                    }
                    else
                    {
                        whereExpression = m => m.Voted_By_Provider == externalProvider && m.Voted_By == externalUserId;

                    }
                    var userVoteInDb = listComplaintVotes.FirstOrDefault(whereExpression);
                    if (userVoteInDb != null)
                    {
                        userVote = (Config.UserVote)userVoteInDb.Is_Positive_Vote;
                    }

                }

            return userVote;
        }


        public static StatusList GetStakeholderValidStatuses(string userName, Config.Language language, Config.AppID appId, Config.PlatformID platformId, int appVersionId)
        {
            if (platformId == Config.PlatformID.IOS)
            {
                userName = "03214304524";
            }

            DbUsers dbUser = DbUsers.GetByUserName(userName);
            if (dbUser != null)
            {
                DbUsersVersionMapping.Update_AddVersion(Config.UserType.Resolver, dbUser.Id, (int)platformId, (int)appId, appVersionId);

                //List<DbStatuses> listDbStatuses = DbStatuses.GetByCampaignId(Convert.ToInt32(dbUser.Campaigns));

                List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, dbUser.Id, (int)Config.Permissions.StatusesForComplaintListing);

                List<DbStatus> listDbStatuses =
                    GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.Id,
                        listDbPermissionsAssignment);

                Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());
                listDbStatuses.GetTranslatedList<DbStatus>("Status", translationDict, language);

                StatusList statusList = new StatusList(listDbStatuses);
                statusList.SetSuccess();
                return statusList;
            }
            return null;
        }

        public static List<DbStatus> GetStatusListByCampaignIdsAndPermissions(List<int> listCampaignIds, int userId, List<DbPermissionsAssignment> listPermissions)
        {
            DbPermissionsAssignment dbPermission = listPermissions
                                    .FirstOrDefault(
                                        n =>
                                            n.Type == (int)Config.PermissionsType.User &&
                                            n.Type_Id == userId &&
                                            n.Permission_Id == (int)Config.Permissions.StatusesForComplaintListing);

            List<DbStatus> listDbStatuses = null;

            if (dbPermission == null)
            {
                listDbStatuses = DbStatus.GetByCampaignIds(listCampaignIds);
            }
            else
            {
                List<int> listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                List<Config.ComplaintStatus> statusList = listStatuses.Select(status => (Config.ComplaintStatus)(status)).ToList();
                listDbStatuses = DbStatus.GetByStatusIds(statusList);
            }

            return listDbStatuses;
        }

        public static LwmcResponseStakeholderLogin SubmitStakeholderLoginWithPhoneNo(SubmitStakeHolderLogin submitStakeHolderLogin, Config.PlatformID platformId)
        {

            //bool isUsernamePresent = DbUsers.IsUsernameAndCnicPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic);
            bool isUsernamePresent = DbUsers.IsPhoneNoAndCnicPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic);
            DbUsers dbUserTemp = null;

            // For IOS platform
            if (platformId == Config.PlatformID.IOS)
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsers> listDbUser = DbUsers.GetByCnic("3520256571439", db);
                foreach (DbUsers dbUser in listDbUser)
                {
                    dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                    db.DbUsers.Attach(dbUser);
                    db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    dbUserTemp = dbUser;
                }
                if (dbUserTemp != null)
                {
                    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                }

                return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
            }
            // End for IOS platform


            if (isUsernamePresent) // username and cnic present
            {
                // Register Device Id
                if (submitStakeHolderLogin.fcm_key != null)
                {
                    DbUsers dbUsers = DbUsers.GetUser(submitStakeHolderLogin.Username,
                        submitStakeHolderLogin.Cnic);
                    string updateCommand = @"UPDATE PITB.User_Wise_Devices
                                        SET Is_Active = 0
                                        WHERE USER_ID = @UserId
                                        
                                        INSERT INTO PITB.User_Wise_Devices
                                        ( User_Id ,
                                          Platform_Id ,
                                          Tag_Id ,
                                          Device_Id ,
                                          Is_Active
                                        )
                                    VALUES  ( @UserId , -- User_Id - int
                                          @Platform_Id , -- Platform_Id - int
                                          @Tag_Id , -- Tag_Id - nvarchar(max)
                                          @Device_Id , -- Device_Id - nvarchar(max)
                                          @Is_Active  -- Is_Active - bit
                                        )

                                        ";
                    Dictionary<string, object> dictParams = new Dictionary<string, object>();
                    dictParams.Add("@UserId", dbUsers.User_Id);
                    dictParams.Add("@Platform_Id", Config.PlatformID.Android.ToDbObj());
                    dictParams.Add("@Tag_Id", "Campaign::47__Type::User__Platform::Android");
                    dictParams.Add("@Device_Id", submitStakeHolderLogin.fcm_key);
                    dictParams.Add("@Is_Active", 1);
                    //dictParams.Add("@UserId", dbUsers.User_Id);
                    DBHelper.GetDataTableByQueryString(updateCommand, dictParams);
                }
                //CMS.Models.DB.DbUserWiseDevices dbUserWiseDevices = new CMS.Models.DB.DbUserWiseDevices();
                //dbUserWiseDevices.Is_Active = true;
                //dbUserWiseDevices.Tag_Id = "Campaign::47__Type::User__Platform::Android";
                //dbUserWiseDevices.User_Id = dbUsers.Id;
                //dbUserWiseDevices.Device_Id = submitStakeHolderLogin.DeviceId;
                //CMS.Helper.Database.DBContextHelperLinq db2 = new CMS.Helper.Database.DBContextHelperLinq();
                //db2.DbUserWiseDevices.Add(dbUserWiseDevices);
                //db2.SaveChanges();
                //End Register Device Id

                string imeiNo = DbUsers.GetImeiAgainstPhoneNoAndCnic(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic);
                if (imeiNo == null) // if imei not registered then register it
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    foreach (DbUsers dbUser in listDbUser)
                    {
                        dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                        db.DbUsers.Attach(dbUser);
                        db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                        db.SaveChanges();
                        dbUserTemp = dbUser;
                    }
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                else if (DbUsers.GetByPhoneNoCnicAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);

                    return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new LwmcResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Already Registered Cannot Register again"));
                }
            }
            else // if username not present
            {
                return new LwmcResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username is incorrect"));
            }

        }


        public static LwmcResponseStakeholderLogin SubmitStakeholderLogin(SubmitStakeHolderLogin submitStakeHolderLogin, Config.PlatformID platformId)
        {

            bool isUsernamePresent = DbUsers.IsUsernameAndCnicPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Cnic);

            DbUsers dbUserTemp = null;

            // For IOS platform
            if (platformId == Config.PlatformID.IOS)
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbUsers> listDbUser = DbUsers.GetByCnic("3520256571439", db);
                foreach (DbUsers dbUser in listDbUser)
                {
                    dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                    db.DbUsers.Attach(dbUser);
                    db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    dbUserTemp = dbUser;
                }
                if (dbUserTemp != null)
                {
                    TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                }

                return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
            }
            // End for IOS platform


            if (isUsernamePresent) // username and cnic present
            {
                string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
                if (imeiNo == null) // if imei not registered then register it
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    foreach (DbUsers dbUser in listDbUser)
                    {
                        dbUser.Imei_No = submitStakeHolderLogin.ImeiNo;
                        db.DbUsers.Attach(dbUser);
                        db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
                        db.SaveChanges();
                        dbUserTemp = dbUser;
                    }
                    if (dbUserTemp != null)
                    {
                        TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
                    }

                    return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);

                    return new LwmcResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new LwmcResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Already Registered Cannot Register again"));
                }
            }
            else // if username not present
            {
                return new LwmcResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username is incorrect"));
            }

        }

        public static SyncModel GetSyncModelAgaistCnicAndAppId(bool useExternal, string cnic, string userId, string userProvider, int appId, Config.Language language) // SmartApp
        {
            List<Config.AppConfig> listAppCampaignConfig = Config.ListAppCampaignConfigurations;
            List<Config.CampaignConfig> listCampaignConfig = Utility.GetCampaignConfigList(listAppCampaignConfig, (Config.AppID)appId);

            List<Config.Campaign> listCampaignIds = listCampaignConfig.Select(n => n.CampaignId).ToList();
            SyncModel syncModel = new SyncModel();
            syncModel.ListCategory = new List<DbComplaintType>();
            syncModel.ListSubCategory = new List<DbComplaintSubType>();
            List<DbComplaintType> listComplaintTypeTemp = null;
            //int districtId = Config.CampDistDictionary[listCampaignIds[0]];
            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());


            foreach (int campaignId in listCampaignIds)
            {
                listComplaintTypeTemp = DbComplaintType.GetByCampaignId(campaignId);

                syncModel.ListCategory.AddRange(listComplaintTypeTemp);
                syncModel.ListSubCategory.AddRange(
                    DbComplaintSubType.GetByComplaintTypes(
                        listComplaintTypeTemp.Select(n => n.Complaint_Category).ToList()));
            }
            DbPersonInformation dbPersonalInfo;
            if (useExternal)
            {
                dbPersonalInfo = DbPersonInformation.GetByExternalUserId(userId, userProvider);
            }
            else
            {
                dbPersonalInfo = DbPersonInformation.GetByCnic(cnic);

            }


            DbProvince dbProvince = DbProvince.GetById(Convert.ToInt32(dbPersonalInfo.Province_Id));
            syncModel.ListProvince = (dbProvince != null) ? new List<DbProvince>() { dbProvince } : new List<DbProvince>(); //DbProvince.GetAllProvincesList();
            //syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            DbDistrict dbDistrict = DbDistrict.GetById(Convert.ToInt32(dbPersonalInfo.District_Id));
            syncModel.ListDistrict = (dbDistrict != null) ? new List<DbDistrict>() { dbDistrict } : new List<DbDistrict>();
            //syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetById(districtId) }; // hardcoding for bahawalpur 

            DbTehsil dbTehsil = DbTehsil.GetById(Convert.ToInt32(dbPersonalInfo.Tehsil_Id));
            syncModel.ListTehsil = (dbTehsil != null) ? new List<DbTehsil>() { dbTehsil } : new List<DbTehsil>();
            //syncModel.ListTehsil = DbTehsil.GetTehsilList(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id);

            //int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId((int) listCampaignConfig[0].CampaignId);

            DbUnionCouncils dbUnionCouncil = DbUnionCouncils.GetById(Convert.ToInt32(dbPersonalInfo.Uc_Id));
            syncModel.ListUnionCouncils = (dbTehsil != null) ? new List<DbUnionCouncils>() { dbUnionCouncil } : new List<DbUnionCouncils>();

            DbWards dbWards = DbWards.GetByWardId(Convert.ToInt32(dbPersonalInfo.Ward_Id));
            syncModel.ListWards = (dbWards != null) ? new List<DbWards>() { dbWards } : new List<DbWards>();

            //syncModel.ListSubCategory = DbComplaintSubType.PopulateImageInBase64FromComplaintsSubtype(syncModel.ListSubCategory);



            /*
            syncModel.ListUnionCouncils = DbUnionCouncils.GetUnionCouncilAgainstCampaign(campaignId);

            if (syncModel.ListUnionCouncils.Count == 0) // no uc exists against campaign
            {
                syncModel.ListUnionCouncils = DbUnionCouncils.GetAllUnionCouncilList();
            }

            syncModel.ListWards = DbWards.GetAllWards();
            */
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Category_UrduName", "Name", translationDict, Config.Language.Urdu);
            syncModel.ListSubCategory.GetTranslatedList<DbComplaintSubType>("SubCategory_UrduName", "Name", translationDict, Config.Language.Urdu);

            return syncModel;
        }
        public static ApiStatus PostToFacebook(PostOnSocialSiteRequestModel request)
        {
            //var accessToken = RefreshAccessToken(request.UserAccessToken);
            FacebookClient fbClient = new FacebookClient(request.UserAccessToken);
            var userProfile = new { name = "", id = "" };

            if (FacebookHandler.IsTokenValid(fbClient))
            {


                var profileData = fbClient.Get("/me?fields=id,name,permissions");

                var userProfileData = JsonConvert.DeserializeObject<FacebookHandler.FBProfile>(profileData.ToString());
                if (userProfileData.Permissions.Data != null && userProfileData.Permissions.Data.Any())
                {
                    var publishPermission = userProfileData.Permissions.Data.FirstOrDefault(m => m.Permission == "publish_actions");
                    if (publishPermission == null)
                    {
                        return new ApiStatus("Failure", "User has not publish_actions permissions");
                    }
                    else
                    {
                        if (publishPermission.Status != "granted")
                            return new ApiStatus("Failure", "User has not publish_actions permissions");

                    }
                }
                ComplaintsForStakeholderFacebookPost complaintData = GetComplaintsForStakeholderFacebookPost(Convert.ToInt32(request.ComplaintId));
                if (complaintData == null) return new ApiStatus("Information", "This complaint wasn't shared socially by complainant");

                if (complaintData.IsAlreadyShared) return new ApiStatus("Information", "This complaint reply already posted !.");
                dynamic parameters = new ExpandoObject();
                if (string.IsNullOrEmpty(complaintData.StakeholderComments))
                {
                    parameters.message = complaintData.ComplaintStatus;

                }
                else
                {
                    parameters.message = complaintData.StakeholderComments;

                }
                if (!string.IsNullOrEmpty(complaintData.AttachmentPath))
                {
                    complaintData.LoadAttachment();
                    parameters.source = new FacebookMediaObject()
                    {
                        ContentType = "image/jpeg",
                        FileName = "Logo",

                    }.SetValue(Convert.FromBase64String(complaintData.LoadedAttachment));

                }


                string postPath = string.Format("/{0}/comments", complaintData.PostId);
                try
                {
                    var publishedResponse = fbClient.Post(postPath, parameters);
                    var dd = JsonConvert.DeserializeObject<FacebookHandler.FBProfile>(publishedResponse.ToString());

                    return new ApiStatus("Success", "This complaint answer is submitted to complainant post on social feed");

                }
                catch (Exception ex)
                {
                    return new ApiStatus("Failure", ex.Message);
                }


            }
            else
            {
                return new ApiStatus("Failure", "Token expired, please login again to continue");

            }




        }

        public static ResponseComplaintStatusChangeLog GetComplaintStatusChangeLog(RequestComplaintStatusLog model, Config.PlatformID platform, Config.Language language)
        {
            ResponseComplaintStatusChangeLog responseComplaintStatusChangeLog = new ResponseComplaintStatusChangeLog() { Status = "Success" };
            ResponseComplaintStatusChangeLog.StatusChangeLog logItem = null;
            List<DbComplaintStatusChangeLog> listDbComplaintStatusChangelogs = DbComplaintStatusChangeLog.GetStatusChangeLogByUsername(model.Username, model.From, model.To);
            responseComplaintStatusChangeLog.TotalLogs =
                DbComplaintStatusChangeLog.GetStatusChangeLogByUsernameCount(model.Username);
            if (listDbComplaintStatusChangelogs != null && listDbComplaintStatusChangelogs.Any())
            {
                responseComplaintStatusChangeLog.Message = string.Format("Total {0} complaint log(s) have been found.", listDbComplaintStatusChangelogs.Count);
                Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());

                foreach (DbComplaintStatusChangeLog log in listDbComplaintStatusChangelogs)
                {
                    logItem = new ResponseComplaintStatusChangeLog.StatusChangeLog();
                    logItem.LogId = log.Id;
                    logItem.ComplaintId = Convert.ToInt32(log.Complaint_Id);
                    logItem.StatusChangeComments = log.Comments;
                    logItem.StatusChangeDateTime = log.StatusChangeDateTime.ToString("dd/MM/yyyy hh:mm tt");
                    logItem.StatusId = Convert.ToByte(log.StatusId);
                    logItem.StatusChangedByUserHierarchy = ((Config.LwmcResolverHirarchyId)Convert.ToByte(log.ChangedBy.User_Hierarchy_Id)).ToString();
                    logItem.Status = ((Config.ComplaintStatus)logItem.StatusId).GetDisplayName();
                    responseComplaintStatusChangeLog.StatusChangeLogs.Add(logItem);
                }
                responseComplaintStatusChangeLog.StatusChangeLogs.GetTranslatedList("Status", "Status", translationDict, language);

            }

            else
            {
                responseComplaintStatusChangeLog.Message = "No complaint log exist for this complaint";
            }



            responseComplaintStatusChangeLog.SetSuccess();
            return responseComplaintStatusChangeLog;
        }


        public static LwmcStakeholderComplaint GetByComplaintById(RequestComplaintStatusLog request, Config.PlatformID platform, Config.Language language)
        {
            LwmcStakeholderComplaint model = new LwmcStakeholderComplaint();
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(request.ComplaintId);

            if (dbComplaint != null)
            {
                model.Complaint_Id = dbComplaint.Id;
                model.Campaign_Name = dbComplaint.Campaign_Name;
                model.Person_Name = dbComplaint.Person_Name;
                model.Person_Contact = dbComplaint.Person_Contact;
                model.Person_Cnic = dbComplaint.Person_Cnic;
                model.District_Name = dbComplaint.District_Name;
                model.Tehsil_Name = dbComplaint.Tehsil_Name;
                model.UnionCouncil_Name = dbComplaint.UnionCouncil_Name;
                model.LocationArea = dbComplaint.LocationArea;
                if (dbComplaint.Latitude.HasValue)
                {
                    model.Latitude = Convert.ToDouble(dbComplaint.Latitude);
                    model.Longitude = Convert.ToDouble(dbComplaint.Longitude);
                }
                model.Stakeholder_Comments = dbComplaint.StatusChangedComments;
                model.Complaint_Category_Name = dbComplaint.Complaint_Category_Name;
                model.Complaint_SubCategory_Name = dbComplaint.Complaint_SubCategory_Name;
                model.Complaint_Remarks = dbComplaint.Complaint_Remarks;
                model.Complaint_Computed_Status = dbComplaint.Complaint_Computed_Status;
                model.Complaint_Computed_Status_Id = dbComplaint.Complaint_Computed_Status_Id.ToString();
                model.Complaint_Computed_Hierarchy = dbComplaint.Complaint_Computed_Hierarchy;
                model.Computed_Remaining_Time_To_Escalate = dbComplaint.Computed_Remaining_Time_To_Escalate;
                model.Created_Date = dbComplaint.Created_Date.Value.ToString("dd/MM/yyyy hh:mm tt");

                model.ListAttachments = DbAttachments.GetByRefAndComplaintId(model.Complaint_Id, Config.AttachmentReferenceType.Add, model.Complaint_Id);
                Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(DbTranslationMapping.GetAllTranslation());

                DbComplaintStatusChangeLog statsChangeLog = DbComplaintStatusChangeLog.GetAllStatusChangeLogByLogId(request.LogId);
                if (statsChangeLog != null)
                {
                    model.Resolver.ListAttachments = DbAttachments.GetByRefAndComplaintId(model.Complaint_Id, Config.AttachmentReferenceType.ChangeStatus);
                    model.Resolver.Complaint_Status_Id = Convert.ToString(statsChangeLog.StatusId);
                    model.Resolver.Stakeholder_Comments = statsChangeLog.Comments;
                    model.Resolver.Complaint_Status = ((Config.ComplaintStatus)Convert.ToInt32(statsChangeLog.StatusId)).GetDisplayName();
                    model.Resolver.GetTranslatedModel("Complaint_Status", translationDict, language);


                }


                model.GetTranslatedModel("Complaint_Category_Name", translationDict, language);
                model.GetTranslatedModel("Complaint_SubCategory_Name", translationDict, language);
                model.GetTranslatedModel("Complaint_Computed_Status", translationDict, language);



                //model.GetTranslatedList("Category_UrduName", "Name", translationDict, language);
                //var statusChangeLogList=DbComplaintStatusChangeLog.GetStatusChangeLogsAgainstComplaintId(complaintId);



                //model.Resolver.Stakeholder_Comments = model.Stakeholder_Comments;
                //model.Resolver.Complaint_Status_Id = model.Complaint_Computed_Status_Id;
                //model.Resolver.Complaint_Status = complaint.Complaint_Computed_Status;



            }



            model.SetSuccess();

            return model;

        }


        public static ComplaintsForStakeholderFacebookPost GetComplaintsForStakeholderFacebookPost(int complaintId)
        {
            ComplaintsForStakeholderFacebookPost model = new ComplaintsForStakeholderFacebookPost();
            DbSocialSharing socialSharingData = DbSocialSharing.GetByComplaintId(complaintId);

            if (socialSharingData == null)
            {
                return null;
            }

            var statusChangeLog = DbComplaintStatusChangeLog.GetLastStatusChangeOfParticularComplaintWhereCommentsAreNotNull(complaintId);
            // DbComplaint complaint = DbComplaint.GetByComplaintId(complaintId);
            //model.ComplaintStatus = complaint.Complaint_Computed_Status;
            var listAttachments = DbAttachments.GetByRefAndComplaintId(complaintId, Config.AttachmentReferenceType.ChangeStatus);
            model.ComplaintId = complaintId;
            if (statusChangeLog != null)
            {
                model.StakeholderComments = statusChangeLog.Comments;

            }
            else
            {
                model.StakeholderComments = "Resolved (Verified)";

            }
            //model.IsAlreadyShared = (socialSharingData.Is_Reply_Submitted.HasValue) && (bool)socialSharingData.Is_Reply_Submitted;
            if (listAttachments != null && listAttachments.Count > 0)
            {
                DbAttachments attachmentOne = listAttachments
                                              .OrderByDescending(m => m.Id).FirstOrDefault(m => m.ReferenceType == (int)Config.AttachmentReferenceType.ChangeStatus);
                if (attachmentOne != null)
                {
                    model.AttachmentPath = attachmentOne.Source_Url;
                }
            }
            model.PostId = socialSharingData.Post_Id;
            //Reset 
            return model;

        }

        public static string RefreshAccessToken(string currentAccessToken)
        {
            FacebookClient fbClient = new FacebookClient();
            Dictionary<string, object> fbParams = new Dictionary<string, object>();
            fbParams["client_id"] = "446813832339709";//"437782676566659"; 
            fbParams["grant_type"] = "fb_exchange_token";
            //fbParams["client_secret"] = "eecb4ec1b33b55893b1216e74db2b293";
            fbParams["client_secret"] = "cca0aab161a824fce0253685aef8ebc7";


            fbParams["fb_exchange_token"] = currentAccessToken;
            JsonObject publishedResponse = fbClient.Get("/oauth/access_token", fbParams) as JsonObject;
            return publishedResponse["access_token"].ToString();
        }
    }
}