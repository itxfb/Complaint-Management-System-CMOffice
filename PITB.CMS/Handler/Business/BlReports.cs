using System.Data;
using System.Dynamic;
using System.Security.Cryptography.Xml;
using AngleSharp.Extensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PITB.CMS.Handler.Configuration;
using PITB.CMS.Helper;
using PITB.CMS.Models.Custom.HtmlTable;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.Custom;
using PITB.CMS.Handler.Authentication;
using System.Collections.Generic;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.View.Reports;
using System.Linq;
using System;
using Newtonsoft.Json;
using PITB.CMS.Models.Custom.CustomForm;
namespace PITB.CMS.Handler.Business
{
    public class BlReports
    {
        public static DataTable GetAgentWiseCount(string from, string to, string agentIds, string campaignIds)
        {
            return DbComplaint.GetAgentListingReport(from, to, agentIds, campaignIds);
        }

        public static VmStakeholderPieChart PieLegentChartProgress(string datelow, string datemax, int campaignId,
            Config.UserWiseGraphType status)
        {
            if (campaignId == (int) Config.Campaign.SchoolEducationEnhanced)
            {
                return GetDataPieLegentChartProgress(datelow, datemax, campaignId, status);
            }
            else //if (campaignId == (int) Config.Campaign.ZimmedarShehri)
            {
                return BlCommonReport.GetDataPieLegentChartProgress(datelow, datemax, campaignId, status);
            }
            return null;
        }

