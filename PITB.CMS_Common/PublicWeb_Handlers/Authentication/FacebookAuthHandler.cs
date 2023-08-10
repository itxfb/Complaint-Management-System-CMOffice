using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Facebook;
using Microsoft.Ajax.Utilities;
using PITB.CMS_Common.Models.Public_Web;
using PITB.CMS_Common.Models.Public_Web.Social;

namespace PITB.CMS_Common.PublicWeb_Handlers.Authentication
{
    public class FacebookAuthHandler : AuthHandler, IAuthHandler
    {

        public bool IsAuthorized()
        {
            bool isAuthenticated = false;

            ClaimsIdentity identity = GetClaimOfLoggedInUser();

            // await (base.GetAuthenticationCookie(httpContext));


            if (identity != null)
                if (identity.IsAuthenticated)
                {

                    string accessToken = identity.FindFirst(Config.FacebookAccessTokenKeyName).Value;
                    try
                    {
                        if (!string.IsNullOrEmpty(accessToken))
                            if (IsUserHasPermissions(accessToken))
                            {
                                dynamic userInfo = new FacebookClient(accessToken).Get(GraphApi.Profile);
                                if (userInfo != null)
                                {
                                    isAuthenticated = true;
                                }
                            }

                    }
                    catch (Exception)
                    {


                    }
                }



            return isAuthenticated;
        }

        public string GetAccessToken()
        {
            ClaimsIdentity identity = GetClaimOfLoggedInUser();
            return identity.FindFirst(Config.FacebookAccessTokenKeyName).Value;
        }

        private static bool IsUserHasPermissions(string accessTokenOfUser)
        {
            dynamic userInfo = new FacebookClient(accessTokenOfUser).Get(GraphApi.Permission);
            if (userInfo != null)
            {

                return (userInfo[0][0][1] == "granted");

            }
            return false;
        }

        public CookieUserModel GetLoggedInUserData()
        {
            CookieUserModel cookie= new CookieUserModel();
            ClaimsIdentity identity = GetClaimOfLoggedInUser();
            cookie.Data.FirstName = identity.FindFirst(Config.FacebookFirstName).Value;
            cookie.Data.LastName= identity.FindFirst(Config.FacebookLastName).Value;
            cookie.Data.UserId = ((identity.FindFirst(Config.FacebookUserId).Value));
            cookie.Data.Provider = "Facebook";

            return cookie;

        }
        private ClaimsIdentity GetClaimOfLoggedInUser()
        {
            var httpContextBase = new HttpContextWrapper(HttpContext.Current);
            return Task.Run(async () => await GetAuthenticationCookie(httpContextBase)).Result;

        }



    }
}