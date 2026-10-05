using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Infrastructure.Authentication;
using LekhaCore.Infrastructure.EntityFramework;
using LekhaCore.Infrastructure.Persistence;
using LekhaCore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LekhaCore.Infrastructure.Extensions;

public static class PersistenceDependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddScoped<IWorkContext, WorkContext>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        return services;
    }
}
