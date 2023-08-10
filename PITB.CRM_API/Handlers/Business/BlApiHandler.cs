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
using PITB.CRM_API.Handler.Messages;
using PITB.CRM_API.Handlers.FileUpload;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Models.DB;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.Custom;
using PITB.CRM_API.Handler.Assignment;
using System.Data;
using Amazon.S3.Model;
using Amazon.S3;
using Newtonsoft.Json;
using PITB.CRM_API.Handlers.Translation;
using Image = Amazon.EC2.Model.Image;

namespace PITB.CRM_API.Handlers.Business
{
    public class BlApiHandler
    {
        public static SyncModel GetSyncModelAgaistCampaignId(int campaignId)
        {
            int districtId = Config.CampDistDictionary[campaignId];
            SyncModel syncModel = new SyncModel();
            syncModel.ListCategory = DbComplaintType.GetByCampaignId(campaignId);
            syncModel.ListSubCategory = DbComplaintSubType.GetByComplaintTypes(syncModel.ListCategory.Select(n => n.Complaint_Category).ToList());
            
            syncModel.ListProvince = DbProvince.GetAllProvincesList();
            //syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetById(districtId) }; // hardcoding for bahawalpur 

            //syncModel.ListTehsil = DbTehsil.GetAllTehsilList();

            syncModel.ListTehsil = DbTehsil.GetTehsilList(districtId);
            /*
            syncModel.ListUnionCouncils = DbUnionCouncils.GetUnionCouncilAgainstCampaign(campaignId);

            if (syncModel.ListUnionCouncils.Count == 0) // no uc exists against campaign
            {
                syncModel.ListUnionCouncils = DbUnionCouncils.GetAllUnionCouncilList();
            }

            syncModel.ListWards = DbWards.GetAllWards();
            */
            return syncModel;
        }

        public static SyncModel GetSyncModelAgaistCampaignIdWithUcAndWards(int campaignId, int appId) // SmartApp
        {
            int? groupId = null;
            int districtId = Config.CampDistDictionary[campaignId];
            SyncModel syncModel = new SyncModel();
            syncModel.ListCategory = DbComplaintType.GetByCampaignId(campaignId);
            syncModel.ListSubCategory = DbComplaintSubType.GetByComplaintTypes(syncModel.ListCategory.Select(n => n.Complaint_Category).ToList());

            syncModel.ListProvince = DbProvince.GetAllProvincesList();
            //syncModel.ListDistrict = DbDistrict.GetAllDistrictList();


            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(campaignId, Config.Hierarchy.District);
            syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetDistrictByDistrictIdAndGroupId(districtId, groupId) }; // hardcoding for bahawalpur 


            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(campaignId, Config.Hierarchy.Tehsil);
            syncModel.ListTehsil = DbTehsil.GetByDistrictAndGroupId(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id, groupId);

            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(campaignId, Config.Hierarchy.UnionCouncil);
            syncModel.ListUnionCouncils =
                DbUnionCouncils.GetByTehsilIdList(syncModel.ListTehsil.Select(n => n.Tehsil_Id).ToList(), groupId);

            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(campaignId, Config.Hierarchy.Ward);
            syncModel.ListWards =
                DbWards.GetByUnionCouncilIdListAndGroupId(syncModel.ListUnionCouncils.Select(n => n.UnionCouncil_Id).ToList(), groupId);

            /*
            syncModel.ListUnionCouncils = DbUnionCouncils.GetUnionCouncilAgainstCampaign(campaignId);

            if (syncModel.ListUnionCouncils.Count == 0) // no uc exists against campaign
            {
                syncModel.ListUnionCouncils = DbUnionCouncils.GetAllUnionCouncilList();
            }

            syncModel.ListWards = DbWards.GetAllWards();
            */
            return syncModel;
        }

