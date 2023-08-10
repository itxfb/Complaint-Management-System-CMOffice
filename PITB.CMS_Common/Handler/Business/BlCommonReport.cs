using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.StakeHolder;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlCommonReport
    {


        public static VmStatusWiseComplaintsData BarChartUserWise(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status, int userId, string categories)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = GetUsersDashboardData(userId, datelow, datemax, status, categories);

            return statusWiseData;
        }

        public static VmStakeholderPieChart GetDataPieLegentChartProgress(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status,string categories="-1")
        {
            VmStakeholderPieChart totalCount = new VmStakeholderPieChart();

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = GetUsersDashboardData(cookie.UserId, datelow, datemax, status, categories);
            if (statusWiseData != null)
            {
                for (int i = 0; i < statusWiseData.ListUserWiseData.Count; i++)
                {
                    for (int j = 0; j < statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount.Count; j++)
                    {
                        singletemp = new VmStatusWiseCount();
                        singletemp.name = statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount[j].StatusId + "-" + statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount[j].StatusString;
                        singletemp.y = statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount[j].Count;
                        temp.Add(singletemp);
                    }
                }
                totalCount.AllStatusColorList = statusWiseData.AllStatusColorList;
            }

            totalCount.HierarchyId = AuthenticationHandler.GetCookie().User_Hierarchy_Id.ToString();
            totalCount.ListStatusWiseCount = temp;

            return totalCount;
        }

        public static VmStatusWiseComplaintsData GetUsersDashboardData(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph,string categories)
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

            switch (userWiseGraph)
            {
                case Config.UserWiseGraphType.MyOwn:
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListing);

                    listDbStatuses =
                        BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                            listDbPermissionsAssignment);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.AssignedToMe, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    break;


                case Config.UserWiseGraphType.LowerIndividual:
                    return GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true, categories);
                
                    break;

                case Config.UserWiseGraphType.CumulationOfLower:
                    // start custom
                    dbUser = DbUsers.GetActiveUser(userId);
                    listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(
                        (int)Config.PermissionsType.User, userId, (int)Config.Permissions.StatusesForComplaintListingAll);

                    //listDbStatuses = DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses);


                    listDbStatuses = BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), userId,
                        listDbPermissionsAssignment, Config.Permissions.StatusesForComplaintListingAll);

                    userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                    paramsSchoolEducation = SetParamsSchoolEducation(Utility.GetUserFromCookie(), startDate, endDate, dbUser.Campaigns, dbUser.Categories, userStatuses,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

                    listVmUserWiseStatus = new List<VmUserWiseStatus>();
                    listVmUserWiseStatus.Add(MakeEmptyStatusModel(dbUser, listDbStatuses));
                    queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);

                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        ds = DBHelper.GetDataSetByQueryString(queryStr, null);

                        statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                        VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, false);
                        return statusWiseComplaintData;
                    }
                    return null;
                    // end custom

                    //statusWiseComplaintData = GetLowerIndividual(userId, startDate, endDate, userWiseGraph, true);
                    //return GetCumulatedStatusWiseComplaintData(statusWiseComplaintData, userId);
                    break;
            }
            return new VmStatusWiseComplaintsData();
        }

        public static VmUserWiseStatus MakeEmptyStatusModel(DbUsers dbUser, List<DbStatus> listStatus)
        {
            VmUserWiseStatus vmUserWiseStatus = new VmUserWiseStatus();
            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();
            VmStatusCount vmStatusCount = null;


            vmUserWiseStatus.UserId = dbUser.User_Id;
            //vmUserWiseStatus.Name = (dbUser.Name!=null) ? dbUser.Name : dbUser.Username;
            //vmUserWiseStatus.Name = (dbUser.Designation_abbr != null)
            //    ? vmUserWiseStatus.Name.Trim() + " [" + dbUser.Designation_abbr.Trim() + "]"
            //    : vmUserWiseStatus.Name.Trim();

            //-------- Adding new code -------
            string hierarchyVal = DbUsers.GetHierarchyVal(dbUser);
            vmUserWiseStatus.Name = (dbUser.Username != null)
               ? hierarchyVal + " [" + dbUser.Username.Trim() + "]"+"__Categories:"+dbUser.Categories+""
               : hierarchyVal;
            //--------------------------------

            foreach (DbStatus dbStatus in listStatus)
            {
                vmStatusCount = new VmStatusCount();
                vmStatusCount.StatusId = dbStatus.Complaint_Status_Id;
                vmStatusCount.StatusString = dbStatus.Status; //Utility.GetAlteredStatus(dbStatus.Status, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                vmStatusCount.Count = 0;
                vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
            }
            return vmUserWiseStatus;
        }

        public static VmStatusWiseComplaintsData GetUserStatusWiseComplaintData(DataSet dataSet, List<VmUserWiseStatus> listVmUserWiseStatus)
        {
            VmStatusWiseComplaintsData vmStatusWiseComplaintData = new VmStatusWiseComplaintsData();
            vmStatusWiseComplaintData.ListUserWiseData = new List<VmUserWiseStatus>();
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
                        if (!isUserPresent)
                        {
                            vmUserWiseStatus = new VmUserWiseStatus();
                            vmUserWiseStatus.UserId = Convert.ToInt32(row["User_Id"]);
                            vmUserWiseStatus.Name = vmUserWiseStatusToMerge.Name;
                            vmUserWiseStatus.ListVmStatusWiseCount = new List<VmStatusCount>();

                            vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatus);
                            isUserPresent = true;
                        }

                        vmStatusCount = new VmStatusCount();
                        vmStatusCount.StatusId = Convert.ToInt32(row["Complaint_Computed_Status_Id"]);
                        vmStatusCount.StatusString = row["Complaint_Computed_Status"].ToString(); //Utility.GetAlteredStatus(row["Complaint_Computed_Status"].ToString(), Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                        vmStatusCount.Count = Convert.ToInt32(row["Count"]);
                        vmUserWiseStatus.ListVmStatusWiseCount.Add(vmStatusCount);
                    }
                    Utility.MergeLists(vmUserWiseStatus.ListVmStatusWiseCount, vmUserWiseStatusToMerge.ListVmStatusWiseCount);
                }
                else
                {
                    vmStatusWiseComplaintData.ListUserWiseData.Add(vmUserWiseStatusToMerge);
                }
                i++;
            }
            return vmStatusWiseComplaintData;
        }


        public static ListingParamsModelBase SetParamsSchoolEducation(DbUsers dbUser, string fromDate, string toDate, string campaign, string category, string complaintStatuses, string commaSeperatedTransferedStatus, DataTableParamsModel dtParams, Config.ComplaintType complaintType, Config.StakeholderComplaintListingType listingType, string spType)
        {
            string extraSelection = " complaints.Department_Name, complaints.Complaint_SubCategory_Name, complaints.RefField1, complaints.RefField2, complaints.RefField3, complaints.RefField4, complaints.RefField5, complaints.RefField6, complaints.Person_Cnic, complaints.Computed_Overdue_Days, ";

            //CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsModelBase paramsModel = new ListingParamsModelBase();
            paramsModel.SelectionFields = "";
            paramsModel.InnerJoinLogic = "";
            if (spType == "ExcelReport")
            {
                paramsModel.SelectionFields = @"CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10)) AS [Complaint No],
					complaints.Complaint_Computed_Status as Complaint_Status,Computed_Remaining_Total_Time, 
                    complaints.Complaint_Computed_Hierarchy as [Escalation Level],
                    complaints.Computed_Overdue_Days,
					complaints.FollowupCount,
					-- C.Person_Name as [Person Name],
					-- C.Cnic_No as [Cnic No],
					-- CASE C.Gender WHEN 1 THEN 'MALE' ELSE 'FEMALE' END AS Gender,
					-- complaints.Person_District_Name as [Caller District],
					-- CONVERT(VARCHAR(10),complaints.Created_Date,120) Date,
					-- C.Mobile_No as [Mobile No],
					-- C.Person_Address as [Person Address],
					D.District_Name [Complaint District],
					B.Name Category  ,
					F.Name as [Sub Category],
					complaints.Complaint_Remarks as [Complaint Remarks],
					--complaints.Agent_Comments as [Agent Comments],
					--P.[Status],
					
					complaints.Created_Date as [Created Date]--,
					--df.FieldName,df.FieldValue";

                paramsModel.InnerJoinLogic = @"INNER JOIN PITB.Complaints_Type B ON complaints.Complaint_Category=B.Id
					INNER JOIN PITB.Complaints_SubType F ON complaints.Complaint_SubCategory=F.Id
					INNER JOIN PITB.Person_Information C ON complaints.Person_Id=C.Person_id
					INNER JOIN PITB.Districts D ON complaints.District_Id=D.id
					INNER JOIN PITB.Statuses P ON p.Id=complaints.Complaint_Computed_Status_Id
                    --LEFT JOIN pitb.Dynamic_ComplaintFields df ON df.ComplaintId = complaints.Id
                    ";
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
            paramsModel.SelectionFields = paramsModel.SelectionFields + extraSelection;
            paramsModel.SpType = spType;


            if (dbUser.SubRole_Id == Config.SubRoles.SDU && (listingType == Config.StakeholderComplaintListingType.AssignedToMe || listingType == Config.StakeholderComplaintListingType.UptilMyHierarchy))
            {
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.SelectionFields = @"complaints.StatusReopenedCount, complaints.Callback_Count, complaints.Callback_Status, complaints.Callback_Comment, complaints.Person_Contact, schoolMap.Assigned_To, schoolMap.Assigned_To_Name,CONVERT(VARCHAR(10),complaints.StatusChangedDate_Time,120) StatusChangedDate_Time ," + paramsModel.SelectionFields;
                paramsModel.InnerJoinLogic = paramsModel.InnerJoinLogic + @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
					ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int)Config.SchoolEducationUserSubRoles.SDU + " OR (complaints.Status_ChangedBy=" + Config.MEALoginId + "AND complaints.Complaint_Status_Id=" + (int)Config.ComplaintStatus.ResolvedVerified + ") ) ";
            }
            else if (dbUser.User_Id == Config.MEALoginId && listingType == Config.StakeholderComplaintListingType.AssignedToMe)
            {
                paramsModel.UserHierarchyId = (int)Config.Hierarchy.None;
                paramsModel.InnerJoinLogic = paramsModel.InnerJoinLogic + @" INNER JOIN pitb.School_Category_User_Mapping schoolMap
					ON complaints.Complaint_SubCategory=schoolMap.Category_Id ";
                paramsModel.WhereLogic = " AND (schoolMap.Assigned_To = " + (int)Config.SchoolEducationUserSubRoles.MEA + " AND complaints.Complaint_Computed_Status_Id=" + (int)Config.ComplaintStatus.ResolvedUnverified + ") ";
                paramsModel.IgnoreComputedHierarchyCheck = true;
                paramsModel.UserCategoryId1 = null;
                paramsModel.UserCategoryId2 = null;
                paramsModel.CheckIfExistInUserSrcId = 0;
            }
            else
            {
                paramsModel.IgnoreComputedHierarchyCheck = false;
            }

            return paramsModel;
        }

        private static VmStatusWiseComplaintsData GetLowerIndividual(int userId, string startDate, string endDate, Config.UserWiseGraphType userWiseGraph, bool canDiscartHierarchyLowerThanImmediateOne, string categories)
        {
            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = null;
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;

            dbUser = DbUsers.GetActiveUser(userId);
            if (categories != "-1")
            {
                dbUser.Categories = categories;
            }
            List<DbUsers> listDbUsers = UsersHandler.FindUserLowerThanCurrentHierarchy2(userId, Utility.GetIntByCommaSepStr(dbUser.Campaigns),
                (Config.Hierarchy)dbUser.Hierarchy_Id, Utility.GetIntByCommaSepStr(DbUsers.GetHierarchyIdValueAgainstHierarchyId(dbUser)), canDiscartHierarchyLowerThanImmediateOne, null, Utility.GetIntList(dbUser.Categories));

            listDbUsers = listDbUsers.Where(n => !Config.ListSchoolUsersToDiscartDashboard.Contains(n.User_Id)).ToList();

            listDbPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdListAndPermissionId(
                (int)Config.PermissionsType.User, listDbUsers.Select(n => n.User_Id).ToList(), (int)Config.Permissions.StatusesForComplaintListing);


            listVmUserWiseStatus = new List<VmUserWiseStatus>();
            queryStr = "";
            foreach (DbUsers user in listDbUsers)
            {

                listDbStatuses = DbStatus.GetByCampaignId(Utility.GetIntByCommaSepStr(user.Campaigns));
                //BlCommon.GetStatusListByCampaignIdsAndPermissions(Utility.GetIntList(dbUser.Campaigns), user.User_Id,
                //    listDbPermissionsAssignment);

                userStatuses = string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());

                paramsSchoolEducation = SetParamsSchoolEducation(user, startDate, endDate, dbUser.Campaigns, user.Categories, userStatuses,
                    "1,0", dtParams, Config.ComplaintType.Complaint,
                    Config.StakeholderComplaintListingType.UptilMyHierarchy, "StatusWiseUserComplaints");

                queryStr = queryStr + StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
                listVmUserWiseStatus.Add(MakeEmptyStatusModel(user, listDbStatuses));
            }
            if (!string.IsNullOrEmpty(queryStr))
            {
                ds = DBHelper.GetDataSetByQueryString(queryStr, null);
                statusWiseComplaintData = GetUserStatusWiseComplaintData(ds, listVmUserWiseStatus);
                VmStatusWiseComplaintsData.SortStatusWiseData(Config.ListSchoolEducationStatuses,
                            statusWiseComplaintData, true);


                statusWiseComplaintData.ListUserWiseData =
                    statusWiseComplaintData.ListUserWiseData.OrderByDescending(
                        n => n.TotalStatusWiseCount).ToList();

                return statusWiseComplaintData;
            }
            return null;
        }
    }
}