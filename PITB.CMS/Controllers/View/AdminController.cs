//using PITB.CMS.Handler.Business;
using PITB.CMS_Common.Handler.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View("~/Views/Admin/Index", BlView.Instance.GetMasterPageFromCookie() /*BlView.Instance.SetMasterPageInCookie("~/Views/Shared/_AdminLayout.cshtml")*/);
        }
    }
}