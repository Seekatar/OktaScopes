using System;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly OktaSettings _okta;
        static string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        public ScopeHandler(ILogger<ScopeHandler> logger, IOptions<OktaSettings> settings)
        {
            _logger = logger;
            _okta = settings.Value;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                    ScopeRequirement requirement)
        {
            if (context.User.Claims.Any(c => c.Type == ScopeClaimType
                                             && c.Value == requirement.Scope
                                             && c.Issuer == _okta.Issuer
                                            ))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}