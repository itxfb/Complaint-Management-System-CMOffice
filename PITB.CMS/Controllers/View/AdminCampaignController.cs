//using PITB.CMS.Handler.Authentication;
//using PITB.CMS.Handler.Authorization;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common.Handler.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class AdminCampaignController : Controller
    {
        // GET: AdminCampaign
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCampaign()
        {
            return View("~/Views/Admincampaign/ViewCampaigns.cshtml", AuthenticationHandler.GetCookie().MasterPage);
        }
    }
}