using System;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using PITB.CMS_Common;
using PITB.CRM.Public_Web.BusinessLayer;
using PITB.CRM.Public_Web.Contacts;
using PITB.CRM.Public_Web.Handler.Authentication;
using PITB.CRM.Public_Web.Models.ViewModels;
namespace PITB.CRM.Public_Web.Controllers
{
    public class PublicController : Controller
    {
        // GET: Public
        public ActionResult Index()
        {
         
            return View();

            
        }

        public ActionResult Detail(string id)
        {
            IAuthHandler authHandler = new FacebookAuthHandler();

            VmNewsFeed model =BlComplaint.GetDetailofComplaint(id);
            if (authHandler.IsAuthorized())
            {
                ViewBag.UserData = authHandler.GetLoggedInUserData();
            }

            return View(model);
        }

        public ActionResult Resolved(string id)
        {
            IAuthHandler authHandler = new FacebookAuthHandler();
            if (authHandler.IsAuthorized())
            {
                ViewBag.UserData = authHandler.GetLoggedInUserData();
            }
            VmNewsFeed model = BlComplaint.GetDetailofComplaint(id);
            return View("Resolved",model);
        }

        [HttpPost]
        public ActionResult Loadmore(VmFilters filters)
        {

            var a = BlComplaint.LoadComplaintsBetween(filters);
            return PartialView("_NewsFeed", a);
        }

        [HttpPost]
        public JsonResult SubmitMyVote(VmSubmitVote model)
        {
            SubmiteVoteResponse response = null;
            IAuthHandler authHandler = new FacebookAuthHandler();
            if (authHandler.IsAuthorized())
            {
                var user = authHandler.GetLoggedInUserData();
                response = BlComplaint.SubmitVoteForComplaint(model, user);

            }
            else
            {
                response = new SubmiteVoteResponse() { Status = false, Message = "Please login first" };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetFilters()
        {
            VmFilters filters = new VmFilters(Config.Campaign.FixItLwmc);
            return PartialView("_PublicViewFilterAll", filters);
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult Encypt(string id)
        {
            var a = Utilities.Utils.Encrypt(Convert.ToInt32(id));
            return Content(a);
        }
        public ActionResult Decrypt(string id)
        {
            var a = Utilities.Utils.Decrypt(id);
            return Content(a.ToString());
        }
    }
}