using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models;

namespace PITB.CMS_Handlers.Authentication
{
    public class AuthenticationHandler
    {
        private static readonly string StateIdentifier = ConfigurationManager.AppSettings["StateManagerIdentifier"]+"_U_";

        private const string UserCookie = "User";

        private const int MaxCookieSize = 4000;//4096;

        private const int MaxUserCookieCount = 15;

        public static CMSCookie GetCookie()
        {
            AuthenticationHandler auth = new AuthenticationHandler();
            return auth.CmsCookie;
        }

        public  CMSCookie CmsCookie
        { 
            get {

                CMSCookie userInfoFromCookie = GetUserDbModelFromCookie();
                if (userInfoFromCookie != null)
                {
                    //CMSCookie cmsCookie = CMSCookie.DBUserToCookie(userFromCookie);
                    /*
                    CMSCookie cmsCookie = new CMSCookie();
                    cmsCookie.UserID = userFromCookie.User_Id;
                    cmsCookie.UserName = userFromCookie.Username;
                    cmsCookie.ProvinceId = userFromCookie.Province_Id;
                    cmsCookie.DistrictId = userFromCookie.District_Id;
                    cmsCookie.DivisionId = userFromCookie.Division_Id;
                    cmsCookie.TehsilId = userFromCookie.Tehsil_Id;
                    cmsCookie.UCId = userFromCookie.UnionCouncil_Id;
                    cmsCookie.Name = userFromCookie.Name;
                    cmsCookie.ListPermissions = userFromCookie.ListUserPermissions;
                    */
                    return userInfoFromCookie;
                    //return new CMSCookie(userFromCookie.User_Id, userFromCookie.Username, (int)userFromCookie.Province_Id, (int)userFromCookie.District_Id);
                }
                return null;
            } 
        }


        

        public  bool GetAuthenticationFromCredentials(string userName, string password)
        {
           // DataAccessHandler da = new DataAccessHandler();
            DbUsers user = DbUsers.GetUserAgainstUserNameAndPassword(userName, password);
            if (user == null) return false;
            SaveCookie(user);
            //StoreUserInfoInCookie(CMSCookie.DbUserToCookie(user));
            return true;
        }
        public DbUsers GetAuthenticatedUserFromUsername(string userName)
        {
            // DataAccessHandler da = new DataAccessHandler();
            DbUsers user = DbUsers.GetUserAgainstUserNameForForgotPassword(userName);
            return user;
        }
        public bool AuthenticateUserWithCNIC(string username, string cnic, string phone, string lastname)
        {
            Boolean auth = DbUsers.AuthenticateUserWithCNIC(username, cnic, phone, lastname);            
            return auth;
        }
        public Boolean AuthenticateUserVerificationCode(string username,string verification_code)
        {
            // DataAccessHandler da = new DataAccessHandler();
            Boolean auth = DbUsers.AuthenticateUserVerificationCode(username,verification_code);
            return auth;
        }
        public void SaveCookie(DbUsers dbUser)
        {
            StoreUserInfoInCookie(CMSCookie.DbUserToCookie(dbUser));
        }

        public void SaveCookie(CMSCookie cmsCookie)
        {
            StoreUserInfoInCookie(cmsCookie);
        }

        public void LogOut()
        {
            for (int i = 0; i < MaxUserCookieCount; i++)
            {
                HttpCookie userCookie = new HttpCookie(StateIdentifier+i) {Expires = DateTime.Now.AddDays(-8)};
                if (userCookie != null)
                {
                    HttpContext.Current.Response.Cookies.Add(userCookie);
                }
            }
            
        }

        /// <summary>
        /// Retrieve cookie from current request and check its existence
        /// </summary>
        /// <param name="httpContext">Current request</param>
        /// <returns>Returns True if it exists</returns>
        public static bool IsAlreadyLoggedIn(HttpContextBase httpContext)
        {
            var cookie = HttpContext.Current.Request.Cookies[StateIdentifier + '0'];
            if (cookie != null)
            {
             
                    return true;
            }
            return false;
        }
        public bool GetAuthenticationFromCookie(out Config.Roles userRole)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[StateIdentifier + '0'];
            CMSCookie userFromCookie=new CMSCookie();
            if (cookie != null && cookie[UserCookie]!=null)
            {
                userFromCookie = GetUserDbModelFromCookie();
                //DbUsers userFromDB = DbUserHandler.GetUserAgainstUserNameAndPassword(userFromCookie.user, userFromCookie.Password);
                //if (cookie != null)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                userRole = userFromCookie.Role;
                return true;
            }
            userRole = userFromCookie.Role;
            return false;
            //User user = da.GetUserAgainstUserNameAndPassword(userName, password);
        }
        /// <summary>
        /// Expire users cookie
        /// </summary>
       
