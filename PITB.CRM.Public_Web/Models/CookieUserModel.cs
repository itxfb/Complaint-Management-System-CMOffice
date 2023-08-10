using PITB.CRM.Public_Web.Models.ViewModels;

namespace PITB.CRM.Public_Web.Models
{
    public class CookieUserModel
    {
        public CookieUserModel()
        {
            Data= new SocialShareData();
        }
        public SocialShareData Data { get; set; }
    }
}