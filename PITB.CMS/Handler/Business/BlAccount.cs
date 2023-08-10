using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Helper.Database;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;

namespace PITB.CMS.Handler.Business
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