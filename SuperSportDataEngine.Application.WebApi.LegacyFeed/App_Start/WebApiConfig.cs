using SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{sport}/{category}",
                defaults: new { category = RouteParameter.Optional, type = RouteParameter.Optional , id = RouteParameter.Optional } 
            );
            
            config.MessageHandlers.Add(new FeedRequestHandler());
        }
    }
}