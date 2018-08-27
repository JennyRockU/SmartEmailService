using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;

namespace SendEmailApi
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        // called when a authorization requested
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var authorizationHeader = actionContext.Request.Headers.Authorization;
            if (authorizationHeader == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }
            
            // get the token from the header 
            var tokenString = authorizationHeader.Scheme;
         
            // validate the provided token 
            if (!Authentication.VaidateUser(tokenString))
            {
                // set unauthorized error response  
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

        }
    }
}