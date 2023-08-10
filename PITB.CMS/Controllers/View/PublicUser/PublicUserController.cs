using CaptchaMvc.HtmlHelpers;
using Newtonsoft.Json;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Route;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Attributes;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Account;
using PITB.CMS_Common.Models.View.PublicUser;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PITB.CMS.Controllers.View
{
    public class PublicUserController : PITB.CMS_Common.Controllers.View.Controller
    {
        // GET: PublicUser
        public ActionResult Login()
        {
            Config.Roles userRole;
            if (new AuthenticationHandler().GetAuthenticationFromCookie(out userRole))
            {
                if (userRole == Config.Roles.PublicUser)
                {
                    BlAccount.UpdateLastOpenedInfo();
                    var url = RouteHandler.GetUrl((byte)userRole);
                    return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
                }
                else
                    return RedirectToAction("SignOut");
            }

            return View("~/Views/PublicUser/LandingPage.cshtml");
        }
        [AllowAnonymous]
        public ActionResult LoginRedirect()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            Config.Roles userRole;
            if (new AuthenticationHandler().GetAuthenticationFromCookie(out userRole))
            {
                if (userRole == Config.Roles.PublicUser)
                {
                    BlAccount.UpdateLastOpenedInfo();
                    var url = RouteHandler.GetUrl((byte)userRole);
                    return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
                }
                else
                    return RedirectToAction("SignOut");
            }
            else
            {
                return View("~/Views/PublicUser/Login.cshtml");
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(VmLoginPublicUser userModel, FormCollection form, string returnUrl)
        {
            if (!this.IsCaptchaValid("Captcha is Invalid"))
            {
                TempData["ErrorMessage"] = "Validation Error";
                return Redirect(Request.UrlReferrer.ToString());
            }




            AuthenticationHandler authHandler = new AuthenticationHandler();
            if (ModelState.IsValid)
            {
                DbUsers user = BlPublicUser.GetUserAgainstEmailAndPassword(userModel.Username, userModel.Password);

                if (user != null && user.Role_Id == Config.Roles.PublicUser)
                {
                    if (authHandler.GetAuthenticationFromCredentialsPublicUser(userModel.Username, userModel.Password))
                    {
                        var url = RouteHandler.GetUrl((byte)AuthenticationHandler.GetCookie().Role);
                        BlAccount.UpdateLastLoginInfoLogin();
                        return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Username or password is incorrect";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Unauthorized login attempt";
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        //[AllowAnonymous]
        public virtual ActionResult SignUpPage()
        {
            ViewBag.Provinces = DbProvince.AllProvincesList();
           
            return PartialView("~/Views/PublicUser/_PartialPublicUserSignUp.cshtml");
        }

        [AjaxRequestOnly]
        public JsonResult GetDistrict(int ProvinceId)
        {
            var districts = DbDistrict.GetDistrictsNameAndIDByProvinceId(ProvinceId);
            return Json(districts, JsonRequestBehavior.AllowGet);
        }


        [AjaxRequestOnly]
        [ValidateJsonAntiForgeryToken]
        public virtual JsonResult SignUpUser(VmSignUp data)
        {
            dynamic message = new ExpandoObject();
            try
            {
                //HelperFunctions.CustomValidator(data);
                //var isvalid = HelperFunctions.Validator(data);
                if (!ModelState.IsValid)
                {
                    message = "notValid";
                }
                else
                {
                    dynamic d = new ExpandoObject();
                    d.UserName = data.UserName.Trim();
                    d.UserCnic = data.UserCnic.Trim();
                    d.UserMobileNo = data.UserMobileNo.Trim();
                    d.UserEmail = data.UserEmail.Trim();
                    d.UserPassword = data.UserPassword.Trim();
                    d.ConfirmPassword = data.ConfirmPassword.Trim();
                    d.UserGender = data.UserGender;
                    d.UserProvince = data.UserProvince;
                    d.UserDistrict = data.UserDistrict;
                    d.UserAddress = data.UserAddress.Trim();
                   // d.Campaign_Id = Config.Campaign.DcChiniot;


                    d.Campaign_Id = Config.Campaign.CMCC;
                    d.Campaigns = new string[]{
                        ((byte)Config.Campaign.CMCC).ToString(),
                        ((byte)Config.Campaign.SchoolEducationEnhanced).ToString(),
                        ((byte)Config.Campaign.DcoOffice).ToString(),
                        ((byte)Config.Campaign.Police).ToString(),
                        ((byte)Config.Campaign.Health).ToString(),
                        ((byte)Config.Campaign.LGCDCMCC).ToString(),



                    };


                    d.OTP = data.OTP;
                    d.src = "web";

                    dynamic result = BlPublicUser.SavePublicUser(d);
                    message = result.message;
                }
            }
            catch (Exception ex)
            {
                message = "error";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        [AjaxRequestOnly]
        public JsonResult SendOtp(string Email, string Cnic,string Phone)
        {
            dynamic d = new ExpandoObject();
            d.email = Email;
            d.cnic = Cnic;
            d.phone = Phone;
            d.src = "web";
            dynamic resp;

            try
            {
                resp = BlPublicUser.SendOtp(d);
                return Json(resp.data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult SignOut()
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            string urlSuffix = null;
            if (cmsCookie.Role == Config.Roles.Stakeholder)
            {
                urlSuffix = BlCampaign.GetCampaignUrlSuffixAgainstCampaignId(cmsCookie.Campaigns);
            }

            BlAccount.UpdateSignOutInfo();

            //Removing cookie
            new AuthenticationHandler().LogOut();
            if (string.IsNullOrEmpty(urlSuffix))
            {
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("LoginWithCampaign", new { id = urlSuffix });
            }
        }


        public ActionResult Settings()
        {
            return View("~/Views/PublicUser/Settings.cshtml", BlView.Instance.GetMasterPageFromCookie(), BlUsers.GetUserModel());
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult OnSettingSave(VmUserSettings model)
        {

            if (ModelState.IsValid)
            {
                return Json(BlUsers.UpdatePublicUserProfile(model), JsonRequestBehavior.AllowGet);
            }
            return View("~/Views/PublicUser/Settings.cshtml", model);
        }


        public ActionResult ChangePassword()
        {
            return PartialView("~/Views/PublicUser/_ChangePassword.cshtml", new VmChangePassword());
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult OnChangePassword(VmChangePassword modelPassword)
        {
            if (ModelState.IsValid)
            {
                return Json(BlUsers.ChangePassword(modelPassword), JsonRequestBehavior.AllowGet);
            }
            return View("~/Views/PublicUser/Settings.cshtml", BlView.Instance.GetMasterPageFromCookie(), modelPassword);
        }

    }
}