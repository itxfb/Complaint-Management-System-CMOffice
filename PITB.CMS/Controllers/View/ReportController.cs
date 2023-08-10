using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Rotativa;
using System.Collections.Generic;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class ReportController : PITB.CMS_Common.Controllers.View.Controller
    {
        public ActionResult AgentWise()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("Crm/AgentWise", BlView.Instance.GetMasterPageFromCookie());
        }

        public ActionResult DepartmentWise()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("Crm/DepartmentWise", BlView.Instance.GetMasterPageFromCookie());
        }

        //[AuthorizePermission]
        public ActionResult dashboard(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));



            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            ViewBag.CampaignName = BlView.GetAlteredCampaignName(cmsCookie, dbCamp.Campaign_Name);
            //if (cmsCookie.navigationalData != null && cmsCookie.navigationalData.campaignName != null/* && dbCamp.Campaign_Name.ToLower() == "dc-office"*/)
            //    ViewBag.CampaignName = cmsCookie.navigationalData.campaignName;
            //else
            //    ViewBag.CampaignName = dbCamp.Campaign_Name;

            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampignIds = campaignId;
            return View("~/Views/Stakeholder/dashboard.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        public ActionResult PieChartReport(string campaignId)
        {
            ViewBag.campaignId = campaignId;
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Stakeholder/_PieChart.cshtml");
        }

        public ActionResult StackedChartReport(string campaignId)
        {
            ViewBag.campaignId = campaignId;
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Stakeholder/_StackedChart.cshtml");
        }

        public ActionResult LineChartReport(string campaignId)
        {
            ViewBag.campaignId = campaignId;
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Stakeholder/_LineChart.cshtml");
        }
        public ActionResult DashboardMain(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            // user cookie
            //VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(new AuthenticationHandler().CmsCookie.UserId, "2016-01-01", "2018-01-01", Config.UserWiseGraphType.CumulationOfLower);
            // end user cookie
            //BlView.Instance.SetMasterPage(this);
            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            //cookie.MasterPage = this.ViewBag.Layout;
            //new AuthenticationHandler().SaveCookie(cookie);
            return View("~/Views/Stakeholder/DashboardMain.cshtml", BlView.Instance.GetMasterPageFromCookie() /*BlView.Instance.SetMasterPageInCookie()*/);
        }
        public ActionResult DashboardSpecific1(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<SelectListItem> listStatuses = null;
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            if (!cookie.Campaigns.Contains(","))
            {
                ViewBag.LogoUrl =
                    DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
            }
            else
            {
                ViewBag.LogoUrl = null;
            }
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns);
            listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListingAll);

            ViewBag.StatusList = listStatuses;
            ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;

            ViewBag.ComplaintTypeList =
                                BlComplaintType.GetUserCategoriesAgainstCampaign(listCampaings);
            return View("~/Views/Stakeholder/DashboardSpecific1.cshtml");
        }

        public ActionResult DashboardTeachingQuality(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.GraphName = Utility.GetUserHierarchyValueString();
            ViewBag.HierarchyIdValue = Utility.GetUserHierarchyIdValue();
            //BlView.Instance.SetStartEndDate(this);
            // user cookie
            //VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(new AuthenticationHandler().CmsCookie.UserId, "2016-01-01", "2018-01-01", Config.UserWiseGraphType.CumulationOfLower);
            // end user cookie
            return View("~/Views/Stakeholder/DashboardTeachingQuality.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        public ActionResult DashboardPHCIPEducation(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.StartDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            //ViewBag.GraphName = Utility.GetUserHierarchyValueString();
            //ViewBag.HierarchyIdValue = Utility.GetUserHierarchyIdValue();
            //BlView.Instance.SetStartEndDate(this);
            // user cookie
            //VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(new AuthenticationHandler().CmsCookie.UserId, "2016-01-01", "2018-01-01", Config.UserWiseGraphType.CumulationOfLower);
            // end user cookie
            return View("~/Views/Stakeholder/DashboardPHCIPEducation.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        public ActionResult TablePhcipCategoryStatReport(string campaignId, string tableName, string startDate, string endData)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.StartDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            ViewBag.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.tableName = tableName;

            var data = BlReports.TablePhcipCategoryStatReport(startDate, endData);

            return PartialView("~/Views/Stakeholder/_PartialTablePhcipCategoryStats.cshtml", data);
        }

        public ActionResult AgingReport(string campaignId, Config.AgingReportType agingReportType)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignId = campaignId;
            ViewBag.GraphName = "Percentage (Resolved Unverified)";//Utility.GetUserHierarchyValueString();
            ViewBag.HierarchyIdValue = Utility.GetUserHierarchyIdValue();

            string masterPage = BlView.Instance.GetMasterPageFromCookie();
            // user cookie
            //VmStatusWiseComplaintsData statusWiseData = BlSchool.GetUsersDashboardData(new AuthenticationHandler().CmsCookie.UserId, "2016-01-01", "2018-01-01", Config.UserWiseGraphType.CumulationOfLower);
            // end user cookie
            switch (agingReportType)
            {
                case Config.AgingReportType.Monthly:
                    //masterPage = "~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml";
                    return View("~/Views/Stakeholder/AgingReportMonthly.cshtml", masterPage);
                    break;

                case Config.AgingReportType.Quarterly:
                    return View("~/Views/Stakeholder/AgingReportQuarterly.cshtml", masterPage);
                    break;

                case Config.AgingReportType.Yearly:
                    return View("~/Views/Stakeholder/AgingReportYearly.cshtml", masterPage);
                    break;
            }
            return View("~/Views/Stakeholder/AgingReportMonthly.cshtml", masterPage);
        }

        public ActionResult PieLegendChartCallVolumeReport(string campaignId, string divTag, Config.PieGraphTypes graphType = Config.PieGraphTypes.SchoolEducationCallVolume)
        {
            ViewBag.campaignId = campaignId;
            ViewBag.divTag = divTag;
            ViewBag.graphType = graphType;
            return PartialView("~/Views/Stakeholder/_PieLegendChartCallVolume.cshtml");
        }
        public ActionResult PieLegendChartTop5ComplaintCategoriesReport(string campaignId, string divTag, Config.PieGraphTypes graphType = Config.PieGraphTypes.SchoolEducationCompliantsByCategory)
        {
            ViewBag.campaignId = campaignId;
            ViewBag.divTag = divTag;
            ViewBag.graphType = graphType;
            return PartialView("~/Views/Stakeholder/_PieLegendChartTop5ComplaintCategories.cshtml");
        }
        public ActionResult PieLegendChartReport(string campaignId, string graphName, Config.PieGraphTypes graphType = Config.PieGraphTypes.None)
        {
            ViewBag.campaignId = campaignId;
            ViewBag.graphName = graphName;
            ViewBag.graphType = graphType;
            BlView.Instance.SetStartEndDate(this);

            return PartialView("~/Views/Stakeholder/_PieLegendChart.cshtml");
        }

        public ActionResult PieLegendChartPhcipReport(string campaignId, string graphName, Config.PieGraphTypes graphType = Config.PieGraphTypes.None)
        {
            ViewBag.campaignId = campaignId;
            ViewBag.graphName = graphName;
            ViewBag.graphType = graphType;
            BlView.Instance.SetStartEndDate(this);

            return PartialView("~/Views/Stakeholder/_PieLegendChartPhcip.cshtml");
        }

        [HttpPost]
        public ActionResult BarChartPhcipReport(string campaignId, string graphName, Config.PieGraphTypes graphType = Config.PieGraphTypes.None)
        {
            ViewBag.campaignId = campaignId;
            ViewBag.graphName = graphName;
            ViewBag.graphType = graphType;

            return PartialView("~/Views/Stakeholder/_BarChartPhcip.cshtml");
        }

        [HttpPost]
        public ActionResult BarChartReport(string campId, string GraphName, int UserId, int graphLevel, string categories)
        {
            ViewBag.campaignId = campId;
            ViewBag.graphName = GraphName.Trim();
            GraphName = GraphName.Replace(" ", "");

            GraphName = Utility.RemoveSpecialCharacters(GraphName);
            ViewBag.graphNameTrimmed = GraphName.Trim();

            ViewBag.UserId = UserId;
            ViewBag.Categories = categories;
            ViewBag.GraphLevel = graphLevel + 1;

            return PartialView("~/Views/Stakeholder/_BarChartStakeholder.cshtml");
        }

        [HttpPost]
        public ActionResult BarChartReportTeachingQuality(string campId, string GraphName, int UserId, int hierarchyId, int graphLevel)
        {
            ViewBag.campaignId = campId;
            GraphName = GraphName.Replace(" ", "");
            GraphName = Utility.RemoveSpecialCharacters(GraphName);
            ViewBag.graphName = GraphName.Trim();

            ViewBag.UserId = UserId;
            ViewBag.HierarchyId = hierarchyId;
            ViewBag.GraphLevel = graphLevel + 1;

            return PartialView("~/Views/Stakeholder/_BarChartStakeholderTeachingQuality.cshtml");
        }

        public ActionResult DistrictWiseGraphMain(string campaignId)
        {
            ViewBag.CampignIds = campaignId;
            return View("~/Views/Stakeholder/DistrictWiseGraphMain.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }



        public ActionResult DistrictWiseGraphReport(string campId, string GraphName, string StatusId)
        {
            ViewBag.campaignId = campId;
            ViewBag.graphName = GraphName.Trim();
            ViewBag.StatusId = StatusId;
            ViewBag.StatusName = new VmStatusWiseComplaintsData().GetStatusName(Convert.ToInt32(StatusId));
            ViewBag.StatusColorCode = new VmStatusWiseComplaintsData().GetColorCode(Convert.ToInt32(StatusId));
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Stakeholder/_DistrictWiseGraph.cshtml");
        }


        public ActionResult CategoryWiseDrillDownGraphMain(string campaignId)
        {
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;

            ViewBag.CampignIds = campaignId;
            ViewBag.GraphName = "Category";
            ViewBag.UserId = Utility.GetUserFromCookie().User_Id;
            ViewBag.CategoryType = Config.CategoryType.Main;
            ViewBag.CategoryId = -1;

            return View("~/Views/Stakeholder/CategoryWiseDrillDownMain.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }


        public ActionResult CategoryWiseDrillDownGraph(string campId, string graphName, int userId, int categoryType, string categoryId, int graphLevel)
        {
            ViewBag.graphName = graphName.Trim();
            graphName = graphName.Replace(" ", "");
            graphName = Utility.RemoveSpecialCharacters(graphName);
            ViewBag.graphNameTrimmed = graphName.Trim();

            ViewBag.campaignId = campId;
            //ViewBag.graphName = graphName.Trim();
            ViewBag.userId = userId;
            ViewBag.categoryType = categoryType;
            ViewBag.categoryId = categoryId;
            ViewBag.GraphLevel = graphLevel + 1;
            //ViewBag.StatusName = new VmStatusWiseComplaintsData().GetStatusName(Convert.ToInt32(StatusId));
            //ViewBag.StatusColorCode = new VmStatusWiseComplaintsData().GetColorCode(Convert.ToInt32(StatusId));
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Stakeholder/_BarChartCategoryWise.cshtml");
        }
        public ActionResult ShowCategoryInBarChart(string campId, string graphName, int userId, int? categoryType, string categoryId, int graphLevel, int divTag)
        {
            ViewBag.graphName = graphName.Trim();
            graphName = graphName.Replace(" ", "");
            graphName = Utility.RemoveSpecialCharacters(graphName);
            ViewBag.graphNameTrimmed = graphName.Trim();
            ViewBag.divTag = divTag;
            ViewBag.campaignId = campId;
            ViewBag.userId = userId;
            ViewBag.categoryType = categoryType;
            ViewBag.categoryId = categoryId;
            ViewBag.GraphLevel = graphLevel + 1;
            return PartialView("~/Views/Stakeholder/_BarChartCategoryWiseForDistrictReport.cshtml");
        }
        public ActionResult ShowCategoryInBarChartsPartialView(string campId, string graphName, int userId, int categoryType, string categoryId, int graphLevel)
        {
            ViewBag.graphName = graphName.Trim();
            graphName = graphName.Replace(" ", "");
            graphName = Utility.RemoveSpecialCharacters(graphName);
            ViewBag.graphNameTrimmed = graphName.Trim();

            ViewBag.campaignId = campId;
            //ViewBag.graphName = graphName.Trim();
            ViewBag.userId = userId;
            ViewBag.categoryType = categoryType;
            ViewBag.categoryId = categoryId;
            ViewBag.GraphLevel = graphLevel + 1;
            //ViewBag.StatusName = new VmStatusWiseComplaintsData().GetStatusName(Convert.ToInt32(StatusId));
            //ViewBag.StatusColorCode = new VmStatusWiseComplaintsData().GetColorCode(Convert.ToInt32(StatusId));

            return PartialView("~/Views/Stakeholder/_BarChartCategoryWise.cshtml");
        }
        public ActionResult MainSummaryReport(string campaignId)
        {

            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;

            ViewBag.CampignIds = campaignId;
            //ViewBag.GraphName = "DivisionWise";
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = Convert.ToInt32(dbUsers.Hierarchy_Id);
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.ReportType = Config.SummaryReportType.General;
            if (dbUsers.SubRole_Id == Config.SubRoles.SDU)
            {
                BlView.Instance.GetMasterPageFromCookie();//BlView.Instance.SetMasterPageInCookie("~/Views/Shared/SchoolEducation/_SduSchoolEducationStakeholderLayout.cshtml"); //"~/Views/Shared/SchoolEducation/_SduSchoolEducationStakeholderLayout.cshtml";
            }
            else
            {
                BlView.Instance.GetMasterPageFromCookie();//BlView.Instance.SetMasterPageInCookie("~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml");//"~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml";
            }

            //ViewBag.CampaignId = campaignId;
            //ViewBag.CategoryType = Config.CategoryType.Main;
            //ViewBag.CategoryId = -1;
            //return new ViewAsPdf("~/Views/Stakeholder/SchoolEducation/MainSummaryReport.cshtml") { IsLowQuality = true, CustomSwitches = "-d 80 " };
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/SchoolEducation/MainSummaryReport.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult DistrictReport(string campaignId)
        {

            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = Convert.ToInt32(dbUsers.Hierarchy_Id);
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.CampignIds = campaignId;
            ViewBag.GraphName = "Category";
            ViewBag.UserId = Utility.GetUserFromCookie().User_Id;
            ViewBag.CategoryType = Config.CategoryType.Main;
            ViewBag.CategoryId = -1;
            ViewBag.DistrictName = Utility.GetHierarchyValueName((Config.Hierarchy)Config.Hierarchy.District, (int)Convert.ToInt32(ViewBag.commaSepVal));
            int[] PostIDArray = { 1, 2, 3, 4, 5 };
            ViewBag.PostIDArray = PostIDArray;
            return View("~/Views/Stakeholder/SchoolEducation/DistrictReport.cshtml");
        }
        public ActionResult DivisionReport(string campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = Convert.ToInt32(dbUsers.Hierarchy_Id);
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.DivisionName = Utility.GetHierarchyValueName((Config.Hierarchy)Config.Hierarchy.Division, (int)Convert.ToInt32(ViewBag.commaSepVal));
            return View("~/Views/Stakeholder/SchoolEducation/DivisionReport.cshtml");
        }

        public ActionResult ProvinceReport(string campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = Convert.ToInt32(dbUsers.Hierarchy_Id) + 1;
            ViewBag.UserHierarchyId = Convert.ToInt32(dbUsers.User_Hierarchy_Id);
            //ViewBag.commaSepVal = DbUsers.GetHierarchy((Config.Hierarchy)dbUsers.Hierarchy_Id, dbUsers);
            ViewBag.commaSepVal = DbDistrict.GetDistrictsByProvinceId(Convert.ToInt32(dbUsers.Hierarchy_Id));
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/SchoolEducation/ProvinceReport.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        //[HttpPost]
        public ActionResult BarChartMainSummary(string graphName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string graphTag)
        {
            ViewBag.graphName = graphName;
            ViewBag.graphNameTrimmed = Regex.Replace(graphName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.graphTag = graphTag;
            ViewBag.divTag = graphTag;

            return PartialView("~/Views/Stakeholder/SchoolEducation/_BarChartMainSummary.cshtml");
        }
        public ActionResult DistrictReportsForProvince(string campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            ViewBag.CampaignIds = campaignId;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            ViewBag.DistrictList = ToSelectList(DbDistrict.GetDistrictsNameAndIDByProvinceId(Convert.ToInt32(dbUsers.Hierarchy_Id)));
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/SchoolEducation/DistrictReportsForProvince.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        [NonAction]
        private SelectList ToSelectList(IEnumerable<object> pList)
        {
            return new SelectList(pList, "DistrictId", "DistrictName");
        }
        public ActionResult DivisionReportsForProvince(string campaignId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            ViewBag.CampaignIds = campaignId;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            ViewBag.DivisionList = new SelectList(DbDivision.GetDivisionsNameAndIDByProvinceId(Convert.ToInt32(dbUsers.Hierarchy_Id)), "DivisionId", "DivisionName");
            BlView.Instance.SetStartEndDate(this);
            return View("~/Views/Stakeholder/SchoolEducation/DivisionReportsForProvince.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult ProvinceDistrictPartialView(string startDate, string endDate, string provinceId, string campaignId, string districtId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampaignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = 3;
            ViewBag.UserHierarchyId = 0;
            ViewBag.commaSepVal = districtId;
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.GraphName = "Category";
            ViewBag.UserId = Utility.GetUserFromCookie().User_Id;
            ViewBag.CategoryType = Config.CategoryType.Main;
            ViewBag.CategoryId = -1;
            ViewBag.DistrictName = Utility.GetHierarchyValueName((Config.Hierarchy)Config.Hierarchy.District, (int)Convert.ToInt32(ViewBag.commaSepVal));
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.DistrictId = districtId;
            ViewBag.ProvinceId = provinceId;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            return PartialView("~/Views/Stakeholder/SchoolEducation/_ProvinceDistricts.cshtml");
        }
        public ActionResult ProvinceDivisionPartialView(string startDate, string endDate, string provinceId, string campaignId, string divisionId)
        {
            DbUsers dbUsers = Utility.GetUserFromCookie();
            DbCampaign dbCamp = DbCampaign.GetById(Convert.ToInt32(campaignId));
            ViewBag.LogoUrl = dbCamp.LogoUrl;
            ViewBag.CampaignName = dbCamp.Campaign_Name;
            ViewBag.CampaignIds = campaignId;
            ViewBag.UserId = dbUsers.User_Id;
            ViewBag.HierarchyId = 2;
            ViewBag.UserHierarchyId = 0;
            ViewBag.commaSepVal = divisionId;
            ViewBag.ReportType = Config.SummaryReportType.General;
            ViewBag.GraphName = "Category";
            ViewBag.UserId = Utility.GetUserFromCookie().User_Id;
            ViewBag.CategoryType = Config.CategoryType.Main;
            ViewBag.CategoryId = -1;
            ViewBag.DivisionName = Utility.GetHierarchyValueName((Config.Hierarchy)Config.Hierarchy.Division, (int)Convert.ToInt32(ViewBag.commaSepVal));
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.DivisionId = divisionId;
            ViewBag.ProvinceId = provinceId;
            ViewBag.ProvinceName = Utility.GetHierarchyValueName(Config.Hierarchy.Province, (int)dbUsers.Hierarchy_Id);
            return PartialView("~/Views/Stakeholder/SchoolEducation/_ProvinceDivisions.cshtml");
        }
        public ActionResult OverDueComplaintsSummary(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_OverdueComplaintsSummary.cshtml");
        }
        public ActionResult DistrictReportComplaintsSummary(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];
            if (divName.Equals("Open"))
            {
                return PartialView("~/Views/Stakeholder/SchoolEducation/_DistrictOpenComplaintsSummary.cshtml");
            }
            else if (divName.Equals("ReOpened"))
            {
                return PartialView("~/Views/Stakeholder/SchoolEducation/_DistrictReOpenedComplaintsSummary.cshtml");
            }
            else if (divName.Equals("Overdue"))
            {
                return PartialView("~/Views/Stakeholder/SchoolEducation/_DistrictOverdueComplaintsSummary.cshtml");
            }
            return View();
        }
        public ActionResult RegionAndStatusWiseCount(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_RegionStatusWiseSummary.cshtml");
        }
        public ActionResult RegionAndStatusWiseCountForProvinceDistrict(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_RegionStatusWiseCountForProvinceDistrict.cshtml");
        }
        public ActionResult CategorywiseAndStatusWiseCount(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, string categoryIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.categoryIds = categoryIds;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_CategorywiseAndStatuswiseCount.cshtml");
        }
        public ActionResult TopOverdueComplaintsByOfficer(string divName, string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, int reportType, string divTag)
        {
            ViewBag.divName = divName;
            ViewBag.divNameTrimmed = Regex.Replace(divName, @"[^0-9a-zA-Z]+", "").Replace(" ", "").Trim();
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.reportType = reportType;
            ViewBag.divTag = divTag;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_TopOverdueComplaintsByOfficers.cshtml");
        }

        public ActionResult ComplaintsSummary(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, string statusIds, string divTag)
        {
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.campId = campId;
            ViewBag.hierarchyId = hierarchyId;
            ViewBag.divTag = divTag;
            ViewBag.userHierarchyId = userHierarchyId;
            ViewBag.commaSepVal = commaSepVal;
            ViewBag.statusIds = statusIds;
            ViewBag.Hierarchy1 = Config.ScoolEducationRegionDict[hierarchyId + 1];
            ViewBag.Hierarchy2 = Config.ScoolEducationRegionDict[hierarchyId + 2];

            return PartialView("~/Views/Stakeholder/SchoolEducation/_ComplaintsSummary.cshtml");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult OverdueComplaintsByDivisionReports(string divName, string startDate, string endDate)
        //{
        //    ViewBag.divName = divName;
        //}

        public ActionResult ComplaintCategoriesWiseRegionWiseCount(string startDate, string endDate, string campId, string hierarchyId, string userHierarchyId, string commaSepVal, string statusIds, string divTag, string complaintCategories)
        {
            ViewBag.StartDate = startDate;
            ViewData["EndDate"] = endDate;
            ViewData["CampaignId"] = campId;
            ViewData["HierarchyId"] = hierarchyId;
            ViewData["UserHierarchyId"] = userHierarchyId;
            ViewData["commaSepVal"] = commaSepVal;
            ViewData["statusIds"] = statusIds;
            ViewData["divTag"] = divTag;
            ViewData["CategoriesId"] = complaintCategories;
            string[] cats = complaintCategories.Split(new char[',']);
            List<int> catList = new List<int>();
            for (int i = 0; i < cats.Length; i++)
            {
                catList.Add(Convert.ToInt32(cats[i]));
            }
            ViewData["Heading"] = string.Join(", ", DbComplaintType.GetByTypeIds(catList).Select(n => n.Name).ToArray<string>());
            return PartialView("~/Views/Stakeholder/SchoolEducation/_ComplaintCategoryWiseRegionWiseCount.cshtml");

        }
        public ActionResult StakeholderComplaintsListingPercentageExpiryViewServerSide(int userId, string from, string to, string statusId, string tabType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            int canShowStatusChangeInDetail = Convert.ToInt32(userId == cookie.UserId);
            DbUsers dbUser = DbUsers.GetActiveUser(userId);

            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromDbUser(dbUser);
            ViewBag.LogoUrl =
                DbCampaign.GetLogoUrlByCampaignId(
                    Utility.GetIntByCommaSepStr(dbUser.Campaigns/*AuthenticationHandler.GetCookie().Campaigns*/));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList =
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, dbUser);

            List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUser.User_Id);
            ViewBag.ListPermissions = listPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.UserId = dbUser.User_Id;
            ViewBag.CanShowStatusChangeInDetail = canShowStatusChangeInDetail;

            List<SelectListItem> listStatuses = null;

            if (tabType == Config.StakeholderComplaintListingType.AssignedToMe.ToString())
            {
                ViewBag.PageHeading = "Complaints (My)";
                ViewBag.ListingType = (int)Config.StakeholderComplaintListingType.AssignedToMe;
                listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
            }
            else if (tabType == Config.StakeholderComplaintListingType.UptilMyHierarchy.ToString())
            {
                ViewBag.PageHeading = "Complaints (Subordinates)";
                ViewBag.ListingType = (int)Config.StakeholderComplaintListingType.UptilMyHierarchy;
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListingAll);
            }
            //}
            //int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            /*List<SelectListItem> listStatuses = null;

            //if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds)) // if is school education new campaign
            {
                if (canShowStatusChangeInDetail == 1)
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId)).Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
                    }
                    else
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
                    }
                }
                else
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId))
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                    else
                    {

                        listStatuses =
                            DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses)
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                }*/
            listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.UnsatisfactoryClosedStatus);
            ViewBag.StatusList = listStatuses;
            ViewBag.TabType = tabType;
            return View("~/Views/Stakeholder/ZimmedarShehri/_ComplaintListingNotification.cshtml");
            //}
            /*else
            {
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList());
                ViewBag.StatusList = listStatuses;
                return View("~/Views/Stakeholder/ComplaintListingsServerSide.cshtml");
            }*/

        }
        public ActionResult HierarchyWiseReport(int hierarchyId, string campaignIds, string statusIds)
        {
            ViewBag.HierarchyId = hierarchyId;
            ViewBag.HierarchyName = Utility.GetHierachyNameByHierarchyId(hierarchyId);
            ViewBag.CampaignName = DbCampaign.GetSingleOrCumulativeCampaignName(campaignIds);
            ViewBag.CampaignIds = campaignIds;
            ViewBag.StatusIds = statusIds;
            ViewBag.LogoUrl = DbCampaign.GetLogoUrlForCumulativeCampaignIds(campaignIds);
            return View("~/Views/Report/HierarchyWiseReport.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult HierarchyWiseReportPartialView(int hierarchyId, string campaignIds, string statusIds)
        {
            ViewBag.HierarchyId = hierarchyId;
            ViewBag.HierarchyName = Utility.GetHierachyNameByHierarchyId(hierarchyId);
            ViewBag.CampaignName = DbCampaign.GetSingleOrCumulativeCampaignName(campaignIds);
            ViewBag.CampaignIds = campaignIds;
            ViewBag.StatusIds = statusIds;
            List<DbStatus> statuses = DbStatus.GetByStatusIds(Utility.GetIntList(statusIds));
            Dictionary<int, string> statusesHeaderName = new Dictionary<int, string>();
            Dictionary<int, string> statusesDBColumnName = new Dictionary<int, string>();
            foreach (var status in statuses)
            {
                var headerName = status.Status;
                var dbColumnName = status.Status.Replace(' ', '_').Replace('(', '_').Replace(')', '_');
                statusesHeaderName.Add(status.Complaint_Status_Id, headerName);
                statusesDBColumnName.Add(status.Complaint_Status_Id, dbColumnName);
            }
            ViewBag.StatusesHeaderName = statusesHeaderName.OrderBy(x => x.Key);
            ViewBag.StatusesDBColumnName = statusesDBColumnName.OrderBy(x => x.Key);
            ViewBag.LogoUrl = DbCampaign.GetLogoUrlForCumulativeCampaignIds(campaignIds);
            BlView.Instance.SetStartEndDate(this);
            return PartialView("~/Views/Report/_HierarchyWiseReportPartialView.cshtml");
        }

        public string RegionWiseResolvedComplaints()
        {
            CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);

            return "";
        }

        public ActionResult GetViewRegionWiseResolvedComplaintReport()
        {
            return View("~/Views/Stakeholder/RegionWiseResolvedComplaints.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        public JsonResult /*JsonResult*/ GetRegionWiseResolvedComplaintsResult()
        {


            //CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            //CustomForm.Post postForm = new CustomForm.Post(Request);
            //PoliceModel.AddAction addActionModel = new PoliceModel.AddAction(cmsCookie, postForm, DateTime.Now, Config.SourceType.Web/*, Config.OtherSystemId.Police*/, postForm.DictQueryParams["complaintId"]);
            //commandMessage = BlPolice.SaveComplaintAction(addActionModel /*new CustomForm.Post(Request)*/);
            //resp.RedirectUrl = Url.Action("StakeholderComplaintsListingServerSide", "Complaint", new { ComplaintType = (int)Config.ComplaintType.Complaint });

            //resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMessage);
            //ControllerModel.Response resp = new ControllerModel.Response();
            //HtmlTableModel htmlTableModel = null;
            //var json = JsonConvert.SerializeObject(htmlTableModel);
            CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
            dynamic response = BlReports.ParamsForRegionWiseResolvedComplaintReport(postForm);

            //json = json.Replace("[", "{ \"data\": [");
            //json = json.Replace("]", "] }");

            JsonResult jsResult = Json(response, JsonRequestBehavior.AllowGet);
            jsResult.MaxJsonLength = 999999999;
            return jsResult;
            //return Json(resp);

            //return View("~/Views/Stakeholder/RegionWiseResolvedComplaints.cshtml");
        }
    }


}