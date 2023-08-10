using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.ComplaintFileHandler;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Wasa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View.PublicUser
{
    public class PublicUserComplaintsController : PITB.CMS_Common.Controllers.View.Controller
    {

        //// GET: PublicUserComplaints
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //[AuthorizePermission]
        //[Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult Index()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("~/Views/PublicUserComplaints/Index.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        public ActionResult GetCampaignDepartments(int id = 0)
        {
            ViewBag.PersonId = id;//Sending personId back to view 
            string masterPage = BlView.Instance.GetMasterPageFromCookie();
            var listOfVmCampaigns = BlComplaints.GetListOfCampaignsByUserId();
            var campaignDepartments = BlCampaignDepartments.GetCampaignsDepartmentByCampaignId(listOfVmCampaigns.Select(s => (int)s.Campaign_Department_Id).ToList());


            if (campaignDepartments.Count == 1)
            {
                var userCampaigns = AuthenticationHandler.GetCookie().Campaigns.Split(',').ToIntList();
                var campaigns = BlCampaign.GetCampaignsByDepartmentId(campaignDepartments.FirstOrDefault().Id).Where(w => userCampaigns.Contains(w.Id));
                ViewBag.UseLayout = true;
                return View("~/Views/PublicUserComplaints/_PartialCampaignList.cshtml",masterPage, campaigns);
            }


            return View("~/Views/PublicUserComplaints/_CampaignDepartmentsList.cshtml", masterPage, campaignDepartments);
        }
        

        public ActionResult GetCampaigns(int CampaignDepartmentId)
        {
            var userCampaigns = AuthenticationHandler.GetCookie().Campaigns.Split(',').ToIntList();
            var campaigns = BlCampaign.GetCampaignsByDepartmentId(CampaignDepartmentId).Where(w => userCampaigns.Contains(w.Id));
            return PartialView("~/Views/PublicUserComplaints/_PartialCampaignList.cshtml", campaigns);
        }

        public ActionResult SuggestionListing()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("~/Views/PublicUserComplaints/SuggestionListing.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }
        public ActionResult InquiryListing()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("~/Views/PublicUserComplaints/InquiryListing.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }




        //[Authorise(Config.Roles.PublicUser)]
        //[AuthorizePermission]
        public ActionResult ComplaintDetail(int complaintId, string viewType, string viewTag)
        {
            ViewBag.viewTag = viewTag;
            return BlView.Instance.GetComplaintDetail(complaintId, viewType, this);
        }

        //[AuthorizePermission]
        //[HttpPost]
        //public ActionResult OnFollowUp(VmComplaintDetailPublicUser vmComplaintDetailAgent)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        BlComplaints.OnFollowupSubmit(vmComplaintDetailAgent.ComplaintId, vmComplaintDetailAgent.FollowupComments);
        //        TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint = " + vmComplaintDetailAgent.ComplaintId + " has been followed up successfully", Config.Message.Failure, Config.Message.Error);

        //        //TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);
        //        return RedirectToAction("Index.cshtml");
        //        //return PartialView(null);
        //    }
        //    return PartialView(null);
        //}

        ////[AuthorizePermission]
        //[HttpPost]
        //public ActionResult OnStatusChange(VmStatusChange vmStatusChange, IEnumerable<HttpPostedFileBase> files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        FileUploadHandler.FileValidationStatus validationStatus = StatusHandler.ChangeStatus(vmStatusChange, Request.Files);
        //        if (validationStatus.ValidationStatus == Config.AttachmentErrorType.NoError)
        //        {
        //            TempData["Message"] = StatusMessage(Config.CommandStatus.Success, validationStatus.ValidationMessage,
        //                Config.Message.UpdateSuccess, Config.Message.Error);
        //        }
        //        else
        //        {
        //            TempData["Message"] = StatusMessage(Config.CommandStatus.Failure, "",
        //                validationStatus.ValidationMessage, Config.Message.Error);
        //        }

        //    }
        //    return RedirectToAction("Index");

        //}

        //[AuthorizePermission]
        //public ActionResult FileViewer(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        //{
        //    VmFileModel vmFileModel = FileHandler.GetVmFileModel(complaintId, attachmentRefType, attachmentRefTypeId);
        //    return PartialView("_FileViewer", vmFileModel);
        //}

    }
}