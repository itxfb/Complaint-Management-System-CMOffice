using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PITB.CMS_Common.Helper.Attributes
{
    #region antiforgerytoken
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryToken : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            if (request.HttpMethod == WebRequestMethods.Http.Post)
            {
                if (request.IsAjaxRequest())
                    AntiForgery.Validate(CookieValue(request), request.Headers["__RequestVerificationToken"]);
                else
                    new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
            }
        }

        private string CookieValue(HttpRequestBase request)
        {
            var cookie = request.Cookies[AntiForgeryConfig.CookieName];
            return cookie?.Value;
            //return cookie != null ? cookie.Value : null;
        }
    }
    #endregion


    public class AjaxRequestOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}
