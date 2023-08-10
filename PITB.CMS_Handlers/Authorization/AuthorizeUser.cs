using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http;
using System.Web.Mvc;
using PITB.CMS_Common.Handler.Authentication;

namespace PITB.CMS_Handlers.Authorization
{
   // [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,AllowMultiple = true)]
    public class AuthorizeUser : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (AuthenticationHandler.IsAlreadyLoggedIn(httpContext))
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new  RedirectResult("~/Account/login");
            //filterContext.Result = new RedirectResult("~/Views/Account/Login.cshtml");
            base.HandleUnauthorizedRequest(filterContext);
        }

    }
}