        public static VmStakeholderPieChart GetDataPieLegentChartProgress(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status)
        {
            VmStakeholderPieChart totalCount = new VmStakeholderPieChart();

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(cookie.UserId, datelow, datemax, status);
            if (statusWiseData != null)
            {
                for (int i = 0; i < statusWiseData.ListUserWiseData.Count; i++)
                {
                    for (int j = 0; j < statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount.Count; j++)
                    {
                        singletemp = new VmStatusWiseCount();
                        singletemp.name = statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount[j].StatusId +"-"+ statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount[j].StatusString;
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
        public static VmComplaintTypeWisePieChart PieLegentChartCallVolume(string datelow, string datemax, int campaignId)
        {
            VmComplaintTypeWisePieChart BlockByBlockCount = new VmComplaintTypeWisePieChart();
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            BlockByBlockCount.ListComplaintTypeWiseCount = BlSchool.GetComplaintsTypeWiseTotalCount(cookie.UserId, datelow, datemax, campaignId);
            BlockByBlockCount.HierarchyId = cookie.User_Hierarchy_Id.ToString();
            return BlockByBlockCount;
        }

        public static VmComplaintTypeWisePieChart PieLegentChartCompliantsByCategory(string datelow, string datemax, int campaignId)
        {
            VmComplaintTypeWisePieChart BlockByBlockCount = new VmComplaintTypeWisePieChart();
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            BlockByBlockCount.ListComplaintTypeWiseCount = BlSchool.GetComplaintsCategoryWiseTotalCount(cookie.UserId, datelow, datemax, campaignId);
            BlockByBlockCount.HierarchyId = cookie.User_Hierarchy_Id.ToString();
            return BlockByBlockCount;
        }
        public static VmStakeholderPieChart PieLegentChartProgressFromTeachingQualitySchoolEducation(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status)
        {
            VmStakeholderPieChart totalCount = new VmStakeholderPieChart();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchool.GetTeachingQualityDataForSchoolEducationDashboard(cookie.UserId, datelow, datemax, status);
            if (statusWiseData != null)
            {
                for (int i = 0; i < statusWiseData.ListUserWiseData.Count; i++)
                {
                    for (int j = 0; j < statusWiseData.ListUserWiseData[i].ListVmStatusWiseCount.Count; j++)
                    {
                        VmStatusWiseCount singletemp = new VmStatusWiseCount();
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

        public static VmStatusWiseComplaintsData BarChartUserWise(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status, int userId)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(userId, datelow, datemax, status);

            return statusWiseData;
        }

        public static VmStatusWiseComplaintsData BarChartAgingReport(string dateFirst, string dateSecond, int campaignId, Config.AgingReportType agingReportType)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchool.GetAgingReportData(dateFirst, dateSecond, campaignId, agingReportType);

            return statusWiseData;
        }

        public static VmStatusWiseComplaintsData BarChartUserWiseTeachingQuality(string datelow, string datemax, int campaignId, Config.UserWiseGraphType status, int userId, int hierarchyId)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchool.GetTeachingQualityDataForSchoolEducationDashboard(userId, datelow, datemax, status, hierarchyId);

            return statusWiseData;
        }

        public static VmStatusWiseComplaintsData BarChartCategoryWiseDashboardData(string datelow, string datemax, int campaignId, Config.CategoryType complaintType, int categoryId)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlComplaints.GetCategoryWiseDashboardData(cookie.UserId, datelow, datemax, campaignId, complaintType, categoryId);

            return statusWiseData;
        }

        
        public static DataTable DistrictWiseGraph(string datelow, string datemax, int campaignId, int StatusId)
        {

            Dictionary<string, object> paramz = new Dictionary<string, object>
            {
                {"@Start_Date", datelow},
                {"@End_Date", datemax},
                {"@Campaign_Id", campaignId},
                {"@Status_Value", StatusId}
            };


            return DBHelper.GetDataTableByStoredProcedure("[PITB].[Get_Status_By_Districts]", paramz);
        }
        public static List<VmHealthDistrictWiseReport> GetHierarchyWiseReportData(int hierarchyId, string campaignIds, string statusIds, DateTime startDate, DateTime endDate)
        {
            VmHierarchyWiseReport vModel = new VmHierarchyWiseReport();
            string hierarchyName =  Utility.GetHierachyNameByHierarchyId(hierarchyId);
            List<DbStatus> statuses = DbStatus.GetByStatusIds(Utility.GetIntList(statusIds));
            Dictionary<string, string> statusesName = new Dictionary<string, string>();
            foreach (var status in statuses)
            {
                var statusName = status.Status.Replace(' ', '_').Replace('(','_').Replace(')','_');
                if (status.Complaint_Status_Id == (int)Config.ComplaintStatus.Resolved)
                {
                    statusesName.Add("4,8,11,17,20", statusName);
                }
                else
                {
                    statusesName.Add(status.Complaint_Status_Id.ToString(), statusName);
                }
                
            }
            string campaignName = null;
            
            string hierarchyColName = Utility.GetHierarchyColumnNameByHierarchyIdComplaintsTable(hierarchyId);

            string selectClause = "SELECT ROW_NUMBER() OVER (ORDER BY " + hierarchyColName + ") AS 'SrNo'," + hierarchyColName.Split(new char[] { ',' }).Last() + " ";
            string whereClause = "";
            string groupByClause = "";
            string orderByClause = "";

            for (int i = 0; i < statusesName.Count; i++)
            {
                selectClause += " ,COUNT(CASE WHEN Complaint_Computed_Status_Id IN( " + statusesName.Keys.ElementAt(i) + ") THEN 1 END) AS '" + statusesName.Values.ElementAt(i) + "' ";
            }
            selectClause += " ,COUNT(CASE WHEN Complaint_Computed_Status_Id IN(" + string.Join(",",statusesName.Keys) + ") THEN 1 END) AS 'Total' ";
            selectClause += " FROM PITB.Complaints ";
            whereClause += " WHERE Compaign_Id IN("+campaignIds+") AND Complaint_Type = 1 AND CONVERT(DATE,Created_Date) >= CONVERT(DATE,'"+startDate.ToShortDateString()+"') AND CONVERT(DATE,Created_Date) <= CONVERT(DATE,'"+endDate.ToShortDateString()+"')";
            groupByClause += " GROUP BY "+hierarchyColName+"";
            string query = string.Concat(selectClause, whereClause, groupByClause);
            DataTable dt = DBHelper.GetDataTableByQueryString(query, null);
            List<VmHealthDistrictWiseReport> vmHealth = new List<VmHealthDistrictWiseReport>();
 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                VmHealthDistrictWiseReport vm = new VmHealthDistrictWiseReport();
                vm.SrNo = dt.Rows[i].Field<Int64>("SrNo").ToString();
                vm.DistrictName = dt.Rows[i].Field<string>("District_Name");
                vm.Overdue = dt.Rows[i].Field<Int32>("Overdue").ToString();
                vm.PendingFresh = dt.Rows[i].Field<Int32>("Pending__Fresh_").ToString();
                vm.PendingReopened = dt.Rows[i].Field<Int32>("Pending__Reopened_").ToString();
                vm.Resolved = dt.Rows[i].Field<Int32>("Resolved").ToString();
                vm.Total = dt.Rows[i].Field<Int32>("Total").ToString();
                vmHealth.Add(vm);
            }
            string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            vModel.DataInJson = json;
            vModel.CampaignIds = campaignIds;
            vModel.CampaignName = campaignName;
            vModel.HierarchyId = hierarchyId;
            vModel.HierarchyName = hierarchyName;
            vModel.StatusIds = statusIds;
            vModel.StatusesName = statusesName;
            vModel.data = dt;
            return vmHealth;
        }

        public static dynamic ParamsForRegionWiseResolvedComplaintReport(CustomForm.Post postForm)
        {
            ControllerModel.Response resp = new ControllerModel.Response();
            
            List<string> listColumns = new List<string>();
            int hierarchyId = int.Parse(Utility.GetKey(postForm.GetElementValue("Region")))+1;
            string[] hierarchyTableConfig = Config.HiertarchyTableConfigDict[hierarchyId].Split(',');
            int campaignId = int.Parse(Utility.GetKey(postForm.GetElementValue("Campaign")));
            int categoryLevel = int.Parse(Utility.GetKey(postForm.GetElementValue("CategoryLevel")));
            string categories = postForm.GetElementValue("Categories");
            string categoriesCheck = "";
            if (!string.IsNullOrEmpty(categories))
            {
                List<string> listCategories =
                    JsonConvert.DeserializeObject<List<string>>(postForm.GetElementValue("Categories"));
                if (categoryLevel != -1 && listCategories != null && listCategories.Count > 0)
                {
                    if (categoryLevel == 1) categoriesCheck = "and Department_Id in (";
                    else if (categoryLevel == 2) categoriesCheck = "and Complaint_Category in (";
                    else if (categoryLevel == 3) categoriesCheck = "and Complaint_SubCategory in (";

                    foreach (string cat in listCategories)
                    {
                        categoriesCheck += cat.Split(new string[] { Config.Separator }, StringSplitOptions.None)[0] + ",";
                    }
                    categoriesCheck = categoriesCheck.Remove(categoriesCheck.Length - 1);
                    categoriesCheck += ")";
                }
            }
            //DateTime fromDt = DateTime.Parse();
            //DateTime toDt = DateTime.Parse(postForm.GetElementValue("To_Date"));
            //DateTime campaign = DateTime.Parse(postForm.GetElementValue("Campaign"));
            //DateTime region = DateTime.Parse(postForm.GetElementValue("Region"));

            DateTime fromDate = Utility.GetDateTime(postForm.GetElementValue("From_Date"));
            DateTime toDate = Utility.GetDateTime(postForm.GetElementValue("To_Date"));
            List<DateTime> listMothDifference = Utility.GetDateDifference(fromDate, toDate, "month");


            Dictionary<string, object> dictParams = new Dictionary<string, object>();
            dictParams.Add("@FromDate", Utility.GetDateTimeStr(postForm.GetElementValue("From_Date")));
            dictParams.Add("@ToDate", Utility.GetDateTimeStr(postForm.GetElementValue("To_Date")));
            dictParams.Add("@CampaignId", Utility.GetKey(postForm.GetElementValue("Campaign")));
            dictParams.Add("@HierarchyId", hierarchyId);
            dictParams.Add("@ComplaintRegionIdColName", DbComplaint.DictHierarchyWiseColumnName[hierarchyId].Split(',')[0]);
            dictParams.Add("@HTableName", hierarchyTableConfig[0]);
            dictParams.Add("@Hid", hierarchyTableConfig[1]);
            dictParams.Add("@Hname", hierarchyTableConfig[2]);
            dictParams.Add("@RegionSelect", DbComplaint.DictHierarchyWiseColumnName[hierarchyId]);
            dictParams.Add("@CategoiesCheck", categoriesCheck); 
            //dictParams.Add("@FromDate", fromDt.ToString(SqlDateFormat));

            string sql = QueryHelper.GetFinalQuery("RegionWiseResolvedComplaint", Config.ConfigType.Query, dictParams);
            DataTable dt = DBHelper.GetDataTableByQueryString(sql,null);
            List<dynamic> listDynamic = dt.ToDynamicList();


            Dictionary<string, string> dictSetting = new Dictionary<string, string>();
            dictSetting.Add("HtmlHead", "<thead class='tableHeader' style='text-align:center'>");
            dictSetting.Add("HtmlHeadEnd", "</thead>");
            dictSetting.Add("HtmlBody", "<tbody>");
            dictSetting.Add("HtmlBodyEnd", "</tbody>");
            dictSetting.Add("HtmlFooter", "<tfoot class='tableHeader' style='text-align:right'>");
            dictSetting.Add("HtmlFooterEnd", "</tfoot>");
            dictSetting.Add("HtmlRow", "<tr>");
            dictSetting.Add("HtmlColTh", "<th>");
            dictSetting.Add("HtmlColTd", "<td style='text-align:left'>");
            HtmlTableModel tableModel = new HtmlTableModel(dictSetting);

            tableModel.NewRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Chief Minister's Complaint Center(0800-02345)",
                //ColumnId = "None",
                //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                DictAttributes = new Dictionary<string, object>
                {
                    //{"style","min-width:150"},
                    {"colspan",""+((listMothDifference.Count*2) +5)}
                },
                ClassName = "SelectAll",
                Type = 1,
                BeforeRowHtml = tableModel.DictSetting["HtmlHead"]
                //PreviousHtml = "<thead>"
                //ColSpan = (listMothDifference.Count*2)
            });

            tableModel.NewRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Sr.No",
                ColumnId = "SrNo",
                //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                DictAttributes = new Dictionary<string, object>
                {
                    //{"style","min-width:150"},
                    {"rowspan",2},
                    //{"colspan",2}
                },
                Type = 1,
                RowSpan = 3
                //DictAttributes = new Dictionary<string, string> { {"rowspan","3"} }
            });

            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Region Name",
                ColumnId = "RegionId",
                DictAttributes = new Dictionary<string, object>
                    {
                        //{"style","min-width:150"},
                        {"rowspan",2},
                        //{"colspan",2}
                    },
                //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                Type = 1,
                //RowSpan = 3
                //DictAttributes = new Dictionary<string, string> { { "rowspan", "3" } }
            });

            for (int i = 0; i < listMothDifference.Count; i++)
            {
                tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
                {
                    InnerHtml = listMothDifference[i].ToString("MMM-yyyy"),
                    //ColumnId = listMothDifference[i].ToString("MMM-yyyy"),
                    //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                    DictAttributes = new Dictionary<string, object>
                    {
                        //{"style","min-width:100"},
                        {"colspan",2}
                        
                    },
                    Type = 1
                    //ColSpan = 3
                    //DictAttributes = new Dictionary<string, string> { {"rowspan","3"} }
                });
            }

            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Total",
                //ColumnId = "Total",
                DictAttributes = new Dictionary<string, object>
                {
                    //{"style","min-width:100"},
                    {"colspan",2}
                        
                },
                Type = 1
            });

            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Resolution",
                //ColumnId = "Total",
                //DictAttributes = new Dictionary<string, object>
                //{
                //    //{"style","min-width:100"},
                //    {"colspan",2}
                        
                //},
                Type = 1
            });



            for (int i = 0; i < listMothDifference.Count; i++)
            {
                if (i == 0)
                {
                     tableModel.SetNewRow();
                }
                
                tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
                {
                    InnerHtml = "Complaints",
                    ColumnId = listMothDifference[i].ToString("MMM-yyyy") + "_" + "Complaints",
                    //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                    //DictAttributes = new Dictionary<string, object>
                    //{
                    //    {"style","min-width:50"},
                    //},
                    Type = 1,
                    //ColSpan = 3
                    //DictAttributes = new Dictionary<string, string> { {"rowspan","3"} }
                });

                tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
                {
                    InnerHtml = "Resolved",
                    ColumnId = listMothDifference[i].ToString("MMM-yyyy") + "_" + "Resolved",
                    //Css = new HtmlTableModel.CssProperties() { MinWidth = "150px" },
                    //DictAttributes = new Dictionary<string, object>
                    //{
                    //    {"style","min-width:50"}
                    //},
                    Type = 1,
                    //NextHtml = "</thead>",
                    //AfterRowHtml = Utility.GetEndingTag(tableModel.DictSetting["HtmlHead"]) 
                    //ColSpan = 3
                    //DictAttributes = new Dictionary<string, string> { {"rowspan","3"} }
                });
            }
            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Complaints",
                ColumnId = "Total" + "_" + "Complaints",
                Type = 1,
            });
            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "Resolved",
                ColumnId = "Total" + "_" + "Resolved",
                Type = 1,
                //AfterRowHtml = Utility.GetEndingTag(tableModel.DictSetting["HtmlHead"]) 
            });
            tableModel.CurrRow.Add(new HtmlTableModel.CellModel()
            {
                InnerHtml = "%",
                ColumnId = "%",
                Type = 1,
                AfterRowHtml = Utility.GetEndingTag(tableModel.DictSetting["HtmlHead"])
            });


            // Start populating data
            tableModel.SetColumns();

            List<DbPermissionsAssignment> listCampStatusMergePermission = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.Campaign, Utility.GetNullableIntList(new List<int>(){campaignId}), Config.CampaignPermissions.ExecutiveCampaignStatusReMap);

            var permissionAssign = listCampStatusMergePermission.Where(n => n.Type_Id == campaignId).FirstOrDefault();
            Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(permissionAssign.Permission_Value);
            List<int?> listStatus = Utility.ConvertStringListToNullableIntList(listStatusMergePermission["8"].Split(',').ToList());

            var regionGroup = listDynamic.GroupBy(n => new {n.Id, n.Name1/*, n.Month_Year*/}).ToList();
            int c = 1;
            List<dynamic> listBodyCells = new List<dynamic>();
            foreach (var group in regionGroup)
            {
                //c = 1;
                dynamic d = new ExpandoObject();
                IDictionary<string, object> dynamicObject = d;
                dynamicObject["SrNo"] = c;
                dynamicObject["RegionId"] = group.Key.Name1;
                int totalResolved = 0, totalComplaints = 0, sum=0;
                for (int i = 0; i < listMothDifference.Count; i++)
                {
                    string currDt = listMothDifference[i].ToString("MMM-yyyy");
                    List<dynamic> listTemp =
                        listDynamic.Where(
                                n => n.Id == group.Key.Id && n.Name1 == group.Key.Name1 && (n.Month_Year==null || n.Month_Year == currDt))
                            .ToList();

                    sum = listTemp.Where(n=>n.Count!=null).Sum(n => n.Count);
                    totalComplaints += sum;
                    dynamicObject[currDt + "_Complaints"] = sum;

                    sum = listTemp.Where(n => listStatus.Contains(n.Complaint_Computed_Status_Id)).Sum(n => n.Count);
                    totalResolved += sum;
                    dynamicObject[currDt + "_Resolved"] = sum;
                }
                dynamicObject["Total" + "_" + "Complaints"] = totalComplaints;
                dynamicObject["Total" + "_" + "Resolved"] = totalResolved;
                dynamicObject["%"] = (((float)totalResolved/totalComplaints) *100f).ToString("0.0") +"%";
                listBodyCells.Add(dynamicObject);
                //group.Key.Id;
                c++;
            }

            Dictionary<string, HtmlTableModel.CellModel> dictCellTemplate = new Dictionary<string, HtmlTableModel.CellModel>()
            {

                {"*", new HtmlTableModel.CellModel(){
                    DictSetting = new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" }, }
                }}
                 
            };

            //HtmlTableModel.CellModel cellTemplate = new HtmlTableModel.CellModel()
            //{
            //    DictSetting = new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" } }
            //};
            tableModel.BindHtmlCell(listBodyCells, dictCellTemplate);
            tableModel.AddTableBody2(listBodyCells, tableModel.ListColumns,2, null);


            //tableModel.AddTableBody(listBodyCells, tableModel.ListColumns, new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" } });
            

             // Adding footer
            //tableModel.NewRow.Add(new HtmlTableModel.CellModel()
            //{
            //    InnerHtml = "Total",
            //    ColumnId = "%",
            //    Type = 3,
            //    DictAttributes = new Dictionary<string, object>
            //    {
            //        {"colspan",2}      
            //    },
            //    BeforeRowHtml = Utility.GetEndingTag(tableModel.DictSetting["HtmlFooter"])
            //});
            
            List<dynamic> listFooterCells = new List<dynamic>();
            dynamic dTemp = new ExpandoObject();
            IDictionary<string, object> dictTemp = dTemp;
            dictCellTemplate = new Dictionary<string, HtmlTableModel.CellModel>()
            {
                {"SrNo", new HtmlTableModel.CellModel(){
                    DictSetting = new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" }, },
                    DictAttributes = new Dictionary<string, object>
                    {
                        {"colspan",2}
                    },
                }},

                {"*", new HtmlTableModel.CellModel(){
                    DictSetting = new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" }, }
                }}
                 
            };
            dictTemp["SrNo"] = "Total";
            //tableModel.BindHtmlCell(dTemp, "SrNo", cellTemplate);
            //tableModel.AddTableBody2(listFooterCells, tableModel.ListColumns, -1, null);
            //listFooterCells.Clear();
            //dictTemp.Clear();
            int totalC=0, resolvedC=0;
            for (int j = 2; j < tableModel.ListColumns.Count-1; j++)
            {
                
               dictTemp[tableModel.ListColumns[j]] = listBodyCells.Select(n => n).Cast<IDictionary<string, object>>().ToList().Sum(n => (int)n[tableModel.ListColumns[j]]);
               if (j == tableModel.ListColumns.Count - 2)
               {
                   resolvedC = (int)dictTemp[tableModel.ListColumns[j]];
               }
               else if (j == tableModel.ListColumns.Count - 3)
               {
                   totalC = (int)dictTemp[tableModel.ListColumns[j]];
               }
               
            }

            dictTemp["%"] = (((float)resolvedC / totalC) * 100f).ToString("0.0") + "%";
            listFooterCells.Add(dictTemp);


            //cellTemplate = new HtmlTableModel.CellModel()
            //{
            //    DictSetting = new Dictionary<string, string>() { { "HtmlColTd", "<td style='text-align:right'>" }, },
            //};
            tableModel.BindHtmlCell(listFooterCells, dictCellTemplate);
            tableModel.AddTableBody2(listFooterCells, tableModel.ListColumns, 3, null);
            //tableModel.AddTableBody(listFooterCells, tableModel.ListColumns, null);
            

            // End populating data



            //string json = JsonConvert.SerializeObject(htmlTableModel);


            //ConfigurationHandler.GetConfiguration("RegionWiseResolvedComplaint_Campaigns", Config.ConfigType.Config);

            //json = json.Replace("[", "{ \"data\": [");
            //json = json.Replace("]", "] }");

            //return json;
            tableModel.ConvertTableToHtml();
            dynamic data = tableModel;
            return data;
            //string json = JsonConvert.SerializeObject(data);
            //return json;
        }

        
       
    }
}