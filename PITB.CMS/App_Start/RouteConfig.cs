using System.Web.Mvc;
using System.Web.Routing;

namespace PITB.CMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
            /*
            routes.MapRoute(
                "Test",
                "SubFolder/Test",
                new { controller = "Test", action = "Index" }
            );
            */
            /*
            routes.MapRoute("", "{controller}/{action}/{orderId}/{fileName}",
                new { controller = "ShoppingCart", action = "PrintQuote" }
                , new string[] { "x.x.x.Controllers" }
            );
            
             */
         

        }
        /*
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SubFolderRoute",
                "SOProblems/SubFolder/ChildController",
                new { controller = "ChildController", action = "Index" },
                new[] { "Practise.Areas.SOProblems.Controllers.SubFolder" }
            );

            context.MapRoute(
                "SOProblems_default",
                "SOProblems/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }*/
    }
}