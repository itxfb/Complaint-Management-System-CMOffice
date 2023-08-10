using System.ComponentModel.DataAnnotations;

namespace PITB.CRM_API.Models.API
{
    public partial class PostOnSocialSiteRequestModel
    {
        [Required]
        public int? ComplaintId { get; set; }
        [Required]

        public string UserAccessToken { get; set; }
        [Required]

        public string UserProvider { get; set; }


    }


}