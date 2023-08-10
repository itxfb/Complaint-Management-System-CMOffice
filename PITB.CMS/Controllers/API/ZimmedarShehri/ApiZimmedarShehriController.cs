using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.Custom.Reports;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.API.ZimmedarShehri
{
    public class ApiZimmedarShehriController : Controller
    {
        // GET: APIZimmedarShehri
        [System.Web.Mvc.HttpPost]
        public JsonResult RegionAndStatusWiseCountSummaryDistrict([FromBody] string startDate, [FromBody] string endDate, [FromBody] int provinceid,[FromBody] string campId, [FromBody] int hierarchyId,
           [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionWiseCountList = BlZimmedarShehri.RegionAndStatusWiseCountReportDistrict(startDate, endDate, provinceid,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult RegionAndStatusWiseCountSummaryDivision([FromBody] string startDate, [FromBody] string endDate, [FromBody] int provinceid, [FromBody] string campId, [FromBody] int hierarchyId,
            [FromBody] int userHierarchyId, [FromBody] string commaSepVal, [FromBody] string statusIds, [FromBody] int reportType, [FromBody] string divTag)
        {

            List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionWiseCountList = BlZimmedarShehri.RegionAndStatusWiseCountReportDivision(startDate, endDate, provinceid,
                campId, hierarchyId, userHierarchyId, commaSepVal, statusIds, reportType);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult RegionStatusWiseCountSummaryTehsil([FromBody] string startDate, [FromBody] string endDate, [FromBody] int districtId, [FromBody] int campaignId,
            [FromBody] int hierarchyId, [FromBody] int userHierarchyId, [FromBody] string commaSepVal,[FromBody] string statusIds,[FromBody] int divTag)            
        {
            List<MainSummaryReport.ZimmedarShehriRegionAndStatusWiseCount> listRegionWiseCountList = BlZimmedarShehri.RegionAndStatusWiseCountReportTehsil(startDate, endDate, districtId,
         campaignId, hierarchyId, userHierarchyId, commaSepVal, statusIds);
            return Json(new
            {
                data = listRegionWiseCountList
            }, JsonRequestBehavior.AllowGet);

            return null;
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult RegionWiseFeedbackReport([FromBody] string startDate, [FromBody] string endDate, [FromBody] int hierarchyId, [FromBody] int provinceId, [FromBody] int campaignId,[FromBody] int upperHierarchyId)
        {
            List<RegionWiseFeedback> lstRegionWiseFeedback = BlZimmedarShehri.RegionWiseFeedbackReport(startDate, endDate, hierarchyId, campaignId, provinceId,upperHierarchyId);
            return Json(new
            {
                data = lstRegionWiseFeedback
            }, JsonRequestBehavior.AllowGet);

        }
    }
}