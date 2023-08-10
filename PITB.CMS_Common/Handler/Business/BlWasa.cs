using System.Data;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Handler.DynamicFields.Wasa;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Models.View.Dynamic;
using PITB.CMS_Common.Models.View.Wasa;
using AutoMapper;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlWasa
    {
        public static VmAddComplaintWasa GetVmAddComplaintMerged(int profileId = 0,
            int campaignId = 0)
        {
            VmAddComplaintWasa viewModel = new VmAddComplaintWasa();
       
            viewModel.ComplaintVm.ListOfTehsils = new List<DbTehsil>();
            viewModel = GetVmAddComplaintMerged(viewModel, profileId, campaignId);
            viewModel.ComplaintVm.Province_Id = 1;
            viewModel.ComplaintVm.Division_Id = 1;
            viewModel.ComplaintVm.District_Id = 1;
            if (viewModel.PersonalInfoVm == null)
            {
                viewModel.PersonalInfoVm = new VmPersonalInfoWasa();
            }
            viewModel.PersonalInfoVm.Province_Id = 1;
            viewModel.PersonalInfoVm.District_Id = 1;
            viewModel.PersonalInfoVm.Division_Id = 1;
            viewModel.ComplaintVm.ListOfTehsils = DbTehsil.GetByDistrictAndGroupId(1, 6).ToList();

            Mapper.CreateMap<VmComplaint, VmSuggestionWasa>();
            viewModel.SuggestionVm = Mapper.Map<VmSuggestionWasa>(viewModel.ComplaintVm);

            Mapper.CreateMap<VmComplaint, VmInquiryWasa>();
            viewModel.InquiryVm = Mapper.Map<VmInquiryWasa>(viewModel.ComplaintVm);

            return viewModel;
        }

        public static DataTable GetStakeHolderServerSideListDenormalized(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            if (dtModel != null)
            {
                /*if(!string.IsNullOrEmpty(commaSeperatedStatuses) &&  commaSeperatedStatuses=="-1" )
                {
                    int userId = AuthenticationHandler.GetCookie().UserId;
                    DbUsers dbUser = DbUsers.GetActiveUser(userId);
                    List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, AuthenticationHandler.GetCookie().UserId, (int)Config.Permissions.StatusesForComplaintListing);

                    List<DbStatus> listDbStatuses =
                        BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                            listDbPermissionsAssignment);
                    commaSeperatedStatuses = Utility.GetCommaSepStrFromList(listDbStatuses.Select(n => n.Complaint_Status_Id).ToList());
                }*/

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
                        "PersonalInfo",
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
            ListingParamsModelBase paramsSchoolEducation = SetStakeholderListingParams(from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
            string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);
            
            return dt;
        }

        public static ListingParamsModelBase SetStakeholderListingParams(string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = "complaints.Person_Contact, PersonalInfo.Person_Address, complaints.Complaint_SubCategory_Name, complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as _complaint_Computed_Status,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time, complaints.Computed_Time_Passed_Since_Complaint_Launch, ";
            string innerJoinLogic =
                "INNER JOIN pitb.Person_Information PersonalInfo ON PersonalInfo.Person_id = complaints.Person_Id ";
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;

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
            paramsModel.UserHierarchyId = Convert.ToInt32(cookie.Hierarchy_Id);
            paramsModel.UserDesignationHierarchyId = Convert.ToInt32(cookie.User_Hierarchy_Id);
            paramsModel.ListingType = Convert.ToInt32(listingType);
            paramsModel.ProvinceId = cookie.ProvinceId;
            paramsModel.DivisionId = cookie.DivisionId;
            paramsModel.DistrictId = cookie.DistrictId;

            paramsModel.Tehsil = cookie.TehsilId;
            paramsModel.UcId = cookie.UcId;
            paramsModel.WardId = cookie.WardId;

            paramsModel.UserId = cookie.UserId;
            paramsModel.UserCategoryId1 = cookie.UserCategoryId1;
            paramsModel.UserCategoryId2 = cookie.UserCategoryId2;
            paramsModel.CheckIfExistInSrcId = 0;
            paramsModel.CheckIfExistInUserSrcId = 0;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.InnerJoinLogic = innerJoinLogic;
            paramsModel.SpType = spType;
            return paramsModel;
        }

        public static VmAddComplaintWasa GetVmAddComplaintMerged(VmAddComplaintWasa viewModel, int profileId = 0, int campaignId = 0)
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
                    }
                }
                viewModel.ComplaintVm.ListDynamicTextBox = listDynamicTextBox;
                viewModel.ComplaintVm.ListDynamicLabel = listDynamicLabel;
                viewModel.ComplaintVm.ListDynamicDropDown = listDynamicDropdown;
                viewModel.ComplaintVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                //viewModel.SuggestionVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.SuggestionVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.SuggestionVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;

                //viewModel.InquiryVm.ListDynamicTextBox = listDynamicTextBox;
                //viewModel.InquiryVm.ListDynamicDropDown = listDynamicDropdown;
                //viewModel.InquiryVm.ListDynamicDropDownServerSide = listDynamicDropdownServerSide;
            }

            viewModel.HardCopyComplaintIntoSuggestionAndInquiry();
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfoWasa>();
            viewModel.PersonalInfoVm = Mapper.Map<VmPersonalInfoWasa>(DbPersonInformation.GetPersonInformationByPersonId(profileId));

            // Copy complaint into suggestion
            Mapper.CreateMap<VmComplaintWasa, VmSuggestionWasa>();
            viewModel.SuggestionVm = Mapper.Map<VmSuggestionWasa>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Suggestion);
            viewModel.SuggestionVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.SuggestionVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);
            //viewModel.SuggestionVm.ListOfDepartment = DbDepartment(campaignId);


            // Copy complaint into inquiry
            Mapper.CreateMap<VmComplaintWasa, VmInquiryWasa>();
            viewModel.InquiryVm = Mapper.Map<VmInquiryWasa>(viewModel.ComplaintVm);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Inquiry);
            viewModel.InquiryVm.ListOfDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
            viewModel.InquiryVm.ListOfComplaintTypes = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);

            return viewModel;
        }

        public static Config.CommandMessage AddComplaint(VmAddComplaintWasa vm, bool isProfileEditing, bool isComplaintEditing)
        {
            #region Add Complaint Section

            DateTime nowDate = DateTime.Now;

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();

            VmPersonalInfoWasa vmPersonalInfo = vm.PersonalInfoVm;
            VmComplaintWasa vmComplaint = vm.ComplaintVm;
            VmSuggestionWasa vmSuggestion = vm.SuggestionVm;
            VmInquiryWasa vmInquiry = vm.InquiryVm;

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

            // complaint info
            paramDict.Add("@Id", -1);
            paramDict.Add("@Person_Id", vmPersonalInfo.Person_id.ToDbObj());
            switch (vm.currentComplaintTypeTab)
            {
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
                case VmAddComplaint.TabComplaintCombined:
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

            paramDict.Add("@Complaint_Status_Id", (isComplaintEditing) ? vmComplaint.Complaint_Status_Id.ToDbObj() : Config.ComplaintStatus.PendingFresh);//If complaint is adding then set complaint status to 1 (Pending(Fresh)
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
            //paramDict.Add("@Cnic_No", vmPersonalInfo.Cnic_No.ToDbObj());
            paramDict.Add("@Is_Cnic_Present", false.ToDbObj());
            paramDict.Add("@Cnic_No", vmPersonalInfo.Account_No.ToDbObj());
            paramDict.Add("@Account_No", vmPersonalInfo.Account_No.ToDbObj());
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
                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory), catRetainingHours) /*nowDate,
                    DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory), catRetainingHours*/);
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

            #endregion
            DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
            int complaintId = Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]);
            Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandlerWasa.SaveDyamicFieldsInDb(vm.ComplaintVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabSuggestion || vm.currentComplaintTypeTab == VmAddComplaint.TabSuggestionCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandlerWasa.SaveDyamicFieldsInDb(vm.SuggestionVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }
            else if (vm.currentComplaintTypeTab == VmAddComplaint.TabInquiryVm || vm.currentComplaintTypeTab == VmAddComplaint.TabInquiryVmCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandlerWasa.SaveDyamicFieldsInDb(vm.InquiryVm, Convert.ToInt32(cm.Value.Split('-')[1]));
            }

            if (cm.Status == Config.CommandStatus.Success && (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined)) // send message on complaint launch
            {
                if (!PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages))
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(vmPersonalInfo.Mobile_No,
                        (int)vmComplaint.Compaign_Id, Convert.ToInt32(cm.Value.Split('-')[1]),
                        (int)vmComplaint.Complaint_Category);
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId));
                }
            }

            //db.DbDynamicComplaintFields.Add();
            //SendMessage(Convert.ToInt32(cm.Value.Split('-')[1]), (int)Config.ComplaintStatus.PendingFresh);
            return cm;
        }

    }
}