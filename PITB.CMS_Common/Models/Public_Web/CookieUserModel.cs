using PITB.CMS_Common.Models.Public_Web.ViewModels;


namespace PITB.CMS_Common.Models.Public_Web
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