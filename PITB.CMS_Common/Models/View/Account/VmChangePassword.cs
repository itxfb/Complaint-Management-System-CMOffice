using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.Models.View.Account
{
    public class VmChangePassword
    {
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        //[NotEqualTo("Password",ErrorMessage ="Current and new Password may not be same")]
        public string OldPassword { get; set; }
        
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(16, ErrorMessage = "Must be between 6 and 16 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Display(Name = "Confirm Password")]
        [StringLength(16, ErrorMessage = "Must be between 6 and 16 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}