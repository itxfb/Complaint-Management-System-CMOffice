using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using PITB.CRM.Public_Web.Handler;
using PITB.CMS_Common;

namespace PITB.CRM.Public_Web
{
    public partial class Startup
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        private void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(Config.AuthenticationType);
            var cookieOptions = new CookieAuthenticationOptions
            {

                LoginPath = new PathString("/Account/Login"),
                AuthenticationMode = AuthenticationMode.Passive,
                AuthenticationType = Config.AuthenticationType,
                CookieName = Config.ExternalCookieName,

            };
            app.UseCookieAuthentication(cookieOptions);
            var fbOptions = new FacebookAuthenticationOptions()
            {
                //AppId = "437782676566659",
                AppId = "471225320052144",
                //AppSecret = "eecb4ec1b33b55893b1216e74db2b293",
                AppSecret = "164d10c891775a4c1d5e07d04b08cb2f",
                
                BackchannelHttpHandler = new FacebookBackChannelHandler(),
                UserInformationEndpoint = "https://graph.facebook.com/v2.8/me?fields=id,name,email,first_name,last_name",
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        //  context.Identity.AddClaim(new Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));
                      //  context.Identity.AddClaim(new Claim("urn:facebook:access_token", context.AccessToken));
                        context.Identity.AddClaim(new Claim(Config.FacebookAccessTokenKeyName, context.AccessToken,XmlSchemaString,Config.ExternalProvider.Facebook.ToString()));

                        foreach (var claim in context.User)
                        {
                            var claimType = string.Format("urn:facebook:{0}", claim.Key);
                            string claimValue = claim.Value.ToString();
                            if (!context.Identity.HasClaim(claimType, claimValue))
                                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, XmlSchemaString, "Facebook"));

                        }
                        return Task.FromResult(0);
                    }
                },
            };
            app.UseFacebookAuthentication(fbOptions);
        }
    }
}