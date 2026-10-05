using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return TableNoTracking.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByEmailTrackedAsync(string email, CancellationToken cancellationToken = default)
    {
        return Table.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Table.FirstOrDefaultAsync(user => user.GUID == id, cancellationToken);
    }

    public Task<User?> GetByPublicIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return TableNoTracking.FirstOrDefaultAsync(user => user.GUID == id, cancellationToken);
    }
}
