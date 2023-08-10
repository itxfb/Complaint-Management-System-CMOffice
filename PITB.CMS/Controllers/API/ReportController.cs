//using System.Collections.Generic;
using System.Data;
//using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Web.Mvc;
using HttpGet = System.Web.Http.HttpGetAttribute;
using HttpPost = System.Web.Http.HttpPostAttribute;

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PITB.CMS_Common.Models.View.Reports;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using System.Linq;

namespace PITB.CMS.Controllers.API
{
    public class ReportController : ApiController
    {
        #region CRM Reports

        [HttpPost]
        public dynamic AgentWiseCount([FromBody] VmReportListing model)
        {
            if (model == null) return new { aaData = new List<string>() };
            return new { aaData = BlReports.GetAgentWiseCount(model.From, model.To, model.Agents.ToCommaSepratedString(), model.Campaigns.ToCommaSepratedString()) };
        }

        [HttpPost]
        public dynamic DepartmentWiseCount([FromBody] VmReportListing model)
        {
            if (model == null) return new { aaData = new List<string>() };

            return new { aaData = BlReports.GetDepartmentWiseCount(model.From, model.To, model.Departments.ToCommaSepratedString(), model.Campaigns.ToCommaSepratedString()) };
        }

        #endregion

        [HttpGet]
        public string PieChartProgress(string datelow, string datemax, string campaignId)
        {
            DataSet ds = BlDashboard.GetChartData(datelow, datemax, BlDashboard.Flag.CategoryAndSubCategory, campaignId);
            var data = new
            {
                DistrictTotalSum = ds.Tables[0],
                Detail = ds.Tables[1]
            };
            return JsonConvert.SerializeObject(data);
        }


