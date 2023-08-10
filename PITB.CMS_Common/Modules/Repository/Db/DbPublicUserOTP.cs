using PITB.CMS_Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models
{
    public partial class DbPublicUserOTP
    {

        public static DbPublicUserOTP GetUserOtp(string Email)
        {
            DBContextHelperLinq db = new DBContextHelperLinq();
            return db.DbPublicUserOTP.Where(w => w.Email == Email).OrderByDescending(c => c.CreatedDate).FirstOrDefault();
        }

        public static bool SavePublicUserOTP(string Email, string OTP)
        {
            try
            {
                {
                    DBContextHelperLinq db = new DBContextHelperLinq();
                    //var otp = OTPHelper.GenerateRandomOTP(10);

                    var otpModel = new DbPublicUserOTP
                    {
                        Email = Email,
                        OTP = OTP,
                        CreatedDate = DateTime.Now,
                        IsVerified = false,
                    };

                    db.DbPublicUserOTP.Add(otpModel);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
