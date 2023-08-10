using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PITB.CRM.Public_Web.Models;

namespace PITB.CRM.Public_Web.Contacts
{
    public interface IAuthHandler
    {
        bool IsAuthorized();
        string GetAccessToken();
        CookieUserModel GetLoggedInUserData();



    }
}