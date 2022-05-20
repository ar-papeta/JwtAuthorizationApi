using BLL.Services;
using BLL.Services.Interfaces;
using BLL.Services.PasswordHash;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        return services;
    }
}
