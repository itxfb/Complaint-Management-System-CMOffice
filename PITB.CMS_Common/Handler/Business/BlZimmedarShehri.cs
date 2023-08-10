using System.Data;
using AutoMapper;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.Custom.Reports;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Models.View.Agent;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.ApiModels.Custom;
using PITB.CMS_Common.ApiHandlers.Translation;
using PITB.CMS_Common.ApiModels.API;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.ComplaintFileHandler;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlZimmedarShehri
	{
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
			ListingParamsModelBase paramsSchoolEducation = SetStakeholderListingParams(dbUser, from, to, commaSeperatedCampaigns, commaSeperatedCategories, commaSeperatedStatuses, commaSeperatedTransferedStatus, dtModel, (Config.ComplaintType)complaintsType, listingType, spType);
			string queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
			return DBHelper.GetDataTableByQueryString(queryStr, null);
		}

        public static VmComplaintZimmedarShehriDetailAgent GetStakeholderComplaintDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            List<DbComplaint> listComplaint = DbComplaint.GetListByComplaintId(complaintId);
            List<DbDynamicComplaintFields> listDynamicFields = DbDynamicComplaintFields.GetByComplaintId(complaintId);
            VmComplaintZimmedarShehriDetailAgent vmComplaintDetail = new VmComplaintZimmedarShehriDetailAgent();
			CMSCookie cookie = AuthenticationHandler.GetCookie();
			vmComplaintDetail.GetComplaintDetail(listComplaint.First(), cookie.UserId, listDynamicFields, detailType);
            //vmComplaintDetail.CanShowFollowUpView = (listComplaint.First().Complaint_Computed_Status_Id != (int)Config.ComplaintStatus.Resolved) ||
            //                                        (listComplaint.First().Complaint_Computed_Status_Id !=
            //                                         (int)Config.ComplaintStatus.Irrelevant);

            
            if (listComplaint.First().Complaint_Computed_Status_Id != (int)Config.ComplaintStatus.Resolved)
            {
                vmComplaintDetail.CanShowFollowUpView = listComplaint.First().Complaint_Computed_Status_Id != (int) Config.ComplaintStatus.Irrelevant;
            }
           
            //vmComplaintDetail.CanShowFollowUpView = !((vmComplaintDetail.currentStatusStr.Trim() == Config.ClosedVerifiedStatus) || (vmComplaintDetail.VmStatusChange.currentStatusId.Equals(((int)Config.ComplaintStatus.Resolved).ToString()) || vmComplaintDetail.VmStatusChange.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedVerified).ToString()) || vmComplaintDetail.VmStatusChange.currentStatusId.Equals(((int)Config.ComplaintStatus.ResolvedUnverified).ToString())));




            vmComplaintDetail.vmFileModel = FileHandler.GetVmFileModel(complaintId, (int)Config.AttachmentReferenceType.Add, complaintId);
            vmComplaintDetail.hasStatusHistory = (StatusHandler.GetComplaintStatusChangeHistoryTableList(complaintId).Count > 0) ? true : false;
            vmComplaintDetail.hasTransferHistory = TransferHandler.GetTransferedHistoryStatus(complaintId);
            //vmComplaintDetail.currDetailType = detailType;

            if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowStakeholderEscalationInDetail, cookie.UserId, cookie.Role))
            {
                vmComplaintDetail.ListEscalationModel =
                    EscalationHandler.GetEscalationListOfComplaint(listComplaint.First());
            }
            //DbStatus.GetByCampaignIdAndCategoryId()

            Mapper.CreateMap<DbPersonInformation, VmPersonalInfo>();
            vmComplaintDetail.vmPersonlInfo = Mapper.Map<VmPersonalInfo>(DbPersonInformation.GetPersonInformationByPersonId((int)listComplaint.FirstOrDefault().Person_Id));
            vmComplaintDetail.Complaint_SubCategory = listComplaint.First().Complaint_SubCategory;

            return vmComplaintDetail;
        }

		public static ListingParamsModelBase SetStakeholderListingParams(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
		{
			string extraSelection = "complaints.Complaint_Computed_Status_Id as Complaint_Computed_Status_Id, complaints.StatusChangedComments as Stakeholder_Comments, complaints.Complaint_Status_Id as Complaint_Status_Id, complaints.Complaint_Status as Complaint_Status,Latitude,Longitude,LocationArea,Computed_Remaining_Total_Time, ";



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
					t.Tehsil_Name [Complaint Tehsil],
					B.Name Category  ,
					F.Name as [Sub Category],
					complaints.Complaint_Remarks as [Complaint Remarks],
					complaints.Agent_Comments as [Agent Comments],
					P.[Status],
					complaints.Complaint_Computed_Hierarchy as [Escalation Level],
					complaints.Created_Date as [Created Date]";

				paramsModel.InnerJoinLogic = @"INNER JOIN PITB.Complaints_Type B ON complaints.Complaint_Category=B.Id
					INNER JOIN PITB.Complaints_SubType F ON complaints.Complaint_SubCategory=F.Id
					INNER JOIN PITB.Person_Information C ON complaints.Person_Id=C.Person_id
					INNER JOIN PITB.Districts D ON complaints.District_Id=D.id
					INNER JOIN PITB.Tehsil T ON complaints.Tehsil_Id=t.Id
					INNER JOIN PITB.Statuses P ON p.Id=complaints.Complaint_Computed_Status_Id";
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

	    public static FileUploadHandler.FileValidationStatus ChangeStatus(VmStatusChange vmStatusChange,
	        HttpFileCollection files)
	    {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
	        DbComplaint dbComplaint = DbComplaint.GetBy(vmStatusChange.ComplaintId);
            Dictionary<string, object> dictStatusChangeParams = new Dictionary<string, object>();
            dictStatusChangeParams.Add("statusId",vmStatusChange.statusID);
            dictStatusChangeParams.Add("statusComments", vmStatusChange.statusChangeComments);
            dictStatusChangeParams.Add("dbComplaint", dbComplaint);
            dictStatusChangeParams.Add("dbUser", DbUsers.GetActiveUser(cookie.UserId));
            dictStatusChangeParams.Add("files", files);

            Dictionary<string, object> statusUpdateData = StatusHandler.PrepareDataForStatusChange(dictStatusChangeParams);

            // if status in progress
            //if (vmStatusChange.statusID == (int) Config.ComplaintStatus.InProgress)
            //{
            //    float catRetainingHours = 0;
            //    float? subcatRetainingHours = 0;
            //    //Config.CategoryType cateogryType = Config.CategoryType.Main;

            //    subcatRetainingHours = DbComplaintSubType.GetRetainingByComplaintSubTypeId(Convert.ToInt32(dbComplaint.Complaint_SubCategory));

            //    if (subcatRetainingHours == null) // when subcategory doesnot have retaining hours
            //    {
            //        catRetainingHours =
            //            DbComplaintType.GetRetainingHoursByTypeId(Convert.ToInt32(dbComplaint.Complaint_Category));
            //        //cateogryType = Config.CategoryType.Main;
            //    }
            //    else
            //    {
            //        catRetainingHours = (float)subcatRetainingHours;
            //        //cateogryType = Config.CategoryType.Sub;
            //    }
            //    List<AssignmentModel> assignmentModelList = (List<AssignmentModel>) statusUpdateData["assignmentModelList"];
            //    for (int i = 0; i < 10; i++)
            //    {
            //        if (i < assignmentModelList.Count)
            //        {
            //            dictStatusChangeParams["@Dt" + (i + 1)] = assignmentModelList[i].Dt.Value.AddHours(catRetainingHours);
            //            dictStatusChangeParams.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
            //            dictStatusChangeParams.Add("@UserSrcId" + (i + 1), assignmentModelList[i].UserSrcId.ToDbObj());
            //            //paramDict.Add("@SrcId" + (i + 1), assignmentModelList[i].SrcId.ToDbObj());
            //        }
	              
            //    }
            //}
            // end if status in progress
	        StatusHandler.UpdateStatusInDb(statusUpdateData);
            FileUploadHandler.FileValidationStatus fileValidationStatus = (FileUploadHandler.FileValidationStatus)statusUpdateData["validationStatus"];
	        return fileValidationStatus;
	        //StatusHandler.ChangeStatus(vmStatusChange, files);
	    }


	    public static List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> RegionAndStatusWiseCountReportDistrict(string startDate, string endDate, int provinceid, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType)
		{

			Dictionary<int, string> dictRegion = new Dictionary<int, string>();

			string divisionId = AuthenticationHandler.GetCookie().DivisionId;
			string districtId = AuthenticationHandler.GetCookie().DistrictId;
			string provinceId = AuthenticationHandler.GetCookie().ProvinceId;
			int userHierarchyId2 = (int)AuthenticationHandler.GetCookie().Hierarchy_Id;
			string parameters = "";
			if (userHierarchyId2 == 1)
			{
				parameters = "";
			}
			else if (userHierarchyId2 == 2)
			{
				parameters = " complaints.Division_Id = " + divisionId + " ";
			}
			//else if (hierarchyId == 3)
			//{
			//    parameters = " WHERE complaints.District_Id = " + districtId + " ";
			//}

			Config.SummaryReportType rType = (Config.SummaryReportType)reportType;
			int campaignId = Utility.GetIntByCommaSepStr(campId);
			List<DbUserCategory> listDbUserCat = null;
			string queryStr = "";
			DataTable dt;
			DataTableParamsModel dtParams = null;
			ListingParamsModelBase paramsSchoolEducation = null;

			//Dictionary<int, string> dictColumnsMap = new Dictionary<int, string>
			//{
			//    {1,"Province_Name"},
			//    {2,"Division_Name"},
			//    {3,"District_Name"},
			//    {4,"Tehsil_Name"},
			//    {5,"UnionCouncil_Name"}
			//};


			string selectionFields = @" complaints.Complaint_Computed_Status_Id StatusId, complaints.Complaint_Computed_Status StatusName, COUNT(*) Total";

			string innerJoinLogic = @"";
			string whereLogic = "";
			string groupLogic = @" complaints.Division_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

			if (hierarchyId == (int)Config.Hierarchy.District) // District
			{
				whereLogic = parameters + whereLogic;
				selectionFields = @"complaints.District_Id Hierarchy1Id, " + selectionFields;
				groupLogic = @"complaints.District_Id , " + groupLogic;
				if (divisionId == null)
				{
					dictRegion = DbDistrict.GetDistrictList(provinceid).ToDictionary(n => n.District_Id, n => n.District_Name); ;
				}
				else
				{
					dictRegion = DbDistrict.GetDistrictList(provinceid).Where(x => x.Division_Id == Int32.Parse(divisionId)).ToDictionary(n => n.District_Id, n => n.District_Name);
				}

			}

			if (rType == Config.SummaryReportType.General) // general report
			{
				CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
				listDbUserCat = cmsCookie.ListDbUserCategory;
			}
			commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
			paramsSchoolEducation = SetParamsDynamicQuery(3, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
					"1,0", dtParams, Config.ComplaintType.Complaint,
					Config.StakeholderComplaintListingType.UptilMyHierarchy, "RegionStatusWiseSummary", selectionFields, innerJoinLogic, groupLogic, "");

			queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
			dt = DBHelper.GetDataTableByQueryString(queryStr, null);

			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.RegionAndStatusWiseCountTempData>();


			List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionAndStatusWiseCount = new List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTemp = null;
			string resolvingOfficerName = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount regionAndStatusWiseCount = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount cumulativeStatusWiseCount = null;



			int i = 1;
			var regionAndStatusWiseList = listRegionAndStatusWiseCountTempData.GroupBy(n => n.Hierarchy1Id);

			cumulativeStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

			foreach (var regionStatusGroup in regionAndStatusWiseList)
			{
				listRegionAndStatusWiseCountTemp = listRegionAndStatusWiseCountTempData.Where(n => n.Hierarchy1Id == regionStatusGroup.Key).ToList();

				regionAndStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

				foreach (MainSummaryReport.RegionAndStatusWiseCountTempData regionStatusWiseCount in listRegionAndStatusWiseCountTemp)
				{
					regionAndStatusWiseCount.Hierarchy1Data = dictRegion[regionStatusWiseCount.Hierarchy1Id];
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingFresh)
					{
						regionAndStatusWiseCount.PendingFresh = regionAndStatusWiseCount.PendingFresh + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
					{
						regionAndStatusWiseCount.PendingReopened = regionAndStatusWiseCount.PendingReopened + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Resolved)
					{
						regionAndStatusWiseCount.Resolved = regionAndStatusWiseCount.Resolved + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ClosedVerified)
					{

					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
					{
						regionAndStatusWiseCount.Overdue = regionAndStatusWiseCount.Overdue + regionStatusWiseCount.Total;
					}

					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Irrelevant)
					{
						regionAndStatusWiseCount.Irrelevant = regionAndStatusWiseCount.Irrelevant + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.InProgress)
					{
						regionAndStatusWiseCount.Forwarded = regionAndStatusWiseCount.Forwarded + regionStatusWiseCount.Total;
					}

				}



				cumulativeStatusWiseCount.Resolved = cumulativeStatusWiseCount.Resolved + regionAndStatusWiseCount.Resolved;
				cumulativeStatusWiseCount.Overdue = cumulativeStatusWiseCount.Overdue + regionAndStatusWiseCount.Overdue;
				regionAndStatusWiseCount.Total = regionAndStatusWiseCount.Total + regionAndStatusWiseCount.PendingFresh + regionAndStatusWiseCount.Resolved +
													 regionAndStatusWiseCount.PendingReopened + regionAndStatusWiseCount.Overdue + regionAndStatusWiseCount.Forwarded + regionAndStatusWiseCount.Irrelevant;

				regionAndStatusWiseCount.SrNo = null;
				listRegionAndStatusWiseCount.Add(regionAndStatusWiseCount);
				i++;
			}
			cumulativeStatusWiseCount.Hierarchy1Data = "Total";
			cumulativeStatusWiseCount.Total = cumulativeStatusWiseCount.PendingFresh + cumulativeStatusWiseCount.Resolved +
											  cumulativeStatusWiseCount.PendingReopened + cumulativeStatusWiseCount.Overdue;
			cumulativeStatusWiseCount.SrNo = null;
			//listRegionAndStatusWiseCount.Add(cumulativeStatusWiseCount);

			//for (int i = 0; i < listRegionAndStatusWiseCount.Count; i++)
			//{
			//    listRegionAndStatusWiseCount[i].SrNo = (i + 1).ToString();
			//    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
			//}

			return listRegionAndStatusWiseCount.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
		}

		public static List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> RegionAndStatusWiseCountReportDivision(string startDate, string endDate, int provinceid, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType)
		{

			Dictionary<int, string> dictRegion = new Dictionary<int, string>();



			Config.SummaryReportType rType = (Config.SummaryReportType)reportType;
			int campaignId = Utility.GetIntByCommaSepStr(campId);
			List<DbUserCategory> listDbUserCat = null;
			string queryStr = "";
			DataTable dt;
			DataTableParamsModel dtParams = null;
			ListingParamsModelBase paramsSchoolEducation = null;

			//Dictionary<int, string> dictColumnsMap = new Dictionary<int, string>
			//{
			//    {1,"Province_Name"},
			//    {2,"Division_Name"},
			//    {3,"District_Name"},
			//    {4,"Tehsil_Name"},
			//    {5,"UnionCouncil_Name"}
			//};


			string selectionFields = @" complaints.Complaint_Computed_Status_Id StatusId, complaints.Complaint_Computed_Status StatusName, COUNT(*) Total";

			string innerJoinLogic = @"";
			string whereLogic = "";
			string groupLogic = @" complaints.Division_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

			if (hierarchyId == (int)Config.Hierarchy.Division) // Division
			{
				selectionFields = @"complaints.Division_Id Hierarchy1Id, " + selectionFields;
				groupLogic = @"complaints.Division_Id , " + groupLogic;
				dictRegion = DbDivision.GetByProvinceId(provinceid).ToDictionary(n => n.Division_Id, n => n.Division_Name);
			}

			if (rType == Config.SummaryReportType.General) // general report
			{
				CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
				listDbUserCat = cmsCookie.ListDbUserCategory;
			}
			commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
			paramsSchoolEducation = SetParamsDynamicQuery(2, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
					"1,0", dtParams, Config.ComplaintType.Complaint,
					Config.StakeholderComplaintListingType.UptilMyHierarchy, "RegionStatusWiseSummary", selectionFields, innerJoinLogic, groupLogic, "");

			queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
			dt = DBHelper.GetDataTableByQueryString(queryStr, null);

			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.RegionAndStatusWiseCountTempData>();


			List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionAndStatusWiseCount = new List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTemp = null;
			string resolvingOfficerName = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount regionAndStatusWiseCount = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount cumulativeStatusWiseCount = null;



			int i = 1;
			var regionAndStatusWiseList = listRegionAndStatusWiseCountTempData.GroupBy(n => n.Hierarchy1Id);

			cumulativeStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

			foreach (var regionStatusGroup in regionAndStatusWiseList)
			{
				listRegionAndStatusWiseCountTemp = listRegionAndStatusWiseCountTempData.Where(n => n.Hierarchy1Id == regionStatusGroup.Key).ToList();

				regionAndStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

				foreach (MainSummaryReport.RegionAndStatusWiseCountTempData regionStatusWiseCount in listRegionAndStatusWiseCountTemp)
				{
					regionAndStatusWiseCount.Hierarchy1Data = dictRegion[regionStatusWiseCount.Hierarchy1Id];
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingFresh)
					{
						regionAndStatusWiseCount.PendingFresh = regionAndStatusWiseCount.PendingFresh + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
					{
						regionAndStatusWiseCount.PendingReopened = regionAndStatusWiseCount.PendingReopened + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Resolved)
					{
						regionAndStatusWiseCount.Resolved = regionAndStatusWiseCount.Resolved + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ClosedVerified)
					{

					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
					{
						regionAndStatusWiseCount.Overdue = regionAndStatusWiseCount.Overdue + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Irrelevant)
					{
						regionAndStatusWiseCount.Irrelevant = regionAndStatusWiseCount.Irrelevant + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.InProgress)
					{
						regionAndStatusWiseCount.Forwarded = regionAndStatusWiseCount.Forwarded + regionStatusWiseCount.Total;
					}

				}



				cumulativeStatusWiseCount.Resolved = cumulativeStatusWiseCount.Resolved + regionAndStatusWiseCount.Resolved;
				cumulativeStatusWiseCount.Overdue = cumulativeStatusWiseCount.Overdue + regionAndStatusWiseCount.Overdue;
				regionAndStatusWiseCount.Total = regionAndStatusWiseCount.Total + regionAndStatusWiseCount.PendingFresh + regionAndStatusWiseCount.Resolved +
													 regionAndStatusWiseCount.PendingReopened + regionAndStatusWiseCount.Overdue + regionAndStatusWiseCount.Forwarded + regionAndStatusWiseCount.Irrelevant;

				regionAndStatusWiseCount.SrNo = null;
				listRegionAndStatusWiseCount.Add(regionAndStatusWiseCount);
				i++;
			}
			cumulativeStatusWiseCount.Hierarchy1Data = "Total";
			cumulativeStatusWiseCount.Total = cumulativeStatusWiseCount.PendingFresh + cumulativeStatusWiseCount.Resolved +
											  cumulativeStatusWiseCount.PendingReopened + cumulativeStatusWiseCount.Overdue;
			cumulativeStatusWiseCount.SrNo = null;
			//listRegionAndStatusWiseCount.Add(cumulativeStatusWiseCount);

			//for (int i = 0; i < listRegionAndStatusWiseCount.Count; i++)
			//{
			//    listRegionAndStatusWiseCount[i].SrNo = (i + 1).ToString();
			//    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
			//}

			return listRegionAndStatusWiseCount.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
		}
		public static ListingParamsModelBase SetParamsDynamicQuery(int hierarchyId, int userHierarchyId, string commaSepVal, List<DbUserCategory> listDbUserCat, string fromDate, string toDate, string campaign, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType, string selectionFields, string innerJoinLogic, string groupByFields, string whereLogic)
		{
			//string extraSelection = "complaints.Person_Contact,complaints.Department_Name, complaints.Complaint_SubCategory_Name, complaints.RefField1, complaints.RefField2, complaints.RefField3, complaints.RefField4, complaints.RefField5, complaints.RefField6, complaints.Person_Cnic,";

			//string selectionFields = "";
			//string groupByFields = "";
			//string whereLogic = "";
			//if (categoryType == Config.CategoryType.Main)
			//{
			//    selectionFields = "complaints.Department_Id AS CatId,complaints.Department_Name AS CatName";
			//    groupByFields =
			//        "complaints.Department_Id,complaints.Department_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

			//    whereLogic = " and  EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
			//                 "',',') X WHERE X.Item=complaints.Department_Id)";
			//}
			//else if (categoryType == Config.CategoryType.Sub)
			//{
			//    selectionFields = "complaints.Complaint_Category AS CatId,complaints.Complaint_Category_Name AS CatName";
			//    groupByFields =
			//        "complaints.Complaint_Category,complaints.Complaint_Category_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";
			//    whereLogic = "  and EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
			//                 "',',') X WHERE X.Item=complaints.Complaint_Category)";
			//}
			//else if (categoryType == Config.CategoryType.Tertiary)
			//{
			//    selectionFields = "complaints.Complaint_SubCategory AS CatId,complaints.Complaint_SubCategory_Name AS CatName";
			//    groupByFields =
			//        "complaints.Complaint_SubCategory,complaints.Complaint_SubCategory_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

			//    whereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + category +
			//                 "',',') X WHERE X.Item=complaints.Complaint_SubCategory)";
			//}


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
			//paramsModel.Category = category;
			paramsModel.Status = complaintStatuses;
			paramsModel.TransferedStatus = commaSeperatedTransferedStatus;
			paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
			paramsModel.UserHierarchyId = hierarchyId;
			paramsModel.UserDesignationHierarchyId = userHierarchyId;
			paramsModel.ListingType = Convert.ToInt32(listingType);
			if (hierarchyId == (int)Config.Hierarchy.Province)
			{
				paramsModel.ProvinceId = commaSepVal;
			}
			else if (hierarchyId == (int)Config.Hierarchy.Division)
			{
				paramsModel.DivisionId = commaSepVal;
			}
			else if (hierarchyId == (int)Config.Hierarchy.District)
			{
				paramsModel.DistrictId = commaSepVal;
			}
			else if (hierarchyId == (int)Config.Hierarchy.Tehsil)
			{
				paramsModel.Tehsil = commaSepVal;
			}
			else if (hierarchyId == (int)Config.Hierarchy.UnionCouncil)
			{
				paramsModel.UcId = commaSepVal;
			}
			//paramsModel.DivisionId = dbUser.Division_Id;
			//paramsModel.DistrictId = dbUser.District_Id;

			//paramsModel.Tehsil = dbUser.Tehsil_Id;
			//paramsModel.UcId = dbUser.UnionCouncil_Id;
			//paramsModel.WardId = dbUser.Ward_Id;

			//paramsModel.UserId = dbUser.User_Id;
			paramsModel.UserCategoryId1 = null;//dbUser.UserCategoryId1;
			paramsModel.UserCategoryId2 = null;//dbUser.UserCategoryId2;

			paramsModel.ListUserCategory = UserCategoryModel.GetListUserCategoryModel(listDbUserCat);
			paramsModel.CheckIfExistInSrcId = 1;
			paramsModel.CheckIfExistInUserSrcId = 1;
			paramsModel.SelectionFields = selectionFields;
			paramsModel.InnerJoinLogic = innerJoinLogic;
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
		private static Dictionary<int, string> GetHierarchyList(int campaignId, int hierarchyId,int upperHierarchyId)
		{
			Dictionary<int, string> lst = new Dictionary<int, string>();
			int groupId = DbHierarchyCampaignGroupMapping.GetGroupIdForCampaignAndHierarchyIds(campaignId, hierarchyId);
			if (hierarchyId == (int)Config.Hierarchy.Division)
			{
                if (upperHierarchyId == -1)
                {
                    if (groupId == -1)
                    {
                        lst = DbDivision.GetByGroupId(null).ToDictionary(x => x.Division_Id, y => y.Division_Name);
                    }
                    else
                    {
                        lst = DbDivision.GetByGroupId(groupId).ToDictionary(x => x.Division_Id, y => y.Division_Name);
                    }
                }
                else
                {
                    if (groupId == -1)
                    {
                        lst = DbDivision.GetByProvinceAndGroupId(upperHierarchyId,null).ToDictionary(x => x.Division_Id, y => y.Division_Name);
                    }
                    else
                    {
                        lst = DbDivision.GetByProvinceAndGroupId(upperHierarchyId, groupId).ToDictionary(x => x.Division_Id, y => y.Division_Name);
                    }
                }  
			}
			else if (hierarchyId == (int)Config.Hierarchy.District)
			{
                if (upperHierarchyId == -1)
                {
                    if (groupId == -1)
                    {
                        lst = DbDistrict.GetByGroupId(null).ToDictionary(x => x.District_Id, y => y.District_Name);
                    }
                    else
                    {
                        lst = DbDistrict.GetByGroupId(groupId).ToDictionary(x => x.District_Id, y => y.District_Name);
                    }
                }
                else
                {
                    if (groupId == -1)
                    {
                        lst = DbDistrict.GetByDivisionAndGroupId(upperHierarchyId,null).ToDictionary(x => x.District_Id, y => y.District_Name);
                    }
                    else
                    {
                        lst = DbDistrict.GetByDivisionAndGroupId(upperHierarchyId,groupId).ToDictionary(x => x.District_Id, y => y.District_Name);
                    }
                }
                
			}
			else if (hierarchyId == (int)Config.Hierarchy.Tehsil)
			{
                if (upperHierarchyId == -1)
                {
                    if (groupId == -1)
                    {
                        lst = DbTehsil.GetByGroupId(null).ToDictionary(x => x.Tehsil_Id, y => y.Tehsil_Name);
                    }
                    else
                    {
                        lst = DbTehsil.GetByGroupId(groupId).ToDictionary(x => x.Tehsil_Id, y => y.Tehsil_Name);
                    }
                }
                else
                {
                    if (groupId == -1)
                    {
                        lst = DbTehsil.GetByDistrictAndGroupId(upperHierarchyId,null).ToDictionary(x => x.Tehsil_Id, y => y.Tehsil_Name);
                    }
                    else
                    {
                        lst = DbTehsil.GetByDistrictAndGroupId(upperHierarchyId, groupId).ToDictionary(x => x.Tehsil_Id, y => y.Tehsil_Name);
                    }
                }   
			}
			return lst;
		}
		public static List<RegionWiseFeedback> RegionWiseFeedbackReport(string startDate, string endDate, int hierarchyId, int campaignId, int provinceId,int upperHierarchyId)
		{
			string colName = Utility.GetHierarchyColumnNameByHierarchyIdComplaintsTable(hierarchyId);
            string query = "";
            if (upperHierarchyId == -1)
            {
                query = @"SELECT COUNT(*) As 'Count', CNFP_Feedback_Id As 'FeedbackId', CNFP_Feedback_Value As 'FeedbackValue',
							{4} as 'HierarchyId', {5} as 'HierarchyName',
							Complaint_Computed_Status_Id As 'StatusId', Complaint_Computed_Status As 'Status' FROM PITB.Complaints
							Where Compaign_Id = {0} AND CONVERT(DATE,Created_Date,120) BETWEEN CONVERT(DATE,'{1}',120) AND CONVERT(DATE,'{2}',120) AND Complaint_Type = 1
							GROUP BY CNFP_Feedback_Id,CNFP_Feedback_Value{3},Complaint_Computed_Status_Id,Complaint_Computed_Status";
                query = string.Format(query, campaignId, startDate, endDate, string.Concat(",", colName), colName.Split(new char[] { ',' }).First(), colName.Split(new char[] { ',' }).Last());           
            }
            else
            {
                string colNameUpper = Utility.GetHierarchyColumnNameByHierarchyIdComplaintsTable(hierarchyId - 1);
                query = @"SELECT COUNT(*) As 'Count', CNFP_Feedback_Id As 'FeedbackId', CNFP_Feedback_Value As 'FeedbackValue',
							{4} as 'HierarchyId', {5} as 'HierarchyName',
							Complaint_Computed_Status_Id As 'StatusId', Complaint_Computed_Status As 'Status' FROM PITB.Complaints
							Where Compaign_Id = {0} AND CONVERT(DATE,Created_Date,120) BETWEEN CONVERT(DATE,'{1}',120) AND CONVERT(DATE,'{2}',120) AND Complaint_Type = 1 AND {6} = {7}
							GROUP BY CNFP_Feedback_Id,CNFP_Feedback_Value{3},Complaint_Computed_Status_Id,Complaint_Computed_Status";
                query = string.Format(query, campaignId, startDate, endDate, string.Concat(",", colName), colName.Split(new char[] { ',' }).First(), colName.Split(new char[] { ',' }).Last(), colNameUpper.Split(new char[] { ',' }).First(), upperHierarchyId);
            }
			DataTable dt = DBHelper.GetDataTableByQueryString(query, null);
			List<RegionWiseFeedback> regionList = new List<RegionWiseFeedback>();
			Dictionary<int, string> HierarchyLst = GetHierarchyList(campaignId, hierarchyId,upperHierarchyId);
			if (dt != null)
			{
				if (dt.Rows != null && dt.Rows.Count > 0)
				{
					//var t = dt.Rows.Cast<DataRow>().GroupBy(x => x.Field<int>("StatusId"));
					//foreach (var r in t)
					//{
					//    Debug.WriteLine(r.First().Field<string>("Status") + ", " + r.Sum(q => q.Field<int>("Count")));
					//}
					foreach (var item in HierarchyLst)
					{
						var rows = dt.Rows.Cast<DataRow>().Where(x => x.Field<int>("HierarchyId") == item.Key).ToList();
						RegionWiseFeedback feedback = new RegionWiseFeedback();
						feedback.Hierarchy1Data = item.Value;

						feedback.Satisfied = GetFeedbackCountById(rows, Config.FeedbackStatuses.Satisfied);
						feedback.Dissatisfied = GetFeedbackCountById(rows, Config.FeedbackStatuses.Dissatisfied);
						feedback.NoAnswer = GetFeedbackCountById(rows, Config.FeedbackStatuses.NoAnswer);
						feedback.Pending = GetFeedbackCountById(rows, Config.FeedbackStatuses.Pending);
						feedback.Busy = GetFeedbackCountById(rows, Config.FeedbackStatuses.Busy);
						feedback.Cancel = GetFeedbackCountById(rows, Config.FeedbackStatuses.Cancel);
						feedback.Congestion = GetFeedbackCountById(rows, Config.FeedbackStatuses.Congestion);
						feedback.NotComplete = GetFeedbackCountById(rows, Config.FeedbackStatuses.NotComplete);
						feedback.NotApplicable = GetFeedbackCountById(rows, Config.FeedbackStatuses.NotApplicable);
						regionList.Add(feedback);
					}
				}
			}
			return regionList;
		}
		private static int GetFeedbackCountById(List<DataRow> rows, Config.FeedbackStatuses feedbackId)
		{
			int count = 0;
			if (feedbackId == Config.FeedbackStatuses.NotApplicable)
			{
				IEnumerable<DataRow> data = rows.Where(x => x.Field<int>("StatusId") != (int)Config.ComplaintStatus.Resolved);
				if (data != null && data.Count() > 0)
				{
					count = data.Sum(x => x.Field<int>("Count"));
				}
			}
			else if (feedbackId == Config.FeedbackStatuses.Pending)
			{
				IEnumerable<DataRow> data = rows.Where(x => x.Field<int>("StatusId") == (int)Config.ComplaintStatus.Resolved);
				if (data != null && data.Count() > 0)
				{
					var items = data.Where(x => x.Field<int?>("feedbackId") == (int)feedbackId || x.Field<int?>("feedbackId") == null);
					if (items != null && items.Count() > 0)
					{
						count = items.Sum(x => x.Field<int>("Count"));
					}
				}
			}
			else
			{
				IEnumerable<DataRow> data = rows.Where(x => x.Field<int>("StatusId") == (int)Config.ComplaintStatus.Resolved);
				if (data != null && data.Count() > 0)
				{
					var items = data.Where(x => x.Field<int?>("feedbackId") == (int)feedbackId);
					if (items != null && items.Count() > 0)
					{
						count = items.Sum(x => x.Field<int>("Count"));
					}
				}
			}
			return count;
		}
		public static List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> RegionAndStatusWiseCountReportTehsil(string startDate, string endDate, int districtId, int campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds)
		{

			Dictionary<int, string> dictRegion = new Dictionary<int, string>();



			int campaignId = Utility.GetIntByCommaSepStr(campId.ToString());
			List<DbUserCategory> listDbUserCat = null;
			string queryStr = "";
			DataTable dt;
			DataTableParamsModel dtParams = null;
			ListingParamsModelBase paramsSchoolEducation = null;

			//Dictionary<int, string> dictColumnsMap = new Dictionary<int, string>
			//{
			//    {1,"Province_Name"},
			//    {2,"Division_Name"},
			//    {3,"District_Name"},
			//    {4,"Tehsil_Name"},
			//    {5,"UnionCouncil_Name"}
			//};


			string selectionFields = @" complaints.Complaint_Computed_Status_Id StatusId, complaints.Complaint_Computed_Status StatusName, COUNT(*) Total";

			string innerJoinLogic = @"";
			string whereLogic = "";
			string groupLogic = @" complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

			if (hierarchyId == (int)Config.Hierarchy.Tehsil) // Division
			{
				selectionFields = @"complaints.Tehsil_id Hierarchy1Id, " + selectionFields;
				groupLogic = @"complaints.Tehsil_id , " + groupLogic;
				dictRegion = DbTehsil.GetByDistrictAndGroupId(districtId, 4).ToDictionary(x => x.Tehsil_Id, x => x.Tehsil_Name);
			}

			CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
			listDbUserCat = cmsCookie.ListDbUserCategory;

			commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
			paramsSchoolEducation = SetParamsDynamicQuery(4, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
					"1,0", dtParams, Config.ComplaintType.Complaint,
					Config.StakeholderComplaintListingType.UptilMyHierarchy, "RegionStatusWiseSummary", selectionFields, innerJoinLogic, groupLogic, "");

			queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
			dt = DBHelper.GetDataTableByQueryString(queryStr, null);

			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.RegionAndStatusWiseCountTempData>();


			List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionAndStatusWiseCount = new List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
			List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTemp = null;
			string resolvingOfficerName = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount regionAndStatusWiseCount = null;
			MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount cumulativeStatusWiseCount = null;



			int i = 1;
			var regionAndStatusWiseList = listRegionAndStatusWiseCountTempData.GroupBy(n => n.Hierarchy1Id);

			cumulativeStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

			foreach (var regionStatusGroup in regionAndStatusWiseList)
			{
				listRegionAndStatusWiseCountTemp = listRegionAndStatusWiseCountTempData.Where(n => n.Hierarchy1Id == regionStatusGroup.Key).ToList();

				regionAndStatusWiseCount = new MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount();

				foreach (MainSummaryReport.RegionAndStatusWiseCountTempData regionStatusWiseCount in listRegionAndStatusWiseCountTemp)
				{
					regionAndStatusWiseCount.Hierarchy1Data = dictRegion[regionStatusWiseCount.Hierarchy1Id];
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingFresh)
					{
						regionAndStatusWiseCount.PendingFresh = regionAndStatusWiseCount.PendingFresh + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
					{
						regionAndStatusWiseCount.PendingReopened = regionAndStatusWiseCount.PendingReopened + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Resolved)
					{
						regionAndStatusWiseCount.Resolved = regionAndStatusWiseCount.Resolved + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ClosedVerified)
					{

					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
					{
						regionAndStatusWiseCount.Overdue = regionAndStatusWiseCount.Overdue + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.Irrelevant)
					{
						regionAndStatusWiseCount.Irrelevant = regionAndStatusWiseCount.Irrelevant + regionStatusWiseCount.Total;
					}
					if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.InProgress)
					{
						regionAndStatusWiseCount.Forwarded = regionAndStatusWiseCount.Forwarded + regionStatusWiseCount.Total;
					}

				}



				cumulativeStatusWiseCount.Resolved = cumulativeStatusWiseCount.Resolved + regionAndStatusWiseCount.Resolved;
				cumulativeStatusWiseCount.Overdue = cumulativeStatusWiseCount.Overdue + regionAndStatusWiseCount.Overdue;
				regionAndStatusWiseCount.Total = regionAndStatusWiseCount.Total + regionAndStatusWiseCount.PendingFresh + regionAndStatusWiseCount.Resolved +
													 regionAndStatusWiseCount.PendingReopened + regionAndStatusWiseCount.Overdue + regionAndStatusWiseCount.Forwarded + regionAndStatusWiseCount.Irrelevant;

				regionAndStatusWiseCount.SrNo = null;
				listRegionAndStatusWiseCount.Add(regionAndStatusWiseCount);
				i++;
			}
			cumulativeStatusWiseCount.Hierarchy1Data = "Total";
			cumulativeStatusWiseCount.Total = cumulativeStatusWiseCount.PendingFresh + cumulativeStatusWiseCount.Resolved +
											  cumulativeStatusWiseCount.PendingReopened + cumulativeStatusWiseCount.Overdue;
			cumulativeStatusWiseCount.SrNo = null;
			//listRegionAndStatusWiseCount.Add(cumulativeStatusWiseCount);

			//for (int i = 0; i < listRegionAndStatusWiseCount.Count; i++)
			//{
			//    listRegionAndStatusWiseCount[i].SrNo = (i + 1).ToString();
			//    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
			//}

			return listRegionAndStatusWiseCount.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount>();
		}




		#region fom API
		public static ZimmedarShehriModel.LoginResponseModel SubmitStakeholderLogin(ZimmedarShehriModel.LoginRequest submitStakeHolderLoginRequest, Config.PlatformID platformId)
		{

			DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLoginRequest.Username, submitStakeHolderLoginRequest.Password);
			bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
			DbUsers dbUserTemp = null;

			if (isUsernamePresent) // username and cnic present
			{
				string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLoginRequest.Username);
				if (imeiNo == null) // if imei not registered then register it
				{
					DBContextHelperLinq db = new DBContextHelperLinq();
					List<DbUsers> listDbUser = new List<DbUsers>();
					listDbUser.Add(user); //DbUsers.GetByCnic(user.Cnic, db);
										  //listDbUser.Add(user);

					foreach (DbUsers dbUser in listDbUser)
					{
						dbUser.Imei_No = submitStakeHolderLoginRequest.ImeiNo;
						db.DbUsers.Attach(dbUser);
						db.Entry(dbUser).Property(x => x.Imei_No).IsModified = true;
						db.SaveChanges();
						dbUserTemp = dbUser;
					}
					if (dbUserTemp != null)
					{
						TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
					}

					return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
				}
				// If user credentials is correct and imei not registered then register imei
				else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLoginRequest.Username, submitStakeHolderLoginRequest.ImeiNo) != null)
				{
					DBContextHelperLinq db = new DBContextHelperLinq();
					//---- old code ----
					//List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
					//---- end old code ----
					List<DbUsers> listDbUser = new List<DbUsers>();
					listDbUser.Add(user); //DbUsers.GetByCnic(user.Cnic, db);
										  //listDbUser.Add(user);

					return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
				}
				else // wrong mobile
				{
					return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
				}
			}
			else // if username not present
			{
				return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
			}

		}


		public static ZimmedarShehriModel.LoginResponseModel SubmitStakeholderLoginImeiNoRestriction(ZimmedarShehriModel.LoginRequest submitStakeHolderLogin, Config.PlatformID platformId)
		{
			DBContextHelperLinq db = new DBContextHelperLinq();
			DbUsers user = DbUsers.GetByUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password, db).FirstOrDefault();
			bool isUsernamePresent = (user != null); //DbUsers.IsUsernameAndPasswordPresent(submitStakeHolderLogin.Username, submitStakeHolderLogin.Password);
			DbUsers dbUserTemp = null;

			// For IOS platform
			/*if (platformId == Config.PlatformID.IOS)
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
             */
			// End for IOS platform


			if (isUsernamePresent) // username and cnic present
			{
				string imeiNo = DbUsers.GetImeiAgainstUsername(submitStakeHolderLogin.Username);
				//if (imeiNo == null) // if imei not registered then register it
				{

					//---- old code ----
					//List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
					//listDbUser.Add(user);
					//---- end old code ----

					List<DbUsers> listDbUser = null;//DbUsers.GetByCnic(user.Cnic, db);
					if (user.Cnic == null)
					{
						listDbUser = new List<DbUsers>();
						listDbUser.Add(user);
						//listDbUser = DbUsers.GetByCnic(user.Cnic, db);
					}
					else
					{
						listDbUser = DbUsers.GetByCnic(user.Cnic, db);
					}
					//listDbUser.Add(user);

					for (int i = 0; i < listDbUser.Count; i++)
					{
						listDbUser[i].Imei_No = submitStakeHolderLogin.ImeiNo;
						db.DbUsers.Attach(listDbUser[i]);
						db.Entry(listDbUser[i]).Property(x => x.Imei_No).IsModified = true;

						dbUserTemp = listDbUser[i];
					}
					db.SaveChanges();
					if (dbUserTemp != null)
					{
						TextMessageHandler.SendVerificationMessageToStakeholder(dbUserTemp);
					}

					return new ZimmedarShehriModel.LoginResponseModel(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined and Imei registered successfully."));
				}
				// If user credentials is correct and imei not registered then register imei
				/*else if (DbUsers.GetByUsernameAndImeiNo(submitStakeHolderLogin.Username, submitStakeHolderLogin.ImeiNo) != null)
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //---- old code ----
                    //List<DbUsers> listDbUser = new List<DbUsers>(); //DbUsers.GetByCnic(submitStakeHolderLogin.Cnic, db);
                    //---- end old code ----
                    List<DbUsers> listDbUser = DbUsers.GetByCnic(user.Cnic, db);
                    //listDbUser.Add(user);

                    return new SEResponseStakeholderLogin(listDbUser, new ApiStatus(Config.ResponseType.Success.ToString(), "User has successfully logined "));
                }
                else // wrong mobile
                {
                    return new SEResponseStakeholderLogin(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "This user is already registered on another device"));
                }*/
			}
			else // if username not present
			{
				return new ZimmedarShehriModel.LoginResponseModel(null, new ApiStatus(Config.ResponseType.Failure.ToString(), "Username or Password is incorrect"));
			}

		}


		public static ZimmedarShehriModel.StatusesModelResponse GetStakeholderValidStatuses(string userName, Config.Language language,
			Config.AppID appId, Config.PlatformID platformId, int appVersionId)
		{
			ZimmedarShehriModel.StatusesModelResponse zhStatusModel = new ZimmedarShehriModel.StatusesModelResponse();


			DbUsers dbUser = DbUsers.GetByUserName(userName);
			if (dbUser != null)
			{
				//DbUsersVersionMapping.Update_AddVersion(Config.UserType.Resolver, dbUser.Id, (int)platformId, (int)appId, appVersionId);

				//List<DbStatuses> listDbStatuses = DbStatuses.GetByCampaignId(Convert.ToInt32(dbUser.Campaigns));

				// Status filter 
				List<DbPermissionsAssignment> listDbPermissionsAssignment = DbPermissionsAssignment
					.GetListOfPermissionsByTypeTypeIdAndPermissionId(
						(int)Config.PermissionsType.User, dbUser.Id,
						(int)Config.Permissions.StatusesForComplaintListing);

				List<DbStatus> listDbStatuses =
					GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), dbUser.Id,
						listDbPermissionsAssignment);

				Dictionary<string, TranslatedModel> translationDict =
					DbTranslationMapping.GetTranslationDictionaryFromTranslationMapping_API(
						DbTranslationMapping.GetAllTranslation());
				listDbStatuses.GetTranslatedList<DbStatus>("Status", translationDict, language);

				//StatusList statusList = new StatusList(listDbStatuses);

				zhStatusModel.ListFilterStatus = listDbStatuses;
				//return statusList;


				// Status compaint assigned to me
				DbPermissionsAssignment dbPermissionAssignment = DbPermissionsAssignment.GetListOfPermissions
					((int)Config.PermissionsType.User, dbUser.Id, (int)Config.Permissions.StakeholderStatusesOnStatusChangeView
					).FirstOrDefault();
				//DbPermissions dbPermission =
				//DbPermissions.GetPermissionsByPermissionAndType(
				// (int) Config.Permissions.StakeholderStatusesOnStatusChangeView, (int)Config.PermissionsType.User);
				if (dbPermissionAssignment != null)
				{
					List<DbStatus> listDbStatus =
						DbStatus.GetByStatusIds(
							Utility.GetIntList(dbPermissionAssignment.Permission_Value));
					zhStatusModel.ListAssignableStatus = listDbStatus;
					SetAlteredStatus(zhStatusModel.ListAssignableStatus);
					SetAlteredStatus(zhStatusModel.ListFilterStatus);
				}
				else
				{
					zhStatusModel.ListAssignableStatus = listDbStatuses;
				}
			}
			zhStatusModel.Message = "Success";
			zhStatusModel.Status = Config.ResponseType.Success.ToString();
			return zhStatusModel;
		}

		public static void SetAlteredStatus(List<DbStatus> listDbStatuses)
		{
			foreach (DbStatus dbStatus in listDbStatuses)
			{
				dbStatus.Status = GetAlterStatusStr(dbStatus.Status);
			}

		}

		public static string GetAlterStatusStr(string statusStr)
		{
			if (statusStr == Config.UnsatisfactoryClosedStatus)
			{
				return Config.SchoolEducationUnsatisfactoryStatus;
			}
			return statusStr;
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
		#endregion
	}
}