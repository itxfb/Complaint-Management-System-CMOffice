using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View.ClientMesages;
using PITB.CMS_Common.Models.View.Message;
using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMessageView()
        {
            return View("~/Views/Stakeholder/MessageListing.cshtml");
        }

        public ActionResult GetMessageReplyPopup(int recordId)
        {
            VmMessageReply vmReply = BlMessages.GetMessageReplyVm(recordId);
            return PartialView("~/Views/Stakeholder/_ReplyPopup.cshtml", vmReply);
        }

        public ActionResult SendMessageToHeadTeachers()
        {
            BlMessages.SendTextMessageToHeadTeachers();
            return View();
        }
        public ActionResult GetMessageThreadView(string phoneNo)
        {
            ViewBag.CallerNo = phoneNo;
            //List<VmStakeholderMessageThreadListing> listMessageThread = BlMessages.GetThreadListingOfPhoneNo(phoneNo);
            return PartialView("~/Views/Stakeholder/_MessageThreadPopup.cshtml");
        }

        public string GetMessageThreadList(string phoneNo)
        {
            List<VmStakeholderMessageThreadListing> listMessageThread = BlMessages.GetThreadListingOfPhoneNo(phoneNo);
            var aoData = new { aaData = listMessageThread };
            var json = JsonConvert.SerializeObject(aoData);
            //json = json.Replace("[", "{ \"data\": [");
            //json = json.Replace("]", "] }");
            return json;
        }


        public ActionResult GetMassMessageReplyPopup()
        {
            VmMassMessageReply vmReply = new VmMassMessageReply();
            vmReply.ReplyMessageStr = @"Dear Sir/Madam,
                                      Thank you for approaching LAHORE HIGH COURT  SAHULAT CENTRE. Your application has been put in process. We shall get back to you soon.
                                      Regards,
                                      Tahir Sabir
                                      Member Inspection Team";
            return PartialView("~/Views/Stakeholder/_MassReplyPopup.cshtml", vmReply);
        }

        public string GetMassMessagePopupList()
        {
            List<VmStakeholderMessageThreadListing> listMessageThread = BlMessages.GetMassMessagePopupList();
            var aoData=new {aaData=listMessageThread};
            var json = JsonConvert.SerializeObject(aoData);
            //json = json.Replace("[", "{ \"data\": [");
            //json = json.Replace("]", "] }");
            return json;
        }

        [HttpPost]
        public ActionResult OnReplySubmit(VmMessageReply vmMessageReply)
        {
            ClientMessage message = null;
            if (ModelState.IsValid)
            {
                message = BlMessages.OnReplySubmit(vmMessageReply);

                return Json(message, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public ActionResult OnMassReplySubmit(VmMassMessageReply vmMassReply)
        {
            ClientMessage message = null;
            if (ModelState.IsValid)
            {
                message = BlMessages.OnMassReplySubmit(vmMassReply);

                return Json(message, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }
}