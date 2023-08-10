using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PITB.CMS_Common.Handler.API;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View.Executive;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Helper;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.ApiModels.Request;
using PITB.CMS_Common.Models.ApiModels.Response;
using System.Dynamic;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Configuration;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlExecutive
    {
        public static string filepath = "";
        public static string ErrorFilePath = "";
        public static string ReponsePath = "";
        public static VmDashboard GetDashboardModel(string startDate, string endDate)
        {
            VmDashboard response = new VmDashboard();
            DbUsers user = Utility.GetUserFromCookie();
            if (user == null)
                return null;
            response.Name = user.Name;
            response.Designation = user.Designation;
            response.ProvinceName = "Punjab";
            //string campaignIds = "1,2,3,4,5,7,9,10,11,12,15,18,19,20,21,22,23,24,29,33,35,36,39,43,47,49,50,54,55,56,57,58,59,60,63";
            //string zimmedarAndShcoolEducation = "1,47";
            //string campaignIds = "19";
            List<int> listCampaignIdsForFeedbackView = new List<int>(){(int)Config.Campaign.DcoOffice,
                                                                   (int)Config.Campaign.SchoolEducationEnhanced,
                                                                   (int)Config.Campaign.Hospital,
                                                                   (int)Config.Campaign.PLRA,
                                                                    (int)Config.Campaign.Police};
            //response.ListCampaignIdsForFeedbackView = listCampaignIdsForFeedbackView;
            response.Campaignlst = GetCompliantsDataByCampaignIds2(startDate, endDate, listCampaignIdsForFeedbackView);
            return response;
        }


        public static List<VmCampaignWiseData> GetCompliantsDataByCampaignIds2(string startDate, string endDate, List<int> listCampaignIdsForFeedbackView)
        {
            
            string campaignIds = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.User, Config.Permissions.MultipleCampaignsAssignment);
            List<int> listCampIds = Utility.GetIntList(campaignIds);
            List<DbPermissionsAssignment> listCampStatusMergePermission = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.Campaign, Utility.GetNullableIntList(listCampIds), Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
            List<DbStatus> listDbStatuses = DbStatus.GetAll();
            List<DbCampaign> listDbCampaign = DbCampaign.GetByIds(listCampIds);


            //Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(campMergeStatus); 
            string queryStr = @"SELECT Compaign_Id CampaignId, Complaint_Computed_Status_Id StatusId, COUNT(1) Count
                             FROM PITB.Complaints
                             WHERE Compaign_Id IN (" + campaignIds + ") AND Complaint_Type = 1 AND CONVERT(DATE, Created_Date) <= CONVERT(date,'" + endDate + "') AND CONVERT(DATE,Created_Date) >= CONVERT(date,'" + startDate + "') " +
                             " GROUP BY Compaign_Id, Complaint_Computed_Status_Id " +
                             " ORDER BY Compaign_Id, Complaint_Computed_Status_Id ";
            queryStr = string.Format(queryStr, campaignIds);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);
            List<DbToModel.CampaignStatusWiseCount> listCampStatusWiseCount = dt.ToList<DbToModel.CampaignStatusWiseCount>();

            // Get Call Landed Count
            queryStr = @"SELECT a.* INTO #Temp 
                        FROM (
                        SELECT campCallLogs.Campaign_Id,COUNT(1) Count
                        FROM  PITB.Campaign_Wise_Call_Logs  campCallLogs --AND campIds.item IS null
                        WHERE CONVERT(DATE, Call_DateTime)>=@StartDate and CONVERT(DATE, Call_DateTime)<=@EndDate
                        GROUP BY campCallLogs.Campaign_Id
                        )  a

                        INSERT INTO #Temp
                                ( Campaign_Id,
		                        Count )
                        SELECT campIds.Item, 
		                        0 AS count
                        FROM dbo.SplitString(@CampaignIds,',') campIds WHERE campIds.Item NOT IN (SELECT Campaign_Id FROM #Temp)


                        SELECT *
                        FROM #Temp t 
                        ORDER BY t.Campaign_Id

                        DROP TABLE #Temp";
            //queryStr = string.Format(queryStr, campaignIds);
            Dictionary<string, object> dictObj = new Dictionary<string, object>();
            dictObj.Add("@CampaignIds",campaignIds);
            dictObj.Add("@StartDate", startDate);
            dictObj.Add("@EndDate", endDate);
            dt = DBHelper.GetDataTableByQueryString(queryStr, dictObj);
            List<dynamic> listCampaignWiseCallLanded= dt.ToDynamicList();


            List<VmCampaignWiseData> response = new List<VmCampaignWiseData>();
            //int count = 0;

            //List<int> listStatus = listCampStatusWiseCount.Select(n => n.StatusId).Distinct().ToList();
            //listStatus.AddRange(listStatusMergePermission.Keys.ToList().Select(int.Parse).ToList());


            int campId = 0;
            for (int i = 0; i < listCampIds.Count; i++)
            {
                campId = listCampIds[i];
                var dbCampaign = listDbCampaign.Where(n => n.Id == campId).FirstOrDefault();
                var tempListCampStatusWiseCount = listCampStatusWiseCount.Where(n => n.CampaignId == campId).ToList();
                var permissionAssign = listCampStatusMergePermission.Where(n => n.Type_Id == campId).FirstOrDefault();
                Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(permissionAssign.Permission_Value);
                var listStatus = Utility.ConvertStringListToIntList(listStatusMergePermission.Keys.ToList());

                VmCampaignWiseData data = new VmCampaignWiseData(listDbStatuses.Where(n => listStatus.Contains(n.Complaint_Status_Id)).ToList());

                data.CampaignId = campId;
                data.CampaignName = Utility.GetTranslation(dbCampaign.Campaign_Name);
                data.CampaignLogoSrc = dbCampaign.LogoUrl;
                data.FormId = i;
                data.CallsLandedCount = listCampaignWiseCallLanded.Where(n => n.Campaign_Id == campId).FirstOrDefault();
                var tempListCampStatCount = listCampStatusWiseCount.Where(n => n.CampaignId == campId).ToList();

                foreach (KeyValuePair<string, string> statusMP in listStatusMergePermission)
                {
                    VmCampaignWiseData.StatusCount vmStatusCount = data.ListStatusWiseCount.Where(n => n.StatusId == Convert.ToInt32(statusMP.Key)).FirstOrDefault();
                    int statusCombinedCount = 0;
                    foreach (int statusIdToMerge in Utility.GetIntList(statusMP.Value))
                    {
                        var tempCampStatusWiseCount =
                            tempListCampStatusWiseCount.Where(n => n.StatusId == statusIdToMerge).FirstOrDefault();
                        if (tempCampStatusWiseCount != null)
                        {
                            statusCombinedCount = statusCombinedCount + tempCampStatusWiseCount.Count;
                        }
                    }
                    vmStatusCount.Count = statusCombinedCount;
                    vmStatusCount.CSSClassName = String.Format("count-{0}", vmStatusCount.StatusName);
                }

                /*for (int j = 0; j < tempListCampStatusWiseCount.Count; j++)
                {
                    DbToModel.CampaignStatusWiseCount campStatusWiseCount = tempListCampStatusWiseCount[j];
                    campStatusWiseCount.StatusId
                }*/
                if(listCampaignIdsForFeedbackView.Contains(campId))
                {
                    int totalfeedback;
                    data.ListfeedbackCategoryWiseCounts = FeedbackHandler.GetFeedbackCategoryWiseCounts(campId,startDate,endDate,out totalfeedback).ToList<FeedbackCategoryWiseCount>();
                    data.FeedbackTotalCount = totalfeedback;
                } 
                response.Add(data);

                //Campaign Id of Police = 71
                if (campId == (int)Config.Campaign.Police)
                {
                    //PopulateRealTimeEntriesForPolice(data, startDate, endDate);
                }
                if (campId == (int)Config.Campaign.PLRA)
                {
                    PopulateRealTimeEntriesForPLRA(data, startDate, endDate);
                }
                AddTotalAndComputePercentage(data);

            }

            //listComplaintdata = GetCompliantsDataByCampaignIds3(startDate, endDate, "68,69,72,73,74");
            int index = response.IndexOf(response.Where(n => n.CampaignId == 69).FirstOrDefault());
            VmCampaignWiseData tempHospitalData1 = response[index];
            response.RemoveAt(index);
            VmCampaignWiseData tempHospitalData2 = GetCompliantsDataByCampaignIds3(startDate, endDate, "68,69,72,73,74",index);
            tempHospitalData2.CallsLandedCount = listCampaignWiseCallLanded.Where(n => n.Campaign_Id == 69).FirstOrDefault();
            tempHospitalData2.ListfeedbackCategoryWiseCounts = tempHospitalData1.ListfeedbackCategoryWiseCounts;
            tempHospitalData2.FeedbackTotalCount = tempHospitalData1.FeedbackTotalCount;
            response.Insert(index, tempHospitalData2);
            //response.Add(GetCompliantsDataByCampaignIds3(startDate, endDate, "68,69,72,73,74"));

            return response;
        }

        public static VmCampaignWiseData GetCompliantsDataByCampaignIds3(string startDate, string endDate, string campaignIds, int index)
        {
            //string campaignIds = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.User, Config.Permissions.MultipleCampaignsAssignment);
            List<int> listCampIds = Utility.GetIntList(campaignIds);
            List<DbPermissionsAssignment> listCampStatusMergePermission =
                DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdListAndPermissionId((int)Config.PermissionsType.Campaign, Utility.GetIntList(campaignIds), (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
            //BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.Campaign, Utility.GetNullableIntList(listCampIds), Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
            List<DbStatus> listDbStatuses = DbStatus.GetAll();
            List<DbCampaign> listDbCampaign = DbCampaign.GetByIds(listCampIds);


            //Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(campMergeStatus); 
            string queryStr = @"SELECT Compaign_Id CampaignId, Complaint_Computed_Status_Id StatusId, COUNT(1) Count
                             FROM PITB.Complaints
                             WHERE Compaign_Id IN (" + campaignIds + ") AND Complaint_Type = 1 AND CONVERT(DATE, Created_Date) <= CONVERT(date,'" + endDate + "') AND CONVERT(DATE,Created_Date) >= CONVERT(date,'" + startDate + "') " +
                             " GROUP BY Compaign_Id, Complaint_Computed_Status_Id " +
                             " ORDER BY Compaign_Id, Complaint_Computed_Status_Id ";
            queryStr = string.Format(queryStr, campaignIds);
            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);
            List<DbToModel.CampaignStatusWiseCount> listCampStatusWiseCount = dt.ToList<DbToModel.CampaignStatusWiseCount>();

            List<VmCampaignWiseData> response = new List<VmCampaignWiseData>();
            //int count = 0;

            //List<int> listStatus = listCampStatusWiseCount.Select(n => n.StatusId).Distinct().ToList();
            //listStatus.AddRange(listStatusMergePermission.Keys.ToList().Select(int.Parse).ToList());


            int campId = 0;
            for (int i = 0; i < listCampIds.Count; i++)
            {
                campId = listCampIds[i];
                var dbCampaign = listDbCampaign.Where(n => n.Id == campId).FirstOrDefault();
                var tempListCampStatusWiseCount = listCampStatusWiseCount.Where(n => n.CampaignId == campId).ToList();
                var permissionAssign = listCampStatusMergePermission.Where(n => n.Type_Id == campId).FirstOrDefault();
                Dictionary<string, string> listStatusMergePermission = Utility.ConvertCollonFormatToDict(permissionAssign.Permission_Value);
                var listStatus = Utility.ConvertStringListToIntList(listStatusMergePermission.Keys.ToList());

                VmCampaignWiseData data = new VmCampaignWiseData(listDbStatuses.Where(n => listStatus.Contains(n.Complaint_Status_Id)).ToList());

                data.CampaignId = campId;
                data.CampaignName = Utility.GetTranslation(dbCampaign.Campaign_Name);
                data.CampaignLogoSrc = dbCampaign.LogoUrl;
                data.FormId = i;
                var tempListCampStatCount = listCampStatusWiseCount.Where(n => n.CampaignId == campId).ToList();

                foreach (KeyValuePair<string, string> statusMP in listStatusMergePermission)
                {
                    VmCampaignWiseData.StatusCount vmStatusCount = data.ListStatusWiseCount.Where(n => n.StatusId == Convert.ToInt32(statusMP.Key)).FirstOrDefault();
                    int statusCombinedCount = 0;
                    foreach (int statusIdToMerge in Utility.GetIntList(statusMP.Value))
                    {
                        var tempCampStatusWiseCount =
                            tempListCampStatusWiseCount.Where(n => n.StatusId == statusIdToMerge).FirstOrDefault();
                        if (tempCampStatusWiseCount != null)
                        {
                            statusCombinedCount = statusCombinedCount + tempCampStatusWiseCount.Count;
                        }
                    }
                    vmStatusCount.Count = statusCombinedCount;
                    vmStatusCount.CSSClassName = String.Format("count-{0}", vmStatusCount.StatusName);
                }

                /*for (int j = 0; j < tempListCampStatusWiseCount.Count; j++)
                {
                    DbToModel.CampaignStatusWiseCount campStatusWiseCount = tempListCampStatusWiseCount[j];
                    campStatusWiseCount.StatusId
                }*/
                response.Add(data);

                AddTotalAndComputePercentage(data);
            }
            return AddTotalAndComputePercentage2(response, index);
            //return response;
        }



        //private static void PopulateRealTimeEntriesForPolice(VmCampaignWiseData vmCampWiseData, string startDate, string endDate)
        //{
        //    string url = "http://202.83.174.202:8090/api/StatisticsServive/fetchStatisticsByDate";
        //    RequestModel.PoliceDashboardCount reqModel = new RequestModel.PoliceDashboardCount();
        //    reqModel.request_userName = "igpcms-PITB";
        //    reqModel.request_password = "P1Tb@1gpCm$";
        //    reqModel.request_startDate = Utility.GetDateTime(/*"MM/dd/yyyy",*/ startDate, "yyyy-MM-dd");
        //    reqModel.request_endDate = Utility.GetDateTime(/*"MM/dd/yyyy",*/ endDate, "yyyy-MM-dd");
        //    reqModel.request_complaintType = "1";
        //    List<ResponseModel.PoliceDashboardCount> respList = APIHelper.HttpClientGetResponseList<ResponseModel.PoliceDashboardCount, RequestModel.PoliceDashboardCount>(url, reqModel, null);
        //    if (respList.Count > 0 && respList.Any(x=> x.response_category != null))
        //    {

        //        var statusId8 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 8).FirstOrDefault();
        //        var disposed = respList.Where(n => n.response_category == "Disposed").FirstOrDefault();
        //        if (statusId8 != null && disposed != null)
        //            statusId8.Count = Int32.Parse(disposed.response_count);
                
        //        var statusId18 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 18).FirstOrDefault();
        //        if (statusId18 != null)
        //            statusId18.Count = 0;

        //        var statusId19 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 19).FirstOrDefault();
        //        var inprocess = respList.Where(n => n.response_category == "Inprocess").FirstOrDefault();
        //        if (statusId19 != null && inprocess != null)
        //            statusId19.Count = Int32.Parse(inprocess.response_count);
        //    }
        //}

        private static void PopulateRealTimeEntriesForPolice(VmCampaignWiseData vmCampWiseData, string startDate, string endDate)
        {
            //string url = "http://202.83.174.202:8090/api/StatisticsServive/fetchStatisticsByDate";
            //RequestModel.PoliceDashboardCount reqModel = new RequestModel.PoliceDashboardCount();
            //reqModel.request_userName = "igpcms-PITB";
            //reqModel.request_password = "P1Tb@1gpCm$";
            //reqModel.request_startDate = Utility.GetDateTime(/*"MM/dd/yyyy",*/ startDate, "yyyy-MM-dd");
            //reqModel.request_endDate = Utility.GetDateTime(/*"MM/dd/yyyy",*/ endDate, "yyyy-MM-dd");
            //reqModel.request_complaintType = "1";
            //List<ResponseModel.PoliceDashboardCount> respList = APIHelper.HttpClientGetResponseList<ResponseModel.PoliceDashboardCount, RequestModel.PoliceDashboardCount>(url, reqModel, null);

            string updateCommand = @"
                                        SELECT Complaint_Computed_Status_Id StatusId, COUNT(1) Count 
                                        FROM PITB.Complaints
                                        WHERE Compaign_Id = @CompaignId and (Created_Date BETWEEN @StartDate AND @EndDate)
                                        GROUP BY Complaint_Computed_Status_Id
                                        ";
            //Dictionary<string, object> dictParams = new Dictionary<string, object>();
            //dictParams.Add("@UserId", dbUsers.User_Id);
            //dictParams.Add("@Platform_Id", Config.PlatformID.Android.ToDbObj());
            //dictParams.Add("@Tag_Id", "Campaign::47__Type::User__Platform::Android");
            //dictParams.Add("@Device_Id", submitStakeHolderLogin.fcm_key);
            //dictParams.Add("@Is_Active", 1);
            //dictParams.Add("@UserId", dbUsers.User_Id);
             Dictionary<string,object> dictParams = new Dictionary<string, object>();
             dictParams.Add("@CompaignId", 78);
             dictParams.Add("@StartDate", Utility.GetDateTimeStr(startDate));
             dictParams.Add("@EndDate", Utility.GetDateTimeStr(endDate));
             List<dynamic> listDynamic = Helper.Database.DBHelper.GetDynamicListByQueryString(updateCommand, dictParams);


             if (listDynamic.Count > 0 /*&& listDynamic.Any(x => x.response_category != null)*/)
            {

                var statusId8 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 8).FirstOrDefault();
                //var disposed = respList.Where(n => n.response_category == "Disposed").FirstOrDefault();
                List<int> listStatuses = new List<int>{2,3};
                var disposed = listDynamic.Where(n => listStatuses.Contains(n.StatusId)).FirstOrDefault(); // resolved complaints
                if (statusId8 != null && disposed != null)
                {
                    statusId8.Count = disposed.Count;
                }
                
                var statusId18 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 18).FirstOrDefault(); // Overdue complaints
                var overdue = listDynamic.Where(n => n.StatusId == 6).FirstOrDefault();
                if (statusId18 != null && overdue!=null)
                {
                    //statusId18.Count = 0; //Int32.Parse(overdue.Count); 
                    statusId18.Count = overdue.Count;
                }

                var statusId19 = vmCampWiseData.ListStatusWiseCount.Where(n => n.StatusId == 19).FirstOrDefault();
                //var inprocess = respList.Where(n => n.response_category == "Inprocess").FirstOrDefault();
                var inprocess = listDynamic.Where(n => /*n.StatusId == 23 ||*/ n.StatusId == 1 || n.StatusId == 7).FirstOrDefault();
                if (statusId19 != null && inprocess != null)
                {
                    //statusId19.Count = Int32.Parse(inprocess.response_count);
                    statusId19.Count = inprocess.Count;

                }
            }

            
        }
        private static string GetWebRequest(string startDate,string endDate,string filepath){
            string url = "http://172.18.0.23/PLRA_Comp_Service.asmx/";
            string actionName = "GetTicketsStatus";
            startDate = Utility.GetDateTimeStr(startDate, "yyyy-MM-dd");
            endDate = Utility.GetDateTimeStr(endDate, "yyyy-MM-dd");
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("DateFrom", startDate);
                paramDict.Add("DateTo", endDate);
               string xml = SOAPHelper.HttpPostProtocolConsumeWebRequest(url, actionName, paramDict);
              return xml;
        }
        private static void PopulateRealTimeEntriesForPLRA(VmCampaignWiseData vmCampWiseData, string startDate, string endDate)
        {
            try
            {
                string url = "http://cms.punjab-zameen.gov.pk/PLRA_Comp_Service.asmx/GetTicketsStatus";
                startDate = Utility.GetDateTimeStr(startDate, "yyyy-MM-dd");
                endDate = Utility.GetDateTimeStr(endDate, "yyyy-MM-dd");
                Dictionary<string, object> paramDict = new Dictionary<string, object>();
                paramDict.Add("DateFrom", startDate);
                paramDict.Add("DateTo",endDate);
                StringBuilder urlParameters = new StringBuilder();
                foreach (var prop in paramDict)
                {
                    urlParameters.AppendFormat("{0}={1}&", prop.Key, prop.Value);
                }
                string urlTemps = urlParameters.ToString().TrimEnd('&');
                url = url + "?" + urlTemps;

                APIHelper.ResponsePath = ReponsePath;
                //string xml = APIHelper.HttpClientGetResponse<string, RequestModel.PLRADashboardCount>(url, reqModel, null); 
                //string xml = System.IO.File.ReadAllText(filepath);
                //string xml = GetWebRequest(startDate, endDate, ReponsePath);
                string xml = SOAPHelper.HttpGetProtocolConsumeWebRequest(url);
                
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                XmlNodeList nodes = document.GetElementsByTagName("tblServiceTickStatus");
                if (nodes != null && nodes.Count > 0)
                {
                    string total = nodes[0].SelectSingleNode("Total_Complaints").InnerText;
                    string pending = nodes[0].SelectSingleNode("Pending_Complaints").InnerText;
                    string resolved = nodes[0].SelectSingleNode("Resolved_Complaints").InnerText;
                    int totalInt = 0;
                    int pendingInt = 0;
                    int resolvedInt = 0;
                    int.TryParse(total, out totalInt);
                    int.TryParse(pending, out pendingInt);
                    int.TryParse(resolved, out resolvedInt);

                    var statusId8 = vmCampWiseData.ListStatusWiseCount.Single(x => x.StatusId == 8);
                    if (statusId8 != null)
                    {
                        statusId8.Count = resolvedInt;
                    }
                    var statusId18 = vmCampWiseData.ListStatusWiseCount.Single(x => x.StatusId == 18);
                    if (statusId18 != null)
                    {
                        statusId18.Count = totalInt - pendingInt - resolvedInt;
                    }
                    var statusId19 = vmCampWiseData.ListStatusWiseCount.Single(x => x.StatusId == 19);
                    if (statusId19 != null && pending != null)
                    {
                        statusId19.Count = pendingInt;
                    }
                }
            }
            catch (Exception ex)
            {
                string obj = JsonConvert.SerializeObject(ex);
                //System.IO.File.AppendAllText(ErrorFilePath, DateTime.Now.ToShortTimeString());
                //System.IO.File.AppendAllText (ErrorFilePath, obj);
                vmCampWiseData.ListStatusWiseCount.ForEach(x => x.Count = 0);
            }
            
        }
        private static void AddTotalAndComputePercentage(VmCampaignWiseData vmCampWiseData)
        {
            VmCampaignWiseData.StatusCount statusWiseCount = new VmCampaignWiseData.StatusCount();
            statusWiseCount.StatusId = -1;
            statusWiseCount.StatusName = "Total";
            statusWiseCount.Count = 0;

            int totalSum = vmCampWiseData.ListStatusWiseCount.Sum(n => n.Count);
            statusWiseCount.Count = totalSum;
            foreach (VmCampaignWiseData.StatusCount vmStatusCount in vmCampWiseData.ListStatusWiseCount)
            {
                //statusWiseCount.Count = statusWiseCount.Count + vmStatusCount.Count;
                if ((float)statusWiseCount.Count == 0)
                {
                    vmStatusCount.Percentage =
                       (float)Math.Round(0.0f, 1);
                }
                else
                {
                    vmStatusCount.Percentage =
                        (float)Math.Round(((float)vmStatusCount.Count / (float)statusWiseCount.Count) * 100, 1);
                }
            }
            vmCampWiseData.ListStatusWiseCount.Insert(0, statusWiseCount);
        }

        private static VmCampaignWiseData AddTotalAndComputePercentage2(List<VmCampaignWiseData> lisVmCampWiseData, int index)
        {
            //VmCampaignWiseData.StatusCount statusWiseCount = new VmCampaignWiseData.StatusCount();
            //statusWiseCount.StatusId = -1;
            //statusWiseCount.StatusName = "Total";
            //statusWiseCount.Count = 0;

            VmCampaignWiseData vmCampData = new VmCampaignWiseData();
            vmCampData.CampaignId = 69;
            vmCampData.CampaignName = "Health";
            vmCampData.CampaignLogoSrc = DbCampaign.GetLogoUrlByCampaignId((int)vmCampData.CampaignId);
            vmCampData.ListStatusWiseCount = new List<VmCampaignWiseData.StatusCount>();
            vmCampData.FormId = index;

            int totalSum = 0;
            foreach (VmCampaignWiseData vmCampWiseData in lisVmCampWiseData)
            {
                //totalSum = totalSum + vmCampWiseData.ListStatusWiseCount.Sum(n => n.Count);

                foreach (VmCampaignWiseData.StatusCount vmStatusCount in vmCampWiseData.ListStatusWiseCount)
                {
                    if (vmStatusCount.StatusId != -1)
                    {
                        VmCampaignWiseData.StatusCount statusCount =
                            vmCampData.ListStatusWiseCount.Where(n => n.StatusId == vmStatusCount.StatusId)
                                .FirstOrDefault();
                        if (statusCount != null)
                        {
                            statusCount.Count = statusCount.Count + vmStatusCount.Count;
                            //vmCampData.ListStatusWiseCount.Add(new VmCampaignWiseData.StatusCount());
                        }
                        else
                        {
                            statusCount = new VmCampaignWiseData.StatusCount();
                            statusCount.Count = vmStatusCount.Count;
                            statusCount.StatusId = vmStatusCount.StatusId;
                            statusCount.StatusName = vmStatusCount.StatusName;
                            statusCount.CSSClassName = vmStatusCount.CSSClassName;
                            vmCampData.ListStatusWiseCount.Add(statusCount);
                        }
                    }
                    //statusWiseCount.Count = statusWiseCount.Count + vmStatusCount.Count;

                }
                //vmCampWiseData.ListStatusWiseCount.Insert(0, statusWiseCount); 
            }
            // statusWiseCount.Count = totalSum;
            AddTotalAndComputePercentage(vmCampData);
            return vmCampData;
            //foreach (VmCampaignWiseData.StatusCount vmStatusCount in vmCampData.ListStatusWiseCount)
            //{
            //    if ((float)statusWiseCount.Count == 0)
            //    {
            //        vmStatusCount.Percentage =
            //           (float)Math.Round(0.0f, 1);
            //    }
            //    else
            //    {
            //        vmStatusCount.Percentage =
            //            (float)Math.Round(((float)vmStatusCount.Count / (float)statusWiseCount.Count) * 100, 1);
            //    }


            //    vmStatusCount.Percentage = (float)Math.Round(((float)vmStatusCount.Count / (float)statusWiseCount.Count) * 100, 1);
            //}



            return vmCampData;
        }


        public static dynamic GetListingView (dynamic d)
        {
            dynamic dataToRet = new ExpandoObject();
            dataToRet.viewBag = new ExpandoObject();
            //ExpandoObject asd =  new ;
            string[] splitArr = ((string)d.tagId).Split(new string[] { "__" }, StringSplitOptions.None);
            string moduleId = splitArr[1];
            int campaignId = Convert.ToInt32(splitArr[2]);

            switch(campaignId)
            {
                case (int)Config.Campaign.Police:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_PoliceComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = "Police ("+ moduleId+")";
                    break;

                case (int)Config.Campaign.DcoOffice:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_ZimmedarShehriComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = "DcOffice (" + moduleId+")";
                    break;
                case (int)Config.Campaign.PLRA:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_PlraComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = "Plra complaint Listing (all)";
                    break;
                case (int)Config.Campaign.SchoolEducationEnhanced:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_SchoolEducationComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = "School Education (" + moduleId + ")";
                    break;
                case (int)Config.Campaign.Hospital:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_HealthComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = "Health (" + moduleId + ")";
                    break;

                default:
                    dataToRet.viewName = "~/Views/ExecutiveView/Listing/_CommonComplaintListing.cshtml";
                    dataToRet.viewBag = d;
                    dataToRet.viewBag.logoUrl = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
                    dataToRet.viewBag.pageHeading = DbCampaign.GetById(campaignId).Campaign_Name+" (" + moduleId + ")";
                    break;
            }
            return dataToRet;
        }

        public static dynamic GetListingData(dynamic data)
        {
            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(data.aoData);
            string startDate = data.startDate;
            string endDate = data.endDate;
            string[] splitArr = ((string)data.tagId).Split(new string[] { "__" }, StringSplitOptions.None);
            string tagName = splitArr[0];
            string moduleId = splitArr[1];
            int campaignId = Convert.ToInt32(splitArr[2]);
            string campaignIdsDbParam = campaignId.ToString();


            if (campaignId == (int)Config.Campaign.Hospital)
            {
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                string permissionVal = cmsCookie.ListPermissions.Where(n=>n.Permission_Id==(int)Config.Permissions.CombineMultipleStatusCountInDashboard).FirstOrDefault().Permission_Value;
                Dictionary<string,string> dictPermission =  Utility.ConvertCollonFormatToDict(permissionVal);
                campaignIdsDbParam = dictPermission["campaignIds"];
            }

            List<DbPermissionsAssignment> listCampStatusMergePermission = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.PermissionsType.Campaign, Utility.GetNullableIntList(new List<int> { campaignId }), Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
            var permissionAssign = listCampStatusMergePermission.Where(n => n.Type_Id == campaignId).FirstOrDefault();
            Dictionary<string, string> dictStatusPermission = Utility.ConvertCollonFormatToDict(permissionAssign.Permission_Value);



            Dictionary<string, string> dictOrderQuery = new Dictionary<string, string>();
            List<string> prefixList = DataTableHandler.GetPrefixList(dtModel, "complaints");
            //List<string> prefixStrList = new List<string>
            //        {
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //            "complaints",
            //        };
            // for joins
            //List<string> prefixStrList = new List<string> { "complaints", "campaign", "districts", "tehsil", "personInfo", "complaints", "complaintType", "Statuses", "complaints" };
            //dictOrderQuery.Add("Complaint_Category_Name", "complaintType.name");
            //dictOrderQuery.Add("Complaint_Computed_Status", "Statuses.Status");
            DataTableHandler.ApplyColumnOrderPrefix(dtModel);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            //dictFilterQuery.Add("a.Hierarchy", "dbo.GetHierarchyStrFromId(dbo.GetHierarchy(a.Dt1,a.SrcId1,a.Dt2,a.SrcId2,a.Dt3,a.SrcId3,a.Dt4,a.SrcId4,a.Dt5,a.SrcId5,@currDate)) Like '%_Value_%'");
            dictFilterQuery.Add("complaints.Created_Date",
                "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintId" }, prefixList,
                    dictFilterQuery);

            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            // data table params
            if (dtModel != null)
            {
                paramDict.Add("@StartRow", (dtModel.Start).ToDbObj());
                paramDict.Add("@EndRow", (dtModel.End).ToDbObj());
                paramDict.Add("@OrderByColumnName", (dtModel.ListOrder[0].columnName).ToDbObj());
                paramDict.Add("@OrderByDirection", (dtModel.ListOrder[0].sortingDirectionStr).ToDbObj());
                paramDict.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch.ToDbObj());
            }


            // Adding tags check
            string queryModule = null;
            string configKey = null;
            if (tagName == "TagStatus")
            {
                Dictionary<string, string> dictModuleStatusMap = new Dictionary<string, string>()
                {
                    { "Resolved", "8"},
                    { "Overdue", "18"},
                    { "Pending", "19"}
                };
                //queryModule = string.Format("ComplaintsListingStatusWise___Campaign::{0}", campaignId);
                List<string> lisKeys = new List<string>()
                {
                    string.Format("Type::Query___Module::ComplaintsListingStatusWise___Campaign::{0}", campaignId),
                    "Type::Query___Module::ComplaintsListingStatusWise"
                };
                configKey = ConfigurationHandler.GetConfiguration(lisKeys);
                string statusIds = "";
                if (moduleId == "Total") // 
                {
                    foreach(KeyValuePair<string,string> keyVal in dictModuleStatusMap)
                    {
                        statusIds = statusIds + dictStatusPermission[keyVal.Value]+",";
                    }
                    statusIds = statusIds.Remove(statusIds.Length - 1, 1);
                    //statusIds = string.Join(",", dictModuleStatusMap.Values.ToArray()); 
                    //statusIds = dictModuleStatusMap.Aggregate(dictModuleStatusMap, (current, next) => listStatusMergePermission[current] + "," + listStatusMergePermission[next.Value]);
                }
                else
                {
                    statusIds = dictStatusPermission[dictModuleStatusMap[moduleId]];
                }
                paramDict.Add("@StatusIds", statusIds.ToDbObj());
            }
            else if (tagName == "TagFeedback")
            {
                Dictionary<string, string> dictModuleFeedbackMap = new Dictionary<string, string>()
                {
                    { "Satisfied", "1"},
                    { "Dissatisfied", "2"}
                };

                List<string> lisKeys = new List<string>()
                {
                    string.Format("Type::Query___Module::ComplaintsListingFeedbackWise___Campaign::{0}", campaignId),
                    "Type::Query___Module::ComplaintsListingFeedbackWise"
                };
                configKey = ConfigurationHandler.GetConfiguration(lisKeys);


                queryModule = string.Format("ComplaintsListingFeedbackWise___Campaign::{0}", campaignId);
                paramDict.Add("@FeedbackIds", dictModuleFeedbackMap[moduleId].ToDbObj());
            }

            paramDict.Add("@CampaignIds", campaignIdsDbParam.ToDbObj());
            //paramDict.Add("@StatusIds", listStatusMergePermission["Resolved"].ToDbObj());
            paramDict.Add("@StartDate", startDate.ToDbObj());
            paramDict.Add("@EndDate", endDate.ToDbObj());

            string queryStr = QueryHelper.GetParamsReplacedQuery(configKey, paramDict);

            List<dynamic> listResult = DBHelper.GetDynamicListByQueryString(queryStr, null); //ds.Tables[0];


            dynamic tableData = new ExpandoObject();
            tableData.data = listResult;
            tableData.draw = dtModel.Draw++;
            tableData.recordsTotal = (listResult != null && listResult.Count > 0) ? listResult[0].Total_Rows : 0;
            tableData.recordsFiltered = tableData.recordsTotal;
            //tableData.recordsTotal = 0;
            return tableData;
        }


        //public static dynamic PopulateListingTable(dynamic d)
        //{
        //    dynamic dataToRet = new ExpandoObject();
        //    //ExpandoObject asd =  new ;
        //    string itemId = d.tagId;
        //    dataToRet.logoUrl = "";
        //    dataToRet.pageHeading = "Munna kaka";
        //    dataToRet.listColumnsData = new List<dynamic>();
        //    dataToRet.listColumnsData.Add(new { isSearchable = true, name = "c1" }.ToExpando());
        //    dataToRet.listColumnsData.Add(new { isSearchable = true, name = "c2" }.ToExpando());
        //    dataToRet.listColumnsData.Add(new { isSearchable = true, name = "c3" }.ToExpando());
        //    string asd = dataToRet.listColumnsData[0].name;
        //    dataToRet.tableId = 1;
        //    //ctrController.RenderViewToString(ctrController.ControllerContext, "~/Views/Shared/ViewUserControls/_MessageBox.cshtml", vmMessage, true);
        //    return dataToRet;
        //}
        /*
        public static List<VmCampaignWiseData> GetCompliantsDataByCampaignIds()
        {
            string campaignIds = BlCommon.GetCampaignIdsFromPermissionAssingment(Config.Permissions.MultipleCampaignsAssignment);
            KeyValuePair<int, int[]>[] mergerIds = BlCommon.GetCampaignMergerIdsFromPermissionAssignment(Config.Permissions.MultipleCampaignsMerger);
            List<DbComplaint> lst = DbComplaint.GetListByCampaignIds(campaignIds);            
            List<VmCampaignWiseData> response = new List<VmCampaignWiseData>();
            int count = 0;      
            if (lst != null)
            {
                foreach (var row in lst.GroupBy(x=> x.Compaign_Id))
                {
                    VmCampaignWiseData data = new VmCampaignWiseData();
                   
                    data.FormId = count;
                    data.CampaignId = row.First().Compaign_Id;
                    data.CampaignName = row.First().Campaign_Name;
                    data.CampaignLogoSrc = DbCampaign.GetLogoUrlByCampaignId((int)data.CampaignId);
                    data.StatusListName = new Dictionary<int, string>();
                    data.StatusListCount = new Dictionary<int, int>();
                    KeyValuePair<int, int[]>[] statuslst = BlCommon.GetStatusListPermissionForCampaign((int)data.CampaignId);
                    if (statuslst != null && statuslst.Length > 0)
                    {
                        var statusList = row.GroupBy(x => x.Complaint_Computed_Status_Id).Select(z => new { StatusId = z.Key, StatusCount = z.Count(), StatusName = z.First().Complaint_Computed_Status });
                        for (int i = 0; i < statuslst.Length; i++)
                        {
                            IEnumerable<int> innerList = statuslst[i].Value;
                            int statusCount = 0;
                            string statusName = DbStatus.GetById(statuslst[i].Key).Status;
                            foreach (var status in statusList.Join(innerList, n => n.StatusId, m => m, (n, m) => n))
                            {
                                statusCount += status.StatusCount;
                            }
                            data.StatusListName.Add(statuslst[i].Key, statusName);
                            data.StatusListCount.Add(statuslst[i].Key, statusCount);
                            
                        }
                    }
                    response.Add(data);
                    count++;
                }
            }           
            return response;
        }
        */

        public static VmCampaignStatusWise GetCompliantsDataBySingleCampaignId(int campaignId, DateTime startDate, DateTime endDate)
        {
            List<DbComplaint> lst = DbComplaint.GetByCampaignId(campaignId);
            lst = lst.Where(x => x.Created_Date >= startDate && x.Created_Date <= endDate).ToList<DbComplaint>();
            VmCampaignStatusWise camp = new VmCampaignStatusWise();
            camp.CampaignId = lst.FirstOrDefault().Compaign_Id;
            camp.CampaignName = lst.FirstOrDefault().Campaign_Name;
            camp.CampaignLogoSrc = DbCampaign.GetLogoUrlByCampaignId((int)campaignId);
            List<DbStatus> statuslst = DbStatus.GetByCampaignId((int)campaignId);
            camp.Status = new List<StatusCountObject>();
            var statusList = lst.GroupBy(x => x.Complaint_Computed_Status_Id).Select(z => new { StatusId = z.Key, StatusCount = z.Count(), StatusName = z.First().Complaint_Computed_Status });
            foreach (var status in statusList.Join(statuslst, n => n.StatusId, m => m.Complaint_Status_Id, (n, m) => n))
            {
                camp.Status.Add(new StatusCountObject() { StatusCount = status.StatusCount, StatusId = status.StatusId, StatusName = status.StatusName });
            }
            return camp;
        }
        public static string GetMappedCampaignName(int campaignId)
        {
            Dictionary<int, string> CampaignMapping = new Dictionary<int, string>();
            CampaignMapping.Add(1, "Zimmedar Shehri");
            CampaignMapping.Add(2, "WASA");
            CampaignMapping.Add(3, "Hajj");
            CampaignMapping.Add(4, "Dengue");
            CampaignMapping.Add(5, "Punjab Khidmat Card");
            CampaignMapping.Add(7, "Metro Bus");
            CampaignMapping.Add(9, "Excise");
            CampaignMapping.Add(10, "Local Government");
            CampaignMapping.Add(11, "BOR");
            CampaignMapping.Add(12, "Agriculture");
            CampaignMapping.Add(15, "Livestock");
            CampaignMapping.Add(18, "Awaz-e-Khalq");
            CampaignMapping.Add(19, "BMS");
            CampaignMapping.Add(20, "DIME");
            CampaignMapping.Add(21, "DSS");
            CampaignMapping.Add(22, "Dengue Tracking");
            CampaignMapping.Add(23, "OCAS");
            CampaignMapping.Add(24, "LHC Litigant Complaint/Query");
            CampaignMapping.Add(29, "Punjab Health Insurance");
            CampaignMapping.Add(33, "PHL-Central Induction");
            CampaignMapping.Add(35, "Wasa (Punjab-FIXIT)");
            CampaignMapping.Add(36, "MCL (Punjab-FIXIT)");
            CampaignMapping.Add(39, "CDGL (Punjab-FIXIT)");
            CampaignMapping.Add(43, "School Education ");
            CampaignMapping.Add(47, "School Education");
            CampaignMapping.Add(49, "LWMC (Punjab-FIXIT)");
            CampaignMapping.Add(50, "WASA (New)");
            CampaignMapping.Add(54, "WASA");
            CampaignMapping.Add(55, "TEPA");
            CampaignMapping.Add(56, "LESCO");
            CampaignMapping.Add(57, "PHA");
            CampaignMapping.Add(58, "Schools");
            CampaignMapping.Add(59, "DC Office");
            CampaignMapping.Add(60, "SNGPL");
            CampaignMapping.Add(63, "TestDynamic");
            CampaignMapping.Add(66, "Zimmedar Shehri Dynamic");
            CampaignMapping.Add(68, "Dengue");
            CampaignMapping.Add(69, "Health");
            CampaignMapping.Add(70, "DC-Office");
            return CampaignMapping[campaignId];
        }

        public static bool SendSmsToExecutives(DateTime startDate, DateTime endDate)
        {
            List<DbPermissionsAssignment> campaignIds = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(3, 1, 1);
            if (campaignIds != null && campaignIds.Count > 0)
            {

                string sms = SmsMessageText(campaignIds.First().Permission_Value, startDate, endDate, true);

//                sms = sms.Substring(0, sms.IndexOf("Health"));

//                sms = sms + @"Health
//Total Complaints: 2320
//Resolved: 2167
//Pending: 153
//Overdue: 0";
                List<DbPermissionsAssignment> generalPhoneNos = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.WindowsService, 1, (int)Config.WindowsServicePermissions.GeneralExecutiveSmsPhoneNos);
                if (generalPhoneNos != null)
                {
                    string permissionValue = generalPhoneNos.First().Permission_Value;
                    if (permissionValue != null)
                    {
                        IEnumerable<string> phoneNos = permissionValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (phoneNos != null)
                        {
                            SendSMSFunction(phoneNos, sms);
                        }
                    }
                }
                /*
                List<DbPermissionsAssignment> campaignSpecificPhoneNos = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.WindowsService, 1, (int)Config.WindowsServicePermissions.CampaignSpecificExecutiveSmsPhoneNos);
                if (campaignSpecificPhoneNos != null)
                {
                    string permissionValue = campaignSpecificPhoneNos.First().Permission_Value;
                    if (permissionValue != null)
                    {
                        Dictionary<string, string> pairList = Utility.ConvertCollonFormatToDict(permissionValue);
                        if (pairList != null)
                        {
                            for (int i = 0; i < pairList.Keys.Count; i++)
                            {
                                string campaignId = pairList.Keys.ElementAt(i);
                                IEnumerable<string> phoneNos = pairList.Values.ElementAt(i).Split(new char[] { ',' });
                                if (phoneNos != null)
                                {
                                    new System.Threading.Thread(delegate()
                                    {
                                        string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" + GetCampaignStatusesCount(campaignId, DbCampaign.GetCampaignShortNameById(Int32.Parse(campaignId)), startDate, endDate);
                                        SendSMSFunction(phoneNos, smsText);
                                    }).Start();
                                }
                            }
                        }
                    }
                }*/
            }
            return true;
        }
        public static string SmsMessageText(string campaignIds, DateTime startDate, DateTime endDate, bool isCumulativeCampaigns = false)
        {
            string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n";
            List<int> campaignList = Utility.GetIntList(campaignIds);
            if (campaignList != null && campaignList.Count > 0)
            {
                foreach (int id in campaignList)
                {
                    smsText += GetCampaignStatusesCount(id.ToString(), DbCampaign.GetCampaignShortNameById(id), startDate, endDate);
                }
            }
            if (isCumulativeCampaigns)
            {
                List<DbPermissionsAssignment> cumulativePermission = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.WindowsService, 1, (int)Config.WindowsServicePermissions.SendSmsToExecutivesCumulativeCampaigns);
                if (cumulativePermission != null && cumulativePermission.Count > 0)
                {
                    Dictionary<string, string> cumlative = Utility.ConvertCollonFormatToDict(cumulativePermission.First().Permission_Value);
                    if (cumlative != null && cumlative.Count > 0)
                    {
                        foreach (var item in cumlative)
                        {
                            smsText += GetCampaignStatusesCount(item.Value, item.Key, startDate, endDate);
                        }
                    }
                }
            }
            return smsText;
        }
        private static string GetCampaignStatusesCount(string campaignIds, string campaignName, DateTime startDate, DateTime endDate)
        {
            string smsText = null;
            List<int> campIds = Utility.GetIntList(campaignIds);
            List<DbPermissionsAssignment> statusPermissions = new List<DbPermissionsAssignment>();
            if (campIds != null)
            {
                // Police Campaign Data
                if (campIds.Contains(71))
                {
                    string url = "http://202.83.174.202:8090/api/StatisticsServive/fetchStatisticsByDate";
                    RequestModel.PoliceDashboardCount reqModel = new RequestModel.PoliceDashboardCount();
                    reqModel.request_userName = "igpcms-PITB";
                    reqModel.request_password = "P1Tb@1gpCm$";
                    reqModel.request_startDate = startDate.ToString("MM/dd/yyyy");
                    reqModel.request_endDate = endDate.ToString("MM/dd/yyyy");
                    reqModel.request_complaintType = "1";
                    List<ResponseModel.PoliceDashboardCount> respList = APIHelper.HttpClientGetResponseList<ResponseModel.PoliceDashboardCount, RequestModel.PoliceDashboardCount>(url, reqModel, null);
                    int resolved = 0;
                    int pending = 0;
                    int total = 0;
                    int overdue = 0;
                    resolved = Convert.ToInt32(respList.Where(n => n.response_category == "Disposed").First().response_count);
                    overdue = 0;
                    pending = Convert.ToInt32(respList.Where(n => n.response_category == "Inprocess").First().response_count); ;
                    total = resolved + pending + overdue;
                    smsText = //"Chief Minister’s Complaint Center Summary " + DateTime.Today.ToShortDateString() + "\n\n" +
                                              campaignName + "\n" +
                                              "Total Complaints: " + total + "\n" +
                                              "Resolved: " + resolved + "\n" +
                                              "Pending: " + pending + "\n" +
                                              "Overdue: " + overdue + "\n\n";
                    return smsText;
                }
                else
                {
                    if (campIds.Count == 1)
                    {
                        statusPermissions.AddRange(DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campIds[0], (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap));
                    }
                    else if (campIds.Count > 1)
                    {
                        for (int i = 0; i < campIds.Count; i++)
                        {
                            statusPermissions.AddRange(DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, campIds[i], (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap));
                        }
                    }
                }
            }
            if (statusPermissions != null && statusPermissions.Count > 0)
            {
                string sqlCommandText = string.Empty;
                for (int i = 0; i < statusPermissions.Count; i++)
                {
                    DbPermissionsAssignment permission = statusPermissions[i];
                    Dictionary<string, string> statuses = Utility.ConvertCollonFormatToDict(permission.Permission_Value);
                    string pending = statuses.First(x => x.Key.Equals("19")).Value.ToString();
                    string overdue = statuses.First(x => x.Key.Equals("18")).Value.ToString();
                    string resolved = statuses.First(x => x.Key.Equals("8")).Value.ToString();
                    string total = string.Format("{0},{1},{2}", pending, overdue, resolved);
                    string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id WHERE c.Compaign_Id IN ({0}) AND c.Complaint_Type = 1 AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name ";
                    sqlCommandText += String.Format(queryText, permission.Type_Id, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                }
                DataSet ds = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (statusPermissions.Count == 1)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var row = ds.Tables[0].Rows[0];
                            smsText = //"Chief Minister’s Complaint Center Summary " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                   campaignName + "\n" +
                                                   "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                   "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                   "Pending: " + row.Field<int>("Pending") + "\n" +
                                                   "Overdue: " + row.Field<int>("Overdue") + "\n\n";
                        }
                    }
                    else if (statusPermissions.Count > 1)
                    {
                        int resolved = 0;
                        int pending = 0;
                        int total = 0;
                        int overdue = 0;
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            if (ds.Tables[i].Rows.Count > 0)
                            {
                                var row = ds.Tables[i].Rows[0];
                                resolved += row.Field<int>("Resolved");
                                pending += row.Field<int>("Pending");
                                total += row.Field<int>("Total");
                                overdue += row.Field<int>("Overdue");
                            }
                        }
                        smsText = //"Chief Minister’s Complaint Center Summary " + DateTime.Today.ToShortDateString() + "\n\n" +
                                               campaignName + "\n" +
                                               "Total Complaints: " + total + "\n" +
                                               "Resolved: " + resolved + "\n" +
                                               "Pending: " + pending + "\n" +
                                               "Overdue: " + overdue + "\n\n";
                    }
                }
            }
            return smsText;
        }
        public static string SendHierarchyMessages(DateTime startDate, DateTime endDate)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>(4);
            dict.Add("Resolved", 0);
            dict.Add("Pending", 0);
            dict.Add("Overdue", 0);
            dict.Add("Total", 0);
            string sqlCommandText = string.Empty;
            List<DbPermissionsAssignment> permission = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.WindowsService, 1, (int)Config.WindowsServicePermissions.SendHierarchyMessages);
            if (permission != null && permission.Count > 0)
            {
                Dictionary<string, string> permissionDict = Utility.ConvertCollonFormatToDict(permission.First().Permission_Value);
                for (int i = 0; i < permissionDict.Keys.Count; i++)
                {
                    string campaignId = permissionDict.Keys.ElementAt(i);
                    string HierarchyIds = permissionDict.Values.ElementAt(i);
                    List<int> hierarchyList = Utility.GetIntList(HierarchyIds);
                    if (hierarchyList != null && hierarchyList.Count > 0)
                    {
                        string queryUsers = "";
                        for (int k = 0; k < hierarchyList.Count; k++)
                        {
                            queryUsers += "SELECT * FROM PITB.USERS WHERE IsActive = 1 AND Campaign_Id = " + campaignId + " AND Hierarchy_Id = " + hierarchyList[k] + "\n";
                        }
                        DataSet users = DBHelper.GetDataSetByQueryString(queryUsers, null);
                        if (users != null)
                        {
                            string pending = "1";
                            string overdue = "6";
                            string resolved = "3";
                            string total = "1,6,3";
                            List<DbPermissionsAssignment> statusPermissions = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId((int)Config.PermissionsType.Campaign, Int32.Parse(campaignId), (int)Config.CampaignPermissions.ExecutiveCampaignStatusReMap);
                            if (statusPermissions != null && statusPermissions.Count > 0)
                            {
                                Dictionary<string, string> statuses = Utility.ConvertCollonFormatToDict(statusPermissions.First().Permission_Value);
                                pending = statuses.First(x => x.Key.Equals("19")).Value.ToString();
                                overdue = statuses.First(x => x.Key.Equals("18")).Value.ToString();
                                resolved = statuses.First(x => x.Key.Equals("8")).Value.ToString();
                                total = string.Format("{0},{1},{2}", pending, overdue, resolved);
                            }
                            DataTableCollection tables = users.Tables;
                            for (int m = 0; m < tables.Count; m++)
                            {
                                IEnumerable<DataRow> usersIEnumerable = tables[m].Rows.Cast<DataRow>();
                                int hierarchyId = usersIEnumerable.First().Field<int>("Hierarchy_Id");
                                switch (hierarchyId)
                                {
                                    case (int)Config.Hierarchy.Province:
                                        string[] provinceIds = usersIEnumerable.GroupBy(z => z.Field<string>("Province_Id")).Select(g => g.First()).Select(c => c.Field<string>("Province_Id")).ToArray();
                                        sqlCommandText = string.Empty;
                                        for (int q = 0; q < provinceIds.Length; q++)
                                        {
                                            string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'Province' = d.Province_Name,'Province_Id' = c.Province_Id,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id INNER JOIN PITB.Provinces AS d ON c.Province_Id = d.Id WHERE c.Compaign_Id IN ({0})  AND c.Complaint_Type = 1 AND c.Province_Id IN(" + provinceIds[q] + ") AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name,d.Province_Name,c.Province_Id \n\n";
                                            sqlCommandText += String.Format(queryText, campaignId, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                                        }
                                        DataSet dspro = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                                        if (dspro != null)
                                        {
                                            for (int y = 0; y < dspro.Tables.Count; y++)
                                            {
                                                if (dspro.Tables[y].Rows.Count > 0)
                                                {
                                                    var row = dspro.Tables[y].Rows.Cast<DataRow>().First();
                                                    string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                    row.Field<string>("CampaignName") + "\n" +
                                                    row.Field<string>("Province") + " Province \n" + "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                    "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                    "Pending: " + row.Field<int>("Pending") + "\n" +
                                                    "Overdue: " + row.Field<int>("Overdue") + "\n";
                                                    SendSMSFunction(usersIEnumerable.Where(z => z.Field<string>("Province_Id") == row.Field<int>("Province_Id").ToString()).Select(x => x.Field<string>("Phone")).ToList<string>(), smsText);
                                                }
                                            }
                                        }
                                        break;
                                    case (int)Config.Hierarchy.Division:
                                        string[] divisionIds = usersIEnumerable.GroupBy(z => z.Field<string>("Division_Id")).Select(g => g.First()).Select(c => c.Field<string>("Division_Id")).ToArray();
                                        sqlCommandText = string.Empty;
                                        for (int q = 0; q < divisionIds.Length; q++)
                                        {
                                            string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'Division' = d.Division_Name,'Division_Id' = c.Division_Id,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id INNER JOIN PITB.Divisions AS d ON c.Division_Id = d.Id WHERE c.Compaign_Id IN ({0}) AND c.Complaint_Type = 1 AND c.Division_Id IN(" + divisionIds[q] + ") AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name,d.Division_Name,c.Division_Id \n\n";
                                            sqlCommandText += String.Format(queryText, campaignId, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                                        }
                                        DataSet dsdiv = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                                        if (dsdiv != null)
                                        {
                                            for (int y = 0; y < dsdiv.Tables.Count; y++)
                                            {
                                                if (dsdiv.Tables[y].Rows.Count > 0)
                                                {
                                                    var row = dsdiv.Tables[y].Rows.Cast<DataRow>().First();
                                                    string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                    row.Field<string>("CampaignName") + "\n" +
                                                    row.Field<string>("Division") + " Division \n" + "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                    "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                    "Pending: " + row.Field<int>("Pending") + "\n" +
                                                    "Overdue: " + row.Field<int>("Overdue") + "\n";
                                                    SendSMSFunction(usersIEnumerable.Where(z => z.Field<string>("Division_Id") == row.Field<int>("Division_Id").ToString()).Select(x => x.Field<string>("Phone")).ToList<string>(), smsText);
                                                }
                                            }
                                        }
                                        break;
                                    case (int)Config.Hierarchy.District:
                                        string[] districtIds = usersIEnumerable.GroupBy(z => z.Field<string>("District_Id")).Select(g => g.First()).Select(c => c.Field<string>("District_Id")).ToArray();
                                        sqlCommandText = string.Empty;
                                        for (int q = 0; q < districtIds.Length; q++)
                                        {
                                            string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'District' = d.District_Name,'District_Id' = c.District_Id,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id INNER JOIN PITB.Districts AS d ON c.District_Id = d.Id WHERE c.Compaign_Id IN ({0}) AND c.Complaint_Type = 1 AND c.District_Id IN(" + districtIds[q] + ") AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name,d.District_Name,c.District_Id \n\n";
                                            sqlCommandText += String.Format(queryText, campaignId, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                                        }
                                        DataSet dsdis = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                                        if (dsdis != null)
                                        {
                                            for (int y = 0; y < dsdis.Tables.Count; y++)
                                            {
                                                if (dsdis.Tables[y].Rows.Count > 0)
                                                {
                                                    var row = dsdis.Tables[y].Rows.Cast<DataRow>().First();
                                                    string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                    row.Field<string>("CampaignName") + "\n" +
                                                    row.Field<string>("District") + " District \n" + "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                    "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                    "Pending: " + row.Field<int>("Pending") + "\n" +
                                                    "Overdue: " + row.Field<int>("Overdue") + "\n";
                                                    SendSMSFunction(usersIEnumerable.Where(z => z.Field<string>("District_Id") == row.Field<int>("District_Id").ToString()).Select(x => x.Field<string>("Phone")).ToList<string>(), smsText);
                                                }
                                            }
                                        }
                                        break;
                                    case (int)Config.Hierarchy.Tehsil:
                                        string[] tehsilIds = usersIEnumerable.GroupBy(z => z.Field<string>("Tehsil_Id")).Select(g => g.First()).Select(c => c.Field<string>("Tehsil_Id")).ToArray();
                                        sqlCommandText = string.Empty;
                                        for (int q = 0; q < tehsilIds.Length; q++)
                                        {
                                            string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'Tehsil' = d.Tehsil_Name,'Tehsil_Id' = c.Tehsil_Id,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id INNER JOIN PITB.Tehsil AS d ON c.Tehsil_Id = d.Id WHERE c.Compaign_Id IN ({0}) AND c.Complaint_Type = 1 AND c.Tehsil_Id IN(" + tehsilIds[q] + ") AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name,d.Tehsil_Name,c.Tehsil_Id \n\n";
                                            sqlCommandText += String.Format(queryText, campaignId, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                                        }
                                        DataSet dsteh = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                                        if (dsteh != null)
                                        {
                                            for (int y = 0; y < dsteh.Tables.Count; y++)
                                            {
                                                if (dsteh.Tables[y].Rows.Count > 0)
                                                {
                                                    var row = dsteh.Tables[y].Rows.Cast<DataRow>().First();
                                                    string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                    row.Field<string>("CampaignName") + "\n" +
                                                    row.Field<string>("Tehsil") + " Tehsil \n" + "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                    "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                    "Pending: " + row.Field<int>("Pending") + "\n" +
                                                    "Overdue: " + row.Field<int>("Overdue") + "\n";
                                                    SendSMSFunction(usersIEnumerable.Where(z => z.Field<string>("Tehsil_Id") == row.Field<int>("Tehsil_Id").ToString()).Select(x => x.Field<string>("Phone")).ToList<string>(), smsText);

                                                }
                                            }
                                        }
                                        break;
                                    case (int)Config.Hierarchy.UnionCouncil:
                                        string[] UnionCouncilIds = usersIEnumerable.GroupBy(z => z.Field<string>("UnionCouncil_Id")).Select(g => g.First()).Select(c => c.Field<string>("UnionCouncil_Id")).ToArray();
                                        sqlCommandText = string.Empty;
                                        for (int q = 0; q < UnionCouncilIds.Length; q++)
                                        {
                                            string queryText = @"SELECT 'CampaignId' = c.Compaign_Id, 'CampaignName' = ca.Campaign_Name,'UnionCouncil' = d.Councils_Name,'UnionCouncil_Id' = c.UnionCouncil_Id,'Pending' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({1}) THEN 1 ELSE 0 END),0),
                                            'Resolved' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({2}) THEN 1 ELSE 0 END),0),
                                            'Overdue' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({3}) THEN 1 ELSE 0 END),0),
                                            'Total' = ISNULL(SUM(CASE WHEN c.Complaint_Computed_Status_Id IN ({4}) THEN 1 ELSE 0 END),0)
                                             FROM PITB.Complaints AS c INNER JOIN PITB.Campaign AS ca ON c.Compaign_Id = ca.Id INNER JOIN PITB.Union_Councils AS d ON c.UnionCouncil_Id = d.Id WHERE c.Compaign_Id IN ({0}) AND c.Complaint_Type = 1 AND c.UnionCouncil_Id IN(" + UnionCouncilIds[q] + ") AND CONVERT(DATE,c.Created_Date) >= '{5}' AND CONVERT(DATE,c.Created_Date) <= '{6}' GROUP BY c.Compaign_Id,ca.Campaign_Name,d.Councils_Name,c.UnionCouncil_Id \n\n";
                                            sqlCommandText += String.Format(queryText, campaignId, pending, resolved, overdue, total, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
                                        }
                                        DataSet dsUni = DBHelper.GetDataSetByQueryString(sqlCommandText, null);
                                        if (dsUni != null)
                                        {
                                            for (int y = 0; y < dsUni.Tables.Count; y++)
                                            {
                                                if (dsUni.Tables[y].Rows.Count > 0)
                                                {
                                                    var row = dsUni.Tables[y].Rows.Cast<DataRow>().First();
                                                    string smsText = "CM’s Complaint Center " + DateTime.Today.ToShortDateString() + "\n\n" +
                                                    row.Field<string>("CampaignName") + "\n" +
                                                    row.Field<string>("UnionCouncil") + " UnionCouncil \n" + "Total Complaints: " + row.Field<int>("Total") + "\n" +
                                                    "Resolved: " + row.Field<int>("Resolved") + "\n" +
                                                    "Pending: " + row.Field<int>("Pending") + "\n" +
                                                    "Overdue: " + row.Field<int>("Overdue") + "\n";
                                                    SendSMSFunction(usersIEnumerable.Where(z => z.Field<string>("UnionCouncil_Id") == row.Field<int>("UnionCouncil_Id").ToString()).Select(x => x.Field<string>("Phone")).ToList<string>(), smsText);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }
        private static bool SendSMSFunction(IEnumerable<string> phoneNos, string smsText)
        {
            new System.Threading.Thread(delegate()
            {
                SmsModel smsModel = new SmsModel((int)(-1), null, smsText,
                   (int)Config.MsgType.ToStakeholder,
                   (int)Config.MsgSrcType.WindowService, DateTime.Now, 1, (int)1);
                TextMessageHandler.SendMessageToPhoneNoList(phoneNos, smsText,true,smsModel);
            }).Start();
            return true;
        }

    }
}