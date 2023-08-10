using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View.Executive;
using PITB.CMS_Common.Handler.Authorization;
using System;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models;
using System.Dynamic;
using System.Collections.Generic;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Helper.Extensions;
using Newtonsoft.Json;

namespace PITB.CMS.Controllers.View
{
    //[AuthorizePermission]
    public class ExecutiveController : PITB.CMS_Common.Controllers.View.Controller
    {
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
            if (cmsCookie.Role == CMS_Common.Config.Roles.Executive)
            {
                var page = AuthenticationHandler.GetCookie().MasterPage;
                return View("~/Views/ExecutiveView/Dashboard.cshtml", page);
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
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult OnStatsLinkClick(FormCollection fc)
        {
            CustomForm.Post postedForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
            dynamic d = new
            {
                tagId = postedForm.GetElementValue("tagId"),
                startDate = postedForm.GetElementValue("startDate"),
                endDate = postedForm.GetElementValue("endDate")
            }.ToExpando();
            dynamic data = BlExecutive.GetListingView(d);
            ViewBag.data = data.viewBag;
            return View(data.viewName);
        }

        [System.Web.Mvc.HttpPost]
        public string GetListingData()
        {
            CustomForm.Post postedForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
            dynamic data = BlExecutive.GetListingData(new
            {
                tagId = postedForm.GetElementValue("tagId"),
                startDate = postedForm.GetElementValue("startDate"),
                endDate = postedForm.GetElementValue("endDate"),
                aoData = postedForm.GetElementValue("aoData")
            }.ToExpando());
            return JsonConvert.SerializeObject(data);

        }


        [HttpGet]
        [AuthorizePermission]
        [OutputCache(Duration = 5, VaryByParam = "*")]
        public JsonResult GetCampaignDataById(int campaignId, string startDate, string endDate)
        {
            VmCampaignStatusWise response = BlExecutive.GetCompliantsDataBySingleCampaignId(campaignId, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AuthorizePermission]
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
            //BlExecutive.filepath = templatePath;
            //BlExecutive.ErrorFilePath = errorFilePath;
            //BlExecutive.ReponsePath = RepsponsePath;
            VmDashboard dashboard = BlExecutive.GetDashboardModel(startDate, endDate);
            return PartialView("~/Views/ExecutiveView/_ExecutiveCampaignView.cshtml", dashboard);
        }

        [AuthorizePermission]
        public ActionResult GetPLRAComplaintDetails(string startDate, string endDate)
        {
            //ViewBag.From = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
            //ViewBag.To = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View("~/Views/Stakeholder/PLRA/_PLRAComplaintDetails.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
    }
}