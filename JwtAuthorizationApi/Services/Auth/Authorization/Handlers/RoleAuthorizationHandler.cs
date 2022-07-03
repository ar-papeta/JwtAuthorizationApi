using BLL.Services;
using JwtAuthorizationApi.Services.Auth.Authorization.Requirements;
using JwtAuthorizationApi.Services.Extentions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JwtAuthorizationApi.Services.Auth.Authorization.Handlers;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequiment>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly SensorsService _sensorService;

    public RoleAuthorizationHandler(IConfiguration configuration, IHttpContextAccessor contextAccessor, SensorsService sensorsService)
    {
        _configuration = configuration;
        _contextAccessor = contextAccessor;
        _sensorService = sensorsService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequiment requirement)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)!.Value;
            var rolePermissions = _configuration.GetJwtPermissionsForRole(role);

            if (rolePermissions.Contains(requirement.Permission))
            {
                if (requirement.Permission == "user:self")
                {
                    if (IsUserEditAuthorized(context))
                    {
                        context.Succeed(requirement);
                    }
                } 
                else if(requirement.Permission == "sensor:read")
                {
                    if (IsSensorReadAuthorized(context))
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
        if (routeData.Values.TryGetValue("userId", out object? identifierValue))
        {
            return identifierValue?.ToString() == context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        return false; 
    }
    private bool IsSensorReadAuthorized(AuthorizationHandlerContext context)
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Admin")
        {
            return true;
        }
        var routeData = _contextAccessor.HttpContext!.GetRouteData();

        if (routeData.Values.TryGetValue("sensorId", out object? routeId))
        {
            var sensor = _sensorService.GetSensorById(routeId.ToString());

            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == sensor?.UserId;
        }
        else if (routeData.Values.TryGetValue("userId", out object? routeUserId))
        {
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == routeUserId?.ToString();
        }

        return false;
    }
}
