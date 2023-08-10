using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Security;
using System.Net;
using System.Xml;
using System.IO;
using PITB.CMS.Models.DB;
using System.Data;
using System.Web.Mvc;
using PITB.CMS.Models.View.Data_Representation;
using System.Collections;
using System.Diagnostics;
using PITB.CMS.Models.DB;
using PITB.CMS.Helper.Database;

namespace PITB.CMS.Handler.Data_Representation
{
    public class BlDataRepresentation
    {
        public static string filePath = "";
        public static string GetHierarchyReponseTimeData(int hierarchyId, string campaignId,int escalationId,  DateTime startDate, DateTime endDate)
        {
            try
            {
                File.AppendAllText(filePath, "Line 1\n");
            }
            catch (Exception ex)
            {
                
            }
            string connStr = ConfigurationManager.ConnectionStrings["PITB.CMS"].ConnectionString;
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder(connStr);
            NetworkCredential netCred = new NetworkCredential(connBuilder.UserID, connBuilder.Password);
            netCred.SecurePassword.MakeReadOnly();
            SecureString securePwd = new SecureString();
            char[] pwArr = connBuilder.Password.ToCharArray();
            for(int i=0;i<pwArr.Length;i++){
                securePwd.AppendChar(pwArr[i]);
            }
            File.AppendAllText(filePath, "Line 2\n");
            securePwd.MakeReadOnly();
            SqlCredential sqlCred = new SqlCredential(connBuilder.UserID, securePwd);
            connBuilder.Remove("User Id");
            connBuilder.Remove("Password");
            SqlConnection conn = new SqlConnection(connBuilder.ConnectionString, sqlCred);
            conn.Open();
            DbPermissionsAssignment permission = null;
            string permissionValue = null;
            File.AppendAllText(filePath, "Line 3\n");
            if (campaignId.Split(new char[] { ',' }).Length > 1)
            {
                string[] arrCampId = campaignId.Split(new char[]{','});
                for (int i = 0; i < arrCampId.Length; i++)
                {
                    permission = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, Int16.Parse(arrCampId[i]), (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap).FirstOrDefault();
                    if(permission != null){
                        permissionValue += permission.Permission_Value + "__";
                    }      
                }
                char[] charsToTrim = {'_'};
                permissionValue = permissionValue.Trim(charsToTrim);
                File.AppendAllText(filePath, "Line 4\n");
            }
            else
            {
                permission = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, int.Parse(campaignId), (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap).FirstOrDefault();
                permissionValue = permission.Permission_Value;
                File.AppendAllText(filePath, "Line 5\n");
            }
            File.AppendAllText(filePath, "Line 6\n");
            Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(permissionValue);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "UPDATE PITB.Complaints SET ResponseTime = ROUND(100-CAST(DATEDIFF(DAY,Created_Date,StatusChangedDate_Time) AS FLOAT)/NULLIF(DATEDIFF(DAY,Created_Date,MaxSrcIdDate),0),2) " +
                              " WHERE Compaign_Id IN (" + campaignId + ") AND Complaint_Computed_Status_Id IN (" + dict[Config.StatusDict.First(x => x.Value == "Resolved").Key.ToString()] + "); " +
                              " SELECT Province_Id,Province_Name,Division_Id,Division_Name,District_Id,District_Name,Tehsil_Id,Tehsil_Name,UnionCouncil_Id,UnionCouncil_Name,Ward_Id,Ward_Name,Complaint_Computed_Status_Id,Complaint_Computed_Status,Created_Date,StatusChangedDate_Time,MaxSrcIdDate,ResponseTime,Complaint_Computed_Hierarchy_Id,Complaint_Computed_Hierarchy FROM PITB.Complaints WHERE Complaint_Type = 1 AND Compaign_Id IN (" + campaignId + ") AND CONVERT(DATE,Created_Date) >= CONVERT(DATE,'" + startDate.Date.ToString("MM/dd/yyyy") + "') AND CONVERT(DATE,Created_Date) <= CONVERT(DATE,'" + endDate.Date.ToString("MM/dd/yyyy") + "')";
            cmd.Connection = conn;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            File.AppendAllText(filePath, "Line 7\n");
            DataTable dt = new DataTable("PITB.Complaints");
            adapter.Fill(dt);
            File.AppendAllText(filePath, "Line 8\n");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            settings.CloseOutput = true;
            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, settings);
            writer.WriteStartDocument();
            File.AppendAllText(filePath, "Line 9\n");
            writer.WriteStartElement("hs","HierarchyList","PITB.CMS");
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0 && dict != null && dict.Keys.Count>0)
            {
                int i=0;
                File.AppendAllText(filePath, "Line 10\n");
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Created_Date"] != null && row["StatusChangedDate_Time"] != null && row["Created_Date"] != System.DBNull.Value && row["StatusChangedDate_Time"] != System.DBNull.Value)
                    {
                        row["ResponseTime"] = (double?)(((DateTime)row["StatusChangedDate_Time"]).Ticks - ((DateTime)row["Created_Date"]).Ticks);
                    }
                    else
                    {
                        row["ResponseTime"] = 0;
                    }
                    Debug.WriteLine(String.Format("i - {0}, responseTime - {1}, length - {2}", i++, row["ResponseTime"], row["ResponseTime"].ToString().Length));
                }
                i = 0;
                Debug.WriteLine("Order By List");
                Array.ForEach(dt.Rows.Cast<DataRow>().OrderBy(x => x.Field<double?>("ResponseTime")).ToArray<DataRow>(), row => Debug.WriteLine(String.Format("x - {0}, responseTime - {1}, length - {2}", i++, row["ResponseTime"], row["ResponseTime"].ToString().Length)));
                
