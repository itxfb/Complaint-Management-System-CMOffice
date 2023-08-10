using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Web.UI;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom.Police;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.View.Police;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Handler.Complaint.Transfer;
using PITB.CMS_Common.Handler.ExportFileHandler;
using PITB.CMS_Common.Handler.Authorization;
using PITB.CMS_Common.Helper.Extensions;
//using PITB.CMS_Common.Handler.Authorization;

namespace PITB.CMS.Controllers.View.Police
{
    [AuthorizePermission]
    public class PoliceController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: Police
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetAgentComplaints([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] int complaintType, [FromBody] int listingType)
        {
            string commaSeperatedCampaigns = string.Join(",", campaign);

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            DataTable dt = BlPolice.GetComplaintListings(dtModel, from, to, commaSeperatedCampaigns, complaintType, "Listing", listingType);
            dt = Utility.GetAlterStatusDataTable(dt, "Campaign_Id", "Status", Config.UnsatisfactoryClosedStatus, Config.SchoolEducationUnsatisfactoryStatus);
            //dt = BlAgent.GetAlterDataTable(dt);
            List<VmAgentComplaintListing> dataTable = dt.ToList<VmAgentComplaintListing>();
            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddComplaintSubmit(VmAddComplaint complaintModel/*, FormCollection collection*/, IEnumerable<HttpPostedFileBase> files)
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
            VmAddComplaint.DiscartUnnecessaryValuesFromModelDictionary(ModelState, complaintModel, false, listValuesToDiscart);
            //if (ModelState.IsValid)
            {
                //string formVal = collection["ComplaintVm.ListDynamicDropDown[0].SelectedItemId"].ToString();
                complaintModel.PersonalInfoVm.IsCnicPresent = !complaintModel.PersonalInfoVm.IsCnicPresent;

                Request.InputStream.Seek(0, SeekOrigin.Begin);
                //string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
                //string radioPersonType = collection["PersonType"];
                //string radioPersonType2 = Request.Form["PersonType"];
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                PoliceModel.AddComplaint addComplaint = new PoliceModel.AddComplaint(cmsCookie, DateTime.Now, Config.SourceType.Web, complaintModel, System.Web.HttpContext.Current.Request.Files, Request.Form, (complaintModel.PersonalInfoVm.Person_id > 0),
                     (complaintModel.ComplaintVm.Id != 0));
                //var status = BlPolice.AddComplaint(complaintModel, Request.Files, Request.Form, (complaintModel.PersonalInfoVm.Person_id > 0),
                //     (complaintModel.ComplaintVm.Id != 0));
                //PoliceModel.AddComplaint addComplaint2 = null;
                //            try
                //            {
                ////                string reqStr = JsonConvert.SerializeObject(context.Request,
                ////Formatting.Indented, new JsonSerializerSettings
                ////{
                ////    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ////    ContractResolver = new IgnoreErrorPropertiesResolver()
                ////});
                //                //JsonSerializerSettings settings = new JsonSerializerSettings();
                //                //settings.Error = (serializer, err) => err.ErrorContext.Handled = true;
                //                //settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //                string json = JsonConvert.SerializeObject(addComplaint, Utility.GetJsonSettings());
                //                //System.IO.File.WriteAllText(@"D:\path.txt", json);
                //                addComplaint2 = JsonConvert.DeserializeObject<PoliceModel.AddComplaint>(json, Utility.GetJsonSettings());
                //            }
                //            catch (Exception ex)
                //            {

                //            }
                var status = BlPolice.AddComplaint(addComplaint);
                //     (complaintModel.ComplaintVm.Id != 0));
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

            return View("~/Views/Complaint/Police/AddPolice.cshtml", vmodel);
            //return View("Add", vmodel);
        }
        //public ActionResult Detail(int complaintId)
        //{
        //    VmComplaintDetail vmComplaintDetail = BlComplaints.GetComplaintDetail(complaintId);
        //    return View("_ComplaintDetail", vmComplaintDetail);
        //}

        public ActionResult StakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, cookie.UserId, detailType);
            return PartialView("~/Views/Stakeholder/Police/PoliceDetail.cshtml", vmComplaintDetail);
        }

        public ActionResult ComplaintAction(int complaintId/*, VmStakeholderComplaintDetail.DetailType detailType*/)
        {
            //VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId/*, detailType*/);
            Pair<string, string> forgeryToken = Utility.GenerateForgeryKeyAgainstRequest(Request);
            VmPolice.VmComplaintAction vmComplaintAction = BlPolice.GetActionForm(complaintId);
            return PartialView("~/Views/Stakeholder/Police/_ComplaintActions.cshtml", vmComplaintAction);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PostComplaintAction(FormCollection fc)
        {
            HttpFileCollectionBase files = Request.Files;
            NameValueCollection formCollection = Request.Form;


            Config.CommandMessage commandMessage = null;
            ControllerModel.Response resp = new ControllerModel.Response();
            try
            {

                //BlPolice.SaveComplaintAction(new CustomForm.Post(Request));
                //string fintla = jsonData;
                //VmPolice.VmComplaintAction vmComplaintAction =
                //    BlPolice.GetActionForm(Convert.ToInt32(postForm.DictQueryParams["complaintId"]));
                //TempData["Message"] = StatusMessage(Config.CommandStatus.Success,
                //    "Complaint action has been submitted successfully ComplaintId  = " +
                //    vmComplaintAction.ComplaintIdStr, Config.Message.UpdateSuccess, Config.Message.UpdateSuccess);
                //int asd = Convert.ToInt32("asdasd");
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                PoliceModel.AddAction addActionModel = new PoliceModel.AddAction(cmsCookie, postForm, DateTime.Now, Config.SourceType.Web/*, Config.OtherSystemId.Police*/, postForm.DictQueryParams["complaintId"]);
                commandMessage = BlPolice.SaveComplaintAction(addActionModel /*new CustomForm.Post(Request)*/);
                resp.RedirectUrl = Url.Action("StakeholderComplaintsListingServerSide", "Complaint", new { ComplaintType = (int)Config.ComplaintType.Complaint });
                //VmMessage vmMessage = StatusMessage(Config.CommandStatus.Success, commandMessage.Value, Config.Message.UpdateSuccess, Config.Message.UpdateSuccess);
                //string partialViewHtml = RenderViewToString(this.ControllerContext, "~/Views/Shared/ViewUserControls/_MessageBox.cshtml", vmMessage, true);
                //resp.AddPartialView(resp.ListPartialViewToLoadAfterRedirect, "#PopupDiv2", partialViewHtml);
                resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMessage);
            }
            catch (Exception ex)
            {
                //TempData["Message"] = StatusMessage(Config.CommandStatus.Exception,
                //    "An exception has occurred while submitting action of ComplaintId  = " +
                //    Convert.ToInt32(postForm.DictQueryParams["complaintId"]), Config.Message.Error, Config.Message.Error);
                resp.AddMessagePartialView(this, resp.ListPartialView, Config.CommandMessage.GetFailureMessage());
            }
            return Json(resp);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult PostComplaintFeedback(FormCollection fc)
        {
            HttpFileCollectionBase files = Request.Files;
            NameValueCollection formCollection = Request.Form;


            Config.CommandMessage commandMessage = null;
            ControllerModel.Response resp = new ControllerModel.Response();
            try
            {
                CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                PoliceModel.AddFeedback addFeedbackModel = new PoliceModel.AddFeedback(cmsCookie, postForm, DateTime.Now, Config.SourceType.Web/*, Config.OtherSystemId.Police*/, postForm.DictQueryParams["complaintId"]);
                commandMessage = BlPolice.SubmitComplaintFeedback(addFeedbackModel/*new CustomForm.Post(Request)*/);
                resp.RedirectUrl = Url.Action("AgentComplaintListingAllServerSide", "Complaint");
                resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMessage);
            }
            catch (Exception ex)
            {
                //TempData["Message"] = StatusMessage(Config.CommandStatus.Exception,
                //    "An exception has occurred while submitting action of ComplaintId  = " +
                //    Convert.ToInt32(postForm.DictQueryParams["complaintId"]), Config.Message.Error, Config.Message.Error);
                resp.AddMessagePartialView(this, resp.ListPartialView, Config.CommandMessage.GetFailureMessage());
            }
            return Json(resp);
        }




        public ActionResult TransferComplaint(int complaintId)
        {
            VmTransferComplaint vmComplaintDetail = TransferHandler.GetTransferVmModel(complaintId);
            return PartialView("~/Views/Stakeholder/Police/_PoliceTransferComplaint.cshtml", vmComplaintDetail);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult GetDashboardLabelsStakeholderData([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] string dashboardType, [FromBody] int userId = -1)
        {
            string spType = "";
            if (dashboardType == "_DashboardLabelStatus")
            {
                spType = "DashboardLabelsStausWise";
            }
            else if (dashboardType == "_DashboardLabelComplaintSrc")
            {
                spType = "DashboardLabelsComplaintSrc";
            }
            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }

            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);
            List<VmStakeholderComplaintDashboard> data = BlPolice.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                spType,
                userId).ToList<VmStakeholderComplaintDashboard>();


            //ExcelPackage data = (ExcelPackage) DataStateMVC.GetDataFromPool(dataId);
            //DataStateMVC.RemoveFromPool(dataId);
            //return FileHandler.Generate(Response, data, "ComplaintsListingData.xlsx");
            return Json(data, JsonRequestBehavior.AllowGet);
            //return Json("aasasd", JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        public JsonResult GetStakeholderComplaintsServerSide([FromBody] string aoData, [FromBody] string from, [FromBody] string to, [FromBody] string[] campaign, [FromBody] string[] cateogries, [FromBody] string[] statuses, [FromBody] string[] transferedStatus, [FromBody] int complaintType, [FromBody] int listingType, [FromBody] int userId = -1)
        {

            string commaSeperatedCampaigns = string.Join(",", campaign);
            string commaSeperatedCategories = string.Join(",", cateogries);
            string commaSeperatedTransferedStatus = string.Join(",", transferedStatus);
            string commaSeperatedStatuses = "";
            if (statuses != null)
            {
                commaSeperatedStatuses = string.Join(",", statuses);
            }


            DataTableParamsModel dtModel = DataTableHandler.ConvertaoDataToModel(aoData);

            List<VmStakeholderComplaintListing> dataTable = BlPolice.GetStakeHolderServerSideListDenormalized(
                from,
                to,
                dtModel,
                commaSeperatedCampaigns,
                commaSeperatedCategories,
                commaSeperatedStatuses,
                commaSeperatedTransferedStatus,
                complaintType,
                (Config.StakeholderComplaintListingType)listingType,
                "Listing",
                userId).ToList<VmStakeholderComplaintListing>();

            int totalRows = dataTable.Count == 0 ? 0 : dataTable.First().Total_Rows;

            return Json(new
            {
                data = dataTable,
                draw = dtModel.Draw,
                recordsTotal = totalRows,//dtModel.Length,
                recordsFiltered = totalRows, //dtModel.Length < rows ? dtModel.Length : rows//dtModel.Length
            }, JsonRequestBehavior.AllowGet);
        }

        //[System.Web.Mvc.HttpPost]
        //public string GetListingData()
        //{
        //    CustomForm.Post postedForm = new CustomForm.Post(Request);
        //    dynamic data = BlPolice.GetListingData(new
        //    {
        //        tagId = postedForm.GetElementValue("tagId"),
        //        startDate = postedForm.GetElementValue("startDate"),
        //        endDate = postedForm.GetElementValue("endDate"),
        //        aoData = postedForm.GetElementValue("aoData")
        //    }.ToExpando());
        //    return JsonConvert.SerializeObject(data);

        //}


        [System.Web.Mvc.HttpPost]
        public JsonResult ExportStakeHolderData()
        {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
            int dataId = BlPolice.Export(jsonData);
            return Json(dataId, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseBase ExportStakeHolderData(int dataId)
        {
            ExcelPackage data = (ExcelPackage)DataStateMVC.GetDataFromPool(dataId);
            string fileName = DataStateMVC.GetStoredObjectFromPool(dataId).FileName;
            string startDate = DataStateMVC.GetStoredObjectFromPool(dataId).StartDate;
            string endDate = DataStateMVC.GetStoredObjectFromPool(dataId).EndDate;
            string fileNameFull = string.Format("{0} {1} {2} {3}", fileName, startDate, endDate, "ComplaintsListingData.xlsx");
            DataStateMVC.RemoveFromPool(dataId);
            return FileHandler.Generate(Response, data, fileNameFull);
        }

    }
}