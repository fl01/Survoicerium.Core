using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Survoicerium.Core;

namespace Survoicerium.GameApi.Authorization
{
    public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        public const string ApiKeyHeader = "X-ApiKey";

        private readonly IApiUserService _userService;

        public ApiKeyHandler(IApiUserService userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvc
                && mvc.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out StringValues apiKeys))
            {
                string apiKey = apiKeys.FirstOrDefault();
                if (_userService.IsValidApiKey(apiKey))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
