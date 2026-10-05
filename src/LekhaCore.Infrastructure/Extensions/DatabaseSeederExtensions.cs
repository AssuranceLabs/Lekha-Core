using LekhaCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        var configuration = services.GetRequiredService<IConfiguration>();

        try
        {
            await context.Database.MigrateAsync();
            await DatabaseSeeder.SeedAsync(context, configuration);
            logger.LogInformation("Database migration and seed completed.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database migration or seed failed.");
            throw;
        }
    }
}