        public static SyncModel GetSyncModelAgaistMultipleCampaignIdWithUcAndWards(string campaignIds, int appId, Config.Language language=Config.Language.Default) // SmartApp
        {
            int? groupId = null;
            List<int> listCampaignIds = campaignIds.Split(',').Select(int.Parse).ToList();
            SyncModel syncModel = new SyncModel();
            syncModel.ListCategory = new List<DbComplaintType>();
            syncModel.ListSubCategory = new List<DbComplaintSubType>();
            List<DbComplaintType> listComplaintTypeTemp = null;
            int districtId = Config.CampDistDictionary[listCampaignIds[0]];


            foreach (int campaignId in listCampaignIds)
            {
                listComplaintTypeTemp = DbComplaintType.GetByCampaignId(campaignId);

                syncModel.ListCategory.AddRange(listComplaintTypeTemp);
                syncModel.ListSubCategory.AddRange(
                    DbComplaintSubType.GetByComplaintTypes(
                        listComplaintTypeTemp.Select(n => n.Complaint_Category).ToList()));
            }
            syncModel.ListProvince = DbProvince.GetAllProvincesList();
            //syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(listCampaignIds[0], Config.Hierarchy.District);
            syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetDistrictByDistrictIdAndGroupId(districtId, groupId) }; // hardcoding for bahawalpur 


            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(listCampaignIds[0], Config.Hierarchy.Tehsil);
            syncModel.ListTehsil = DbTehsil.GetByDistrictAndGroupId(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id, groupId);


            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(listCampaignIds[0], Config.Hierarchy.UnionCouncil);
            syncModel.ListUnionCouncils =
                DbUnionCouncils.GetByTehsilIdList(syncModel.ListTehsil.Select(n => n.Tehsil_Id).ToList(), groupId);

            groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignIdAndHierarchyId(listCampaignIds[0], Config.Hierarchy.Ward);
            syncModel.ListWards =
                DbWards.GetByUnionCouncilIdListAndGroupId(syncModel.ListUnionCouncils.Select(n => n.UnionCouncil_Id).ToList(), groupId);

            syncModel.ListSubCategory = DbComplaintSubType.PopulateImageInBase64FromComplaintsSubtype(syncModel.ListSubCategory);

            //syncModel.ListImages = DbComplaintSubType.GetUrlListFromComplaintsSubtype(syncModel.ListSubCategory);

