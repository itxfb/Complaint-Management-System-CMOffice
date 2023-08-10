using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Complaint;
using PITB.CMS.Handler.StakeHolder;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.Custom.DataTable;
using PITB.CMS.Models.Custom.Reports;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS.Handler.Business
{
    public class BlSchoolReports
    {

        public static List<MainSummaryReport.OverDueComplaint> GetOverdueComplaintsReport(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType)
        {
            Config.SummaryReportType rType = (Config.SummaryReportType)reportType;
            int campaignId = Utility.GetIntByCommaSepStr(campId);
            List<DbUserCategory> listDbUserCat = null;
            string queryStr = "";
            DataTable dt;
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            Dictionary<int, string> dictColumnsMap = new Dictionary<int, string>
            {
                {1,"Province_Name"},
                {2,"Division_Name"},
                {3,"District_Name"},
                {4,"Tehsil_Name"},
                {5,"UnionCouncil_Name"}
            };


            string selectionFields = @" DISTINCT (CAST((complaints.Compaign_Id) as nvarchar(max))+ '-' + CAST((complaints.Id) as nvarchar(max))) AS ComplaintNo, complaints.Department_Name Department, complaints.Complaint_Category_Name Category, complaints.Complaint_SubCategory_Name SubCategory,
					RTRIM(LTRIM(CAST(users.Name as nvarchar(max))))+' : '+RTRIM(LTRIM(CAST(users.Designation_abbr as nvarchar(max)))) ResolvingOfficer, CONVERT(DATE,complaints.Created_Date,120) [Date],
					DATEDIFF(DAY,complaints.MaxSrcIdDate, GETDATE()) DaysOverDue, schoolMap.school_name SchoolName,schoolMap.school_emis_code EmisCode, complaints." +
                                     dictColumnsMap[hierarchyId + 1] + " as Hierarchy1Data, complaints." + dictColumnsMap[hierarchyId + 2] + " as Hierarchy2Data";

            string innerJoinLogic = @"INNER JOIN dbo.Schools_Mapping schoolMap ON schoolMap.Id = complaints.TableRowRefId
					INNER JOIN PITB.User_Wise_Complaints userWiseComplaints ON (userWiseComplaints.Complaint_Id = complaints.Id AND userWiseComplaints.Complaint_Type = 1 AND userWiseComplaints.Complaint_Subtype = 1)
					INNER JOIN PITB.Users users ON users.Id = userWiseComplaints.User_Id";
            string whereLogic = "";

            if (rType == Config.SummaryReportType.General) // general report
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                listDbUserCat = cmsCookie.ListDbUserCategory;
            }
            else if(rType == Config.SummaryReportType.Specific)
            {
                listDbUserCat = DbUserCategory.GetEmptyParentChildList();
            }

            Dictionary<int, string> dictRegion = new Dictionary<int, string>();

            if (hierarchyId == (int)Config.Hierarchy.Province) // Province
            {
                dictRegion = DbDivision.GetByProvinceIdsStr(commaSepVal).ToDictionary(n => n.Division_Id, n => n.Division_Name);
                selectionFields = selectionFields + ", " + "complaints.Province_Id as CrmId";
            }
            if (hierarchyId == (int)Config.Hierarchy.Division) // Division
            {
                dictRegion = DbDistrict.GetByDivisionIdsStr(commaSepVal).ToDictionary(n => n.District_Id, n => n.District_Name);
                selectionFields = selectionFields + ", " + "complaints.Division_Id as CrmId";
            }
            if (hierarchyId == (int)Config.Hierarchy.District) // District
            {
                dictRegion = DbTehsil.GetByDistrictIdsStr(commaSepVal).ToDictionary(n => n.Tehsil_Id, n => n.Tehsil_Name);
                selectionFields = selectionFields + ", " + "complaints.District_Id as CrmId";
            }

            commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());


            paramsSchoolEducation = SetParamsDynamicQuery(hierarchyId+1, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
                    "1,0", dtParams, Config.ComplaintType.Complaint,
                    Config.StakeholderComplaintListingType.UptilMyHierarchy, "OverDueComplaintsSummary", selectionFields, innerJoinLogic, "", "");

            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            dt = DBHelper.GetDataTableByQueryString(queryStr, null);

            List<MainSummaryReport.OverDueComplaint> listOverdueComplaints = dt.ToList<MainSummaryReport.OverDueComplaint>();


            List<MainSummaryReport.OverDueComplaint> listOverdueComplaintsGroupedBy = new List<MainSummaryReport.OverDueComplaint>();
            List<MainSummaryReport.OverDueComplaint> listOverDueComplaintOfGroup = null;
            string resolvingOfficerName = null;
            MainSummaryReport.OverDueComplaint overdueComplaint = null;


            var complaintsGroupByList = listOverdueComplaints.GroupBy(n => n.ComplaintNo);
            foreach (var complaintGroup in complaintsGroupByList)
            {
                listOverDueComplaintOfGroup = listOverdueComplaints.Where(n => n.ComplaintNo == complaintGroup.Key).ToList();
                resolvingOfficerName = String.Join(",", listOverDueComplaintOfGroup.Select(n => n.ResolvingOfficer).ToList());

                overdueComplaint = listOverDueComplaintOfGroup.FirstOrDefault();
                overdueComplaint.ResolvingOfficer = resolvingOfficerName;
                listOverdueComplaintsGroupedBy.Add(overdueComplaint);
            }

            listOverdueComplaintsGroupedBy = listOverdueComplaintsGroupedBy.OrderByDescending(n => n.DaysOverDue).ToList();


            for (int i = 0; i < listOverdueComplaintsGroupedBy.Count; i++)
            {
                listOverdueComplaintsGroupedBy[i].SrNo = (i + 1);
                //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
            }

            return listOverdueComplaintsGroupedBy;
        }
        public static List<Tuple<int,string,int>> GetComplaintsSummaryReport(string startDate, string endDate, string campId, int hierarchyId, string commaSepVal, string statusIds)
        {
            Dictionary<int, string> dictRegion = new Dictionary<int, string>();
            string whereClause = null;
            if (hierarchyId == (int)Config.Hierarchy.Province) // Province
            {
                //dictRegion = DbDivision.GetByProvinceIdsStr(commaSepVal).ToDictionary(n => n.Division_Id, n => n.Division_Name);
                whereClause = "Division_Id ";
            }
            if (hierarchyId == (int)Config.Hierarchy.Division) // Division
            {
                //dictRegion = DbDistrict.GetByDivisionIdsStr(commaSepVal).ToDictionary(n => n.District_Id, n => n.District_Name);
                whereClause = "District_Id ";
            }
            if (hierarchyId == (int)Config.Hierarchy.District) // District
            {
               // dictRegion = DbTehsil.GetByDistrictIdsStr(commaSepVal).ToDictionary(n => n.Tehsil_Id, n => n.Tehsil_Name);
                whereClause = "Tehsil_Id ";
            }

            //commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
            string queryStr = "SELECT SUM(CASE WHEN Complaint_Type = 0 THEN 1 ELSE 0 END) AS 'None' , " +
            "SUM(CASE WHEN Complaint_Type = 1 THEN 1 ELSE 0 END) AS 'Complaint', " +
            "SUM(CASE WHEN Complaint_Type = 2 THEN 1 ELSE 0 END) AS 'Suggestion', " +
            "SUM(CASE WHEN Complaint_Type = 3 THEN 1 ELSE 0 END) AS 'Inquiry', " +
            "SUM(CASE WHEN (Complaint_Type = 3 OR Complaint_Type = 2) THEN 1 ELSE 0 END) AS 'Suggestion&Inquiry', " +
            "SUM(CASE WHEN (Complaint_Type = 3 OR Complaint_Type = 2 OR Complaint_Type = 1) THEN 1 ELSE 0 END) AS 'Total' " +
            "FROM PITB.Complaints WHERE Compaign_Id = "+campId+" AND Created_Date >= '"+ startDate +"'  " +
            "AND Created_Date <= '"+endDate+"'  " +
            "AND "+ whereClause + " IN ("+ commaSepVal+") " +
            "AND Complaint_Status_Id IN ("+ statusIds +") " +
            "AND Complaint_Type is not null;";
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);
            List<Tuple<int, string, int>> lst = new List<Tuple<int, string, int>>();
            DataRow row =  dt.Rows.Cast<DataRow>().First();
            Tuple<int, string, int> item = new Tuple<int,string,int>(1,"Suggestion + Inquiry",Int32.Parse(row["Suggestion&Inquiry"].ToString()));
            lst.Add(item);
            item = new Tuple<int, string, int>(2, "Complaint", Int32.Parse(row["Complaint"].ToString()));
            lst.Add(item);
            item = new Tuple<int, string, int>(3, "Total", Int32.Parse(row["Total"].ToString()));
            lst.Add(item);
            return lst;     
        }

        public static List<MainSummaryReport.TopOverDueComplaintsByOfficer> GetTopOverdueComplaintsByOfficerReport(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType)
        {
            try
            {


                Config.SummaryReportType rType = (Config.SummaryReportType)reportType;
                int campaignId = Utility.GetIntByCommaSepStr(campId);
                List<DbUserCategory> listDbUserCat = null;
                string queryStr = "";
                DataTable dt;
                DataTableParamsModel dtParams = null;
                ListingParamsModelBase paramsSchoolEducation = null;

                Dictionary<int, string> dictColumnsMap = new Dictionary<int, string>
                {
                    {1,"Province_Name"},
                    {2,"Division_Name"},
                    {3,"District_Name"},
                    {4,"Tehsil_Name"},
                    {5,"UnionCouncil_Name"}
                };


                string selectionFields = @"(CAST((complaints.Compaign_Id) as nvarchar(max))+ '-' + CAST((complaints.Id) as nvarchar(max))) AS ComplaintNo, users.id UserId,
					RTRIM(LTRIM(CAST(users.Name as nvarchar(max))))+' : '+RTRIM(LTRIM(CAST(users.Designation_abbr as nvarchar(max)))) ResolvingOfficer," +
                                         dictColumnsMap[hierarchyId + 1] + " as Hierarchy1Data, complaints." + dictColumnsMap[hierarchyId + 2] + " as Hierarchy2Data";

                string innerJoinLogic = @"
					INNER JOIN PITB.User_Wise_Complaints userWiseComplaints ON (userWiseComplaints.Complaint_Id = complaints.Id AND userWiseComplaints.Complaint_Type = 1 AND userWiseComplaints.Complaint_Subtype = 1)
					INNER JOIN PITB.Users users ON users.Id = userWiseComplaints.User_Id";
                string whereLogic = "";

                Dictionary<int, string> dictRegion = new Dictionary<int, string>();

                if (hierarchyId == (int)Config.Hierarchy.Province) // Province
                {
                    dictRegion = DbDivision.GetByProvinceIdsStr(commaSepVal).ToDictionary(n => n.Division_Id, n => n.Division_Name);
                    selectionFields = selectionFields + ", " + "complaints.Province_Id as CrmId";
                }
                if (hierarchyId == (int)Config.Hierarchy.Division) // Division
                {
              
                    dictRegion = DbDistrict.GetByDivisionIdsStr(commaSepVal).ToDictionary(n => n.District_Id, n => n.District_Name);
                    selectionFields = selectionFields + ", " + "complaints.Division_Id as CrmId";
                }
                if (hierarchyId == (int)Config.Hierarchy.District) // District
                {
                   
                    dictRegion = DbTehsil.GetByDistrictIdsStr(commaSepVal).ToDictionary(n => n.Tehsil_Id, n => n.Tehsil_Name);
                    selectionFields = selectionFields + ", " + "complaints.District_Id as CrmId";
                }

                commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());

                if (rType == Config.SummaryReportType.General) // general report
                {
                    CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                    listDbUserCat = cmsCookie.ListDbUserCategory;
                }

                paramsSchoolEducation = SetParamsDynamicQuery(hierarchyId+1, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
                        "1,0", dtParams, Config.ComplaintType.Complaint,
                        Config.StakeholderComplaintListingType.UptilMyHierarchy, "OverDueComplaintsSummary", selectionFields, innerJoinLogic, "", "");

                queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
                dt = DBHelper.GetDataTableByQueryString(queryStr, null);



                List<MainSummaryReport.TopOverDueComplaintsByOfficer> listTopOverdueComplaints = dt.ToList<MainSummaryReport.TopOverDueComplaintsByOfficer>();


                List<MainSummaryReport.TopOverDueComplaintsByOfficer> listTopOverdueComplaintsGroupedBy = new List<MainSummaryReport.TopOverDueComplaintsByOfficer>();
                List<MainSummaryReport.TopOverDueComplaintsByOfficer> listTopOverDueComplaintOfGroup = null;
                string resolvingOfficerName = null;
                MainSummaryReport.TopOverDueComplaintsByOfficer overdueComplaint = null;


                var complaintsGroupByList = listTopOverdueComplaints.GroupBy(n => n.UserId);
                foreach (var complaintGroup in complaintsGroupByList)
                {
                    listTopOverDueComplaintOfGroup = listTopOverdueComplaints.Where(n => n.UserId == complaintGroup.Key).ToList();
                    var totalCount = listTopOverDueComplaintOfGroup.Count;
                    var districtCount = listTopOverDueComplaintOfGroup.GroupBy(s => s.Hierarchy1Data).ToList().Count;
                    resolvingOfficerName = listTopOverDueComplaintOfGroup.FirstOrDefault().ResolvingOfficer; //String.Join(",", listTopOverDueComplaintOfGroup.Select(n => n.ResolvingOfficer).ToList());

                    overdueComplaint = listTopOverDueComplaintOfGroup.FirstOrDefault();
                    overdueComplaint.OverdueComplaints = listTopOverDueComplaintOfGroup.Count();
                    overdueComplaint.ResolvingOfficer = resolvingOfficerName;
                    listTopOverdueComplaintsGroupedBy.Add(overdueComplaint);
                }

                listTopOverdueComplaintsGroupedBy = listTopOverdueComplaintsGroupedBy.OrderByDescending(n => n.OverdueComplaints).ToList().Take(5).ToList();
                for (int i = 0; i < listTopOverdueComplaintsGroupedBy.Count; i++)
                {
                    listTopOverdueComplaintsGroupedBy[i].SrNo = (i + 1);
                    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
                }
                //return null;
                return listTopOverdueComplaintsGroupedBy;

            }
            catch (Exception exception)
            {

                throw;
            }
            return null;
        }
        public static List<MainSummaryReport.CategoryWiseAndStatusWiseCount> CategorywiseStatuswiseCount(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, string categoryIds, int reportType)
        {
            string query = @"SELECT
                    complaints.Complaint_Category As CategoryId , 
                    complaints.Complaint_Category_Name As CategoryName,
                     complaints.Complaint_Computed_Status_Id As StatusId,
                      complaints.Complaint_Computed_Status As StatusName,
                        Count(*) As Total
                       FROM PITB.Complaints as complaints 
                      WHERE complaints.Complaint_Type = 1 AND EXISTS(SELECT 1 FROM dbo.SplitString('" + categoryIds + "',',') X WHERE X.Item=complaints.Complaint_Category) AND EXISTS(SELECT 1 FROM dbo.SplitString('" + campId + "',',') X WHERE X.Item=complaints.Compaign_Id) AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '"+startDate+"' AND '"+endDate+"' ) AND EXISTS(SELECT 1 FROM dbo.SplitString('"+statusIds+"',',') X WHERE X.Item=complaints.Complaint_Computed_Status_Id) GROUP BY complaints.Complaint_Category,complaints.Complaint_Category_Name,complaints.Complaint_Computed_Status_Id,complaints.Complaint_Computed_Status";
					
            DataTable dt = DBHelper.GetDataTableByQueryString(query, null);
            List<MainSummaryReport.CategoryAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.CategoryAndStatusWiseCountTempData>();
            List<MainSummaryReport.CategoryWiseAndStatusWiseCount> data = new List<MainSummaryReport.CategoryWiseAndStatusWiseCount>();

            foreach (var group in listRegionAndStatusWiseCountTempData.GroupBy(x => x.CategoryId))
            {
                
                MainSummaryReport.CategoryWiseAndStatusWiseCount value = new MainSummaryReport.CategoryWiseAndStatusWiseCount();
                value.CateogoryId = group.Key;
                value.CategoryName = group.First().CategoryName;
                
                if (group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.PendingFresh) != null)
                    value.Opened = group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.PendingFresh).Total;
                
                if (group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.PendingReopened) != null)
                    value.Reopened = group.First(x => x.StatusId == (int)Config.ComplaintStatus.PendingReopened).Total;

                if (group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.ResolvedUnverified || x.StatusId == (int)Config.ComplaintStatus.ResolvedVerified) != null)
                    value.Resolved = group.First(x => x.StatusId == (int)Config.ComplaintStatus.ResolvedUnverified || x.StatusId == (int)Config.ComplaintStatus.ResolvedVerified).Total;
                
                if (group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.ClosedVerified) != null)
                    value.Closed = group.First(x => x.StatusId == (int)Config.ComplaintStatus.ClosedVerified).Total;

                if (group.FirstOrDefault(x => x.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed) != null)
                    value.Overdue = group.First(x => x.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed).Total;
                
                if (value.Closed + value.Reopened + value.Overdue == 0)
                {
                    value._closure_rate = 0d;
                }
                else
                {
                    value._closure_rate = (double)value.Closed / (value.Closed + value.Reopened + value.Overdue);
                }
                value.Total = value.Opened + value.Overdue + value.Reopened + value.Resolved+value.Closed;
                data.Add(value);

            }

            //------- Combine value -----------
            int resolved = 0, opened = 0, overdue = 0, reopened = 0, closed = 0, total = 0;
            double closureRate=0;
            List<MainSummaryReport.CategoryWiseAndStatusWiseCount> listCatToCombine = data.Where(n => new List<string> { "820", "821", "808", "807" }.Contains(n.CateogoryId)).ToList();
            foreach (MainSummaryReport.CategoryWiseAndStatusWiseCount cat in listCatToCombine)
            {
                resolved += cat.Resolved;
                opened += cat.Opened;
                overdue += cat.Overdue;
                reopened += cat.Reopened;
                closed += cat.Closed;
            }
            MainSummaryReport.CategoryWiseAndStatusWiseCount d = new MainSummaryReport.CategoryWiseAndStatusWiseCount();
            d.CategoryName = "PMIU Complaints";
            d.Resolved = resolved;
            d.Opened = opened;
            d.Overdue = overdue;
            d.Reopened = reopened;
            d.Closed = closed;

            if (d.Closed + d.Reopened + d.Overdue == 0)
            {
                d._closure_rate = 0d;
            }
            else
            {
                d._closure_rate = (double)d.Closed / (d.Closed + d.Reopened + d.Overdue);
            }
            d.Total = d.Opened + d.Overdue + d.Reopened + d.Resolved + d.Closed;

            //------- End Combine Value ----------

            data = data.Except(listCatToCombine).ToList();
            data.Add(d);
            return data.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.CategoryWiseAndStatusWiseCount>();
        }
        public static List<MainSummaryReport.RegionAndStatusWiseCount> RegionAndStatusWiseCountForProvinceDistrictReport(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType,string categories = "-1")
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


            if (hierarchyId == (int)Config.Hierarchy.Province) // Province
            {
                selectionFields = @"complaints.Division_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.Division_Id , " + groupLogic;
                dictRegion = DbDivision.GetByProvinceIdsStr(commaSepVal).ToDictionary(n => n.Division_Id, n => n.Division_Name);
            }
            if (hierarchyId == (int)Config.Hierarchy.Division) // Division
            {
                selectionFields = @"complaints.District_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.District_Id , " + groupLogic;
                dictRegion = DbDistrict.GetByDivisionIdsStr(commaSepVal).ToDictionary(n => n.District_Id, n => n.District_Name);
            }
            if (hierarchyId == (int)Config.Hierarchy.District) // District
            {
                selectionFields = @"complaints.Tehsil_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.Tehsil_Id , " + groupLogic;
                dictRegion = DbTehsil.GetByDistrictIdsStr(commaSepVal).ToDictionary(n => n.Tehsil_Id, n => n.Tehsil_Name);
            }

            if (rType == Config.SummaryReportType.General) // general report
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                listDbUserCat = cmsCookie.ListDbUserCategory;
            }
            string categoriesClause = null;
            if (categories.Equals("-1"))
            {
                //categoriesClause = "and (complaints.MaxSrcId is null or complaints.MaxSrcId >=3)";
                categoriesClause = " ";
            }
            else
            {
                categoriesClause = "and (complaints.Complaint_Category IN ("+categories+"))";
                dictRegion = new Dictionary<int, string>();
                dictRegion.Add(7, "Bahawalnagar");
                dictRegion.Add(12, "Muzaffargarh");
                dictRegion.Add(9, "Rahim Yar Khan");
                dictRegion.Add(3, "Bhakkar");
                dictRegion.Add(15, "Jhang");
                dictRegion.Add(11, "Layyah");
                dictRegion.Add(26, "Khanewal");
                dictRegion.Add(8, "Bahawalpur");
                dictRegion.Add(10, "Dera Ghazi Khan");
                dictRegion.Add(23, "Kasur");
                dictRegion.Add(29, "Pakpattan");
                dictRegion.Add(6, "Okara");
                dictRegion.Add(36, "Chiniot");
                dictRegion.Add(27, "Lodhran");
                dictRegion.Add(31, "Vehari");
                dictRegion.Add(13, "Rajanpur");
            }
            

            commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
            queryStr = "SELECT complaints.District_Id Hierarchy1Id,  complaints.Complaint_Computed_Status_Id StatusId, complaints.Complaint_Computed_Status StatusName, COUNT(*) Total " +
                    " FROM pitb.Complaints complaints " +
                    "WHERE complaints.Complaint_Type = 1 and EXISTS(SELECT 1 FROM dbo.SplitString('47',',') X WHERE X.Item=complaints.Compaign_Id) " +
                    "AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + startDate + "' AND '" + endDate + "' ) " +
                    "AND complaints.Complaint_Category NOT IN (324,820,821,808,807) " +
                    "AND EXISTS(SELECT 1 FROM dbo.SplitString('" + statusIds + @"',',') X WHERE X.Item=complaints.Complaint_Computed_Status_Id) " + categoriesClause + "  " +
                     " and EXISTS(SELECT 1 FROM dbo.SplitString('" + commaSepVal + "',',') X WHERE X.Item=complaints.District_Id) Group by complaints.District_Id ,  complaints.Division_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";
            dt = DBHelper.GetDataTableByQueryString(queryStr, null);    
            List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.RegionAndStatusWiseCountTempData>();


            List<MainSummaryReport.RegionAndStatusWiseCount> listRegionAndStatusWiseCount = new List<MainSummaryReport.RegionAndStatusWiseCount>();
            List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTemp = null;
            string resolvingOfficerName = null;
            MainSummaryReport.RegionAndStatusWiseCount regionAndStatusWiseCount = null;
            MainSummaryReport.RegionAndStatusWiseCount cumulativeStatusWiseCount = null;



            int i = 1;
            var regionAndStatusWiseList = listRegionAndStatusWiseCountTempData.GroupBy(n => n.Hierarchy1Id);

            cumulativeStatusWiseCount = new MainSummaryReport.RegionAndStatusWiseCount();

            foreach (var regionStatusGroup in regionAndStatusWiseList)
            {
                listRegionAndStatusWiseCountTemp = listRegionAndStatusWiseCountTempData.Where(n => n.Hierarchy1Id == regionStatusGroup.Key).ToList();

                regionAndStatusWiseCount = new MainSummaryReport.RegionAndStatusWiseCount();

                foreach (MainSummaryReport.RegionAndStatusWiseCountTempData regionStatusWiseCount in listRegionAndStatusWiseCountTemp)
                {
                    regionAndStatusWiseCount.Hierarchy1Data = dictRegion[regionStatusWiseCount.Hierarchy1Id];
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingFresh)
                    {
                        regionAndStatusWiseCount.Opened = regionAndStatusWiseCount.Opened + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
                    {
                        regionAndStatusWiseCount.Reopened = regionAndStatusWiseCount.Reopened + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ResolvedUnverified ||
                        regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ResolvedVerified)
                    {
                        regionAndStatusWiseCount.Resolved = regionAndStatusWiseCount.Resolved + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ClosedVerified)
                    {
                        regionAndStatusWiseCount.Closed = regionAndStatusWiseCount.Closed + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
                    {
                        regionAndStatusWiseCount.Overdue = regionAndStatusWiseCount.Overdue + regionStatusWiseCount.Total;
                    }

                }
                if (regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Reopened == 0)
                {
                    regionAndStatusWiseCount._percentage_closed = 0.0;
                }
                else
                {
                    regionAndStatusWiseCount._percentage_closed = (double)regionAndStatusWiseCount.Closed / (regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Reopened);
                }
                if (regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Reopened + regionAndStatusWiseCount.Overdue == 0)
                {
                    regionAndStatusWiseCount._closure_rate = 0d;
                }
                else
                {
                    regionAndStatusWiseCount._closure_rate = (double)regionAndStatusWiseCount.Closed / (regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Reopened + regionAndStatusWiseCount.Overdue);
                }
                cumulativeStatusWiseCount.Opened +=  regionAndStatusWiseCount.Opened;
                cumulativeStatusWiseCount.Resolved += regionAndStatusWiseCount.Resolved;
                cumulativeStatusWiseCount.Closed += regionAndStatusWiseCount.Closed;
                cumulativeStatusWiseCount.Overdue += regionAndStatusWiseCount.Overdue;
                cumulativeStatusWiseCount.Reopened += regionAndStatusWiseCount.Reopened;
                //cumulativeStatusWiseCount._percentage_closed = (double)cumulativeStatusWiseCount.Closed / (cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Reopened);

                regionAndStatusWiseCount.Total = regionAndStatusWiseCount.Total + regionAndStatusWiseCount.Opened + regionAndStatusWiseCount.Resolved +
                                                     regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Overdue + regionAndStatusWiseCount.Reopened;

                regionAndStatusWiseCount.SrNo = null;
                listRegionAndStatusWiseCount.Add(regionAndStatusWiseCount);
                i++;
            }
            cumulativeStatusWiseCount.Hierarchy1Data = "Total";
            cumulativeStatusWiseCount.Total = cumulativeStatusWiseCount.Opened + cumulativeStatusWiseCount.Resolved +
                                              cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Overdue + cumulativeStatusWiseCount.Reopened;
            if (cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Reopened == 0)
            {
                cumulativeStatusWiseCount._percentage_closed = 0.0;
            }
            else
            {
                cumulativeStatusWiseCount._percentage_closed = (double)cumulativeStatusWiseCount.Closed / (cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Reopened);
            }
            
            cumulativeStatusWiseCount.SrNo = null;
            //listRegionAndStatusWiseCount.Add(cumulativeStatusWiseCount);

            //for (int i = 0; i < listRegionAndStatusWiseCount.Count; i++)
            //{
            //    listRegionAndStatusWiseCount[i].SrNo = (i + 1).ToString();
            //    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
            //}

            return listRegionAndStatusWiseCount.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.RegionAndStatusWiseCount>();
        }
        public static List<MainSummaryReport.RegionAndStatusWiseCount> RegionAndStatusWiseCountReport(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType)
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


            if (hierarchyId == (int)Config.Hierarchy.Province) // Province
            {
                selectionFields = @"complaints.Division_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.Division_Id , " + groupLogic;
                dictRegion = DbDivision.GetByProvinceIdsStr(commaSepVal).ToDictionary(n => n.Division_Id, n => n.Division_Name);
            }
            if (hierarchyId == (int)Config.Hierarchy.Division) // Division
            {
                selectionFields = @"complaints.District_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.District_Id , " + groupLogic;
                dictRegion = DbDistrict.GetByDivisionIdsStr(commaSepVal).ToDictionary(n => n.District_Id, n => n.District_Name);
            }
            if (hierarchyId == (int)Config.Hierarchy.District) // District
            {
                selectionFields = @"complaints.Tehsil_Id Hierarchy1Id, " + selectionFields;
                groupLogic = @"complaints.Tehsil_Id , " + groupLogic;
                dictRegion = DbTehsil.GetByDistrictIdsStr(commaSepVal).ToDictionary(n => n.Tehsil_Id, n => n.Tehsil_Name);
            }

            if (rType == Config.SummaryReportType.General) // general report
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                listDbUserCat = cmsCookie.ListDbUserCategory;
            }
            commaSepVal = Utility.GetCommaSepStrFromList(dictRegion.Keys.ToList());
            paramsSchoolEducation = SetParamsDynamicQuery(hierarchyId+1, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), statusIds,
                    "1,0", dtParams, Config.ComplaintType.Complaint,
                    Config.StakeholderComplaintListingType.UptilMyHierarchy, "RegionStatusWiseSummary", selectionFields, innerJoinLogic, groupLogic, "");

            queryStr = StakeholderListingLogic.GetListingQuery(paramsSchoolEducation);
            dt = DBHelper.GetDataTableByQueryString(queryStr, null);

            List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTempData = dt.ToList<MainSummaryReport.RegionAndStatusWiseCountTempData>();


            List<MainSummaryReport.RegionAndStatusWiseCount> listRegionAndStatusWiseCount = new List<MainSummaryReport.RegionAndStatusWiseCount>();
            List<MainSummaryReport.RegionAndStatusWiseCountTempData> listRegionAndStatusWiseCountTemp = null;
            string resolvingOfficerName = null;
            MainSummaryReport.RegionAndStatusWiseCount regionAndStatusWiseCount = null;
            MainSummaryReport.RegionAndStatusWiseCount cumulativeStatusWiseCount = null;

            

            int i = 1;
            var regionAndStatusWiseList = listRegionAndStatusWiseCountTempData.GroupBy(n => n.Hierarchy1Id);

            cumulativeStatusWiseCount = new MainSummaryReport.RegionAndStatusWiseCount();

            foreach (var regionStatusGroup in regionAndStatusWiseList)
            {
                listRegionAndStatusWiseCountTemp = listRegionAndStatusWiseCountTempData.Where(n => n.Hierarchy1Id == regionStatusGroup.Key).ToList();

                regionAndStatusWiseCount = new MainSummaryReport.RegionAndStatusWiseCount();

                foreach (MainSummaryReport.RegionAndStatusWiseCountTempData regionStatusWiseCount in listRegionAndStatusWiseCountTemp)
                {
                    regionAndStatusWiseCount.Hierarchy1Data = dictRegion[regionStatusWiseCount.Hierarchy1Id];
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingFresh ||
                        regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
                    {
                        regionAndStatusWiseCount.Opened = regionAndStatusWiseCount.Opened + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.PendingReopened)
                    {
                        regionAndStatusWiseCount.Reopened = regionAndStatusWiseCount.Reopened + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ResolvedUnverified ||
                        regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ResolvedVerified)
                    {
                        regionAndStatusWiseCount.Resolved = regionAndStatusWiseCount.Resolved + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.ClosedVerified)
                    {
                        regionAndStatusWiseCount.Closed = regionAndStatusWiseCount.Closed + regionStatusWiseCount.Total;
                    }
                    if (regionStatusWiseCount.StatusId == (int)Config.ComplaintStatus.UnsatisfactoryClosed)
                    {
                        regionAndStatusWiseCount.Overdue = regionAndStatusWiseCount.Overdue + regionStatusWiseCount.Total;
                    }

                }
                regionAndStatusWiseCount._percentage_closed = (double)regionAndStatusWiseCount.Closed / (regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Reopened);
                cumulativeStatusWiseCount.Opened = cumulativeStatusWiseCount.Opened + regionAndStatusWiseCount.Opened;
                cumulativeStatusWiseCount.Resolved = cumulativeStatusWiseCount.Resolved + regionAndStatusWiseCount.Resolved;
                cumulativeStatusWiseCount.Closed = cumulativeStatusWiseCount.Closed + regionAndStatusWiseCount.Closed;
                cumulativeStatusWiseCount.Overdue = cumulativeStatusWiseCount.Overdue + regionAndStatusWiseCount.Overdue;
                cumulativeStatusWiseCount.Reopened = cumulativeStatusWiseCount.Reopened + regionAndStatusWiseCount.Reopened;
                cumulativeStatusWiseCount._percentage_closed = (double)cumulativeStatusWiseCount.Closed / (cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Reopened);

                regionAndStatusWiseCount.Total = regionAndStatusWiseCount.Total + regionAndStatusWiseCount.Opened + regionAndStatusWiseCount.Resolved +
                                                     regionAndStatusWiseCount.Closed + regionAndStatusWiseCount.Overdue;

                regionAndStatusWiseCount.SrNo = null;
                listRegionAndStatusWiseCount.Add(regionAndStatusWiseCount);
                i++;
            }
            cumulativeStatusWiseCount.Hierarchy1Data = "Total";
            cumulativeStatusWiseCount.Total = cumulativeStatusWiseCount.Opened + cumulativeStatusWiseCount.Resolved +
                                              cumulativeStatusWiseCount.Closed + cumulativeStatusWiseCount.Overdue;
            cumulativeStatusWiseCount.SrNo = null;
            //listRegionAndStatusWiseCount.Add(cumulativeStatusWiseCount);

            //for (int i = 0; i < listRegionAndStatusWiseCount.Count; i++)
            //{
            //    listRegionAndStatusWiseCount[i].SrNo = (i + 1).ToString();
            //    //listOverdueComplaintsGroupedBy[i].SrNo = i.ToString();
            //}

            return listRegionAndStatusWiseCount.OrderByDescending(x => x.Overdue).ToList<MainSummaryReport.RegionAndStatusWiseCount>();
        }

        public static VmStatusWiseComplaintsData GetTertiaryCategoryWiseData(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string graphTag)
        {
            Config.SummaryReportType rType = (Config.SummaryReportType)reportType;
            int campaignId = Utility.GetIntByCommaSepStr(campId);
            Config.CategoryType categoryType = Config.CategoryType.Tertiary;

            List<DbStatus> listDbStatuses = DbStatus.GetByStatusIds(Utility.GetIntList(statusIds));
            List<DbComplaintType> listCat = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, null);
            List<DbComplaintSubType> listSubCat = DbComplaintSubType.GetAllSubTypesByDepartmentAndGroup(campaignId,
                null);
            List<DbUserCategory> listDbUserCat = null;

            if (rType == Config.SummaryReportType.General) // general report
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                listDbUserCat = cmsCookie.ListDbUserCategory;
                //return GetCategoryWiseDashboardData(hierarchyId, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId, listDbStatuses, Config.CategoryType.Tertiary,listCat, listSubCat);
            }


            DbUsers dbUser = null;
            List<DbPermissionsAssignment> listDbPermissionsAssignment = null;
            //List<DbStatus> listDbStatuses = null;
            string userStatuses = null;
            string categoryIds = "";
            DataTableParamsModel dtParams = null;
            ListingParamsModelBase paramsSchoolEducation = null;

            string queryStr = "";
            DataSet ds;
            VmStatusWiseComplaintsData statusWiseComplaintData;
            List<VmUserWiseStatus> listVmUserWiseStatus;
            int? groupId = null;


            userStatuses = Utility.GetCommaSepStrFromList(listDbStatuses.Select(n => n.Complaint_Status_Id).ToList()); //string.Join(",", listDbStatuses.Select(n => n.Complaint_Status_Id).ToArray());
            categoryIds = Utility.GetCommaSepStrFromList(listSubCat.Select(n => n.Complaint_SubCategory).ToList());



            listVmUserWiseStatus = new List<VmUserWiseStatus>();

            DbComplaintType dbComplaintType = null;
            string strToShow = "";
            foreach (DbComplaintSubType dbComplaintSubType in listSubCat)
            {
                dbComplaintType =
                    listCat.Where(n => n.Complaint_Category == dbComplaintSubType.Complaint_Type_Id)
                        .FirstOrDefault();
                strToShow = dbComplaintType.Name.Trim() + " : " + dbComplaintSubType.Name.Trim();
                listVmUserWiseStatus.Add(MakeEmptyStatusModel(new KeyValuePair<int, string>(dbComplaintSubType.Complaint_SubCategory, strToShow), listDbStatuses));
            }
            string selectionFields = "";
            string groupByFields = "";
            string whereLogic = "";

            if (categoryType == Config.CategoryType.Main)
            {
                selectionFields = "complaints.Department_Id AS CatId,complaints.Department_Name AS CatName";
                groupByFields =
                    "complaints.Department_Id,complaints.Department_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

                whereLogic = " and  EXISTS(SELECT 1 FROM dbo.SplitString('" + categoryIds +
                             "',',') X WHERE X.Item=complaints.Department_Id)";
            }
            else if (categoryType == Config.CategoryType.Sub)
            {
                selectionFields = "complaints.Complaint_Category AS CatId,complaints.Complaint_Category_Name AS CatName";
                groupByFields =
                    "complaints.Complaint_Category,complaints.Complaint_Category_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";
                whereLogic = "  and EXISTS(SELECT 1 FROM dbo.SplitString('" + categoryIds +
                             "',',') X WHERE X.Item=complaints.Complaint_Category)";
            }
            else if (categoryType == Config.CategoryType.Tertiary)
            {
                selectionFields = "complaints.Complaint_SubCategory AS CatId,complaints.Complaint_SubCategory_Name AS CatName";
                groupByFields =
                    "complaints.Complaint_SubCategory,complaints.Complaint_SubCategory_Name, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status";

                whereLogic = " and EXISTS(SELECT 1 FROM dbo.SplitString('" + categoryIds +
                             "',',') X WHERE X.Item=complaints.Complaint_SubCategory)";
            }




            paramsSchoolEducation = SetParamsDynamicQuery(hierarchyId, userHierarchyId, commaSepVal, listDbUserCat, startDate, endDate, campaignId.ToString(), userStatuses,
                "1,0", dtParams, Config.ComplaintType.Complaint,
                Config.StakeholderComplaintListingType.UptilMyHierarchy, "CategoryWiseStatusComplaintCounts", selectionFields, "", groupByFields, whereLogic);

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





    }
}