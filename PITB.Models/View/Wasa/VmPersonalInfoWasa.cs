using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITB.CMS_Models.View.Wasa
{
    public class VmPersonalInfoWasa 
    {
        public int? Person_id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        [StringLength(100)]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only text allowed in {0} field")]
        public string Person_Name { get; set; }

        [Display(Name = "Father/Husband Name")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only text allowed in {0} field")]
        public string Person_Father_Name { get; set; }

        //[StringLength(13, MinimumLength = 13, ErrorMessage = "Provide complete {0}")]
        //[Required(AllowEmptyStrings = false)]
        //[Display(Name = "CNIC No")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0} field")]
        //public string Cnic_No { get; set; }
        // [Required]
        [Display(Name = "Gender")]
        public Config.Gender? Gender { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Provide complete {0}")]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Contact No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0}")]
        public string Mobile_No { get; set; }

        [StringLength(15)]
        [Display(Name = "Secondary contact")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in {0}")]
        public string Secondary_Mobile_No { get; set; }

        [StringLength(10)]
        public string LandLine_No { get; set; }

        [StringLength(250)]
        public string Person_Address { get; set; }

        [StringLength(30)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Nearest_Place { get; set; }
        [Required(ErrorMessage = "The {0} field is required")]
        [Display(Name = "Province")]
        public int? Province_Id { get; set; }

        public int? Division_Id { get; set; }
        //[Required(ErrorMessage = "The {0} field is required")]
        //[Display(Name = "District")]
        public int? District_Id { get; set; }

        public string District_Name { get; set; }
        public int? Tehsil_Id { get; set; }

        public int? Town_Id { get; set; }

        public int? Uc_Id { get; set; }
        public virtual DbDistrict District { get; set; }
        public virtual DbProvince Province { get; set; }




        [StringLength(8, MinimumLength = 8, ErrorMessage = "Provide complete {0}")]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Account No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0} field")]
        public string Account_No { get; set; }
    }
}