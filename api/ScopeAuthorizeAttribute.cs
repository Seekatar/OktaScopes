using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    class ScopeAuthorizeAttribute : AuthorizeAttribute
    {

    }
    class ScopeRequirement : IAuthorizationRequirement
    {
        public ScopeRequirement(string scope)
        {
            Scope = scope;
        }
        public string Scope { get; }
    }

    class ScopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        private readonly ILogger<ScopeHandler> _logger;

        public ScopeHandler(ILogger<ScopeHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                    ScopeRequirement requirement)
        {
            if (context.User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope"
                                             && c.Value == requirement.Scope
                                             // && c.Issuer == _okta.Issuer
                                            ))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}