using System.ComponentModel.DataAnnotations;
using Foolproof;
using PITB.CMS_Common.Helper.Validation;

namespace PITB.CMS_Handlers.View
{
    public class VmPersonalInfo
    {
       // [Key]
        public int? Person_id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        [StringLength(100)]
        [RegularExpression("^[a-zA-Z ]*$",ErrorMessage="Only text allowed in {0} field")]
        public string Person_Name { get; set; }

        [Display(Name = "Father")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Only text allowed in {0} field")]
        public string Person_Father_Name { get; set; }

        
        //[Required(AllowEmptyStrings = false)]

        //[Required(AllowEmptyStrings = false)]
        //[RequiredIfTrue("IsCnicPresent2", ErrorMessage = "Required")]
        //[RequiredIfTrue("IsCnicPresent2", ErrorMessage = "Required")]
        [Display(Name = "CNIC No")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "Provide complete {0}")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0} field")]
        public string Cnic_No { get; set; }
       // [Required]

       // public bool IsCnicPresent2 { get; set; }

        //---- test for custom validation

        //public bool IsEmail { get; set; }
        //[Display(Name = "SMS")]
        //[MyCustomValidation("IsEmail,IsAlert")]
        //public bool IsSMS { get; set; }
        //[Display(Name = "Alert")]
        //public bool IsAlert { get; set; }
        //---- end test for custom validation


        public bool IsCnicPresent { get; set; }

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
        [Display(Name = "Address")]
        public string Person_Address { get; set; }

        [StringLength(30)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Nearest_Place { get; set; }
        [Required(ErrorMessage = "The {0} field is required")]
        [Display(Name = "Province")]
        public int? Province_Id { get; set; }

        public int? Division_Id { get; set; }
        [Required(ErrorMessage = "The {0} field is required")]
        [Display(Name = "District")]
        public int? District_Id { get; set; }

        public string District_Name { get; set; }
        public int? Tehsil_Id { get; set; }

        public int? Town_Id { get; set; }

        public int? Uc_Id { get; set; }
        public virtual DbDistrict District { get; set; }
        public virtual DbProvince Province { get; set; }
    }
}