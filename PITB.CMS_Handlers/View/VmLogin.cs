using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Handlers.View
{
    public class VmLogin
    {
        [Required(ErrorMessage = "Please provide username")]
        [RegularExpression(@"^([a-zA-Z0-9._-]+)$", ErrorMessage = "Username cannot contain special chracter")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide password")]
        public string Password { get; set; }

        public string LayoutImageUrl { get; set; }

        public string LayoutPopupImageUrl { get; set; }

    }
}