        /*private CMSCookie GetUserDbModelFromCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[StateIdentifier];

            if (cookie != null && cookie[UserCookie] != null)
            {
                string userStr = ModelToJsonString(cookie[UserCookie]);
                userStr = GetOrignalCookieString(userStr);
                
                return JsonStringToUserModel(GetDecryptedString(userStr));
            }
            return null;
        }*/

        private CMSCookie GetUserDbModelFromCookie()
        {
            bool hasFound = true;
            int noOfCookiesFound = -1;
            string userStr = "";
            string orignalCookie = "";

            for (int i = 0; i < MaxUserCookieCount && hasFound ; i++)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[StateIdentifier+i];

                if (cookie != null && cookie[UserCookie] != null)
                {
                    orignalCookie = ModelToJsonString(cookie[UserCookie]);
                    orignalCookie = GetOrignalCookieString(orignalCookie);
                    userStr = userStr + orignalCookie;
                }
                else
                {
                    hasFound = false;
                }
                noOfCookiesFound = i;
            }
            if (noOfCookiesFound != -1) // if cookie is found
            {
                //userStr = GetOrignalCookieString(userStr);

                return JsonStringToUserModel(Utility.GetDecryptedString(userStr));
            }
            return null;
        }

        internal void StoreUserInfoInCookie(CMSCookie user)
        {

            try
            {
                LogOut();
                //HttpCookie userCookie = new HttpCookie(StateIdentifier);
                //userCookie.Expires = DateTime.Now.AddDays(7);
                string userCookieStr = Utility.GetEncryptedString(ModelToJsonString(user));

                int from, length;
                //int count = 0;
                string userSubStr = "";
                int noOfUserCookies = userCookieStr.Length/MaxCookieSize;
                
                for (int i = 0; i <= noOfUserCookies; i++)
                {
                    HttpCookie userCookie = new HttpCookie(StateIdentifier + i);
                    userCookie.Expires = DateTime.Now.AddDays(7);
                    from = MaxCookieSize*i;
                    //to = MaxCookieSize*(i + 1) <= userCookieStr.Length
                    //    ? MaxCookieSize*(i + 1)
                    //    : userCookieStr.Length - (MaxCookieSize*(i));
                    length = (from + MaxCookieSize) > userCookieStr.Length
                        ? userCookieStr.Length%MaxCookieSize
                        : MaxCookieSize;
                    userSubStr = userCookieStr.Substring(from, length);
                    userCookie.Values.Add(UserCookie, userSubStr);
                    //count = count + MaxCookieSize;

                    HttpContext.Current.Response.Cookies.Remove(StateIdentifier + i);
                    HttpContext.Current.Response.Cookies.Add(userCookie);
                }



                //for (int i = noOfUserCookies+1; i < MaxUserCookieCount; i++)
                //{
                //    HttpCookie userCookie = new HttpCookie(StateIdentifier + i) { Expires = DateTime.Now.AddDays(-8) };
                //    if (userCookie != null)
                //    {
                //        HttpContext.Current.Response.Cookies.Add(userCookie);
                //    }
                //}



                //userCookie.Values.Add(UserCookie, userCookieStr);   //GetEncryptedString(ModelToJsonString(user)

                //HttpContext.Current.Response.Cookies.Add(userCookie);
            }
            catch (Exception ex)
            {
                
            }
        }

        private string ModelToJsonString(object obj)
        {
            //var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return JsonConvert.SerializeObject(obj);
        }

        private CMSCookie JsonStringToUserModel(string str)
        {
            return JsonConvert.DeserializeObject<CMSCookie>(str);
        }

        

        private string GetOrignalCookieString(string str)
        {
            str = str.Remove(str.Length - 1, 1);
            str = str.Remove(0, 1);
            str = str.Replace("\\", "");
            return str;
        }

       

        
    }
}