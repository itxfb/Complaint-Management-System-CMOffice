using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using OfficeOpenXml;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View.Wasa.Stakeholder;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.ExportFileHandler;

namespace PITB.CMS.Controllers.API.Wasa
{
    public class APIWasaController : Controller
    {
        [System.Web.Mvc.HttpPost]
        public JsonResult GetStakeholderComplaintsServerSide([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType)
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
            DataTable dt = BlWasa.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "Listing");

            //Utility.GetAlterStatusDataTable(dt, "_complaint_Computed_Status", Config.PendingFreshStatus,
             //   Config.SchoolEducationPendingFreshStatus);


            List<VmWasaStakeholderComplaintListing> stComplaintListing = dt.ToList<VmWasaStakeholderComplaintListing>();

            int totalRows = stComplaintListing.Count == 0 ? 0 : stComplaintListing.First().Total_Rows;

            return Json(new
            {
                data = stComplaintListing,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetDashboardLabelsStakeholderData([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] string dashboardType)
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

            DataTable dt = BlWasa.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                spType);

            Utility.GetAlterStatusDataTable(dt, "Name", Config.PendingFreshStatus,
                Config.WasaPendingFreshStatus);

            List<VmStakeholderComplaintDashboard> listDashboard = dt.ToList<VmStakeholderComplaintDashboard>();


            //ExcelPackage data = (ExcelPackage) DataStateMVC.GetDataFromPool(dataId);
            //DataStateMVC.RemoveFromPool(dataId);
            //return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
            return Json(listDashboard, JsonRequestBehavior.AllowGet);
            //return Json("aasasd", JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult /*HttpResponseBase*/ ExportStakeHolderData([FromBody] string aoData, [FromBody]string from, [FromBody]string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType)
        {
            //Response.

            /*
            if (status == null) status = "";
            DataTable data = BlComplaints.GetStakeholderComplaintsForExport(
                startDate,
                endDate,
                campaign,
                category,
                status,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType);
            */

            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable data = BlWasa.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "ExcelReport");

            int rowCount = data.Rows.Count;
            //return FileHandler.GetFile(Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData");
            //return FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");

            //HttpResponseBase responseBase = FileHandler.Generate(Response, Config.FileType.Excel, data, "Complaint Listing Data", "ComplaintsListingData.xlsx");
            //return Json(responseBase, JsonRequestBehavior.AllowGet);
            ExcelPackage excelPack = FileHandler.ExportToExcel(data, "Complaint Listing Data");
            int dataId = DataStateMVC.AddInPool(excelPack);

            //string asd = "sfdsdf";
            return Json(dataId, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportStakeHolderData(int dataId)
        {
            ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
            DataStateMVC.RemoveFromPool(dataId);
            return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
        }
    }
}