using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Core.Domain.Common;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class AuditLogRepository(AppDbContext context) : Repository<AuditLog>(context), IAuditLogRepository
{
    public override Task<List<AuditLog>> GetAllAsync()
    {
        return TableNoTracking
            .OrderByDescending(log => log.DateTime)
            .ToListAsync();
    }

    public Task<int> CountAsync()
    {
        return Table.CountAsync();
    }
}
