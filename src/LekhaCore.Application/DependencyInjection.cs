using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LekhaCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMenuService, MenuService>();

        return services;
    }
}
