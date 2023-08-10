using System;
using AutoMapper;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.Permission;
using System.Linq;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Helper.Database;
using System.Collections.Generic;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Helper;
using System.Data;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Models.View.Dynamic;
using System.Data.Entity;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.API;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.ApiModels.Request;
using PITB.CMS_Common.Models.ApiModels.Response;
using PITB.CMS_Common.Handler.ComplaintFileHandler;
using System.Dynamic;
using PITB.CMS_Common.Helper.Extensions;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlComplaints
    {
        public static dynamic GetAddComplaintSpModel(dynamic dParam)
        {
            dynamic d = new ExpandoObject();
            d.id = Utility.GetPropertyValueFromDynamic(dParam, "id") ?? -1;
            d.personId = dParam.personId;
            d.departmentId = Utility.GetPropertyValueFromDynamic(dParam, "departmentId");
            d.complaintType = dParam.complaintType;
            d.categoryId = dParam.categoryId;
            d.subcategoryId = dParam.subcategoryId;
            d.campaignId = dParam.campaignId;
            d.provinceId = dParam.provinceId;
            d.divisionId = Utility.GetPropertyValueFromDynamic(dParam, "divisionId");
            d.districtId = dParam.districtId;
            d.tehsilId = dParam.tehsilId;
            d.ucId = Utility.GetPropertyValueFromDynamic(dParam, "ucId") == null ? 0 : dParam.ucId;
            d.wardId = Utility.GetPropertyValueFromDynamic(dParam, "wardId") == null ? 0 : dParam.wardId;
            d.complaintDetail = dParam.complaintDetail;
            d.agentComments = dParam.agentComments;
            d.agentId = Utility.GetPropertyValueFromDynamic(dParam, "complaintCreatedBy");
            d.complaintAddress = Utility.GetPropertyValueFromDynamic(dParam, "complaintAddress");
            d.businessAddress = Utility.GetPropertyValueFromDynamic(dParam, "businessAddress");
            d.complaintStatusId = Utility.GetPropertyValueFromDynamic(dParam, "complaintStatusId") ?? (int)Config.ComplaintStatus.PendingFresh;
            d.complaintCreatedDate = Utility.GetPropertyValueFromDynamic(dParam, "complaintCreatedDate") ?? DateTime.Now;
            d.complaintCreatedBy = Utility.GetPropertyValueFromDynamic(dParam, "complaintCreatedBy");
            d.complaintAssignedDate = Utility.GetPropertyValueFromDynamic(dParam, "complaintAssignedDate");
            d.complaintCompletedDate = Utility.GetPropertyValueFromDynamic(dParam, "complaintCompletedDate");
            d.complaintUpdatedBy = d.complaintCreatedBy;
            d.complaintIsDeleted = Utility.GetPropertyValueFromDynamic(dParam, "complaintIsDeleted") ?? false;
            d.complaintDeletedDate = Utility.GetPropertyValueFromDynamic(dParam, "complaintDeletedDate");
            d.complaintDeletedBy = Utility.GetPropertyValueFromDynamic(dParam, "complaintDeletedBy");
            d.complaintSrc = Utility.GetPropertyValueFromDynamic(dParam, "complaintSrc");
            d.isComplaintEditing = Utility.GetPropertyValueFromDynamic(dParam, "isComplaintEditing") ?? false;

            d.personId = dParam.personId;
            d.personName = dParam.personName;
            d.personFatherName = Utility.GetPropertyValueFromDynamic(dParam, "personFatherName");
            d.personIsCnicPresent = dParam.personIsCnicPresent;//Utility.GetPropertyValueFromDynamic(dParam, "personIsCnicPresent");
            d.personCnic = GetPersonCnic(dParam);
            d.personGender = dParam.personGender;
            d.personMobileNo = dParam.personMobileNo;
            d.personSecondaryNo = dParam.personSecondaryNo;
            d.personLandlineNo = Utility.GetPropertyValueFromDynamic(dParam, "personLandlineNo");
            d.personAddress = dParam.personAddress;
            d.personEmail = Utility.GetPropertyValueFromDynamic(dParam, "personEmail");
            d.personNearestPlace = Utility.GetPropertyValueFromDynamic(dParam, "personNearestPlace");
            d.personProvinceId = dParam.personProvinceId;
            d.personDivisionId = dParam.personDivisionId;
            d.personDistrictId = dParam.personDistrictId;
            d.personTehsilId = Utility.GetPropertyValueFromDynamic(dParam, "personTehsilId");
            d.personTownId = d.personTehsilId;
            d.personUcId = Utility.GetPropertyValueFromDynamic(dParam, "personUcId");
            d.personCreatedBy = d.complaintCreatedBy;
            d.personUpdatedBy = d.complaintCreatedBy;
            d.personIsProfileEditing = dParam.personIsProfileEditing;//dParam.personId==null?true:false;
            //List<AssignmentModel> listAssignmentModel = d.listAssignmentModel;


            PopulateAssigmentMatrixProperties(d);

            return d;

        }

        public static Dictionary<string, object> GetAddComplaintSpParamsDict(dynamic d)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@Id", -1);
            paramDict.Add("@Person_Id", (d.personId as object).ToDbObj());
            paramDict.Add("@DepartmentId", (d.departmentId as object).ToDbObj());
            paramDict.Add("@Complaint_Type", (d.complaintType as object).ToDbObj());
            paramDict.Add("@Complaint_Category", (d.categoryId as object).ToDbObj());
            paramDict.Add("@Complaint_SubCategory", (d.subcategoryId as object).ToDbObj());
            paramDict.Add("@Compaign_Id", (d.campaignId as object).ToDbObj());
            paramDict.Add("@Province_Id", (d.provinceId as object).ToDbObj());
            paramDict.Add("@Division_Id", (d.divisionId as object).ToDbObj());
            paramDict.Add("@District_Id", (d.districtId as object).ToDbObj());
            paramDict.Add("@Tehsil_Id", (d.tehsilId as object).ToDbObj());
            paramDict.Add("@UnionCouncil_Id", (d.ucId as object).ToDbObj());
            paramDict.Add("@Ward_Id", (d.wardId as object).ToDbObj());
            paramDict.Add("@Complaint_Remarks", (d.complaintDetail as object).ToDbObj());
            paramDict.Add("@Agent_Comments", (d.agentComments as object).ToDbObj());

            paramDict.Add("@Agent_Id", (d.agentId as object).ToDbObj());
            paramDict.Add("@Complaint_Address", (d.complaintAddress as object).ToDbObj());
            paramDict.Add("@Business_Address", (d.businessAddress as object).ToDbObj());

            paramDict.Add("@Complaint_Status_Id", (d.complaintStatusId as object).ToDbObj());//If complaint is adding then set complaint status to 1 (Pending(Fresh) 
            paramDict.Add("@Created_Date", (d.complaintCreatedDate as object).ToDbObj());
            paramDict.Add("@Created_By", (d.complaintCreatedBy as object).ToDbObj());
            paramDict.Add("@Complaint_Assigned_Date", (d.complaintAssignedDate as object).ToDbObj());
            paramDict.Add("@Completed_Date", (d.complaintCompletedDate as object).ToDbObj());
            //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
            paramDict.Add("@Updated_By", (d.complaintUpdatedBy as object).ToDbObj());
            paramDict.Add("@Is_Deleted", (d.complaintIsDeleted as object).ToDbObj());
            paramDict.Add("@Date_Deleted", (d.complaintDeletedDate as object).ToDbObj());
            paramDict.Add("@Deleted_By", (d.complaintDeletedBy as object).ToDbObj());
            paramDict.Add("@ComplaintSrc", (d.complaintSrc as object).ToDbObj());
            paramDict.Add("@IsComplaintEditing", (d.isComplaintEditing as object).ToDbObj());

            //Personal Information
            paramDict.Add("@p_Person_id", (d.personId as object).ToDbObj());
            paramDict.Add("@Person_Name", (d.personName as object).ToDbObj());
            paramDict.Add("@Person_Father_Name", (d.personFatherName as object).ToDbObj());
            paramDict.Add("@Is_Cnic_Present", (d.personIsCnicPresent as object).ToDbObj());
            paramDict.Add("@Cnic_No", (d.personCnic as object).ToDbObj());
            paramDict.Add("@Gender", (d.personGender as object).ToDbObj());
            paramDict.Add("@Mobile_No", (d.personMobileNo as object).ToDbObj());
            paramDict.Add("@Secondary_Mobile_No", (d.personSecondaryNo as object).ToDbObj());
            paramDict.Add("@LandLine_No", (d.personLandlineNo as object).ToDbObj());
            paramDict.Add("@Person_Address", (d.personAddress as object).ToDbObj());
            paramDict.Add("@Email", (d.personEmail as object).ToDbObj());
            paramDict.Add("@Nearest_Place", (d.personNearestPlace as object).ToDbObj());
            paramDict.Add("@p_Province_Id", (d.personProvinceId as object).ToDbObj());
            paramDict.Add("@p_Division_Id", (d.personDivisionId as object).ToDbObj());
            paramDict.Add("@p_District_Id", (d.personDistrictId as object).ToDbObj());
            paramDict.Add("@p_Tehsil_Id", (d.personTehsilId as object).ToDbObj());
            paramDict.Add("@p_Town_Id", (d.personTownId as object).ToDbObj());
            paramDict.Add("@p_Uc_Id", (d.personUcId as object).ToDbObj());
            paramDict.Add("@p_Created_By", (d.personCreatedBy as object).ToDbObj());
            paramDict.Add("@p_Updated_By", (d.personUpdatedBy as object).ToDbObj());
            paramDict.Add("@IsProfileEditing", (d.personIsProfileEditing as object).ToDbObj());
            for (int i = 0; i < 10; i++)
            {
                if (i < d.listAssignmentModel.Count)
                {
                    paramDict.Add("@Dt" + (i + 1), (Utility.GetPropertyValueFromDynamic(d, string.Format("dt{0}", i + 1)) as object).ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), (Utility.GetPropertyValueFromDynamic(d, string.Format("srcId{0}", i + 1)) as object).ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), (Utility.GetPropertyValueFromDynamic(d, string.Format("userSrcId{0}", i + 1)) as object).ToDbObj());
                }
                else
                {
                    paramDict.Add("@Dt" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@SrcId" + (i + 1), (null as object).ToDbObj());
                    paramDict.Add("@UserSrcId" + (i + 1), (null as object).ToDbObj());
                }
            }

            paramDict.Add("@MaxLevel", (d.listAssignmentModel.Count as object).ToDbObj());

            paramDict.Add("@MinSrcId", (AssignmentHandler.GetMinSrcId(d.listAssignmentModel) as object).ToDbObj());
            paramDict.Add("@MaxSrcId", (AssignmentHandler.GetMaxSrcId(d.listAssignmentModel) as object).ToDbObj());

            paramDict.Add("@MinSrcIdDate", (AssignmentHandler.GetMinDate(d.listAssignmentModel) as object).ToDbObj());
            paramDict.Add("@MaxSrcIdDate", (AssignmentHandler.GetMaxDate(d.listAssignmentModel) as object).ToDbObj());
            return paramDict;
        }

        public static string GetPersonCnic(dynamic d)
        {
            // Person Cnic
            //d.personIsCnicPresent = true;
            //if (string.IsNullOrEmpty(d.personCnic))
            //{
            //    d.personIsCnicPresent = false;
            //}
            if (!d.personIsCnicPresent && !d.personIsProfileEditing)
            {
                decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
                string cnicStr = cnic.ToString();
                cnicStr = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
                d.personCnic = cnicStr;
            }

            return d.personCnic;
        }


        public static bool UpdateComplaint(DbComplaint complaint)
        {
            string queryStr = @"BEGIN TRY 
                                UPDATE PITB.Complaints
                                SET Complaint_Remarks = @complaintRemarks, District_Id = @districtId, District_Name = @districtName
                                WHERE Id = @complaintId

                                SELECT 'true' as Status;
                                END TRY

                                BEGIN CATCH
                                    
                                    SELECT 'false' as Status;

                                END CATCH";
            //queryStr = QueryHelper.GetParamsReplacedQuery(queryStr, new Dictionary<string, object>() { 
            //    { "@complaintId", complaintId },
            //    { "@complaintTime", complaintTime}
            //});
            Dictionary<string, object> dictParams = new Dictionary<string, object>() {
                { "@complaintRemarks",complaint.Complaint_Remarks },
                {"@complaintId",complaint.Id },
                {"@districtId", complaint.District_Id },
                {"@districtName", complaint.District_Name }
            };
            var result = DBHelper.GetDataTableByQueryString(queryStr, dictParams);
            return result.Rows[0][0].ToString() == "true" ? true : false;

        }

        public static dynamic ChangeComplaintTiming(dynamic d)
        {
            //dynamic dToReturn = new ExpandoObject();
            //int complaintId = Convert.ToInt32(d.complaintId);
            //float complaintTime = Convert.ToInt32(d.complaintTime);
            if (d.complaintTime != 0)
            {
                string queryStr = @" UPDATE PITB.Complaints
                                SET Retaining_Hrs = @complaintTime
                                WHERE id = @complaintId";
                //queryStr = QueryHelper.GetParamsReplacedQuery(queryStr, new Dictionary<string, object>() { 
                //    { "@complaintId", complaintId },
                //    { "@complaintTime", complaintTime}
                //});
                Dictionary<string, object> dictParams = new Dictionary<string, object>() {
                { "@complaintId", d.complaintId },
                { "@complaintTime", d.complaintTime}
            };
                DBHelper.GetDataTableByQueryString(queryStr, dictParams);
                //Config.CommandMessage commandMsg = new Config.CommandMessage(Config.CommandStatus.Success, "Complaint time changed successfully");
                return Utility.GetDynamicSuccessResponse("Complaint time changed successfully");
                //return commandMsg;
            }
            return Utility.GetDynamicSuccessResponse("Time is null");
        }

        public static dynamic ChangeComplaintCategory(dynamic d)
        {
            Dictionary<string, string> dictChangePerm = PermissionHandler.GetUserPermissionDict(Config.Permissions.ChangeCategoryInStakeholderDetail, d.userId);

            DbComplaintType dbComplaintType = DbComplaintType.GetById(d.categoryId);
            DbComplaintSubType dbComplaintSubtype = DbComplaintSubType.GetById(d.subcategoryId);

            //int categoryId = Convert.ToInt32(d.categoryId);
            //int subcategoryId = d.subcategoryId(d.subcategoryId);
            string queryStr = @" UPDATE PITB.Complaints
                                SET Complaint_Category = @categoryId,
                                Complaint_Category_Name = @categoryName,
                                Complaint_SubCategory = @subcategoryId,
                                Complaint_SubCategory_Name = @subcategoryName
                                WHERE id = @complaintId";

            Dictionary<string, object> dictParams = new Dictionary<string, object>() {
                { "@complaintId", d.complaintId },
                { "@categoryId", d.categoryId },
                { "@categoryName", dbComplaintType.Name },
                { "@subcategoryId",  d.subcategoryId},
                { "@subcategoryName", dbComplaintSubtype.Name }
            };
            DBHelper.GetDataTableByQueryString(queryStr, dictParams);
            if ((string)dictChangePerm.GetValue("allowStatusChange") == "true")
            {
                d.statusId = int.Parse(dictChangePerm.GetValue("statusIdOnChange").ToString());
                d.statusComments = dictChangePerm.GetValue("statusMsgOnChange").ToString();
                d.assignmentMatrixTag = PermissionHandler.GetUserPermissionValue(Config.Permissions.AssignmentMatrixTagOnStatusChange, d.userId);
                StatusHandler.ChangeStatusDynamic(d);
            }

            return Utility.GetDynamicSuccessResponse("Category successfully changed, ComplaintId = " + d.complaintId);
        }

        public static void PopulateAssigmentMatrixProperties(dynamic d)
        {
            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            List<AssignmentModel> listAssignmentModel = null;
            if (d.complaintType == (int)Config.ComplaintType.Complaint) // when there is complaint populate assignment matrix
            {
                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)d.subcategoryId);
                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)d.categoryId);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
                {
                    new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)d.provinceId),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)d.divisionId),
                    new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)d.districtId),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (int?)d.tehsilId),
                    new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)d.ucId),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)d.wardId)
                };


                listAssignmentModel = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment((DateTime)d.complaintCreatedDate,
                DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)d.campaignId, (int)d.categoryId, (int)d.subcategoryId, null, null, listHierarchyPair), catRetainingHours) /* nowDate,
				DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours*/);
            }
            else
            {
                listAssignmentModel = new List<AssignmentModel>();
            }

            for (int i = 0; i < 10; i++)
            {
                if (i < listAssignmentModel.Count)
                {
                    Utility.SetPropertyOfDynamic(d, string.Format("dt{0}", i + 1), listAssignmentModel[i].Dt);
                    Utility.SetPropertyOfDynamic(d, string.Format("srcId{0}", i + 1), listAssignmentModel[i].SrcId);
                    Utility.SetPropertyOfDynamic(d, string.Format("userSrcId{0}", i + 1), listAssignmentModel[i].UserSrcId);
                }
                else
                {
                    Utility.SetPropertyOfDynamic(d, string.Format("dt{0}", i + 1), null);
                    Utility.SetPropertyOfDynamic(d, string.Format("srcId{0}", i + 1), null);
                    Utility.SetPropertyOfDynamic(d, string.Format("userSrcId{0}", i + 1), null);
                }
            }

            d.maxLevel = listAssignmentModel.Count;
            d.minSrcId = AssignmentHandler.GetMinSrcId(listAssignmentModel);
            d.maxSrcId = AssignmentHandler.GetMaxSrcId(listAssignmentModel);
            d.minSrcIdDate = AssignmentHandler.GetMinDate(listAssignmentModel);
            d.maxSrcIdDate = AssignmentHandler.GetMaxDate(listAssignmentModel);
            d.listAssignmentModel = listAssignmentModel;
        }

        public static VmAddComplaint GetVmAddComplaintMerged(VmAddComplaint viewModel, int profileId = 0, int campaignId = 0)
        {
            viewModel.ComplaintVm.Compaign_Id = campaignId;
            viewModel.PersonalInfoVm.Person_id = profileId;

            viewModel.ComplaintVm.ListOfProvinces = DbProvince.AllProvincesList();

            int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Complaint);
            viewModel.ComplaintVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.ComplaintVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);

            //viewModel.ComplaintVm.ListOfDepartment = DbDepartment.GetByCampaignId(campaignId);
            viewModel.ComplaintVm.hasDepartment = (viewModel.ComplaintVm.ListOfDepartment != null &&
                                                   viewModel.ComplaintVm.ListOfDepartment.Count > 0) ? true : false;
            //viewModel.ComplaintVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignId(campaignId);

            viewModel.ComplaintVm.DynamicFieldsCount = 0;
            //viewModel.ComplaintVm.ListDynamicFields = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId);

            List<VmDynamicField> listVmDynamic = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId).OrderBy(n => n.Priority).ToList();
            if (listVmDynamic != null && listVmDynamic.Count > 0)
            {
                viewModel.ComplaintVm.DynamicFieldsCount = listVmDynamic.Count;
                viewModel.ComplaintVm.MinDynamicFormPriority = listVmDynamic.First().Priority;
                viewModel.ComplaintVm.MaxDynamicFormPriority = listVmDynamic.Last().Priority;


                List<VmDynamicTextbox> listDynamicTextBox = new List<VmDynamicTextbox>();
                List<VmDynamicLabel> listDynamicLabel = new List<VmDynamicLabel>();
                List<VmDynamicDropDownList> listDynamicDropdown = new List<VmDynamicDropDownList>();
                List<VmDynamicDropDownListServerSide> listDynamicDropdownServerSide = new List<VmDynamicDropDownListServerSide>();
                List<VmCheckBox> listDynamicCheckBox = new List<VmCheckBox>();
                foreach (VmDynamicField dfField in listVmDynamic)
                {
                    switch (dfField.ControlType)
                    {
                        case Config.DynamicControlType.TextBox:
                            listDynamicTextBox.Add(dfField as VmDynamicTextbox);
                            break;

                        case Config.DynamicControlType.Label:
                            listDynamicLabel.Add(dfField as VmDynamicLabel);
                            break;

                        case Config.DynamicControlType.DropDownList:
                            listDynamicDropdown.Add(dfField as VmDynamicDropDownList);
                            break;

                        case Config.DynamicControlType.DropDownListServerSideSearchable:
                            listDynamicDropdownServerSide.Add(dfField as VmDynamicDropDownListServerSide);
                            break;
                        case Config.DynamicControlType.CheckBox:
                            listDynamicCheckBox.Add(dfField as VmCheckBox);
                            break;
                    }
                }
                viewModel.ComplaintVm.ListDynamicTextBox = listDynamicTextBox;
                viewModel.ComplaintVm.ListDynamicLabel = listDynamicLabel;
                viewModel.ComplaintVm.ListDynamicDropDown = listDynamicDropdown;
                viewModel.ComplaintVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;
                viewModel.ComplaintVm.listDynamicCheckBox = listDynamicCheckBox;
                //viewModel.ComplaintVm.PopulateListDynamicFields();


                //viewModel.SuggestionVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.SuggestionVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.SuggestionVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                //viewModel.InquiryVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.InquiryVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.InquiryVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;
            }

            viewModel.HardCopyComplaintIntoSuggestionAndInquiry();
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            viewModel.PersonalInfoVm = Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetPersonInformationByPersonId(profileId));

            // Copy complaint into suggestion
            Mapper.CreateMap<VmComplaint, VmSuggestion>();
            viewModel.SuggestionVm = Mapper.Map<VmSuggestion>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Suggestion);
            viewModel.SuggestionVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.SuggestionVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);
            //viewModel.SuggestionVm.ListOfDepartment = DbDepartment(campaignId);


            // Copy complaint into inquiry
            Mapper.CreateMap<VmComplaint, VmInquiry>();
            viewModel.InquiryVm = Mapper.Map<VmInquiry>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Inquiry);
            viewModel.InquiryVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.InquiryVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);

            return viewModel;
        }

        public static VmComplaintDetail GetComplaintDetail(DbComplaint dbComplaint /*int complaintId*/)
        {
            //List<DbComplaint> listComplaint = DbComplaint.GetListByComplaintId(complaintId);
            List<DbDynamicComplaintFields> listDynamicFields = DbDynamicComplaintFields.GetByComplaintId(dbComplaint.Id/*complaintId*/);
            VmComplaintDetail vmComplaintDetail = VmComplaintDetail.GetComplaintDetail(/*listComplaint.First()*/dbComplaint, listDynamicFields);
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            DbPersonInformation dbPersonIfo =
                DbPersonInformation.GetPersonInformationByPersonId((int)dbComplaint.Person_Id/*(int)listComplaint.FirstOrDefault().Person_Id)*/);
            vmComplaintDetail.vmPersonlInfo = Mapper.Map<VmPersonalInfo>(dbPersonIfo);
            if (dbPersonIfo.Is_Cnic_Present != true)
            {
                vmComplaintDetail.vmPersonlInfo.Cnic_No = "N/A";
            }
            vmComplaintDetail.currentStatusStr = Utility.GetAlteredStatus(Convert.ToInt32(vmComplaintDetail.Compaign_Id), vmComplaintDetail.currentStatusStr);
            return vmComplaintDetail;
        }


        public static VmStakeholderComplaintDetail GetStakeholderComplaintDetail(int complaintId, int userId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            DbUsers dbUser = DbUsers.GetActiveUser(userId);
            List<DbComplaint> listComplaint = DbComplaint.GetListByComplaintId(complaintId);
            DbComplaint dbComplaint = listComplaint.First();

            List<DbDynamicComplaintFields> listDynamicFields = DbDynamicComplaintFields.GetByComplaintId(complaintId);
            VmStakeholderComplaintDetail vmComplaintDetail = new VmStakeholderComplaintDetail();
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            vmComplaintDetail.GetComplaintDetail(listComplaint.First(), userId, listDynamicFields, detailType);

            vmComplaintDetail.vmFileModel = FileHandler.GetVmFileModel(complaintId, (int)Config.AttachmentReferenceType.Add, complaintId);
            vmComplaintDetail.hasStatusHistory = (StatusHandler.GetComplaintStatusChangeHistoryTableList(complaintId).Count > 0) ? true : false;
            vmComplaintDetail.hasTransferHistory = TransferHandler.GetTransferedHistoryStatus(complaintId);
            //vmComplaintDetail.currDetailType = detailType;

            if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStakeholderEscalationInDetail, (int)dbUser.User_Id, (Config.Roles)dbUser.Role_Id))
            {
                vmComplaintDetail.ListEscalationModel =
                    EscalationHandler.GetEscalationListOfComplaint(listComplaint.First());
            }
            //DbStatus.GetByCampaignIdAndCategoryId()

            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            DbPersonInformation dbPersonIfo =
                DbPersonInformation.GetPersonInformationByPersonId((int)listComplaint.FirstOrDefault().Person_Id);
            vmComplaintDetail.vmPersonlInfo = Mapper.Map<VmPersonalInfo>(dbPersonIfo);

            if (dbPersonIfo != null && dbPersonIfo.Is_Cnic_Present != true)
            {
                vmComplaintDetail.vmPersonlInfo.Cnic_No = "N/A";
            }
            vmComplaintDetail.Complaint_SubCategory = listComplaint.First().Complaint_SubCategory;

            //if (dbComplaint.Dt1 != null && dbComplaint.Dt2 != null)
            //{
            //    //DateTime dt1 = (DateTime)listComplaint.First().Dt1 ;
            //    //DateTime createdDate = (DateTime)listComplaint.First().Created_Date;
            //    //TimeSpan dtChange = (dt1 - createdDate).Duration();
            //    //int asd = (dt1 - createdDate).TotalHours+;
            //    vmComplaintDetail.ComplaintEscalationTimeInHrs = (float)((DateTime)dbComplaint.Dt2 - (DateTime)dbComplaint.Dt1).Duration().TotalHours;

            //}

            if (dbComplaint.Dt1 == null)
            {
                vmComplaintDetail.ComplaintEscalationTimeInHrs = (float)DbComplaintType.GetById((int)dbComplaint.Complaint_Category).RetainingHours;
            }
            else
            {
                vmComplaintDetail.ComplaintEscalationTimeInHrs = Math.Abs(((TimeSpan)(Convert.ToDateTime(dbComplaint.Dt1) - Convert.ToDateTime(dbComplaint.Created_Date))).Hours);
            }


            vmComplaintDetail.VmDistrictList = DbDistrict.GetAllDistrictList().Select(district => new System.Web.Mvc.SelectListItem { Text = district.District_Name, Value = district.District_Id.ToString(), Selected = district.District_Id.ToString() == vmComplaintDetail.District_Id.ToString() ? true : false }).ToList();

            return vmComplaintDetail;
        }


        public static Config.CommandMessage CallStatusSubmit(VmCallSubmit vmCallSubmit)
        {
            try
            {
                int campaignId = (int)vmCallSubmit.Campaign_Id;
                int complaintId = (int)vmCallSubmit.Complaint_Id;
                var callstatusid = vmCallSubmit.callStatusId as int?;
                string callcomments = vmCallSubmit.CallComments;
                DateTime calldatetime = DateTime.Now;
                int callbyuserid = AuthenticationHandler.GetCookie().UserId;
                if (callstatusid != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    List<DbComplaintCallLogs> dbComplaintCallLogs = db.DbComplaint_Call_Logs.Where(m => m.complaint_id == complaintId).ToList();
                    if (dbComplaintCallLogs.Count > 0)
                    {
                        foreach (var item in dbComplaintCallLogs)
                        {
                            item.is_currently_active = false;
                        }
                    }
                    var record = new DbComplaintCallLogs() { call_by_userid = callbyuserid, call_time = calldatetime, status_Id = (int)callstatusid, comments = callcomments, complaint_id = complaintId, is_currently_active = true };
                    db.DbComplaint_Call_Logs.Add(record);

                    var dbComplaint = DbComplaint.GetByComplaintId(db, complaintId);
                    dbComplaint.Callback_Count = dbComplaintCallLogs.Count + 1;
                    dbComplaint.Callback_Status = (int)callstatusid;
                    dbComplaint.Callback_Comment = callcomments;
                    db.SaveChanges();
                    return new Config.CommandMessage(Config.CommandStatus.Success, "Call Status for " + campaignId + "-" + complaintId + " has been submitted Successfully!!");
                }
                return new Config.CommandMessage(Config.CommandStatus.Failure, "Please select call status.");
            }
            catch (Exception ex)
            {
                return new Config.CommandMessage(Config.CommandStatus.Failure, "An Error has occured");
                throw;
            }
        }
        public static Config.CommandMessage ChangeComplaintTypeAndSubType(VmCategoryChange vmCategoryChange)
        {
            try
            {
                int campaignId = (int)vmCategoryChange.Compaign_Id;
                int complaintId = (int)vmCategoryChange.ComplaintId;
                int selectedCategoryId = (int)vmCategoryChange.selectedComplaintCategory;
                int selectedSubcategoryId = (int)vmCategoryChange.selectedComplaintSubCategory;
                int selectedDepartmentId = (int)vmCategoryChange.DepartmentId;

                if (selectedCategoryId != vmCategoryChange.Complaint_Category ||
                    selectedSubcategoryId != vmCategoryChange.Complaint_SubCategory)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId, db);
                    db.DbComplaints.Attach(dbComplaint);
                    StatusHandler.ResetComplaintStatus(db, dbComplaint);

                    dbComplaint.Complaint_Category = selectedCategoryId;
                    db.Entry(dbComplaint).Property(n => n.Complaint_Category).IsModified = true;

                    dbComplaint.Complaint_Category_Name = DbComplaintType.GetById((int)dbComplaint.Complaint_Category).Name;
                    db.Entry(dbComplaint).Property(n => n.Complaint_Category_Name).IsModified = true;


                    dbComplaint.Complaint_SubCategory = selectedSubcategoryId;
                    db.Entry(dbComplaint).Property(n => n.Complaint_SubCategory).IsModified = true;

                    dbComplaint.Complaint_SubCategory_Name = DbComplaintSubType.GetById((int)dbComplaint.Complaint_SubCategory).Name;
                    db.Entry(dbComplaint).Property(n => n.Complaint_SubCategory_Name).IsModified = true;

                    dbComplaint.Department_Name = DbDepartment.GetByDepartmentId(selectedDepartmentId).Name;
                    dbComplaint.Department_Id = selectedDepartmentId;

                    List<AssignmentModel> assignmentModelList = AssignmentHandler.GetAssignmnetModelByCampaignCategorySubCategory(campaignId, selectedCategoryId, selectedSubcategoryId, true);
                    for (int i = 0; i < 10; i++)
                    {
                        if (i < assignmentModelList.Count)
                        {
                            Utility.SetPropertyThroughReflection(dbComplaint, "Dt" + (i + 1), assignmentModelList[i].Dt);
                            Utility.SetPropertyThroughReflection(dbComplaint, "SrcId" + (i + 1), assignmentModelList[i].SrcId);
                            Utility.SetPropertyThroughReflection(dbComplaint, "UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId);
                        }
                        else
                        {
                            Utility.SetPropertyThroughReflection(dbComplaint, "Dt" + (i + 1), (DateTime?)null);
                            Utility.SetPropertyThroughReflection(dbComplaint, "SrcId" + (i + 1), (int?)null);
                            Utility.SetPropertyThroughReflection(dbComplaint, "UserSrcId" + (i + 1), (int?)null);
                        }
                        db.Entry(dbComplaint).Property("Dt" + (i + 1)).IsModified = true;
                        db.Entry(dbComplaint).Property("SrcId" + (i + 1)).IsModified = true;
                        db.Entry(dbComplaint).Property("UserSrcId" + (i + 1)).IsModified = true;
                    }
                    db.SaveChanges();
                    return new Config.CommandMessage(Config.CommandStatus.Success, "Your Complaint " + dbComplaint.Compaign_Id + "-" + dbComplaint.Id + " Category/Subcateogry has been changed Successfully!!");
                }
                return new Config.CommandMessage(Config.CommandStatus.Failure, "Change category other than this");
            }
            catch (Exception ex)
            {
                return new Config.CommandMessage(Config.CommandStatus.Failure, "An Error has occured");
                throw;
            }
        }

        public static Config.CommandMessage AddComplaint(VmAddComplaint vm, bool isProfileEditing, bool isComplaintEditing)
        {
            #region Add Complaint Section

            DateTime nowDate = DateTime.Now;

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();

            VmPersonalInfo vmPersonalInfo = vm.PersonalInfoVm;
            VmComplaint vmComplaint = vm.ComplaintVm;
            VmSuggestion vmSuggestion = vm.SuggestionVm;
            VmInquiry vmInquiry = vm.InquiryVm;

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

            // complaint info
            paramDict.Add("@Id", -1);
            paramDict.Add("@Person_Id", vmPersonalInfo.Person_id.ToDbObj());

            if (string.IsNullOrEmpty(vm.PersonalInfoVm.Cnic_No))
            {
                vmPersonalInfo.IsCnicPresent = false;
            }

            if (!vmPersonalInfo.IsCnicPresent && !isProfileEditing)
            {
                decimal cnic = DbUniqueIncrementor.GetUniqueValue(Config.UniqueIncrementorTag.Cnic);
                string cnicStr = cnic.ToString();
                cnicStr = Utility.PaddLeft(cnicStr, Config.CnicLength, '0');
                vmPersonalInfo.Cnic_No = cnicStr;
            }

            switch (vm.currentComplaintTypeTab)
            {
                case VmAddComplaint.TabComplaintCombined:
                case VmAddComplaint.TabComplaint:
                    paramDict.Add("@DepartmentId", vmComplaint.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Complaint);
                    paramDict.Add("@Complaint_Category", vmComplaint.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmComplaint.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmComplaint.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmComplaint.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmComplaint.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmComplaint.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmComplaint.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmComplaint.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmComplaint.Agent_Comments.ToDbObj());
                    break;
                case VmAddComplaint.TabSuggestionCombined:
                case VmAddComplaint.TabSuggestion:
                    paramDict.Add("@DepartmentId", vmSuggestion.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Suggestion);
                    paramDict.Add("@Complaint_Category", vmSuggestion.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmSuggestion.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmSuggestion.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vmSuggestion.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmSuggestion.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmSuggestion.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmSuggestion.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmSuggestion.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmSuggestion.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmSuggestion.Agent_Comments.ToDbObj());
                    break;

                case VmAddComplaint.TabInquiryVmCombined:
                case VmAddComplaint.TabInquiryVm:
                    paramDict.Add("@DepartmentId", vmInquiry.departmentId.ToDbObj());
                    paramDict.Add("@Complaint_Type", Config.ComplaintType.Inquiry);
                    paramDict.Add("@Complaint_Category", vmInquiry.Complaint_Category.ToDbObj());
                    paramDict.Add("@Complaint_SubCategory", vmInquiry.Complaint_SubCategory.ToDbObj());
                    paramDict.Add("@Compaign_Id", vmComplaint.Compaign_Id.ToDbObj());
                    paramDict.Add("@Province_Id", vmInquiry.Province_Id.ToDbObj());
                    paramDict.Add("@Division_Id", DbDistrict.GetById((int)vmInquiry.District_Id).Division_Id.ToDbObj());
                    paramDict.Add("@District_Id", vmInquiry.District_Id.ToDbObj());
                    paramDict.Add("@Tehsil_Id", vmInquiry.Tehsil_Id.ToDbObj());
                    paramDict.Add("@UnionCouncil_Id", vmInquiry.UnionCouncil_Id ?? 0);
                    paramDict.Add("@Ward_Id", vmInquiry.Ward_Id ?? 0);
                    paramDict.Add("@Complaint_Remarks", vmInquiry.Complaint_Remarks.ToDbObj());
                    paramDict.Add("@Agent_Comments", vmInquiry.Agent_Comments.ToDbObj());
                    break;

            }


            paramDict.Add("@Agent_Id", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Complaint_Address", vmComplaint.Complaint_Address.ToDbObj());
            paramDict.Add("@Business_Address", vmComplaint.Business_Address.ToDbObj());

            object complaintStatusId = null;
            if (isComplaintEditing)
            {
                complaintStatusId = vmComplaint.Complaint_Status_Id.ToDbObj();
            }
            else
            {
                List<string> lisKeys = new List<string>()
                {
                    string.Format("Type::Config___Module::ComplaintLaunchStatus___CampaignId::{0}",vmComplaint.Compaign_Id),
                    "Type::Config___Module::ComplaintLaunchStatus"
                };
                complaintStatusId = int.Parse(ConfigurationHandler.GetConfiguration(lisKeys,
                    ((int)paramDict["@Complaint_Type"]).ToString()));
            }

            //paramDict.Add("@Complaint_Status_Id", (isComplaintEditing) ? vmComplaint.Complaint_Status_Id.ToDbObj() : Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
            paramDict.Add("@Complaint_Status_Id", complaintStatusId);//If complaint is adding then set complaint status to 1 (Pending(Fresh) 
            paramDict.Add("@Created_Date", nowDate.ToDbObj());
            paramDict.Add("@Created_By", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Complaint_Assigned_Date", (null as object).ToDbObj());
            paramDict.Add("@Completed_Date", (null as object).ToDbObj());
            //paramDict.Add("@Updated_Date", (null as object).ToDbObj());
            paramDict.Add("@Updated_By", cmsCookie.UserId.ToDbObj());
            paramDict.Add("@Is_Deleted", false);
            paramDict.Add("@Date_Deleted", (null as object).ToDbObj());
            paramDict.Add("@Deleted_By", (null as object).ToDbObj());
            paramDict.Add("@ComplaintSrc", ((int)Config.ComplaintSource.Agent).ToDbObj());

            paramDict.Add("@IsComplaintEditing", isComplaintEditing);



            //Personal Information
            paramDict.Add("@p_Person_id", vmPersonalInfo.Person_id.ToDbObj());
            paramDict.Add("@Person_Name", vmPersonalInfo.Person_Name.ToDbObj());
            paramDict.Add("@Person_Father_Name", vmPersonalInfo.Person_Father_Name.ToDbObj());
            paramDict.Add("@Is_Cnic_Present", vmPersonalInfo.IsCnicPresent.ToDbObj());
            paramDict.Add("@Cnic_No", vmPersonalInfo.Cnic_No.ToDbObj());
            paramDict.Add("@Gender", vmPersonalInfo.Gender.ToDbObj());
            paramDict.Add("@Mobile_No", vmPersonalInfo.Mobile_No.ToDbObj());
            paramDict.Add("@Secondary_Mobile_No", vmPersonalInfo.Secondary_Mobile_No.ToDbObj());
            paramDict.Add("@LandLine_No", vmPersonalInfo.LandLine_No.ToDbObj());
            paramDict.Add("@Person_Address", vmPersonalInfo.Person_Address.ToDbObj());
            paramDict.Add("@Email", vmPersonalInfo.Email.ToDbObj());
            paramDict.Add("@Nearest_Place", vmPersonalInfo.Nearest_Place.ToDbObj());
            paramDict.Add("@p_Province_Id", vmPersonalInfo.Province_Id.ToDbObj());
            paramDict.Add("@p_Division_Id", vmPersonalInfo.Division_Id.ToDbObj());
            paramDict.Add("@p_District_Id", vmPersonalInfo.District_Id.ToDbObj());
            paramDict.Add("@p_Tehsil_Id", vmPersonalInfo.Tehsil_Id.ToDbObj());
            paramDict.Add("@p_Town_Id", vmPersonalInfo.Town_Id.ToDbObj());
            paramDict.Add("@p_Uc_Id", vmPersonalInfo.Uc_Id ?? 0);
            paramDict.Add("@p_Created_By", cmsCookie.UserId);
            paramDict.Add("@p_Updated_By", cmsCookie.UserId);

            paramDict.Add("@IsProfileEditing", isProfileEditing);

            //info is kept anonymous
            paramDict.Add("@IsAnonymous", vmComplaint.Is_Anonymous);


            float catRetainingHours = 0;
            float? subcatRetainingHours = 0;
            int categoryId = -1;
            //Config.CategoryType cateogryType = Config.CategoryType.Main;

            //AssignmentMatrix
            List<AssignmentModel> assignmentModelList = null;
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined) // when there is complaint populate assignment matrix
            {
                subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId((int)vmComplaint.Complaint_SubCategory);
                if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
                {
                    catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)vmComplaint.Complaint_Category);
                    //cateogryType = Config.CategoryType.Main;
                }
                else
                {
                    catRetainingHours = (float)subcatRetainingHours;
                    //cateogryType = Config.CategoryType.Sub;
                }

                List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
                {
                    new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)paramDict["@Province_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)paramDict["@Division_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)paramDict["@District_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (int?)paramDict["@Tehsil_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)paramDict["@UnionCouncil_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)paramDict["@Ward_Id"])
                };


                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours) /* nowDate,
				DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours*/);
            }
            else
            {
                assignmentModelList = new List<AssignmentModel>();
            }
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

            #endregion
            DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
            int complaintId = Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]);
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.ComplaintVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabSuggestion || vm.currentComplaintTypeTab == VmAddComplaint.TabSuggestionCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.SuggestionVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabInquiryVm || vm.currentComplaintTypeTab == VmAddComplaint.TabInquiryVmCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.InquiryVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }

            if (cm.Status == Config.CommandStatus.Success && (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined)) // send message on complaint launch
            {

                if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(vmPersonalInfo.Mobile_No,
                        (int)vmComplaint.Compaign_Id, Convert.ToInt32(cm.Value.Split('-')[1]),
                        (int)vmComplaint.Complaint_Category, vmPersonalInfo.Person_Name);
                    string msgToText = null;
                    if (vmComplaint.Compaign_Id == (int)Config.Campaign.DcoOffice)
                    {
                        //DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId); 
                        DbComplaintType dbComplaintType = DbComplaintType.GetById((int)dbComplaint.Complaint_Category);
                        DbComplaintSubType dbComplaintSubType = DbComplaintSubType.GetById((int)dbComplaint.Complaint_SubCategory);
                        TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
                        DateTime dueDate = DateTime.Now.Add(span);
                        msgToText = "Chief Minister's Complaint Center Alert\n\n" +
                        "Dear Concerned, Complaint no." + complaintId + " has been assigned to you on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString() + ".\n" +
                        "Category: " + dbComplaintType.Name + "\n" +
                        "Subcategory: " + dbComplaintSubType.Name + "\n" +
                        "Please resolve by " + dueDate.ToString() + "\n" +
                        "To view details, please visit: https://crm.punjab.gov.pk";
                    }
                    else
                    {
                        msgToText = null;
                    }

                    //TextMessageHandler.SendSms(new Dictionary<string, object>()
                    //{
                    //    {"dbComplaint",DbComplaint.GetByComplaintId(complaintId)},
                    //    {"smsText",msgToText},
                    //    {"campaignId",vmComplaint.Compaign_Id},
                    //    {"tagId","AddComplaint"},
                    //});
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId), msgToText);
                }
            }

            //db.DbDynamicComplaintFields.Add();
            //SendMessage(Convert.ToInt32(cm.Value.Split('-')[1]), (int)Config.ComplaintStatus.PendingFresh);
            return cm;
        }

        public static void PushComplaintToCNFP(DbComplaint dbComplaint, DBContextHelperLinq db)
        {
            try
            {
                string url = "https://dashboard.cfmp.punjab.gov.pk/index.php/punjab_api/syncCMCCData";
                RequestModel.CNFP.PostComplaints postComplaints = new RequestModel.CNFP.PostComplaints(Config.CNPF_ACCESS_TOKEN);
                RequestModel.CNFP.PostComplaints.Complaint complaintModel = new RequestModel.CNFP.PostComplaints.Complaint(dbComplaint);
                postComplaints.complaintData.Add(complaintModel);
                ResponseModel.CNFP.PostComplaintsResponse cnfpResp = APIHelper.HttpClientGetResponse<ResponseModel.CNFP.PostComplaintsResponse, RequestModel.CNFP.PostComplaints>(
                    url, postComplaints, null);

                if (cnfpResp.Status == "Success")
                {
                    db.DbComplaints.Attach(dbComplaint);
                    dbComplaint.CNFP_Is_FeedbackGiven = false;
                    db.Entry(dbComplaint).Property(n => n.CNFP_Is_FeedbackGiven).IsModified = true;
                    //    dbComplaint.Complaint_Category = selectedCategoryId;
                    //db.Entry(dbComplaint).Property(n => n.Complaint_Category).IsModified = true;
                    dbComplaint.CNFP_Feedback_Ref_Id = -1;
                    db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Ref_Id).IsModified = true;

                    dbComplaint.CNFP_Feedback_Id = -1;
                    db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Id).IsModified = true;

                    dbComplaint.CNFP_Feedback_Value = Enum.GetName(typeof(Config.FeedbackStatuses), -1);
                    db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Value).IsModified = true;

                    dbComplaint.CNFP_Feedback_Comments = null;
                    db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Comments).IsModified = true;
                }
            }
            catch (Exception ex)
            {
            }
        }


        /*
        private static void SaveDyamicFieldsInDb(VmComplaint vmComplaint, int complaintId)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DbDynamicComplaintFields dbDf = null;

            //for TextBox
            if (vmComplaint.ListDynamicTextBox != null && vmComplaint.ListDynamicTextBox.Count > 0)
            {
                foreach (VmDynamicTextbox vmTxtBox in vmComplaint.ListDynamicTextBox)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                    dbDf.CategoryTypeId = -1;
                    dbDf.ControlId = vmTxtBox.ControlId;
                    dbDf.FieldName = vmTxtBox.FieldName;
                    dbDf.FieldValue = vmTxtBox.FieldValue.Trim();
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }


            //for Dropdown List
            if (vmComplaint.ListDynamicDropDown != null && vmComplaint.ListDynamicDropDown.Count > 0)
            {
                int selectedId = 0;
                string selectedValue = null;
                string[] tempStrArr = null;

                foreach (VmDynamicDropDownList vmDropDownList in vmComplaint.ListDynamicDropDown)
                {
                    dbDf = new DbDynamicComplaintFields();
                    dbDf.ComplaintId = complaintId;
                    dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                    dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);
                    tempStrArr = vmDropDownList.SelectedItemId.Split( new string[] { Config.Separator }, StringSplitOptions.None);
                    dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                    dbDf.ControlId = vmDropDownList.ControlId;
                    dbDf.FieldName = vmDropDownList.FieldName;
                    dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }
            db.SaveChanges();
            //DBContextHelperLinq dbContext = new 
            //using (var db = new DBContextHelperLinq())
            //{
            //    return db.DbProvinces.AsNoTracking().OrderBy(m => m.Province_Name).ToList();
            //}
        }
        */

        private static void SendMessage(int complaintId, int statusId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>
            {
                {"@ComplaintId", complaintId},
                {"@StatusId", statusId}
            };

            DataTable dt = DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Message_By_Campaign]", paramDict);
            if (!(bool)dt.Rows[0][0]) return; //Return from function if campaign SMS facility is not enabled
            var row = dt.Rows[0];

            if (row[1] != DBNull.Value)
            {
                //If campaign message is defined for that particular status type


                //preparing data for sms
                int campaignId = (int)row[3];
                string messageText = row[1].ToString();
                messageText = messageText.Replace("%", campaignId + "-" + complaintId);

                string mobileNumber = row[2].ToString();
                int profileId = (int)row[4];
                //sending text message
                //new TextMessageHandler(mobileNumber, messageText, 0, campaignId, profileId).SendMessage();

            }
            //@MessageText AS [Message] ,
            //@MobileNo AS MobileNo,
            //@CampaignId AS CampaignId,
            //@ProfileId AS ProfileId
        }


        /*
        public static List<VmAgentComplaintListing> GetStakeholderComplaintListings(string fromDate, string toDate, string campaign, string category, string statuses)
        {
            List<VmAgentComplaintListing> listOfComplaints = new List<VmAgentComplaintListing>();
            switch (new AuthenticationHandler().CmsCookie.Role)
            {
                case Config.Roles.Agent:
                    listOfComplaints = GetComplaintsOfAgents(fromDate, toDate, campaign);
                    break;
                case Config.Roles.AgentSuperVisor:
                    listOfComplaints = GetComplaintsAllComplaintsSupervisor(fromDate, toDate, campaign);
                    break;

            }
            return listOfComplaints;
        }
        */
        public static List<VmAgentComplaintListing> GetComplaintsOfAgents(string fromDate, string toDate, string campaign)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@UserId", new AuthenticationHandler().CmsCookie.UserId);

            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Complaints_Agent]", paramDict).ToList<VmAgentComplaintListing>();
        }

        public static List<VmStakeholderComplaintListing> GetComplaintsOfStakeholder(string fromDate, string toDate, string campaign, string category, string complaintStatuses)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@Category", category.ToDbObj());
            paramDict.Add("@Status", complaintStatuses.ToDbObj());
            paramDict.Add("@UserHierarchyId", Convert.ToInt32(cookie.Hierarchy_Id).ToDbObj());
            paramDict.Add("@ProvinceId", Convert.ToInt32(cookie.ProvinceId).ToDbObj());

            paramDict.Add("@DistrictId", Convert.ToInt32(cookie.DistrictId).ToDbObj());

            paramDict.Add("@Tehsil", Convert.ToInt32(cookie.TehsilId).ToDbObj());

            paramDict.Add("@UserId", cookie.UserId.ToDbObj());

            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints]", paramDict).ToList<VmStakeholderComplaintListing>();
        }

        public static DataTable GetComplaintsOfStakeholderServerSide(string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@StartRow", (dtParams.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtParams.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", dtParams.ListOrder[0].columnName.ToDbObj());
            paramDict.Add("@OrderByDirection", dtParams.ListOrder[0].sortingDirectionStr.ToDbObj());

            paramDict.Add("@WhereOfMultiSearch", dtParams.WhereOfMultiSearch.ToDbObj());

            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@Category", category.ToDbObj());
            paramDict.Add("@Status", complaintStatuses.ToDbObj());
            paramDict.Add("@TransferedStatus", commaSeperatedTransferedStatus.ToDbObj());
            paramDict.Add("@ComplaintType", (Convert.ToInt32(complaintType)).ToDbObj());
            paramDict.Add("@UserHierarchyId", Convert.ToInt32(cookie.Hierarchy_Id).ToDbObj());
            paramDict.Add("@UserDesignationHierarchyId", Convert.ToInt32(cookie.User_Hierarchy_Id).ToDbObj());
            paramDict.Add("@ListingType", Convert.ToInt32(listingType).ToDbObj());
            paramDict.Add("@ProvinceId", (cookie.ProvinceId).ToDbObj());
            paramDict.Add("@DivisionId", (cookie.DivisionId).ToDbObj());
            paramDict.Add("@DistrictId", (cookie.DistrictId).ToDbObj());

            paramDict.Add("@Tehsil", (cookie.TehsilId).ToDbObj());
            paramDict.Add("@UcId", (cookie.UcId).ToDbObj());
            paramDict.Add("@WardId", (cookie.WardId).ToDbObj());

            paramDict.Add("@UserId", cookie.UserId.ToDbObj());
            //paramDict.Add("@UserCategoryId1", cookie.UserCategoryId1.ToDbObj());
            //paramDict.Add("@UserCategoryId2", cookie.UserCategoryId2.ToDbObj());
            //paramDict.Add("@CheckIfExistInSrcId", 1);
            //paramDict.Add("@CheckIfExistInUserSrcId", 1);
            paramDict.Add("@SpType", spType);

            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ServerSide]", paramDict);
        }

        /*
        public static List<VmAgentComplaintListing> GetComplaintsAllComplaintsSupervisor(DataTableParamsModel dtParams, string fromDate, string toDate, string campaign, Config.ComplaintType complaintType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@StartRow", (dtParams.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtParams.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", dtParams.ListOrder[0].columnName.ToDbObj());
            paramDict.Add("@OrderByDirection", dtParams.ListOrder[0].sortingDirectionStr.ToDbObj());
            paramDict.Add("@WhereOfMultiSearch", dtParams.WhereOfMultiSearch.ToDbObj());
			
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@ComplaintType", Convert.ToInt32(complaintType).ToDbObj());
            paramDict.Add("@UserId", cookie.UserId.ToDbObj());
            paramDict.Add("@RoleId", cookie.Role.ToDbObj());


            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Complaints_Supervisor]", paramDict).ToList<VmAgentComplaintListing>();
        }
        */

        public static void OnFollowupSubmit(int complaintId, string followupComments)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            DateTime currDateTime = DateTime.Now;
            MakeLastLogOfFollowupInactive(complaintId, db);
            DbComplaintFollowupLogs dbFollowupLog = SaveComplaintFollowupInLog(complaintId, Config.FollowupId,
                currDateTime, followupComments, db);
            db.SaveChanges();

            int statusLogId = dbFollowupLog.Id;
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(db, complaintId);
            dbComplaint.FollowupId = Config.FollowupId;
            dbComplaint.FollowupComment = followupComments;
            dbComplaint.FollowupCount = Convert.ToInt32(dbComplaint.FollowupCount) + 1;
            db.DbComplaints.Add(dbComplaint);
            db.Entry(dbComplaint).State = EntityState.Modified;

            db.SaveChanges();


            /*
           *
           * ***************** SEND SMS TO STAKEHOLDER WHENEVER A FOLLOWUP SUBMITTED FOR DCO-OFFICE CAMPAIGN ****************
           *   S T A R T
           */
            if (dbComplaint.Compaign_Id == (int)Config.Campaign.DcoOffice)
            {
                if (dbComplaint.Complaint_Status_Id == (int)Config.ComplaintStatus.Resolved ||
                    dbComplaint.Complaint_Status_Id == (int)Config.ComplaintStatus.Irrelevant)
                {
                    //Do nothing
                }
                else
                {
                    string textMessage = "Action Required\n\n" +

                                         "Dear Concerned, citizen has inquired about complaint no. {0} received on Chief Minister’s Complaint Center and was assigned to you on {1}. Please Resolve";

                    textMessage = string.Format(textMessage, complaintId,
                        dbComplaint.Created_Date.Value.ToString("dd MM, yyyy"));


                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(
                        dbComplaint,
                        textMessage);



                }
            }


            /***** E N D ****/






        }

        private static void MakeLastLogOfFollowupInactive(int complaintId, DBContextHelperLinq db)
        {
            //DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaintFollowupLogs followupChangeLog = DbComplaintFollowupLogs.GetLastStatusChangeOfParticularComplaint(complaintId, db);
            if (followupChangeLog != null)
            {
                followupChangeLog.IsCurrentlyActive = false;
                db.DbComplaintFollowupLogs.Add(followupChangeLog);
                db.Entry(followupChangeLog).State = EntityState.Modified;
                //db.SaveChanges();
            }
        }

        private static DbComplaintFollowupLogs SaveComplaintFollowupInLog(int complaintId, int followupId, DateTime followupSaveDateTime, string comments, DBContextHelperLinq db)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbComplaintFollowupLogs dbFollowupChangeLog = new DbComplaintFollowupLogs();
            dbFollowupChangeLog.FollowupMarkedByUserId = cookie.UserId;
            dbFollowupChangeLog.Complaint_Id = complaintId;
            dbFollowupChangeLog.FollowupId = followupId;
            dbFollowupChangeLog.FollowupDateTime = followupSaveDateTime;
            dbFollowupChangeLog.Comments = comments;
            dbFollowupChangeLog.IsCurrentlyActive = true;
            db.DbComplaintFollowupLogs.Add(dbFollowupChangeLog);
            //db.SaveChanges();
            return dbFollowupChangeLog;
        }


        public static List<VmAgentComplaintSearchListing> GetComplaintForAgentByProfileId(int personId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@PersonId", personId.ToDbObj());
            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Complaints_By_PersonId]", paramDict).ToList<VmAgentComplaintSearchListing>();
        }

        /*
        public static void ChangeComplaintStatus(VmStakeholderComplaintDetail vmStakeholderComplaint, HttpFileCollectionBase files)
        {
            int campaignId = (int)vmStakeholderComplaint.Compaign_Id;
            int complaintId = vmStakeholderComplaint.ComplaintId;
            int statusId = (int)vmStakeholderComplaint.statusID;
            DateTime currentDateTime = DateTime.Now;

            float catRetainingHours = DbComplaintType.GetRetainingHoursByTypeId((int)vmStakeholderComplaint.Complaint_Category);
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbComplaint dbComplaint = DbComplaint.GetListByComplaintId(complaintId).First();
            List<AssignmentModel> assignmentModelList = AssignmentHandler.GetAssignmentModelOnStatusChange(Convert.ToInt32(cookie.Hierarchy_Id), dbComplaint, statusId, DateTime.Now, DbAssignmentMatrix.GetByCampaignIdAndCategoryId(campaignId), catRetainingHours);

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

            paramDict.Add("@ComplaintId", complaintId.ToDbObj());
            paramDict.Add("@StatusId", statusId.ToDbObj());
	 
            paramDict.Add("@Status_ChangedBy", cookie.UserId);
            paramDict.Add("@Status_ChangedBy_Name", cookie.UserName);
            paramDict.Add("@StatusChangedDate_Time", currentDateTime);
            paramDict.Add("@StatusChangedBy_RoleId", Convert.ToInt32(cookie.Role));
            paramDict.Add("@StatusChangedBy_HierarchyId", Convert.ToInt32(cookie.Hierarchy_Id));
            paramDict.Add("@StatusChanged_Comments", vmStakeholderComplaint.statusChangeComments.ToDbObj());

            DBHelper.GetDataTableByStoredProcedure("[PITB].[Update_Complaints_Status]", paramDict).ToList<VmAgentComplaintListing>();


            DBContextHelperLinq db = new DBContextHelperLinq();
            DbComplaintStatusChangeLog dbStatusChangeLog = new DbComplaintStatusChangeLog();
            dbStatusChangeLog.StatusChangedByUserId = cookie.UserId;
            dbStatusChangeLog.Complaint_Id = complaintId;
            dbStatusChangeLog.StatusId = statusId;
            dbStatusChangeLog.StatusChangeDateTime = currentDateTime;
            dbStatusChangeLog.Comments = vmStakeholderComplaint.statusChangeComments;
            dbStatusChangeLog.IsCurrentlyActive = true;
            db.DbComplaintStatusChangeLog.Add(dbStatusChangeLog);
            db.SaveChanges();

            int statusLogId = dbStatusChangeLog.Id;
            FileUploadHandler.UploadMultipleFiles(files, Utility.GetComplaintIdStr(campaignId, complaintId), statusLogId);
        }
        */


        public static DataTable GetStakeholderComplaintsForExport(string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@StartRow", (dtParams.Start).ToDbObj());
            paramDict.Add("@EndRow", (dtParams.End).ToDbObj());
            paramDict.Add("@OrderByColumnName", dtParams.ListOrder[0].columnName.ToDbObj());
            paramDict.Add("@OrderByDirection", dtParams.ListOrder[0].sortingDirectionStr.ToDbObj());

            paramDict.Add("@WhereOfMultiSearch", dtParams.WhereOfMultiSearch.ToDbObj());

            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@Category", category.ToDbObj());
            paramDict.Add("@Status", complaintStatuses.ToDbObj());
            paramDict.Add("@TransferedStatus", commaSeperatedTransferedStatus.ToDbObj());
            paramDict.Add("@ComplaintType", (Convert.ToInt32(complaintType)).ToDbObj());
            paramDict.Add("@UserHierarchyId", Convert.ToInt32(cookie.Hierarchy_Id).ToDbObj());
            paramDict.Add("@UserDesignationHierarchyId", Convert.ToInt32(cookie.User_Hierarchy_Id).ToDbObj());
            paramDict.Add("@ListingType", Convert.ToInt32(listingType).ToDbObj());
            paramDict.Add("@ProvinceId", (cookie.ProvinceId).ToDbObj());
            paramDict.Add("@DivisionId", (cookie.DivisionId).ToDbObj());
            paramDict.Add("@DistrictId", (cookie.DistrictId).ToDbObj());

            paramDict.Add("@Tehsil", (cookie.TehsilId).ToDbObj());
            paramDict.Add("@UcId", (cookie.UcId).ToDbObj());
            paramDict.Add("@WardId", (cookie.WardId).ToDbObj());

            paramDict.Add("@UserId", cookie.UserId.ToDbObj());

            paramDict.Add("@SpType", spType);

            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ServerSide]", paramDict);
        }


        public static DataTable GetStakeholderComplaintsForExport(string fromDate, string toDate, string campaign, string category, string complaintStatuses, int complaintType, Config.StakeholderComplaintListingType listingType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@ComplaintType", ((Config.ComplaintType)(complaintType)).ToDbObj());
            paramDict.Add("@Category", category.ToDbObj());
            paramDict.Add("@Status", complaintStatuses.ToDbObj());
            paramDict.Add("@UserHierarchyId", Convert.ToInt32(cookie.Hierarchy_Id).ToDbObj());
            paramDict.Add("@ListingType", Convert.ToInt32(listingType).ToDbObj());
            paramDict.Add("@ProvinceId", Convert.ToInt32(cookie.ProvinceId).ToDbObj());

            paramDict.Add("@DistrictId", Convert.ToInt32(cookie.DistrictId).ToDbObj());

            paramDict.Add("@Tehsil", Convert.ToInt32(cookie.TehsilId).ToDbObj());

            paramDict.Add("@UserId", cookie.UserId.ToDbObj());

            paramDict.Add("@SpType", "ExcelReport");

            //return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ForExport]", paramDict);
            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Stakeholder_Complaints_ServerSide]", paramDict);
        }


        public static DataTable GetAgentComplaintsForExport(string fromDate, string toDate, string campaign, int complaintType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@From", fromDate.ToDbObj());
            paramDict.Add("@To", toDate.ToDbObj());
            paramDict.Add("@Campaign", campaign.ToDbObj());
            paramDict.Add("@UserId", cookie.UserId.ToDbObj());
            paramDict.Add("@ComplaintType", complaintType.ToDbObj());
            paramDict.Add("@RoleId", Convert.ToInt32(cookie.Role).ToDbObj());


            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_All_Complaints_Supervisor_Export]", paramDict);
        }

        #region Campaigns

        public static List<VmCampaign> GetListOfCampaignsByUserId()
        {
            Mapper.CreateMap<DbCampaign, VmCampaign>();
            return Mapper.Map<List<VmCampaign>>(DbCampaign.GetByUserId(new AuthenticationHandler().CmsCookie.UserId));
        }

        #endregion

        #region Listing

        //public static DataTable GetComplaintListings(DataTableParamsModel dtModel, string fromDate, string toDate, string campaign, int complaintType, string spType, int listingType)
        //{
        //    List<string> prefixStrList = new List<string> { "complaints", "campaign", "personalInfo", "complaints", "complaintType", "Statuses" };
        //    DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

        //    Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
        //    dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

        //    DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintNo" }, prefixStrList, dictFilterQuery);
        //    ListingParamsAgent paramsComplaintListing = SetAgentListingParams(dtModel, fromDate, toDate, campaign, (Config.ComplaintType)complaintType, spType, listingType);
        //    /*
        //    switch (new AuthenticationHandler().CmsCookie.Role)
        //    {
        //        case Config.Roles.Agent:
        //            listOfComplaints = GetComplaintsOfAgents(fromDate, toDate, campaign);
        //            break;
        //        case Config.Roles.AgentSuperVisor:
        //            listOfComplaints = GetComplaintsAllComplaintsSupervisor(dtModel, fromDate, toDate, campaign);
        //            break;

        //    }*/
        //    //return listOfComplaints;
        //    string queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
        //    return DBHelper.GetDataTableByQueryString(queryStr, null);
        //}


        public static ListingParamsAgent SetAgentListingParams(DataTableParamsModel dtParams, string fromDate, string toDate, string campaign, Config.ComplaintType complaintType, string spType, int listingType)
        {
            string extraSelection = "";

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsAgent paramsModel = new ListingParamsAgent();
            paramsModel.StartRow = dtParams.Start;
            paramsModel.EndRow = dtParams.End;
            paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
            paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
            paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;

            paramsModel.From = fromDate;
            paramsModel.To = toDate;
            paramsModel.Campaign = campaign;
            paramsModel.RoleId = (int)cookie.Role;
            paramsModel.ListingType = listingType;
            //paramsModel.Category = category;
            //paramsModel.Status = complaintStatuses;
            //paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            //paramsModel.UserHierarchyId = Convert.ToInt32(cookie.Hierarchy_Id);
            //paramsModel.UserDesignationHierarchyId = Convert.ToInt32(cookie.User_Hierarchy_Id);
            //paramsModel.ListingType = Convert.ToInt32(listingType);
            //paramsModel.ProvinceId = cookie.ProvinceId;
            //paramsModel.DivisionId = cookie.DivisionId;
            //paramsModel.DistrictId = cookie.DistrictId;

            //paramsModel.Tehsil = cookie.TehsilId;
            //paramsModel.UcId = cookie.UcId;
            //paramsModel.WardId = cookie.WardId;

            paramsModel.UserId = cookie.UserId;
            //paramsModel.UserCategoryId1 = cookie.UserCategoryId1;
            //paramsModel.UserCategoryId2 = cookie.UserCategoryId2;
            //paramsModel.CheckIfExistInSrcId = 0;
            //paramsModel.CheckIfExistInUserSrcId = 0;
            //paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            return paramsModel;
        }


        /*
        public static List<VmStakeholderComplaintListing> GetStakeHolderServerSideList(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, Config.StakeholderComplaintListingType listingType)
        {
            //string commaSeperatedCampaigns = string.Join(",", campaign);
            //string commaSeperatedCategories = string.Join(",", cateogries);
            //string commaSeperatedStatuses = string.Join(",", statuses);


            Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
            //dictOrderQuery.Add("Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate))");
            //List<string> prefixStrList = new List<string> { "a", "a", "a", "a", "a", "a", "a", "a", "a" };


            //List<string> prefixStrList = new List<string> { "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints" };
            // for joins
            List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
            dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
            dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
            dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            // for joins
            dictFilterQuery.Add("complaintType.Complaint_Category_Name", "complaintType.name Like '%_Value_%'");
            dictFilterQuery.Add("Statuses.Complaint_Computed_Status", "Statuses.[Status] Like '%_Value_%'");
            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList, dictFilterQuery);

            return GetComplaintsOfStakeholderServerSide(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, dtModel, Config.ComplaintType.Complaint, listingType);
			
        }*/
        //public static DataTable GetComplaintAssignedToData(List<VmStakeholderComplaintListing> dtModel)        
        //{
        //    DBContextHelperLinq db = new DBContextHelperLinq();
        //    var data = from m in dtModel select m.ComplaintId;

        //}

        public static DataTable GetStakeHolderServerSideListDenormalized(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            DbUsers dbUser = null;
            if (userId == -1)
            {
                dbUser = Utility.GetUserFromCookie();
            }
            else
            {
                dbUser = DbUsers.GetActiveUser(userId);
            }
            if (dtModel != null)
            {
                Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
                //dictOrderQuery.Add("Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate))");
                //List<string> prefixStrList = new List<string> { "a", "a", "a", "a", "a", "a", "a", "a", "a" };


                List<string> prefixStrList = new List<string>
                    {
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints",
                        "complaints"
                    };
                // for joins
                //List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
                //dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
                //dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
                DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);

                Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
                //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
                dictFilterQuery.Add("complaints.Created_Date",
                    "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

                // for joins
                //dictFilterQuery.Add("complaintType.Complaint_Category_Name", "complaintType.name Like '%_Value_%'");
                //dictFilterQuery.Add("Statuses.Complaint_Computed_Status", "Statuses.[Status] Like '%_Value_%'");
                DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList,
                    dictFilterQuery);
                //return GetComplaintsOfStakeholderServerSide(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);

            }

            // Start my Own Custom Code

            Dictionary<string, object> dictParam = new Dictionary<string, object>();
            dictParam.Add("@fromDate", Utility.GetDateTime(from));
            dictParam.Add("@toDate", Utility.GetMaximumTimeForDate(to));
            dictParam.Add("@campaignIds", commaSeperatedCampaigns);
            dictParam.Add("@categoryIds", commaSeperatedCategories);

            dictParam.Add("@transferedStatuses", commaSeperatedTransferedStatus);
            dictParam.Add("@complaintComputedStatus", commaSeperatedStatuses);
            dictParam.Add("@hierarchyId", (int)dbUser.Hierarchy_Id);
            dictParam.Add("@userHierarchyId", dbUser.User_Hierarchy_Id);
            dictParam.Add("@regionId", QueryHelper.GetFinalQuery("RegionId=" + (int)dbUser.Hierarchy_Id, Config.ConfigType.Query));
            dictParam.Add("@regionValue", DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser));


            //paramsModel.StartRow = dtParams.Start;
            //paramsModel.EndRow = dtParams.End;
            //paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
            //paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
            //paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;

            if (dtModel != null)
            {
                dictParam.Add("@StartRow", dtModel.Start);
                dictParam.Add("@EndRow", dtModel.End);
                dictParam.Add("@OrderByColumnName", dtModel.ListOrder[0].columnName);
                dictParam.Add("@OrderByDirection", dtModel.ListOrder[0].sortingDirectionStr);
                dictParam.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch);
            }


            //dictParam.Add("@regionValue", QueryHelper.GetFinalQuery("RegionIdVaue=" + (int)dbUser.Hierarchy_Id, Config.ConfigType.Query));

            string queryStr = null;
            string excelExportQuery = (spType != "ExcelReport") ? "" : spType;
            string dashboardLabelStatusWise = (spType != "DashboardLabelsStausWise") ? "" : spType;
            string moduleStr = null;
            if (complaintsType == (int)Config.ComplaintType.Complaint)
            {
                if (listingType == Config.StakeholderComplaintListingType.AssignedToMe)
                {
                    if (spType == "Listing")
                    {
                        moduleStr = "ComplaintsListing_Mine";
                    }
                    else if (spType == "DashboardLabelsStausWise")
                    {
                        moduleStr = "ComplaintsListing_Mine_DashboardLabelsStausWise";
                    }
                    else if (spType == "ExcelReport")
                    {
                        moduleStr = "ComplaintsListing_Mine_Export";
                    }
                }
                else if (listingType == Config.StakeholderComplaintListingType.UptilMyHierarchy)
                {
                    if (spType == "Listing")
                    {
                        moduleStr = "ComplaintsListing_All";
                    }
                    else if (spType == "DashboardLabelsStausWise")
                    {
                        moduleStr = "ComplaintsListing_All_DashboardLabelsStausWise";
                    }
                    else if (spType == "ExcelReport")
                    {
                        moduleStr = "ComplaintsListing_All_Export";
                    }
                }
            }
            else if (complaintsType == (int)Config.ComplaintType.Suggestion)
            {
                if (spType == "Listing")
                {
                    moduleStr = "ComplaintsListing_Suggestion";
                }
                else if (spType == "ExcelReport")
                {
                    moduleStr = "ComplaintsListing_Suggestion_Export";
                }

            }
            else if (complaintsType == (int)Config.ComplaintType.Inquiry)
            {
                if (spType == "Listing")
                {
                    moduleStr = "ComplaintsListing_Inquiry";
                }
                else if (spType == "ExcelReport")
                {
                    moduleStr = "ComplaintsListing_Inquiry_Export";
                }
            }

            //queryStr = QueryHelper.GetFinalQuery(moduleStr,
            //            Utility.GetIntByCommaSepStr("701").ToString(),
            //            Config.ConfigType.Query,
            //            dictParam);

            queryStr = QueryHelper.GetFinalQuery(moduleStr,
                        Utility.GetIntByCommaSepStr(commaSeperatedCampaigns).ToString(),
                        Config.ConfigType.Query,
                        dictParam);

            // if no query is found run general
            if (queryStr == null)
            {

                //return null;

                // End my own custom code
                ListingParamsModelBase paramsModel = SetStakeholderListingParams(dbUser, from, to,
                    commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses,
                    commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
                queryStr = StakeholderListingLogic.GetListingQuery(paramsModel);
            }

            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }


        public static ListingParamsModelBase SetStakeholderListingParams(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = "complaints.Department_Name,complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time, CNFP_Feedback_Id,CNFP_Feedback_Value, CNFP_Feedback_Ref_Id,CNFP_Is_FeedbackGiven,CNFP_Feedback_Comments,complaints.Computed_Overdue_Days,complaints.StatusReopenedCount,complaints.Computed_Remaining_Time_Percentage,Computed_Total_Time_Percentage_Since_Launch,";



            CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();

            string permitiveFeilds = "";

            if(PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.CMCCExtraColumnInComplaintExport, cookie.UserId, cookie.Role)){

                DbPermissionsAssignment permission = cookie.ListPermissions.FirstOrDefault(p =>
                  p.Type == (int)Config.PermissionsType.User &&
                  p.Type_Id == dbUser.User_Id &&
                  p.Permission_Id == (int)Config.Permissions.CMCCExtraColumnInComplaintExport
                  );

                if(permission!=null)
                    permitiveFeilds += permission.Permission_Value+",";
            }


            if (spType == "ExcelReport")
            {
                extraSelection = $@"CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10)) AS [Complaint No],
					complaints.Complaint_Computed_Status as Complaint_Status,Computed_Remaining_Total_Time, 
					 C.Person_Name as [Person Name],
					 C.Cnic_No as [Cnic No],
					 CASE C.Gender WHEN 1 THEN 'MALE' ELSE 'FEMALE' END AS Gender,
					 complaints.Person_District_Name as [Caller District],
					 CONVERT(VARCHAR(10),complaints.Created_Date,120) Date,
					C.Mobile_No as [Mobile No],
					C.Person_Address as [Person Address],
					D.District_Name [Complaint District],
                    {permitiveFeilds}
                    B.Name Category,
					F.Name as [Sub Category],
					complaints.Complaint_Remarks as [Complaint Remarks],
					complaints.Agent_Comments as [Agent Comments],
					P.[Status],
					complaints.Complaint_Computed_Hierarchy as [Escalation Level],
					complaints.Created_Date as [Created Date],
					df.FieldName,df.FieldValue";

                paramsModel.InnerJoinLogic = @"INNER JOIN PITB.Complaints_Type B ON complaints.Complaint_Category=B.Id
					INNER JOIN PITB.Complaints_SubType F ON complaints.Complaint_SubCategory=F.Id
					INNER JOIN PITB.Person_Information C ON complaints.Person_Id=C.Person_id
					INNER JOIN PITB.Districts D ON complaints.District_Id=D.id
					INNER JOIN PITB.Statuses P ON p.Id=complaints.Complaint_Computed_Status_Id
					LEFT JOIN pitb.Dynamic_ComplaintFields df ON df.ComplaintId = complaints.Id";
            }


            if (dtParams != null)
            {
                paramsModel.StartRow = dtParams.Start;
                paramsModel.EndRow = dtParams.End;
                paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
                paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
                paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            }
            paramsModel.From = fromDate;
            paramsModel.To = toDate;
            paramsModel.Campaign = campaign;
            paramsModel.Category = category;
            paramsModel.Status = complaintStatuses;
            paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            paramsModel.UserHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
            paramsModel.UserDesignationHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
            paramsModel.ListingType = Convert.ToInt32(listingType);
            paramsModel.ProvinceId = dbUser.Province_Id;
            paramsModel.DivisionId = dbUser.Division_Id;
            paramsModel.DistrictId = dbUser.District_Id;

            paramsModel.Tehsil = dbUser.Tehsil_Id;
            paramsModel.UcId = dbUser.UnionCouncil_Id;
            paramsModel.WardId = dbUser.Ward_Id;

            paramsModel.UserId = dbUser.User_Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(dbUser.ListDbUserCategory);
            paramsModel.CheckIfExistInSrcId = 0;
            paramsModel.CheckIfExistInUserSrcId = 0;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            return paramsModel;
        }

        /*private static void ApplyStakeHolderServerSideListDenormalizedConfiguration(DataTableParamsModel dtModel)
        {
            Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
            //dictOrderQuery.Add("Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate))");
            //List<string> prefixStrList = new List<string> { "a", "a", "a", "a", "a", "a", "a", "a", "a" };


            List<string> prefixStrList = new List<string> { "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints", "complaints" };
            // for joins
            //List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
            //dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
            //dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList, dictOrderQuery);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
            dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            // for joins
            //dictFilterQuery.Add("complaintType.Complaint_Category_Name", "complaintType.name Like '%_Value_%'");
            //dictFilterQuery.Add("Statuses.Complaint_Computed_Status", "Statuses.[Status] Like '%_Value_%'");
            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList, dictFilterQuery);
        }*/

        #endregion

        public static List<DbAttachments> GetComplaintAttachments(int complaintId)
        {
            List<DbAttachments> attachments = new List<DbAttachments>();
            using (DBContextHelperLinq db = new DBContextHelperLinq())
            {
                attachments = db.DbAttachments.Where(x => x.Complaint_Id == complaintId).ToList<DbAttachments>();
            }
            return attachments;
        }

        #region DashboardCategoryWise
        public static VmStatusWiseComplaintsData GetCategoryWiseDashboardData(int userId, string startDate, string endDate, int campaignId, Config.CategoryType categoryType, int categoryId)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = "";
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;
            int? groupId = null;

            switch (categoryType)
            {
                case Config.CategoryType.Main:
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);


                    listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));
                    //listDbStatuses =
                    //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //        listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());



                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    //List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
                    List<DbDepartment> listDepartment =
                        DbDepartment.GetByCampaignAndGroupId(Utility.GetIntByCommaSepStr(dbUser.Campaigns), null);
                    foreach (DbDepartment dbDepartment in listDepartment)
                    {
                        listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbDepartment.Id, dbDepartment.Name), listDbStatuses));
                    }

                    paramsSchoolEducation = SetParamsDynamicQuery(dbUser, categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listDepartment.Select(n => n.Id).ToList()), userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);

                        statusWiseComplaintData.ListUserWiseData =
                        statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                            n => n.TotalStatusWiseCount).ToList();

                        return statusWiseComplaintData;
                    }
                    return null;
                    break;


                case Config.CategoryType.Sub:
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);



                    listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));

                    //listDbStatuses =
                    //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //        listDbPermissionsAssignment);


                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());


                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    groupId =
                        DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId,
                            Config.ComplaintType.Complaint);
                    List<DbComplaintType> listComplaintType = DbComplaintType.GetByDepartmentId(categoryId); //DbComplaintType.GetByDepartmentAndGroupId(categoryId, groupId);
                    //List<DbDepartment> listDepartment =
                    //    DbDepartment.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
                    foreach (DbComplaintType dbComplaintType in listComplaintType)
                    {
                        listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbComplaintType.Complaint_Category, dbComplaintType.Name), listDbStatuses));
                    }

                    paramsSchoolEducation = SetParamsDynamicQuery(Utility.GetUserFromCookie(), categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listComplaintType.Select(n => n.Complaint_Category).ToList()), userStatuses,
                     "1,0", dtParams, Config.ComplaintType.Complaint,
                     Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);

                        statusWiseComplaintData.ListUserWiseData =
                        statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                            n => n.TotalStatusWiseCount).ToList();

                        return statusWiseComplaintData;
                    }
                    return null;
                    break;

                case Config.CategoryType.Tertiary:
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));

                    //listDbStatuses =
                    //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                    //        listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());



                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    groupId =
                        DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId,
                            Config.ComplaintType.Complaint);
                    List<DbComplaintSubType> listComplaintSubtType = DbComplaintSubType.GetByComplaintType(categoryId); //DbComplaintType.GetByDepartmentAndGroupId(categoryId, groupId);
                    //List<DbDepartment> listDepartment =
                    //    DbDepartment.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
                    foreach (DbComplaintSubType dbComplaintSubType in listComplaintSubtType)
                    {
                        listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbComplaintSubType.Complaint_SubCategory, dbComplaintSubType.Name), listDbStatuses));
                    }

                    paramsSchoolEducation = SetParamsDynamicQuery(Utility.GetUserFromCookie(), categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listComplaintSubtType.Select(n => n.Complaint_SubCategory).ToList()), userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);

                        statusWiseComplaintData.ListUserWiseData =
                        statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                            n => n.TotalStatusWiseCount).ToList();


                        return statusWiseComplaintData;
                    }
                    return null;
                    break;
            }
            return new VmStatusWiseComplaintsData();
        }

        private static VmStatusWiseComplaintsData GetUserStatusWiseComplaintData(DataSet dataSet, List<VmUserWiseStatus> listVmUserWiseStatus)
        {
            VmStatusWiseComplaintsData vmStatusWiseComplaintData = new VmStatusWiseComplaintsData();
            //vmStatusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();
            VmUserWiseStatus vmUserWiseStatus = null;
            VmStatusCount vmStatusCount = null;
            VmUserWiseStatus vmUserWiseStatusToMerge = null;

            bool isUserPresent = false;
            int i = 0;
            foreach (DataTable dt in dataSet.Tables)
            {
                isUserPresent = false;
                vmUserWiseStatusToMerge = listVmUserWiseStatus[i];
                //vmStatusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //if (!isUserPresent)
                        {
                            //vmUserWiseStatus = new VmUserWiseStatus();
                            vmUserWiseStatus = listVmUserWiseStatus.Where(n => n.UserId == Convert.ToInt32(row["CatId"]))
                                .FirstOrDefault();
                            //vmUserWiseStatus.UserId = Convert.ToInt32(row["CatId"]);
                            //vmUserWiseStatus.Name = row["CatName"].ToString();//vmUserWiseStatusToMerge.Name;
                            //vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();

                            //vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatus);
                            //isUserPresent = true;
                        }

                        //vmStatusCount = new VmStatusCount();
                        vmStatusCount = vmUserWiseStatus.ListVmStatusWiseCount.Where(n => n.StatusId == Convert.ToInt32(row["Complaint_Computed_Status_Id"])).FirstOrDefault();
                        //vmStatusCount.StatusId = Convert.ToInt32(row["Complaint_Computed_Status_Id"]);
                        //vmStatusCount.StatusString = BlSchool.GetAlteredStatus(row["Complaint_Computed_Status"].ToString());
                        vmStatusCount.Count = Convert.ToInt32(row["Count"]);
                        //vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
                    }
                    //Utility.MergeLists(vmUserWiseStatus.ListVmStatusWiseCount, vmUserWiseStatusToMerge.ListVmStatusWiseCount);
                }
                /*else
                {
                    vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatusToMerge);
                }*/
                i++;
            }
            vmStatusWiseComplaintData.ListUserWiseData = listVmUserWiseStatus;
            return vmStatusWiseComplaintData;
        }




        public static ListingParamsModelBase SetParamsDynamicQuery(DbUsers dbUser, Config.CategoryType categoryType, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            //string extraSelection = "complaints.Person_Contact,complaints.Department_Name, complaints.Complaint_SubCategory_Name, complaints.RefField1, complaints.RefField2, complaints.RefField3, complaints.RefField4, complaints.RefField5, complaints.RefField6, complaints.Person_Cnic,";

            string selectionFields = "";
            string groupByFields = "";
            string whereLogic = "";
            if (categoryType == Config.CategoryType.Main)
            {
                selectionFields = "complaints.Department_Id AS CatId,complaints.Department_Name AS CatName";
                groupByFields =
                    "complaints.Department_Id,complaints.Department_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

                whereLogic = " and  EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
                             "',',') X WHERE X.Item=complaints.Department_Id)";
            }
            else if (categoryType == Config.CategoryType.Sub)
            {
                selectionFields = "complaints.Complaint_Category AS CatId,complaints.Complaint_Category_Name AS CatName";
                groupByFields =
                    "complaints.Complaint_Category,complaints.Complaint_Category_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";
                whereLogic = "  and EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
                             "',',') X WHERE X.Item=complaints.Complaint_Category)";
            }
            else if (categoryType == Config.CategoryType.Tertiary)
            {
                selectionFields = "complaints.Complaint_SubCategory AS CatId,complaints.Complaint_SubCategory_Name AS CatName";
                groupByFields =
                    "complaints.Complaint_SubCategory,complaints.Complaint_SubCategory_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

                whereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
                             "',',') X WHERE X.Item=complaints.Complaint_SubCategory)";
            }


            //CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();
            if (dtParams != null)
            {
                paramsModel.StartRow = dtParams.Start;
                paramsModel.EndRow = dtParams.End;
                paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
                paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
                paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            }

            paramsModel.From = fromDate;
            paramsModel.To = toDate;
            paramsModel.Campaign = campaign;
            paramsModel.Category = category;
            paramsModel.Status = complaintStatuses;
            paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            paramsModel.UserHierarchyId = Convert.ToInt32(dbUser.Hierarchy_Id);
            paramsModel.UserDesignationHierarchyId = Convert.ToInt32(dbUser.User_Hierarchy_Id);
            paramsModel.ListingType = Convert.ToInt32(listingType);
            paramsModel.ProvinceId = dbUser.Province_Id;
            paramsModel.DivisionId = dbUser.Division_Id;
            paramsModel.DistrictId = dbUser.District_Id;

            paramsModel.Tehsil = dbUser.Tehsil_Id;
            paramsModel.UcId = dbUser.UnionCouncil_Id;
            paramsModel.WardId = dbUser.Ward_Id;

            paramsModel.UserId = dbUser.User_Id;
            paramsModel.UserCategoryId1 = dbUser.UserCategoryId1;
            paramsModel.UserCategoryId2 = dbUser.UserCategoryId2;
            paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(dbUser.ListDbUserCategory);
            paramsModel.CheckIfExistInSrcId = 1;
            paramsModel.CheckIfExistInUserSrcId = 1;
            paramsModel.SelectionFields = selectionFields;
            paramsModel.GroupByLogic = groupByFields;
            paramsModel.SpType = spType;
            paramsModel.WhereLogic = whereLogic;

            /*
            if (dbUser.SubRole_Id == Config.SubRoles.SDU && listingType == Config.StakeholderComplaintListingType.AssignedToMe)
            {
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.SelectionFields = @" schoolMap.Assigned_To, schoolMap.Assigned_To_Name,CONVERT(VARCHAR(10),complaints.StatusChangedDate_Time,120) StatusChangedDate_Time ," + paramsModel.SelectionFields;
                paramsModel.InnerJoinLogic = @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
                ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int)Config.SchoolEducationUserSubRoles.SDU + " OR (complaints.Status_ChangedBy=" + Config.MEALoginId + "AND complaints.Complaint_Status_Id=" + (int)Config.ComplaintStatus.ResolvedVerified + ") ) ";
            }
            else
            {
                paramsModel.IgnoreComputedHierarchyCheck = false;
            }*/

            return paramsModel;
        }

        private static VmUserWiseStatus MakeEmptyStatusModel(KeyValuePair<int, string> keyValCateogy, List<DbStatus> listStatus)
        {
            VmUserWiseStatus vmUserWiseStatus = new VmUserWiseStatus();
            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();
            VmStatusCount vmStatusCount = null;

            vmUserWiseStatus.UserId = keyValCateogy.Key;
            vmUserWiseStatus.Name = keyValCateogy.Value;


            foreach (DbStatus dbStatus in listStatus)
            {
                vmStatusCount = new VmStatusCount();
                vmStatusCount.StatusId = dbStatus.Complaint_Status_Id;
                vmStatusCount.StatusString = Utility.GetAlteredStatus(dbStatus.Status, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                vmStatusCount.Count = 0;
                vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
            }
            return vmUserWiseStatus;
        }


        //public static VmStatusWiseComplaintsData GetTertiaryCategoryWiseData(int userId, string startDate, string endDate, int campaignId, Config.CategoryType categoryType, int categoryId)
        //{
        //    DbUsers dbUser = null;
        //    List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
        //    List<DbStatus> listDbStatuses = null;
        //    string userStatuses = null;
        //    DataTableParamsModel dtParams = null;
        //    ListingParamsModelBase paramsSchoolEducation = null;

        //    string queryStr = "";
        //    DataSet ds;
        //    VmStatusWiseComplaintsData statusWiseComplaintData;
        //    List<VmUserWiseStatus> listVmUserWiseStatus;
        //    int? groupId = null;

        //    switch (categoryType)
        //    {
        //        case Config.CategoryType.Main:
        //            dbUser = DbUsers.GetActiveUser(userId);
        //            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
        //                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);


        //            listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));
        //            //listDbStatuses =
        //            //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
        //            //        listDbPermissionsAssignment);

        //            userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());



        //            listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //            //List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
        //            List<DbDepartment> listDepartment =
        //                DbDepartment.GetByCampaignAndGroupId(Utility.GetIntByCommaSepStr(dbUser.Campaigns), null);
        //            foreach (DbDepartment dbDepartment in listDepartment)
        //            {
        //                listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbDepartment.Id, dbDepartment.Name), listDbStatuses));
        //            }

        //            paramsSchoolEducation = SetParamsDynamicQuery(dbUser, categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listDepartment.Select(n => n.Id).ToList()), userStatuses,
        //                "1,0", dtParams, Config.ComplaintType.Complaint,
        //                Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

        //            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //            if (!string.IsNullOrEmpty(queryStr))
        //            {
        //                ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
        //                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, false);

        //                statusWiseComplaintData.ListUserWiseData =
        //                statusWiseComplaintData.ListUserWiseData.OrderByDescending(
        //                    n => n.TotalStatusWiseCount).ToList();

        //                return statusWiseComplaintData;
        //            }
        //            return null;
        //            break;


        //        case Config.CategoryType.Sub:
        //            dbUser = DbUsers.GetActiveUser(userId);
        //            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
        //                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);



        //            listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));

        //            //listDbStatuses =
        //            //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
        //            //        listDbPermissionsAssignment);


        //            userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());


        //            listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //            groupId =
        //                DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId,
        //                    Config.ComplaintType.Complaint);
        //            List<DbComplaintType> listComplaintType = DbComplaintType.GetByDepartmentId(categoryId); //DbComplaintType.GetByDepartmentAndGroupId(categoryId, groupId);
        //            //List<DbDepartment> listDepartment =
        //            //    DbDepartment.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
        //            foreach (DbComplaintType dbComplaintType in listComplaintType)
        //            {
        //                listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbComplaintType.Complaint_Category, dbComplaintType.Name), listDbStatuses));
        //            }

        //            paramsSchoolEducation = SetParamsDynamicQuery(Utility.GetUserFromCookie(), categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listComplaintType.Select(n => n.Complaint_Category).ToList()), userStatuses,
        //             "1,0", dtParams, Config.ComplaintType.Complaint,
        //             Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

        //            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //            if (!string.IsNullOrEmpty(queryStr))
        //            {
        //                ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
        //                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, false);

        //                statusWiseComplaintData.ListUserWiseData =
        //                statusWiseComplaintData.ListUserWiseData.OrderByDescending(
        //                    n => n.TotalStatusWiseCount).ToList();

        //                return statusWiseComplaintData;
        //            }
        //            return null;
        //            break;

        //        case Config.CategoryType.Tertiary:
        //            dbUser = DbUsers.GetActiveUser(userId);
        //            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
        //                (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

        //            listDbStatuses = BlCommon.GetStatusesForAgainstCampaign((Config.Campaign)Utility.GetIntByCommaSepStr(dbUser.Campaigns));

        //            //listDbStatuses =
        //            //    BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
        //            //        listDbPermissionsAssignment);

        //            userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());



        //            listVmUserWiseStatus = new List<VmUserWiseStatus>();
        //            groupId =
        //                DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId,
        //                    Config.ComplaintType.Complaint);
        //            List<DbComplaintSubType> listComplaintSubtType = DbComplaintSubType.GetByComplaintType(categoryId); //DbComplaintType.GetByDepartmentAndGroupId(categoryId, groupId);
        //            //List<DbDepartment> listDepartment =
        //            //    DbDepartment.GetByCampaignId(Utility.GetIntByCommaSepStr(dbUser.Campaigns));
        //            foreach (DbComplaintSubType dbComplaintSubType in listComplaintSubtType)
        //            {
        //                listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbComplaintSubType.Complaint_SubCategory, dbComplaintSubType.Name), listDbStatuses));
        //            }

        //            paramsSchoolEducation = SetParamsDynamicQuery(Utility.GetUserFromCookie(), categoryType, startDate, endDate, dbUser.Campaigns, Utility.GetCommaSepStrFromList(listComplaintSubtType.Select(n => n.Complaint_SubCategory).ToList()), userStatuses,
        //                "1,0", dtParams, Config.ComplaintType.Complaint,
        //                Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts");

        //            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

        //            if (!string.IsNullOrEmpty(queryStr))
        //            {
        //                ds = DBHelper.GetDataSetByQueryString(queryStr, null);

        //                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
        //                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
        //                    statusWiseComplaintData, false);

        //                statusWiseComplaintData.ListUserWiseData =
        //                statusWiseComplaintData.ListUserWiseData.OrderByDescending(
        //                    n => n.TotalStatusWiseCount).ToList();


        //                return statusWiseComplaintData;
        //            }
        //            return null;
        //            break;
        //    }
        //    return new VmStatusWiseComplaintsData();
        //}

        #endregion


    }
}