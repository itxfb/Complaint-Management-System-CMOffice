using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authorization;
//using PITB.CMS.Handler.Authorization;
//using PITB.CMS.Handler.Business;
//using PITB.CMS.Models.View;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class StakeholderController : Controller
    {
        // GET: Stakeholder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetResolverDetail(int userId)
        {
            //VmComplaintDetail vmComplaintDetail = BlComplaints.GetComplaintDetail(complaintId);
            VmResolverDetail vmResolverDetail = BlUsers.GetResolverDetail(userId);
            return PartialView("_ResolverDetail", vmResolverDetail);
        }
    }
}