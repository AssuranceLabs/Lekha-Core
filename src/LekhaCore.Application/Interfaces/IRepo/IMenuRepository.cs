using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IMenuRepository : IRepository<Menu>
{
    Task<IReadOnlyList<Menu>> ListActiveAsync(CancellationToken cancellationToken = default);
}
