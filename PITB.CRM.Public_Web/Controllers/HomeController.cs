
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using PITB.CRM.Public_Web.Contacts;
using PITB.CRM.Public_Web.Handler;
using PITB.CRM.Public_Web.Handler.Authentication;
using PITB.CRM.Public_Web.Models.Social;

namespace PITB.CRM.Public_Web.Controllers
{
    public class HomeController : Controller
    {
        //http://localhost:9838/Account/Login?ReturnUrl=%2Fhome%2Findex

        public ActionResult Index()
        {
            IAuthHandler authHandler = new FacebookAuthHandler();
            if (authHandler.IsAuthorized())
            {
                int h= new FacebookHandler().PostComment(null);
            }
            else
            {
                ViewBag.Data = "Not authorized";
            }

           // var authManager=this.HttpContext.GetOwinContext().Authentication;

           //  var authResult = await authManager.AuthenticateAsync("ExternalCookie");
           // if (!authResult.Identity.IsAuthenticated)
           // { }

           // var a=authResult.Identity.FindFirst(m => m.Value == "FacebookAccessToken");
           // var externalIdentity = authResult.Identity;
           // var accessToken = externalIdentity.FindFirst("FacebookAccessToken").Value;
           // string a1=string.Empty;
           // foreach (var claim in externalIdentity.Claims)
           // {
           //     a1 += string.Format("{0},", claim.ToString());
           // }
           // ViewBag.Data = a1;
           //dynamic userInfo = new FacebookClient(accessToken).Get("/me?fields=email,first_name,last_name");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}