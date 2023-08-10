using System;
using System.Collections.Generic;
using PITB.CMS_Common.Enums;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.Messages;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.View;
using PITB.CMS_Common.Models.View.Account;
using PITB.CMS_Common.Models.View.ClientMesages;
using System.Threading;
using PITB.CMS_Common.Models;
using System.Net.Mail;
using System.Net;

namespace PITB.CMS_Common.Handler.Business
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

        public static Boolean SendVerificationCodeToUserPhone(int User_Id, string Verification_Code)
        {
            var user = DbUsers.GetActiveUser(User_Id);
            if (user != null && !String.IsNullOrWhiteSpace(user.Phone))
            {
                string messageStr = "SED: " + Verification_Code + " - use this verification code to change your password.";
                SmsModel smsModel = null;
                smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, user.Phone, messageStr,
                (int)Config.MsgType.ToStakeholder,
                (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholder);
                new Thread(delegate ()
                {
                    TextMessageHandler.SendMessageToPhoneNo(user.Phone, messageStr, true, smsModel);

                }).Start();
                return true;
            }
            return false;
        }

        public static bool SendVerificationCodeToEmail(int User_Id, string Verification_Code)
        {
            try
            {
                //return false;
                var user = DbUsers.GetActiveUser(User_Id);
                if (user != null && !String.IsNullOrWhiteSpace(user.Email))
                {
                    string messageStr = "SED: " + Verification_Code + " - use this verification code to change your password.";

                    // Start Send Email Code
                    return SendEmail(user.Email.Trim(), messageStr);
                    // End send email code
                }
            }
            catch (Exception ex)
            {

            }



            //var user = DbUsers.GetActiveUser(User_Id);
            //if (user != null && !String.IsNullOrWhiteSpace(user.Phone))
            //{
            //    string messageStr = "SED: " + Verification_Code + " - use this verification code to change your password.";
            //    SmsModel smsModel = null;
            //    smsModel = new SmsModel((int)Config.Campaign.SchoolEducationEnhanced, user.Phone, messageStr,
            //    (int)Config.MsgType.ToStakeholder,
            //    (int)Config.MsgSrcType.Web, DateTime.Now, 1, (int)Config.MsgTags.SchoolEducationStakeholder);
            //    new Thread(delegate ()
            //    {
            //        TextMessageHandler.SendMessageToPhoneNo(user.Phone, messageStr, true, smsModel);

            //    }).Start();
            //    return true;
            //}
            return false;
        }

        public static bool SendPublicUserOtpOnMobileAndEmail(string email, string contact, string verificationCode)
        {

            try
            {
                string messageStr = "Signup OTP: " + verificationCode + " - use this verification code to complete signup.";


                new Thread(delegate ()
                {
                    TextMessageHandler.SendMessageToPhoneNo(contact, messageStr);

                }).Start();


            }
            catch (Exception e)
            {

            }
            try
            {
                var result = AuthenticationHandler.IsValidEmail(email);
                if (result == true)
                {
                    string messageStr = "Signup OTP: " + verificationCode + " - use this verification code to complete signup.";

                    new Thread(delegate ()
                    {
                        SendEmail(email, messageStr);

                    }).Start();
                }

            }
            catch (Exception e)
            {

            }


            return true;
        }
        public static bool SendPublicUserOTP(string Email, string Verification_Code)
        {
            try
            {
                string messageStr = "Signup OTP: " + Verification_Code + " - use this verification code to complete signup.";

                // Start Send Email Code
                return SendEmail(Email, messageStr);
                // End send email code
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool SendEmail(string ToEmail, string Message)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 25;
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential("mm.zeeshanmirza@gmail.com", "xxxxxxxxx");
                //smtp.Host = "10.50.16.87";//"webmail.xxxxxxx.com";
                smtp.Host = "103.226.216.248";

                MailMessage mail = new MailMessage();
                mail.From = new System.Net.Mail.MailAddress("crm@pitb.gov.pk");
                //mail.To.Add("mm.zeeshanmirza@gmail.com".Trim());
                mail.To.Add(ToEmail);

                mail.IsBodyHtml = true;
                mail.Subject = "Verification code";
                mail.Body = Message;//"this is my test email body";
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
            if (listDbUser.Count == 0)
            {
                listDbUser = new List<DbUsers>() { DbUsers.GetActiveUser(userId) };
            }
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            VmRole vmRole = new VmRole(listDbUser, userCookie.UserId);


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
                return new ClientMessage("Current password is incorrect", false);
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

        public static ClientMessage UpdatePublicUserProfile(VmUserSettings userSettings)
        {
            CMSCookie userCookie = AuthenticationHandler.GetCookie();
            //Reading user Id from Cookies 
            var opResult = DbUsers.UpdatePublicUserProfile(
                    userCookie.UserId,
                    userSettings.Name,
                    userSettings.Phone);
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