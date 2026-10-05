using LekhaCore.Application.Interfaces;
using LekhaCore.Domain.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LekhaCore.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=LekhaCore;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options, new DesignTimeWorkContext());
    }

    private sealed class DesignTimeWorkContext : IWorkContext
    {
        public string CurrentUserId => AuditActors.System;

        public string? IpAddress => null;

        public string? CorrelationId => null;

        public void SetCurrentUser(Guid userId)
        {
        }
    }
}
