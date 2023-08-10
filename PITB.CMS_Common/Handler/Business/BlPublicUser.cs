using Newtonsoft.Json;
using PITB.CMS_Common.Handler.Authentication;
using PITB.CMS_Common.Handler.CustomJsonConverter;
using PITB.CMS_Common.Handler.DataTableJquery;
using PITB.CMS_Common.Handler.Tag;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom;
using PITB.CMS_Common.Models.Custom.DataTable;
using PITB.CMS_Common.Models.View.PublicUser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace PITB.CMS_Common.Handler.Business
{
    public class BlPublicUser
    {

        public static dynamic SendOtp(dynamic dParam)
        {
            dynamic dResp = new ExpandoObject();
            string Email = string.IsNullOrEmpty(dParam.email) ? dParam.cnic : dParam.email;
            string Cnic = dParam.cnic;
            string Phone = dParam.phone;
            string src = dParam.src;
            bool emailExists = BlPublicUser.CheckUserExistsWithEmail(Email);
            bool cnicExists = BlPublicUser.CheckUserExistsWithCnic(Cnic);
            var userOtp = BlPublicUser.GetUserOtp(Email);


            if (userOtp == null || userOtp.CreatedDate.AddMinutes(3).Ticks < DateTime.Now.Ticks)
            {
                var result = AuthenticationHandler.IsValidEmail(dParam.email);
                if (result == true && emailExists==true)
                {
                        dResp.data = "email exists";
                        dResp.code = 2;
                }
                else
                {
                    //if (!emailExists && !cnicExists)
                    if (!cnicExists)
                    {
                        var otp = BlPublicUser.GeneratePublicUserOTP();
                        bool isOtpSent = BlUsers.SendPublicUserOtpOnMobileAndEmail(Email, Phone, otp);
                        if (isOtpSent == true)
                        {
                            bool isOtpSaved = BlPublicUser.SavePublicUserOTP(Email, otp);

                            dResp.data = isOtpSaved;
                            dResp.code = isOtpSaved ? 1 : -1;
                        }
                        else
                        {
                            dResp.data = isOtpSent;
                            dResp.code = -1;
                        }
                    }
                    else
                    {
                        //if (emailExists)
                        //{
                        //    dResp.data = "email exists";
                        //    dResp.code = 2;
                        //}
                        if (cnicExists)
                        {
                            dResp.data = "cnic exists";
                            dResp.code = 2;
                        }
                    }
                }
            }
            else
            {
                dResp.data = "otp already sent";
                dResp.code = 3;
            }
            return dResp;

        }

        public static dynamic SavePublicUser(dynamic dParam)
        {
            dynamic dResp = new ExpandoObject();

            string src = dParam.src;

            var otpModel = GetUserOtp(dParam.UserEmail);

            if (string.IsNullOrEmpty(dParam.OTP))
            {
                //dResp.isSignUpSuccessfull = false;
                dResp.status = false;
                dResp.message = "empty otp";
            }
            else if (otpModel == null || dParam.OTP.Trim() != otpModel.OTP.Trim())
            {
                //dResp.isSignUpSuccessfull = false;
                dResp.status = false;
                dResp.message = "invalid otp";
            }
            else if (otpModel.CreatedDate.AddMinutes(3).Ticks < DateTime.Now.Ticks)
            {
                //dResp.isSignUpSuccessfull = false;
                dResp.status = false;
                dResp.message = "expired otp";
            }
            else
            {
                if (CheckUserExistsWithEmail(dParam.UserEmail))
                {
                    //dResp.isSignUpSuccessfull = false;
                    dResp.status = false;
                    dResp.message = "email exists";
                }
                else if (CheckUserExistsWithCnic(dParam.UserCnic))
                {
                    //dResp.isSignUpSuccessfull = false;
                    dResp.status = false;
                    dResp.message = "cnic exists";
                }
                else
                {
                    bool result = SavePublicUserToDb(dParam);
                    if (result)
                    {
                        //dResp.isSignUpSuccessfull = true;
                        dResp.status = true;
                        dResp.message = "user saved";
                    }
                    else
                    {
                        //dResp.isSignUpSuccessfull = false;
                        dResp.status = false;
                        dResp.message = "error";
                    }
                }
            }
            return dResp;
        }

        public static DbPublicUserOTP GetUserOtp(string UserEmail)
        {
            return DbPublicUserOTP.GetUserOtp(UserEmail);
        }
        public static string GeneratePublicUserOTP()
        {
            return OTPHelper.GenerateRandomOTP(10);
        }

        public static bool SavePublicUserOTP(string Email, string OTP)
        {
            return DbPublicUserOTP.SavePublicUserOTP(Email, OTP);
        }
        public static bool CheckUserExistsWithEmail(string Email)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                bool isExistsInPersonInfo = db.DbPersonalInfo.Where(w => w.Email == Email).FirstOrDefault() == null ? false : true;
                bool isExistsInUser = db.DbUsers.Where(w => w.Username == Email).FirstOrDefault() == null ? false : true;

                return (isExistsInPersonInfo || isExistsInUser) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckUserExistsWithCnic(string Cnic)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                return db.DbPersonalInfo.Where(w => w.Cnic_No == Cnic).FirstOrDefault() == null ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool SavePublicUserToDb(dynamic model)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();

                Dictionary<string, object> param = new Dictionary<string, object>();

                param.Add("@UserName", ((object)model.UserName).ToDbObj());
                param.Add("@UserCnic", ((object)model.UserCnic).ToDbObj());
                param.Add("@UserMobileNo", ((object)model.UserMobileNo).ToDbObj());
                param.Add("@UserEmail", ((object)model.UserEmail).ToDbObj());
                param.Add("@UserPassword", ((object)model.UserPassword).ToDbObj());
                param.Add("@UserGender", ((object)model.UserGender).ToDbObj());
                param.Add("@UserProvince", ((object)model.UserProvince).ToDbObj());
                param.Add("@UserDistrict", ((object)model.UserDistrict).ToDbObj());
                param.Add("@UserAddress", ((object)model.UserAddress).ToDbObj());
                param.Add("@Campaign_Id", ((object)(byte)model.Campaign_Id).ToDbObj());
                param.Add("@Role_Id", ((object)(int)Config.Roles.PublicUser).ToDbObj());
                try
                {
                    param.Add("@Campaigns", string.Join(",", (string[])model.Campaigns).ToDbObj());
                }
                catch (Exception e)
                {
                    param.Add("@Campaigns", (object)(byte)model.Campaign_Id);
                }

                //param.Add("@Verification_code", "1110000111");


                DBHelper.CrudOperation("[PITB].[spSavePublicUser]", param);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static DbUsers GetUserAgainstUsernameAndPassword(string email, string password)
        {
            try
            {
                return DbUsers.GetUserAgainstUserNameAndPassword(email, password);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DbUsers GetUserAgainstEmailAndPassword(string email, string password)
        {
            try
            {
                var db = new DBContextHelperLinq();
                return db.DbUsers.Include(n => n.ListDbUserCategory).Where(n => n.Username == email && n.Password == password && n.IsActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static DataTable GetComplaintListings(DataTableParamsModel dtModel, string fromDate, string toDate, string campaign, int complaintType, string spType, int listingType)
        {
            List<string> prefixStrList = new List<string> { "complaints", "campaign", "personalInfo", "complaints", "complaintType", "Statuses" };
            DataTableHandler.ApplyColumnOrderPrefix(dtModel, prefixStrList);

            Dictionary<string, string> dictFilterQuery = new Dictionary<string, string>();
            dictFilterQuery.Add("complaints.Created_Date", "CONVERT(VARCHAR(10),complaints.Created_Date,120) Like '%_Value_%'");

            DataTableHandler.ApplyColumnFilters(dtModel, new List<string>() { "ComplaintNo" }, prefixStrList, dictFilterQuery);
            ListingParamsAgent paramsComplaintListing = SetPublicUserListingParams(dtModel, fromDate, toDate, campaign, (Config.ComplaintType)complaintType, spType, listingType);


            //return listOfComplaints;
            CMSCookie cookie = new AuthenticationHandler().CmsCookie;
            Dictionary<string, object> dictParam = new Dictionary<string, object>();
            dictParam.Add("@fromDate", Utility.GetDateTimeStr(fromDate));
            dictParam.Add("@toDate", Utility.GetDateTimeStr(toDate));
            dictParam.Add("@campaignIds", campaign);
            dictParam.Add("@createdBy", cookie.UserId);

            if (dtModel != null)
            {
                dictParam.Add("@StartRow", dtModel.Start);
                dictParam.Add("@EndRow", dtModel.End);
                dictParam.Add("@OrderByColumnName", dtModel.ListOrder[0].columnName);
                dictParam.Add("@OrderByDirection", dtModel.ListOrder[0].sortingDirectionStr);
                dictParam.Add("@WhereOfMultiSearch", dtModel.WhereOfMultiSearch);
            }

            string queryStr = null;
            //if (spType == "Export")
            //{
            //    queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
            //}
            string exportStr = string.Empty;
            if (spType == "Export")
            {
                exportStr = "_Export";
            }
            // If listing added by me


            if (cookie.Role == Config.Roles.PublicUser)
            {
                if (complaintType == (int)Config.ComplaintType.Complaint)
                {
                    queryStr = QueryHelper.GetFinalQuery("PublicUser_ComplaintsListing_Mine" + exportStr, Config.ConfigType.Query, dictParam);
                }
                else if (complaintType == (int)Config.ComplaintType.Suggestion)
                {
                    queryStr = QueryHelper.GetFinalQuery("PublicUser_ComplaintsListing_Suggestion" + exportStr, Config.ConfigType.Query, dictParam);
                }
                else if (complaintType == (int)Config.ComplaintType.Inquiry)
                {
                    queryStr = QueryHelper.GetFinalQuery("PublicUser_ComplaintsListing_Inquiry" + exportStr, Config.ConfigType.Query, dictParam);
                }
            }
            //string asd = AgentListingLogic.GetListingQuery(paramsComplaintListing);

            DataTable dt = DBHelper.GetDataTableByQueryString(queryStr, null);

            // New implementation of status mask
            List<DbStatus> listDbStatuses = DbStatus.GetAll();
            //dynamic dStatusMask = null;
            //List<int> listCampaigns = Utility.GetIntList(campaign);
            //string tagKey = string.Format("RoleId::{0}__CampaignId::{1}", (int)cookie.Role, Convert.ToInt32(99));
            //string tagId = string.Format("Config::MaskedStatuses__RoleId::{0}", (int)cookie.Role);

            //string tagValue = TagWiseBestMatchHandler.GetBestMatch(tagId, tagKey);
            //if (tagValue != null)
            //{
            //    dStatusMask = BlStatus.GetStatusMaskingModel(tagValue, listDbStatuses);
            //}

            string campaignPermission = TagWiseBestMatchHandler.GetBestMatch("Config::CampaignPermission", "CampaignId::{dbUser.Campaign_Id}");
            dynamic dCampPerm = JsonConvert.DeserializeObject(campaignPermission, typeof(ExpandoObject), new DynamicJsonConverter());
            //Dictionary<int, string> dictIconMap = BlStatus.GetStatusIconMappingDict(dCampPerm.statusIconMap);
            dynamic dStatusMask = BlStatus.GetStatusMaskingModel(dCampPerm.roleId_9.statusMask, listDbStatuses);
            //dynamic dStatusMask = null;

            Dictionary<string, string> dictStatusStrMap = dStatusMask.dictStatusStrMap;
            string statusName = null;
            foreach (DataRow row in dt.Rows)
            {
                statusName = row["Status"].ToString();
                if (dictStatusStrMap.ContainsKey(statusName))
                {
                    row["status"] = dictStatusStrMap[statusName];
                }

            }
            // end new implementation of status mask


            //foreach()
            //string queryStr = AgentListingLogic.GetListingQuery(paramsComplaintListing);
            return dt;
        }

        public static ListingParamsAgent SetPublicUserListingParams(DataTableParamsModel dtParams, string fromDate, string toDate, string campaign, Config.ComplaintType complaintType, string spType, int listingType)
        {
            string extraSelection = "";

            CMSCookie cookie = new AuthenticationHandler().CmsCookie;

            ListingParamsAgent paramsModel = new ListingParamsAgent();
            paramsModel.StartRow = dtParams.Start;
            paramsModel.EndRow = dtParams.End;
            paramsModel.OrderByColumnName = dtParams.ListOrder[0].columnName;
            paramsModel.OrderByDirection = dtParams.ListOrder[0].sortingDirectionStr;
            paramsModel.WhereOfMultiSearch = dtParams.WhereOfMultiSearch;
            DateTime fromDateRes;
            DateTime toDateRes;
            if (DateTime.TryParseExact(fromDate, new string[] { "d/M/yyyy", "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateRes))
            {
                paramsModel.From = fromDateRes.ToString("MM/dd/yyyy");
            }
            else
            {
                paramsModel.From = fromDate;
            }
            if (DateTime.TryParseExact(toDate, new string[] { "d/M/yyyy", "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out toDateRes))
            {
                paramsModel.To = toDateRes.ToString("MM/dd/yyyy");
            }
            else
            {
                paramsModel.To = toDate;
            }
            paramsModel.Campaign = campaign;
            paramsModel.RoleId = (int)cookie.Role;
            paramsModel.ListingType = listingType;
            paramsModel.ComplaintType = (Convert.ToInt32(complaintType));
            paramsModel.UserId = cookie.UserId;
            paramsModel.SpType = spType;
            return paramsModel;
        }
    }
}
