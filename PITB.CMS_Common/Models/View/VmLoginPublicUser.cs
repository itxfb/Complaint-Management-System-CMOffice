using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Common.Models.View
{
    public class VmLoginPublicUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LayoutImageUrl { get; set; }
        public string LayoutPopupImageUrl { get; set; }

    }
}