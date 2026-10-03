using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class AuditLogRepository(AppDbContext context) : IAuditLogRepository
{
    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await context.AuditLogs
            .Include(x => x.PerformedByUser)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await context.AuditLogs.AddAsync(auditLog);
    }

    public async Task<int> CountAsync()
    {
        return await context.AuditLogs.CountAsync(x => !x.IsDeleted);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
