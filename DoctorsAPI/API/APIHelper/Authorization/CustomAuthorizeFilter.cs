using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Seagull.API.APIHelper;

namespace API.Authorization
{
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public AuthorizationPolicy Policy { get; }

        public CustomAuthorizeFilter(AuthorizationPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string[] allowController = new string[] { "Auth" , "Listening", "PublicApi", "ShowsApi", "OrderApi", "LookUps" };

            string controllerName = context.RouteData.Values["controller"].ToString();

            if (allowController.Contains(controllerName))
                return;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(Policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(Policy, authenticateResult, context.HttpContext, context);

            if (authorizeResult.Challenged)
            {
                // Return custom 401 result
                APIJsonResult data = new APIJsonResult
                {
                    Access = false,
                    success = false,
                    Msg = new List<string> { "SessionTimeOut" },
                    url = "/Home/Index",
                    data = null,
                    token = string.Empty
                };
                context.Result = new JsonResult(data);
                //new CustomUnauthorizedResult("Authorization failed.");
            }
            else if (authorizeResult.Forbidden)
            {
                // Return default 403 result
                context.Result = new ForbidResult(Policy.AuthenticationSchemes.ToArray());
            }
        }
    }
}
