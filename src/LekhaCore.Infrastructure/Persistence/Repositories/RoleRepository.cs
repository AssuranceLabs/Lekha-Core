using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class RoleRepository(AppDbContext context) : Repository<Role>(context), IRoleRepository
{
}
