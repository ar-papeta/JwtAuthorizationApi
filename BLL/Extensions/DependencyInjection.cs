using BLL.Services;
using BLL.Services.Cached;
using BLL.Services.Interfaces;
using BLL.Services.PasswordHash;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        services.AddScoped<SensorsService>();
        services.AddScoped<IPeriodDataService, PeriodDataService>();
        services.AddMemoryCache();
        services.AddScoped<SensorsDataService>();
        services.AddScoped<ISensorsDataService>(
            x => new SensorsDataServiceCached(x.GetRequiredService<SensorsDataService>(), x.GetRequiredService<IMemoryCache>()));
        
        return services;
    }
}
