using System.Web.Mvc;
using System.Web.Routing;

namespace PITB.CRM.Public_Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "public", action = "index", id = UrlParameter.Optional },
                namespaces:new [] {"PITB.CRM.Public_Web.Controllers"}
            );
        }
    }
}
