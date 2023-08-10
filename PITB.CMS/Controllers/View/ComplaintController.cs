using Amazon.SimpleSystemsManagement;
using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Complaint;
using PITB.CMS_Common.Handler.Complaint.Status;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.ComplaintFileHandler;
using PITB.CMS_Common.Handler.DynamicFields;
using PITB.CMS_Common.Handler.FileUpload;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Extensions;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Dynamic;
using PITB.CMS_Common.Models.View.Wasa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    [AuthorizeUser]
    //[AuthorizePermission]
    public class ComplaintController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: Complaint
        [AuthorizePermission]
        protected override void HandleUnknownAction(string actionName)
        {
            RedirectToAction("Search").ExecuteResult(ControllerContext);
        }

        //[System.Web.Mvc.HttpPost]
        //public string GetListingData()
        //{
        //    CustomForm.Post postedForm = new CustomForm.Post(Request);
        //    dynamic data = BlComplaints.GetListingData(new
        //    {
        //        tagId = postedForm.GetElementValue("tagId"),
        //        startDate = postedForm.GetElementValue("startDate"),
        //        endDate = postedForm.GetElementValue("endDate"),
        //        aoData = postedForm.GetElementValue("aoData")
        //    }.ToExpando());
        //    return JsonConvert.SerializeObject(data);

        //}

        [AuthorizePermission]
        public ActionResult Detail(int complaintId)
        {
            return BlView.Instance.GetComplaintDetail(complaintId, "StakeholderComplaintDetail", this);
        }


        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        [AuthorizePermission]
        public ActionResult AgentDetail(int complaintId)
        {
            return BlView.Instance.GetComplaintDetail(complaintId, "AgentComplaintDetail", this);
        }

        [AuthorizePermission]
        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, cookie.UserId, detailType);

            //BlView.Instance.GetConfiguredView((int)cookie.Role, (int)vmComplaintDetail.Compaign_Id, "ComplaintDetail");
            string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, vmComplaintDetail.Compaign_Id), "ComplaintDetail");

            return PartialView(viewPage, vmComplaintDetail);
        }

        [AuthorizePermission]
        public ActionResult TransferComplaint(int complaintId)
        {
            VmTransferComplaint vmComplaintDetail = TransferHandler.GetTransferVmModel(complaintId);
            return PartialView("~/Views/Stakeholder/_TransferComplaint.cshtml", vmComplaintDetail);
        }


       
        public ActionResult GetFieldsByHeirarchyId(int hierarchyId, int campaignId)
        {
            var vmComplaint = new VmAddComplaint();
            var listVmDynamic = DynamicFieldsHandler.GetDynamicFieldsAgainstCampaignId(campaignId).Where(a => a.CategoryHierarchyId >= hierarchyId).OrderBy(n => n.Priority).ToList();
           
            if (listVmDynamic != null && listVmDynamic.Count > 0)
            {
               
                List<VmDynamicDropDownList> listDynamicDropdown = new List<VmDynamicDropDownList>();
                
                
                foreach (VmDynamicField dfField in listVmDynamic)
                {
                    switch (dfField.ControlType)
                    {
                       

                        case Config.DynamicControlType.DropDownList:
                            listDynamicDropdown.Add(dfField as VmDynamicDropDownList);
                            break;
                       
                    }
                }
              
                vmComplaint.ComplaintVm.ListDynamicDropDown = listDynamicDropdown;
              
               
            }
            return PartialView("~/Views/PublicUserComplaints/_AddComplaintPartial.cshtml",vmComplaint);
        }

        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult Index()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();

            ActionResult viewResult = BlView.Instance.GetAgentListView(this, "AgentListingComplaintsAssignedToMe");
            ((ViewResult)viewResult).MasterName = BlView.Instance.GetMasterPageFromCookie();
            return viewResult;
        }

        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult AgentComplaintListingAllServerSide()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            ActionResult viewResult = BlView.Instance.GetAgentListView(this, "AgentListingComplaintsAll");
            ((ViewResult)viewResult).MasterName = BlView.Instance.GetMasterPageFromCookie();
            return viewResult;
        }

        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult AgentSuggestionListingServerSide()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("~/Views/Agent/SuggestionsListingServerSide.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }

        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult AgentInquiryListingServerSide()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            return View("~/Views/Agent/InquiryListingServerSide.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }


        [AuthorizePermission]
        public ActionResult FileViewer(int complaintId, int attachmentRefType, int attachmentRefTypeId)
        {
            VmFileModel vmFileModel = FileHandler.GetVmFileModel(complaintId, attachmentRefType, attachmentRefTypeId);
            return PartialView("_FileViewer", vmFileModel);
        }

        //public string SendSMS()
        //{
        //    PITB.CMS.Handler.Messages.TextMessageHandler.SendMessageOnStatusChange("03214226005",70,345968,240,5,string.Empty);
        //    return "Message sent";
        //}
        /*
        public ActionResult StakeholderComplaints()
        {
            ViewBag.Campaigns = BlCampaign.GetCampaignSelectListItemsFromCookie();
            List<SelectListItem> listCampaings =  ViewBag.Campaigns;
            ViewBag.ComplaintTypeList = BlComplaintType.GetUserCategoriesAgainstCampaign((List<SelectListItem>)ViewBag.Campaigns);
            ViewBag.StatusList = BlCommon.GetStatusListByCampaignId(Convert.ToInt32(listCampaings[0].Value));
            return View("~/Views/Stakeholder/ComplaintListings.cshtml");
        }*/

        //[Route("complaints/all")]
        //[Authorise(Config.Roles.Stakeholder)]
        //[AuthorizePermission]


        [AuthorizePermission]
        public ActionResult StakeholderComplaintsListingServerSide(int ComplaintType = (int)Config.ComplaintType.Complaint)
        {
            ViewResult viewResult = (ViewResult)BlView.Instance.GetListingView(ComplaintType, "ComplaintsMy", this);
            viewResult.ViewBag.PreviousUrl = @Url.Action("StakeholderComplaintsListingServerSide", "Complaint", new { ComplaintType = ComplaintType });
            viewResult.MasterName = BlView.Instance.GetMasterPageFromCookie();

            BlView.Instance.SetStartEndDate(this);

            return viewResult;
        }


        //[Authorise(Config.Roles.Stakeholder)]
        public ActionResult StakeholderComplaintsListingLowerHierarchyServerSide(int ComplaintType = (int)Config.ComplaintType.Complaint)
        {
            ViewBag.StartDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
            ViewBag.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            if (cmsCookie.Role == CMS_Common.Config.Roles.Stakeholder)
            {
                BlView.Instance.SetStartEndDate(this); //SetStartEndDate
                ActionResult actionResult = BlView.Instance.GetListingView(ComplaintType, "ComplaintsLowerHierarchy", this);
                ((ViewResult)actionResult).MasterName = BlView.Instance.GetMasterPageFromCookie();
                return actionResult;
            }
            else
            {
                var navigateCookie = CMSCookie.DbUserToCookie(DbUsers.GetActiveUser((int)cmsCookie.PreviousLoginId));
                navigateCookie.UrlConfig = "Controller::Complaint__Action::StakeholderComplaintsListingLowerHierarchyServerSide";
                BlView.Instance.UpdateUserLogin(cmsCookie.UserId, cmsCookie, navigateCookie);
                return RedirectToAction("Login", "Account");
            }
        }


        [AuthorizePermission]
        [Authorise(Config.Roles.Stakeholder)]
        public ActionResult StakeholderSuggestionListingServerSide(int ComplaintType = (int)Config.ComplaintType.Suggestion)
        {
            ActionResult actionResult = BlView.Instance.GetListingView(ComplaintType, "ComplaintsSuggestion", this);
            ((ViewResult)actionResult).MasterName = BlView.Instance.GetMasterPageFromCookie(); BlView.Instance.SetStartEndDate(this);
            return actionResult;
        }


        [Authorise(Config.Roles.Stakeholder)]
        [AuthorizePermission]
        public ActionResult StakeholderInquiryListingServerSide(int ComplaintType = (int)Config.ComplaintType.Inquiry)
        {
            ActionResult actionResult = BlView.Instance.GetListingView(ComplaintType, "ComplaintsInquiry", this);
            ((ViewResult)actionResult).MasterName = BlView.Instance.GetMasterPageFromCookie(); BlView.Instance.SetStartEndDate(this);
            return actionResult;
        }


        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        [AuthorizePermission]
        public ActionResult SearchOld()
        {
            return View("~/Views/Agent/Search.cshtml");
        }


        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        public ActionResult Search()

        {
            return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie());
        }


        [AuthorizePermission]
        [HttpPost]
        public ActionResult SearchListByCellNo(VmSearch model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.CellNumber))
            {
                ViewBag.RequestTypeCode = "CellNo";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Cell No. is not valid.";

                return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
            }
            List<VmPersonalInfo> vmodel = BlPersonInformation.GetPersonalInfoListByCellNumer(model.CellNumber);
            if (vmodel == null || vmodel.Count == 0)
            {
                ViewBag.RequestTypeCode = "CellNo";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Cell No. does not exist in system.";
                vmodel = null;
            }
            else
            {
                ViewBag.RequestTypeCode = "CellNo";
                ViewBag.RequestIsSuccessful = true;
                ViewBag.RequestStatusText = "Request succeeded.";
            }
            ViewBag.PersonInformation = vmodel;
            return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
        }


        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        [HttpPost]
        public ActionResult SearchByCnic(VmSearch model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.CnicNo))
            {
                ViewBag.RequestTypeCode = "Cnic";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Cnic is not valid.";

                return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
            }
            VmPersonalInfo vmodel = BlPersonInformation.GetPersonalInfoByCnic(model.CnicNo);
            ViewBag.Campaigns = BlComplaints.GetListOfCampaignsByUserId();
            if (vmodel == null)
            {
                ViewBag.RequestTypeCode = "Cnic";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Cnic does not exist in system.";
            }
            else
            {
                ViewBag.RequestTypeCode = "Cnic";
                ViewBag.RequestIsSuccessful = true;
                ViewBag.RequestStatusText = "Request succeeded.";
            }
            ViewBag.PersonInformation = vmodel;
            return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
        }


        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        [HttpPost]
        public ActionResult SearchByCellNo(VmSearch model)
        {

            VmPersonalInfo vmodel = BlPersonInformation.GetPersonalInfoByCellNumer(model.CellNumber);
            if (vmodel == null)
            {
                ViewBag.RequestTypeCode = "CellNo";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Cell No. does not exist in system.";
            }
            else
            {
                ViewBag.RequestTypeCode = "CellNo";
                ViewBag.RequestIsSuccessful = true;
                ViewBag.RequestStatusText = "Request succeeded.";
            }
            ViewBag.PersonInformation = vmodel;
            return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
        }


        [AuthorizePermission]
        [Authorise(Config.Roles.Agent, Config.Roles.AgentSuperVisor)]
        [HttpPost]
        public ActionResult SearchByComplaintNo(VmSearch model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.ComplaintNo))
            {
                ViewBag.RequestTypeCode = "ComplaintNo";
                ViewBag.RequestIsSuccessful = false;
                ViewBag.RequestStatusText = "Complaint No. is not valid.";

                return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
            }

            //if (model.ComplaintNo.Contains("-"))
            {
                VmPersonalInfo vmodel = BlPersonInformation.GetPersonalInfoByComplaintNo(model.ComplaintNo);
                if (vmodel == null)
                {
                    ViewBag.RequestTypeCode = "ComplaintNo";
                    ViewBag.RequestIsSuccessful = false;
                    ViewBag.RequestStatusText = "Complaint No. does not exist in system.";
                }
                else
                {
                    ViewBag.RequestTypeCode = "ComplaintNo";
                    ViewBag.RequestIsSuccessful = true;
                    ViewBag.RequestStatusText = "Request succeeded.";
                }
                ViewBag.PersonInformation = vmodel;
            }
            return View("~/Views/Agent/SearchList.cshtml", BlView.Instance.GetMasterPageFromCookie(), model);
        }


        [AuthorizePermission]
        public ActionResult PersonComplaintsHistory(int Person_Id)
        {
            return PartialView("_PersonComplaints", BlComplaints.GetComplaintForAgentByProfileId(Person_Id));
        }




        // Get : Add complaint Method
        [AuthorizePermission]
        [HttpPost]
        public ActionResult AddComplaint(FormCollection formCollection)
        {
            ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            int campaignId = Convert.ToInt32(formCollection["campaign"]);
            int personId = Convert.ToInt32(formCollection["personId"]);
            ViewBag.Campaignname = DbCampaign.GetById(campaignId).LogoUrl;
            VmAddComplaint vmodel = null;
            string masterPage = null;
            if (Request.IsAjaxRequest())
            {
                masterPage = null;
            }
            else
            {
                masterPage = BlView.Instance.GetMasterPageFromCookie();
            }

            if (campaignId == (int)Config.Campaign.SchoolEducationEnhanced)
            {
                vmodel = BlSchool.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/SchoolEducation/AddSchoolEducation.cshtml", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.WasaNew)
            {
                VmAddComplaintWasa vmodelWasa = BlWasa.GetVmAddComplaintMerged(personId, campaignId);
                return View("~/Views/Complaint/Wasa/AddWasa.cshtml", masterPage, vmodelWasa);
            }
            else if (campaignId == 63)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("AddDynamic", masterPage, vmodel);
            }
            else if (campaignId == 66)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("AddDynamic2", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.DcoOffice)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/DC Office/AddDcOffice.cshtml", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.Police)
            {
                vmodel = BlPolice.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/Police/AddPolice.cshtml", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.DcChiniot)
            {
                //vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), Convert.ToInt32(formCollection["personId"]),
                //campaignId);
                ViewBag.campaignId = campaignId;
                ViewBag.personId = personId;
                return View("~/Views/Complaint/DC Chiniot/AddComplaintDcChiniot.cshtml", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.SpecialEducation)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/SpecialEducation/AddSpecialEducation.cshtml", masterPage, vmodel);
            }
            else if (campaignId == (int)Config.Campaign.LGCD)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/LGCD/AddLGCD.cshtml", masterPage, vmodel);
            }


            else if (campaignId == (int)Config.Campaign.CMCC || campaignId == (int)Config.Campaign.Health || campaignId == (int)Config.Campaign.LGCDCMCC)
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("~/Views/Complaint/CMCC/AddCMCC.cshtml", masterPage, vmodel);
            }

            else
            {
                vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), personId, campaignId);
                return View("Add", masterPage, vmodel);
            }
        }


        //POST : Add complaint method
        [AuthorizePermission]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddComplaintSubmit(VmAddComplaint complaintModel)
        {
            var cook = AuthenticationHandler.GetCookie();
            if (cook.Role != Config.Roles.Agent)
            {
                var result = DbComplaint.GetByComplainRecord((int)complaintModel.PersonalInfoVm.Person_id, (int)complaintModel.ComplaintVm.Complaint_SubCategory);
                if (result.Item2 == 3)
                {
                    TempData["Message"] = StatusMessage(0, string.Format("{0} Complaints already exist for this user against department", result.Item1), Config.Message.ComplainError, Config.Message.ComplainError);
                    return Redirect(RedirectionURL(Request));
                }
                //else if (result.Item1 > 0 && result.Item2 > 0) //item1 db count of complaints, item2 it not-resolved complaint
                //{
                //    TempData["Message"] = StatusMessage(0, "Complaint already exist for this user against department", Config.Message.ComplainError, Config.Message.ComplainError);
                //    return Redirect(RedirectionURL(Request));
                //}
            }
            List<string> listValuesToDiscart = null;

            if (complaintModel.PersonalInfoVm.Person_id > 0)
            {
                listValuesToDiscart = new List<string>()
                {
                    "cnic"
                };
            }
            VmAddComplaint.DiscartUnnecessaryValuesFromModelDictionary(ModelState, complaintModel, false, listValuesToDiscart);
            if (ModelState.IsValid)
            {
                complaintModel.PersonalInfoVm.IsCnicPresent = !complaintModel.PersonalInfoVm.IsCnicPresent;
                var status = BlComplaints.AddComplaint(complaintModel, (complaintModel.PersonalInfoVm.Person_id > 0),
                     (complaintModel.ComplaintVm.Id != 0));
                TempData["Message"] = StatusMessage(status.Status, "Complaint added successfully , your complaint number : " + status.Value, Config.Message.Failure, Config.Message.Error);

                return Redirect(RedirectionURL(Request));

            }
            ViewBag.Ddl = UtilityExtensions.GetDummySelectList();
            VmAddComplaint vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), Convert.ToInt32(complaintModel.PersonalInfoVm.Person_id),
                 Convert.ToInt32(complaintModel.ComplaintVm.Compaign_Id));

            ViewBag.Campaignname = DbCampaign.GetById((int)complaintModel.ComplaintVm.Compaign_Id).LogoUrl;

            return View("Add", vmodel);
        }


        [AuthorizePermission]
        [HttpPost]
        public ActionResult OnStatusChange(VmStatusChange vmStatusChange, IEnumerable<HttpPostedFileBase> files)
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
            }

            if (vmStatusChange.Compaign_Id == 83)
                return RedirectToAction("Index");
            else
                return RedirectToAction("StakeholderComplaintsListingServerSide");
        }


        //[AuthorizePermission]
        [HttpPost]
        public ActionResult OnCategoryChange(VmCategoryChange vmCategoryChange)
        {
            if (ModelState.IsValid)
            {
                Config.CommandMessage status = BlComplaints.ChangeComplaintTypeAndSubType(vmCategoryChange);
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

            return RedirectToAction("StakeholderComplaintsListingServerSide");
        }

        [HttpPost]
        public ActionResult OnUpdateComplaint(string Complaint_Remarks,int District_Id,int ComplaintId)
        {
            var result = DbComplaint.GetByComplaintId(ComplaintId);

            result.Complaint_Remarks = Complaint_Remarks;
            result.District_Name = DbDistrict.GetById(District_Id).District_Name;
            result.District_Id = District_Id;

            if (ModelState.IsValid)
            {
                bool status = BlComplaints.UpdateComplaint(result);
                if (status)
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint Updated Successfully!",
                        Config.Message.UpdateSuccess, Config.Message.UpdateSuccess);
                }
                else
                {
                    TempData["Message"] = StatusMessage(Config.CommandStatus.Failure,CommandStatus.Failed,
                        Config.Message.Failure, Config.Message.Failure);
                }

            }

            return RedirectToAction("StakeholderComplaintsListingLowerHierarchyServerSide");


        }

        [AuthorizePermission]
        [HttpPost]
        public ActionResult OnTransferComplaint(VmTransferComplaint vmTransferComplaint)
        {
            if (ModelState.IsValid)
            {
                TransferHandler.OnTransferComplaint(vmTransferComplaint);
                TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint = " + vmTransferComplaint.ComplaintId + " has been transfered successfully", Config.Message.Failure, Config.Message.Error);

                return RedirectToAction("StakeholderComplaintsListingServerSide");
            }
            return PartialView(null);
        }


        [AuthorizePermission]
        [HttpPost]
        public ActionResult OnFollowUp(VmComplaintDetailAgent vmComplaintDetailAgent)
        {
            if (ModelState.IsValid)
            {
                BlComplaints.OnFollowupSubmit(vmComplaintDetailAgent.ComplaintId, vmComplaintDetailAgent.FollowupComments);
                TempData["Message"] = StatusMessage(Config.CommandStatus.Success, "Complaint = " + vmComplaintDetailAgent.ComplaintId + " has been followed up successfully", Config.Message.Failure, Config.Message.Error);

                return RedirectToAction("Index.cshtml");
            }
            return PartialView(null);
        }


        [AuthorizePermission]
        [HttpGet]
        public ActionResult GetCompaigns(int id = 0)
        {
            ViewBag.PersonId = id;//Sending personId back to view 
            string masterPage = BlView.Instance.GetMasterPageFromCookie();
            var listOfVmCampaigns = BlComplaints.GetListOfCampaignsByUserId();
            if (listOfVmCampaigns != null && listOfVmCampaigns.Count > 1)
            {
                List<VmCampaign> list = listOfVmCampaigns.Where(x => x.Id == 1 || x.Id == 70 || x.Id == 47).ToList();
                if (list != null && list.Count > 0)
                {
                    list.Sort(new CampaignComparer());
                    foreach (VmCampaign item in list)
                    {
                        listOfVmCampaigns.Remove(item);
                    }
                    for (int i = 0; i < list.Count(); i++)
                    {
                        listOfVmCampaigns.Insert(i, list[i]);
                    }
                }
            }

            if (listOfVmCampaigns.Count == 1)
            {

                VmAddComplaint vmodel = null;

                var firstElementFromListofCampaigns = listOfVmCampaigns.FirstOrDefault();


                if (firstElementFromListofCampaigns.Id == (int)Config.Campaign.Police)
                {
                    ViewBag.Campaignname = DbCampaign.GetById(firstElementFromListofCampaigns.Id).LogoUrl;
                    vmodel = BlPolice.GetVmAddComplaintMerged(new VmAddComplaint(),
                        Convert.ToInt32(id),
                        firstElementFromListofCampaigns.Id);

                    return View("~/Views/Complaint/Police/AddPolice.cshtml", masterPage, vmodel);
                }
                else if (firstElementFromListofCampaigns.Id == (int)Config.Campaign.SpecialEducation)
                {
                    ViewBag.Campaignname = DbCampaign.GetById(firstElementFromListofCampaigns.Id).LogoUrl;
                    vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(),
                        Convert.ToInt32(id),
                        firstElementFromListofCampaigns.Id);
                    return View("~/Views/Complaint/SpecialEducation/AddSpecialEducation.cshtml", masterPage, vmodel);
                }
                else if (firstElementFromListofCampaigns.Id == (int)Config.Campaign.LGCD)
                {
                    ViewBag.Campaignname = DbCampaign.GetById(firstElementFromListofCampaigns.Id).LogoUrl;
                    vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(),
                        Convert.ToInt32(id),
                        firstElementFromListofCampaigns.Id);
                    return View("~/Views/Complaint/LGCD/AddLGCD.cshtml", masterPage, vmodel);
                }
                else
                {
                    vmodel = BlComplaints.GetVmAddComplaintMerged(new VmAddComplaint(), id,
                        //Id is personId here
                        firstElementFromListofCampaigns.Id);
                    ViewBag.Campaignname = firstElementFromListofCampaigns.LogoUrl;
                }

                return View("Add", masterPage, vmodel);
            }
            ViewBag.PersonId = id;
            if (Request.IsAjaxRequest())
            {
                masterPage = null;
            }

            return View("~/Views/Agent/_CampaignsList.cshtml", masterPage, listOfVmCampaigns);
        }

        #region Excel Export
        //[HttpGet]
        //public ActionResult Export(string startDate, string endDate, string category, string status, string soc, string districts, string towns, string departments, string sType)
        //{

        //    DataTable data = new DataTable();
        //    if (data.Rows.Count > 0)
        //    {
        //        ExcelPackage excelDocument = (new Common().ExportToExcel(data, "Health CMS Complaints"));
        //        excelDocument.SaveAs(Response.OutputStream);

        //    }
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment;  filename=" + "Complaints" + "Of_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
        //    return Content("");
        //}
        #endregion
    }
}