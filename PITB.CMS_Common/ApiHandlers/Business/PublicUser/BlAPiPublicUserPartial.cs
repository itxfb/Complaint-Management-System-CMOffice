using Newtonsoft.Json;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Modules.Api.Response;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace PITB.CMS_Common.ApiHandlers.Business.PublicUser
{
    public partial class BlApiPublicUser
    {
        //public static dynamic PublicUserSignupOTP(dynamic dParam)
        //{
        //    dynamic dResp = new ExpandoObject();

        //    dynamic formData = new ExpandoObject();
        //    formData.src = "mobile";


        //    formData.cnic = (string)GetFieldValue(dParam.data.fields, "userCnic").Replace("-", "");
        //    formData.email = (string)GetFieldValue(dParam.data.fields, "userEmail");
        //    dynamic result = BlPublicUser.SendOtp(formData);
        //    if (result.code == 1)
        //    {
        //        //dResp.OtpSent = true;
        //        dResp.status = true;
        //        dResp.message = "otp sent successfully";
        //    }
        //    else if (result.code == -1)
        //    {
        //        //dResp.OtpSent = false;
        //        dResp.status = false;
        //        dResp.message = "otp sending failed";
        //    }
        //    else if (result.code == 2)
        //    {
        //        //dResp.OtpSent = false;
        //        dResp.status = false;
        //        dResp.message = result.data;//== "email exists";
        //    }


        //    return dResp;
        //}



        public static dynamic PublicSignupFormPost(dynamic dParam)
        {
            dynamic dResp = new ExpandoObject();

            dynamic formData = new ExpandoObject();
            formData.src = "mobile";

            formData.OTP = (string)GetFieldValue(dParam.data.fields, "userOTP");

            if (!string.IsNullOrEmpty(formData.OTP))
            {
                formData.UserName = (string)GetFieldValue(dParam.data.fields, "userName");
                formData.UserProvince = GetFieldValue(dParam.data.fields, "userProvince");
                formData.UserDistrict = GetFieldValue(dParam.data.fields, "userDistrict");
                formData.UserCnic = GetFieldValue(dParam.data.fields, "userCnic").Replace("-", "");
                formData.UserMobileNo = GetFieldValue(dParam.data.fields, "userMobileNo").Replace("-", "");
                formData.UserEmail = (string)GetFieldValue(dParam.data.fields, "userEmail");
                formData.UserGender = GetFieldValue(dParam.data.fields, "userGender");
                formData.UserPassword = (string)GetFieldValue(dParam.data.fields, "userPassword");
                formData.UserAddress = (string)GetFieldValue(dParam.data.fields, "userAddress");
                dResp = BlPublicUser.SavePublicUser(formData);
            }
            else
            {

                formData.cnic = (string)GetFieldValue(dParam.data.fields, "userCnic").Replace("-", "");
                formData.email = (string)GetFieldValue(dParam.data.fields, "userEmail");
                dynamic result = BlPublicUser.SendOtp(formData);
                if (result.code == 1)
                {
                    dResp.status = true;
                    dResp.message = "otp sent successfully";
                }
                else if (result.code == -1)
                {
                    dResp.status = false;
                    dResp.message = "otp sending failed";
                }
                else if (result.code == 2)
                {
                    dResp.status = false;
                    dResp.message = result.data;
                }
                else if (result.code == 3)
                {
                    dResp.status = true;
                    dResp.message = result.data;
                }
            }



            return dResp;
        }

        public static dynamic ChangeComplaintTimeandCategory(dynamic dParam)
        {
            dynamic dResp = new ExpandoObject();
            dynamic formData = new ExpandoObject();
            formData.src = "mobile";



            formData.userToken = (string)dParam.data.userToken;
            DbUsers dbUser = DbUsers.GetActiveUser(DbUsers.GetUserIdFromHashString(dParam.data.userToken));
            formData.userId = dbUser.User_Id;
            formData.complaintId = Convert.ToInt32((string)GetFieldValue(dParam.data.fields, "complaintId").Split('-')[1]);
            formData.categoryId = Convert.ToInt32(GetFieldValue(dParam.data.fields, "complaintCategory"));
            formData.subcategoryId = Convert.ToInt32(GetFieldValue(dParam.data.fields, "complaintSubCategory"));
            string complaintTime = (string)GetFieldValue(dParam.data.fields, "newTime");
            formData.complaintTime = float.Parse(complaintTime == "" ? "0" : complaintTime);



            dynamic saveResp = null;

            BlComplaints.ChangeComplaintTiming(formData);
            saveResp = BlComplaints.ChangeComplaintCategory(formData);
            //dResp.isCategoryChanged = true;
            dResp.status = true;
            dResp.message = saveResp.message;

            return dResp;
        }
    }
}