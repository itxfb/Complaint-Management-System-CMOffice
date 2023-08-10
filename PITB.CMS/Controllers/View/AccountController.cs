using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System;
using CaptchaMvc.HtmlHelpers;
using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Handler.Route;
using PITB.CMS_Common.Models.View.ClientMesages;
using PITB.CMS_Common.Models.View.Account;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View.Executive;
using System.Dynamic;
using System.Text;

namespace PITB.CMS.Controllers
{
    public class AccountController : PITB.CMS_Common.Controllers.View.Controller
    {
        //
        // GET: /Account/

        public ActionResult Login()
        {

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            Config.Roles userRole;
            if (new AuthenticationHandler().GetAuthenticationFromCookie(out userRole))
            {
                //BlComplaints.AddComplaint(new VmAddComplaint(), false, false);
                BlAccount.UpdateLastOpenedInfo();
                var url = RouteHandler.GetUrl((byte)userRole);
                return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
            }
            else
            {
                return View(null, null);
            }
        }




        #region restore password
        public ActionResult PasswordRestore()
        {
            ViewBag.Controller = (Request.UrlReferrer != null && Request.UrlReferrer.ToString().ToLower().Contains("publicuser")) ? "PublicUser" : "Account";
            return View("~/Views/Account/PasswordRestore.cshtml", null /*BlView.Instance.SetMasterPageInCookie()*/);
        }

