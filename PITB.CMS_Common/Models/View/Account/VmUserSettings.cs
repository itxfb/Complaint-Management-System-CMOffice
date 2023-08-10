using PITB.CMS_Common.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Common.Models.View.Account
{
    public class VmUserSettings
    {
        public string Username { get; set; }

        private string _name;
        [Required(ErrorMessage = "Employee Address is required")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only")]
        public string Name
        {
            get { return (string.IsNullOrEmpty(_name) ? string.Empty : _name.Trim()); }
            set { _name = value; }
        }

        private string _phone;
        [Required]
        //[RegularExpression("^(\\d{11})+([,]\\d{11})*$", ErrorMessage = "Only numbers and commas allowed")]
        [RegularExpression("^\\d{11}(,\\d{11})*$", ErrorMessage = "Invalid number, Multiple numbers with comma separator are allowed")]
        public string Phone
        {
            get { return (string.IsNullOrEmpty(_phone)) ? _phone : _phone.Trim(); }
            set { _phone = value; }
        }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string CNIC { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastNamePlaceholder
        {
            get 
            { 
                char asterik = '*';
                return LastName.Substring(0, 1).PadRight(LastName.Length, asterik); 
            }
        }
        public string RoleName
        {
            get
            {
                return (Role == Config.Roles.Stakeholder)
                     ? ((Hierarchy.HasValue) ? Hierarchy.Value.GetDisplayName() : Role.GetDisplayName())
                     : Role.GetDisplayName();
            }
        }

        public Config.Roles Role { get; set; }
        public Config.Hierarchy? Hierarchy { get; set; }
        public DateTime? PasswordUpdateDate { get; set; }
        public List<string> CampaignName { get; set; }
        public string Address { get; set; }
        public string Verification_Code { get; set; }
        public VmChangePassword VmChangePassword { get; set; }

        public VmUserSettings()
        {
            VmChangePassword = new VmChangePassword();
        }
    }
}