using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View.Executive;
using PITB.CMS_Common.Handler.Authorization;
using System;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Helper.Extensions;
using System.Globalization;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models;
using PITB.CMS_Common;
using System.Dynamic;

namespace PITB.CMS.Controllers.View.ExecutiveCampaign
{
    public class ExecutiveCampaignsController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: ExecutiveCampaign
        public ActionResult Index()
        {
            return View();
        }

        //[System.Web.Mvc.HttpPost]
        //public ActionResult OnStatsLinkClick(FormCollection fc)
        //{
        //    CustomForm.Post postedForm = new CustomForm.Post(Request);

        //    //dynamic d = new ExpandoObject();
        //    //idict
        //    ViewBag.data = new
        //    {
        //        tagId = postedForm.GetElementValue("tagId"),
        //        startDate = postedForm.GetElementValue("startDate"),
        //        endDate = postedForm.GetElementValue("endDate")
        //    }.ToExpando();
        //    ViewBag.data.logoUrl = "";
        //    ViewBag.data.pageHeading = "Munna kaka";
        //    //ViewBag.data = BlExecutive.PopulateListingTable(new
        //    //{
        //    //    tagId = postedForm.GetElementValue("itemId"),
        //    //    startDate = postedForm.GetElementValue("startDate"),
        //    //    endDate = postedForm.GetElementValue("endDate")
        //    //}.ToExpando());
        //    //return PartialView("~/Views/Stakeholder/Police/_PoliceComplaintListing.cshtml");
        //    //ViewEngineResult result = ViewEngines.Engines.FindView(this.ControllerContext, "~/Views/Stakeholder/Police/_PoliceComplaintListing.cshtml", null);
        //    //ViewEngineResult result2 = ViewEngines.Engines.FindView(this.ControllerContext, "~/Views/Shared/Partial/Table/_TableLayout.cshtml", null);
        //    //return View("~/Views/Stakeholder/Police/_PoliceComplaintListing.cshtml");
        //    dynamic d = new {
        //        tagId = postedForm.GetElementValue("tagId"),
        //        startDate = postedForm.GetElementValue("startDate"),
        //        endDate = postedForm.GetElementValue("endDate")
        //    }.ToExpando();
        //    dynamic data = BlExecutive.GetListingView(d);
        //    ViewBag.data = data.viewBag;
        //    return View(data.viewName);

        //    //return View("~/Views/Shared/Partial/Table/_TableLayout.cshtml");

        //    //HttpFileCollectionBase files = Request.Files;
        //    //NameValueCollection formCollection = Request.Form;


        //    //Config.CommandMessage commandMessage = null;
        //    //ControllerModel.Response resp = new ControllerModel.Response();
        //    //try
        //    //{
        //    //    CustomForm.Post postForm = new CustomForm.Post(Request);
        //    //    CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
        //    //    PoliceModel.AddFeedback addFeedbackModel = new PoliceModel.AddFeedback(cmsCookie, postForm, DateTime.Now, Config.SourceType.Web/*, Config.OtherSystemId.Police*/, postForm.DictQueryParams["complaintId"]);
        //    //    commandMessage = BlPolice.SubmitComplaintFeedback(addFeedbackModel/*new CustomForm.Post(Request)*/);
        //    //    resp.RedirectUrl = Url.Action("AgentComplaintListingAllServerSide", "Complaint");
        //    //    resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMessage);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    resp.AddMessagePartialView(this, resp.ListPartialView, Config.CommandMessage.GetFailureMessage());
        //    //}
        //    //return Json(resp);
        //}


        public ActionResult Dashboard()
        {
            ViewBag.StartDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
            ViewBag.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            //ViewResult myView = View("~/Views/ExecutiveView/Dashboard.cshtml");
            //myView.MasterName = AuthenticationHandler.GetCookie().MasterPage;
            //return myView;


            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            if (cmsCookie == null)
                return RedirectToAction("Login", "Account");
            else if (cmsCookie.navigationalData != null)
            {
                if (cmsCookie.navigationalData.StartDate != null)
                {
                    ViewBag.StartDate = cmsCookie.navigationalData.StartDate;
                    ViewBag.EndDate = cmsCookie.navigationalData.EndDate;
                }
            }
            if (cmsCookie.Role == CMS_Common.Config.Roles.ExecutiveCampaigns)
            {
                var page = AuthenticationHandler.GetCookie().MasterPage;
                return View("~/Views/ExecutiveCampaigns/Dashboard.cshtml", page);
            }
            else
            {


                //DbUsers dbUser = DbUsers.GetActiveUser(cmsCookie.PreviousLoginId);/*31735*/;
                //CMSCookie navigateCookie = CMSCookie.DbUserToCookie(dbUser);
                //cmsCookie = CMSCookie.DbUserToCookie(dbUser);
                //cmsCookie.UrlConfig = null;
                BlView.Instance.UpdateUserLogin(cmsCookie.UserId, cmsCookie, CMSCookie.DbUserToCookie(DbUsers.GetActiveUser((int)cmsCookie.PreviousLoginId)));
                return RedirectToAction("Login", "Account");
            }
            //return View("~/Views/ExecutiveCampaigns/Dashboard.cshtml", AuthenticationHandler.GetCookie().MasterPage);
        }



        [HttpGet]
        [OutputCache(Duration = 5, VaryByParam = "*")]
        public JsonResult GetCampaignDataById(int campaignId, string startDate, string endDate)
        {
            VmCampaignStatusWise response = BlExecutiveCampaigns.GetCompliantsDataBySingleCampaignId(campaignId, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OutputCache(Duration = 5, VaryByParam = "*")]
        public ActionResult ExecutiveCampaignView(string startDate, string endDate)
        {
            DateTime d1 = new DateTime();
            DateTime d2 = new DateTime();
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy h:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out d1)
                || DateTime.TryParseExact(startDate, "MM/dd/yyyy h:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out d1))
            {
                startDate = d1.ToString("yyyy-MM-dd");
            }
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy h:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out d2)
                || DateTime.TryParseExact(startDate, "MM/dd/yyyy h:mm:ss tt", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out d2))
            {
                endDate = d2.ToString("yyyy-MM-dd");
            }

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            cmsCookie.navigationalData = new ExpandoObject();
            cmsCookie.navigationalData.StartDate = startDate;
            cmsCookie.navigationalData.EndDate = endDate;
            new AuthenticationHandler().SaveCookie(cmsCookie);
            string templatePath = Server.MapPath(@"~/GeoData/PLRA.xml");
            //string errorFilePath = Server.MapPath(@"~/Exception.txt");
            //string RepsponsePath = Server.MapPath(@"~/Response.txt");
            //BlExecutiveCampaigns.filepath = templatePath;
            //BlExecutiveCampaigns.ErrorFilePath = errorFilePath;
            //BlExecutiveCampaigns.ReponsePath = RepsponsePath;
            //DateTime startDt = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //DateTime endDt = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //string asd = CMS_Common.Utility.GetSQLDateTimeFormat(startDt);
            VmDashboard dashboard = BlExecutiveCampaigns.GetDashboardModel(startDate, endDate);
            return PartialView("~/Views/ExecutiveCampaigns/_ExecutiveCampaignView.cshtml", dashboard);
        }

        public ActionResult GetPLRAComplaintDetails()
        {
            ViewBag.From = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
            ViewBag.To = DateTime.Now.ToString("yyyy-MM-dd");
            return View("~/Views/Stakeholder/PLRA/_PLRAComplaintDetails.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
    }
}