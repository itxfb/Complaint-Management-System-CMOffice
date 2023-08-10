using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace PITB.CRM.Public_Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            return new ChallengeResult("Facebook",
                    Url.Action("ExternalLoginCallback",  new { ReturnUrl = returnUrl }));
        }

        public ActionResult Signout()
        {
            Request.GetOwinContext()
                   .Authentication
                   .SignOut(HttpContext.GetOwinContext()
                                       .Authentication.GetAuthenticationTypes()
                                       .Select(o => o.AuthenticationType).ToArray());

            return RedirectToAction("index", "public");
        }
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            return new RedirectResult(returnUrl);
        }
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}