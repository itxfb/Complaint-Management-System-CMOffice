using PITB.CMS_Common;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PITB.CMS_Common.Handler.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermission : AuthorizeAttribute
    {
        private static int LoggedInUnauthCount = 0;
        private CMSCookie cookie;
        private int ignoreAuthStatus;
        public AuthorizePermission()
        {

        }
        public AuthorizePermission(int ignoreAuth)
        {
            this.ignoreAuthStatus = ignoreAuth;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!AuthenticationHandler.IsAlreadyLoggedIn(httpContext))
                return false;

            cookie = null;
            //Retrieve user data from cookies
            try
            {
                cookie = new AuthenticationHandler().CmsCookie;
                return GetAuthorizeResult(httpContext, cookie);
            }
            catch (Exception)
            {
                return false;
            }

        }

        private bool GetAuthorizeResult(HttpContextBase httpContext, CMSCookie cookie)
        {
            if (ignoreAuthStatus == 1) // completely ignore auth
            {
                return true;
            }

            if (cookie == null)
            {
                return false;
            }

            if (ignoreAuthStatus == 2) // ignore auth if cookie exists
            {
                return true;
            }

            HttpRequest request = HttpContext.Current.Request;
            bool isAjaxReq = false;
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            isAjaxReq = (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));

            if (isAjaxReq)
            {
                return true;
            }

            //Uri theRealURL = new Uri(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.RawUrl);

            //string yourValue = HttpUtility.ParseQueryString(theRealURL.Query).Get("yourParm");

            //bool isAuthorized = false;
            //var rd = httpContext.Request.RequestContext.RouteData;
            //string currentAction = rd.GetRequiredString("action");
            //string currentController = rd.GetRequiredString("controller");
            //string currentArea = rd.Values["area"] as string;
            //string strToMatch = string.Format("{0}/{1}?{2}", currentController, currentAction, currentArea);
            string valToMatch = HttpContext.Current.Request.RawUrl.Substring(1, HttpContext.Current.Request.RawUrl.Length - 1);//.Split('?')[0];
            //string valToMatch = "";
            Dictionary<string, string> toMatchParamsDict = Utility.GetDictParamsFromUrl(valToMatch);
            foreach (KeyValuePair<string, string> srcKeyVal in cookie.DictCtrlAction)
            {
                if (srcKeyVal.Value.Contains("{*}"))
                {
                    List<string> listStr = srcKeyVal.Value.Split(new[] { "{*}" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (listStr.Count > 0) // if splitted list exists
                    {
                        string concatStr = "";
                        int matchedCount = 0;
                        foreach (string str in listStr)
                        {
                            if (valToMatch.Contains(str))
                            {
                                concatStr = concatStr + str;
                                matchedCount++;
                            }
                        }
                        if (matchedCount == listStr.Count)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                if (valToMatch.Split('?')[0] == srcKeyVal.Value.Split('?')[0]) //if controller/action is equal
                {
                    Dictionary<string, string> srcParamsDict = Utility.GetDictParamsFromUrl(srcKeyVal.Value);
                    if (srcParamsDict.Count == 0 && toMatchParamsDict.Count == 0) // if action/controller matched and url params donot exist
                    {
                        return true;
                    }
                    else if (srcParamsDict.Count == toMatchParamsDict.Count) // if action/controller matched and url param exist
                    {
                        foreach (KeyValuePair<string, string> srcParamKeyVal in srcParamsDict)
                        {
                            if (toMatchParamsDict.Where(n => n.Key == srcParamKeyVal.Key && (n.Value == srcParamKeyVal.Value || srcParamKeyVal.Value == "{n}")).FirstOrDefault().Key != null)
                            {
                                return true;
                            }
                        }
                    }
                }
                //string[] srcValArr = keyVal.Value.Split('?');
                //if (srcValArr[0] == valToMatch)
                //{
                //    if(srcValArr.Length>0)
                //    {
                //        string[] paramsSplitArr = srcValArr[1].Split('&');
                //        Dictionary<string, string> dict = new Dictionary<string, string>();
                //        foreach (string stringParam in paramsSplitArr)
                //        {
                //            string[] valParam = stringParam.Split('=');
                //            dict.Add(valParam[0], valParam[1]);
                //        }
                //    }
                //}
            }
            //KeyValuePair<string,string> keyValCtrlAction = cookie.DictCtrlAction.Where(n => n.Value == valToMatch).FirstOrDefault();
            ////if(keyValCtrlAction.)
            //return (keyValCtrlAction.Key!=null);
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            
            string valToMatch = HttpContext.Current.Request.RawUrl.Substring(1, HttpContext.Current.Request.RawUrl.Length - 1);//.Split('?')[0];
            if (cookie == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
            //else if (cookie.DictCtrlAction == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignOut" }));
            //}
            else
            {
                
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "CommonView", action = "GetErrorPage" }));
            }
            //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            //filterContext.Result = new RedirectResult("~/Account/login");
            //filterContext.Result = new RedirectResult("~/CommonView/GetErrorPage");
            //base.HandleUnauthorizedRequest(filterContext);
        }
    }
}