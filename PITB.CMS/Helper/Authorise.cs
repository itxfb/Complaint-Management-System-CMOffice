using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Handler.Authentication
{
  
        public sealed class Authorise : AuthorizeAttribute
        {
            /// <summary>
            /// Validates user for controller/action and check its role for running action
            /// </summary>
            /// <param name="httpContext">HttpRequest from client</param>
            /// <returns>True if users role matches</returns>
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                if (!AuthenticationHandler.IsAlreadyLoggedIn(httpContext))
                    return false;

                CMSCookie data = null;
                //Retrieve user data from cookies
                try
                {
                    data = new AuthenticationHandler().CmsCookie;
                }
                catch (Exception)
                {

                    return false;
                }

                //For comparing user role name
                //First extracting from cookies and converting to String for role verfication
                var role = Convert.ToString(data.Role);
                //If role collection contains current users role than it will allow user to continue            
                return Roles.Split(',').Any(definedRole => definedRole.Equals(role));
            }
            /// <summary>
            /// Default Constructor for Authorising multi roles for an Action or Controller
            /// </summary>
            /// <param name="roles">List of multiple user roles</param>
            public Authorise(params Config.Roles[] roles)
            {
                if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                    throw new ArgumentException("roles");
                //Joining comma seprated roles for AuthorizeCore in Roles collection
                Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
            }
            /// <summary>
            /// When AutorizeCore rejects the request then it will redirect it to Login page
            /// </summary>
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
               // filterContext.Result = new RedirectResult("~/"); 
                
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
              //  base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
