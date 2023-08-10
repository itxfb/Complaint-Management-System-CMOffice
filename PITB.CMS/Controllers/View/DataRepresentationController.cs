using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Data_Representation;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View.Data_Representation;
using PITB.CMS_Common.Handler.Authorization;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class DataRepresentationController : Controller
    {
        public ActionResult DistrictResponseTimeReport()
        {
            VmResponseTime parameterList = BlDataRepresentation.GetParametersForResponseTimeReport();
            return View("~/Views/Data Representation/DistrictResponseTimeReport.cshtml", BlView.Instance.GetMasterPageFromCookie()/*BlView.Instance.SetMasterPageInCookie("~/Views/Shared/Executive/_ExecutiveDashboardLayout.cshtml")*/, parameterList);
        }
        [HttpGet]
        public ActionResult GetHierarchyList(string campaignText)
        {
            if (!string.IsNullOrWhiteSpace(campaignText))
            {
                IEnumerable<SelectListItem> hierarchyList =  BlDataRepresentation.GetHierarchyList(campaignText);
                return Json(hierarchyList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpGet]
        public ActionResult GetEscalationLevelList(string campaignText)
        {
            if (!string.IsNullOrWhiteSpace(campaignText))
            {
                IEnumerable<SelectListItem> escalationLevelList = BlDataRepresentation.GetEscalationLevelList(campaignText);
                return Json(escalationLevelList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [HttpGet]
        public ActionResult GetStatusList(string campaignText)
        {
            if (!string.IsNullOrWhiteSpace(campaignText))
            {
                IEnumerable<SelectListItem> statusList = BlDataRepresentation.GetStatusList(campaignText);
                return Json(statusList, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public ActionResult AreawiseCategoryReport()
        {
            VmAreawiseCategoryModel parameterList = BlDataRepresentation.GetParametersForAreawiseCategoryReport();
            return View("~/Views/Stakeholder/AreawiseCategoryReport.cshtml",BlView.Instance.GetMasterPageFromCookie(),parameterList);
        }
        public ActionResult LoadAreawiseCategoryReportPartialView()
        {
            return PartialView("~/Views/Stakeholder/_AreawiseCategoryReport.cshtml");
        }
        public ActionResult GetGeoJsonData()
        {
            string filePath = @"~\GeoData\Punjab_Districts.geojson";
            string json = System.IO.File.ReadAllText(Server.MapPath(filePath));
            return Content(json, "application/json");
        }
        [HttpGet]
        public ActionResult GetMapData()
        {
            List<MapData> data = BlMaps.GetComplaintsStatusData((int)Config.Campaign.SchoolEducationEnhanced, 6);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(data);
            return Content(json, "application/json");
        }
        [HttpGet]
        public ActionResult GetMapDataWithParams(string startDate,string endDate,string campaignIds,string statusId)
        {
            List<MapData> data = BlMaps.GetComplaintsStatusData(campaignIds, statusId, startDate, endDate);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(data);
            return Content(json, "application/json");
        }
    }
}