using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Authentication
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "You are not authorized !");
            }
            else
            {
                // Gets key from header parameters  
                string key = actionContext.Request.Headers.Authorization.Parameter;

                // Validate key 
                if (!ApiSecurity.VaidateUser(key))
                {
                    // returns unauthorized error  
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "You are not authorized !"); ;
                }
            }

            base.OnAuthorization(actionContext);
        }
    }
}