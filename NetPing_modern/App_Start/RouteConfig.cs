using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NetPing
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute("about_aspx", "about.aspx", new { controller = "InnerPages", action = "About" });

            routes.MapRoute("contacts_aspx", "contacts.aspx", new { controller = "InnerPages", action = "Contacts" });

            routes.MapRoute("dealers_aspx", "dealer.aspx", new { controller = "InnerPages", action = "Dealers" });

            routes.MapRoute("zakaz_aspx", "zakaz.aspx", new { controller = "InnerPages", action = "Question" });

            routes.MapRoute("cache_updated", "cache_updated", new { controller = "InnerPages", action = "UCache" });

            routes.MapRoute("default_aspx", "default.aspx", new { controller = "MainPage", action = "Index" });

            routes.MapRoute("catalog_aspx", "catalog.aspx", new { controller = "Catalog", action = "Index" });

            routes.MapRoute("product_item_aspx", "product_item.aspx", new { controller = "Product_item", action = "Index" });

            routes.MapRoute(
               name: "ASPX",
               url: "{controller}.aspx/{id}",
               defaults: new { action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Blog",
                url: "Blog/{action}",
                defaults: new { controller = "Blog", action = "Main" }
            );

            routes.MapRoute(
                name: "BlogArticle",
                url: "Blog/{postname}",
                defaults: new { controller = "Blog", action = "Record" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "MainPage", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}