using OfficeOpenXml;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.ExportFileHandler;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class ExportController : Controller
    {
        // GET: Export
        public ActionResult Index()
        {
            List<ExportReportObject> listOfExportReports = FileHandler.GetReportsToBeExportedList();
            ViewData["reports"] = listOfExportReports;
            return View("~/Views/Stakeholder/SchoolEducation/ExportReport.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult UserReports()
        {
            List<ExportReportObject> listOfExportReports = FileHandler.GetReportsToBeExportedListForPriviledgedUser();
            ViewData["reports"] = listOfExportReports;
            return View("~/Views/Stakeholder/SchoolEducation/ExportReport.cshtml");
        }
        // GET: Export/Details/5
        public JsonResult GetExportReportData(string reportName, string fromDate, string toDate)
        {
            DataTable lObjReportData = FileHandler.GetExportReportDataFromDatabase(reportName, fromDate, toDate);
            CMSCookie user = AuthenticationHandler.GetCookie();
            if (user.Role == Config.Roles.PriviledgedUser)
            {
                if (reportName == "Assignee Data" && lObjReportData.Columns.Contains("ResolverPhoneNo"))
                {
                    lObjReportData.Columns.Remove("ResolverPhoneNo");
                }
            }
            if (lObjReportData != null && lObjReportData.Rows.Count > 0)
            {
                ExcelPackage excelPack = FileHandler.ExportToExcel(lObjReportData, reportName + " (" + Convert.ToDateTime(fromDate).ToString("MM-dd-yyyy") + " - " + Convert.ToDateTime(toDate).ToString("MM-dd-yyyy") + ")");
                int dataId = DataStateMVC.AddInPool(excelPack);
                return Json(dataId, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        //[AuthorizePermission(2)]
        //[AuthorizePermission(2)]
        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportReport(int dataId,string reportName)
        {
                ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
                DataStateMVC.RemoveFromPool(dataId);
                return FileHandler.Generate(Response, data, String.Format("{0}.xlsx", reportName + " on " + DateTime.Now.Date.ToString("MM-dd-yyyy")));                        
        }
    }
}
