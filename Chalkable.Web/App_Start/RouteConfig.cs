using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Chalkable.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", // Route name
                            "{controller}/{action}.{format}", // URL with parameters
                new { controller = "Home", action = "Index", format = "aspx" } // Parameter defaults
            );

            routes.MapRoute(
              "Default2", // Route name
              "{controller}/{action}", // URL with parameters
              new { controller = "Home", action = "Index" } // Parameter defaults
            );
        }
    }
}