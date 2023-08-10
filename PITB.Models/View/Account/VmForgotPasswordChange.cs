using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Models.View.Account
{
    public class VmForgotPasswordChange
    {
        [Required(ErrorMessage="Username is required")]
        [DataType(DataType.Text)]
        [StringLength(100)]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Password must be at least 8 characters.")]
        [Display(Name="Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        //"A secure password is at least 8 characters long, contains at least one upper, one lower case, one special character and one number."
    }
}