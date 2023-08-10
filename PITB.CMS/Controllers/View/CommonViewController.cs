using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Models.View;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    public class CommonViewController : PITB.CMS_Common.Controllers.View.Controller
    {
        [HttpGet]
        public ActionResult GetErrorPage()
        {
            return View("~/Views/ErrorPage.cshtml");
        }

        [HttpGet]
        public ActionResult NotFoundPage()
        {
            return View("~/Views/NotFoundPage.cshtml");
        }

        [HttpGet]
        public ActionResult ServerErrorPage()
        {
            return View("~/Views/SeverError.cshtml");
        }


        // GET: CommonView
        [System.Web.Mvc.HttpPost]
        public JsonResult FormPost()
        {
            CMSCookie cookie = AuthenticationHandler.GetCookie();
            Config.CommandMessage commandMessage = null;
            ControllerModel.Response controllerResp = new ControllerModel.Response();

            VmStakeholderComplaintDetail vmComplaintDetail = BlComplaints.GetStakeholderComplaintDetail(123, cookie.UserId, VmStakeholderComplaintDetail.DetailType.AssignedToMe);
            
            //BlView.Instance.GetConfiguredView((int)cookie.Role, (int)vmComplaintDetail.Compaign_Id, "ComplaintDetail");
            string viewPage = BlView.Instance.GetConfiguredView(string.Format("Type::Config___Module::Pages___RoleId::{0}", (int)cookie.Role),
                string.Format("Type::Config___Module::Pages___RoleId::{0}___Campaign::{1}", (int)cookie.Role, vmComplaintDetail.Compaign_Id), "ComplaintDetail");
            controllerResp.AddMessagePartialView(this, controllerResp.ListPartialView, Config.CommandMessage.GetFailureMessage());
            //return PartialView(viewPage, vmComplaintDetail);

            //---------------

            //Config.CommandMessage commandMessage = null;
            //ControllerModel.Response controllerResp = new ControllerModel.Response();
            try
            {
                CustomForm.Post postForm = new CustomForm.Post(System.Web.HttpContext.Current.Request);
                Dictionary<string,string> dictMethod = Utility.ConvertCollonFormatToDict(Config.DictFormPostTag[postForm.GetElementValue("PostTag")]);
                dynamic d = new ExpandoObject();
                if (dictMethod["MethodType"] == "Static")
                {
                    //postForm.ControllerResponse = new ControllerModel.Response();
                    d.postForm = postForm;
                    d.controllerResponse = controllerResp;
                    Type t = Type.GetType(dictMethod["ClassName"]);
                    MethodInfo staticMethodInfo = t.GetMethod(dictMethod["MethodName"]);
                    dynamic response = Convert.ToInt32(staticMethodInfo.Invoke(null, new object[] {d/*, 5*/}));
                    JsonResult jsResult = Json(response, JsonRequestBehavior.AllowGet);
                    jsResult.MaxJsonLength = 999999999;
                    return jsResult; 
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
            //return View();
        }
    }
}