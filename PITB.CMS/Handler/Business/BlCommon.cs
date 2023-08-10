using System.Data;
using System.Web.Http;
using Amazon.EC2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PITB.CMS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Helper;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Models.Custom;
using System.Web.Mvc;
using PITB.CMS.Models.Custom.DataTable;
using PITB.CMS.Handler.DataTableJquery;
using PITB.CMS.Handler.Export;
using PITB.CMS.Models.View;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Amazon.DynamoDBv2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Business;
using PITB.CMS.Handler.Complaint.Status;
using PITB.CMS.Handler.Export;
using PITB.CMS.Handler.Notification;
using PITB.CMS.Helper;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using System.Linq;
using PITB.CMS.Handler.DataTableJquery;
using PITB.CMS.Models.Custom.DataTable;
using System.Collections.Generic;
using System.Data;
using System;
using PITB.CMS.Models.View;
using PITB.CMS.Models.View.Message;
using PITB.CMS.Models.View.Select2;
using PITB.CMS.Models.View.Table;
using PITB.CMS.Handler.Complaint.Transfer;
using OfficeOpenXml;
using PreMailer.Net;
using System.Web.Configuration;
using System.IO;
using System.Diagnostics;
using PITB.CMS.Models.View.Reports;
using System.Dynamic;
using PITB.CMS.Handler.Data_Representation;
using System.Text;
using PITB.CMS.Helper.Attributes;
using System.Runtime.Serialization.Formatters.Binary;
using PITB.CMS.Models.Custom.CustomForm;

