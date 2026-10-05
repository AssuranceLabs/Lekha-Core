using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class MenuRepository(AppDbContext context) : Repository<Menu>(context), IMenuRepository
{
    public async Task<IReadOnlyList<Menu>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return await TableNoTracking
            .Where(menu => menu.IsActive)
            .ToListAsync(cancellationToken);
    }
}
