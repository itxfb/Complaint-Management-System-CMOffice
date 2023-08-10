using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System;
using OfficeOpenXml;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom.Reports;
using PITB.CMS_Common.Handler.ExportFileHandler;
using System.Dynamic;

namespace PITB.CMS.Controllers.API
{
    public class ApiSchoolEducationController : Controller
    {
        // GET: ApiSchoolEducation
       
        [System.Web.Mvc.HttpPost]
        public JsonResult /*string*/ GetStakeholderComplaintsServerSide([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId=-1)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            //List<VmStakeholderComplaintListingSchoolEducation> dataTable = BlSchool.GetStakeHolderServerSideListDenormalized(
            //    from,
            //    to,
            //    dtModel,
            //    commaSeperatedCampaigns,
            //    commaSeperatedCategories,
            //    commaSeperatedStatuses,
            //    commaSeperatedTransferedStatus,
            //    complaintType,
            //    (Config.StakeholderComplaintListingType)listingType,
            //    "Listing",
            //    userId).ToList<VmStakeholderComplaintListingSchoolEducation>();


            //---------- new code -----------
            //List<List<dynamic>> listD = BlSchool.GetStakeHolderServerSideListDenormalized(
            //    from,
            //    to,
            //    dtModel,
            //    commaSeperatedCampaigns,
            //    commaSeperatedCategories,
            //    commaSeperatedStatuses,
            //    commaSeperatedTransferedStatus,
            //    complaintType,
            //    (Config.StakeholderComplaintListingType)listingType,
            //    "Listing",
            //    userId);

            //int totalRows = listD[0].Count == 0 ? 0 : listD.First()[0].count;

            //dynamic tableData = new ExpandoObject();
            //tableData.data = listD[1];
            //tableData.draw = dtModel.Draw++;
            //tableData.recordsTotal = totalRows;
            //tableData.recordsFiltered = totalRows;

            //return JsonConvert.SerializeObject(tableData); ;

            //--------- end new code ----------

            List<VmStakeholderComplaintListingSchoolEducation> dataTable = BlSchool.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "Listing",
                userId).ToList<VmStakeholderComplaintListingSchoolEducation>();

            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult /*HttpResponseBase*/ ExportStakeHolderDataToExcel([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {
            

            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable data = BlSchool.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "ExcelReport",
                userId);

            int rowCount = data.Rows.Count;
            string fileName = DbCampaign.GetById(Int32.Parse(campaign.First())).Campaign_Name;
            string startDate = from;
            string endDate = to;
            ExcelPackage excelPack = FileHandler.ExportToExcel(data, "Complaint Listing Data");
            int dataId = DataStateMVC.AddInPool(excelPack,fileName,startDate,endDate);

            return Json(dataId, JsonRequestBehavior.AllowGet);
             
            return null;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetStakeholderExpiringComplaintsList([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {
            try
            {
                string spType = "ListingExpiring";//"ListingExpiring";

                string commaSeperatedCampaigns = string.Join(",", campaign);
                string commaSeperatedCategories = string.Join(",", cateogries);
                string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
                string commaSeperatedStatuses = "";
                if (statuses != null)
                {
                    commaSeperatedStatuses = string.Join(",", statuses);
                }

                DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

                List<VmStakeholderComplaintListingSchoolEducation> dataTable = BlSchool.GetStakeHolderServerSideListDenormalized(
                    from,
                    to,
                    dtModel,
                    commaSeperatedCampaigns,
                    commaSeperatedCategories,
                    commaSeperatedStatuses,
                    commaSeperatedTransferedStatus,
                    complaintType,
                    (Config.StakeholderComplaintListingType)listingType,
                    spType,
                    userId).ToList<VmStakeholderComplaintListingSchoolEducation>();

                int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

                return Json(new
                {
                    data = dataTable,
                    draw = dtModel.Draw,
                    recordsTotal = totalRows,//dtModel.Length,
                    recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [System.Web.Mvc.HttpPost]
        public JsonResult GetDashboardLabelsStakeholderData([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] string dashboardType, [FromBody] int userId = -1)
        {
            string spType = "";
            if (dashboardType == "_DashboardLabelStatus")
            {
                spType = "DashboardLabelsStausWise";
            }
            else if (dashboardType == "_DashboardLabelComplaintSrc")
            {
                spType = "DashboardLabelsComplaintSrc";
            }
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            List<VmStakeholderComplaintDashboard> data = BlSchool.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                spType,
                userId).ToList<VmStakeholderComplaintDashboard>();

            if(spType== "DashboardLabelsStausWise")
            {
                data.Add(new VmStakeholderComplaintDashboard { Id = -1, Name = "Total", Count = data.Sum(s => s.Count) });
            }

            //ExcelPackage data = (ExcelPackage) DataStateMVC.GetDataFromPool(dataId);
            //DataStateMVC.RemoveFromPool(dataId);
            //return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
            return Json(data, JsonRequestBehavior.AllowGet);
            //return Json("aasasd", JsonRequestBehavior.AllowGet);

        }
        

        //[System.Web.Mvc.HttpGet]
        //public HttpResponseBase ExportStakeHolderData(int dataId)
        //{
        //    ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
        //    DataStateMVC.RemoveFromPool(dataId);
        //    return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
        //}
        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportStakeHolderData(int dataId)
        {
            ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
            string fileName = DataStateMVC.GetStoredObjectFromPool(dataId).FileName;
            string startDate = DataStateMVC.GetStoredObjectFromPool(dataId).StartDate;
            string endDate = DataStateMVC.GetStoredObjectFromPool(dataId).EndDate;
            string fileNameFull = string.Format("{0} {1} {2} {3}", fileName, startDate, endDate, "ComplaintsListingData.xlsx");
            DataStateMVC.RemoveFromPool(dataId);
            return FileHandler.Generate(Response, data, fileNameFull);
        }
        [System.Web.Mvc.HttpGet]
        public string BarChartMainSummaryData(string startDate, string endDate, string campId, int hierarchyId,
            int userHierarchyId, string commaSepVal, string statusIds, int reportType, string graphTag)
        {

            VmStatusWiseCount singletemp = new VmStatusWiseCount();
            List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            VmStatusWiseComplaintsData statusWiseData = BlSchoolReports.GetTertiaryCategoryWiseData(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType, graphTag);

            //return statusWiseData;

            var data = new { Total = statusWiseData };

            return JsonConvert.SerializeObject(data);
            //return JsonConvert.SerializeObject(statusWiseData);

        }
        [System.Web.Mvc.HttpPost]
        public JsonResult GetComplaintsSummary([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId,
           [FromBody] string commaSepVal, [FromBody] int hierarchyId, [FromBody] string statusIds)
        {
            List<Tuple<int, string, int>> data = BlSchoolReports.GetComplaintsSummaryReport(startDate, endDate,
               campId,hierarchyId, commaSepVal, statusIds);

            return Json(data,JsonRequestBehavior.AllowGet);
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult GetOverDueComplaintSummary([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.OverDueComplaint> listOverDueComplaints = BlSchoolReports.GetOverdueComplaintsReport(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listOverDueComplaints
            }, JsonRequestBehavior.AllowGet);
         
            return null;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult RegionAndStatusWiseCountSummary([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.RegionAndStatusWiseCount> listRegionWiseCountList = BlSchoolReports.RegionAndStatusWiseCountReport(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ComplaintCategoriesWiseRegionWiseCount([FromBody] string startDate,[FromBody] string endDate,[FromBody] string campId,[FromBody] int hierarchyId,
            [FromBody] int userHierarchyId, [FromBody] string commaSepVal,[FromBody] string statusIds, [FromBody] string divTag,[FromBody] string categories)
        {
            List<MainSummaryReport.RegionAndStatusWiseCount> listRegionWiseCountList = BlSchoolReports.RegionAndStatusWiseCountForProvinceDistrictReport(startDate, endDate, campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, -1, categories);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult RegionAndStatusWiseCountSummaryForProvinceDistrictReport([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {
            
            List<MainSummaryReport.RegionAndStatusWiseCount> listRegionWiseCountList = BlSchoolReports.RegionAndStatusWiseCountForProvinceDistrictReport(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult CategorywiseAndStatuswiseCount([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds,[FromBody] string categoryIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.CategoryWiseAndStatusWiseCount> listRegionWiseCountList = BlSchoolReports.CategorywiseStatuswiseCount(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds,categoryIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult TopOverDueComplaintsByOfficer([FromBody] string startDate, [FromBody] string endDate, [FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.TopOverDueComplaintsByOfficer> listRegionWiseCountList = BlSchoolReports.GetTopOverdueComplaintsByOfficerReport(startDate, endDate,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }

        //[System.Web.Mvc.HttpGet]
        //public HttpResponseBase ExportStakeHolderData2()
        //{
        //    return null;
        //}

        //[System.Web.Mvc.HttpGet]
        //public HttpResponseBase BarChartMainSummaryData()
        //{
        //    return null;
        //}

        //[System.Web.Mvc.HttpGet]
        //public static HttpResponseBase BarChartMainSummaryData2()
        //{

        //   // VmStatusWiseCount singletemp = new VmStatusWiseCount();
        //  //  List<VmStatusWiseCount> temp = new List<VmStatusWiseCount>();

        //   // CMSCookie cookie = new AuthenticationHandler().CmsCookie;
        //   // VmStatusWiseComplaintsData statusWiseData = BlSchoolReports.GetTertiaryCategoryWiseData(startDate, endDate, campId, hierarchyId, userHierarchyId, commaSepVal, reportType, graphTag);

        //    return null;
        //}
    }
}