namespace PITB.CMS.Handler.Business
{
    public class BlCommon
    {
        public static int Export([FromBody]JToken jsonBody)
        {
            try
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
                int val = 2;
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
                else if (commaSeperatedCampaigns.Contains(((int)Config.Campaign.FixItLwmc).ToString()))
                {
                    DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(dict["aoData"]);
                    List<VmStakeholderComplaintListing> dataTable = BlLwmc.GetStakeHolderServerSideListDenormalized(
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
                        Convert.ToInt32(dict["userId"])).ToList<VmStakeholderComplaintListing>();
                    BlLwmc.GetComplaintAssignedToUser(dataTable);
            
                    Stopwatch timer = new Stopwatch();
                    timer.Start();        
                  //  BlLwmc.SetPictureData(dataTable,val);
                    timer.Stop();
                    //UtilityExtensions.WriteToFile(string.Format(GetSetPictureDataFunctionName(val) + " Time elapsed: {0:hh\\:mm\\:ss}", timer.Elapsed),"E:\\error.txt");
                    Debug.WriteLine(GetSetPictureDataFunctionName(val) + " Data Time elapsed: {0:hh\\:mm\\:ss}", timer.Elapsed);
                    //UtilityExtensions.WriteToFile(string.Format("TaskBased Request Time elapsed: {0:hh\\:mm\\:ss}", timer.Elapsed), "E:\\timer.txt");

                    data = dataTable.ToDataTableForReport("Report1");
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
                Stopwatch excelTimer = new Stopwatch();
                excelTimer.Start();
                ExcelPackage excelPack = FileHandler.ExportToExcel(data, "Complaint Listing Data");
                excelTimer.Stop();
               // UtilityExtensions.WriteToFile(string.Format(GetSetPictureDataFunctionName(val) + " ExcelTimer Time elapsed: {0:hh\\:mm\\:ss}", excelTimer.Elapsed), "E:\\error.txt");
                Debug.WriteLine(GetSetPictureDataFunctionName(val) + "Time elapsed: {0:hh\\:mm\\:ss}", excelTimer.Elapsed);
                string fileName = DbCampaign.GetById(Int32.Parse(commaSeperatedCampaigns.Split(',').First())).Campaign_Name;
                string startDate = dict["from"];
                string endDate = dict["to"];
                int dataId = DataStateMVC.AddInPool(excelPack,fileName,startDate,endDate);
                return dataId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static dynamic GetCategories(CustomForm.Post postForm)
        {
            int campaignId = int.Parse(postForm.GetElementValue("campaignId").ToString().Split(new string[] { "___" }, StringSplitOptions.None)[0]);
            int categoryLevel = int.Parse(postForm.GetElementValue("categoryLevel"));
            return GetCategories(campaignId, categoryLevel);
        }

        public static dynamic LinearizeCategories(dynamic d , int categoryLevel)
        {
            List<dynamic> listData = (List<dynamic>) d;
            string key=null, value=null;
            for (int i = 0; i < listData.Count; i++)
            {
                IDictionary<string, object> dictVal = listData[i];
                List<string> listDictKeys = dictVal.Keys.ToList();
                value = "";
                for (int j = 0; j < listDictKeys.Count; j++)
                {
                    if (listDictKeys[j].Contains("value"))
                    {
                        value = value + "["+ dictVal[listDictKeys[j]]+"] ";
                    }
                    if (listDictKeys[j].Contains("key" + categoryLevel))
                    {
                        key = dictVal[listDictKeys[j]].ToString();
                    }

                }
                dictVal.Add("key", key);
                dictVal.Add("value", value);
            }
            return d;
        }

        public static dynamic GetCategories(int campaignId, int categoryLevel)
        {
            dynamic resp = new ExpandoObject();
            resp.data = null;
            string concatStr = "";
            List<DbComplaintType> listDbComplaintType = null;
            List<DbDepartment> listDepartment = null;

            for (int i = categoryLevel; i>=0;  i--)
            {
                int? groupId = DbCategoryGroupMapping.GetModelByCampaignIdAndTypeId(campaignId, (Config.ComplaintType)1);
                if (i == 1) //Department
                {
                    listDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
                    if (resp.data == null)
                    {
                        //listDepartment = DbDepartment.GetByCampaignAndGroupId(campaignId, groupId);
                        resp.data = listDepartment.Select(x =>
                        {
                            dynamic e = new ExpandoObject();
                            e.key1 = x.Id;
                            e.ref1 = null;
                            e.value1 = x.Name;
                            return e;
                        }).ToList();
                    }
                    else
                    {
                        List<dynamic> listExpObj = resp.data;
                        for (int j = 0; j < listExpObj.Count; j++)
                        {
                            IDictionary<string, object> dictVal = listExpObj[j];
                            if (dictVal["ref" + (i + 1)] != null)
                            {
                                DbDepartment dbDepartment = listDepartment.Where(n => n.Id == (int.Parse(dictVal["ref" + (i + 1)].ToString()))).FirstOrDefault();
                                dictVal.Add("key" + i, int.Parse(dictVal["ref" + (i + 1)].ToString()));
                                dictVal.Add("value" + i, dbDepartment.Name);
                                dictVal.Add("ref" + i, null);
                            }
                        }
                    }
                }
                else if (i == 2) //Category
                {
                    listDbComplaintType = DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId);
                    if (resp.data == null)
                    {
                        resp.data = listDbComplaintType.Select(x =>
                        {
                            dynamic e = new ExpandoObject();
                            e.key2 = x.Complaint_Category;
                            e.ref2 = x.DepartmentId;
                            e.value2 = x.Name;
                            return  e;
                        }).ToList();
                    }
                    else
                    {
                        List<dynamic> listExpObj = resp.data;
                        for (int j=0; j<listExpObj.Count; j++)
                        {
                            IDictionary<string,object> dictVal =  listExpObj[j];
                            if (dictVal["ref" + (i + 1)] != null)
                            {
                                DbComplaintType dbComplaintType = listDbComplaintType.Where(n => n.Complaint_Category == (int.Parse(dictVal["ref" + (i + 1)].ToString()))).FirstOrDefault();
                                dictVal.Add("key" + i, int.Parse(dictVal["ref" + (i + 1)].ToString()));
                                dictVal.Add("value" + i, dbComplaintType.Name);
                                dictVal.Add("ref" + i, dbComplaintType.DepartmentId);
                            }
                        }
                    }
                }
                else if (i == 3) //Category
                {

                    List<DbComplaintSubType> listComplaintSubtype = DbComplaintSubType.GetByComplaintTypes(DbComplaintType.GetByCampaignIdAndGroupId(campaignId, groupId).Select(n=>n.Complaint_Category).ToList());
                    resp.data = listComplaintSubtype.Select(x =>
                    {
                        dynamic e = new ExpandoObject();
                        e.key3 = x.Complaint_SubCategory;
                        e.ref3 = x.Complaint_Type_Id;
                        e.value3 = x.Name;
                        return e;
                    }).ToList();
                }
            }
            LinearizeCategories(resp.data, categoryLevel);
            return resp;
        }

        private static void StartPopulatingCategories(dynamic d, int categoryLevel )
        {
            for (int i = categoryLevel; i >= 0; i--)
            {

            }

        }

        private static string GetSetPictureDataFunctionName(int val)
        {
            string FunctionName = string.Empty;
            switch (val)
            {
                case 1:
                    FunctionName = "GetSynchronousData()";
                    break;
                case 2:
                    FunctionName = "GetSynchronousDelegateData()";
                    break;
                case 3:
                    FunctionName = "GetAsynchronousDelegateData()";
                    break;
                case 4:
                    FunctionName = "GetMultiThreadedData()";
                    break;
                case 5:
                    FunctionName = "GetThreadPoolData()";
                    break;
                case 6:
                    FunctionName = "GetTaskBasedData()";
                    break;
                case 7:
                    FunctionName = "GetDataParallelismBasedData()";
                    break;
                case 8:
                    FunctionName = "GetAwaitAsuncBasedData()";
                    break;
                case 9:
                    FunctionName = "GetStepWaitAsyncDelegateData()";
                    break;
                default:
                    FunctionName = "GetSynchronousData()";
                    break;
            }
            return FunctionName;
        }
        /*public static object GetUserCategoriesAgainstCampaign(string[] campaignIds)
        {
            //var jsonToReturn = from sb in DbComplaintType.GetByCampaignIds(campaignIds.ToIntList()).OrderBy(m => m.Name)
            //                   select new { Value = sb.Complaint_Category, Text = sb.Name };

            List<DbComplaintType> listComplaintType = DbComplaintType.GetByCampaignIds(campaignIds.ToIntList());
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            listComplaintType.RemoveAll(n=>!cookie.CategoryIds.Contains(n.Complaint_Category));
            //cookie.CategoryIds.RemoveAll(n=>)
            
            var valueTextPair = from sb in listComplaintType
                               select new { Value = sb.Complaint_Category, Text = sb.Name };
            return valueTextPair;
        }*/

        /*
        public static List<Pair<string,List<SelectList>,string>> GetDropDownListUnderHierarchy(DbUsers dbUsers, List<int> listHiearchy )
        {
            List<Tuple<int, string>> listHierarchyValuePair =
                Utility.GetHierarchyMappingListByUser(DbUsers.GetUser(AuthenticationHandler.GetCookie().UserId),
                    (int)dbUsers.Hierarchy_Id);

            listHierarchyValuePair = listHierarchyValuePair.Where(n => listHiearchy.Contains((int) n.Item1)).ToList();

            //listHierarchyValuePair
            Tuple<int, string> tupleValue = listHierarchyValuePair[0];

            List<Pair<string,List<SelectList>,string>> listDropdown = new List<Pair<string, List<SelectList>, string>>();

            if (tupleValue.Item1 == (int)Config.Hierarchy.Province)
            {
               
               DbProvince.GetById(Convert.ToInt32(tupleValue.Item2.Split(',').First()));
            }

            listHiearchy = listHiearchy.OrderBy(n => n).ToList();
            for (int i = 0; i < listHiearchy.Count; i++)
            {
                int hierarchyId = (int)dbUser.Hierarchy_Id;
                if(hierarchyId==(int)Config.Hierarchy.District)
                dbUser
            }


        

        }
        */
        public static KeyValuePair<int, int[]>[] GetStatusListPermissionForCampaign(int campaignId)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbPermissionsAssignment dbPermission = cookie.ListCampaignPermissions.Where(
               n =>
                   n.Type == (int)Config.PermissionsType.Campaign &&
                   n.Type_Id == campaignId &&
                   n.Permission_Id == (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap
                   ).FirstOrDefault();
            if (dbPermission != null)
            {
                string value = dbPermission.Permission_Value;

                string[] data = value.Split(new char[] { ';' });

                KeyValuePair<int, int[]>[] arr = new KeyValuePair<int, int[]>[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    string key = data[i].Split(new char[] { ':' }).First();
                    string valuepair = data[i].Split(new char[] { ':' }).Last();
                    arr[i] = new KeyValuePair<int, int[]>(Int32.Parse(key), valuepair.Split(',').ToIntList().ToArray());
                }
                return arr;
            }
            return null;
        }
        public static string GetCampaignIdsFromPermissionAssingment(Config.PermissionsType permissionsType, Config.Permissions permission)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbPermissionsAssignment dbPermission = cookie.ListPermissions.Where(
                n =>
                    n.Type == (int)permissionsType &&
                    n.Type_Id == cookie.UserId &&
                    n.Permission_Id == (int)permission
                    ).FirstOrDefault();
            if (dbPermission != null)
            {
                return dbPermission.Permission_Value;
                //string value = dbPermission.Permission_Value;
                //string campaignIds = value;
                //return campaignIds;
            }

            return null;
        }
        public static List<DbPermissionsAssignment> GetCampaignIdsFromPermissionAssingment(Config.PermissionsType permissionsType, List<int?> listPermissionTypeId, Config.CampaignPermissions permission)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbPermissionsAssignment> listDbPermission = new List<DbPermissionsAssignment>();
            if (cookie.ListCampaignPermissions != null)
            {
                listDbPermission = cookie.ListCampaignPermissions.Where(
                n =>
                    n.Type == (int)permissionsType &&
                    listPermissionTypeId.Contains(n.Type_Id) &&
                    n.Permission_Id == (int)permission
                    ).ToList();
            }
            return listDbPermission;
        }
        /*public static KeyValuePair<int, int[]>[] GetCampaignMergerIdsFromPermissionAssignment(Config.Permissions permission)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbPermissionsAssignment dbPermission = cookie.ListCampaignPermissions.Where(
                n =>
                    n.Type == (int)Config.PermissionsType.User &&
                    n.Type_Id == cookie.UserId &&
                    n.Permission_Id == (int)permission
                    ).FirstOrDefault();
            
            if (dbPermission != null)
            {
                string value = dbPermission.Permission_Value;
                string [] data = value.Split(new char[]{';'});
                KeyValuePair<int, int[]>[] arr = new KeyValuePair<int, int[]>[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    string key = data[i].Split(new char[] { ':' }).First();
                    string valuepair = data[i].Split(new char[] { ':' }).Last();
                    arr[i] = new KeyValuePair<int, int[]>(Int32.Parse(key), valuepair.Split(',').ToIntList().ToArray()); 
                }
                return arr;                
            }
            return null;
        }*/
        public static List<SelectListItem> GetStatusListByCampaignIds(List<int> listCampaignIds, Config.Permissions userPermissions)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            DbPermissionsAssignment dbPermission = cookie.ListPermissions.Where(
                n =>
                    n.Type == (int)Config.PermissionsType.User &&
                    n.Type_Id == cookie.UserId &&
                    n.Permission_Id == (int)userPermissions
                    ).FirstOrDefault();

