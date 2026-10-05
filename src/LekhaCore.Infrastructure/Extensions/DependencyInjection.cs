using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LekhaCore.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddLekhaCoreAuthentication(configuration);
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IEffectivePermissionSource, EfEffectivePermissionSource>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRbacAdminService, RbacAdminService>();

        return services;
    }
}