        [HttpGet]
        public string StackedChartProgress(string datelow, string datemax, string campaignId)
        {
            DataSet ds = BlDashboard.GetChartData(datelow, datemax, BlDashboard.Flag.CategoryWiseResolvedPending, campaignId);
            var data = new
            {
                CategoryTable = ds.Tables[0],
            };
            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string LineChart(string datelow, string datemax, string campaignId)
        {
            DataSet ds = BlDashboard.GetChartData(datelow, datemax, BlDashboard.Flag.CategoryWiseLineChart, campaignId);
            var data = new
            {
                CategoryTable = ds.Tables[0],
            };
            return JsonConvert.SerializeObject(data);
        }
        [HttpGet]
        public JsonResult GetUsersVerificationAndPasswordCount(string Campaigns)
        {
            Dictionary<string, int> data = JsonConvert.DeserializeObject<Dictionary<string, int>>(BIStakeholderAdmin.GetVerificationAndPasswordChangedCount(Int32.Parse(Campaigns)));
            JArray array = new JArray();
            array.Add(data["verificationcodecount"]);
            JArray array2 = new JArray();
            array2.Add(data["passwordchangedcount"]);
            JObject o = new JObject();
            o["VerificationCount"] = array;
            o["PasswordCount"] = array2;
            return new JsonResult() { Data = o.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /*[HttpGet]
        public JsonResult GetAllUser(string Campaigns, int Hierarchy_id)
        {
            return new JsonResult() { Data = BIStakeholderAdmin.GetComplaintsOfAgents(Campaigns, Hierarchy_id).ToDynamic(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //var json = JsonConvert.SerializeObject(data);
            //json = json.Replace("[", "{ \"aaData\": [");
            //json = json.Replace("]", "] }");
            //return json;
        }*/

        [HttpPost]
        public JsonResult GetAllUserP([FromBody] JObject data)
        {
            string aoData = data["aoData"].ToString();
            string campaigns = data["campaigns"].ToString();
            int hierarchy_id = (int)data["hierarchy_id"];

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            Tuple<int, DataTable> dup = BIStakeholderAdmin.GetUsersList_P(campaigns, hierarchy_id, dtModel);
            int totalRows = dup.Item1;

            return new JsonResult()
            {
                Data = new
                {
                    data = dup.Item2,
                    draw = dtModel.Draw,
                    recordsTotal = totalRows,//dtModel.Length,
                    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public string PieLegendChartComplaintTypeWise(string datelow, string datemax, int campaignId, string divTag, int graphType)
        {
            VmComplaintTypeWisePieChart viewComplaintTypeWise = new VmComplaintTypeWisePieChart();
            Config.PieGraphTypes pieGraphType = (Config.PieGraphTypes)graphType;
            if (pieGraphType == Config.PieGraphTypes.SchoolEducationCallVolume)
            {

                viewComplaintTypeWise = BlReports.PieLegentChartCallVolume(datelow, datemax, campaignId);
            }
            else if (pieGraphType == Config.PieGraphTypes.SchoolEducationCompliantsByCategory)
            {
                viewComplaintTypeWise = BlReports.PieLegentChartCompliantsByCategory(datelow, datemax, campaignId);
            }
            var data = new { Total = viewComplaintTypeWise };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]

        public string PieLegentChartProgress(string datelow, string datemax, int campaignId, string graphName, int graphType)
        {

            VmStakeholderPieChart totalCount = new VmStakeholderPieChart();
            //List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();
            Config.PieGraphTypes pieGraphTypes = (Config.PieGraphTypes)graphType;

            if (pieGraphTypes == Config.PieGraphTypes.None ||
                pieGraphTypes == Config.PieGraphTypes.SchoolEducationStatuses)
            {

                if (graphName == "Own")
                    totalCount = BlReports.PieLegentChartProgress(datelow, datemax, campaignId,
                        Config.UserWiseGraphType.MyOwn);
                else
                    totalCount = BlReports.PieLegentChartProgress(datelow, datemax, campaignId,
                        Config.UserWiseGraphType.CumulationOfLower);
            }
            else if (pieGraphTypes == Config.PieGraphTypes.SchoolEducationTeachingQuality)
            {
                if (graphName == "Own")
                    totalCount = BlReports.PieLegentChartProgressFromTeachingQualitySchoolEducation(datelow, datemax, campaignId,
                        Config.UserWiseGraphType.MyOwn);
            }
            else if (pieGraphTypes == Config.PieGraphTypes.SchoolEducationPhcipDashboard)
            {
                if (graphName == "complaintsStatus")
                {
                    totalCount = BlReports.PieChartPhcipDashboardComplaintsStatusStats(datelow, datemax);
                }
                else if (graphName == "complaintsSource")
                {
                    totalCount = BlReports.PieChartPhcipDashboardComplaintsSource(datelow, datemax);
                }
                //else if (graphName == "complaintsByStakeholder")
                //{

                //}
            }
            //VmStakeholderPieChart totalCount = new VmStakeholderPieChart();

            //totalCount.HierarchyId = AuthenticationHandler.GetCookie().User_Hierarchy_Id.ToString();

            //totalCount.ListStatusWiseCount = temp;

            var data = new { Total = totalCount };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string BarChartUserWise(string datelow, string datemax, int campaignId, int UserId, string categories)
        {
            VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

            if (campaignId == 47)
            {
                temp = BlReports.BarChartUserWise(datelow, datemax, campaignId, Config.UserWiseGraphType.LowerIndividual,
                    UserId);
            }
            else
            {
                temp = BlCommonReport.BarChartUserWise(datelow, datemax, campaignId, Config.UserWiseGraphType.LowerIndividual,
                    UserId, categories);
            }

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string BarChartPhcipDashboard(string datelow, string datemax, int campaignId)
        {
            VmStakeholderBarChart chartData = new VmStakeholderBarChart();
            chartData = BlReports.BarChartPhcipDashboard(datelow, datemax, campaignId);

            var data = new { Total = chartData };
            return JsonConvert.SerializeObject(data);
        }


        [HttpGet]
        public string BarChartAgingReport(string dateFirst, string dateSecond, int campaignId, Config.AgingReportType agingReportType)
        {
            VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

            temp = BlReports.BarChartAgingReport(dateFirst, dateSecond, campaignId, agingReportType);

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string BarChartUserWiseTeachingQuality(string datelow, string datemax, int campaignId, int UserId, int hierarchyId)
        {
            //ViewBag.HierarchyId = hierarchyId - 1;

            VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

            temp = BlReports.BarChartUserWiseTeachingQuality(datelow, datemax, campaignId, Config.UserWiseGraphType.LowerIndividual, UserId, hierarchyId);

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string DistrictWiseGraph(string datelow, string datemax, int campaignId, int StatusId)
        {
            DataTable temp = new DataTable();

            temp = BlReports.DistrictWiseGraph(datelow, datemax, campaignId, StatusId);

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }


        [HttpGet]
        public string BarChartCategoryWise(string dateFirst, string dateSecond, int campaignId, int categoryType, int categoryId)
        {
            VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

            temp = BlReports.BarChartCategoryWiseDashboardData(dateFirst, dateSecond, campaignId, (Config.CategoryType)categoryType, categoryId);

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        public string BarChartDivisionWise(string dateFirst, string dateSecond, int campaignId, int categoryType, int categoryId)
        {
            VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

            temp = BlReports.BarChartCategoryWiseDashboardData(dateFirst, dateSecond, campaignId, (Config.CategoryType)categoryType, categoryId);

            var data = new { Total = temp };

            return JsonConvert.SerializeObject(data);
        }

        //[HttpGet]
        //public string BarChartMainSummary(string dateFirst, string dateSecond, int campaignId, int categoryType, int categoryId)
        //{
        //    VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

        //    temp = BlReports.BarChartCategoryWiseDashboardData(dateFirst, dateSecond, campaignId, (Config.CategoryType)categoryType, categoryId);

        //    var data = new { Total = temp };

        //    return JsonConvert.SerializeObject(data);
        //}



        //[HttpGet]
        //public string BarChartMainSummary(string startDate, string endDate, string campId, int hierarchyId, int userHierarchyId, string commaSepVal, int reportType, string graphTag)
        //{
        //    VmStatusWiseComplaintsData temp = new VmStatusWiseComplaintsData();

        //    temp = BlSchoolReports.GetTertiaryCategoryWiseData(dateFirst, dateSecond, campaignId, (Config.CategoryType)categoryType, categoryId);

        //    var data = new { Total = temp };

        //    return JsonConvert.SerializeObject(data);
        //}
    }
}