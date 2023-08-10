using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_Common.Models.View.PublicUser
{
    public class VmSignUp
    {
        [Required]
        [RegularExpression("^([a-zA-Z ,.'/-]{1,20}$)")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^\d{13}$")]
        public string UserCnic { get; set; }

        [Required]
        [RegularExpression(@"^\d{11}$")]
        public string UserMobileNo { get; set; }

        [Required]
        [RegularExpression(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$")]
        public string UserEmail { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z!@('@')#$%^&*_\d]{6,20}$")]
        public string UserPassword { get; set; }

        [Required]
        [Compare("UserPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int UserGender { get; set; }
        [Required]
        public int UserProvince { get; set; }

        [Required]
        public int UserDistrict { get; set; }

        [Required]
        [RegularExpression(@"([a-zA-Z0-9'\.\-\s\,\#\/\w]{15,200})$")]
        public string UserAddress { get; set; }

        public string OTP { get; set; }
    }
}