                int pending = 0;
                int unresolved = 0;
                int resolved = 0;
                int total = 0;
                string responseTime = "";
                string str_hierarchyId = null;
                string str_hierarchyName = null;
                EnumerableRowCollection<DataRow> rowsCol = null;
                if (escalationId == -1)
                {
                    rowsCol = (EnumerableRowCollection<DataRow>)dt.AsEnumerable();
                }
                else
                {
                    rowsCol = (EnumerableRowCollection<DataRow>)dt.AsEnumerable().Where(x => x.Field<int>("Complaint_Computed_Hierarchy_Id") == escalationId);
                }
                
                switch (hierarchyId)
                {
                    case (int)Config.Hierarchy.Province:
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("Province_Id")).OrderBy(x => x.Average(z => z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("Province_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("Province_Name")) ? null : item.First().Field<string>("Province_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "Province", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    case (int)Config.Hierarchy.Division:
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("Division_Id")).OrderBy(x=> x.Average(z=> z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("Division_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("Division_Name")) ? null : item.First().Field<string>("Division_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "Division", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    case (int)Config.Hierarchy.District:
                        rowsCol.OrderBy(x => x.Field<string>("District_Name"));
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("District_Id")).OrderBy(x => x.Average(z => z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("District_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("District_Name")) ? null : item.First().Field<string>("District_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "District", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    case (int)Config.Hierarchy.Tehsil:
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("Tehsil_Id")).OrderBy(x => x.Average(z => z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("Tehsil_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("Tehsil_Name")) ? null : item.First().Field<string>("Tehsil_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "Tehsil", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    case (int)Config.Hierarchy.UnionCouncil:
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("UnionCouncil_Id")).OrderBy(x => x.Average(z => z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("UnionCouncil_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("UnionCouncil_Name")) ? null: item.First().Field<string>("UnionCouncil_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "UnionCouncil", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    case (int)Config.Hierarchy.Ward:
                        foreach (var item in rowsCol.GroupBy(x => x.Field<int>("Ward_Id")).OrderBy(x => x.Average(z => z.Field<double?>("ResponseTime"))))
                        {
                            str_hierarchyId = item.First().Field<int>("Ward_Id").ToString();
                            str_hierarchyName = string.IsNullOrWhiteSpace(item.First().Field<string>("Ward_Name")) ? null : item.First().Field<string>("Ward_Name").ToString();
                            pending = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Pending").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            unresolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Unresolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            resolved = item.Where(x => dict[Config.StatusDict.First(y => y.Value == "Resolved").Key.ToString()].Split(new char[] { ',' }).Contains(x.Field<int>("Complaint_Computed_Status_Id").ToString())).Count();
                            total = item.Count();
                            responseTime = GetResponseTimeFormat((long)item.Average(x => x.Field<double?>("ResponseTime")));
                            XMlWriterFunction(writer, str_hierarchyId, str_hierarchyName, "Ward", pending, unresolved, resolved, total, responseTime);
                        }
                        break;
                    default:
                        break;
                }
                File.AppendAllText(filePath, "Line 11\n");
            }
            File.AppendAllText(filePath, "Line 12\n");
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            conn.Close();
            conn.Dispose();
            File.AppendAllText(filePath, "Line 13\n");
            return sw.ToString();
        }

        public static void XMlWriterFunction(XmlWriter writer, string hierarchyId, string HierachyName, string HierarchyHeading, int pending, int unresolved, int resolved, int total, string responseTime)
        {
            if (responseTime == null)
            {
                responseTime = "";
            }
            else
            {
                //responseTime = Math.Round(Convert.ToDouble(responseTime), 2, MidpointRounding.AwayFromZero);
            }
            double resolutionPercentage = Math.Ceiling((double)(resolved*1.0/total)*100);
            writer.WriteStartElement("hs", "Hierarchy", "PITB.CMS");
            writer.WriteElementString("hs", "HierarchyHeading", "PITB.CMS", HierarchyHeading);
            writer.WriteElementString("hs", "HierarchyId", "PITB.CMS", hierarchyId);
            writer.WriteElementString("hs", "HierarchyName", "PITB.CMS", HierachyName);
            writer.WriteElementString("hs", "Pending", "PITB.CMS", pending.ToString());
            writer.WriteElementString("hs", "Overdue", "PITB.CMS", unresolved.ToString());
            writer.WriteElementString("hs", "Resolved", "PITB.CMS", resolved.ToString());
            writer.WriteElementString("hs", "ResolutionPercentage", "PITB.CMS", resolutionPercentage.ToString());
            writer.WriteElementString("hs", "ResponseTime", "PITB.CMS", responseTime.ToString());
            writer.WriteElementString("hs", "Total", "PITB.CMS", total.ToString());
            writer.WriteEndElement();
        }
        public static string GetResponseTimeFormat(long ticks)
        {
            TimeSpan t1 = new TimeSpan(ticks);
            string responseTime = null;
            string responseTimeddhh = null;
            int days = t1.Days;
            int hours = t1.Hours;
            if (t1.Days > 0)
            {
                responseTime += t1.Days + " Days ";
            }
            if (t1.Hours > 0)
            {
                responseTime += t1.Hours + " Hours ";
            }
            responseTimeddhh = t1.Days + ":" + t1.Hours;
            if (t1.Days == 0 && t1.Hours == 0)
            {
                responseTime += "Unresolved";
            }
            return responseTime;
        }
        public static IEnumerable<IGrouping<int,DataRow>> GetEnumerator(DataTable dt,Config.Hierarchy hierarchy)
        {
            IEnumerable<IGrouping<int,DataRow>> response = null;
            switch (hierarchy)
            {
                case Config.Hierarchy.Province:
                    dt.AsEnumerable().OrderBy(x => x.Field<string>("Province_Name"));
                    response = (IEnumerable<IGrouping<int,DataRow>>)dt.AsEnumerable().GroupBy(x=> x.Field<int?>("Province_Id"));
                break;
                case Config.Hierarchy.Division:
                dt.AsEnumerable().OrderBy(x => x.Field<string>("Division_Name"));
                response = (IEnumerable<IGrouping<int, DataRow>>)dt.AsEnumerable().GroupBy(x => x.Field<int?>("Division_Id"));
                break;
                case Config.Hierarchy.District:
                dt.AsEnumerable().OrderBy(x => x.Field<string>("District_Name"));
                response = (IEnumerable<IGrouping<int, DataRow>>)dt.AsEnumerable().GroupBy(x => x.Field<int?>("District_Id"));
                break;
                case Config.Hierarchy.Tehsil:
                dt.AsEnumerable().OrderBy(x => x.Field<string>("Tehsil_Name"));
                response = (IEnumerable<IGrouping<int, DataRow>>)dt.AsEnumerable().GroupBy(x => x.Field<int?>("Tehsil_Id"));
                break;
                case Config.Hierarchy.UnionCouncil:
                dt.AsEnumerable().OrderBy(x => x.Field<string>("UnionCouncil_Name"));
                response = (IEnumerable<IGrouping<int, DataRow>>)dt.AsEnumerable().GroupBy(x => x.Field<int?>("UnionCouncil_Id"));
                break;
                case Config.Hierarchy.Ward:
                dt.AsEnumerable().OrderBy(x => x.Field<string>("Ward_Name"));
                response = (IEnumerable<IGrouping<int, DataRow>>)dt.AsEnumerable().GroupBy(x => x.Field<int?>("Ward_Id"));
                break;
            }
            return response;
        }
        public static VmResponseTime GetParametersForResponseTimeReport()
        {
            var response = new VmResponseTime();
            response.CampaignList = GetCampaignsList();
            response.HierarchyList = GetHierarchyList();
            response.EscalationLevelList = GetEscalationLevelList();
            return response;
        }
        public static VmAreawiseCategoryModel GetParametersForAreawiseCategoryReport()
        {
            var response = new VmAreawiseCategoryModel();
            response.CampaignList = GetCampaignsList();
            response.StatusLevelList = GetStatusList();
            return response;
        }
        public static IEnumerable<SelectListItem> GetCampaignsList()
        {
            string defaultCampaign = string.Empty;
            IList<SelectListItem> Campaignlist = new List<SelectListItem>() { 
                new SelectListItem()
                {
                    Text = "Health",
                    Value = "68,69,72,73,74"
                },
                new SelectListItem()
                {
                    Text = "Education",
                    Value = "47"
                },
                new SelectListItem()
                {
                    Text = "DC-Office",
                    Value = "70",
                    Selected = true
                },
                new SelectListItem()
                {
                    Text = "Police",
                    Value = "78",
                    Selected = true
                }
            };
            var campaigntip = new SelectListItem()
            {
                Value = "",
                Text = "--- select campaign ---"
            };
            defaultCampaign = "70";
            Campaignlist.Insert(0, campaigntip);
            return new SelectList(Campaignlist, "Value", "Text", defaultCampaign);
        }
        public static IEnumerable<SelectListItem> GetHierarchyList()
        {
            IList<SelectListItem> HierarchyList = new List<SelectListItem>() {
            new SelectListItem(){
                Value = null,
                Text = " " 
                } 
            };
            return HierarchyList;
        }
        public static IEnumerable<SelectListItem> GetStatusList()
        {
            IList<SelectListItem> StatusList = new List<SelectListItem>() {
            new SelectListItem(){
                Value = "",
                Text = "--- select status ---" 
                } 
            };
            return StatusList;
        }
        public static IEnumerable<SelectListItem> GetStatusList(string campaignText)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            string defaultStatus = string.Empty;
            SelectListItem item = GetCampaignsList().Single(x => x.Text.Equals(campaignText));
            string[] campaignIds = item.Value.Split(',');
            List<DbStatus> statuses = new List<Models.DB.DbStatus>();
            using(DBContextHelperLinq db = new DBContextHelperLinq()){
                foreach(var status in db.DbStatuses){
                    string[] statusCampaignIds = status.Campaigns.Split(',');
                    bool flag = false;
                    for (int i = 0; i < campaignIds.Length; i++)
                    {
                        for (int j = 0; j < statusCampaignIds.Length; j++)
                        {
                            if (campaignIds[i] == statusCampaignIds[j])
                            {
                                flag = true;
                            }
                        }
                    }
                    if (flag == true)
                    {
                        statuses.Add(status);
                    }
                }
            }
            for (int k = 0; k < statuses.Count; k++)
            {
                SelectListItem select = new SelectListItem() { Text = statuses[k].Status, Value = statuses[k].Complaint_Status_Id.ToString() };
                list.Add(select);
            }
            
            
            var statustip = new SelectListItem()
            {
                Text = "--- select status ---",
                Value = "",
            };
            list.Insert(0, statustip);
            return new SelectList(list, "Value", "Text", defaultStatus);
        }
        public static IEnumerable<SelectListItem> GetEscalationLevelList()
        {
            IList<SelectListItem> EscalationList = new List<SelectListItem>() {
            new SelectListItem(){
                Value = null,
                Text = " " 
                } 
            };
            return EscalationList;
        }
        public static IEnumerable<SelectListItem> GetHierarchyList(string campaignText)
        {
            IList<SelectListItem> list = null;
            string defaultHierarchy = string.Empty;
            switch (campaignText)
            {
                case "Health":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value="1",
                            Text="Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District",
                            Selected=true
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "UnionCouncil"
                        }
                    };
                    defaultHierarchy = "3";
                    break;
                case "Police":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value="1",
                            Text="Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District",
                            Selected=true
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "UnionCouncil"
                        }
                    };
                    defaultHierarchy = "2";
                    break;
                case "Education":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value="1",
                            Text="Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District",
                            Selected=true
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "Markaz"
                        }
                    };
                    defaultHierarchy = "4";
                    break;
                case "DC-Office":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value="1",
                            Text="Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District",
                            Selected=true
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                    };
                    defaultHierarchy = "3";
                    break;
                default:
                    list = new List<SelectListItem>();
                    break;
            }
            var hierarchytip = new SelectListItem()
            {
                Text = "--- select Hierarchy ---",
                Value = "",
            };
            list.Insert(0, hierarchytip);
            return new SelectList(list, "Value", "Text",defaultHierarchy);
        }
        public static IEnumerable<SelectListItem> GetEscalationLevelList(string campaignText)
        {
            IList<SelectListItem> list = null;
            string defaultEscalation = string.Empty;
            switch (campaignText)
            {
                case "Health":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value = "-1",
                            Text = "All",
                            Selected = true
                        },
                        //new SelectListItem(){
                        //    Value = "0",
                        //    Text = "Exceeded",
                        //},
                        new SelectListItem(){
                            Value = "1",
                            Text = "Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District"
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "UnionCouncil"
                        }
                    };
                    defaultEscalation = "3";
                    break;
                case "Police":
                    list = new List<SelectListItem>{
                        new SelectListItem(){
                            Value = "-1",
                            Text = "All",
                            Selected = true
                        },
                        //new SelectListItem(){
                        //    Value = "0",
                        //    Text = "Exceeded",
                        //},
                        new SelectListItem(){
                            Value = "1",
                            Text = "Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District"
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "UnionCouncil"
                        }
                    };
                    defaultEscalation = "2";
                    break;
                case "Education":
                    list = new List<SelectListItem>{
                         new SelectListItem(){
                            Value = "-1",
                            Text = "All",
                            Selected = true
                        },
                        //new SelectListItem(){
                        //    Value = "0",
                        //    Text = "Exceeded",
                        //},
                        new SelectListItem(){
                            Value = "1",
                            Text = "Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District"
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                        new SelectListItem(){
                            Value = "5",
                            Text = "UnionCouncil"
                        }
                    };
                    defaultEscalation = "3";
                    break;
                case "DC-Office":
                    list = new List<SelectListItem>{
                         new SelectListItem(){
                            Value = "-1",
                            Text = "All",
                            Selected = true
                        },
                        //new SelectListItem(){
                        //    Value = "0",
                        //    Text = "Exceeded",
                        //},
                        new SelectListItem(){
                            Value = "1",
                            Text = "Province"
                        },
                        new SelectListItem(){
                            Value = "2",
                            Text = "Division"
                        },
                        new SelectListItem(){
                            Value = "3",
                            Text = "District"
                        },
                        new SelectListItem(){
                            Value = "4",
                            Text = "Tehsil"
                        },
                    };
                    defaultEscalation = "3";
                    break;
                default:
                    list = new List<SelectListItem>();
                    break;
            }
            var hierarchytip = new SelectListItem()
            {
                Text = "--- select escalation level---",
                Value = "",
            };
            list.Insert(0, hierarchytip);
            return new SelectList(list, "Value", "Text",defaultEscalation);
        }
    
    }
}