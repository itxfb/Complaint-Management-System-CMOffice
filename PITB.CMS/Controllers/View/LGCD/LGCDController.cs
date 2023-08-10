using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.Custom.Police;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Handler.Authorization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PITB.CMS.Controllers.View.LGCD
{
    [AuthorizePermission]
    public class LGCDController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: LGCD
        public ActionResult Index()
        {
            return View();
        }
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OnAddComplaintSubmit(VmAddComplaint complaintModel,FormCollection collection, IEnumerable<HttpPostedFileBase> files, int Person_id = 0)
        {
            var cook = AuthenticationHandler.GetCookie();


            //var result = DbComplaint.GetByComplainRecord((int)complaintModel.PersonalInfoVm.Person_id, (int)complaintModel.ComplaintVm.Complaint_SubCategory);
            //if (result.Item2 == 3)
            //{
            //    TempData["Message"] = StatusMessage(0, string.Format("{0} Complaints already exist for this user against department", result.Item1), Config.Message.ComplainError, Config.Message.ComplainError);
            //    return Redirect(RedirectionURL(Request));
            //}
            //if (cook.Role == Roles.PublicUser)
            //{
            //    var result = DbComplaint.GetByComplainRecord((int)complaintModel.PersonalInfoVm.Person_id, (int)complaintModel.ComplaintVm.Complaint_SubCategory);

            //    if (result.Item1 > 0 && result.Item2 > 0)
            //    {
            //        TempData["Message"] = StatusMessage(0, "Complaint already exist for this user against department", Config.Message.ComplainError, Config.Message.ComplainError);
            //        return Redirect(RedirectionURL(Request));
            //    }
            //}
            Config.CommandMessage commandMessage = null;
            ControllerModel.Response resp = new ControllerModel.Response();
            try
            {
                //Dictionary<string,string> dictForm = Utility.GetDictionary(Request.Form);
                CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);



                if (postForm.ListElementData.FirstOrDefault(x => x.Key == "ComplaintVm.departmentId") == null ? false : String.IsNullOrEmpty(postForm.ListElementData.FirstOrDefault(x => x.Key == "ComplaintVm.departmentId").Value))
                {
                    try
                    {
                       
                        var department = JObject.Parse(collection["departmentId"])["Value"];
                        CustomForm.ElementData departmentId = new CustomForm.ElementData
                        {
                            Key = "ComplaintVm.departmentId",
                            Value = department.ToString()
                        };
                        
                        postForm.ListElementData[postForm.ListElementData.FindIndex(x => x.Key == "ComplaintVm.departmentId")] = departmentId;
                        
                        var tehsil = JObject.Parse(collection["ComplaintVm.ListDynamicDropDown[0].Setting"])["Value"];
                        CustomForm.ElementData tehsilId = new CustomForm.ElementData
                        {
                            Key = "ComplaintVm.Tehsil_Id",
                            Value = tehsil.ToString()
                        };
                        postForm.ListElementData.Add( tehsilId);
                    }
                    catch
                    {

                    }
                   
                }


                string str = JsonConvert.SerializeObject(postForm.ListElementData);
                commandMessage = BlLGCD.AddComplaint(postForm);
                //if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("publicuser"))
                //{
                //    resp.RedirectUrl = Url.Action("Index", "PublicUserComplaints");
                //}
                //else
                //    resp.RedirectUrl = Url.Action("Search", "Complaint"/*, new { ComplaintType = (int)Config.ComplaintType.Complaint }*/);
                resp.RedirectUrl = RedirectionURL(Request);
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
    }
}