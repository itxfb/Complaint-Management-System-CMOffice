using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Handlers.View
{
    public class VmSearch
    {
        [StringLength(13, MinimumLength = 13, ErrorMessage = "Provide complete {0}")]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "CNIC No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0} field")]
        public string CnicNo { get; set; }
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Provide complete {0}")]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Cell No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed in in {0}")]
        public string CellNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:nn-nn}")]
        [Required(AllowEmptyStrings=false)]
        public string ComplaintNo { get; set; }
    }
}