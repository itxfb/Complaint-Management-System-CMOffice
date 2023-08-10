using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CRM_API.Models.API;
using PITB.CRM_API.Helper.Database;
using PITB.CRM_API.Models.DB;
using System.Data;
using PITB.CRM_API.Models.API.Datapack;
using System.Diagnostics;

namespace PITB.CRM_API.Handlers.Business.Datapack
{
    public class BlDatapack
    {
        public static List<ComplaintsModel> GetMasterComplaintData(DateTime startDate, DateTime endDate)
        {
            List<ComplaintsModel> lstResponse = null;
            try
            {
                string queryStr = "SELECT c.Id,c.Department_Name As 'Department_Name',c.Complaint_Category_Name As 'TypeName'," + " " +
                "c.Complaint_SubCategory_Name As 'SubTypeName',c.District_Name,c.Tehsil_Name,c.UnionCouncil_Name As 'Markaz_Name'," + " " +
                "sMap.school_emis_code, sMap.school_name,sMap.school_level,sMap.school_gender,complaintsAssignee.Assignee AS 'Assignee'," + " " +
                "c.Created_Date As 'CreatedDate',CONVERT(VARCHAR(10), c.Created_Date, 101) As 'Created_Date',Month(convert(datetime,c.Created_Date,101)) AS Month," + " " +
                "DATENAME(month, convert(datetime,c.Created_Date,101)) AS 'Month_Name',c.StatusChangedDate_Time,c.Computed_Overdue_Days," + " " +
                "c.Complaint_Computed_Status from PITB.Complaints As c INNER JOIN (SELECT a.Complaint_Id, Assignee = " + " " +
                "(SELECT (SELECT  RTRIM(LTRIM(CAST(users.Name as nvarchar(max))))+':'+RTRIM(LTRIM(users.Designation_abbr))+',  ')" + " " +
                "FROM PITB.User_Wise_Complaints b INNER JOIN PITB.Users users ON  users.id = b.User_Id" + " " +
                "WHERE  b.Complaint_Subtype = 1 AND b.Complaint_Id = a.Complaint_Id" + " " +
                "FOR XML PATH(''))," + " " +
                "AssigneePhoneNo = (SELECT (SELECT  RTRIM(LTRIM(CAST(users.Phone as nvarchar(max)))) +',  ')" + " " +
                "FROM PITB.User_Wise_Complaints b INNER JOIN PITB.Users users ON  users.id = b.User_Id" + " " +
                "WHERE  b.Complaint_Subtype = 1 AND b.Complaint_Id = a.Complaint_Id" + " " +
                "FOR XML PATH(''))" + " " +
                "FROM PITB.User_Wise_Complaints  a" + " " +
                "WHERE a.Complaint_Subtype = 1 " + " " +
                "GROUP BY a.Complaint_Id) complaintsAssignee ON complaintsAssignee.Complaint_Id = c.Id" + " " +
                "LEFT JOIN dbo.Schools_Mapping sMap ON sMap.school_emis_code = c.RefField1 " + " " +
                "WHERE Compaign_Id = 47 AND Complaint_Category NOT IN(324,333) AND Created_Date >= '" + startDate.ToString("MM/dd/yyyy") + "' AND Created_Date <= '" + endDate.ToString("MM/dd/yyyy") + "'";
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    lstResponse = new List<ComplaintsModel>();
                    foreach (DataRow row in lObjData.Rows)
                    {
                        ComplaintsModel model = new ComplaintsModel();
                        model.Id = (int)row["Id"];
                        model.DepartmentName = (row["Department_Name"] == DBNull.Value) ? null : row["Department_Name"].ToString().TrimEndLine();
                        model.TypeName = (row["TypeName"] == DBNull.Value) ? null : row["TypeName"].ToString().TrimEndLine();
                        model.SubTypeName = (row["SubTypeName"] == DBNull.Value) ? null : row["SubTypeName"].ToString().TrimEndLine();
                        model.DistrictName = (row["District_Name"] == DBNull.Value) ? null : row["District_Name"].ToString().TrimEndLine();
                        model.TehsilName = (row["Tehsil_Name"] == DBNull.Value) ? null : row["Tehsil_Name"].ToString().TrimEndLine();
                        model.MarkazName = (row["Markaz_Name"] == DBNull.Value) ? null : row["Markaz_Name"].ToString().TrimEndLine();
                        model.SchoolEmisCode = (row["school_emis_code"] == DBNull.Value) ? null : row["school_emis_code"].ToString().TrimEndLine();
                        model.SchoolGender = (row["school_gender"] == DBNull.Value) ? null : row["school_gender"].ToString().TrimEndLine();
                        model.SchoolLevel = (row["school_level"] == DBNull.Value) ? null : row["school_level"].ToString().TrimEndLine();
                        model.SchoolName = (row["school_name"] == DBNull.Value) ? null : row["school_name"].ToString().TrimEndLine();
                        model.Assignee = (row["Assignee"] == DBNull.Value) ? null : row["Assignee"].ToString().TrimEndLine();
                        model.CreatedDate = (row["CreatedDate"] == DBNull.Value) ? null : row["CreatedDate"].ToString().TrimEndLine();
                        model.Created_Date = (row["Created_Date"] == DBNull.Value) ? null : row["Created_Date"].ToString().TrimEndLine();
                        model.Month = (row["Month"] == DBNull.Value) ? 0 : (int)row["Month"];
                        model.MonthName = (row["Month_Name"] == DBNull.Value) ? null : row["Month_Name"].ToString().TrimEndLine();
                        model.StatusChangeDateTime = (row["StatusChangedDate_Time"] == DBNull.Value) ? null : row["StatusChangedDate_Time"].ToString().TrimEndLine();
                        model.ComplaintComputedStatus = (row["Complaint_Computed_Status"] == DBNull.Value) ? null : row["Complaint_Computed_Status"].ToString().TrimEndLine();
                        model.OverdueDays = (row["Computed_Overdue_Days"] == DBNull.Value) ? 0 : (int)row["Computed_Overdue_Days"];
                        lstResponse.Add(model);
                    }
                }    
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstResponse;
        }
        public static ClosureAndTimelinessModel GetClosureAndTimelinessData(ClosureAndTimelinessParameterModel parameter)
        {
            ClosureAndTimelinessModel model = null;
            try
            {
                string ParameterClause = null;
                string SelectClause = null;
                string WhereClause = null;
                string GroupyByClause = null;
                string OrderByClause = null;
                string AssigneeClause = null;
                List<string> lstSelectClause = new List<string>();
                List<string> lstWhereClause = new List<string>();
                List<string> lstGroupByClause = new List<string>();
                List<string> lstOrderByClause = new List<string>();
                List<string> lstAssigneeClause = new List<string>();
                // District settings

                if (parameter.District.Equals("All", StringComparison.OrdinalIgnoreCase) || parameter.District.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',District_Name AS 'DistrictName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("District_Id");
                    lstGroupByClause.Add("District_Name");
                    lstOrderByClause.Add("District_Name");

                }
                else 
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',District_Name AS 'DistrictName'");
                    lstWhereClause.Add("District_Id IN (" + parameter.District + ")");
                    lstGroupByClause.Add("District_Id");
                    lstGroupByClause.Add("District_Name");
                    lstOrderByClause.Add("District_Name");
                }

                // Tehsil settings

                if (parameter.Tehsil.Equals("All", StringComparison.OrdinalIgnoreCase) || parameter.Tehsil.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 4 AND Crm_Id = c.Tehsil_Id ) As 'TehsilId',Tehsil_Name As 'TehsilName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("Tehsil_Id");
                    lstGroupByClause.Add("Tehsil_Name");
                    lstOrderByClause.Add("Tehsil_Name");

                }
                else
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 4 AND Crm_Id = c.Tehsil_Id ) As 'TehsilId',Tehsil_Name As 'TehsilName'");
                    lstWhereClause.Add("Tehsil_Id IN (" + parameter.Tehsil + ")");
                    lstGroupByClause.Add("Tehsil_Id");
                    lstGroupByClause.Add("Tehsil_Name");
                    lstOrderByClause.Add("Tehsil_Name");
                }

                // Markaz settings

                if (parameter.Markaz.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'MarkazId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 5 AND Crm_Id = c.UnionCouncil_Id ) As 'MarkazId',UnionCouncil_Name As 'MarkazName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("UnionCouncil_Id");
                    lstGroupByClause.Add("UnionCouncil_Name");
                    lstOrderByClause.Add("UnionCouncil_Name");

                }
                else if (parameter.Markaz.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }else 
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'MarkazId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 5 AND Crm_Id = c.UnionCouncil_Id ) As 'MarkazId',UnionCouncil_Name As 'MarkazName'");
                    lstWhereClause.Add("UnionCouncil_Id IN (" + parameter.Markaz + ")");
                    lstGroupByClause.Add("UnionCouncil_Id");
                    lstGroupByClause.Add("UnionCouncil_Name");
                    lstOrderByClause.Add("UnionCouncil_Name");
                }

                // SchoolLevel settings

                if (parameter.SchoolLevel.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstSchoolLevel = ClosureAndTimelinessParameterModel.SchoolLevelAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolLevel.Length; i++)
                    {
                        lstSchoolLevel[i] = "'" + lstSchoolLevel[i] + "'";
                    }
                    lstWhereClause.Add("RefField3 IN (" + string.Join(",", lstSchoolLevel) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstSchoolLevel = parameter.SchoolLevel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolLevel.Length; i++)
                    {
                        lstSchoolLevel[i] = "'" + lstSchoolLevel[i] + "'";
                    }
                    lstWhereClause.Add("RefField3 IN (" + string.Join(",", lstSchoolLevel) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }

                // SchoolGender settings

                if (parameter.SchoolGender.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstSchoolGender = ClosureAndTimelinessParameterModel.SchoolGenderAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolGender.Length; i++)
                    {
                        lstSchoolGender[i] = "'" + lstSchoolGender[i] + "'";
                    }
                    lstWhereClause.Add("RefField5 IN (" + string.Join(",", lstSchoolGender) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstSchoolGender = parameter.SchoolGender.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolGender.Length; i++)
                    {
                        lstSchoolGender[i] = "'" + lstSchoolGender[i] + "'";
                    }
                    lstWhereClause.Add("RefField5 IN (" + string.Join(",", lstSchoolGender) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }


                if (parameter.Assignee.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstAssignee = ClosureAndTimelinessParameterModel.AssigneeAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstAssignee.Length; i++)
                    {
                        lstAssignee[i] = "'" + lstAssignee[i] + "'";
                    }
                    lstAssigneeClause.Add("Designation_abbr IN (" + string.Join(",", lstAssignee) + ")");
                    lstWhereClause.Add("");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstAssignee = parameter.Assignee.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstAssignee.Length; i++)
                    {
                        lstAssignee[i] = "'" + lstAssignee[i] + "'";
                    }
                    lstAssigneeClause.Add("Designation_abbr IN (" + string.Join(",", lstAssignee) + ")");
                    lstWhereClause.Add("");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }

                lstAssigneeClause.RemoveAll(x => x.Equals(String.Empty));
                lstWhereClause.RemoveAll(x => x.Equals(String.Empty));
                lstSelectClause.RemoveAll(x => x.Equals(String.Empty));
                lstGroupByClause.RemoveAll(x => x.Equals(String.Empty));
                lstGroupByClause.RemoveAll(x => x.Equals(String.Empty));
                lstOrderByClause.RemoveAll(x => x.Equals(String.Empty));

                ParameterClause = "DECLARE @startDate VARCHAR(50) = '" + parameter.StartDate + "'; " +
                "DECLARE @endDate VARCHAR(50) = '" + parameter.EndDate + "'; " +
                "DECLARE @ClosedVerified INT;  " +
                "DECLARE @PendingFresh INT;   " +
                "DECLARE @PendingReopened INT;   " +
                "DECLARE @PendingOverdue INT;   " +
                "DECLARE @ResolvedUnverified INT;   " +
                "DECLARE @ResolvedVerified INT;   " +
                "SET @ClosedVerified = 11;  " +
                "SET @PendingFresh = 1;  " +
                "SET @PendingReopened = 7;  " +
                "SET @PendingOverdue = 6;  " +
                "SET @ResolvedUnverified = 2;  " +
                "SET @ResolvedVerified = 3;  ";

                AssigneeClause = string.Join(" AND ", lstAssigneeClause);
                SelectClause = "SELECT " + string.Join(",", lstSelectClause) + ", " +
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @ClosedVerified THEN 1 END) AS 'ClosedVerified',  " + 
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingFresh THEN 1 END) AS 'PendingFresh',    " + 
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingReopened THEN 1 END) AS 'PendingReopened',    " + 
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingOverdue THEN 1 END) AS 'PendingOverdue',    " + 
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @ResolvedUnverified THEN 1 END) AS 'ResolvedUnverified',  " +   
                "COUNT(CASE WHEN Complaint_Computed_Status_Id = @ResolvedVerified THEN 1 END) AS 'ResolvedVerified',    " + 
                "CEILING((COUNT(CASE WHEN Complaint_Computed_Status_Id = @ClosedVerified THEN 1 END)*1.0*100)/NULLIF(COUNT(CASE WHEN Complaint_Computed_Status_Id = @ClosedVerified THEN 1 END) + COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingReopened THEN 1 END),0)) AS 'ClosureRate',   " + 
                "CEILING(100*(1-((COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingOverdue THEN 1 END)*1.0)/NULLIF(COUNT(Complaint_Computed_Status_Id)-COUNT(CASE WHEN Complaint_Computed_Status_Id = @PendingFresh THEN 1 END),0)))) AS 'Timeliness',    " +
                "COUNT(Complaint_Computed_Status_Id) AS 'GrandTotal'    " + 
                "FROM PITB.Complaints AS c ";// +
                //"LEFT JOIN (  " +
                //"SELECT a.Complaint_Id,  " +
                //"Assignee = (SELECT (SELECT  RTRIM(LTRIM(CAST(users.Name as nvarchar(max))))+':'+RTRIM(LTRIM(users.Designation_abbr)) +',  ')  " +
                //"FROM PITB.User_Wise_Complaints b INNER JOIN PITB.Users users ON  users.id = b.User_Id  " +
                //"WHERE  b.Complaint_Subtype = 1 AND b.Complaint_Id = a.Complaint_Id AND " + AssigneeClause + ") " +
                //"FROM PITB.User_Wise_Complaints  a " +
                //"WHERE a.Complaint_Subtype = 1  " +
                //"GROUP BY a.Complaint_Id) complaintsAssignee ON complaintsAssignee.Complaint_Id = c.Id  ";
                WhereClause = " WHERE Compaign_Id = 47 AND Complaint_Type = 1 /*AND CONVERT(DATE,Created_Date) >= CONVERT(DATE,@startDate)*/ AND CONVERT(DATE,Created_Date) <= CONVERT(DATE,@endDate) AND Complaint_Category NOT IN(324,333) AND" +
                " " + string.Join(" AND ", lstWhereClause) + " ";
                GroupyByClause = "GROUP BY Province_Id, " + string.Join(",", lstGroupByClause) + " ";
                OrderByClause = "ORDER BY " + string.Join(",", lstOrderByClause) + " ";

                string queryStr = String.Concat(new string[] { ParameterClause, SelectClause, WhereClause, GroupyByClause, OrderByClause });
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    model = new ClosureAndTimelinessModel();
                    model.RowLabel = "GrandTotal";
                    model.ClosedVerified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("ClosedVerified"));
                    model.PendingReopened = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("PendingReopened"));
                    model.PendingFresh = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("PendingFresh"));
                    model.PendingOverdue = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("PendingOverdue"));
                    model.ResolvedUnverified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("ResolvedUnverified"));
                    model.ResolvedVerified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("ResolvedVerified"));
                    model.GrandTotal = null;
                    model.ClosureRate = null;
                    model.Timeliness = null;
                    if (!parameter.District.Equals("-1"))
                    {
                        model.HierarchyName = "DistrictList";
                        model.HierarchyList = new List<ClosureAndTimelinessModel>();
                        foreach (var district in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("DistrictId")))
                        {
                            ClosureAndTimelinessModel districtModel = new ClosureAndTimelinessModel();
                            districtModel.RowLabel = district.First().Field<string>("DistrictName").ToString().TrimEndLine();
                            districtModel.RowId = district.First().Field<int>("DistrictId").ToString().TrimEndLine();
                            districtModel.ClosedVerified = district.Sum(x => x.Field<int?>("ClosedVerified"));
                            districtModel.PendingReopened = district.Sum(x => x.Field<int?>("PendingReopened"));
                            districtModel.PendingFresh = district.Sum(x => x.Field<int?>("PendingFresh"));
                            districtModel.PendingOverdue = district.Sum(x => x.Field<int?>("PendingOverdue"));
                            districtModel.ResolvedUnverified = district.Sum(x => x.Field<int?>("ResolvedUnverified"));
                            districtModel.ResolvedVerified = district.Sum(x => x.Field<int?>("ResolvedVerified"));
                            districtModel.GrandTotal = district.Sum(x => x.Field<int?>("GrandTotal"));
                            SetClosureAndTimelinessProperties(districtModel);
                            if (!parameter.Tehsil.Equals("-1"))
                            {
                                districtModel.HierarchyName = "TehsilList";
                                districtModel.HierarchyList = new List<ClosureAndTimelinessModel>();
                                foreach (var tehsil in district.AsEnumerable().GroupBy(x => x.Field<int>("TehsilId")))
                                {
                                    ClosureAndTimelinessModel tehsilModel = new ClosureAndTimelinessModel();
                                    
                                    tehsilModel.RowLabel = tehsil.First().Field<string>("TehsilName").ToString().TrimEndLine();
                                    tehsilModel.RowId = tehsil.First().Field<int>("TehsilId").ToString().TrimEndLine();
                                    tehsilModel.ClosedVerified = tehsil.Sum(x => x.Field<int?>("ClosedVerified"));
                                    tehsilModel.PendingReopened = tehsil.Sum(x => x.Field<int?>("PendingReopened"));
                                    tehsilModel.PendingFresh = tehsil.Sum(x => x.Field<int?>("PendingFresh"));
                                    tehsilModel.PendingOverdue = tehsil.Sum(x => x.Field<int?>("PendingOverdue"));
                                    tehsilModel.ResolvedUnverified = tehsil.Sum(x => x.Field<int?>("ResolvedUnverified"));
                                    tehsilModel.ResolvedVerified = tehsil.Sum(x => x.Field<int?>("ResolvedVerified"));
                                    tehsilModel.GrandTotal = tehsil.Sum(x => x.Field<int?>("GrandTotal"));
                                    SetClosureAndTimelinessProperties(tehsilModel);
                                    if (!parameter.Markaz.Equals("-1"))
                                    {
                                        tehsilModel.HierarchyName = "MarkazList";
                                        tehsilModel.HierarchyList = new List<ClosureAndTimelinessModel>();
                                        foreach (var markaz in tehsil.AsEnumerable().Where(w=> w.ItemArray[4] != DBNull.Value).GroupBy(x => x.Field<int>("MarkazId")))
                                        {
                                            ClosureAndTimelinessModel markazModel = new ClosureAndTimelinessModel();
                                            markazModel.RowLabel = markaz.First().Field<string>("MarkazName").ToString().TrimEndLine();
                                            markazModel.RowId = markaz.First().Field<int>("MarkazId").ToString().TrimEndLine();
                                            markazModel.ClosedVerified = markaz.Sum(x => x.Field<int?>("ClosedVerified"));
                                            markazModel.PendingReopened = markaz.Sum(x => x.Field<int?>("PendingReopened"));
                                            markazModel.PendingFresh = markaz.Sum(x => x.Field<int?>("PendingFresh"));
                                            markazModel.PendingOverdue = markaz.Sum(x => x.Field<int?>("PendingOverdue"));
                                            markazModel.ResolvedUnverified = markaz.Sum(x => x.Field<int?>("ResolvedUnverified"));
                                            markazModel.ResolvedVerified = markaz.Sum(x => x.Field<int?>("ResolvedVerified"));
                                            markazModel.GrandTotal = markaz.Sum(x => x.Field<int?>("GrandTotal"));
                                            SetClosureAndTimelinessProperties(markazModel);
                                            tehsilModel.HierarchyList.Add(markazModel);
                                        }              
                                    }                                
                                    districtModel.HierarchyList.Add(tehsilModel);
                                }
                            }
                            if (parameter.Tehsil.Equals("-1") && !parameter.Markaz.Equals("-1"))
                            {
                                districtModel.HierarchyName = "MarkazList";
                                districtModel.HierarchyList = new List<ClosureAndTimelinessModel>();
                                foreach (var markaz in district.AsEnumerable().Where(w => w.ItemArray[4] != DBNull.Value).GroupBy(x => x.Field<int>("MarkazId")))
                                {
                                    ClosureAndTimelinessModel markazModel = new ClosureAndTimelinessModel();
                                    if (districtModel.HierarchyList.Count == 68)
                                    {
                                        
                                    }
                                    if (markaz.First() == null)
                                    {
                                        
                                    }
                                    markazModel.RowLabel = markaz.First().Field<string>("MarkazName").ToString().TrimEndLine();
                                    markazModel.RowId = markaz.First().Field<int>("MarkazId").ToString().TrimEndLine();
                                    markazModel.ClosedVerified = markaz.Sum(x => x.Field<int?>("ClosedVerified"));
                                    markazModel.PendingReopened = markaz.Sum(x => x.Field<int?>("PendingReopened"));
                                    markazModel.PendingFresh = markaz.Sum(x => x.Field<int?>("PendingFresh"));
                                    markazModel.PendingOverdue = markaz.Sum(x => x.Field<int?>("PendingOverdue"));
                                    markazModel.ResolvedUnverified = markaz.Sum(x => x.Field<int?>("ResolvedUnverified"));
                                    markazModel.ResolvedVerified = markaz.Sum(x => x.Field<int?>("ResolvedVerified"));
                                    markazModel.GrandTotal = markaz.Sum(x => x.Field<int?>("GrandTotal"));
                                    SetClosureAndTimelinessProperties(markazModel);
                                    districtModel.HierarchyList.Add(markazModel);
                                }
                               
                            }
                            model.HierarchyList.Add(districtModel);

                        }
                        
                    }
                    if (parameter.District.Equals("-1") && !parameter.Tehsil.Equals("-1"))
                    {
                        model.HierarchyName = "TehsilList";
                        model.HierarchyList = new List<ClosureAndTimelinessModel>();
                        foreach (var tehsil in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("TehsilId")))
                        {
                            ClosureAndTimelinessModel tehsilModel = new ClosureAndTimelinessModel();
                            tehsilModel.RowLabel = tehsil.First().Field<string>("TehsilName").ToString().TrimEndLine();
                            tehsilModel.RowId = tehsil.First().Field<int>("TehsilId").ToString().TrimEndLine();
                            tehsilModel.ClosedVerified = tehsil.Sum(x => x.Field<int?>("ClosedVerified"));
                            tehsilModel.PendingReopened = tehsil.Sum(x => x.Field<int?>("PendingReopened"));
                            tehsilModel.PendingFresh = tehsil.Sum(x => x.Field<int?>("PendingFresh"));
                            tehsilModel.PendingOverdue = tehsil.Sum(x => x.Field<int?>("PendingOverdue"));
                            tehsilModel.ResolvedUnverified = tehsil.Sum(x => x.Field<int?>("ResolvedUnverified"));
                            tehsilModel.ResolvedVerified = tehsil.Sum(x => x.Field<int?>("ResolvedVerified"));
                            tehsilModel.GrandTotal = tehsil.Sum(x => x.Field<int?>("GrandTotal"));
                            SetClosureAndTimelinessProperties(tehsilModel);
                            if (!parameter.Markaz.Equals("-1"))
                            {
                                tehsilModel.HierarchyName = "MarkazList";
                                tehsilModel.HierarchyList = new List<ClosureAndTimelinessModel>();
                                foreach (var markaz in tehsil.AsEnumerable().Where(w => w.ItemArray[4] != DBNull.Value).GroupBy(x => x.Field<int>("MarkazId")))
                                {
                                    ClosureAndTimelinessModel markazModel = new ClosureAndTimelinessModel();
                                    markazModel.RowLabel = markaz.First().Field<string>("MarkazName").ToString().TrimEndLine();
                                    markazModel.RowId = markaz.First().Field<int>("MarkazId").ToString().TrimEndLine();
                                    markazModel.ClosedVerified = markaz.Sum(x => x.Field<int?>("ClosedVerified"));
                                    markazModel.PendingReopened = markaz.Sum(x => x.Field<int?>("PendingReopened"));
                                    markazModel.PendingFresh = markaz.Sum(x => x.Field<int?>("PendingFresh"));
                                    markazModel.PendingOverdue = markaz.Sum(x => x.Field<int?>("PendingOverdue"));
                                    markazModel.ResolvedUnverified = markaz.Sum(x => x.Field<int?>("ResolvedUnverified"));
                                    markazModel.ResolvedVerified = markaz.Sum(x => x.Field<int?>("ResolvedVerified"));
                                    markazModel.GrandTotal = markaz.Sum(x => x.Field<int?>("GrandTotal"));
                                    SetClosureAndTimelinessProperties(markazModel);
                                    tehsilModel.HierarchyList.Add(markazModel);
                                }
                            }
                            model.HierarchyList.Add(tehsilModel);
                        }
                    }
                    if (parameter.District.Equals("-1") && parameter.Tehsil.Equals("-1") && !parameter.Markaz.Equals("-1"))
                    {
                        model.HierarchyName = "MarkazList";
                        model.HierarchyList = new List<ClosureAndTimelinessModel>();
                        foreach (var markaz in lObjData.AsEnumerable().Where(w => w.ItemArray[4] != DBNull.Value).GroupBy(x => x.Field<int>("MarkazId")))
                        {
                            ClosureAndTimelinessModel markazModel = new ClosureAndTimelinessModel();
                            markazModel.RowLabel = markaz.First().Field<string>("MarkazName").ToString().TrimEndLine();
                            markazModel.RowId = markaz.First().Field<int>("MarkazId").ToString().TrimEndLine();
                            markazModel.ClosedVerified = markaz.Sum(x => x.Field<int?>("ClosedVerified"));
                            markazModel.PendingReopened = markaz.Sum(x => x.Field<int?>("PendingReopened"));
                            markazModel.PendingFresh = markaz.Sum(x => x.Field<int?>("PendingFresh"));
                            markazModel.PendingOverdue = markaz.Sum(x => x.Field<int?>("PendingOverdue"));
                            markazModel.ResolvedUnverified = markaz.Sum(x => x.Field<int?>("ResolvedUnverified"));
                            markazModel.ResolvedVerified = markaz.Sum(x => x.Field<int?>("ResolvedVerified"));
                            markazModel.GrandTotal = markaz.Sum(x => x.Field<int?>("GrandTotal"));
                            SetClosureAndTimelinessProperties(markazModel);
                            model.HierarchyList.Add(markazModel);
                        }
                       
                    }
                
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                throw ex;
            }
            OrderByListRecursive(model.HierarchyList, parameter);
            SetTopAndWorstQualityClosureAndTimeliness(model);
            return model;
        }
        private static void SetClosureAndTimelinessProperties(ClosureAndTimelinessModel model)
        {
            if (model.PendingFresh != null && model.PendingOverdue != null && model.PendingReopened != null && model.GrandTotal != null && ((model.GrandTotal - model.PendingFresh) > 0))
            {
                // Version 1 Formula
                //model.Timeliness = Math.Ceiling((double)(100 * (1 - (model.PendingOverdue * 1.0 / (model.GrandTotal + model.PendingFresh)))));
                // Version 2 Formula
                model.Timeliness = Math.Ceiling((double)(100*(1-((model.PendingOverdue*1.0+model.PendingReopened*1.0)/(model.GrandTotal-model.PendingFresh)))));
            }            
            else 
            {
                model.Timeliness = 0;
            }
            if (model.ClosedVerified != null && model.PendingReopened != null && model.PendingOverdue != null && ((model.PendingReopened + model.ClosedVerified + model.PendingOverdue) > 0))
            {
                // Version 1 Formula
                //model.ClosureRate = Math.Ceiling((double)(model.ClosedVerified * 100 * 1.0 / (model.ClosedVerified + model.PendingReopened)));
                // Version 2 Formula
                model.ClosureRate = Math.Ceiling((double)(model.ClosedVerified*100*1.0/(model.ClosedVerified+model.PendingReopened+model.PendingOverdue)));
            }
            else if (model.ClosedVerified != null && model.PendingReopened != null && ((model.PendingReopened + model.ClosedVerified) == 0))
            {
                model.ClosureRate = -1;
            }
            else
            {
                model.ClosureRate = 0;
            }
        }      
        public static ClosureAndTimelinessAggregateModel GetClosureAndTimelinessAggregateData(ClosureAndTimelinessParameterModel parameter)
        {
            ClosureAndTimelinessAggregateModel model = null;
            try
            {
                string ParameterClause = null;
                string SelectClause = null;
                string WhereClause = null;
                string GroupyByClause = null;
                string OrderByClause = null;
                string AssigneeClause = null;
                List<string> lstSelectClause = new List<string>();
                List<string> lstWhereClause = new List<string>();
                List<string> lstGroupByClause = new List<string>();
                List<string> lstOrderByClause = new List<string>();
                List<string> lstAssigneeClause = new List<string>();
                // District settings

                if (parameter.District.Equals("All", StringComparison.OrdinalIgnoreCase) || parameter.District.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',District_Name AS 'DistrictName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("District_Id");
                    lstGroupByClause.Add("District_Name");
                    lstOrderByClause.Add("District_Name");

                }
                else
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',District_Name AS 'DistrictName'");
                    lstWhereClause.Add("District_Id IN (" + parameter.District + ")");
                    lstGroupByClause.Add("District_Id");
                    lstGroupByClause.Add("District_Name");
                    lstOrderByClause.Add("District_Name");
                }

                // Tehsil settings

                if (parameter.Tehsil.Equals("All", StringComparison.OrdinalIgnoreCase) || parameter.Tehsil.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 4 AND Crm_Id = c.Tehsil_Id ) As 'TehsilId',Tehsil_Name As 'TehsilName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("Tehsil_Id");
                    lstGroupByClause.Add("Tehsil_Name");
                    lstOrderByClause.Add("Tehsil_Name");

                }
                else
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 4 AND Crm_Id = c.Tehsil_Id ) As 'TehsilId',Tehsil_Name As 'TehsilName'");
                    lstWhereClause.Add("Tehsil_Id IN (" + parameter.Tehsil + ")");
                    lstGroupByClause.Add("Tehsil_Id");
                    lstGroupByClause.Add("Tehsil_Name");
                    lstOrderByClause.Add("Tehsil_Name");
                }

                // Markaz settings

                if (parameter.Markaz.Equals("All", StringComparison.OrdinalIgnoreCase) || parameter.Markaz.Equals("-1", StringComparison.OrdinalIgnoreCase))
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'MarkazId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 5 AND Crm_Id = c.UnionCouncil_Id ) As 'MarkazId',UnionCouncil_Name As 'MarkazName'");
                    lstWhereClause.Add("");
                    lstGroupByClause.Add("UnionCouncil_Id");
                    lstGroupByClause.Add("UnionCouncil_Name");
                    lstOrderByClause.Add("UnionCouncil_Name");

                }
                else
                {
                    lstSelectClause.Add("( SELECT e.OTS_Id As 'MarkazId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 5 AND Crm_Id = c.UnionCouncil_Id ) As 'MarkazId',UnionCouncil_Name As 'MarkazName'");
                    lstWhereClause.Add("UnionCouncil_Id IN (" + parameter.Markaz + ")");
                    lstGroupByClause.Add("UnionCouncil_Id");
                    lstGroupByClause.Add("UnionCouncil_Name");
                    lstOrderByClause.Add("UnionCouncil_Name");
                }

                // SchoolLevel settings

                if (parameter.SchoolLevel.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstSchoolLevel = ClosureAndTimelinessParameterModel.SchoolLevelAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolLevel.Length; i++)
                    {
                        lstSchoolLevel[i] = "'" + lstSchoolLevel[i] + "'";
                    }
                    lstWhereClause.Add("RefField3 IN (" + string.Join(",", lstSchoolLevel) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstSchoolLevel = parameter.SchoolLevel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolLevel.Length; i++)
                    {
                        lstSchoolLevel[i] = "'" + lstSchoolLevel[i] + "'";
                    }
                    lstWhereClause.Add("RefField3 IN (" + string.Join(",", lstSchoolLevel) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }

                // SchoolGender settings

                if (parameter.SchoolGender.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstSchoolGender = ClosureAndTimelinessParameterModel.SchoolGenderAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolGender.Length; i++)
                    {
                        lstSchoolGender[i] = "'" + lstSchoolGender[i] + "'";
                    }
                    lstWhereClause.Add("RefField5 IN (" + string.Join(",", lstSchoolGender) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstSchoolGender = parameter.SchoolGender.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstSchoolGender.Length; i++)
                    {
                        lstSchoolGender[i] = "'" + lstSchoolGender[i] + "'";
                    }
                    lstWhereClause.Add("RefField5 IN (" + string.Join(",", lstSchoolGender) + ")");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }


                if (parameter.Assignee.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lstAssignee = ClosureAndTimelinessParameterModel.AssigneeAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstAssignee.Length; i++)
                    {
                        lstAssignee[i] = "'" + lstAssignee[i] + "'";
                    }
                    lstAssigneeClause.Add("Designation_abbr IN (" + string.Join(",", lstAssignee) + ")");
                    lstWhereClause.Add("");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");

                }
                else
                {
                    string[] lstAssignee = parameter.Assignee.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lstAssignee.Length; i++)
                    {
                        lstAssignee[i] = "'" + lstAssignee[i] + "'";
                    }
                    lstAssigneeClause.Add("Designation_abbr IN (" + string.Join(",", lstAssignee) + ")");
                    lstWhereClause.Add("");
                    lstSelectClause.Add("");
                    lstGroupByClause.Add("");
                    lstGroupByClause.Add("");
                    lstOrderByClause.Add("");
                }

                lstAssigneeClause.RemoveAll(x => x.Equals(String.Empty));
                lstWhereClause.RemoveAll(x => x.Equals(String.Empty));
                lstSelectClause.RemoveAll(x => x.Equals(String.Empty));
                lstGroupByClause.RemoveAll(x => x.Equals(String.Empty));
                lstGroupByClause.RemoveAll(x => x.Equals(String.Empty));
                lstOrderByClause.RemoveAll(x => x.Equals(String.Empty));

                ParameterClause = "DECLARE @startDate VARCHAR(50) = '" + parameter.StartDate + "'; " +
                "DECLARE @endDate VARCHAR(50) = '" + parameter.EndDate + "'; " +
                "DECLARE @currentDate datetime = CONVERT(datetime,@endDate,101); " +
                "DECLARE @currentMonthFirstDay datetime = DATEADD(month, DATEDIFF(month, 0, @currentDate), 0); " +
                "DECLARE @previousMonthFirstDay datetime = DATEADD(month, DATEDIFF(month, 0, @currentDate)-1, 0); " +
                "DECLARE @nextMonthFirstDay datetime = DATEADD(month, DATEDIFF(month, 0, @currentDate)+1, 0); " +
                "DECLARE @currentMonthLastDay datetime = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE())+1,0)); " +
                "DECLARE @previousMonthLastDay datetime = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE()),0)); " +
                "DECLARE @nextMonthLastDay datetime = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE())+2,0)); " +
                "DECLARE @ClosedVerified VARCHAR(50) = 'Closed (Verified)'; " +
                "DECLARE @PendingFresh VARCHAR(50) = 'Pending (Fresh)'; " +
                "DECLARE @PendingReopened VARCHAR(50) = 'Pending (Reopened)'; " +
                "DECLARE @PendingOverdue VARCHAR(50) = 'Unsatisfactory Closed'; " +
                "DECLARE @ResolvedUnverified VARCHAR(50) = 'Resolved (Unverified)'; " +
                "DECLARE @ResolvedVerified VARCHAR(50) = 'Resolved (Verified)'; ";

                AssigneeClause = string.Join(" AND ", lstAssigneeClause);
                SelectClause = "SELECT " + string.Join(",", lstSelectClause) + ", " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthClosedVerified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingFresh AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthPendingFresh', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingReopened AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthPendingReopened', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthPendingOverdue', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedUnverified AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthResolvedUnverified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedVerified AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthResolvedVerified', " +
                "SUM(CASE WHEN Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END) AS 'currentMonthGrandTotal', " +

                "SUM(CASE WHEN c.Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthClosedVerified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingFresh AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthPendingFresh', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingReopened AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthPendingReopened', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthPendingOverdue', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedUnverified AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthResolvedUnverified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedVerified AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthResolvedVerified', " +
                "SUM(CASE WHEN Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END) AS 'previousMonthGrandTotal', " +


                "SUM(CASE WHEN c.Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthClosedVerified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingFresh AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthPendingFresh', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingReopened AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthPendingReopened', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthPendingOverdue', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedUnverified AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthResolvedUnverified', " +
                "SUM(CASE WHEN c.Complaint_Computed_Status = @ResolvedVerified AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthResolvedVerified', " +
                "SUM(CASE WHEN Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END) AS 'nextMonthGrandTotal', " +


                "CEILING(SUM(CASE WHEN (Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay)  THEN 1 ELSE 0 END)*100*1.0/NULLIF(SUM(CASE WHEN " +
                "(Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay) THEN 1 ELSE 0 END) + SUM(CASE WHEN (Complaint_Computed_Status = @PendingReopened  " +
                                "AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay) THEN 1 ELSE 0 END),0)) AS 'CurrentMonthClosureRate', " +

                "CEILING(100*(1-(SUM(CASE WHEN (Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay) THEN 1 ELSE 0 END)*1.0/NULLIF(COUNT(CASE WHEN  " +
                                "Created_Date >= @currentMonthFirstDay AND Created_Date <= @currentMonthLastDay THEN 1 ELSE 0 END)-SUM(CASE WHEN (Complaint_Computed_Status = @PendingFresh AND Created_Date >= @currentMonthFirstDay  " +
                                "AND Created_Date <= @currentMonthLastDay) THEN 1 ELSE 0 END),0)))) AS 'CurrentMonthTimeliness', " +

                "CEILING(SUM(CASE WHEN (Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay) THEN 1 ELSE 0 END)*100*1.0/NULLIF(SUM(CASE WHEN  " +
                "(Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay) THEN 1 ELSE 0 END) + SUM(CASE WHEN (Complaint_Computed_Status = @PendingReopened  " +
                                "AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay) THEN 1 ELSE 0 END),0)) AS 'PrevioustMonthClosureRate', " +

                "CEILING(100*(1-(SUM(CASE WHEN (Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay) THEN 1 ELSE 0 END)*1.0/NULLIF(COUNT(CASE WHEN  " +
                                "Created_Date >= @previousMonthFirstDay AND Created_Date <= @previousMonthLastDay THEN 1 ELSE 0 END)-SUM(CASE WHEN (Complaint_Computed_Status = @PendingFresh AND Created_Date >= @previousMonthFirstDay  " +
                                "AND Created_Date <= @previousMonthLastDay) THEN 1 ELSE 0 END),0)))) AS 'PreviousMonthTimeliness', " +

                "CEILING(SUM(CASE WHEN (Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay) THEN 1 ELSE 0 END)*100*1.0/NULLIF(SUM(CASE WHEN  " +
                "(Complaint_Computed_Status = @ClosedVerified AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay) THEN 1 ELSE 0 END) + SUM(CASE WHEN (Complaint_Computed_Status = @PendingReopened  " +
                                "AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay) THEN 1 ELSE 0 END),0)) AS 'NextMonthClosureRate', " +

                "CEILING(100*(1-(SUM(CASE WHEN (Complaint_Computed_Status = @PendingOverdue AND Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay) THEN 1 ELSE 0 END)*1.0/NULLIF(COUNT(CASE WHEN  " +
                                "Created_Date >= @nextMonthFirstDay AND Created_Date <= @nextMonthLastDay THEN 1 ELSE 0 END)-SUM(CASE WHEN (Complaint_Computed_Status = @PendingFresh AND Created_Date >= @nextMonthFirstDay  " +
                                "AND Created_Date <= @nextMonthLastDay) THEN 1 ELSE 0 END),0)))) AS 'NextMonthTimeliness', " +

                "COUNT(c.Complaint_Status) AS 'GrandTotal'  " +
                "FROM PITB.Complaints AS c  ";// +
                    //"LEFT JOIN (SELECT a.Complaint_Id, Assignee =  " +
                    //"(SELECT (SELECT  RTRIM(LTRIM(CAST(users.Name as nvarchar(max))))+':'+RTRIM(LTRIM(users.Designation_abbr)) +',  ')  " +
                    //"FROM PITB.User_Wise_Complaints b INNER JOIN PITB.Users users ON  users.id = b.User_Id  " +
                    //"WHERE  b.Complaint_Subtype = 1 AND b.Complaint_Id = a.Complaint_Id AND " + AssigneeClause + ") " +
                    //"FROM PITB.User_Wise_Complaints  a  " +
                    //"WHERE a.Complaint_Subtype = 1  " +
                    //"GROUP BY a.Complaint_Id) complaintsAssignee ON complaintsAssignee.Complaint_Id = c.Id ";
                WhereClause = "WHERE Compaign_Id = 47 AND Complaint_Type = 1 AND Complaint_Category NOT IN(324,333) AND " +
                " " + string.Join(" AND ", lstWhereClause) + " ";
                GroupyByClause = "GROUP BY Province_Id, " + string.Join(",", lstGroupByClause) + " ";
                OrderByClause = "ORDER BY " + string.Join(",", lstOrderByClause) + " ";

                string queryStr = String.Concat(new string[] { ParameterClause, SelectClause, WhereClause, GroupyByClause, OrderByClause });
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    model = new ClosureAndTimelinessAggregateModel();
                    model.RowLabel = "GrandTotal";
                    model.ClosedVerified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthClosedVerified"));
                    model.PendingReopened = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthPendingReopened"));
                    model.PendingFresh = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthPendingFresh"));
                    model.PendingOverdue = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthPendingOverdue"));
                    model.ResolvedUnverified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthResolvedUnverified"));
                    model.ResolvedVerified = lObjData.Rows.Cast<DataRow>().Sum(x => x.Field<int?>("currentMonthResolvedVerified"));
                    model.GrandTotal = null;
                    model.CurrentMonthAggregateClosureRate = null;
                    model.CurrentMonthAggregateTimeliness = null;
                    model.PreviousMonthAggregateClosureRate = null;
                    model.PreviousMonthAggregateTimeliness = null;
                    model.NextMonthAggregateClosureRate = null;
                    model.NextMonthAggregateTimeliness = null;

                    if (!parameter.District.Equals("-1"))
                    {
                        model.HierarchyName = "DistrictList";
                        model.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                        foreach (var district in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("DistrictId")))
                        {
                            ClosureAndTimelinessAggregateModel districtModel = new ClosureAndTimelinessAggregateModel();
                            SetClosureAndTimelinessAggregateValues("DistrictName", districtModel, district);
                            if (!parameter.Tehsil.Equals("-1"))
                            {
                                districtModel.HierarchyName = "TehsilList";
                                districtModel.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                                foreach (var tehsil in district.AsEnumerable().GroupBy(x => x.Field<int>("TehsilId")))
                                {
                                    ClosureAndTimelinessAggregateModel tehsilModel = new ClosureAndTimelinessAggregateModel();
                                    SetClosureAndTimelinessAggregateValues("TehsilName", tehsilModel, tehsil);
                                    if (!parameter.Markaz.Equals("-1"))
                                    {
                                        tehsilModel.HierarchyName = "MarkazList";
                                        tehsilModel.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                                        foreach (var markaz in tehsil.AsEnumerable().GroupBy(x => x.Field<int>("MarkazId")))
                                        {
                                            ClosureAndTimelinessAggregateModel markazModel = new ClosureAndTimelinessAggregateModel();
                                            SetClosureAndTimelinessAggregateValues("MarkazName", markazModel, markaz);
                                            tehsilModel.HierarchyList.Add(markazModel);
                                        }
                                    }
                                    districtModel.HierarchyList.Add(tehsilModel);
                                }
                            }
                            if (parameter.Tehsil.Equals("-1") && !parameter.Markaz.Equals("-1"))
                            {
                                districtModel.HierarchyName = "MarkazList";
                                districtModel.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                                foreach (var markaz in district.AsEnumerable().GroupBy(x => x.Field<int>("MarkazId")))
                                {
                                    ClosureAndTimelinessAggregateModel markazModel = new ClosureAndTimelinessAggregateModel();
                                    SetClosureAndTimelinessAggregateValues("MarkazName", markazModel, markaz);
                                    districtModel.HierarchyList.Add(markazModel);
                                }
                            }
                            model.HierarchyList.Add(districtModel);
                        }
                    }
                    if (parameter.District.Equals("-1") && !parameter.Tehsil.Equals("-1"))
                    {
                        model.HierarchyName = "TehsilList";
                        model.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                        foreach (var tehsil in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("TehsilId")))
                        {
                            ClosureAndTimelinessAggregateModel tehsilModel = new ClosureAndTimelinessAggregateModel();
                            SetClosureAndTimelinessAggregateValues("TehsilName", tehsilModel, tehsil);
                            if (!parameter.Markaz.Equals("-1"))
                            {
                                tehsilModel.HierarchyName = "MarkazList";
                                tehsilModel.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                                foreach (var markaz in tehsil.AsEnumerable().GroupBy(x => x.Field<int>("MarkazId")))
                                {
                                    ClosureAndTimelinessAggregateModel markazModel = new ClosureAndTimelinessAggregateModel();
                                    SetClosureAndTimelinessAggregateValues("MarkazName", markazModel, markaz);
                                    tehsilModel.HierarchyList.Add(markazModel);
                                }
                            }
                            model.HierarchyList.Add(tehsilModel);
                        }
                    }
                    if (parameter.District.Equals("-1") && parameter.Tehsil.Equals("-1") && !parameter.Markaz.Equals("-1"))
                    {
                        model.HierarchyName = "MarkazList";
                        model.HierarchyList = new List<ClosureAndTimelinessAggregateModel>();
                        foreach (IGrouping<int, DataRow> markaz in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("MarkazId")))
                        {
                            ClosureAndTimelinessAggregateModel markazModel = new ClosureAndTimelinessAggregateModel();
                            SetClosureAndTimelinessAggregateValues("MarkazName", markazModel, markaz);
                            model.HierarchyList.Add(markazModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
        public static List<SchoolsWithRecurringComplaintsModel> GetSchoolsWithRecurringComplaintsData(DateTime startDate, DateTime endDate)
        {
            List<SchoolsWithRecurringComplaintsModel> lstResponse = null;
            try
            {
                string queryStr = "SELECT c.Id As 'ComplaintId',( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',c.District_Name As 'DistrictName', " +
                "s.school_emis_code As 'SchoolEmisCode', c.UnionCouncil_Id AS 'MarkazId', c.UnionCouncil_Name As 'MarkazName', " + 
                "s.school_name As 'SchoolName',c.Complaint_Category As 'TypeId' ,c.Complaint_Category_Name As 'TypeName', " + 
                "c.Complaint_SubCategory As 'SubTypeId',c.Complaint_SubCategory_Name As 'SubTypeName' " +  
                "FROM PITB.Complaints As c INNER JOIN dbo.Schools_Mapping As s ON s.school_emis_code = c.RefField1 " + 
                "WHERE c.Compaign_Id = 47 AND Created_Date >= '" + startDate.ToString("MM/dd/yyyy") + "' " +
                "AND Created_Date <= '" + endDate.ToString("MM/dd/yyyy") + "' AND c.Complaint_Category NOT IN(324,333) " +
                "GROUP BY c.District_Id,c.District_Name,s.school_emis_code,c.UnionCouncil_Id,c.UnionCouncil_Name, " + 
                "s.school_name,c.Complaint_Category,c.Complaint_Category_Name,c.Complaint_SubCategory, " +  
                "c.Complaint_SubCategory_Name,c.Id ORDER BY c.District_Name,s.school_emis_code ";
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    lstResponse = new List<SchoolsWithRecurringComplaintsModel>();
                    foreach (var districtRow in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("DistrictId")))
                    {
                        SchoolsWithRecurringComplaintsModel model = new SchoolsWithRecurringComplaintsModel();
                        model.DistrictName = districtRow.First().Field<string>("DistrictName").ToString().TrimEndLine();
                        model.DistrictId = districtRow.First().Field<int>("DistrictId").ToString().TrimEndLine();
                        model.CountOfSchoolEmisCode = districtRow.Count();
                        model.SchoolEmisCodeNameList = new List<clsSchoolEmisCodeName>();
                        foreach (var schoolemiscodeRow in districtRow.AsEnumerable().GroupBy(y => y.Field<string>("SchoolEmisCode")))
                        {
                            clsSchoolEmisCodeName schoolemiscode = new clsSchoolEmisCodeName();
                            schoolemiscode.SchoolEmisCodeName = schoolemiscodeRow.First().Field<string>("SchoolEmisCode").ToString().TrimEndLine();
                            schoolemiscode.CountOfMarkazName = schoolemiscodeRow.Count();
                            schoolemiscode.MarkazNameList = new List<clsMarkazName>();
                            foreach (var markaznameRow in schoolemiscodeRow.AsEnumerable().GroupBy(z => z.Field<int>("MarkazId")))
                            {
                                clsMarkazName markazName = new clsMarkazName();
                                markazName.MarkazName = markaznameRow.First().Field<string>("MarkazName").ToString().TrimEndLine(); ;
                                markazName.MarkazId = markaznameRow.First().Field<int>("MarkazId").ToString().TrimEndLine(); ;
                                markazName.CountOfSchoolName = markaznameRow.Count();
                                markazName.SchoolNameList = new List<clsSchoolName>();
                                foreach (var schoolnameRow in markaznameRow.AsEnumerable().GroupBy(d => d.Field<string>("SchoolName")))
                                {
                                    clsSchoolName schoolName = new clsSchoolName();
                                    schoolName.SchoolName = markaznameRow.First().Field<string>("SchoolName").ToString().TrimEndLine(); ;
                                    schoolName.CountOfTypeName = markaznameRow.Count();
                                    schoolName.TypeNameList = new List<clsTypeName>();
                                    foreach (var typenameRow in markaznameRow.AsEnumerable().GroupBy(a => a.Field<string>("TypeName")))
                                    {
                                        clsTypeName typeName = new clsTypeName();
                                        typeName.TypeName = typenameRow.First().Field<string>("TypeName").ToString().TrimEndLine();
                                        typeName.TypeId = typenameRow.First().Field<int>("TypeId").ToString().TrimEndLine();
                                        typeName.CountOfSubTypeName = typenameRow.Count();
                                        typeName.SubTypeNameList = new List<clsSubTypeName>();
                                        foreach (var subtypenameRow in typenameRow.AsEnumerable().GroupBy(b => b.Field<string>("SubTypeName")))
                                        {
                                            clsSubTypeName subTypeName = new clsSubTypeName();
                                            subTypeName.SubTypeName = subtypenameRow.First().Field<string>("SubTypeName").ToString().TrimEndLine(); ;
                                            subTypeName.SubTypeId = subtypenameRow.First().Field<int>("SubTypeId").ToString().TrimEndLine(); ;
                                            subTypeName.SubTypeCount = subtypenameRow.Count();
                                            typeName.SubTypeNameList.Add(subTypeName);
                                        }
                                        //typeName.SubTypeNameList.Sort((x, y) => x.SubTypeCount.CompareTo(y.SubTypeCount));
                                        schoolName.TypeNameList.Add(typeName);
                                    }
                                    schoolName.TypeNameList.Sort((x, y) => x.CountOfSubTypeName.CompareTo(y.CountOfSubTypeName));
                                    markazName.SchoolNameList.Add(schoolName);
                                }
                                //markazName.SchoolNameList.Sort((x, y) => x.CountOfTypeName.CompareTo(y.CountOfTypeName));
                                schoolemiscode.MarkazNameList.Add(markazName);
                            }
                            //schoolemiscode.MarkazNameList.Sort((x,y) => x.CountOfSchoolName.CompareTo(y.CountOfSchoolName));
                            model.SchoolEmisCodeNameList.Add(schoolemiscode);
                        }
                        model.SchoolEmisCodeNameList.OrderByDescending(x => x.MarkazNameList.OrderByDescending(s => s.SchoolNameList.OrderBy(w => w.TypeNameList.OrderByDescending(e => e.CountOfSubTypeName))));
                        lstResponse.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstResponse;
        }

        public static List<SchoolsWithRecurringComplaintsNewFormModel> GetSchoolsWithRecurringComplaintsNewFormData(DateTime startDate, DateTime endDate)
        {
            List<SchoolsWithRecurringComplaintsNewFormModel> lstResponse = null;
            try
            {
                string queryStr = "SELECT c.Id As 'ComplaintId',( SELECT e.OTS_Id As 'DistricId' FROM PITB.CrmIdsMappingToOtherSystems e WHERE e.Crm_Module_Id = 1 AND Crm_Module_Cat1 = 3 AND Crm_Id = c.District_Id ) As 'DistrictId',c.District_Name As 'DistrictName', " +
                "s.school_emis_code As 'SchoolEmisCode', c.UnionCouncil_Id AS 'MarkazId', c.UnionCouncil_Name As 'MarkazName', " +
                "s.school_name As 'SchoolName',c.Complaint_Category As 'TypeId' ,c.Complaint_Category_Name As 'TypeName', " +
                "c.Complaint_SubCategory As 'SubTypeId',c.Complaint_SubCategory_Name As 'SubTypeName' " +
                "FROM PITB.Complaints As c INNER JOIN dbo.Schools_Mapping As s ON s.school_emis_code = c.RefField1 " +
                "WHERE c.Compaign_Id = 47 AND Created_Date >= '" + startDate.ToString("MM/dd/yyyy") + "' " +
                "AND Created_Date <= '" + endDate.ToString("MM/dd/yyyy") + "' AND c.Complaint_Category NOT IN(324,333) " +
                "GROUP BY c.District_Id,c.District_Name,s.school_emis_code,c.UnionCouncil_Id,c.UnionCouncil_Name, " +
                "s.school_name,c.Complaint_Category,c.Complaint_Category_Name,c.Complaint_SubCategory, " +
                "c.Complaint_SubCategory_Name,c.Id ORDER BY c.District_Name,s.school_emis_code ";
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);
                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    lstResponse = new List<SchoolsWithRecurringComplaintsNewFormModel>();
                    int i = 0;
                    foreach (var districtRow in lObjData.AsEnumerable().GroupBy(x => x.Field<int>("DistrictId")))
                    {

                        int j = 0;
                        SchoolsWithRecurringComplaintsNewFormModel model = new SchoolsWithRecurringComplaintsNewFormModel();
                        model.DistrictName = districtRow.First().Field<string>("DistrictName").ToString().TrimEndLine();
                        model.DistrictId = districtRow.First().Field<int>("DistrictId").ToString().TrimEndLine();
                        model.schools = new List<SchoolIdentification>();
                        foreach (var schoolemiscodeRow in districtRow.AsEnumerable().GroupBy(y => y.Field<string>("SchoolEmisCode")))
                        {
                            if (i == 1 && j == 249)
                            {
                                int k = 0;
                            }   
                            SchoolIdentification school = new SchoolIdentification();
                            Debug.WriteLine("i = " + i +" , J = "+j+ ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.DistrictId = model.DistrictId;
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.DistrictName = model.DistrictName;
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.SchoolName = schoolemiscodeRow.First().Field<string>("SchoolName").ToString().TrimEndLine();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.SchoolEmisCode = schoolemiscodeRow.First().Field<string>("SchoolEmisCode").ToString().TrimEndLine();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.MarkazName = schoolemiscodeRow.AsEnumerable<DataRow>().First(x => x.ItemArray[5] != null).ItemArray[5].ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.CountOfTotalComplaints = schoolemiscodeRow.Count().ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.MarkazId = schoolemiscodeRow.First().Field<int>("MarkazId").ToString().TrimEndLine();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.MaxTypeName = schoolemiscodeRow.GroupBy(s => s.Field<int>("TypeId")).Select(group => new { TypeId = group.Key, TypeName=group.First().Field<string>("TypeName"), MaxNoOfSubTypeComplaints = group.GroupBy(e => e.Field<int>("SubTypeId")).Max(r => r.Count()), SubTypeName = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).First().First().Field<string>("SubTypeName").ToString().TrimEndLine() }).First().TypeName.ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.CountOfMaxSubTypeNameComplaint = schoolemiscodeRow.GroupBy(s => s.Field<int>("TypeId")).Select(group => new { TypeId = group.Key, TypeName = group.First().Field<string>("TypeName"), MaxNoOfSubTypeComplaints = group.GroupBy(e => e.Field<int>("SubTypeId")).Max(r => r.Count()), SubTypeName = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).First().First().Field<string>("SubTypeName").ToString().TrimEndLine(), SubTypeId = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).FirstOrDefault().First().Field<int>("SubTypeId").ToString().TrimEndLine() }).First().MaxNoOfSubTypeComplaints.ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.MaxSubTypeName = schoolemiscodeRow.GroupBy(s => s.Field<int>("TypeId")).Select(group => new { TypeId = group.Key, TypeName = group.First().Field<string>("TypeName"), MaxNoOfSubTypeComplaints = group.GroupBy(e => e.Field<int>("SubTypeId")).Max(r => r.Count()), SubTypeName = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).First().First().Field<string>("SubTypeName").ToString().TrimEndLine() }).First().SubTypeName.ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.TypeId = schoolemiscodeRow.GroupBy(s => s.Field<int>("TypeId")).Select(group => new { TypeId = group.Key, TypeName = group.First().Field<string>("TypeName"), MaxNoOfSubTypeComplaints = group.GroupBy(e => e.Field<int>("SubTypeId")).Max(r => r.Count()), SubTypeName = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).First().First().Field<string>("SubTypeName").ToString().TrimEndLine(), SubTypeId = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).FirstOrDefault().First().Field<int>("SubTypeId").ToString().TrimEndLine() }).First().TypeId.ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            school.SubTypeId = schoolemiscodeRow.GroupBy(s => s.Field<int>("TypeId")).Select(group => new { TypeId = group.Key, TypeName = group.First().Field<string>("TypeName"), MaxNoOfSubTypeComplaints = group.GroupBy(e => e.Field<int>("SubTypeId")).Max(r => r.Count()), SubTypeName = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).First().First().Field<string>("SubTypeName").ToString().TrimEndLine(), SubTypeId = group.GroupBy(e => e.Field<int>("SubTypeId")).OrderByDescending(t => t.Count()).FirstOrDefault().First().Field<int>("SubTypeId").ToString().TrimEndLine() }).First().SubTypeId.ToString();
                            Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());
                            model.schools.Add(school);
                            j++;
                        }
                        Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());  
                        model.schools = model.schools.OrderByDescending(x=> x.CountOfTotalComplaints).Take(5).ToList();
                        Debug.WriteLine("i = " + i + " , J = " + j + ", Line Number: " + new StackFrame(0, true).GetFileLineNumber());     
                        lstResponse.Add(model);
                        i++;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstResponse;
        }
        public static List<CategoryDistributionModel> GetCategoryDistributionData(DateTime startDate, DateTime endDate)
        {
            List<CategoryDistributionModel> lstResponse = null;
            try
            {
                string queryStr = "" + " " +
                "DECLARE @startDateStr AS datetime = '" + startDate.ToString("MM/dd/yyyy") + "';" + "  " +
                "DECLARE @endDateStr AS datetime = '" + endDate.ToString("MM/dd/yyyy") + "';" + "  " +

                "CREATE TABLE #Temp" + "  " +
                "(Id INT IDENTITY(1,1), DistrictId INT,DistrictName VARCHAR(MAX),DepartmentId INT, DepartmentName VARCHAR(MAX));" + "  " +

                "DECLARE @startDate datetime = CONVERT(datetime,@startDateStr,101);" + "  " +
                "DECLARE @endDate datetime = CONVERT(datetime,@endDateStr,101);" + "  " +
                "DECLARE @startMonthFirstDay datetime = DATEADD(month, DATEDIFF(month, 0, @startDate), 0);" + "  " +
                "DECLARE @LastMonthLastDay datetime = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@endDate)+1,0));" + "  " +
                "DECLARE @MonthName VARCHAR(20);" + "  " +
                "DECLARE @Year INT;" + "  " +
                "DECLARE @ColumnName VARCHAR(30);" + "  " +
                "DECLARE @ListOfDistrictIds TABLE(Id INT);" + "  " +
                "DECLARE @ListOfDepartmentIds TABLE(Id INT);" + "  " +
                "DECLARE @sql VARCHAR(MAX);" + "  " +
                "DECLARE @sqlTemp VARCHAR(MAX);" + "  " +

                "DECLARE @DistrictId INT;" + "  " +
                "DECLARE @DistrictName VARCHAR(50);" + "  " +
                "DECLARE District_Cursor CURSOR FOR "  + "  " +
                "SELECT Districts.id,District_Name FROM PITB.Districts" + "  " +
                "INNER JOIN PITB.Divisions ON Districts.Division_Id = Divisions.Id" + "  " +
                "INNER JOIN PITB.Provinces ON Divisions.Province_Id = Provinces.id" + "  " +
                "WHERE Districts.Is_Active = 1;" + "  " +

                "DECLARE @DepartmentId INT;" + "  " +
                "DECLARE @DepartmentName VARCHAR(50);" + "  " +
                "DECLARE Department_Cursor CURSOR FOR "  + "  " +
                "SELECT Id,Name FROM PITB.Department" + "  " +
                "WHERE Campaign_Id = 47 AND Id <=4;" + "  " +


                "INSERT INTO #Temp(DistrictId,DistrictName,DepartmentId,DepartmentName)" + "  " +
                "SELECT Districts.id,Districts.District_Name,Department.Id,Department.Name FROM PITB.Complaints" + "  " +
                "INNER JOIN PITB.Districts ON Districts.id = Complaints.District_Id" + "  " +
                "INNER JOIN PITB.Divisions ON Districts.Division_Id = Divisions.Id" + "  " +
                "INNER JOIN PITB.Provinces ON Divisions.Province_Id = Provinces.id" + "  " +
                "INNER JOIN PITB.Department ON Department.Id = Complaints.Department_Id" + "  " +
                "WHERE Compaign_Id = 47 AND Complaint_Type = 1 AND Department.Id <=4" + "  " +
                "GROUP BY Districts.id,Districts.District_Name,Department.Id,Department.Name"  + "  " +
                "ORDER BY Districts.id;" + "  " +
				

                "WHILE(@startMonthFirstDay <= @LastMonthLastDay)" + "  " +
                "BEGIN" + "  " +
                "SET @MonthName = DATENAME(month, CONVERT(datetime,@startMonthFirstDay,101));" + "  " +
	            "SET @Year = YEAR(@startMonthFirstDay);" + "  " +
	            "SET @ColumnName = @MonthName + '_' + CONVERT(VARCHAR(5),@Year);" + "  " +
	
	            "EXEC ('ALTER TABLE #Temp ADD '+ @ColumnName + ' VARCHAR(50);')" + "  " +
	            "DECLARE @startMonthLastDay datetime = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@startMonthFirstDay)+1,0));" + "  " +

	            "SET @startMonthFirstDay = CONVERT(datetime,@startMonthFirstDay,101);" + "  " +
	            "SET @startMonthLastDay = CONVERT(datetime,@startMonthLastDay,101);" + "  " +
	            "OPEN District_Cursor;  " + "  " +
	            "FETCH NEXT FROM District_Cursor INTO @DistrictId, @DistrictName;" + "  " + 
	            "WHILE @@FETCH_STATUS = 0  " + "  " +
	            "   BEGIN " + "  " +

	            "	   OPEN Department_Cursor; " + "  " + 
	            "		FETCH NEXT FROM Department_Cursor INTO @DepartmentId, @DepartmentName; " + "  " +
	            "		WHILE @@FETCH_STATUS = 0  " + "  " +
	            "		   BEGIN " + "  " +
	
	            "		    SET @sql = 'UPDATE  #Temp ' + " + "  " + 
	            "			'SET '+@ColumnName+' = (SELECT ' + " + "  " +
	            "			'CAST(ROUND(SUM(CASE WHEN C.Department_Id = '+CONVERT(VARCHAR(5),@DepartmentId) +' " + "  " +
                            "AND C.Created_Date >= '''+CONVERT(VARCHAR(30),@startMonthFirstDay,101)+''' " + "  " +
                            "AND C.Created_Date <= '''+ CONVERT(VARCHAR(30),@startMonthLastDay,101) + ''' THEN 1 ELSE 0 END)*1.0*100/" + "  " +
                            "NULLIF(SUM(CASE WHEN C.Department_Id IN(1,2,3,4) AND C.Created_Date >= '''+ CONVERT(VARCHAR(30),@startMonthFirstDay,101) +''' " + "  " +
                            "AND '+" + "  " +
	            "			'C.Created_Date <= '''+CONVERT(VARCHAR(30),@startMonthLastDay,101)+''' THEN 1 ELSE 0 END),0),2) " + "  " +
                "           as numeric(36,2))   '+" + "  " +
	            "			'FROM PITB.Complaints As C WHERE '+" + "  " + 
	            "			'C.Compaign_Id = 47 '+ " + "  " +
	            "			' AND C.Complaint_Type = 1 AND C.District_Id = '+ CONVERT(VARCHAR(5),@DistrictId) +" + "  " +
	            "			' AND C.Department_Id IN(1,2,3,4))' + " + "  " +
	            "			' WHERE DepartmentId = '+ CONVERT(VARCHAR(5),@DepartmentId) +" + "  " +
	            "			' AND DistrictId = '+ CONVERT(VARCHAR(5),@DistrictId) + ';'" + "  " +
	
	            "			PRINT @sql;" + "  " +
	            "			EXEC(@sql)			" + "  " +
		  
	            "			  FETCH NEXT FROM Department_Cursor INTO @DepartmentId, @DepartmentName;" + "  " + 
	            "		   END;  " + "  " +
	            "		CLOSE Department_Cursor;" + "  " +  

	            "	  FETCH NEXT FROM District_Cursor INTO @DistrictId, @DistrictName;" + "  " + 
	                "  END;  " + "  " +
	            "CLOSE District_Cursor;" + "  " +  
	            "SET @startMonthFirstDay = DATEADD(mm,1,@startMonthFirstDay)" + "  " +
                "END" + "  " +

                "SELECT * FROM #Temp;" + "  " +
                "DEALLOCATE District_Cursor;  " + "  " +
                "DEALLOCATE Department_Cursor;  " + " " + 
                "DROP TABLE #Temp;";
                DataTable lObjData = DBHelper.GetDataTableByQueryString(queryStr, null);

                if (lObjData != null && lObjData.Rows.Count > 0)
                {
                    lstResponse = new List<CategoryDistributionModel>();
                    
                    foreach (var district in lObjData.AsEnumerable().GroupBy(s=> s.Field<int>("DistrictId")))
                    {
                        CategoryDistributionModel districtModel = new CategoryDistributionModel();
                        districtModel.DistrictName = district.First().Field<string>("DistrictName");
                        districtModel.data = new List<dataModel>();
                        lstResponse.Add(districtModel);
                        foreach (DataRow row in district)
                        {
                            foreach (DataColumn column in row.Table.Columns)
                            {
                                if ((column.ColumnName.Contains("DistrictId") || column.ColumnName.Contains("DepartmentId") || column.ColumnName.Contains("DistrictName") || column.ColumnName.Contains("DepartmentName") || column.ColumnName.Contains("Id")))
                                {
                                    continue;
                                }
                                dataModel data = new dataModel();
                                data.DepartmentId = row.Field<int>("DepartmentId");
                                data.DistrictId = row.Field<int>("DistrictId");
                                data.DistrictName = row.Field<string>("DistrictName");
                                data.DepartmentName = row.Field<string>("DepartmentName");
                                data.MonthName = column.ColumnName;
                                data.MonthPercentage = row.Field<string>(column.ColumnName);
                                districtModel.data.Add(data);
                            }
                        }   
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstResponse;
        }
        private static void OrderByHierarchyList(ClosureAndTimelinessParameterModel parameter,List<ClosureAndTimelinessModel> model)
        {
            if (!parameter.OrderByParam.Equals("-1"))
            {
                if (!parameter.OrderBy.Equals("-1"))
                {
                    if (parameter.OrderBy.Equals("ASC"))
                    {
                        if (parameter.OrderByParam.Equals("Closurerate"))
                        {
                            ClosureComparer c = new ClosureComparer("ASC");
                            model.Sort(c);
                        }
                        else if (parameter.OrderByParam.Equals("Timeliness"))
                        {
                            TimelinessComparer c = new TimelinessComparer("ASC");
                            model.Sort(c);
                        }
                    }
                    else if (parameter.OrderBy.Equals("DESC"))
                    {
                        if (parameter.OrderByParam.Equals("Closurerate"))
                        {
                            ClosureComparer c = new ClosureComparer("DESC");
                            model.Sort(c);
                        }
                        else if (parameter.OrderByParam.Equals("Timeliness"))
                        {
                            TimelinessComparer c = new TimelinessComparer("DESC");                           
                            model.Sort(c);
                        }
                    }
                }                
            }
        }
        
        private static void OrderByListRecursive(List<ClosureAndTimelinessModel> lst, ClosureAndTimelinessParameterModel parameter)
        {
            if (lst != null)
            {
                OrderByHierarchyList(parameter, lst);
                foreach (ClosureAndTimelinessModel item in lst)
                {
                    OrderByListRecursive(item.HierarchyList, parameter);
                }

            }
        }
        private static void SetTopAndWorstQualityClosureAndTimeliness(ClosureAndTimelinessModel model)
        {
            if (model != null)
            {
                if (model.HierarchyList != null && model.HierarchyList.Count > 0)
                {
                    model.TopClosureRateHierarchyList = model.HierarchyList.OrderByDescending(x => x.ClosureRate).SkipWhile(x => x.ClosureRate == -1).TakeWhile(y=> y.ClosureRate != -1).Take(5).ToList<ClosureAndTimelinessModel>();
                    model.TopTimelinessHierarchyList = model.HierarchyList.OrderByDescending(x => x.Timeliness).SkipWhile(x => x.Timeliness == -1).TakeWhile(y => y.Timeliness != -1).Take(5).ToList<ClosureAndTimelinessModel>();
                    model.WorstClosureRateHierarchyList = model.HierarchyList.OrderBy(x => x.ClosureRate).SkipWhile(x => x.ClosureRate == -1).TakeWhile(y=> y.ClosureRate != -1).Take(5).ToList<ClosureAndTimelinessModel>();
                    model.WorstTimelinessHierarchyList = model.HierarchyList.OrderBy(x => x.Timeliness).SkipWhile(x => x.Timeliness == -1).TakeWhile(y => y.Timeliness != -1).Take(5).ToList<ClosureAndTimelinessModel>();
                    foreach (ClosureAndTimelinessModel item in model.HierarchyList)
                    {
                        SetTopAndWorstQualityClosureAndTimeliness(item);
                    }
                }
                
            }
        }
        private static void SetClosureAndTimelinessAggregateValues(string rowLabel,ClosureAndTimelinessAggregateModel model, IGrouping<int, DataRow> row)
        {
            model.RowLabel = row.First().Field<string>(rowLabel).ToString().TrimEndLine();
            model.RowId = row.First().Field<int>(string.Concat(rowLabel.Substring(0, rowLabel.Length - 4), "Id")).ToString().TrimEndLine();
            model.ClosedVerified = row.Sum(x => x.Field<int?>("currentMonthClosedVerified"));
            model.PendingReopened = row.Sum(x => x.Field<int?>("currentMonthPendingReopened"));
            model.PendingFresh = row.Sum(x => x.Field<int?>("currentMonthPendingFresh"));
            model.PendingOverdue = row.Sum(x => x.Field<int?>("currentMonthPendingOverdue"));
            model.ResolvedUnverified = row.Sum(x => x.Field<int?>("currentMonthResolvedUnverified"));
            model.ResolvedVerified = row.Sum(x => x.Field<int?>("currentMonthResolvedVerified"));
            model.GrandTotal = row.Sum(x => x.Field<int?>("currentMonthGrandTotal"));

            var currentMonthClosedVerified = row.Sum(x => x.Field<int?>("currentMonthClosedVerified"));
            var currentMonthPendingFresh = row.Sum(x => x.Field<int?>("currentMonthPendingFresh"));
            var currentMonthPendingReopened = row.Sum(x => x.Field<int?>("currentMonthPendingReopened"));
            var currentMonthPendingOverdue = row.Sum(x => x.Field<int?>("currentMonthPendingOverdue"));
            var currentMonthResolvedUnverified = row.Sum(x => x.Field<int?>("currentMonthResolvedUnverified"));
            var currentMonthResolvedVerified = row.Sum(x => x.Field<int?>("currentMonthResolvedVerified"));
            var currentMonthGrandTotal = row.Sum(x => x.Field<int?>("currentMonthGrandTotal"));


            var previousMonthClosedVerified = row.Sum(x => x.Field<int?>("previousMonthClosedVerified"));
            var previousMonthPendingFresh = row.Sum(x => x.Field<int?>("previousMonthPendingFresh"));
            var previousMonthPendingReopened = row.Sum(x => x.Field<int?>("previousMonthPendingReopened"));
            var previousMonthPendingOverdue = row.Sum(x => x.Field<int?>("previousMonthPendingOverdue"));
            var previousMonthResolvedUnverified = row.Sum(x => x.Field<int?>("previousMonthResolvedUnverified"));
            var previousMonthResolvedVerified = row.Sum(x => x.Field<int?>("previousMonthResolvedVerified"));
            var previousMonthGrandTotal = row.Sum(x => x.Field<int?>("previousMonthGrandTotal"));


            var nextMonthClosedVerified = row.Sum(x => x.Field<int?>("nextMonthClosedVerified"));
            var nextMonthPendingFresh = row.Sum(x => x.Field<int?>("nextMonthPendingFresh"));
            var nextMonthPendingReopened = row.Sum(x => x.Field<int?>("nextMonthPendingReopened"));
            var nextMonthPendingOverdue = row.Sum(x => x.Field<int?>("nextMonthPendingOverdue"));
            var nextMonthResolvedUnverified = row.Sum(x => x.Field<int?>("nextMonthResolvedUnverified"));
            var nextMonthResolvedVerified = row.Sum(x => x.Field<int?>("nextMonthResolvedVerified"));
            var nextMonthGrandTotal = row.Sum(x => x.Field<int?>("nextMonthGrandTotal"));


            if (currentMonthClosedVerified != null && currentMonthPendingReopened != null && ((currentMonthPendingReopened + currentMonthClosedVerified) > 0))
            {
                model.CurrentMonthAggregateClosureRate = Math.Ceiling((double)(currentMonthClosedVerified * 100 * 1.0 / (currentMonthClosedVerified + currentMonthPendingReopened)));
            }
            else
            {
                model.CurrentMonthAggregateClosureRate = 0;
            }
            if (currentMonthPendingFresh != null && currentMonthPendingOverdue != null && currentMonthGrandTotal != null && ((currentMonthGrandTotal - currentMonthPendingFresh) > 0))
            {
                model.CurrentMonthAggregateTimeliness = Math.Ceiling((double)(100 * (1 - (currentMonthPendingOverdue * 1.0 / (currentMonthGrandTotal + currentMonthPendingFresh)))));
            }
            else
            {
                model.CurrentMonthAggregateTimeliness = -1;
            }

            if (previousMonthClosedVerified != null && previousMonthPendingReopened != null && ((previousMonthPendingReopened + previousMonthClosedVerified) > 0))
            {
                model.PreviousMonthAggregateClosureRate = Math.Ceiling((double)(previousMonthClosedVerified * 100 * 1.0 / (previousMonthClosedVerified + previousMonthPendingReopened)));
            }
            else
            {
                model.PreviousMonthAggregateClosureRate = 0;
            }
            if (previousMonthPendingFresh != null && previousMonthPendingOverdue != null && previousMonthGrandTotal != null && ((previousMonthGrandTotal - previousMonthPendingFresh) > 0))
            {
                model.PreviousMonthAggregateTimeliness = Math.Ceiling((double)(100 * (1 - (previousMonthPendingOverdue * 1.0 / (previousMonthGrandTotal + previousMonthPendingFresh)))));
            }
            else
            {
                model.PreviousMonthAggregateTimeliness = -1;
            }

            if (nextMonthClosedVerified != null && nextMonthPendingReopened != null && ((nextMonthPendingReopened + nextMonthClosedVerified) > 0))
            {
                model.NextMonthAggregateClosureRate = Math.Ceiling((double)(nextMonthClosedVerified * 100 * 1.0 / (nextMonthClosedVerified + nextMonthPendingReopened)));
            }
            else
            {
                model.NextMonthAggregateClosureRate = 0;
            }
            if (nextMonthPendingFresh != null && nextMonthPendingOverdue != null && nextMonthGrandTotal != null && ((nextMonthGrandTotal - nextMonthPendingFresh) > 0))
            {
                model.NextMonthAggregateTimeliness = Math.Ceiling((double)(100 * (1 - (nextMonthPendingOverdue * 1.0 / (nextMonthGrandTotal + nextMonthPendingFresh)))));
            }
            else
            {
                model.NextMonthAggregateTimeliness = -1;
            }
        }
    }
}