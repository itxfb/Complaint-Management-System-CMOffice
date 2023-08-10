using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Handler.Permission;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
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
    public class SchoolEducationComplaintController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: SchoolEducationComplaint
        public ActionResult Index(VmAddComplaint vmodel)
        {

            return View("~/Views/Complaint/SchoolEducation/AddSchoolEducation.cshtml", vmodel);
        }

        
        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            VmSEStakeholderComplaintDetail vmComplaintDetail = BlSchool.GetStakeholderComplaintDetail(complaintId, detailType);
            return PartialView("~/Views/Stakeholder/SchoolEducation/SchoolEducationDetail.cshtml", vmComplaintDetail);
        }

        public ActionResult DashboardStakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType, int canShowStatusChange)
        {
            VmSEStakeholderComplaintDetail vmComplaintDetail = BlSchool.GetStakeholderComplaintDetail(complaintId, detailType);
            ViewBag.canShowStatusChange = canShowStatusChange;
            vmComplaintDetail.VmStatusChange.returnUrl = "~/Views/Stakeholder/DashboardMain.cshtml";
            return PartialView("~/Views/Stakeholder/SchoolEducation/DashboardSchoolEducationDetail.cshtml", vmComplaintDetail);
        }

        public ActionResult TransferComplaint(int complaintId)
        {
            VmTransferComplaint vmComplaintDetail = TransferHandler.GetTransferVmModel(complaintId);
            return PartialView("~/Views/Stakeholder/SchoolEducation/_SchoolEducationTransferComplaint.cshtml", vmComplaintDetail);
        }

        [Authorise(Config.Roles.Stakeholder)]
        public ActionResult StakeholderComplaintsListingServerSide(int userId, string from, string to, string statusId)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            int canShowStatusChangeInDetail = 0;//Convert.ToInt32(userId == cookie.UserId);
            canShowStatusChangeInDetail = Convert.ToInt32((userId == cookie.UserId) &&

                                              !PermissionHandler.IsPermissionAllowedInUser(
                                                  Config.Permissions.HideStatusChangeInComplaintsAssignedToMeStakeholder, cookie.UserId, cookie.Role));
            DbUsers dbUser = DbUsers.GetActiveUser(userId);

            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromDbUser(dbUser);
            ViewBag.LogoUrl =
                DbCampaign.GetLogoUrlByCampaignId(
                    Utility.GetIntByCommaSepStr(dbUser.Campaigns/*AuthenticationHandler.GetCookie().Campaigns*/));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList =
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, dbUser);

            List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUser.User_Id);
            ViewBag.ListPermissions = listPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.UserId = dbUser.User_Id;
            ViewBag.CanShowStatusChangeInDetail = canShowStatusChangeInDetail;
            //}
            //int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            List<SelectListItem> listStatuses = null;

            //if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds)) // if is school education new campaign
            {
                if (canShowStatusChangeInDetail == 1)
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId)).Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
                    }
                    else
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
                    }
                }
                else
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId))
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                    else
                    {

                        listStatuses =
                            DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses)
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                }
                //listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
                listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(),
                                Config.Permissions.StatusesForComplaintListingAll);
                listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus,
                    Config.SchoolEducationUnsatisfactoryStatus);
                ViewBag.StatusList = listStatuses;
                BlView.Instance.SetStartEndDate(this);
                return View("~/Views/Stakeholder/SchoolEducation/_SchoolEducationComplaintListingsServerSide.cshtml");
            }
            /*else
            {
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList());
                ViewBag.StatusList = listStatuses;
                return View("~/Views/Stakeholder/ComplaintListingsServerSide.cshtml");
            }*/

        }


        public ActionResult StakeholderComplaintsListingPercentageExpiryViewServerSide(int userId, string from, string to, string statusId, string tabType)
        {
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            int canShowStatusChangeInDetail = Convert.ToInt32(userId == cookie.UserId);
            DbUsers dbUser = DbUsers.GetActiveUser(userId);

            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromDbUser(dbUser);
            ViewBag.LogoUrl =
                DbCampaign.GetLogoUrlByCampaignId(
                    Utility.GetIntByCommaSepStr(dbUser.Campaigns/*AuthenticationHandler.GetCookie().Campaigns*/));
            List<SelectListItem> listCampaings = ViewBag.Campaigns;
            ViewBag.ComplaintTypeList =
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns, dbUser);

            List<DbPermissionsAssignment> listPermissions = DbPermissionsAssignment.GetListOfPermissions((int)Config.PermissionsType.User, dbUser.User_Id);
            ViewBag.ListPermissions = listPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.UserId = dbUser.User_Id;
            ViewBag.CanShowStatusChangeInDetail = canShowStatusChangeInDetail;

            List<SelectListItem> listStatuses = null;

            if (tabType == Config.StakeholderComplaintListingType.AssignedToMe.ToString())
            {
                ViewBag.PageHeading = "Complaints (My)";
                ViewBag.ListingType = (int)Config.StakeholderComplaintListingType.AssignedToMe;
                listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
            }
            else if (tabType == Config.StakeholderComplaintListingType.UptilMyHierarchy.ToString())
            {
                ViewBag.PageHeading = "Complaints (Subordinates)";
                ViewBag.ListingType = (int)Config.StakeholderComplaintListingType.UptilMyHierarchy;
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListingAll);
            }
            //}
            //int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            //CMSCookie cookie = AuthenticationHandler.GetCookie();
            /*List<SelectListItem> listStatuses = null;

            //if (Utility.IsArrayElementPresentInString(cookie.Campaigns, campaignIds)) // if is school education new campaign
            {
                if (canShowStatusChangeInDetail == 1)
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId)).Select(n => new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status }).ToList();
                    }
                    else
                    {
                        listStatuses =
                            BlCommon.GetStatusListByCampaignIds(
                                listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), dbUser);
                    }
                }
                else
                {
                    if (statusId != "-1")
                    {
                        listStatuses =
                            DbStatus.GetByStatusIds(Utility.GetIntList(statusId))
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                    else
                    {

                        listStatuses =
                            DbStatus.GetByStatusIds(Config.ListSchoolEducationStatuses)
                                .Select(
                                    n =>
                                        new SelectListItem() { Value = n.Complaint_Status_Id.ToString(), Text = n.Status })
                                .ToList();
                    }
                }*/
            listStatuses = Utility.GetAlteredStatus(listStatuses, Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            ViewBag.StatusList = listStatuses;
            ViewBag.TabType = tabType;
            return View("~/Views/Stakeholder/SchoolEducation/_SchoolEducationComplaintListingNotification.cshtml");
            //}
            /*else
            {
                listStatuses = BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList());
                ViewBag.StatusList = listStatuses;
                return View("~/Views/Stakeholder/ComplaintListingsServerSide.cshtml");
            }*/

        }


        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddComplaintSubmit(VmAddComplaint complaintModel, FormCollection collection)
        {
            var cook = AuthenticationHandler.GetCookie();
            if (cook.Role != Config.Roles.Agent)
            {
                var result = DbComplaint.GetByComplainRecord((int)complaintModel.PersonalInfoVm.Person_id, (int)complaintModel.ComplaintVm.Complaint_SubCategory);

                if (result.Item1 > 0 && result.Item2 > 0)
                {
                    TempData["Message"] = StatusMessage(0, "Complaint already exist for this user against department", Config.Message.ComplainError, Config.Message.ComplainError);
                    return Redirect(RedirectionURL(Request));
                }
            }
            List<string> listValuesToDiscart = null;

            if (complaintModel.PersonalInfoVm.Person_id > 0)
            {
                listValuesToDiscart = new List<string>()
                {
                    "cnic"
                };
            }
            VmAddComplaint.DiscartUnnecessaryValuesFromModelDictionary(ModelState, complaintModel, true, listValuesToDiscart);
            if (ModelState.IsValid)
            {
                //string formVal = collection["ComplaintVm.ListDynamicDropDown[0].SelectedItemId"].ToString();
                //----orignal code
                complaintModel.PersonalInfoVm.IsCnicPresent = !complaintModel.PersonalInfoVm.IsCnicPresent;
                var status = BlSchool.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0),
                     (complaintModel.ComplaintVm.Id != 0));
                //---- end orignal code
                //var status = BlSchool.AssignComplaintsToUsers(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0), (complaintModel.ComplaintVm.Id != 0));
                TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);
                //if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("publicuser"))
                //    return RedirectToAction("Index", "PublicUserComplaints");
                //else
                //    return RedirectToAction("Search", "Complaint");

                return Redirect(RedirectionURL(Request));

            }
            ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            VmAddComplaint vmodel = BlSchool.GetVmAddComplaintMerged(new VmAddComplaint(), Convert.ToInt32(complaintModel.PersonalInfoVm.Person_id),
                 Convert.ToInt32(complaintModel.ComplaintVm.Compaign_Id));

            ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;

            //return View("AddSchoolEducation", vmodel);
            return View("~/Views/Complaint/SchoolEducation/AddSchoolEducation.cshtml", vmodel);
        }


        [HttpGet]
        //SchoolEducationComplaint/OnStatusChangeBulk
        public ActionResult OnStatusChangeBulk()
        {
            bool validationStatus = SchoolEducationStatusHandler.ChangeStatusBulk();
            return Content("done " + validationStatus);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult OnStatusChange(/*VmStatusChange vmStatusChange,*/ /*IEnumerable<HttpPostedFileBase> files*/)
        {
            VmStatusChange vmStatusChange = null;




            //if (ModelState.IsValid)
            {
                //custom = new CustomForm.Post(Request);
                //PostModel.File postedFiles = postForm.postedFiles;
                //string personVm = "VmStatusChange";

                //int campaignId = postForm.GetElementValue(string.Format("VmStatusChange.Compaign_Id")).CastObj<int>();
                CustomForm.Post post = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                vmStatusChange = VmStatusChange.GetModel(post);

                FileUploadHandler.FileValidationStatus validationStatus = SchoolEducationStatusHandler.ChangeStatus(vmStatusChange, System.Web.HttpContext.Current.Request.Files);
                if (validationStatus.ValidationStatus == Config.AttachmentErrorType.NoError)
                {
                    ControllerModel.Response resp = new ControllerModel.Response();
                    //resp.RedirectUrl = vmStatusChange.returnUrl;
                    resp.RedirectUrl = post.GetElementValue(string.Format("PreviousUrl")).CastObj<string>();
                    Config.CommandMessage commandMsg = new Config.CommandMessage(Config.CommandStatus.Success, "Status changed successfully complaintId = " + vmStatusChange.Compaign_Id + "-" + vmStatusChange.ComplaintId);
                    resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMsg);

                    return Json(resp);
                    //TempData["Message"] = StatusMessage(Config.CommandStatus.Success, validationStatus.ValidationMessage,
                    //    Config.Message.UpdateSuccess, Config.Message.Error);
                }
                else
                {
                    ControllerModel.Response resp = new ControllerModel.Response();
                    //resp.RedirectUrl = Url.Action("Complaint", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                    Config.CommandMessage commandMsg = new Config.CommandMessage(Config.CommandStatus.Failure, "Add attachment");
                    resp.AddMessagePartialView(this, resp.ListPartialView, commandMsg);

                    return Json(resp);
                    //TempData["Message"] = StatusMessage(Config.CommandStatus.Failure, "",
                    //    validationStatus.ValidationMessage, Config.Message.Error);
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
            //if (string.IsNullOrEmpty(vmStatusChange.returnUrl))
            //{
            //    ControllerModel.Response resp = new ControllerModel.Response();
            //    //resp.RedirectUrl = Url.Action("Complaint", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
            //    Config.CommandMessage commandMsg = new Config.CommandMessage(Config.CommandStatus.Failure, "Add attachment");
            //    resp.AddMessagePartialView(this, resp.ListPartialView, commandMsg);

            //    return Json(resp);
            //    //return RedirectToAction("SchoolEducationStakeholderComplaintsListingServerSide");
            //}
            //else
            //{
            //    ControllerModel.Response resp = new ControllerModel.Response();
            //    resp.RedirectUrl = vmStatusChange.returnUrl;
            //    Config.CommandMessage commandMsg = new Config.CommandMessage(Config.CommandStatus.Success, "Status changed Successfully complaintId = " + vmStatusChange.Compaign_Id + "-" + vmStatusChange.ComplaintId);
            //    resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMsg);

            //    return Json(resp);
            //    //return View(vmStatusChange.returnUrl);
            //}


            //ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            //VmAddComplaint vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), (int)complaintModel.PersonalInfoVm.Person_id,
            //     (int)complaintModel.ComplaintVm.Compaign_Id);

            //ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;
            //return View("Add", vmodel);
            //return null;
            //return RedirectToAction("StakeholderComplaints");
            //return Json(true);
        }

        [HttpPost]
        public ActionResult OnCallStatusSubmit(VmCallSubmit vmCallSubmit)
        {
            if (ModelState.IsValid)
            {

                Config.CommandMessage status = BlComplaints.CallStatusSubmit(vmCallSubmit);
                if (status.Status == Config.CommandStatus.Success)
                {
                    TempData["Message"] = StatusMessage(status.Status, status.Value,
                        Config.Message.UpdateSuccess, Config.Message.UpdateSuccess);
                }
                else
                {
                    TempData["Message"] = StatusMessage(status.Status, status.Value,
                        Config.Message.Failure, Config.Message.Failure);
                }

            }
            return RedirectToAction("SchoolEducationStakeholderComplaintsListingServerSide");
        }

        public ActionResult SchoolEducationStakeholderComplaintsListingServerSide()
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
                BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns);
            ViewBag.StatusList =
                BlCommon.GetStatusListByCampaignIds(listCampaings.Select(n => Convert.ToInt32(n.Value)).ToList(), Config.Permissions.StatusesForComplaintListing);
            ViewBag.ListPermissions = AuthenticationHandler.GetCookie().ListPermissions;
            ViewBag.ListTransfered = Utility.GetBinarySelectedListItem();
            //}
            int[] campaignIds = new[] { (int)Config.Campaign.SchoolEducationEnhanced };

            CMSCookie cookie = AuthenticationHandler.GetCookie();
            if (cookie.SubRoles == Config.SubRoles.SDU)
            {
                return View("~/Views/Stakeholder/SchoolEducation/SDU/SDUSchoolEducationComplaintListingsServerSide.cshtml");
            }
            else
            {
                return View("~/Views/Stakeholder/SchoolEducation/SchoolEducationComplaintListingsServerSide.cshtml");
            }
        }
    }
}