using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IAccessRequestRepository
{
    Task<AccessRequest?> GetByIdAsync(Guid id);

    IQueryable<AccessRequest> Query();

    Task<int> CountPendingAsync();

    Task AddAsync(AccessRequest accessRequest);

    Task UpdateAsync(AccessRequest accessRequest);

    Task DeleteAsync(AccessRequest accessRequest);

    Task SaveChangesAsync();
}
