using LekhaCore.Domain.Entities;
using LekhaCore.Domain.Enums;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(Guid id);

    Task<List<User>> GetByRoleAsync(Role role);
}
