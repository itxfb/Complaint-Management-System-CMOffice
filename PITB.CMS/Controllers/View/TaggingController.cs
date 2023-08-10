using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class TaggingController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: Tagging
        public ActionResult AgentTaggingLIst()
        {
            return View("~/Views/Agent/TagListing.cshtml");
        }
        public ActionResult GetTagEditView(int recordId)
        {
            VmTagEdit vmTagEdit = BlTag.GetTagEditVm(recordId);
            return PartialView("~/Views/Agent/_TagEdit.cshtml", vmTagEdit);
        }

        public ActionResult OnEditTag(VmTagEdit tagEdit)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                message = BlTag.OnEditTag(tagEdit);
                TempData["Message"] = StatusMessage(Config.CommandStatus.Success, message, Config.Message.Failure, Config.Message.Error);
            }
        
            //return Json(message, JsonRequestBehavior.AllowGet);
            return RedirectToAction("AgentTaggingLIst");
        }
    }
}