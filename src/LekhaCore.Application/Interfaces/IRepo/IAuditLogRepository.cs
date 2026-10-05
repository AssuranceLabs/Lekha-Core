using LekhaCore.Core.Domain.Common;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<int> CountAsync();
}
