using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SuperSportDataEngine.Application.WebApi.SystemApi.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(SuperSportDataEngine.Application.WebApi.SystemApi.Startup))]
namespace SuperSportDataEngine.Application.WebApi.SystemApi
{   
    /// <summary>
    /// OWIN Start Up Class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Accepts IAppBuilder which will be supplied at run-time
        /// *app* parameter is an interface which will be used to compose the application
        /// for the OWIN server
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        
        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}