using System.ComponentModel.DataAnnotations;

namespace PITB.CMS_Common.ApiModels.API
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