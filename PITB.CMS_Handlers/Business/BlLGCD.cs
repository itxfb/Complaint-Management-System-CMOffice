using System;
using System.Collections.Generic;
using System.Data;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Business
{
    public class BlLGCD
    {
        public static Config.CommandMessage AddComplaint(CustomForm.Post form)
        {
            PostModel.File postedFiles = form.postedFiles;

            CMSCookie cmsCookie;

            DateTime? nowDate, minSrcIdDate, maxSrcIdDate,
                 complaintCreatedDate, complaintAssignedDate, complaintCompletedDate, complaintDeletedDate;

            bool isProfileEditing, isComplaintEditing, isFormValid, personIsCnicPresent=false, complaintIsDeleted;
            int? complaintProvinceId, complaintDistrictId, complaintDivisionId, complaintTehsilId, complaintUcId=0, complaintWardId=0,
                departmentId, categoryId=-1, subCategoryId=-1, agentId,
                complaintStatusId, userId, ComplaintCreatedBy, complaintUpdatedBy, complaintDeletedBy,
                personId, personGender, personProvinceId, personDivisionId, personDistrictId, personTehsilId, personUcId,
                personCreatedBy, personUpdatedBy,
                maxLevel, minSrcId, maxSrcId;
            int complaintType=-1, campaignId, complaintSrc;
            string currentTab, modelName=null, complaintVm=null, personVm=null, complaintRemarks=null, complaintAgentComments= null, complaintAddress=null,
                complaintBusinessAddress = null, personCnic=null, personName=null, personFatherName=null, personMobileNo=null, personSecondaryNo=null,
                personLandlineNo=null, personAddress=null, personEmail=null, personNearestPlace=null;

            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;

            isFormValid = form.IsFormValid();

            if (!isFormValid)
            {
                Config.CommandMessage exceptionMsg = new Config.CommandMessage(Config.CommandStatus.Exception, "An error has occured");
                return exceptionMsg;
            }
            FileUploadHandler.FileValidationStatus validationStatus = FileUploadHandler.GetFileValidationStatus(postedFiles);

            //form.IsFormAuthentic();
            //int asd = Convert.ToInt32("hahaha");

            personVm = "PersonalInfoVm";
            complaintVm = "ComplaintVm";

            nowDate = DateTime.Now;
            cmsCookie = AuthenticationHandler.GetCookie();
            isProfileEditing = form.GetElementValue(string.Format("{0}.Person_id", personVm)).CastObj<int>() > 0;//(complaintModel.PersonalInfoVm.Person_id > 0); //form.GetElementValue("ComplaintVm.IsProfileEditing").CastObj<bool>();
            isComplaintEditing = form.GetElementValue(string.Format("{0}.Id", complaintVm)).CastObj<int>() != 0;//(complaintModel.ComplaintVm.Id != 0); //form.GetElementValue("ComplaintVm.IsComplaintEditing").CastObj<bool>();
            personCnic = form.GetElementValue(string.Format("{0}.Cnic_No", personVm)).CastObj<string>();
            campaignId = form.GetElementValue(string.Format("{0}.Compaign_Id", complaintVm)).CastObj<int>();
            personIsCnicPresent = form.GetElementValue(string.Format("{0}.IsCnicPresent", personVm)).CastObj<bool>();

            // Current Tab
            currentTab = form.GetElementValue("currentComplaintTypeTab").CastObj<string>();
            
            if (currentTab == VmAddComplaint.TabComplaint)
            {
                modelName = "ComplaintVm";
                complaintType = (int)Config.ComplaintType.Complaint;
            }
            else if (currentTab == VmAddComplaint.TabSuggestion)
            {
                modelName = "SuggestionVm";
                complaintType = (int)Config.ComplaintType.Suggestion;
            }
            else if (currentTab == VmAddComplaint.TabInquiry)
            {
                modelName = "InquiryVm";
                complaintType = (int)Config.ComplaintType.Inquiry;
            }

            // Person Cnic
            personIsCnicPresent = true;
            if (string.IsNullOrEmpty(personCnic))
            {
                personIsCnicPresent = false;
            }
            if (!personIsCnicPresent && !isProfileEditing)
            {
                decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
                string cnicStr = cnic.ToString();
                cnicStr = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
                personCnic = cnicStr;
            }

            // Complaint Status
            if (isComplaintEditing)
            {
                complaintStatusId = form.GetElementValue(string.Format("{0}.Complaint_Status_Id", complaintVm)).CastObj<int?>();
            }
            else
            {
                List<string> lisKeys = new List<string>()
                {
                    string.Format("Type::Config___Module::ComplaintLaunchStatus___CampaignId::{0}",campaignId),
                    "Type::Config___Module::ComplaintLaunchStatus"
                };
                complaintStatusId = int.Parse(ConfigurationHandler.GetConfiguration(lisKeys,
                    complaintType.ToString()));
            }


            departmentId = form.GetElementValue(string.Format("{0}.departmentId", modelName)).CastObj<int?>();
            categoryId = form.GetElementValue(string.Format("{0}.Complaint_Category", modelName)).CastObj<int?>();
            subCategoryId = form.GetElementValue(string.Format("{0}.Complaint_SubCategory", modelName)).CastObj<int?>();


            complaintProvinceId = form.GetElementValue(string.Format("{0}.Province_Id", modelName)).CastObj<int?>();
            complaintDistrictId = form.GetElementValue(string.Format("{0}.District_Id", modelName)).CastObj<int?>();
            complaintDivisionId = DbDistrict.GetById((int)complaintDistrictId).Division_Id;
            complaintTehsilId = form.GetElementValue(string.Format("{0}.Tehsil_Id", modelName)).CastObj<int?>();
            complaintUcId = form.GetElementValue(string.Format("{0}.UnionCouncil_Id", modelName)).CastObj<int?>();
            complaintWardId = form.GetElementValue(string.Format("{0}.Ward_Id", modelName)).CastObj<int?>();



            // Assignment Matrix
         
            //int categoryId = -1;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;

            //AssignmentMatrix
            List<AssignmentModel> assignmentModelList = null;
            if (currentTab == VmAddComplaint.TabComplaint) // when there is complaint populate assignment matrix
            {
                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)subCategoryId);
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

                List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
				{
					new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)complaintProvinceId),
					new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)complaintDivisionId),
					new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)complaintDistrictId),
					new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (int?)complaintTehsilId),
					new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)complaintUcId),
					new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)complaintWardId)
				};


                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment((DateTime)nowDate,
                DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)campaignId, (int)categoryId, (int)subCategoryId, null, null, listHierarchyPair), catRetainingHours) /* nowDate,
				DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours*/);
            }
            else
            {
                assignmentModelList = new List<AssignmentModel>();
            }




            complaintRemarks = form.GetElementValue(string.Format("{0}.Complaint_Remarks", modelName)).CastObj<string>();
            complaintAgentComments = form.GetElementValue(string.Format("{0}.Agent_Comments", modelName)).CastObj<string>();

            complaintAddress = form.GetElementValue(string.Format("{0}.Complaint_Address", modelName)).CastObj<string>();
            complaintBusinessAddress = form.GetElementValue(string.Format("{0}.Business_Address", modelName)).CastObj<string>();

            userId = cmsCookie.UserId;
            agentId = userId;

            complaintStatusId = complaintStatusId;
            complaintCreatedDate = nowDate;
            ComplaintCreatedBy = userId;
            complaintAssignedDate = null;
            complaintCompletedDate = null;
            complaintUpdatedBy = userId;
            complaintIsDeleted = false;
            complaintDeletedDate = null;
            complaintDeletedBy = null;
            complaintSrc = (int)Config.ComplaintSource.Agent;

            personId = form.GetElementValue(string.Format("{0}.Person_id", personVm)).CastObj<int?>();
            personName = form.GetElementValue(string.Format("{0}.Person_Name", personVm)).CastObj<string>();
            personFatherName = form.GetElementValue(string.Format("{0}.Person_Father_Name", personVm)).CastObj<string>();
            //personIsCnicPresent = personIsCnicPresent;
            //personCnic = personCnic;
            personGender = form.GetElementValue(string.Format("{0}.Gender", personVm)).CastObj<int?>();
            personMobileNo = form.GetElementValue(string.Format("{0}.Mobile_No", personVm)).CastObj<string>();
            personSecondaryNo = form.GetElementValue(string.Format("{0}.Secondary_Mobile_No", personVm)).CastObj<string>();
            personLandlineNo = form.GetElementValue(string.Format("{0}.LandLine_No", personVm)).CastObj<string>();
            personAddress = form.GetElementValue(string.Format("{0}.Person_Address", personVm)).CastObj<string>();
            personEmail = form.GetElementValue(string.Format("{0}.Email", personVm)).CastObj<string>();
            personNearestPlace = form.GetElementValue(string.Format("{0}.Nearest_Place", personVm)).CastObj<string>();
            personProvinceId = form.GetElementValue(string.Format("{0}.Province_Id", personVm)).CastObj<int?>();
            personDivisionId = form.GetElementValue(string.Format("{0}.Division_Id", personVm)).CastObj<int?>();
            personDistrictId = form.GetElementValue(string.Format("{0}.District_Id", personVm)).CastObj<int?>();
            personTehsilId = form.GetElementValue(string.Format("{0}.Tehsil_Id", personVm)).CastObj<int?>();
            personTehsilId = form.GetElementValue(string.Format("{0}.Town_Id", personVm)).CastObj<int?>();
            personUcId = form.GetElementValue(string.Format("{0}.Uc_Id", personVm)).CastObj<int?>();
            personCreatedBy = userId;
            personUpdatedBy = userId;
            

            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@Id", -1);
            paramDict.Add("@Person_Id", personId.ToDbObj());
            paramDict.Add("@DepartmentId", departmentId.ToDbObj());
            paramDict.Add("@Complaint_Type", complaintType);
            paramDict.Add("@Complaint_Category", categoryId.ToDbObj());
            paramDict.Add("@Complaint_SubCategory", subCategoryId.ToDbObj());
            paramDict.Add("@Compaign_Id", campaignId.ToDbObj());
            paramDict.Add("@Province_Id", complaintProvinceId.ToDbObj());
            paramDict.Add("@Division_Id", complaintDivisionId.ToDbObj());
            paramDict.Add("@District_Id", complaintDistrictId.ToDbObj());
            paramDict.Add("@Tehsil_Id", complaintTehsilId.ToDbObj());
            paramDict.Add("@UnionCouncil_Id", complaintUcId ?? 0);
            paramDict.Add("@Ward_Id", complaintWardId ?? 0);
            paramDict.Add("@Complaint_Remarks", complaintRemarks.ToDbObj());
            paramDict.Add("@Agent_Comments", complaintAgentComments.ToDbObj());

            paramDict.Add("@Agent_Id", agentId.ToDbObj());
            paramDict.Add("@Complaint_Address", complaintAddress.ToDbObj());
            paramDict.Add("@Business_Address", complaintBusinessAddress.ToDbObj());

            paramDict.Add("@Complaint_Status_Id", complaintStatusId.ToDbObj());//If complaint is adding then set complaint status to 1 (Pending(Fresh) 
            paramDict.Add("@Created_Date", complaintCreatedDate.ToDbObj());
            paramDict.Add("@Created_By", ComplaintCreatedBy.ToDbObj());
            paramDict.Add("@Complaint_Assigned_Date", complaintAssignedDate.ToDbObj());
            paramDict.Add("@Completed_Date", complaintCompletedDate.ToDbObj());
            //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
            paramDict.Add("@Updated_By", complaintUpdatedBy.ToDbObj());
            paramDict.Add("@Is_Deleted", complaintIsDeleted.ToDbObj());
            paramDict.Add("@Date_Deleted", complaintDeletedDate.ToDbObj());
            paramDict.Add("@Deleted_By", complaintDeletedBy.ToDbObj());
            paramDict.Add("@ComplaintSrc", complaintSrc.ToDbObj());
            paramDict.Add("@IsComplaintEditing", isComplaintEditing.ToDbObj());

            //Personal Information
            paramDict.Add("@p_Person_id", personId.ToDbObj());
            paramDict.Add("@Person_Name", personName.ToDbObj());
            paramDict.Add("@Person_Father_Name", personFatherName.ToDbObj());
            paramDict.Add("@Is_Cnic_Present", personIsCnicPresent.ToDbObj());
            paramDict.Add("@Cnic_No", personCnic.ToDbObj());
            paramDict.Add("@Gender", personGender.ToDbObj());
            paramDict.Add("@Mobile_No", personMobileNo.ToDbObj());
            paramDict.Add("@Secondary_Mobile_No", personSecondaryNo.ToDbObj());
            paramDict.Add("@LandLine_No", personLandlineNo.ToDbObj());
            paramDict.Add("@Person_Address", personAddress.ToDbObj());
            paramDict.Add("@Email", personEmail.ToDbObj());
            paramDict.Add("@Nearest_Place", personNearestPlace.ToDbObj());
            paramDict.Add("@p_Province_Id", personProvinceId.ToDbObj());
            paramDict.Add("@p_Division_Id", personDivisionId.ToDbObj());
            paramDict.Add("@p_District_Id", personDistrictId.ToDbObj());
            paramDict.Add("@p_Tehsil_Id", personTehsilId.ToDbObj());
            paramDict.Add("@p_Town_Id", personTehsilId.ToDbObj());
            paramDict.Add("@p_Uc_Id", personUcId.ToDbObj());
            paramDict.Add("@p_Created_By", personCreatedBy.ToDbObj());
            paramDict.Add("@p_Updated_By", personUpdatedBy.ToDbObj());
            paramDict.Add("@IsProfileEditing", isProfileEditing.ToDbObj());
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

            paramDict.Add("@MaxLevel", assignmentModelList.Count);

            paramDict.Add("@MinSrcId", AssignmentHandler.GetMinSrcId(assignmentModelList).ToDbObj());
            paramDict.Add("@MaxSrcId", AssignmentHandler.GetMaxSrcId(assignmentModelList).ToDbObj());

            paramDict.Add("@MinSrcIdDate", AssignmentHandler.GetMinDate(assignmentModelList).ToDbObj());
            paramDict.Add("@MaxSrcIdDate", AssignmentHandler.GetMaxDate(assignmentModelList).ToDbObj());

            if (!string.IsNullOrEmpty(form.GetElementValue("RadioInPerson")))
            {
                paramDict.Add("@RefField1",
                    form.GetElementValue("RadioInPerson")
                        .Split(new string[] {Config.Separator}, StringSplitOptions.None)[1].CastObj<string>());
            }
            if (!string.IsNullOrEmpty(form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")))
            {
                paramDict.Add("@RefField2",
                    form.GetElementValue("ComplaintVm.ListDynamicDropDown[2].SelectedItemId")
                        .Split(new string[] {Config.Separator}, StringSplitOptions.None)[1].CastObj<string>());
            }
            paramDict.Add("@RefField3", form.GetElementValue("DynamicComplaintVmCnicInPerson").CastObj<string>());
            paramDict.Add("@RefField4", form.GetElementValue("DynamicComplaintVmMobileNoInPerson").CastObj<string>());
            paramDict.Add("@RefField5", form.GetElementValue("DynamicComplaintVmNameInPerson").CastObj<string>());


            // Add ComplaintSp
            DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
            string complaintIdStr = dt.Rows[0][1].ToString();
            int complaintId = Convert.ToInt32(complaintIdStr.Split('-')[1]);
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());

            FileUploadHandler.UploadMultipleFiles(postedFiles, Config.AttachmentReferenceType.Add, complaintIdStr, complaintId, Config.TAG_COMPLAINT_ADD);

            DynamicFieldsHandler.SaveDyamicFieldsInDb(form, currentTab, complaintId);


            if (cm.Status == Config.CommandStatus.Success && currentTab == VmAddComplaint.TabComplaint) // send message on complaint launch
            {

                if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(personMobileNo,
                        (int)campaignId, Convert.ToInt32(cm.Value.Split('-')[1]),
                        (int)categoryId, personName);
                    string msgToText = null;
                    
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId), msgToText);
                }
            }

            return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint action added successfully Id = " + campaignId + "-" + complaintId);
           
        }
    }
}