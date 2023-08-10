using System;
using System.Collections.Generic;
using System.Data;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.DataTable;

namespace PITB.CMS_Handlers.Business
{
    public class BlAgent
    {
        public static DataTable GetComplaintListings(DataTableParamsModel dtModel, string fromDate, string toDate, string campaign, int complaintType, string spType, int listingType)
        {
            List<string> prefixStrList = new List<string> { "complaints", "campaign", "personalInfo", "complaints", "complaintType", "Statuses" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintNo" }, prefixStrList, dictFilterQuery);
            ListingParamsAgent paramsComplaintListing = SetAgentListingParams(dtModel, fromDate, toDate, campaign, (Config.ComplaintType)complaintType, spType, listingType);
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
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> dictParam = new Dictionary<string, object>();
            dictParam.Add("@fromDate", Utility.GetDateTimeStr(fromDate));
            dictParam.Add("@toDate", Utility.GetDateTimeStr(toDate));
            dictParam.Add("@campaignIds", campaign);
            dictParam.Add("@createdBy", cookie.UserId);

            if (dtModel != null)
            {
                dictParam.Add("@StartRow", dtModel.Start);
                dictParam.Add("@EndRow", dtModel.End);
                dictParam.Add("@OrderByColumnName", dtModel.ListOrder[0].columnName);
                dictParam.Add("@OrderByDirection", dtModel.ListOrder[0].sortingDirectionStr);
                dictParam.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch);
            }

            string queryStr = null;
            //if (spType == "Export")
            //{
            //    queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
            //}
            string exportStr = "";
            if (spType == "Export")
            {
                exportStr = "_Export";
            }
            // If listing added by me

            if (cookie.Role == Config.Roles.Agent || cookie.Role == Config.Roles.AgentSuperVisor)
            {

                if (listingType == (int)Config.AgentComplaintListingType.AddedByMe)
                {
                    if (complaintType == (int)Config.ComplaintType.Complaint)
                    {
                        queryStr = QueryHelper.GetFinalQuery("Agent_ComplaintsListing_Mine" + exportStr, Config.ConfigType.Query,
                            dictParam);
                    }
                    if (complaintType == (int)Config.ComplaintType.Suggestion)
                    {
                        queryStr = QueryHelper.GetFinalQuery("Agent_ComplaintsListing_Suggestion" + exportStr, Config.ConfigType.Query,
                                dictParam);
                    }
                    else if (complaintType == (int)Config.ComplaintType.Inquiry)
                    {
                        queryStr = QueryHelper.GetFinalQuery("Agent_ComplaintsListing_Inquiry" + exportStr, Config.ConfigType.Query,
                                dictParam);
                    }
                }
                else if (listingType == (int)Config.AgentComplaintListingType.All)
                {
                    queryStr = QueryHelper.GetFinalQuery("Agent_ComplaintsListing_All" + exportStr, Config.ConfigType.Query,
                        dictParam);
                }
            }

            //string asd = AgentListingLogic.GetListingQuery(paramsComplaintListing);


            //string queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
            return DBHelper.GetDataTableByQueryString(queryStr, null);
        }

        public static DataTable GetAlterDataTable(DataTable dt)
        {
            foreach (DataRow dtRow in dt.Rows)
            {
                // On all tables' columns
                if (Convert.ToInt32(dtRow["Campaign_Id"]) == (int)Config.Campaign.SchoolEducationEnhanced)
                {
                    dtRow["Status"] = Utility.GetAlteredStatus(dtRow["Status"].ToString(), Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                }
            }

            return dt;
        }

        public static ListingParamsAgent SetAgentListingParams(DataTableParamsModel dtParams, string fromDate, string toDate, string campaign, Config.ComplaintType complaintType, string spType, int listingType)
        {
            string extraSelection = "";

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsAgent paramsModel = new ListingParamsAgent();
            paramsModel.StartRow = dtParams.Start;
            paramsModel.EndRow = dtParams.End;
            paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
            paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
            paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            DateTime fromDateRes;
            DateTime toDateRes;
            if (DateTime.TryParseExact(fromDate, new string[] { "d/M/yyyy", "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateRes))
            {
                paramsModel.From = fromDateRes.ToString("MM/dd/yyyy");
            }
            else
            {
                paramsModel.From = fromDate;
            }
            if (DateTime.TryParseExact(toDate, new string[] { "d/M/yyyy", "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out toDateRes))
            {
                paramsModel.To = toDateRes.ToString("MM/dd/yyyy");
            }
            else
            {
                paramsModel.To = toDate;
            }
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
            //paramsModel.SelectionFields = extraSelection;
            paramsModel.SpType = spType;
            return paramsModel;
        }
    }
}