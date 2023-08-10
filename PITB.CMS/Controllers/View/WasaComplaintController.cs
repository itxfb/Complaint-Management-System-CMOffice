using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Web;
using System.IO;
using PITB.CMS_Common.Models.View.Wasa;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View
{
    [AuthorizePermission]
    public class WasaComplaintController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: WasaComplaint
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddComplaintSubmit(VmAddComplaintWasa complaintModel, FormCollection collection)
        {
            VmAddComplaintWasa.DiscartUnnecessaryValuesFromModelDictionary(ModelState, complaintModel, false);

            if (ModelState.IsValid)
            {
                //string formVal = collection["ComplaintVm.ListDynamicDropDown[0].SelectedItemId"].ToString();
                var status = BlWasa.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0),
                     (complaintModel.ComplaintVm.Id != 0));
                TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);

                //if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("publicuser"))
                //    return RedirectToAction("Index", "PublicUserComplaints");
                //else
                //    return RedirectToAction("Search", "Complaint");

                return Redirect(RedirectionURL(Request));

            }
            ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            VmAddComplaint vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), Convert.ToInt32(complaintModel.PersonalInfoVm.Person_id),
                 Convert.ToInt32(complaintModel.ComplaintVm.Compaign_Id));

            ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;

            return View("~/Views/Add.cshtml", BlView.Instance.GetMasterPageFromCookie(), vmodel);
        }

        public ActionResult StakeholderComplaintsListingServerSide(int ComplaintType = (int)Config.ComplaintType.Complaint)
        {
            /*if (PermissionHandler.IsPermissionAllowedInUser(Config.Permissions.ShowOnlyComplaintsAllInResolver))
            {
                return StakeholderComplaintsListingLowerHierarchyServerSide();
            }
            else
            {*/
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            ViewBag.LogoUrl =
                DbCampaign.GetLogoUrlByCampaignId(
                    Utility.GetIntByCommaSepStr(AuthenticationHandler.GetCookie().Campaigns));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList =
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, (Config.ComplaintType)ComplaintType);

            ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            //}
            int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<SelectListItem> listStatuses = null;


            listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListing);
            listStatuses = Utility.GetSortedSelectedListItem(listStatuses, Config.ListWasaStatusSorted);
            ViewBag.StatusList = Utility.GetAlteredStatus(listStatuses, Config.PendingFreshStatus, Config.WasaPendingFreshStatus);
            return View("~/Views/Stakeholder/Wasa/WasaComplaintListingsServerSide.cshtml");

        }

        public ActionResult StakeholderSuggestionListingServerSide(int ComplaintType = (int)Config.ComplaintType.Suggestion)
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            ViewBag.LogoUrl = DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, (Config.ComplaintType)ComplaintType);
            //ViewBag.StatusList = BlCommon.GetStatusListByCampaignId(Convert.ToInt32(listCampaings[0].Value));

            int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            CMSCookie cookie = AuthenticationHandler.GetCookie();
            return View("~/Views/Stakeholder/Wasa/WasaSuggestionListingsServerSide.cshtml");

        }

        public ActionResult StakeholderInquiryListingServerSide(int ComplaintType = (int)Config.ComplaintType.Inquiry)
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            ViewBag.LogoUrl = DbCampaign.GetLogoUrlByCampaignId(Convert.ToInt32(AuthenticationHandler.GetCookie().Campaigns));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, (Config.ComplaintType)ComplaintType);
            //ViewBag.StatusList = BlCommon.GetStatusListByCampaignId(Convert.ToInt32(listCampaings[0].Value));

            int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            CMSCookie cookie = AuthenticationHandler.GetCookie();
            return View("~/Views/Stakeholder/Wasa/WasaInquiryListingsServerSide.cshtml");
        }

        public ActionResult StakeholderComplaintsListingLowerHierarchyServerSide(int ComplaintType = (int)Config.ComplaintType.Complaint)
        {
            List<SelectListItem> listStatuses = null;
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            ViewBag.LogoUrl = DbCampaign.GetLogoUrlByCampaignId(Utility.GetIntByCommaSepStr(AuthenticationHandler.GetCookie().Campaigns));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns);
            listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListingAll);
            listStatuses = Utility.GetSortedSelectedListItem(listStatuses, Config.ListWasaStatusSorted);
            //listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            ViewBag.StatusList = Utility.GetAlteredStatus(listStatuses, Config.PendingFreshStatus, Config.WasaPendingFreshStatus);
            ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;
            CMSCookie cookie = AuthenticationHandler.GetCookie();

            int[] campaignIdsSmartLahore = new[] { 35, 36, 39 };
            int[] campaignIdsSchoolEducation = new[] { (int)Config.Campaign.SchoolEducationEnhanced };
            return View("~/Views/Stakeholder/Wasa/WasaComplaintListingsLowerHierarchyServerSide.cshtml");
        }

        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, cookie.UserId, detailType);
            return PartialView("~/Views/Stakeholder/Wasa/WasaDetail.cshtml", vmComplaintDetail);
        }

        public ActionResult TransferComplaint(int complaintId)
        {
            VmTransferComplaint vmComplaintDetail = TransferHandler.GetTransferVmModel(complaintId);
            return PartialView("~/Views/Stakeholder/_TransferComplaint.cshtml", vmComplaintDetail);
        }

        [HttpPost]
        public ActionResult OnStatusChange(VmStatusChange vmStatusChange/*, IEnumerable<HttpPostedFileBase> files*/)
        {
            if (ModelState.IsValid)
            {
                FileUploadHandler.FileValidationStatus validationStatus = StatusHandler.ChangeStatus(vmStatusChange, System.Web.HttpContext.Current.Request.Files);
                if (validationStatus.ValidationStatus == Config.AttachmentErrorType.NoError)
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Success, validationStatus.ValidationMessage,
                        Config.Message.UpdateSuccess, Config.Message.Error);
                }
                else
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Failure, "",
                        validationStatus.ValidationMessage, Config.Message.Error);
                }
                //TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint " + vmStakeholderComplaint.Compaign_Id+"-"+vmStakeholderComplaint.ComplaintId + " status changed successfully!! ", Config.Message.UpdateSuccess, Config.Message.Error);


                //var status = vmStakeholderComplaint.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0),
                //     (complaintModel.ComplaintVm.Id != 0));
                //TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);
                //return RedirectToAction("Search");

            }
            /*
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            return RedirectToAction("controllerName", "actionName");
            */

            //return StakeholderComplaintsListingServerSide();
            return RedirectToAction("~/Views/Stakeholder/Wasa/WasaStakeholderComplaintsListingServerSide");


            //ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            //VmAddComplaint vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), (int)complaintModel.PersonalInfoVm.Person_id,
            //     (int)complaintModel.ComplaintVm.Compaign_Id);

            //ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;
            //return View("Add", vmodel);
            //return null;
            //return RedirectToAction("StakeholderComplaints");
            //return Json(true);
        }
    }
}