using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.API;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Assignment;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.Custom.Police;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Dynamic;
using AutoMapper;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Models.View.Police;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Handler.ExportFileHandler;
using System.Dynamic;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlPolice
    {
        #region Agent

        public static DataTable GetComplaintListings(DataTableParamsModel dtModel, string fromDate, string toDate, string campaign, int complaintType, string spType, int listingType)
        {
            List<string> prefixStrList = new List<string> { "complaints", "campaign", "personalInfo", "complaints", "complaintType", "Statuses" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintNo" }, prefixStrList, dictFilterQuery);
            dynamic dynamicParams = new System.Dynamic.ExpandoObject();
            //dynamicParams.SelectionFields = " personalInfo.Mobile_No, complaints.District_Name, complaints.Department_Name, complaints.Complaint_SubCategory_Name,complaints.Is_Action_Completed,complaints.Complainant_Feedback_Total_Count,";
            dynamicParams.SelectionFields = " personalInfo.Mobile_No, complaints.District_Name,complaints.Is_Action_Completed,complaints.Complainant_Feedback_Total_Count,";

            //Dictionary<string,string> paramsDict = new Dictionary<string, string>()
            //{
            //    {"SelectionFields",extraSelection}
            //};
            ListingParamsAgent paramsComplaintListing = SetAgentListingParams(dynamicParams, dtModel, fromDate, toDate, campaign, (Config.ComplaintType)complaintType, spType, listingType);
            /*
            switch (new AuthenticationHandler().CmsCookie.Role)
            {
                case Config.Roles.Agent:
                    listOfComplaints = GetComplaintsOfAgents(fromDate, toDate, campaign);
                    break;
                case Config.Roles.AgentSuperVisor:
                    listOfComplaints = GetComplaintsAllComplaintsSupervisor(dtModel, fromDate, toDate, campaign);
                    break;

            }*/
            //return listOfComplaints;
            string queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }

        public static ListingParamsAgent SetAgentListingParams(dynamic dynamicParams, DataTableParamsModel dtParams, string fromDate, string toDate, string campaign, Config.ComplaintType complaintType, string spType, int listingType)
        {
            //string extraSelection = " personalInfo.Mobile_No, complaints.District_Name, complaints.Department_Name, complaints.Complaint_SubCategory_Name,complaints.Is_Action_Completed,complaints.Complainant_Feedback_Total_Count,";

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
            paramsModel.SelectionFields = dynamicParams.SelectionFields;
            paramsModel.SpType = spType;
            return paramsModel;
        }

        #endregion

        public static VmComplaintDetail GetComplaintDetail(DbComplaint dbComplaint /*int complaintId*/)
        {
            //List<DbComplaint> listComplaint = DbComplaint.GetListByComplaintId(complaintId);
            List<DbDynamicComplaintFields> listDynamicFields = DbDynamicComplaintFields.GetByComplaintId(dbComplaint.Id/*complaintId*/);
            VmComplaintDetail vmComplaintDetail = VmComplaintDetail.GetComplaintDetail(/*listComplaint.First()*/dbComplaint, listDynamicFields);
            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            vmComplaintDetail.vmPersonlInfo = Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetPersonInformationByPersonId((int)dbComplaint.Person_Id/*(int)listComplaint.FirstOrDefault().Person_Id)*/));
            vmComplaintDetail.currentStatusStr = Utility.GetAlteredStatus(Convert.ToInt32(vmComplaintDetail.Compaign_Id), vmComplaintDetail.currentStatusStr);
            return vmComplaintDetail;
        }

        public static VmAddComplaint GetVmAddComplaintMerged(VmAddComplaint viewModel, int profileId = 0, int campaignId = 0)
        {
            viewModel.ComplaintVm.Compaign_Id = campaignId;
            viewModel.PersonalInfoVm.Person_id = profileId;

            viewModel.ComplaintVm.ListOfProvinces = DbProvince.AllProvincesList();
            int? groupId = DbHierarchyCampaignGroupMapping.GetModelByCampaignId(campaignId, Config.Hierarchy.District);
            viewModel.ComplaintVm.ListOfDivision = DbDivision.GetByProvinceIdsStrAndGroupId("1", groupId);

            groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, Config.ComplaintType.Complaint);
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

        public static VmPolice.VmComplaintAction GetActionForm(int complaintId)
        {

            VmPolice.VmComplaintAction vmComplaintAction = new VmPolice.VmComplaintAction();

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            if (cmsCookie.Role == Config.Roles.Stakeholder)
            {
                vmComplaintAction.CanSaveComplaintAction = true;
            }
            else
            {
                vmComplaintAction.CanSaveComplaintAction = false;
            }

            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
            vmComplaintAction.ComplaintIdStr = dbComplaint.Compaign_Id + "-" + dbComplaint.Id;
            //vmComplaintAction.DbComplaint = dbComplaint;
            DbPoliceAction dbPoliceAction = DbPoliceAction.GetAction(complaintId, true);
            //List<DbDynamicComplaintFields> listDbDynamicFields = DbDynamicComplaintFields.GetBy(complaintId, "ComplaintAction");
            List<DbDynamicComplaintFields> listDbDynamicFields = null;
            if (dbPoliceAction == null)
            {
                listDbDynamicFields = new List<DbDynamicComplaintFields>();
                vmComplaintAction.CurrentStep = 0;
            }
            else
            {
                listDbDynamicFields = DbDynamicComplaintFields.GetBy(complaintId, dbPoliceAction.Id);
                vmComplaintAction.CurrentStep = Convert.ToInt32(dbPoliceAction.CurrentStep);
                vmComplaintAction.ListDbActionReportLogs = DbPoliceActionReportLogs.GetActionReportLogs(complaintId,
                    dbPoliceAction.Id);
            }
            List<DbComplaintSubType> listDbComplaintSubtypes = DbComplaintSubType.GetByComplaintType(Convert.ToInt32(dbComplaint.Complaint_Category), "ComplaintDisposal");

            vmComplaintAction.ListDbDynamicComplaintFields = listDbDynamicFields;
            vmComplaintAction.ListDbDisposalCategories = listDbComplaintSubtypes;
            vmComplaintAction.ListSelectDisposalCat =
              vmComplaintAction.ListDbDisposalCategories.Select(x =>
                                   new SelectListItem()
                                   {
                                       Text = x.Name,
                                       Value = x.Complaint_SubCategory + "___" + x.Name
                                   }).ToList();
            //vmComplaintAction.ListSelectDisposalCat =
            //    vmComplaintAction.ListDbDisposalCategories.ToSelectList("Complaint_SubCategory", "Name", null,
            //        "--Select--", 0);
            vmComplaintAction.ComplaintId = complaintId;
            return vmComplaintAction;
        }

        private static void SaveFormStepWise()
        {

            //DbPoliceAction dbPoliceAction = DbPoliceAction.GetAction()
        }

        private static Dictionary<string, int> DictControlIdsMapping = new Dictionary<string, int>
        {
            {"RadioCallWithin08Hours",48},
            {"CallWithin08HoursReason",49},
            {"RadioMeetingWithin24Hours",50},
            {"MeetingWithin24HoursReason",51},
                
            //-- Disposal Category --
            {"DisposalCatId",52},
            {"IdealAction",97},

            //-- Investigation Categories --
            {"RadioCompromiseInvestigationAffidavitAttached",81},
            {"RadioCompromiseInvestigationSignedReport",82},

            {"FalseInvestigationReason",53},
            {"RadioFalseInvestigationSignedReport",54},

            {"RedressedInvestigationReason",55},
            {"RadioRedressedInvestigationSignedReport",56},

            {"Non-CognizableInvestigationReason",57},
            {"RadioNon-CognizableInvestigationSignedReport",58},

            {"Matter-In-CourtInvestigationNameofCourt",59},
            {"RadioMatter-In-CourtInvestigationSignedReport",60},

            {"Other-DepartmentsInvestigationDepartmentName",61},
            {"RadioOther-DepartmentsInvestigationSignedReport",62},

            {"FIR-RegisteredInvestigationFirNo",63},
            {"FIR-RegisteredInvestigationUnderSection",64},
            {"FIR-RegisteredInvestigationDistrictInfo",65},
            {"FIR-RegisteredInvestigationPoliceStationInfo",66},

            {"RaptRegisteredInvestigationDDNo",67},
            {"RaptRegisteredInvestigationDetail",68},
            {"RaptRegisteredInvestigationDistrictInfo",69},
            {"RaptRegisteredInvestigationPoliceStationInfo",70},

            {"FIR-CancelledInvestigationFirNo",71},
            {"FIR-CancelledInvestigationUnderSection",72},
            {"FIR-CancelledInvestigationDistrictInfo",73},
            {"FIR-CancelledInvestigationPoliceStationInfo",74},

            {"RadioNotPursuing",75},

            {"ProceedingInvestigationInfo",76},
            {"RadioProceedingInvestigationSignedReport",77},

            {"RadioInvestigationChanged",78},
            {"InvestigationChangedInvestigationInfo",79},
            {"RadioInvestigationChangedSignedReport",80},



            //-- Departmental issues --
            {"FalseDepartmentalIssueDetail",83},
            {"RadioFalseDepartmentalIssueSignedReport",84},

            {"RedressedDepartmentalIssueDetail",85},
            {"RadioRedressedDepartmentalIssueSignedReport",86},

            {"MatterCourtNameAndWritNoDepartmentalIssueDetail",87},
            {"RadioMatterInCourtDepartmentalIssueSignedReport",88},

            {"NoMeritDepartmentalIssueDetail",89},
            {"RadioNoMeritDepartmentalIssueSignedReport",90},

            {"NotEligibleDepartmentalIssueDetail",91},
            {"RadioNotEligibleDepartmentalIssueSignedReport",92},

            {"ProceedingDepartmentalIssueDetail",93},
            {"RadioProceedingDepartmentalIssueSignedReport",94},

            {"RadioNotWillingToPursueDepartmentalIssue",95},


            //-- Statisfaction of complainant

            {"RadioSatisfactionOfComplainant", 96}

        };

        private static List<Pair<int, int, int>> _listStepWiseControlsRange;
        private static List<Pair<int, int, int>> ListStepWiseControlsRange
        {
            get
            {
                if (_listStepWiseControlsRange == null)
                {
                    _listStepWiseControlsRange = new List<Pair<int, int, int>>();
                    _listStepWiseControlsRange.Add(new Pair<int, int, int>(0, 0, 1));
                    _listStepWiseControlsRange.Add(new Pair<int, int, int>(1, 2, 3));
                    _listStepWiseControlsRange.Add(new Pair<int, int, int>(2, -1, -1));
                    _listStepWiseControlsRange.Add(new Pair<int, int, int>(3, -1, -1));
                    _listStepWiseControlsRange.Add(new Pair<int, int, int>(4, 4, DictControlIdsMapping.Count - 1));
                }
                return _listStepWiseControlsRange;
            }

        }
        public static Config.CommandMessage SaveComplaintAction(PoliceModel.AddAction addAction/*CustomForm.Post postedData*/)
        {
            CustomForm.Post postedData = addAction.PostedData;

            int complaintId = -1;
            // If call coming from other system Api
            if (addAction.SourceType == Config.SourceType.OtherSystemApi)
            {
                complaintId = Convert.ToInt32(addAction.ComplaintId);
            }
            else
            {
                complaintId = Convert.ToInt32(postedData.DictQueryParams["complaintId"]);
            }



            int endingStepIndex = Convert.ToInt32(postedData.GetElementValue("CurrentDivIndex"));
            int startingStepIndex = 0;

            DateTime nowDateTime = addAction.CreatedDateTime;//DateTime.Now;
            CMSCookie cmsCookie = addAction.Cookie;//AuthenticationHandler.GetCookie();
            DbPoliceAction dbPoliceAction = DbPoliceAction.GetAction(complaintId, false, true);


            DBContextHelperLinq db = new DBContextHelperLinq();

            if (dbPoliceAction == null)
            {
                dbPoliceAction = new DbPoliceAction();
                dbPoliceAction.Complaint_Id = complaintId;
                dbPoliceAction.CreatedDateTime = nowDateTime;
                dbPoliceAction.CurrentStep = 0;
                dbPoliceAction.IsComplete = false;
                dbPoliceAction.IsActive = true;

                db.DbPoliceAction.Add(dbPoliceAction);
                db.SaveChanges();
            }
            else
            {
                startingStepIndex = Convert.ToInt32(dbPoliceAction.CurrentStep);
            }

            List<DbPoliceAction> listDbPoliceAction = DbPoliceAction.GetActions(complaintId);
            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);


            DbPoliceActionReportLogs dbPoliceActionReportLogs = null;
            List<PostModel.File.Single> listFiles = null;
            List<DbAttachments> listDbAttachments = null;
            List<DbDynamicFormControls> listDynamicFormControls = DbDynamicFormControls.GetByControlIds(DictControlIdsMapping.Select(n => n.Value).ToList());
            List<CustomForm.ElementData> listElementData = postedData.ListElementData;

            //List<DbDynamicComplaintFields> listDynamicComplaintFields = new List<DbDynamicComplaintFields>();
            //DbDynamicComplaintFields dbDf = null;

            //DbDynamicFormControls dbDynamicControl = null;
            //CustomForm.ElementData elementData;

            // Sort according to the given list
            List<CustomForm.ElementData> listElementStepData = new List<CustomForm.ElementData>();
            List<string> listDictKeys = DictControlIdsMapping.Keys.ToList();
            //for (int i = 0; i < listKeys.Count; i++)
            //{
            //    CustomForm.ElementData tempElData = listElementData.Where(n => n.Key == listKeys[i]).FirstOrDefault();
            //    if (tempElData != null)
            //    {
            //        listElementDataSorted.Add(tempElData);
            //    }
            //}
            //List<string> listKeys = DictControlIdsMapping.Keys.ToList();
            //listElementData = listElementData.OrderBy(n => listDictKeys.IndexOf(n.Key)).ToList();

            Pair<int, int, int> stepWiseControlsRange = null;
            int startIndex, elemAfterStart;
            for (int currStep = startingStepIndex; currStep <= endingStepIndex; currStep++)
            {
                stepWiseControlsRange = ListStepWiseControlsRange.Where(n => n.Item1 == currStep).FirstOrDefault();
                listElementStepData.Clear();
                // Prepare element Data for step
                if (stepWiseControlsRange.Item2 >= 0)
                {
                    for (int i = stepWiseControlsRange.Item2; i <= stepWiseControlsRange.Item3; i++)
                    {
                        CustomForm.ElementData elementData =
                            listElementData.Where(n => n.Key == listDictKeys[i]).FirstOrDefault();
                        if (elementData != null)
                        {
                            listElementStepData.Add(elementData);
                        }
                    }
                }
                startIndex = stepWiseControlsRange.Item2;
                elemAfterStart = stepWiseControlsRange.Item3 - startIndex;

                if (currStep == 0) // Call the complainant within time
                {
                    dbPoliceAction.CallWithin08Hours = Utility.GetIntFromYesNo(postedData.GetElementValue("RadioCallWithin08Hours"));
                    dbPoliceAction.CallWithin08HoursReason = postedData.GetElementValue("CallWithin08HoursReason");
                    PopulatePoliceActionDynamicFields(complaintId, currStep, dbPoliceAction, db,
                        listElementStepData, listDynamicFormControls);
                    dbPoliceAction.Step1UpdatedDateTime = nowDateTime;
                    dbPoliceAction.Step1UpdatedBy = cmsCookie.UserId;
                }
                if (currStep == 1) // Meeting within 24 hours
                {
                    dbPoliceAction.MeetingWithin24Hours = Utility.GetIntFromYesNo(postedData.GetElementValue("RadioMeetingWithin24Hours"));
                    dbPoliceAction.MeetingWithin24HoursReason = postedData.GetElementValue("MeetingWithin24HoursReason");
                    PopulatePoliceActionDynamicFields(complaintId, currStep, dbPoliceAction, db,
                        listElementStepData, listDynamicFormControls);
                    dbPoliceAction.Step2UpdatedDateTime = nowDateTime;
                    dbPoliceAction.Step2UpdatedBy = cmsCookie.UserId;
                }
                if (currStep == 2) // Proceeding Interim Report
                {
                    List<CustomForm.ElementData> listInterimReport = listElementData.Where(n => n.Key.Contains("Proceeding_")).ToList();
                    //if (elementData.Key.Contains("Proceeding_") /*|| elementData.Key.Contains("FinalReport")*/)

                    List<int> listInterimReportIndexes = listInterimReport.Select(n => Convert.ToInt32(n.Key.Split('_')[n.Key.Split('_').Length - 1]))
                        .GroupBy(n => n).Select(n => n.Key)
                        .ToList();
                    //bool isIndexPresent = false;
                    int interimIndex = -1;




                    for (int i = 0; i < listInterimReportIndexes.Count; i++)
                    {
                        interimIndex = listInterimReportIndexes[i];
                        dbPoliceActionReportLogs = new DbPoliceActionReportLogs();
                        dbPoliceActionReportLogs.Filter1Name = Config.FILTER_NAME_POLICE_ACTION;
                        dbPoliceActionReportLogs.Filter1 = dbPoliceAction.Id;
                        dbPoliceActionReportLogs.UserId = cmsCookie.UserId;
                        dbPoliceActionReportLogs.Complaint_Id = complaintId;
                        dbPoliceActionReportLogs.CreatedDateTime = nowDateTime;
                        string proceedingDate = postedData.GetElementValue("Proceeding_Date_" + interimIndex);
                        if (!string.IsNullOrEmpty(proceedingDate))
                        {
                            dbPoliceActionReportLogs.ProceedingDateTime =
                                Convert.ToDateTime(postedData.GetElementValue("Proceeding_Date_" + interimIndex));
                            //DateTime.Now;
                        }
                        else
                        {
                            dbPoliceActionReportLogs.ProceedingDateTime = null;
                        }
                        dbPoliceActionReportLogs.Comments = postedData.GetElementValue("Proceeding_Detail_" + interimIndex);
                        dbPoliceActionReportLogs.TagId = Config.TAG_COMPLAINT_ACTION_INTERIM;
                        dbPoliceActionReportLogs.Type = Config.PoliceActionReportType.Interim;

                        // Upload Attachment
                        listFiles = new List<PostModel.File.Single>();
                        PostModel.File.Single postedFile = postedData.GetFile("file-Proceeding_Attachment_" + interimIndex);
                        if (postedFile != null)
                        {
                            listFiles.Add(postedFile);
                            listDbAttachments = FileUploadHandler.UploadMultipleFiles(new PostModel.File(listFiles) /*listFiles*/, Config.AttachmentReferenceType.Add, dbComplaint.Compaign_Id + "-" + complaintId, complaintId, Config.TAG_COMPLAINT_ACTION_INTERIM);
                            dbPoliceActionReportLogs.AttachmentRefId = listDbAttachments.FirstOrDefault().Id;
                        }
                        db.DbPoliceActionReportLogs.Add(dbPoliceActionReportLogs);
                    }
                    if (listInterimReportIndexes.Count > 0)
                    {
                        dbPoliceAction.Step3UpdatedDateTime = nowDateTime;
                        dbPoliceAction.Step3UpdatedBy = cmsCookie.UserId;
                    }

                    // If status is pending fresh 
                    /*if (dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.PendingFresh
                        || dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.PendingReopened
                        || dbComplaint.Complaint_Computed_Status_Id == (int)Config.ComplaintStatus.UnsatisfactoryClosed) // If status is pending fresh
                    {
                        StatusHandler.ChangeStatus(cmsCookie.UserId, complaintId, (int)Config.ComplaintStatus.InProceeding, "Complaint has gone in proceeding");
                    }*/
                }
                if (currStep == 3) // Final Report
                {
                    // Add Final Report
                    dbPoliceActionReportLogs = new DbPoliceActionReportLogs();
                    dbPoliceActionReportLogs.Filter1Name = Config.FILTER_NAME_POLICE_ACTION;
                    dbPoliceActionReportLogs.Filter1 = dbPoliceAction.Id;
                    dbPoliceActionReportLogs.UserId = cmsCookie.UserId;
                    dbPoliceActionReportLogs.Complaint_Id = complaintId;
                    dbPoliceActionReportLogs.CreatedDateTime = nowDateTime;
                    dbPoliceActionReportLogs.ProceedingDateTime = Convert.ToDateTime(postedData.GetElementValue("FinalReportDate"));  //DateTime.Now;
                    dbPoliceActionReportLogs.Comments = postedData.GetElementValue("FinalReportDetail");
                    dbPoliceActionReportLogs.TagId = Config.TAG_COMPLAINT_ACTION_FINAL;
                    dbPoliceActionReportLogs.Type = Config.PoliceActionReportType.Final;
                    //FinalReportDetailAttachment
                    listFiles = new List<PostModel.File.Single>
                    {
                        postedData.GetFile("file-FinalReportDetailAttachment")
                    };
                    listDbAttachments = FileUploadHandler.UploadMultipleFiles(new PostModel.File(listFiles)/*listFiles*/, Config.AttachmentReferenceType.Add, dbComplaint.Compaign_Id + "-" + complaintId, complaintId, Config.TAG_COMPLAINT_ACTION_INTERIM);
                    dbPoliceActionReportLogs.AttachmentRefId = listDbAttachments.FirstOrDefault().Id;
                    db.DbPoliceActionReportLogs.Add(dbPoliceActionReportLogs);

                    dbPoliceAction.Step4UpdatedDateTime = nowDateTime;
                    dbPoliceAction.Step4UpdatedBy = cmsCookie.UserId;


                }
                if (currStep == 4) // Disposal
                {
                    string disposalCatStr = postedData.GetElementValue("DisposalCatId");
                    string[] disposalCat = Utility.Split(disposalCatStr, Config.Separator);

                    dbPoliceAction.DisposalCategoryId = Convert.ToInt32(disposalCat[0]);
                    dbPoliceAction.DisposalCategoryName = disposalCat[1];

                    dbPoliceAction.DisposalCategoryWillBeUseInCaseOf = postedData.GetElementValue("IdealAction");


                    //dbPoliceAction.SatisfactionOfComplainant = Utility.GetIntFromYesNo(postedData.GetElementValue("RadioSatisfactionOfComplainant"));

                    dbPoliceAction.SatisfactionOfComplainant = 1;

                    PopulatePoliceActionDynamicFields(complaintId, currStep, dbPoliceAction, db,
                        listElementStepData, listDynamicFormControls);

                    dbPoliceAction.Step5UpdatedDateTime = nowDateTime;
                    dbPoliceAction.Step5UpdatedBy = cmsCookie.UserId;

                    dbPoliceAction.CompletedDateTime = addAction.CreatedDateTime;//DateTime.Now;
                    dbPoliceAction.IsComplete = true;
                    dbPoliceAction.IsActive = true;

                    if (dbPoliceAction.SatisfactionOfComplainant == 2) // if complainant is not satisfied
                    {
                        StatusHandler.ChangeStatus(cmsCookie.UserId, complaintId,
                            (int)Config.ComplaintStatus.PendingReopened, "Complainant is not satisfied");
                        dbPoliceAction.IsActive = false;
                    }
                    else if (dbPoliceAction.SatisfactionOfComplainant == 1) // if complainant is satisfied
                    {
                        DbUsers dbUser = DbUsers.GetUser(cmsCookie.UserId);
                        StatusHandler.ChangeStatus(cmsCookie.UserId, complaintId,
                            (int)Config.ComplaintStatus.ResolvedUnverified, string.Format("Resolved (Unverified) by {0}-{1}",
                        dbUser.Designation.NullToEmptyStr().Trim(), DbUsers.GetHierarchyVal(dbUser)));
                    }

                }

                dbPoliceAction.CurrentStep = currStep + 1;
            }

            if (endingStepIndex == 2) // If proceeding keep step to same 
            {
                dbPoliceAction.CurrentStep = dbPoliceAction.CurrentStep - 1;
            }

            //for (int i = 0; i < listElementData.Count; i++)
            //{
            //    elementData = listElementData[i];
            //    int controlId = -1;
            //    if (DictControlIdsMapping.TryGetValue(elementData.Key, out controlId))
            //    {
            //        dbDynamicControl =
            //            listDynamicFormControls.Where(n => n.Id == controlId)
            //                .FirstOrDefault();
            //    }
            //    else
            //    {
            //        dbDynamicControl = null;
            //    }

            //    if (dbDynamicControl != null)
            //    {
            //        if (elementData.DictAttributes["type"] == "radio" || elementData.DictAttributes["type"] == "select")
            //        {
            //            dbDf = new DbDynamicComplaintFields();
            //            dbDf.ComplaintId = complaintId;
            //            dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
            //            dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);

            //            dbDf.ControlId = dbDynamicControl.Id;
            //            dbDf.FieldName = dbDynamicControl.FieldName;
            //            //dbDf.FieldName = formField.Name;
            //            if (!string.IsNullOrEmpty(elementData.Value) || !elementData.Value.Equals("-1"))
            //            {
            //                string[] tempStrArr = elementData.Value.Split(new string[] { Config.Separator },
            //                    StringSplitOptions.None);
            //                dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
            //                dbDf.FieldValue = tempStrArr[1].ToString().Trim();
            //            }
            //            else
            //            {
            //                dbDf.CategoryTypeId = -1;
            //                dbDf.FieldValue = null;
            //            }
            //            dbDf.TagId = dbDynamicControl.TagId;
            //        }
            //        else if (elementData.DictAttributes["type"] == "text")
            //        {
            //            dbDf = new DbDynamicComplaintFields();
            //            dbDf.ComplaintId = complaintId;
            //            dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
            //            dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
            //            dbDf.CategoryTypeId = -1;
            //            dbDf.ControlId = dbDynamicControl.Id;
            //            dbDf.FieldName = dbDynamicControl.FieldName;
            //            //dbDf.FieldName = formField.Name;
            //            if (!string.IsNullOrEmpty(elementData.Value))
            //            {
            //                dbDf.FieldValue = elementData.Value.Trim();
            //            }
            //            dbDf.TagId = dbDynamicControl.TagId;
            //        }
            //        //listDynamicComplaintFields.Add(dbDf);
            //        db.DbDynamicComplaintFields.Add(dbDf);
            //    }
            //}



            //// Add Interim Report
            //List<CustomForm.ElementData> listInterimReport = listElementData.Where(n => n.Key.Contains("Proceeding_")).ToList();
            ////if (elementData.Key.Contains("Proceeding_") /*|| elementData.Key.Contains("FinalReport")*/)

            //List<int> listInterimReportIndexes = listInterimReport.Select(n => Convert.ToInt32(n.Key.Split('_')[n.Key.Split('_').Length - 1]))
            //    .GroupBy(n => n).Select(n => n.Key)
            //    .ToList();
            ////bool isIndexPresent = false;
            //int interimIndex = -1;
            //DbPoliceActionReportLogs dbPoliceActionReportLogs = null;
            //List<HttpPostedFileBase> listFiles = null;
            //List<DbAttachments> listDbAttachments = null;

            //for (int i = 0; i < listInterimReportIndexes.Count; i++)
            //{
            //    interimIndex = listInterimReportIndexes[i];
            //    dbPoliceActionReportLogs = new DbPoliceActionReportLogs();
            //    dbPoliceActionReportLogs.Complaint_Id = complaintId;
            //    dbPoliceActionReportLogs.CreatedDateTime = DateTime.Now;
            //    dbPoliceActionReportLogs.ProceedingDateTime = Convert.ToDateTime(postedData.GetElementValue("Proceeding_Date_" + interimIndex));  //DateTime.Now;
            //    dbPoliceActionReportLogs.Comments = postedData.GetElementValue("Proceeding_Detail_" + interimIndex);
            //    dbPoliceActionReportLogs.TagId = Config.TAG_COMPLAINT_ACTION_INTERIM;
            //    dbPoliceActionReportLogs.Type = Config.PoliceActionReportType.Interim;

            //    // Upload Attachment
            //    listFiles = new List<HttpPostedFileBase>
            //    {
            //        postedData.GetFile("file-Proceeding_Attachment_" + interimIndex)
            //    };
            //    //listDbAttachments = FileUploadHandler.UploadMultipleFiles(listFiles, Config.AttachmentReferenceType.Add, dbComplaint.Compaign_Id +"-"+ complaintId, complaintId,Config.TAG_COMPLAINT_ACTION_INTERIM);
            //    dbPoliceActionReportLogs.AttachmentRefId = listDbAttachments.FirstOrDefault().Id;
            //    db.DbPoliceActionReportLogs.Add(dbPoliceActionReportLogs);
            //}

            //// Add Final Report
            //dbPoliceActionReportLogs = new DbPoliceActionReportLogs();
            //dbPoliceActionReportLogs.Complaint_Id = complaintId;
            //dbPoliceActionReportLogs.CreatedDateTime = DateTime.Now;
            //dbPoliceActionReportLogs.ProceedingDateTime = Convert.ToDateTime(postedData.GetElementValue("FinalReportDate"));  //DateTime.Now;
            //dbPoliceActionReportLogs.Comments = postedData.GetElementValue("FinalReportDetail");
            //dbPoliceActionReportLogs.TagId = Config.TAG_COMPLAINT_ACTION_FINAL;
            //dbPoliceActionReportLogs.Type = Config.PoliceActionReportType.Final;
            ////FinalReportDetailAttachment
            //listFiles = new List<HttpPostedFileBase>
            //{
            //    postedData.GetFile("FinalReportDetailAttachment")
            //};
            ////listDbAttachments = FileUploadHandler.UploadMultipleFiles(listFiles, Config.AttachmentReferenceType.Add, dbComplaint.Compaign_Id +"-"+ complaintId, complaintId,Config.TAG_COMPLAINT_ACTION_INTERIM);
            //dbPoliceActionReportLogs.AttachmentRefId = listDbAttachments.FirstOrDefault().Id;
            //db.DbPoliceActionReportLogs.Add(dbPoliceActionReportLogs);

            // Update entry in db
            if (dbPoliceAction.IsActive == true) // It means log entry is still present
            {
                dbComplaint.Current_Action_Step = dbPoliceAction.CurrentStep;
                dbComplaint.Is_Action_Completed = dbPoliceAction.IsComplete;
            }
            else
            {
                dbComplaint.Current_Action_Step = 0;
                dbComplaint.Is_Action_Completed = false;
            }
            dbComplaint.Action_Logs_Count = listDbPoliceAction.Count;

            db.DbComplaints.Attach(dbComplaint);
            db.Entry(dbComplaint).Property(x => x.Current_Action_Step).IsModified = true;
            db.Entry(dbComplaint).Property(x => x.Is_Action_Completed).IsModified = true;
            db.Entry(dbComplaint).Property(x => x.Action_Logs_Count).IsModified = true;


            db.DbPoliceAction.Add(dbPoliceAction);
            db.Entry(dbPoliceAction).State = EntityState.Modified;




            // Save action wise logs in db

            // Post data to crm
            //if (Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police) // current deployment server
            //if (addAction.SourceType == Config.SourceType.Web /*Config.SourceType.OtherSystemApi*/)
            if ((Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police ||
                    Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Local) &&
                    addAction.SourceType == Config.SourceType.Web)
            {
                //addAction.PostedData = postedData;
                addAction.SourceType = Config.SourceType.OtherSystemApi;
                addAction.OtherSystemId = Config.OtherSystemId.Police;
                addAction.ComplaintId = dbComplaint.Ref_Complaint_Id.ToString();
                //addAction.ComplaintId = dbComplaint.Ref_Complaint_Id.ToString(); //complaintId.ToString();

                //string url = "https://crm.punjab.gov.pk/rest/api/Police/PostAction";
                //string url = "http://localhost:2826/api/Police/PostAction";
                string url = Config.DictUrl["Police-PostAction"];
                Dictionary<string, string> headersDict = new Dictionary<string, string>();
                headersDict.Add("SystemName", "Police8787");
                headersDict.Add("SystemUserName", "Police");
                headersDict.Add("Password", "p@4o!li#0c@e");
                headersDict.Add("Username", "P#0el@9ic$e");
                string response = APIHelper.HttpClientGetResponse<string, PoliceModel.AddAction>(url, addAction, headersDict);
                // DBContextHelperLinq db = new DBContextHelperLinq();
                // Call Api
            }
            db.SaveChanges();
            // end post data to crm


            return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint action added successfully Id = " + dbComplaint.Compaign_Id + "-" + dbComplaint.Id);
        }

        public static void UpdateStatusComments()
        {
            try
            {
                List<DbUsers> listDbUsers = DbUsers.GetUsersAgainstCampaign(78);
                DBContextHelperLinq db = new DBContextHelperLinq();
                List<DbComplaint> listDbComplaints = DbComplaint.GetByCampaignId(db, 78, Config.ComplaintType.Complaint);
                listDbComplaints = listDbComplaints.Where(n => n.Complaint_Status_Id == 2).ToList();
                //listDbComplaints = list
                DbComplaint dbComplaint = null;
                DbUsers dbUser = null;
                for (int i = 0; i < listDbComplaints.Count; i++)
                {
                    dbComplaint = listDbComplaints[i];
                    dbUser = listDbUsers.Where(n => n.User_Id == dbComplaint.Status_ChangedBy).First();
                    string statusChangeComments = string.Format("Resolved (Unverified) by {0}-{1}",
                        dbUser.Designation.NullToEmptyStr().Trim(), DbUsers.GetHierarchyVal(dbUser));
                    dbComplaint.StatusChangedComments = statusChangeComments;
                    DBContextHelperLinq.UpdateEntity(dbComplaint, db, new List<string> { "StatusChangedComments" });

                    DbComplaintStatusChangeLog dbComplaintStatusChangeLog =
                        DbComplaintStatusChangeLog.GetLastStatusChangeOfParticularComplaint(dbComplaint.Id, db);

                    if (dbComplaintStatusChangeLog.Comments == "Complainant is satisfied")
                    {
                        dbComplaintStatusChangeLog.Comments = statusChangeComments;
                        DBContextHelperLinq.UpdateEntity(dbComplaintStatusChangeLog, db, new List<string> { "Comments" });
                    }
                    //DBContextHelperLinq.UpdateEntity();
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Config.CommandMessage SubmitComplaintFeedback(PoliceModel.AddFeedback addFeedback/*CustomForm.Post postedData*/)
        {
            CustomForm.Post postedData = addFeedback.PostedData;
            CMSCookie cmsCookie = addFeedback.Cookie;
            int complaintId = -1;
            // If call coming from other system Api
            if (addFeedback.SourceType == Config.SourceType.OtherSystemApi)
            {
                complaintId = Convert.ToInt32(addFeedback.ComplaintId);
            }
            else
            {
                complaintId = Convert.ToInt32(postedData.DictQueryParams["complaintId"]);
            }

            //CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            //int complaintId = Convert.ToInt32(postedData.DictQueryParams["complaintId"]);
            DBContextHelperLinq db = new DBContextHelperLinq();

            DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId, db);
            DbComplainantFeedbackLog dbFeedbackLog = new DbComplainantFeedbackLog();
            dbFeedbackLog.SubmittedByUserId = cmsCookie.UserId;
            dbFeedbackLog.ComplainantId = dbComplaint.Person_Id;
            dbFeedbackLog.ComplaintId = complaintId;
            dbFeedbackLog.CampaignId = dbComplaint.Compaign_Id;

            string elemVal = postedData.GetElementValue("RadioCallWithin08Hours");
            dbFeedbackLog.CallWithin08HoursStatusId = Utility.GetIntFromYesNo(elemVal);
            dbFeedbackLog.CallWithin08HoursStatusIdStr = Utility.GetKeyVal(elemVal).Value;
            dbFeedbackLog.CallWithin08HoursStatusComments = postedData.GetElementValue("CallWithin08HoursReason");

            elemVal = postedData.GetElementValue("RadioConferenceCallArranged");
            dbFeedbackLog.ConferenceCallArrangedId = Utility.GetIntFromYesNo(elemVal);
            dbFeedbackLog.ConferenceCallArrangedIdStr = Utility.GetKeyVal(elemVal).Value;


            elemVal = postedData.GetElementValue("RadioMeetingWithin24Hours");
            dbFeedbackLog.MeetingWithin24HoursStatusId = Utility.GetIntFromYesNo(elemVal);
            dbFeedbackLog.MeetingWithin24HoursStatusIdStr = Utility.GetKeyVal(elemVal).Value;
            dbFeedbackLog.MeetingWithin24HoursStatusComments = postedData.GetElementValue("MeetingWithin24HoursReason");

            elemVal = postedData.GetElementValue("RadioProceedingWithin48Hours");
            dbFeedbackLog.ProceedingWithin48HoursStatusId = Utility.GetIntFromYesNo(elemVal);
            dbFeedbackLog.ProceedingWithin48HoursStatusIdStr = Utility.GetKeyVal(elemVal).Value;
            dbFeedbackLog.ProceedingWithin48HoursStatusComments = postedData.GetElementValue("ProceedingWithin48HoursReason");

            elemVal = postedData.GetElementValue("RadioSatisfactionOfComplainant");
            dbFeedbackLog.FeedbackStatusId = Utility.GetIntFromYesNo(elemVal);
            dbFeedbackLog.FeedbackStatusIdStr = Utility.GetKeyVal(elemVal).Value;
            dbFeedbackLog.FeedbackStatusComments = postedData.GetElementValue("SatisfactionOfComplainantReason");

            dbFeedbackLog.CreatedDateTime = DateTime.Now;
            dbFeedbackLog.IsCurrentlyActive = true;

            DbComplainantFeedbackLog.InsertLog(dbFeedbackLog, db);

            // Update complaint feedback status
            db.DbComplaints.Attach(dbComplaint);
            dbComplaint.Complainant_Feedback_Status_Id = dbFeedbackLog.FeedbackStatusId;
            db.Entry(dbComplaint).Property(n => n.Complainant_Feedback_Status_Id).IsModified = true;

            dbComplaint.Complainant_Feedback_Status_Id_Str = dbFeedbackLog.FeedbackStatusIdStr;
            db.Entry(dbComplaint).Property(n => n.Complainant_Feedback_Status_Id_Str).IsModified = true;

            dbComplaint.Complainant_Feedback_Status_Id_Comment = dbFeedbackLog.FeedbackStatusComments;
            db.Entry(dbComplaint).Property(n => n.Complainant_Feedback_Status_Id_Comment).IsModified = true;

            dbComplaint.Complainant_Feedback_Total_Count++;
            db.Entry(dbComplaint).Property(n => n.Complainant_Feedback_Total_Count).IsModified = true;
            if (dbFeedbackLog.FeedbackStatusId == 2) // if complainant is not satisfied
            {
                dbComplaint.Current_Action_Step = 0;
                db.Entry(dbComplaint).Property(n => n.Current_Action_Step).IsModified = true;
                dbComplaint.Is_Action_Completed = false;
                db.Entry(dbComplaint).Property(n => n.Is_Action_Completed).IsModified = true;

                DbPoliceAction dbPoliceAction = DbPoliceAction.GetAction(complaintId, true, true, db);
                db.DbPoliceAction.Attach(dbPoliceAction);

                dbPoliceAction.IsActive = false;
                db.Entry(dbComplaint).Property(n => n.Is_Action_Completed).IsModified = true;
            }

            if (dbFeedbackLog.FeedbackStatusId == 2) // if complainant is not satisfied
            {
                dbComplaint.CNFP_Feedback_Id = 2;
                dbComplaint.CNFP_Feedback_Comments = "Complainant dissatisfied";
                dbComplaint.CNFP_Feedback_Value = "Dissatisfied";
            }
            else if (dbFeedbackLog.FeedbackStatusId == 1)// if complainant is satisfied
            {
                dbComplaint.CNFP_Feedback_Id = 1;
                dbComplaint.CNFP_Feedback_Comments = "Complainant satisfied";
                dbComplaint.CNFP_Feedback_Value = "Satisfied";
            }

            db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Id).IsModified = true;
            db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Comments).IsModified = true;
            db.Entry(dbComplaint).Property(n => n.CNFP_Feedback_Value).IsModified = true;

            if (dbFeedbackLog.FeedbackStatusId == 2) // if complainant is not satisfied
            {
                StatusHandler.ChangeStatus(cmsCookie.UserId, complaintId, (int)Config.ComplaintStatus.PendingReopened,
                    "Complainant feedback (Dissatisfied)");
            }
            else if (dbFeedbackLog.FeedbackStatusId == 1)// if complainant is satisfied
            {
                StatusHandler.ChangeStatus(cmsCookie.UserId, complaintId, (int)Config.ComplaintStatus.ResolvedVerified,
                    "Complainant feedback (Satisfied)");
            }

            db.SaveChanges();

            //if (addFeedback.SourceType == Config.SourceType.Web /*Config.SourceType.OtherSystemApi*/)

            if ((Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police ||
                Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Local) &&
                addFeedback.SourceType == Config.SourceType.Web)
            {
                //addAction.PostedData = postedData;
                addFeedback.SourceType = Config.SourceType.OtherSystemApi;
                addFeedback.OtherSystemId = Config.OtherSystemId.Police;
                addFeedback.ComplaintId = dbComplaint.Ref_Complaint_Id.ToString();
                //addAction.ComplaintId = dbComplaint.Ref_Complaint_Id.ToString(); //complaintId.ToString();

                //string url = "https://crm.punjab.gov.pk/rest/api/Police/PostAction";
                //string url = "http://localhost:2826/api/Police/PostAction";
                string url = Config.DictUrl["Police-PostFeedback"];
                Dictionary<string, string> headersDict = new Dictionary<string, string>();
                headersDict.Add("SystemName", "Police8787");
                headersDict.Add("SystemUserName", "Police");
                headersDict.Add("Password", "p@4o!li#0c@e");
                headersDict.Add("Username", "P#0el@9ic$e");
                string response = APIHelper.HttpClientGetResponse<string, PoliceModel.AddFeedback>(url, addFeedback, headersDict);
                // DBContextHelperLinq db = new DBContextHelperLinq();
                // Call Api
            }

            return new Config.CommandMessage(Config.CommandStatus.Success, "Complaint feedback submitted successfully Complaint Id = " + dbComplaint.Compaign_Id + "-" + dbComplaint.Id);
        }

        private static void PopulatePoliceActionDynamicFields(int complaintId, int currentStep, DbPoliceAction dbPoliceAction, DBContextHelperLinq db, List<CustomForm.ElementData> listElementData, List<DbDynamicFormControls> listDynamicFormControls)
        {
            DbDynamicComplaintFields dbDf = null;
            DbDynamicFormControls dbDynamicControl = null;
            CustomForm.ElementData elementData;
            for (int i = 0; i < listElementData.Count; i++)
            {
                elementData = listElementData[i];
                int controlId = -1;
                if (DictControlIdsMapping.TryGetValue(elementData.Key, out controlId))
                {
                    dbDynamicControl =
                        listDynamicFormControls.Where(n => n.Id == controlId)
                            .FirstOrDefault();
                }
                else
                {
                    dbDynamicControl = null;
                }

                if (dbDynamicControl != null)
                {
                    if (elementData.DictAttributes["type"] == "radio" || elementData.DictAttributes["type"] == "select")
                    {
                        dbDf = new DbDynamicComplaintFields();
                        dbDf.ComplaintId = complaintId;
                        dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.MultiSelection);
                        dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.MainCategory);

                        dbDf.ControlId = dbDynamicControl.Id;
                        dbDf.FieldName = dbDynamicControl.FieldName;
                        //dbDf.FieldName = formField.Name;
                        if (!string.IsNullOrEmpty(elementData.Value) && !elementData.Value.Equals("-1"))
                        {
                            string[] tempStrArr = elementData.Value.Split(new string[] { Config.Separator },
                                StringSplitOptions.None);
                            dbDf.CategoryTypeId = Convert.ToInt32(tempStrArr[0]);
                            dbDf.FieldValue = tempStrArr[1].ToString().Trim();
                        }
                        else
                        {
                            dbDf.CategoryTypeId = -1;
                            dbDf.FieldValue = null;
                        }
                        dbDf.TagId = dbDynamicControl.TagId;
                    }
                    else if (elementData.DictAttributes["type"] == "text")
                    {
                        dbDf = new DbDynamicComplaintFields();
                        dbDf.ComplaintId = complaintId;
                        dbDf.SaveTypeId = Convert.ToInt32(Config.DynamicFieldType.SingleText);
                        dbDf.CategoryHierarchyId = Convert.ToInt32(Config.CategoryHierarchyType.None);
                        dbDf.CategoryTypeId = -1;
                        dbDf.ControlId = dbDynamicControl.Id;
                        dbDf.FieldName = dbDynamicControl.FieldName;
                        //dbDf.FieldName = formField.Name;
                        if (!string.IsNullOrEmpty(elementData.Value))
                        {
                            dbDf.FieldValue = elementData.Value.Trim();
                        }
                        dbDf.TagId = dbDynamicControl.TagId;
                    }
                    //listDynamicComplaintFields.Add(dbDf);
                    dbDf.Filter1Name = Config.FILTER_NAME_POLICE_ACTION;
                    dbDf.Filter1 = dbPoliceAction.Id;
                    dbDf.Filter2Name = Config.FILTER_NAME_POLICE_ACTION_CURRENT_STEP;
                    dbDf.Filter2 = currentStep;
                    db.DbDynamicComplaintFields.Add(dbDf);
                }
            }
        }

        private static ConfigCatWiseDynamicForm _formSubmitConfig;
        public static ConfigCatWiseDynamicForm FormSubmitConfig
        {
            get
            {
                if (_formSubmitConfig == null)
                {
                    _formSubmitConfig = new ConfigCatWiseDynamicForm();

                    List<ConfigCatWiseDynamicForm.FormField> listFormField = null;
                    ConfigCatWiseDynamicForm.Category catInfo = null;
                    // On Department change
                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.departmentId", Value = "153" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 25, Type = Config.DynamicControlType.DropDownList, Name = "PersonType", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 26, Type = Config.DynamicControlType.DropDownList, Name = "ApplicationSubmission", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 27, Type = Config.DynamicControlType.TextBox, Name = "DateOfApplicationSubmission", IsRequired = false });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 28, Type = Config.DynamicControlType.TextBox, Name = "DetailOfApplicationSubmission", IsRequired = false });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.departmentId", Value = "155" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 29, Type = Config.DynamicControlType.DropDownList, Name = "OfficerType", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 30, Type = Config.DynamicControlType.TextBox, Name = "OfficerName", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 31, Type = Config.DynamicControlType.TextBox, Name = "OfficerRank", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 32, Type = Config.DynamicControlType.TextBox, Name = "OfficerNo", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 33, Type = Config.DynamicControlType.TextBox, Name = "OfficerPosted", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    // Subcategory validation
                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_Category", Value = "522" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 35, Type = Config.DynamicControlType.DropDownList, Name = "InvestigationNo", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_Category", Value = "523" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 35, Type = Config.DynamicControlType.DropDownList, Name = "ArrestOfAccused", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_Category", Value = "524" };// Recovery of Abductees
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 36, Type = Config.DynamicControlType.DropDownList, Name = "Sex", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 37, Type = Config.DynamicControlType.DropDownList, Name = "Age", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 38, Type = Config.DynamicControlType.DropDownList, Name = "DistrictInfo", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 39, Type = Config.DynamicControlType.DropDownList, Name = "PoliceStationInfo", IsRequired = true });
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 40, Type = Config.DynamicControlType.TextBox, Name = "FIRNoInvestigation", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    // Tertiary Category
                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "1796" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 41, Type = Config.DynamicControlType.TextBox, Name = "CrimeAgainstPropertyOther", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "1818" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 42, Type = Config.DynamicControlType.TextBox, Name = "CrimeAgainstPersonOther", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "2087" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 43, Type = Config.DynamicControlType.TextBox, Name = "PoliceOther", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "2093" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 44, Type = Config.DynamicControlType.TextBox, Name = "PoliceServicesOther", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "2094" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 45, Type = Config.DynamicControlType.TextBox, Name = "PoliceServicesComplaintAgainst15Service", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "2095" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 46, Type = Config.DynamicControlType.TextBox, Name = "PoliceServicesComplaintAgainstKhidmatMarkaz", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);

                    listFormField = new List<ConfigCatWiseDynamicForm.FormField>();
                    catInfo = new ConfigCatWiseDynamicForm.Category() { Name = "ComplaintVm.Complaint_SubCategory", Value = "2100" };
                    listFormField.Add(new ConfigCatWiseDynamicForm.FormField() { ControlId = 47, Type = Config.DynamicControlType.TextBox, Name = "OtherOther", IsRequired = true });
                    _formSubmitConfig.DictConfig.Add(catInfo, listFormField);
                }
                return _formSubmitConfig;
            }
        }

        public static Config.CommandMessage AddComplaint(PoliceModel.AddComplaint addComplaintModel/*VmAddComplaint vm, HttpFileCollectionBase files, NameValueCollection formCollection, bool isProfileEditing, bool isComplaintEditing*/)
        {
            VmAddComplaint vm = addComplaintModel.VmAddComplaint;
            //HttpFileCollectionBase files = addComplaintModel.Files;
            //NameValueCollection formCollection = addComplaintModel.FormCollection;
            Dictionary<string, string> dictFormCollection = addComplaintModel.FormCollectionDict;
            PostModel.File postedFiles = addComplaintModel.ListPostedFiles;
            bool isProfileEditing = addComplaintModel.IsProfileEditing;
            bool isComplaintEditing = addComplaintModel.IsComplaintEditing;

            // Business logic for additional Fields
            bool isFormValid = FormSubmitConfig.IsFormValid(dictFormCollection);

            if (!isFormValid)
            {
                Config.CommandMessage exceptionMsg = new Config.CommandMessage(Config.CommandStatus.Exception, "An error has occured");
                return exceptionMsg;
            }

            // End Business logic for dynamic Fields


            #region Add Complaint Section

            FileUploadHandler.FileValidationStatus validationStatus = FileUploadHandler.GetFileValidationStatus(postedFiles);

            DateTime nowDate = addComplaintModel.CreatedDateTime;//DateTime.Now;

            CMSCookie cmsCookie = addComplaintModel.Cookie;//AuthenticationHandler.GetCookie();

            VmPersonalInfo vmPersonalInfo = vm.PersonalInfoVm;
            VmComplaint vmComplaint = vm.ComplaintVm;
            VmSuggestion vmSuggestion = vm.SuggestionVm;
            VmInquiry vmInquiry = vm.InquiryVm;

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            //vm.ComplaintVm.Division_Id = DbDistrict.GetById((int)vm.ComplaintVm.District_Id).Division_Id;

            // complaint info

            vmComplaint.Province_Id = 1;
            if (vmComplaint.Ward_Id != null)
            {
                if (vmComplaint.Ward_Id == -1)
                {
                    vmComplaint.Ward_Id = null;
                }
                else
                {
                    vmComplaint.UnionCouncil_Id = DbWards.GetByWardId(vmComplaint.Ward_Id).Uc_Id;
                    vmComplaint.Tehsil_Id =
                        DbUnionCouncils.GetById(Convert.ToInt32(vmComplaint.UnionCouncil_Id)).Tehsil_Id;
                }
            }
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
            paramDict.Add("@Created_Date", addComplaintModel.CreatedDateTime.ToDbObj());
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
                //int? tehsilTestId = (paramDict["@Tehsil_Id"] == DBNull.Value) ? null : (int?)paramDict["@Tehsil_Id"];
                List<Pair<int?, int?>> listHierarchyPair = new List<Pair<int?, int?>>
                {
                    new Pair<int?, int?>((int?)Config.Hierarchy.Province, (int?)paramDict["@Province_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Division, (int?)paramDict["@Division_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.District, (int?)paramDict["@District_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Tehsil, (paramDict["@Tehsil_Id"] == DBNull.Value) ? null : (int?)paramDict["@Tehsil_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.UnionCouncil, (int?)paramDict["@UnionCouncil_Id"]),
                    new Pair<int?, int?>((int?)Config.Hierarchy.Ward, (int?)paramDict["@Ward_Id"])
                };

                float overrideRetainingHours = -1f;
                /*if (vmComplaint.departmentId == 153 && vmComplaint.Complaint_Category == 517 && dictFormCollection["PersonType"] != null)
                {
                    if (dictFormCollection["PersonType"] == "6204___Unknown Person")
                    {
                        overrideRetainingHours = 8;
                    }
                }*/
                assignmentModelList = AssignmentHandler.GetAssignmentModel(new FuncParamsModel.Assignment(nowDate,
                DbAssignmentMatrix.GetByCampaignIdAndCategoryId((int)vmComplaint.Compaign_Id, (int)vmComplaint.Complaint_Category, (int)vmComplaint.Complaint_SubCategory, null, null, listHierarchyPair), catRetainingHours, overrideRetainingHours)/*nowDate,
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

            if (addComplaintModel.SourceType == Config.SourceType.OtherSystemApi)
            {
                paramDict.Add("@Ref_Complaint_Src", addComplaintModel.OtherSystemId);
                paramDict.Add("@Ref_Complaint_Id", addComplaintModel.ComplaintId);

            }



            paramDict.Add("@Is_Action_Completed", (false).ToDbObj());
            paramDict.Add("@Current_Action_Step", (0).ToDbObj());
            paramDict.Add("@Action_Logs_Count", (0).ToDbObj());
            paramDict.Add("@Complainant_Feedback_Total_Count", (0).ToDbObj());

            int count = 1;
            // Populate dynamic fields
            foreach (VmDynamicDropDownList vmDynamicDropdown in vmComplaint.ListDynamicDropDown)
            {
                //int controlId = vmDynamicDropdown.ControlId;
                if (vmDynamicDropdown.SelectedItemId != null)
                {
                    string[] tempStrArr = vmDynamicDropdown.SelectedItemId.Split(new string[] { Config.Separator },
                        StringSplitOptions.None);
                    //string controlValue = vmDynamicDropdown.FieldValue;

                    //Set values
                    int controlId = Convert.ToInt32(tempStrArr[0]);
                    string controlValue = tempStrArr[1].ToString().Trim();
                    paramDict.Add("@RefField" + count + "_Int", controlId);
                    paramDict.Add("@RefField" + count, controlValue);
                }
                else
                {
                    paramDict.Add("@RefField" + count + "_Int", (null as object).ToDbObj());
                    paramDict.Add("@RefField" + count, (null as object).ToDbObj());
                }
                count++;
            }


            // testing fields validation
            List<DbDynamicComplaintFields> listDbDynamicComplaintFields2 = FormSubmitConfig.GetDbDynamicComplaintFields(dictFormCollection, -1);
            // end testing validation

            #endregion
            DataTable dt = DBHelper.GetDataTableByStoredProcedure("PITB.Add_Complaints_Crm", paramDict);
            string complaintIdStr = dt.Rows[0][1].ToString();
            int complaintId = Convert.ToInt32(dt.Rows[0][1].ToString().Split('-')[1]);

            FileUploadHandler.UploadMultipleFiles(postedFiles, Config.AttachmentReferenceType.Add, complaintIdStr, complaintId, Config.TAG_COMPLAINT_ADD);

            Config.CommandMessage cm = new Config.CommandMessage(UtilityExtensions.GetStatus(dt.Rows[0][0].ToString()), dt.Rows[0][1].ToString());
            if (vm.currentComplaintTypeTab == VmAddComplaint.TabComplaint || vm.currentComplaintTypeTab == VmAddComplaint.TabComplaintCombined)
            // when there is complaint populate assignment matrix
            {
                DynamicFieldsHandler.SaveDyamicFieldsInDb(vm.ComplaintVm, Convert.ToInt32(cm.Value.Split('-')[1]));

                List<DbDynamicComplaintFields> listDbDynamicComplaintFields = FormSubmitConfig.GetDbDynamicComplaintFields(dictFormCollection, complaintId);
                DbDynamicComplaintFields.Save(listDbDynamicComplaintFields);
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
                //if (((Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police ||
                //    Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Local) &&
                //    addComplaintModel.SourceType == Config.SourceType.Web) &&  /*string.IsNullOrEmpty(addComplaintModel.ComplaintId) &&*/ !PermissionHandler.IsPermissionAllowedInCampagin(Config.CampaignPermissions.DontSendMessages, addComplaintModel.Cookie))
                {
                    TextMessageHandler.SendMessageOnComplaintLaunch(vmPersonalInfo.Mobile_No,
                        (int)vmComplaint.Compaign_Id, Convert.ToInt32(cm.Value.Split('-')[1]),
                        (int)vmComplaint.Complaint_Category, vmPersonalInfo.Person_Name);
                    string msgToText = null;
                    //if (vmComplaint.Compaign_Id == (int)Config.Campaign.DcoOffice )
                    {
                        DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId);
                        DbComplaintType dbComplaintType = DbComplaintType.GetById((int)dbComplaint.Complaint_Category);
                        DbComplaintSubType dbComplaintSubType = DbComplaintSubType.GetById((int)dbComplaint.Complaint_SubCategory);
                        TimeSpan span = Utility.GetTimeSpanFromString(dbComplaint.Computed_Remaining_Time_To_Escalate);
                        DateTime dueDate = addComplaintModel.CreatedDateTime;//DateTime.Now.Add(span);
                        msgToText = "Chief Minister's Complaint Center Alert\n\n" +
                        "Dear Concerned, Complaint no." + complaintId + " has been assigned to you on " + /*DateTime.Now.ToShortDateString()*/addComplaintModel.CreatedDateTime.ToShortDateString() + " at " + addComplaintModel.CreatedDateTime.ToShortTimeString()/*DateTime.Now.ToShortTimeString()*/ + ".\n" +
                        "Category: " + dbComplaintType.Name + "\n" +
                        "Subcategory: " + dbComplaintSubType.Name + "\n" +
                        "Please resolve by " + dueDate.ToString() + "\n" +
                        "To view details, please visit: https://crm.punjab.gov.pk";
                    }
                    //else
                    //{
                    //    msgToText = null;
                    //}
                    TextMessageHandler.PrepareAndSendMessageToStakeholdersOnComplaintLaunch(DbComplaint.GetByComplaintId(complaintId), msgToText);
                }
            }

            // Post data to crm
            //if (Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police) // current deployment server
            //if (string.IsNullOrEmpty(addComplaintModel.ComplaintId))
            //if(addComplaintModel.SourceType == Config.SourceType.Web)
            if ((Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Police ||
                    Config.CURRENT_DEPLOYMENT_SERVER == Config.DeployedOnServer.Local) &&
                    addComplaintModel.SourceType == Config.SourceType.Web)
            {
                //addComplaintModel.PostedData = postedData;
                addComplaintModel.SourceType = Config.SourceType.OtherSystemApi;
                addComplaintModel.OtherSystemId = Config.OtherSystemId.Police;
                addComplaintModel.ComplaintId = complaintId.ToString();
                //addComplaintModel.CreatedDateTime = 

                //string url = "https://crm.punjab.gov.pk/rest/api/Police/AddComplaint";
                //string url = "http://localhost:2826/api/Police/AddComplaint";
                string url = Config.DictUrl["Police-AddComplaint"];
                Dictionary<string, string> headersDict = new Dictionary<string, string>();
                headersDict.Add("SystemName", "Police8787");
                headersDict.Add("SystemUserName", "Police");
                headersDict.Add("Password", "p@4o!li#0c@e");
                headersDict.Add("Username", "P#0el@9ic$e");
                Config.CommandMessage response = APIHelper.HttpClientGetResponse<Config.CommandMessage, PoliceModel.AddComplaint>(url, addComplaintModel, headersDict);
                DBContextHelperLinq db = new DBContextHelperLinq();
                DbComplaint dbComplaint = DbComplaint.GetByComplaintId(complaintId, db);
                dbComplaint.Ref_Complaint_Src = (int)Config.OtherSystemId.Crm;
                dbComplaint.Ref_Complaint_Id = Convert.ToInt32(response.Value.Split('-')[1]);//complaintId;
                DBContextHelperLinq.UpdateEntity(dbComplaint, db, new List<string> { "Ref_Complaint_Id", "Ref_Complaint_Src" });
                db.SaveChanges();
                //db.Entry()
                // Call Api
            }
            // end post data to crm

            //db.DbDynamicComplaintFields.Add();
            //SendMessage(Convert.ToInt32(cm.Value.Split('-')[1]), (int)Config.ComplaintStatus.PendingFresh);
            return cm;
        }


        public static void ChangeComplaintDistrict(int complaintId, int districtToChange)
        {
            DateTime currentDate = DateTime.Now;
            string updateCommand = @"UPDATE PITB.Complaints
                        SET Dt1 = DATEADD(day, 7, @Created_Date),
                        SrcId1 = 3,
                        UserSrcId1 = 10,

                        Dt2 = DATEADD(day, 14, @Created_Date),
                        SrcId2 = 3,
                        UserSrcId2 = 20,

                        Dt3 = DATEADD(day, 21, @Created_Date),
                        SrcId3 = 2,
                        UserSrcId3 = 30,

                        Dt4 = DATEADD(day, 28, @Created_Date),
                        SrcId4 = 1,
                        UserSrcId4 = 40,

                        MinSrcIdDate = DATEADD(day, 7, @Created_Date),
                        MaxSrcIdDate = DATEADD(day, 28, @Created_Date),
                        MaxSrcId = 3,
                        MinSrcId = 1
                        --SELECT * 
                        FROM PITB.Complaints
                        WHERE Compaign_Id = 78 AND id = @ComplaintId

                                        ";
            Dictionary<string, object> dictParams = new Dictionary<string, object>();
            dictParams.Add("@ComplaintId", complaintId);
            dictParams.Add("@Created_Date", currentDate);
            //dictParams.Add("@UserId", dbUsers.User_Id);
            Helper.Database.DBHelper.GetDataTableByQueryString(updateCommand, dictParams);
        }

        //public static dynamic GetListingData(dynamic data)
        //{
        //    DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(data.aoData);
        //    string startDate = data.startDate;
        //    string endDate = data.endDate;
        //    string[] splitArr = ((string)data.tagId).Split(new string[] { "__" }, StringSplitOptions.None);
        //    string tagName = splitArr[0];
        //    string moduleId = splitArr[1];
        //    int campaignId = Convert.ToInt32(splitArr[2]);

        //    List<DbPermissionsAssignment> listCampStatusMergePermission = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.Campaign, Utility.GetNullableIntList(new List<int> { campaignId }), Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
        //    var permissionAssign = listCampStatusMergePermission.Where(n => n.Type_Id == campaignId).FirstOrDefault();
        //    Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(permissionAssign.Permission_Value);



        //    Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
        //    List<string> prefixStrList = new List<string>
        //            {
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //                "complaints",
        //            };
        //    // for joins
        //    //List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
        //    //dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
        //    //dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
        //    DataTableHandler.ApplyColumnOrderPrefix(dtModel);

        //    Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
        //    //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
        //    dictFilterQuery.Add("complaints.Created_Date",
        //        "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

        //    DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixStrList,
        //            dictFilterQuery);

        //    Dictionary<string, object> paramDict = new Dictionary<string, object>();

        //    // data table params
        //    if (dtModel != null)
        //    {
        //        paramDict.Add("@StartRow", (dtModel.Start).ToDbObj());
        //        paramDict.Add("@EndRow", (dtModel.End).ToDbObj());
        //        paramDict.Add("@OrderByColumnName", (dtModel.ListOrder[0].columnName).ToDbObj());
        //        paramDict.Add("@OrderByDirection", (dtModel.ListOrder[0].sortingDirectionStr).ToDbObj());
        //        paramDict.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());
        //    }


        //    // Adding tags check
        //    string queryModule = null;
        //    if(tagName == "TagStatus")
        //    {
        //        Dictionary<string, string> dictModuleStatusMap = new Dictionary<string, string>()
        //        {
        //            { "Resolved", "8"},
        //            { "Overdue", "18"},
        //            { "Pending", "19"}
        //        };
        //        queryModule = "ComplaintsListingStatusWise";
        //        paramDict.Add("@StatusIds", listStatusMergePermission[dictModuleStatusMap[moduleId]].ToDbObj());
        //    }
        //    else if (tagName == "TagFeedback")
        //    {
        //        Dictionary<string, string> dictModuleFeedbackMap = new Dictionary<string, string>()
        //        {
        //            { "Satisfied", "1"},
        //            { "Dissatisfied", "2"}
        //        };
        //        queryModule = "ComplaintsListingFeedbackWise";
        //        paramDict.Add("@FeedbackIds", dictModuleFeedbackMap[moduleId].ToDbObj());
        //    }

        //    paramDict.Add("@CampaignIds", campaignId.ToDbObj());
        //    //paramDict.Add("@StatusIds", listStatusMergePermission["Resolved"].ToDbObj());
        //    paramDict.Add("@StartDate", startDate.ToDbObj());
        //    paramDict.Add("@EndDate", endDate.ToDbObj());

        //    string queryStr = QueryHelper.GetFinalQuery(queryModule, campaignId.ToString(), Config.ConfigType.Query,
        //                    paramDict);

        //    List<dynamic> listResult = DBHelper.GetDynamicListByQueryString(queryStr, null); //ds.Tables[0];
        //    //int totalRows = 0, totalFilteredRows = 0;
        //    //if (dt.Rows.Count > 0)
        //    //{
        //    //    //totalRows = (int) ds.Tables[1].Rows[0]["Total_Count"];

        //    //    totalRows = (int)dt.Rows[0]["Total_Rows"];
        //    //}




        //    dynamic tableData = new ExpandoObject();
        //    tableData.data = listResult;
        //    tableData.draw = dtModel.Draw ++;
        //    tableData.recordsTotal = (listResult!=null && listResult.Count>0) ? listResult[0].Total_Rows : 0;
        //    tableData.recordsFiltered = tableData.recordsTotal;
        //    //tableData.recordsTotal = 0;
        //    return tableData;
        //    //data
        //    //return Json(new
        //    //{
        //    //    data = dataTable,
        //    //    draw = dtModel.Draw,
        //    //    recordsTotal = totalRows,//dtModel.Length,
        //    //    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
        //    //}, JsonRequestBehavior.AllowGet);
        //    ////string module = 
        //    //return null;
        //}

        public static DataTable GetStakeHolderServerSideListDenormalized(string from, string to, DataTableParamsModel dtModel, string commaSeperatedCampaigns, string commaSeperatedCategories, string commaSeperatedStatuses, string commaSeperatedTransferedStatus, int complaintsType, Config.StakeholderComplaintListingType listingType, string spType, int userId = -1)
        {
            commaSeperatedCampaigns = string.IsNullOrEmpty(commaSeperatedCampaigns.Trim()) ? "-1" : commaSeperatedCampaigns;
            commaSeperatedCategories = string.IsNullOrEmpty(commaSeperatedCategories.Trim()) ? "-1" : commaSeperatedCategories;

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
            ListingParamsModelBase paramsSchoolEducation = SetStakeholderListingParams(dbUser, from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
            string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }

        public static ListingParamsModelBase SetStakeholderListingParams(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time, Ward_Name, Is_Action_Completed, Current_Action_Step,Action_Logs_Count,StatusReopenedCount,";



            //CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();


            if (spType == "ExcelReport")
            {
                extraSelection = @"CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10)) AS [Complaint No],
					complaints.Complaint_Computed_Status as Complaint_Status,Computed_Remaining_Total_Time, 
					 C.Person_Name as [Person Name],
					 C.Cnic_No as [Cnic No],
					 CASE C.Gender WHEN 1 THEN 'MALE' ELSE 'FEMALE' END AS Gender,
					 complaints.Person_District_Name as [Caller District],
					 CONVERT(VARCHAR(10),complaints.Created_Date,120) Date,
					C.Mobile_No as [Mobile No],
					C.Person_Address as [Person Address],
					D.District_Name [Complaint District],
					B.Name Category  ,
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
            paramsModel.CheckIfExistInSrcId = 1;
            paramsModel.CheckIfExistInUserSrcId = 1;
            paramsModel.IgnoreComputedHierarchyCheck = false;
            paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            return paramsModel;
        }

        public static int Export([FromBody] JToken jsonBody)
        {
            Dictionary<string, object> dictObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody.ToString());
            Dictionary<string, string> dict = Utility.ConvertNewtonsoftDictionaryResponse(dictObj);
            string commaSeperatedCampaigns = string.Join(",", dict["campaign"]);
            string commaSeperatedCategories = string.Join(",", dict["cateogries"]);
            string commaSeperatedTransferedStatus = string.Join(",", dict["transferedStatus"]);
            string commaSeperatedStatuses = "";
            if (dict["statuses"] != null)
            {
                commaSeperatedStatuses = string.Join(",", dict["statuses"]);
            }
            if (!dict.ContainsKey("userId"))
            {
                dict.Add("userId", "-1");
            }
            DataTable data = null;

            if (commaSeperatedCampaigns.Contains(((int)Config.Campaign.ZimmedarShehri).ToString()))
            {
                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(dict["aoData"]);
                data = BlZimmedarShehri.GetStakeHolderServerSideListDenormalized(
                    dict["from"],
                    dict["to"],
                    dtModel,
                    commaSeperatedCampaigns,
                    commaSeperatedCategories,
                    commaSeperatedStatuses,
                    commaSeperatedTransferedStatus,
                    Convert.ToInt32(dict["complaintType"]),
                    (Config.StakeholderComplaintListingType)Convert.ToInt32(dict["listingType"]),
                    "ExcelReport",
                    Convert.ToInt32(dict["userId"]));
            }

            else
            {
                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(dict["aoData"]);
                data = BlComplaints.GetStakeHolderServerSideListDenormalized(
                    dict["from"],
                    dict["to"],
                    dtModel,
                    commaSeperatedCampaigns,
                    commaSeperatedCategories,
                    commaSeperatedStatuses,
                    commaSeperatedTransferedStatus,
                    Convert.ToInt32(dict["complaintType"]),
                    (Config.StakeholderComplaintListingType)Convert.ToInt32(dict["listingType"]),
                    "ExcelReport",
                    Convert.ToInt32(dict["userId"]));
            }

            int rowCount = data.Rows.Count;
            //return FileHandler.GetFile(Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData");
            //return FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");

            //HttpResponseBase responseBase = FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");
            //return Json(responseBase, JsonRequestBehavior.AllowGet);
            ExcelPackage excelPack = FileHandler.ExportToExcel(data, "Complaint Listing Data");

            string fileName = DbCampaign.GetById(Int32.Parse(commaSeperatedCampaigns.Split(',').First())).Campaign_Name;
            string startDate = dict["from"];
            string endDate = dict["to"];
            int dataId = DataStateMVC.AddInPool(excelPack, fileName, startDate, endDate);
            return dataId;
        }
    }


}