        public ActionResult EnterUsername()
        {
            return PartialView("~/Views/Account/_EnterUsername.cshtml", new VmUserSettings());
        }
        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        public JsonResult AuthenticateUsernameCheck([System.Web.Http.FromBody] string username)
        {
            ClientMessage message = new ClientMessage("Please enter username", false);
            AuthenticationHandler authHandler = new AuthenticationHandler();
            if (username != null)
            {
                DbUsers user = authHandler.GetAuthenticatedUserFromUsername(username);
                if (user != null)
                {
                    if (!String.IsNullOrWhiteSpace(user.Phone))
                    {
                        message.Message = "Success";
                        message.IsSuccess = true;
                    }
                    else
                    {
                        message.Message = "User does not have a valid phone number. Please contact administrator.";
                        message.IsSuccess = false;
                    }
                }
                else
                {
                    message.Message = "Please enter valid username";
                    message.IsSuccess = false;
                }
            }
            return Json(new { Message = message.Message, IsSuccess = message.IsSuccess });
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult CheckIfUsernameExists(string Username)
        {
            var result = BlAccount.CheckIfUsernameExists(Username);
            if (result)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        public ActionResult AuthenticateUsername([System.Web.Http.FromBody] string username)
        {
            try
            {
                AuthenticationHandler authHandler = new AuthenticationHandler();
                if (username != null)
                {
                    DbUsers user = authHandler.GetAuthenticatedUserFromUsername(username);
                    if (user != null)
                    {
                        if (user.Verification_code != null && user.VerificationCodeSentDate != null)
                        {
                            DateTime codeTime = user.VerificationCodeSentDate.Value;
                            TimeSpan timeElapsed = TimeSpan.FromTicks(DateTime.Now.Ticks) - TimeSpan.FromTicks(codeTime.Ticks);
                            //if (timeElapsed.Days == 0 && timeElapsed.Hours <= 2 && timeElapsed.Minutes >= 30)
                            //{

                            //    VmUserSettings userSettings = new VmUserSettings();
                            //    userSettings.Username = username;
                            //    userSettings.Name = user.Name.Trim();
                            //    userSettings.FirstName = userSettings.Name.Split(new char[] { ' ' }).FirstOrDefault();
                            //    userSettings.LastName = userSettings.Name.Split(new char[] { ' ' }).LastOrDefault();
                            //    return PartialView("~/Views/Account/_ShowCnicForm.cshtml", userSettings);
                            //}
                        }
                        if (!String.IsNullOrWhiteSpace(user.Phone))
                        {
                            VmUserSettings userSettings = new VmUserSettings();
                            userSettings.Username = user.Username;
                            userSettings.Phone = BlUsers.MakeUserPhoneAsterik(user.Phone);
                            return PartialView("~/Views/Account/_ShowAuthenticatedPhone.cshtml", userSettings);
                        }
                        else
                        {
                            ViewBag.ReturnMessage = "User does not have a valid phone number. Please contact administrator.";
                            TempData["PhoneMessage"] = ViewBag.ReturnMessage.ToString();
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.ReturnMessage = "Please enter valid username.";
                        TempData["ErrorMessage"] = ViewBag.ReturnMessage.ToString();
                        return View();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ViewBag.ReturnMessage = "Please enter valid username.";
            TempData["ErrorMessage"] = ViewBag.ReturnMessage.ToString();
            return View();
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult AuthenticateCNICDetails([System.Web.Http.FromBody] string username, [System.Web.Http.FromBody] string lastname, [System.Web.Http.FromBody] string cnic, [System.Web.Http.FromBody] string phone)
        {
            AuthenticationHandler auth = new AuthenticationHandler();
            ClientMessage message = new ClientMessage("Failure", false);
            Boolean result = auth.AuthenticateUserWithCNIC(username.Trim(), cnic.Trim(), phone.Trim(), lastname.Trim());
            if (result)
            {

                message.Message = "Success";
                message.IsSuccess = true;

            }
            else
            {
                message.Message = "Authentication failed.";
                message.IsSuccess = false;
            }

            return Json(new { Message = message.Message, IsSuccess = message.IsSuccess });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult MoveToPasswordChangePage([System.Web.Http.FromBody] string username)
        {
            VmForgotPasswordChange userChangePassword = new VmForgotPasswordChange();
            userChangePassword.Username = username;
            return PartialView("~/Views/Account/_ForgotPasswordChange.cshtml", userChangePassword);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult SendVerificationCode([System.Web.Http.FromBody] string username)
        {
            AuthenticationHandler authHandler = new AuthenticationHandler();

            DbUsers user = authHandler.GetAuthenticatedUserFromUsername(username);
            VmUserSettings userSettings = new VmUserSettings();
            userSettings.Username = username;
            if (user != null)
            {
                int verificationCode = Utility.GetRandomNumber(6);
                BlUsers.UpdateVerificationCode(user.User_Id, verificationCode.ToString());
                BlUsers.SendVerificationCodeToUserPhone(user.User_Id, verificationCode.ToString());
                BlUsers.SendVerificationCodeToEmail(user.User_Id, verificationCode.ToString());
            }
            return PartialView("~/Views/Account/_EnterVerificationCode.cshtml", userSettings);
        }
        public JsonResult EnterVerificationCodeCheck(string username, string verificationCode)
        {
            ClientMessage message = new ClientMessage("Failure", false);
            AuthenticationHandler authHandler = new AuthenticationHandler();
            if (String.IsNullOrWhiteSpace(verificationCode))
            {
                message.Message = "Please enter code.";
                message.IsSuccess = false;
            }
            else
            {
                Boolean auth = authHandler.AuthenticateUserVerificationCode(username, verificationCode);
                if (auth)
                {
                    VmForgotPasswordChange userChangePassword = new VmForgotPasswordChange();
                    userChangePassword.Username = username;
                    message.Message = "Success";
                    message.IsSuccess = true;
                }
                else
                {
                    message.Message = "Entered code is incorrect.";
                    message.IsSuccess = false;
                }
            }

            return Json(new { Message = message.Message, IsSuccess = message.IsSuccess });
        }
        public ActionResult EnterVerificationCode(string username, string verificationCode)
        {
            AuthenticationHandler authHandler = new AuthenticationHandler();
            Boolean auth = authHandler.AuthenticateUserVerificationCode(username, verificationCode);
            if (auth)
            {
                VmForgotPasswordChange userChangePassword = new VmForgotPasswordChange();
                userChangePassword.Username = username;
                return PartialView("~/Views/Account/_ForgotPasswordChange.cshtml", userChangePassword);
            }
            return View();
        }
        public ActionResult OnForgotPasswordChange(VmForgotPasswordChange modelPassword)
        {
            dynamic showMessage = string.Empty;
            if (ModelState.IsValid && modelPassword.Password == modelPassword.ConfirmPassword)
            {

                showMessage = new
                {
                    param1 = 200,
                    param2 = BlUsers.ForgotPasswordChange(modelPassword).Message,
                };
                //return Json(showMessage, JsonRequestBehavior.AllowGet);
                return Json(showMessage, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Login", "Account", null);// Json(BlUsers.ForgotPasswordChange(modelPassword), JsonRequestBehavior.AllowGet);
            }
            else if (ModelState.IsValid == false && modelPassword.Password == modelPassword.ConfirmPassword)
            {
                showMessage = new
                {
                    param1 = 201,
                    param2 = "Password must be at least 8 characters."//"Confirm password does not match. Try again!!!"
                };
                return Json(showMessage, JsonRequestBehavior.AllowGet);
            }
            else
            {
                showMessage = new
                {
                    param1 = 201,
                    param2 = "Confirm password does not match. Try again!!!"
                };
                return Json(showMessage, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        [System.Web.Mvc.Route("~/campaign/{id?}")]
        public ActionResult LoginWithCampaign(string id)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            Config.Roles userRole;

            if (new AuthenticationHandler().GetAuthenticationFromCookie(out userRole))
            {
                //BlComplaints.AddComplaint(new VmAddComplaint(), false, false);

                var url = RouteHandler.GetUrl((byte)userRole);
                return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
            }
            else
            {
                VmLogin vmLogin = BlCampaign.GetLoginVmAgainstCampaignUrlSuffix(id);
                if (!string.IsNullOrEmpty(vmLogin.LayoutImageUrl))
                {
                    return View(null, BlView.Instance.SetMasterPageInCookie(), vmLogin);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
                return View(null, BlView.Instance.SetMasterPageInCookie(), vmLogin);
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(VmLogin userModel, FormCollection form, string returnUrl)
        {
            if (!this.IsCaptchaValid("Captcha is Invalid"))
            {
                ViewBag.ErrorMessage = "Validation Error";
                TempData["ErrorMessage"] = ViewBag.ErrorMessage.ToString();
                return Redirect(Request.UrlReferrer.ToString());
            }
            //if (string.IsNullOrWhiteSpace(form["g-recaptcha-response"])) {
            //     ViewBag.ErrorMessage = "Validation Error";
            //     TempData["ErrorMessage"] = ViewBag.ErrorMessage.ToString();
            //     //Throw error as required  
            // }
            // Config.CaptchaResponse response = Utility.ValidateCaptcha(Request["g-recaptcha-response"]);  
            // if (!(response.Success && ModelState.IsValid))
            // {
            //     ViewBag.ErrorMessage = "Validation Error";
            //     TempData["ErrorMessage"] = ViewBag.ErrorMessage.ToString();
            //     return Redirect(Request.UrlReferrer.ToString());
            // }



            AuthenticationHandler authHandler = new AuthenticationHandler();
            if (ModelState.IsValid)
            {
                //AuthenticationHandler authHandler = new AuthenticationHandler();
                if (authHandler.GetAuthenticationFromCredentials(userModel.Username, userModel.Password))
                {
                    var url = RouteHandler.GetUrl((byte)AuthenticationHandler.GetCookie().Role);
                    var cookie = AuthenticationHandler.GetCookie();
                    BlAccount.UpdateLastLoginInfoLogin();
                    return RedirectToAction(url.Action, url.Controller, url.ParamsDict);
                    //return RedirectToAction("Add", "Complaint");
                }
                else
                {
                    ViewBag.ErrorMessage = "Username or password is incorrect";
                    TempData["ErrorMessage"] = ViewBag.ErrorMessage.ToString();
                }
            }
            //return LoginWithCampaign("Awazekhalq");
            //return RedirectToAction(this.Request.UrlReferrer.AbsolutePath);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult SignOut()
        {
            //if (AuthenticationHandler.IsAlreadyLoggedIn(HttpContext))
            //{
            //    //new AuthHelper().LogUser(new AuthHelper().UserData().LoginAuditId, 0, 0, 0, string.Empty, string.Empty, string.Empty, DateTime.Now,
            //    //    DateTime.Now, false);

            //}

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
            //BlView.Instance.SetMasterPage(this);

            return View("~/Views/Account/Settings.cshtml", BlView.Instance.GetMasterPageFromCookie(), BlUsers.GetUserModel());
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult OnSettingSave(VmUserSettings model)
        {

            if (ModelState.IsValid)
            {
                return Json(BlUsers.UpdateProfile(model), JsonRequestBehavior.AllowGet);
            }
            return View("Settings", model);
        }

        public ActionResult Roles()
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            if (cmsCookie == null) BlView.Instance.SetMasterPageInCookie(null, "~/Views/Shared/_StakeholderLayout.cshtml");
            else

            if (cmsCookie.Role == Config.Roles.Stakeholder)
            {
                if (cmsCookie.Campaigns == Convert.ToInt32(Config.Campaign.SchoolEducationEnhanced).ToString())
                {
                    if (cmsCookie.SubRoles == Config.SubRoles.SDU)
                    {
                        //BlView.Instance.GetMasterPageFromCookie();
                        BlView.Instance.SetMasterPageInCookie(null, "~/Views/Shared/SchoolEducation/_SduSchoolEducationStakeholderLayout.cshtml");
                    }
                    else
                    {
                        //BlView.Instance.GetMasterPageFromCookie();
                        BlView.Instance.SetMasterPageInCookie(null, "~/Views/Shared/SchoolEducation/_SchoolEducationStakeholderLayout.cshtml");
                    }
                }
                else
                {
                    //BlView.Instance.GetMasterPageFromCookie();
                    BlView.Instance.SetMasterPageInCookie(null, "~/Views/Shared/_StakeholderLayout.cshtml");
                }
            }
            return View("RoleSetting", BlView.Instance.GetMasterPageFromCookie(), BlUsers.GetRole(new AuthenticationHandler().CmsCookie.UserId));
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult RoleSave(VmRole vmRole)
        {
            //DbUsers dbUser = DbUsers.GetUser(vmRole.ListRoleEntries.Where(n => n.IsSelected).FirstOrDefault().Id);
            DbUsers dbUser = DbUsers.GetUser(vmRole.SelectedValue.Id);
            CMSCookie cookie = CMSCookie.DbUserToCookie(dbUser);
            BlView.Instance.SetMasterPageInCookie(cookie);
            new AuthenticationHandler().SaveCookie(cookie);

            vmRole.Message = "Role changed to " + vmRole.SelectedValue.Value;
            vmRole.Title = "Success";
            return Json(vmRole, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult SwitchUser(VmCampaignWiseData data)
        {
            //DbUsers dbUser = DbUsers.GetUser(vmRole.ListRoleEntries.Where(n => n.IsSelected).FirstOrDefault().Id);
            CMSCookie prevCookie = AuthenticationHandler.GetCookie();
            //int currUserId = prevCookie.UserId;
            string permissionVal = prevCookie.ListPermissions.Where(
                n => n.Permission_Id == (int)Config.Permissions.ExecutiveCampaignWiseNavigation).FirstOrDefault().Permission_Value;
            Dictionary<string, string> dict = Utility.ConvertCollonFormatToDict(permissionVal);
            DbUsers dbUser = DbUsers.GetActiveUser(Convert.ToInt32(dict[data.CampaignId.ToString()])/*31735*/);


            CMSCookie cmsCookie = CMSCookie.DbUserToCookie(dbUser);
            //cmsCookie.navigationalData = prevCookie.navigationalData;
            //cmsCookie.PreviousLoginId = (int?)currUserId;
            cmsCookie.UrlConfig = "Controller::Complaint__Action::StakeholderComplaintsListingLowerHierarchyServerSide";


            if (prevCookie.Role == Config.Roles.Executive)
            {
                prevCookie.navigationalData = prevCookie.navigationalData == null ? new ExpandoObject() : prevCookie.navigationalData;
                prevCookie.navigationalData.topBarHeading = "Chief Minister’s Complaint Center";
            }
            // Rename DC with revenue
            else if (prevCookie.Role == Config.Roles.ExecutiveCampaigns)
            {
                if (dbUser.Campaign_Id == (int)Config.Campaign.DcoOffice)
                {
                    prevCookie.navigationalData = prevCookie.navigationalData == null ? new ExpandoObject() : prevCookie.navigationalData;
                    prevCookie.navigationalData.campaignName = "Revenue";
                }
                prevCookie.navigationalData.topBarHeading = "Centralized Complaint Center (CCC)";
                //new AuthenticationHandler().SaveCookie(cmsCookie);
            }
            else
            {
                prevCookie.navigationalData = null;
            }




            //new AuthenticationHandler().LogOut();
            //new AuthenticationHandler().SaveCookie(cmsCookie);
            //vmRole.Message = "Role changed to " + vmRole.SelectedValue.Value;
            //vmRole.Title = "Success";

            BlView.Instance.UpdateUserLogin(prevCookie.UserId, prevCookie, cmsCookie);
            return Json("Success", JsonRequestBehavior.AllowGet);
            //return Login();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SwitchUserWithUserId([FromBody] int userId)
        {
            //DbUsers dbUser = DbUsers.GetUser(vmRole.ListRoleEntries.Where(n => n.IsSelected).FirstOrDefault().Id);
            CMSCookie prevCookie = AuthenticationHandler.GetCookie();
            DbUsers dbUser = DbUsers.GetActiveUser(userId)/*31735*/;


            CMSCookie cmsCookie = CMSCookie.DbUserToCookie(dbUser);
            //cmsCookie.navigationalData = prevCookie.navigationalData;
            //cmsCookie.PreviousLoginId = cmsCookie.UserId;
            cmsCookie = CMSCookie.DbUserToCookie(dbUser);
            cmsCookie.UrlConfig = null;
            BlView.Instance.UpdateUserLogin(prevCookie.UserId, prevCookie, cmsCookie);

            //new AuthenticationHandler().SaveCookie(cmsCookie);
            //vmRole.Message = "Role changed to " + vmRole.SelectedValue.Value;
            //vmRole.Title = "Success";
            return Json("Success", JsonRequestBehavior.AllowGet);
            //return Login();
        }

        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword", new VmChangePassword());
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult OnChangePassword(VmChangePassword modelPassword)
        {
            if (ModelState.IsValid)
            {
                return Json(BlUsers.ChangePassword(modelPassword), JsonRequestBehavior.AllowGet);
            }
            return View("Settings", modelPassword);
        }

    }
}
