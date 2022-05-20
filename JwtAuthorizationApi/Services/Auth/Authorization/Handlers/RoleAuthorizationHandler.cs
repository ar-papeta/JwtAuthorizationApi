using JwtAuthorizationApi.Services.Auth.Authorization.Requirements;
using JwtAuthorizationApi.Services.Extentions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JwtAuthorizationApi.Services.Auth.Authorization.Handlers;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequiment>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _contextAccessor;

    public RoleAuthorizationHandler(IConfiguration configuration, IHttpContextAccessor contextAccessor)
    {
        _configuration = configuration;
        _contextAccessor = contextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequiment requirement)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)!.Value;
            var rolePermissions = _configuration.GetJwtPermissionsForRole(role);

            if (rolePermissions.Contains(requirement.Permission))
            {
                if (requirement.Permission == "user:write")
                {
                    if (IsUserEditAuthorized(context))
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
        }
        return Task.CompletedTask;
    }

    private bool IsUserEditAuthorized(AuthorizationHandlerContext context)
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "Admin")
        {
            return true;
        }
        var routeData = _contextAccessor.HttpContext!.GetRouteData();
        if (routeData.Values.TryGetValue("id", out object? identifierValue))
        {
            return identifierValue?.ToString() == context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        return false; 
    }
}
