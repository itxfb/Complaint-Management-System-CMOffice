using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View.DcChiniot
{
    public class DcChiniotController : PITB.CMS_Common.Controllers.View.Controller
    {
        public const string TagStatusChange = "TagStatusChange";
        public const string TagTimeChange = "TagTimeChange";
        public const string TagAddComplaint = "TagAddComplaint";
        public const string TagCategoryChange = "TagCategoryChange";

        // GET: DcChiniot
        public ActionResult GetViewPersonInfo(int personId)
        {
            //CustomForm.Post postForm = new CustomForm.Post(Request);

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            //CustomForm.Post postForm = new CustomForm.Post(Request);
            //personId = (personId == 0) ? (int)cmsCookie.PersonalInfo_Id : personId;
            ViewBag.personForm = BlDcChiniot.GetPersonInfo(personId/*int.Parse(postForm.GetElementValue("personId"))*/);

            return PartialView("~/Views/Complaint/DC Chiniot/_AddPersonlInfoDcChiniot.cshtml");
            //return View("");
        }
        public ActionResult GetViewComplaintSection(int campaignId)
        {
            //CustomForm.Post postForm = new CustomForm.Post(Request);

            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
            ViewBag.campaignForm = BlDcChiniot.GetComplaintInfo(campaignId);

            return PartialView("~/Views/Complaint/DC Chiniot/_AddComplaintDcChiniot.cshtml");
            //return View("");
        }

        //[System.Web.Mvc.HttpPost]
        //public ActionResult OnAddComplaintSubmit()
        //{
        //    Config.CommandMessage commandMessage = null;
        //    ControllerModel.Response resp = new ControllerModel.Response();
        //    try
        //    {
        //        //Dictionary<string,string> dictForm = Utility.GetDictionary(Request.Form);
        //        CustomForm.Post postForm = new CustomForm.Post(Request);
        //        dynamic d = new ExpandoObject();
        //        d.postedForm = postForm;
        //        d.srcTag = "src::web__module::agent";
        //        string str = JsonConvert.SerializeObject(postForm.ListElementData);
        //        postForm.PrintForm();

        //        commandMessage = BlDcChiniot.AddComplaint2(d);
        //        //return null;
        //        //if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("publicuser"))
        //        //    resp.RedirectUrl = Url.Action("Index", "PublicUserComplaints");
        //        //else
        //        //    resp.RedirectUrl = Url.Action("Search", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
        //        resp.RedirectUrl = RedirectionURL(Request);
        //        resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, commandMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        //TempData["Message"] = StatusMessage(Config.CommandStatus.Exception,
        //        //    "An exception has occurred while submitting action of ComplaintId  = " +
        //        //    Convert.ToInt32(postForm.DictQueryParams["complaintId"]), Config.Message.Error, Config.Message.Error);
        //        resp.AddMessagePartialView(this, resp.ListPartialView, Config.CommandMessage.GetFailureMessage());
        //    }
        //    return Json(resp);
        //}

        public ActionResult GetViewStakeholderDetail(int complaintId, VmStakeholderComplaintDetail.DetailType detailType)
        {
            //VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(complaintId, detailType);
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            VmStakeholderComplaintDetail vmComplaintDetail = BlDcChiniot.GetComplaintDetail(complaintId, cookie.UserId, detailType);
            return PartialView("~/Views/Stakeholder/DcChiniot/DcChiniotDetail.cshtml", vmComplaintDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostData()
        {
            //Config.CommandMessage commandMessage = null;
            dynamic blResp = null;
            ControllerModel.Response resp = new ControllerModel.Response();
            try
            {
                CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                string tagId = postForm.GetElementValue("tagId").ToString();
                //string str = JsonConvert.SerializeObject(postForm.ListElementData);
                postForm.PrintForm();

                CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
                dynamic dParam = new ExpandoObject();
                dParam.postedForm = postForm;
                dParam.srcTag = "src::web__module::agent";
                switch (tagId)
                {
                    case TagAddComplaint:
                        
                        blResp = BlDcChiniot.AddComplaint2(dParam);
                        if (cmsCookie.Role == Config.Roles.Agent || cmsCookie.Role == Config.Roles.AgentSuperVisor)
                        {
                            resp.RedirectUrl = Url.Action("Search", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        }
                        else if (cmsCookie.Role == Config.Roles.PublicUser)
                        {
                            resp.RedirectUrl = Url.Action("Index", "PublicUserComplaints"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        }
                        //resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, blResp);
                        //resp.RedirectUrl = Url.Action(Request.UrlReferrer.Segments[2], Request.UrlReferrer.Segments[1].Remove(Request.UrlReferrer.Segments[1].Length - 1, 1)/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, blResp);
                        break;
                    //case TagTimeChange:
                    //    //dynamic dForm = postForm.GetDynamicForm();
                    //    blResp = BlComplaints.ChangeComplaintTiming(postForm.GetDynamicForm());//BlDcChiniot.PostTimeChange(dParam);
                    //    resp.AddMessagePartialView(this, resp.ListPartialView, blResp);
                    //    break;
                    case TagCategoryChange:
                        //dynamic dForm = postForm.GetDynamicForm();
                        dynamic dForm = new ExpandoObject();
                        dForm.complaintId = int.Parse(postForm.GetElementValue("complaintId"));
                        string complaintTime = postForm.GetElementValue("complaintTime");
                        dForm.complaintTime = float.Parse(complaintTime == ""?"0":complaintTime);
                        dForm.categoryId = int.Parse(postForm.GetElementValue("VmCategoryChange.selectedComplaintCategory"));
                        dForm.subcategoryId = int.Parse(postForm.GetElementValue("VmCategoryChange.selectedComplaintSubCategory"));
                        dForm.userId = cmsCookie.UserId;
                        BlComplaints.ChangeComplaintTiming(dForm);
                        blResp = BlComplaints.ChangeComplaintCategory(dForm);
                        resp.RedirectUrl = Url.Action(Request.UrlReferrer.Segments[2], Request.UrlReferrer.Segments[1].Remove(Request.UrlReferrer.Segments[1].Length - 1, 1)/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, blResp);
                        break;
                    case TagStatusChange:
                       
                        blResp = BlDcChiniot.PostStatusChange(dParam);
                        //resp.RedirectUrl = Url.Action("StakeholderComplaintsListingServerSide", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        //var asd = Request.UrlReferrer;
                        resp.RedirectUrl = Url.Action( Request.UrlReferrer.Segments[2], Request.UrlReferrer.Segments[1].Remove(Request.UrlReferrer.Segments[1].Length - 1, 1)/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                        resp.AddMessagePartialView(this, resp.ListPartialViewToLoadAfterRedirect, blResp);
                        break;
                }
            }
            catch (Exception ex)
            {
                resp.AddMessagePartialView(this, resp.ListPartialView, Config.CommandMessage.GetFailureMessage());
            }
            return Json(resp);
        }

        public ActionResult PostTime()
        {
            return null;
        }

        public ActionResult PostStatus()
        {
            return null;
        }
    }
}