            /*
            syncModel.ListUnionCouncils = DbUnionCouncils.GetUnionCouncilAgainstCampaign(campaignId);

            if (syncModel.ListUnionCouncils.Count == 0) // no uc exists against campaign
            {
                syncModel.ListUnionCouncils = DbUnionCouncils.GetAllUnionCouncilList();
            }

            syncModel.ListWards = DbWards.GetAllWards();
            */
            syncModel.Message = "Successfully Synced";
            syncModel.Status = Config.ResponseType.Success.ToString();

            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Name", translationDict, language);
            syncModel.ListSubCategory.GetTranslatedList<DbComplaintSubType>("Name", translationDict, language);
            return syncModel;
        }

        

        public static SyncModel GetSyncModelAgaistCnicAndAppId(bool useExternal,string cnic,string userId,string userProvider, int appId, Config.Language language) // SmartApp
        {
            List<Config.AppConfig> listAppCampaignConfig= Config.ListAppCampaignConfigurations;
            List<Config.CampaignConfig> listCampaignConfig = Utility.GetCampaignConfigList(listAppCampaignConfig, (Config.AppID) appId);
            
            List<Config.Campaign> listCampaignIds = listCampaignConfig.Select(n=>n.CampaignId).ToList();
            SyncModel syncModel = new SyncModel();
            syncModel.ListCategory = new List<DbComplaintType>();
            syncModel.ListSubCategory = new List<DbComplaintSubType>();
            List<DbComplaintType> listComplaintTypeTemp = null;
            //int districtId = Config.CampDistDictionary[listCampaignIds[0]];
            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());


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
            syncModel.ListProvince = (dbProvince!=null) ? new List<DbProvince>(){dbProvince} : new List<DbProvince>(); //DbProvince.GetAllProvincesList();
            //syncModel.ListDistrict = DbDistrict.GetAllDistrictList();

            DbDistrict dbDistrict = DbDistrict.GetById(Convert.ToInt32(dbPersonalInfo.District_Id));
            syncModel.ListDistrict = (dbDistrict != null) ? new List<DbDistrict>() {dbDistrict} : new List<DbDistrict>(); 
            //syncModel.ListDistrict = new List<DbDistrict>() { DbDistrict.GetById(districtId) }; // hardcoding for bahawalpur 

            DbTehsil dbTehsil = DbTehsil.GetById(Convert.ToInt32(dbPersonalInfo.Tehsil_Id));
            syncModel.ListTehsil = (dbTehsil != null) ? new List<DbTehsil>() { dbTehsil } : new List<DbTehsil>(); 
            //syncModel.ListTehsil = DbTehsil.GetTehsilList(syncModel.ListDistrict.ToList().FirstOrDefault().District_Id);

            //int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId((int) listCampaignConfig[0].CampaignId);

            DbUnionCouncils dbUnionCouncil = DbUnionCouncils.GetById(Convert.ToInt32(dbPersonalInfo.Uc_Id));
            syncModel.ListUnionCouncils = (dbTehsil != null) ? new List<DbUnionCouncils>() { dbUnionCouncil } : new List<DbUnionCouncils>();

            DbWards dbWards = DbWards.GetByWardId(Convert.ToInt32(dbPersonalInfo.Ward_Id));
            syncModel.ListWards = (dbWards != null) ? new List<DbWards>() { dbWards } : new List<DbWards>();

            syncModel.ListSubCategory = DbComplaintSubType.PopulateImageInBase64FromComplaintsSubtype(syncModel.ListSubCategory);

          

            /*
            syncModel.ListUnionCouncils = DbUnionCouncils.GetUnionCouncilAgainstCampaign(campaignId);

            if (syncModel.ListUnionCouncils.Count == 0) // no uc exists against campaign
            {
                syncModel.ListUnionCouncils = DbUnionCouncils.GetAllUnionCouncilList();
            }

            syncModel.ListWards = DbWards.GetAllWards();
            */
            syncModel.ListCategory.GetTranslatedList<DbComplaintType>("Category_UrduName","Name", translationDict, Config.Language.Urdu);
            syncModel.ListSubCategory.GetTranslatedList<DbComplaintSubType>("SubCategory_UrduName", "Name", translationDict, Config.Language.Urdu);

            return syncModel;
        }

        public static decimal StoreApiRequestInDb(string jsonStr, string ipAddress, bool isValid, string exception, bool removePictureList = true)
        {
            try
            {
                if (removePictureList)
                {
                    jsonStr = RemoveValueIfPresentFromJsonStr(jsonStr, "PicturesList");
                }
                //jsonStr = RemoveValueIfPresentFromJsonStr(jsonStr, "video");

                DBContextHelperLinq db = new DBContextHelperLinq();
                DbApiRequestLogs dbApiRequest = new DbApiRequestLogs();
                dbApiRequest.JsonString = jsonStr;
                dbApiRequest.IpAddress = ipAddress;
                dbApiRequest.CreatedDateTime = DateTime.Now;
                dbApiRequest.IsValid = isValid;
                dbApiRequest.Exception = exception;
                db.DbApiRequestLogs.Add(dbApiRequest);
                db.SaveChanges();
                return dbApiRequest.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
            //db.db

        }

        public static string RemoveValueIfPresentFromJsonStr(string jsonStr, string keyToRemove)
        {
            SubmitComplaintModel submitComplaintModel =
                    (SubmitComplaintModel)JsonConvert.DeserializeObject(jsonStr, typeof(SubmitComplaintModel));

            submitComplaintModel.PicturesList = null;
            submitComplaintModel.video = null;

            //Dictionary<string, string> jsonDict = (Dictionary<string, string>)JsonConvert.DeserializeObject(jsonStr, typeof(Dictionary<string, string>));
            //jsonDict.Remove(keyToRemove);
            return JsonConvert.SerializeObject(submitComplaintModel);
        }


        public static ComplaintSubmitResponse SubmitComplaint(SubmitComplaintModel submitComplaintModel, Int64 apiRequestId, int appId)
        {
            try
            {
                int provinceId = 0, divisionId = 0, districtId = 0, tehsilId = 0, ucId = 0, wardId = 0;
                Config.AppID app = (Config.AppID) appId;
                DateTime nowDate = DateTime.Now;

                Dictionary<string, object> paramDict = new Dictionary<string, object>();

                //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

                DbPersonInformation dbPersonInfo = DbPersonInformation.GetByCnic(submitComplaintModel.cnic);

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
                if (app == Config.AppID.Awazekhalq)  // Hardcoding for Bahawalpur
                {
                    provinceId = 1;
                    divisionId = 6;
                    districtId = 8;

                    paramDict.Add("@Province_Id", 1); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", 6);
                    //DbDistrict.GetById((int) submitComplaintModel.districtID).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", 8); //submitComplaintModel.districtID.ToDbObj());
                }
                else if(app == Config.AppID.SmartLahore)
                {

                    provinceId = submitComplaintModel.provinceID;
                    divisionId = Convert.ToInt32(DbDistrict.GetById((int)submitComplaintModel.districtID).Division_Id);
                    districtId = submitComplaintModel.districtID;

                    paramDict.Add("@Province_Id", submitComplaintModel.provinceID.ToDbObj()); //submitComplaintModel.provinceID.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int) submitComplaintModel.districtID).Division_Id.ToDbObj());

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

                DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
                //Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
                string[] complaintIdStrArr = dt.Rows[0][1].ToString().Split('-');
                int campaignId = Convert.ToInt32(complaintIdStrArr[0]);
                int complaintId = Convert.ToInt32(complaintIdStrArr[1]);
                string complaintIdStr = dt.Rows[0][1].ToString();

                SaveMobileRequest(submitComplaintModel, Convert.ToInt32(complaintIdStr.Split('-')[1]), apiRequestId);
                if (submitComplaintModel.PicturesList != null)
                {
                    foreach (Picture picture in submitComplaintModel.PicturesList)
                    {
                        FileUploadModel fileUploadModel = new FileUploadModel(picture.picture, Config.AttachmentSaveType.WebServer, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.FileType.File), apiRequestId);
                        FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                        //FileUploadHandler.StartUploadUtilityPWS(picture.picture, "Image", "image/jpeg", ".jpg", campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, apiRequestId);
                        //StartUploadUtility(picture.picture, "Image", "image/jpeg", ".jpg", complaintIdStr, Config.FileType.File, apiRequestId);
                    }
                }
                if (!string.IsNullOrEmpty(submitComplaintModel.video))
                {
                    FileUploadModel fileUploadModel = new FileUploadModel(submitComplaintModel.video, Config.AttachmentSaveType.WebServer, "Video", "application/octet-stream", submitComplaintModel.videoFileExtension, campaignId, complaintId, complaintId, Config.AttachmentReferenceType.Add, Convert.ToInt32(Config.FileType.Video), apiRequestId);
                    FileUploadHandler.StartUploadUtilityPWS(fileUploadModel);
                    //StartUploadUtility(submitComplaintModel.video, "Video", "application/octet-stream",
                    //    submitComplaintModel.videoFileExtension, complaintIdStr, Config.FileType.Video, apiRequestId);
                }
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(), "Your Complaint Id = " + dt.Rows[0][1].ToString());
                //return Utility.GetStatus(Config.ResponseType.Success.ToString(),  dt.Rows[0][1].ToString());
                //return "Your Complaint Id = " + dt.Rows[0][1].ToString();
                // Send message

                
                new Thread(delegate()
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(submitComplaintModel.personContactNumber,
                    submitComplaintModel.campaignID, Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]),
                    submitComplaintModel.categoryID);
                }).Start();

                new Thread(delegate()
                {
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(complaintId, submitComplaintModel.personContactNumber);
                }).Start();
                
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
                apiRequestId = Convert.ToInt32(BlApiHandler.StoreApiRequestInDb(JsonConvert.SerializeObject(submitComplaintModel),"", false, ex.Message));

                return new ComplaintSubmitResponse(Config.ResponseType.Failure.ToString(), "Server Error", "");
            }
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
                        ? (double?) null
                        : double.Parse(submitComplaintModel.lattitude),
                Longitude =
                    (string.IsNullOrEmpty(submitComplaintModel.longitude))
                        ? (double?) null
                        : double.Parse(submitComplaintModel.longitude),
                RequestType = (int) Config.MobileUserRequestType.AddComplaint,
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

        private static void StoreImageUrlInDb(string url, int complaintId, Int64 apiRequestId, string contentType, string fileExtension, Config.FileType fileType)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbAttachments dbAttachments = new DbAttachments();
            dbAttachments.Source_Id = (int)Config.AttachmentSaveType.WebServer;
            dbAttachments.Source_Url = url;
            dbAttachments.Complaint_Id = complaintId;
            dbAttachments.ApiRequestId = apiRequestId;
            dbAttachments.ReferenceType = (int)Config.AttachmentReferenceType.Add;
            dbAttachments.ReferenceTypeId = complaintId;
            dbAttachments.FileName = Config.FileName;
            dbAttachments.FileExtension = fileExtension;
            dbAttachments.FileContentType = contentType;
            dbAttachments.FileType = (int)fileType;
            db.DbAttachments.Add(dbAttachments);
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
                if (complaintModel!=null)
                {
                    listAttachments.Clear();
                    listChangeStatusLogIds = DbComplaintStatusChangeLog.GetActiveStatusChangeLogIdsListAgainstComplaintId(complaintId);
                    foreach (int changeLogId in listChangeStatusLogIds)
                    {
                        listAttachments.AddRange(DbAttachments.GetByRefAndComplaintId(complaintId,Config.AttachmentReferenceType.ChangeStatus,changeLogId)); 
                    }
                    
                    listComplaintStatusModel.Add(new ApiComplaintStatusModel()
                    {
                        ComplaintId = complaintId,
                        //StatusId = (complaintModel.Complaint_Computed_Status_Id==(int)Config.ComplaintStatus.PendingFresh || complaintModel.Complaint_Computed_Status_Id==(int)Config.ComplaintStatus.PendingReopened ) ? 1 : 2,
                        StatusId = (int)complaintModel.Complaint_Computed_Status_Id,
                        StatusMessage = complaintModel.Complaint_Computed_Status,
                        ListPicturesUrl = listAttachments.Select(n=>new Picture(){picture = n.Source_Url}).ToList()
                    });
                }
            }
            responseModel.ListApiStatusModel = listComplaintStatusModel;
            Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
            responseModel.ListApiStatusModel.GetTranslatedList<ApiComplaintStatusModel>("StatusMessage", translationDict, language);
            return responseModel;
        }

        public static ApiStatus SubmitComplainantRemarks(SubmitComplaintRemarks submitComplaintRemarks)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbComplaint dbComplaint =
                    DbComplaint.GetByComplaintId(Convert.ToInt32(submitComplaintRemarks.ComplaintId), db);
                db.DbComplaints.Attach(dbComplaint);

                dbComplaint.Complainant_Remark_Id = (int) submitComplaintRemarks.RemarkType;
                db.Entry(dbComplaint).Property(x => x.Complainant_Remark_Id).IsModified = true;

                dbComplaint.Complainant_Remark_Str = submitComplaintRemarks.RemarkStr;
                db.Entry(dbComplaint).Property(x => x.Complainant_Remark_Str).IsModified = true;

                db.SaveChanges();

                return new ApiStatus(Config.ResponseType.Success.ToString(), Config.ResponseType.Success.ToString());
            }
            catch (Exception ex)
            {
                return new ApiStatus(Config.ResponseType.Failure.ToString(), Config.ResponseType.Failure.ToString());
                throw;
            }
        }

        public static ResponseStakeholderLogin SubmitStakeholderLogin(SubmitStakeHolderLogin submitStakeHolderLogin, Config.PlatformID platformId)
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

                return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
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

                    return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
                }
                // If user credentials is correct and imei not registered then register imei
                else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo)!=null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);

                    return new ResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new ResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Already Registered Cannot Register again"));
                }
            }
            else // if username not present
            {
                return new ResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username is incorrect"));
            }
           
        }



        public static SyncModel SubmitComplainantLogin(SubmitComplainantLogin submitComplainantLogin, int appId, Config.PlatformID platformId, Config.Language language, int dbVersionId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(submitComplainantLogin.Cnic, submitComplainantLogin.MobileNo, db);
            SyncModel syncModel = null;

            int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID)appId);

            string status, message;

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
                message = "User has successfully logined and Imei registered successfully.";

                if (dbVersionId < currDbVersion)
                {
                    syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,cnic,null,null,appId,language);
                }
                else
                {
                    syncModel = new SyncModel();
                }
                TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);

                syncModel.Status = status;
                syncModel.Message = message;
                syncModel.DbVersionId = currDbVersion;

                return syncModel;
            }
            // End For IOS platform


            if (dbPersonalInfo != null) // username and cnic present
            {
                if (dbPersonalInfo.Imei_No == null) // if imei not registered then register it
                {
                    // DbUsers dbUser = DbUsers.GetByUserName(submitStakeHolderLogin.Username, db);
                    dbPersonalInfo.Imei_No = submitComplainantLogin.ImeiNo;
                    db.DbPersonalInfo.Attach(dbPersonalInfo);
                    db.Entry(dbPersonalInfo).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    status = Config.ResponseType.Success.ToString();
                    message = "User has successfully logined and Imei registered successfully.";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,submitComplainantLogin.Cnic,null,null, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }
                    TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                // If user credentials is correct and imei not registered then register imei
                else if (dbPersonalInfo.Imei_No == submitComplainantLogin.ImeiNo)
                {
                    status = Config.ResponseType.Success.ToString();
                    message = "User has successfully logined ";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,submitComplainantLogin.Cnic,null,null, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }
                    TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                else // wrong mobile
                {
                    status = Config.ResponseType.Failure.ToString();
                    message = "Already Registered Cannot Register again";
                }
            }
            else // if username not present
            {
                status = Config.ResponseType.Failure.ToString();
                message = "Mobile no or cnic incorrect";
            }
            if (syncModel == null)
            {
                syncModel = new SyncModel();
            }

            syncModel.Status = status;
            syncModel.Message = message;
            syncModel.DbVersionId = currDbVersion;

            return syncModel;
        }
        [Obsolete]
        public static SyncModel SubmitComplainantLogin1(SubmitComplainantLogin submitComplainantLogin, int appId, Config.PlatformID platformId, Config.Language language, int dbVersionId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(submitComplainantLogin.Cnic, submitComplainantLogin.MobileNo, db);
            SyncModel syncModel = null;

            int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID)appId);

            string status, message;
            
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
                message = "User has successfully logined and Imei registered successfully.";

                if (dbVersionId < currDbVersion)
                {
                    syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,cnic,string.Empty,string.Empty, appId, language);
                }
                else
                {
                    syncModel = new SyncModel();
                }
                TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);

                syncModel.Status = status;
                syncModel.Message = message;
                syncModel.DbVersionId = currDbVersion;

                return syncModel;
            }
            // End For IOS platform


            if (dbPersonalInfo!=null) // username and cnic present
            {
                if (dbPersonalInfo.Imei_No == null) // if imei not registered then register it
                {   
                   // DbUsers dbUser = DbUsers.GetByUserName(submitStakeHolderLogin.Username, db);
                    dbPersonalInfo.Imei_No = submitComplainantLogin.ImeiNo;
                    db.DbPersonalInfo.Attach(dbPersonalInfo);
                    db.Entry(dbPersonalInfo).Property(x => x.Imei_No).IsModified = true;
                    db.SaveChanges();
                    status = Config.ResponseType.Success.ToString();
                    message = "User has successfully logined and Imei registered successfully.";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,string.Empty,string.Empty,submitComplainantLogin.Cnic, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }
                    TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                // If user credentials is correct and imei not registered then register imei
                else if (dbPersonalInfo.Imei_No == submitComplainantLogin.ImeiNo)
                {
                    status = Config.ResponseType.Success.ToString();
                    message = "User has successfully logined ";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,string.Empty ,string.Empty ,submitComplainantLogin.Cnic, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }
                    TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                else // wrong mobile
                {
                    status = Config.ResponseType.Failure.ToString();
                    message = "Already Registered Cannot Register again";
                }
            }
            else // if username not present
            {
                status = Config.ResponseType.Failure.ToString();
                message = "Mobile no or cnic incorrect";
            }
            if (syncModel == null)
            {
                syncModel = new SyncModel();
            }

            syncModel.Status = status;
            syncModel.Message = message;
            syncModel.DbVersionId = currDbVersion;

            return syncModel;
        }


        public static SyncModel SyncComplainant(SubmitSyncComplainant submitSyncComplainant, int appId, Config.Language language, Config.PlatformID platformId, int dbVersionId, int appVersionId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbPersonInformation dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(submitSyncComplainant.Cnic, submitSyncComplainant.MobileNo, db);
            SyncModel syncModel = null;

            int currDbVersion = DbVersion.GetDbVersion(Config.VersionType.MobileApp, (Config.AppID) appId);

            string status, message;

            // platformId Check
            if (platformId == Config.PlatformID.IOS)
            {
                string cnic = "3333335555555";
                string mobileNo = "03332222225";

                status = Config.ResponseType.Success.ToString();
                message = "Successfully Synced";
                dbPersonalInfo = DbPersonInformation.GetByCnicAndMobileNo(cnic, mobileNo, db);

                if (dbVersionId < currDbVersion)
                {
                    syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,cnic,string.Empty,string.Empty, appId, language);
                    
                }
                else
                {
                    syncModel = new SyncModel();
                }

                if (dbPersonalInfo != null)
                {
                    DbUsersVersionMapping.Update_AddVersion(Config.UserType.Complainant, dbPersonalInfo.Person_id,(int) platformId,appId, appVersionId);
                }

                syncModel.Status = status;
                syncModel.Message = message;
                syncModel.DbVersionId = currDbVersion;

                return syncModel;
            }
            // End platform Id check

            if (dbPersonalInfo != null) // username and cnic present
            {
                if (dbPersonalInfo.Imei_No==null || dbPersonalInfo.Imei_No == submitSyncComplainant.ImeiNo)
                {

                    status = Config.ResponseType.Success.ToString();
                    message = "Successfully Synced";
                    if (dbVersionId < currDbVersion)
                    {
                        syncModel = BlApiHandler.GetSyncModelAgaistCnicAndAppId(false,submitSyncComplainant.Cnic,string.Empty,string.Empty, appId,
                            language);
                    }
                    else
                    {
                        syncModel = new SyncModel();
                    }

                    if (dbPersonalInfo != null)
                    {
                        DbUsersVersionMapping.Update_AddVersion(Config.UserType.Complainant, dbPersonalInfo.Person_id, (int) platformId, appId, appVersionId);
                    }

                    //TextMessageHandler.SendVerificationMessageToComplainant(dbPersonalInfo);
                }
                else // wrong mobile
                {
                    status = Config.ResponseType.Failure.ToString();
                    message = "Sync Unsuccessful Already Registered with another imei";
                }
            }
            else // if username not present
            {
                status = Config.ResponseType.Failure.ToString();
                message = "Sync Unsuccessful Mobile no or cnic incorrect";
            }
            if (syncModel == null)
            {
                syncModel = new SyncModel();
            }

            syncModel.Status = status;
            syncModel.Message = message;
            syncModel.DbVersionId = currDbVersion;
            return syncModel;
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

                List<DbStatuses> listDbStatuses = DbStatuses.GetByCampaignId(Convert.ToInt32(dbUser.Campaigns));
                //listDbStatuses.Remove(
                //    listDbStatuses.Where(n => n.Id == (int) Config.ComplaintStatus.PendingFresh).FirstOrDefault());
                //listDbStatuses.Remove(
                //    listDbStatuses.Where(n => n.Id == (int) Config.ComplaintStatus.UnsatisfactoryClosed)
                //        .FirstOrDefault());
                Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
                listDbStatuses.GetTranslatedList<DbStatuses>("Status", translationDict, language);

                StatusList statusList = new StatusList(listDbStatuses);
                
                return statusList;
            }
            return null;
        }

        public static GetComplainantComplaintModel GetComplainantComplaints(string cnic, int appId, Config.Language language, int campaignId)
        {
            GetComplainantComplaintModel getComlaintsModel = new GetComplainantComplaintModel();
            getComlaintsModel.ListComplaint = new List<ComplainantComplaintModel>();
            
            List<Picture> listStatusPicturesUrl = new List<Picture>();
            List<Picture> listComplaintPicturesUrl = new List<Picture>();
            List<Video> listComplaintVideoUrl = new List<Video>();
            List<int> listChangeStatusLogIds = null;
            List<DbAttachments> listDbAttachment = new List<DbAttachments>();
            
            List<DbComplaint> listDbComplaints = DbComplaint.GetListByPersonCnic(cnic, campaignId);
            foreach (DbComplaint dbComplaint in listDbComplaints)
            {
                //listAttachments.Clear();
                listChangeStatusLogIds = DbComplaintStatusChangeLog.GetActiveStatusChangeLogIdsListAgainstComplaintId(dbComplaint.Id);
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


                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id,Config.FileType.File);

                if (listDbAttachment != null)
                {
                    listComplaintPicturesUrl = listDbAttachment.Select(n => new Picture()
                    {
                        picture = n.Source_Url
                    }).ToList();
                }

                listDbAttachment = DbAttachments.GetByRefAndComplaintIdAndFileType(dbComplaint.Id, Config.AttachmentReferenceType.Add, dbComplaint.Id, Config.FileType.Video);

                if (listDbAttachment != null)
                {
                    listComplaintVideoUrl = listDbAttachment.Select(n => new Video()
                    {
                        video = n.Source_Url
                    }).ToList();
                }


                ComplainantComplaintModel complaintModel = new ComplainantComplaintModel()
                   {
                       complaintID = dbComplaint.Id,
                       campaignID = Convert.ToInt32(dbComplaint.Compaign_Id),
                       categoryID = Convert.ToInt32(dbComplaint.Complaint_Category),
                       
                       cnic = dbComplaint.Person_Cnic,
                       comment = dbComplaint.Complaint_Remarks,
                       date = Convert.ToDateTime(dbComplaint.Created_Date).ToString("dd/MM/yyyy hh:mm tt"),
                       departmentId = Convert.ToInt32(dbComplaint.Department_Id),
                       districtID = Convert.ToInt32(dbComplaint.District_Id),
                       
                       ListPicturesComplaintsUrl = listComplaintPicturesUrl,
                       ListPicturesStatusUrl = listStatusPicturesUrl,
                       ListVideoComplaintsUrl = listComplaintVideoUrl,
                       personContactNumber = dbComplaint.Person_Contact,
                       personName = dbComplaint.Person_Name,
                       provinceID = Convert.ToInt32(dbComplaint.Province_Id),
                       
                       subCategoryID = Convert.ToInt32(dbComplaint.Complaint_SubCategory),
                       
                       tehsilID = Convert.ToInt32(dbComplaint.Tehsil_Id),
                       
                       ucID = Convert.ToInt32(dbComplaint.UnionCouncil_Id),
                       wardID = Convert.ToInt32(dbComplaint.Ward_Id),
                       statusId = Convert.ToInt32(dbComplaint.Complaint_Computed_Status_Id),
                       statusStr = dbComplaint.Complaint_Computed_Status
                   };
                //complaintModel.categoryName = DbComplaintType.GetById(complaintModel.categoryID);

                

                complaintModel.categoryName = dbComplaint.Complaint_Category_Name;//(complaintModel.categoryID != 0) ? DbComplaintType.GetById(complaintModel.categoryID).Name : "";
                complaintModel.subCategoryName = dbComplaint.Complaint_SubCategory_Name;//(complaintModel.subCategoryID != 0) ? DbComplaintSubType.GetById(complaintModel.subCategoryID).Name : "";

                Dictionary<string, TranslatedModel> translationDict = DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping(DbTranslationMapping.GetAllTranslation());
                complaintModel.GetTranslatedModel<ComplainantComplaintModel>("categoryName", translationDict, language);
                complaintModel.GetTranslatedModel<ComplainantComplaintModel>("subCategoryName", translationDict, language);

                complaintModel.GetTranslatedModel<ComplainantComplaintModel>("statusStr", translationDict, language);


                complaintModel.provinceName = dbComplaint.Province_Name;//(complaintModel.provinceID != 0) ? DbProvince.GetById(complaintModel.provinceID).Province_Name : "";
                complaintModel.districtName = dbComplaint.District_Name;//(complaintModel.districtID != 0) ? DbDistrict.GetById(complaintModel.districtID).District_Name : "";
                complaintModel.tehsilName = dbComplaint.Tehsil_Name; //(complaintModel.tehsilID != 0) ? DbTehsil.GetById(complaintModel.tehsilID).Tehsil_Name : "";
                complaintModel.ucName = dbComplaint.UnionCouncil_Name;

                complaintModel.campaignName = DbCampaign.GetById(complaintModel.campaignID).Campaign_Name;
                complaintModel.GetTranslatedModel<ComplainantComplaintModel>("campaignName", translationDict, language);

                getComlaintsModel.ListComplaint.Add(
                   complaintModel
                );
            }
            getComlaintsModel.Status = Config.ResponseType.Success.ToString();
            getComlaintsModel.Message = Config.ResponseType.Success.ToString();
            return getComlaintsModel;
        }

       
    }
} 