using LekhaCore.Core.Domain.Common;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync();

    Task AddAsync(AuditLog auditLog);

    Task<int> CountAsync();

    Task SaveChangesAsync();
}