            List<int> listStatuses = null;
            List<DbStatus> statusList = null;

            if (dbPermission != null)
            {
                listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                statusList = DbStatus.GetByStatusIds(listStatuses);
            }
            else
            {
                statusList = DbStatus.GetByCampaignIds(listCampaignIds);
            }



            statusList.Add(new Models.DB.DbStatus() { Complaint_Status_Id = -1, Status = "Total" });

            return statusList.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
            /*
            return new List<SelectListItem>(){
                new SelectListItem { Text = "Resolved", Value = ""+Convert.ToInt32(Config.ComplaintStatus.ResolvedUnverified)},
                new SelectListItem { Text = "Unresolved", Value = ""+Convert.ToInt32(Config.ComplaintStatus.PendingFresh) }
            };
           */
        }

        public static List<SelectListItem> GetStatusListByCampaignIds(List<int> listCampaignIds, DbUsers dbUser)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<DbPermissionsAssignment> listPermissionsAssignment = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.User,
                (int)dbUser.User_Id, (int)Config.Permissions.StatusesForComplaintListing);
            DbPermissionsAssignment dbPermission = listPermissionsAssignment.Where(
                n =>
                    n.Type == (int)Config.PermissionsType.User &&
                    n.Type_Id == dbUser.User_Id &&
                    n.Permission_Id == (int)Config.Permissions.StatusesForComplaintListing
                    ).FirstOrDefault();

            List<int> listStatuses = null;
            List<DbStatus> statusList = null;

            if (dbPermission != null)
            {
                listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                statusList = DbStatus.GetByStatusIds(listStatuses);
            }
            else
            {
                statusList = DbStatus.GetByCampaignIds(listCampaignIds);
            }





            return statusList.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
            /*
            return new List<SelectListItem>(){
                new SelectListItem { Text = "Resolved", Value = ""+Convert.ToInt32(Config.ComplaintStatus.ResolvedUnverified)},
                new SelectListItem { Text = "Unresolved", Value = ""+Convert.ToInt32(Config.ComplaintStatus.PendingFresh) }
            };
           */
        }

        public static List<DbStatus> GetStatusListByCampaignIdsAndPermissions(List<int> listCampaignIds, int userId, List<DbPermissionsAssignment> listPermissions)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();

            DbPermissionsAssignment dbPermission = listPermissions.Where(
                n =>
                    n.Type == (int)Config.PermissionsType.User &&
                    n.Type_Id == userId &&
                    n.Permission_Id == (int)Config.Permissions.StatusesForComplaintListing
                    ).FirstOrDefault();

            List<int> listStatuses = null;
            List<DbStatus> statusList = null;

            if (dbPermission != null)
            {
                listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                statusList = DbStatus.GetByStatusIds(listStatuses);
            }
            else
            {
                statusList = DbStatus.GetByCampaignIds(listCampaignIds);
            }

            //return statusList.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
            return statusList;
        }

        public static List<DbStatus> GetStatusListByCampaignIdsAndPermissions(List<int> listCampaignIds, int userId, List<DbPermissionsAssignment> listPermissions, Config.Permissions permission)
        {
            //CMSCookie cookie = AuthenticationHandler.GetCookie();

            DbPermissionsAssignment dbPermission = listPermissions.Where(
                n =>
                    n.Type == (int)Config.PermissionsType.User &&
                    n.Type_Id == userId &&
                    n.Permission_Id == (int)permission
                    ).FirstOrDefault();

            List<int> listStatuses = null;
            List<DbStatus> statusList = null;

            if (dbPermission != null)
            {
                listStatuses = Utility.GetIntList(dbPermission.Permission_Value);
                statusList = DbStatus.GetByStatusIds(listStatuses);
            }
            else
            {
                statusList = DbStatus.GetByCampaignIds(listCampaignIds);
            }

            //return statusList.Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
            return statusList;
        }

        public static string GetStatusStrCommaSepByCampaignId(string campaignIds)
        {
            List<int> statusList = DbStatus.GetByCampaignId(Utility.GetIntByCommaSepStr(campaignIds)).Select(n => n.Complaint_Status_Id).ToList();
            return string.Join<int>(",", statusList);
        }

        public static List<DbStatus> GetStatusesForAgainstCampaign(Config.Campaign campaign)
        {
            List<DbStatus> listDbStauses = null;
            if (Config.CampaignWiseStatusDict.ContainsKey(campaign))
            {
                listDbStauses = DbStatus.GetByStatusIds(Config.CampaignWiseStatusDict[campaign]);
            }
            else
            {
                listDbStauses = DbStatus.GetByCampaignId((int)campaign);
            }
            return listDbStauses;
        }
    }
}