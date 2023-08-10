using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Facebook;
using PITB.CMS_Common;
using PITB.CRM.Public_Web.Contacts;
using PITB.CRM.Public_Web.Models.Social;

namespace PITB.CRM.Public_Web.Handler.Authentication
{
    public abstract class AuthHandler 
    {
        public async Task<ClaimsIdentity> GetAuthenticationCookie(HttpContextBase httpContext)
        {
            var authManager = httpContext.GetOwinContext().Authentication;
            var a = authManager.AuthenticateAsync(Config.AuthenticationType);
            var authResult = await authManager.AuthenticateAsync(Config.AuthenticationType);
            if(authResult!=null)
            if (authResult.Identity.IsAuthenticated)
            {
                return authResult.Identity;
            }
            return null;
        }


    }
}