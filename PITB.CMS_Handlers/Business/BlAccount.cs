using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Handlers.Business
{
    public class BlAccount
    {
        public static void UpdateLastLoginInfoLogin()
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            DbUsers.UpdateLastLoginDate(cmsCookie.UserId);
        }

        public static void UpdateLastOpenedInfo()
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            DbUsers.UpdateLastOpenedDate(cmsCookie.UserId);
        }

        public static void UpdateSignOutInfo()
        {
            CMSCookie cmsCookie = AuthenticationHandler.GetCookie();
            DbUsers.UpdateSignOutDate(cmsCookie.UserId);
        }
        public static bool CheckIfUsernameExists(string username)
        {
            return DbUsers.CheckIfUsernameExists(username);
        }
    }
}