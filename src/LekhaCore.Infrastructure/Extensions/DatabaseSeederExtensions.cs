using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LekhaCore.Infrastructure.Extensions;

public static class DatabaseSeederExtensions
{
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<AppDbContext>>();
        var context = services.GetRequiredService<AppDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        try
        {
            await context.Database.MigrateAsync();
            await DatabaseSeeder.SeedAsync(context, passwordHasher);
            logger.LogInformation("Database seed completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database seed failed.");
            throw;
        }
    }
}
