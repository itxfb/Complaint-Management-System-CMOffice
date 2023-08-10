using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using PITB.CMS.Enums;
using PITB.CMS.Handler.Authentication;
using PITB.CMS.Handler.Messages;
using PITB.CMS.Helper.Extensions;
using PITB.CMS.Models.Custom;
using PITB.CMS.Models.DB;
using PITB.CMS.Models.View;
using PITB.CMS.Models.View.Account;
using PITB.CMS.Models.View.ClientMesages;
using System.Threading;

namespace PITB.CMS.Handler.Business
{
    public class BlUsers
    {
        public static VmUserSettings GetUserModel()
        {
            VmUserSettings userSettings = new VmUserSettings();
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            var user = DbUsers.GetActiveUser(userCookie.UserId);
            var campaignList = DbCampaign.GetCampaignNameByUserId(userCookie.UserId, Utility.GetIntList(userCookie.Campaigns));
            if (user != null)
            {
                userSettings.Username = user.Username;
                userSettings.Name = user.Name;
                userSettings.Phone = user.Phone;
                userSettings.Email = user.Email;
                userSettings.CampaignName = campaignList;
                userSettings.PasswordUpdateDate = user.Password_Updated;
                userSettings.Role = user.Role_Id;

            }
            return userSettings;
        }
     
        public static Boolean SendVerificationCodeToUserPhone(int User_Id,string Verification_Code)
        {
            var user = DbUsers.GetActiveUser(User_Id);
            if (user != null && !String.IsNullOrWhiteSpace(user.Phone))
            {
                string messageStr = "SED: " + Verification_Code + " - use this verification code to change your password.";
                SmsModel smsModel = null;
                smsModel = new SmsModel((int)CMS.Config.Campaign.SchoolEducationEnhanced, user.Phone, messageStr,
                (int)Config.MsgType.ToStakeholder,
                (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholder);
                new Thread(delegate()
                {
                    TextMessageHandler.SendMessageToPhoneNo(user.Phone, messageStr, true, smsModel);

                }).Start();
                return true;
            }
            return false;
        }
        public static string MakeUserPhoneAsterik(string phoneNumber)
        {
            //phoneNumber = "03338334262";
            string phone = phoneNumber.Trim();
            int length = phone.Length;

            int firts30percentIndex = (int)Math.Ceiling((double)(length / 2.8));
            int last20percentIndex = length - (int)Math.Floor((double)(length / 5));
            //phone = phone.Substring(firts30percentIndex, last20percentIndex - firts30percentIndex).PadLeft(last20percentIndex, '*').PadRight(length, '*');
            phone = phone.Substring(0, firts30percentIndex).PadRight(last20percentIndex, '*') + phone.Substring(last20percentIndex, length - last20percentIndex);
            return phone;
        }
        public static bool IsCookieHaveMultipleRoles()
        {
            VmRole vmRole = BlUsers.GetRole(new AuthenticationHandler().CmsCookie.UserId);
            if (vmRole.ListRoleEntries == null || vmRole.ListRoleEntries.Count <= 1)
            {
                return false;
            }
            return true;
        }

        public static VmRole GetRole(int userId)
        {
            DbUsers dbUser = DbUsers.GetUser(userId);
            List<DbUsers> listDbUser = DbUsers.GetUsers(dbUser.Cnic);
            if (listDbUser.Count==0)
            {
                listDbUser = new List<DbUsers>() {DbUsers.GetActiveUser(userId)};
            }
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            VmRole vmRole = new VmRole(listDbUser,userCookie.UserId);
           
           
            //vmRole.SelectedId = 10295;
            return vmRole;
        }

        public static ClientMessage ChangePassword(VmChangePassword modelVmChangePassword)
        {
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            //Reading user Id from Cookies 
            Config.CommandStatus opResult = DbUsers.ChangePassword(
                    userCookie.UserId,
                    modelVmChangePassword.OldPassword,
                    modelVmChangePassword.Password);
            if (opResult == Config.CommandStatus.Success)
            {
                return ClientsideMessageHandler.GetMessage(MessageCatalog.Task.ChangePassword, opResult);
            }
            else
            {
                return new ClientMessage("Old password is incorrect",false);
            }
        }
        public static ClientMessage ForgotPasswordChange(VmForgotPasswordChange modelVmChangePassword)
        {

            Config.CommandStatus opResult = DbUsers.ForgotPasswordChange(
                    modelVmChangePassword.Username,
                    modelVmChangePassword.Password);
            if (opResult == Config.CommandStatus.Success)
            {
                return new ClientMessage("Password is successfully changed.", true);
            }
            else
            {
                return new ClientMessage("Old password is incorrect", false);
            }
        }
        public static ClientMessage UpdateProfile(VmUserSettings userSettings)
        {
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            //Reading user Id from Cookies 
            var opResult = DbUsers.UpdateProfile(
                    userCookie.UserId,
                    userSettings.Name,
                    userSettings.Phone,
                    userSettings.Email);
            var cookie = CMSCookie.DbUserToCookie(DbUsers.GetActiveUser(userCookie.UserId));
            new AuthenticationHandler().StoreUserInfoInCookie(cookie);

            return ClientsideMessageHandler.GetMessage(MessageCatalog.Task.UserSettings, opResult);
        }
        public static ClientMessage UpdateVerificationCode(int userId, string verification_code)
        {
            var opResult = DbUsers.UpdateVerificaionCode(
                userId: userId, verification_code: verification_code);
            return ClientsideMessageHandler.GetMessage(MessageCatalog.Task.UserVerificationCode, opResult);
        }
        public static VmResolverDetail GetResolverDetail(int resolverId)
        {
            return new VmResolverDetail(resolverId);
        }
    }
}